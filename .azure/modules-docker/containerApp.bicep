@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Container Apps Parameters ---------------- */

@description('The Id Of The Container App Environment.')
param containerAppEnvironmentId string

@description('The Name Of The Container App.')
param containerAppName string

@description('The Docker Container Image To Deploy.')
param containerImage string

@description('The Docker Registry Server')
param registryServer string

@description('The Docker Registry User Name')
param registryUserName string

@description('The Password Of The Container Registry Account')
@secure()
param registryPassword string

@description('Revision Suffix')
param revisionSuffix string

@description('Does The Container Allows External Connections')
param external bool = true

@description('The Minimum Number Of The Container Replicas')
param minReplicas int = 1

@description('The Maximum Number Of The Container Replicas')
param maxReplicas int = 1

@description('The Target Port')
param targetPort int = 80

@description('The Path For The Health Check')
param healthCheckPath string = '/'

@description('The Environment Variables Of The Container App')
param env object = {}

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string = ''

/*----------------------- Redis Cache  Parameters -------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string = ''

/*----------------------- Variables  --------------------------- */

var registryPasswordRef = 'dockerhub-pwd'
var serviceBusDataOwnerResourceID = '090c5cfd-751d-490a-894a-3ce6f1109419'
var dataOwnerRoleAssignmentName= guid(serviceBusNamespaceName, resourceGroup().id, containerAppName)
var builtInRedisAccessPolicyName = 'Data Contributor'

var containerVariables = [ for key in objectKeys(env): {
  name: key
  value: env[key]
} ]

/*-----------------------  RESOURCES   ------------------------- */

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppEnvironmentId  
    configuration: {      
      ingress: {
        external: external
        targetPort: targetPort
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: [
        {
          passwordSecretRef: registryPasswordRef
          server: registryServer
          username: registryUserName
        }
      ]
      secrets: [
        {
          name: registryPasswordRef
          value: registryPassword
        }
      ]
    }
    template: {      
      revisionSuffix: revisionSuffix
      containers: [
        {          
          name: containerAppName
          image: containerImage          
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'             
          }
          env: containerVariables
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                path: healthCheckPath
                port: targetPort
              }
              periodSeconds: 30
              failureThreshold: 3
              initialDelaySeconds: 30
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}


/*------------   Role And Access Policy Assignment   ----------- */

resource redisCache  'Microsoft.Cache/redis@2024-03-01' existing = if(!empty(redisCacheName)) {
  name: empty(redisCacheName) ? 'placeholder' : redisCacheName
}

resource redisAccessPolicyAssignment 'Microsoft.Cache/redis/accessPolicyAssignments@2024-03-01' = if(!empty(redisCacheName)) {
  name: '${containerApp.name}assignment'
  parent: redisCache

  properties: {
    accessPolicyName: builtInRedisAccessPolicyName
    objectId: containerApp.identity.principalId
    objectIdAlias: containerApp.name
  }
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing =  if(!empty(serviceBusNamespaceName))  {
  name: empty(serviceBusNamespaceName) ? 'placeholder' : serviceBusNamespaceName
}

resource serviceBusRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if(!empty(serviceBusNamespaceName))  {
  name: dataOwnerRoleAssignmentName
  scope: serviceBusNamespace
  properties: {
    principalId: containerApp.identity.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', serviceBusDataOwnerResourceID)
    principalType: 'ServicePrincipal'
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn

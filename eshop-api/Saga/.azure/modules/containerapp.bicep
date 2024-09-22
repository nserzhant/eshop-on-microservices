@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Container Apps Parameters ---------------- */

@description('The Name Of The Container App.')
param containerAppName string

@description('The Name Of The Container App Environment.')
param containerAppEnvironmentName string

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

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string

/*----------------------- SQL Server  Parameters --------------- */

@description('The Name Of The Database Server')
param sqlServerName string

@description('The Name Of The Database')
param sqlDbName string

/*----------------------- Variables  --------------------------- */

var registryPasswordRef = 'dockerhub-pwd'
var serviceBusDataOwnerResourceID = '090c5cfd-751d-490a-894a-3ce6f1109419'
var databaseContributorRoleDefinitionResourceID = '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec'
var dataOwnerRoleAssignmentName= guid(serviceBusNamespaceName, resourceGroup().id, serviceBusDataOwnerResourceID)
var databaseContributorRoleAssignmentName= guid(sqlServerName, resourceGroup().id, databaseContributorRoleDefinitionResourceID)

/*-----------------------  RESOURCES   ------------------------- */

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing = {
  name: serviceBusNamespaceName
}

resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' existing = {
  name: sqlServerName
}

/*----------------------- Container App     -------------------- */

resource containerAppEnv 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: containerAppEnvironmentName
  location: location
  
  properties: {
  }
}

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {      
      ingress: {
        external: true
        targetPort: 8080
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
          env: [
            {
              name: 'initDbOnStartup'
              value: 'true'
            }
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
             name: 'ASPNETCORE_HTTP_PORTS'
             value: '8080'
            }
            {
              name: 'ConnectionStrings__sagaDbConnectionString'
              value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${sqlDbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";' 
            }
            {
             name: 'broker__azureServiceBusConnectionString'
             value: 'sb://${serviceBusNamespace.name}.servicebus.windows.net'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

resource serviceBusRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: dataOwnerRoleAssignmentName
  scope: serviceBusNamespace
  properties: {
    principalId: containerApp.identity.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', serviceBusDataOwnerResourceID)
    principalType: 'ServicePrincipal'
  }
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: sqlServer
  name: databaseContributorRoleAssignmentName
  properties: {
    principalId: containerApp.identity.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', databaseContributorRoleDefinitionResourceID)
    principalType: 'ServicePrincipal'
  }
}

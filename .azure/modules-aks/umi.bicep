@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- UMI Parameters  -------------------- */

@description('The Name Of The User Assigned Manage Identity Resource.')
param umiName string

@description('Names Of The AKS Service Account.')
param aksServiceAccountNames string[]

@description('The Name Of The Namespace Where Service Account Created.')
param aksNamespace string

@description('The Reference To The AKS OIDC Issuer.')
param aksOidcIssuer string

/*----------------------- Redis Cache  Parameters -------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string = ''

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string = ''

/*----------------------- Variables  --------------------------- */

var serviceBusDataOwnerResourceID = '090c5cfd-751d-490a-894a-3ce6f1109419'
var dataOwnerRoleAssignmentName= guid(serviceBusNamespaceName, resourceGroup().id, umiName)
var builtInRedisAccessPolicyName = 'Data Contributor'

/*----------------------- RESOURCES    --------------------------- */


resource umi 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: umiName
  location: location

  @batchSize(1)
  resource saFederatedCredentials 'federatedIdentityCredentials' = [for serviceAccountName in aksServiceAccountNames: {
      name: '${serviceAccountName}-fi'
      properties: {
        audiences: [ 'api://AzureADTokenExchange' ]
        issuer: aksOidcIssuer
        subject: 'system:serviceaccount:${aksNamespace}:${serviceAccountName}'
      }
    }
  ]
}

/*------------   Role And Access Policy Assignment   ----------- */

resource redisCache  'Microsoft.Cache/redis@2024-03-01' existing = if(!empty(redisCacheName)) {
  name: empty(redisCacheName) ? 'placeholder' : redisCacheName
}

resource redisAccessPolicyAssignment 'Microsoft.Cache/redis/accessPolicyAssignments@2024-03-01' = if(!empty(redisCacheName)) {
  name: '${umi.name}assignment'
  parent: redisCache

  properties: {
    accessPolicyName: builtInRedisAccessPolicyName
    objectId: umi.properties.principalId
    objectIdAlias: umi.name
  }
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing =  if(!empty(serviceBusNamespaceName))  {
  name: empty(serviceBusNamespaceName) ? 'placeholder' : serviceBusNamespaceName
}

resource serviceBusRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if(!empty(serviceBusNamespaceName))  {
  name: dataOwnerRoleAssignmentName
  scope: serviceBusNamespace
  properties: {
    principalId: umi.properties.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', serviceBusDataOwnerResourceID)
    principalType: 'ServicePrincipal'
  }
}

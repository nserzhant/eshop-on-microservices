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

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string

/*----------------------- Variables  --------------------------- */

var databaseContributorRoleDefinitionResourceID = '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec'
var roleAssignmentName= guid(umiName, resourceGroup().id, databaseContributorRoleDefinitionResourceID)

/*----------------------- RESOURCES    --------------------------- */


resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' existing = {
  name: sqlServerName
}

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

resource dbAccessRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: sqlServer
  name: roleAssignmentName
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', databaseContributorRoleDefinitionResourceID)
    principalId: umi.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

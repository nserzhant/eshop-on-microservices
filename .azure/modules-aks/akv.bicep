@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- AKV Parameters  -------------------- */

@description('The Name Of The Azure Key Vault Resource.')
param akvName string

@description('The Name Of The Client Authorization Server Signing Key Akv Secret.')
param clientAuthSigningKeySecretName string

@description('The Name Of The Employee Authorization Server Signing Key Akv Secret.')
param employeeAuthSigningKeySecretName string

@description('The Client Authorization Server Signing Key.')
@secure()
param clientAuthSigningKey string

@description('The Employee Authorization Server Signing Key')
@secure()
param employeeAuthSigningKey string

@description('The AKS Principal Id')
param akskeyValutProviderUMI string

/*----------------------- Variables  --------------------------- */

var keyVaultSecretsUserRoleDefinitionResourceId = '4633458b-17de-408a-b874-0445c86b69e6'
var aksRoleAssignmentName = guid(akvName, resourceGroup().id, keyVaultSecretsUserRoleDefinitionResourceId)

/*----------------------- RESOURCES    --------------------------- */

resource akv 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: akvName
  location: location

  properties: {
    sku: {
      family: 'A'
      name: 'standard' 
    }
    enableRbacAuthorization: true
    tenantId: tenant().tenantId
  }  

  resource clientSigningKeySecret 'secrets@2023-07-01' = {
    name: clientAuthSigningKeySecretName
    properties: {
      value: clientAuthSigningKey 
    }
  }

  resource employeeSigningKeySecret 'secrets@2023-07-01' = {
    name: employeeAuthSigningKeySecretName
    properties: {
      value: employeeAuthSigningKey 
    }
  }
}

resource aksRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: aksRoleAssignmentName
  scope: akv
  
  properties: {
    principalId: akskeyValutProviderUMI
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleDefinitionResourceId)
    principalType: 'ServicePrincipal'
  }
}

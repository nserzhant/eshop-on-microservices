@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters        --------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Name Of The Subnet.')
param subNetName string

/*-----------------------   Gateway Parameters         -------------------- */

@description('The Id Of The Web App Service Plan')
param webAppServicePlanId string

@description('The Name Of The Gateway App')
param gatewayAppName string

@description('The Reference To The Reverse Proxy Docker Image')
param gatewayAppLinuxVersion string = 'DOCKER|nginx:1.25.3'

/*----------------------- Storage Account Parameters    -------------------- */

@description('The Name Of The Storage Account')
param storageAccountName string

@description('The Name Of The Storage Account Container')
param storageAccountContainerName string

/*----------------------- Variables             --------------------------- */

var blobContributorRoleDefinitionResourceID = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
var roleAssignmentName= guid(gatewayAppName, resourceGroup().id)

/*----------------------- RESOURCES             --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' existing = {
  name: vnetName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2023-06-01' existing = {
  name: subNetName
  parent: virtualNetwork
}

/*-----------------------     Web App     --------------------------- */

resource gatewayApp 'Microsoft.Web/sites@2022-09-01' = {
  name: gatewayAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlanId
    siteConfig: {
      linuxFxVersion: gatewayAppLinuxVersion
    }  
    vnetRouteAllEnabled: true
    virtualNetworkSubnetId: subnet.id    
  }

  resource web 'config' = {
    name: 'web'

    properties: {
      httpLoggingEnabled: true
      logsDirectorySizeLimit: 100
      detailedErrorLoggingEnabled: true
    }
  }
}

/*----------------------- Role Assignment To Access Storage Account ---------------------- */


resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storageAccount
  name: roleAssignmentName
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', blobContributorRoleDefinitionResourceID)
    principalId: gatewayApp.identity.principalId
    principalType:'ServicePrincipal'
  }
}

resource mountConfiguration 'Microsoft.Web/sites/config@2022-09-01' = {
  name: 'azurestorageaccounts'
  parent: gatewayApp
  properties: {
    'config-mount':{
      type: 'AzureBlob'
      accountName: storageAccountName
      shareName: storageAccountContainerName
      mountPath: '/etc/nginx'
      accessKey: storageAccount.listKeys().keys[0].value
    }
  }

  dependsOn: [
    roleAssignment
  ]
}

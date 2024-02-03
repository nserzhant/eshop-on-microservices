@description('Location for all resources.')
param location string = resourceGroup().location

/*-----------------------  Gateway  Parameters          -------------------- */

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

/*-----------------------       RESOURCES        -------------------------- */

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

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
}

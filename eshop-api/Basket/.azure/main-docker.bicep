@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- VNet  Parameters        ---------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string = 'basket-api-vnet-${suffix}'

/*----------------------- Redis Cache  Parameters ---------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string = 'basket-rediscache-${suffix}'

@description('The Pricing Tier Of The Azure Redis Cache')
param redisCacheSKU string = 'Basic'

@description('The Family For The SKU. C = Basic/Standard, P = Premium')
param redisCacheFamily string = 'C'

@description('The Size Of The Azurere Redis Cache instance')
param redisCacheCapacity int = 0

@description('Specify a boolean value that indicates whether to allow access via non-SSL ports')
param enableNonSslPort bool = false

/*----------------------- Web App Parameters -------------------- */

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string = 'basket-api-splan-${suffix}'

@description('The Name Of The Web App SKU')
param webAppSku string = 'B1'

@description('The Name Of The Web App')
param webAppName string = 'basket-api-${suffix}'

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOCKER|mcr.microsoft.com/appsvc/staticsite:latest'

@description('The Name Of The Customer Authorization Server Web App')
param customerAuthServerWebAppName string = 'customer-authserver-web-app-${suffix}'
/*----------------------- RESOURCES  --------------------------- */

module vnet 'modules/vnet.bicep' = {
  name: 'vnet'
  params: {
    location: location
    vnetName: vnetName 
  }
}

module webApp 'modules/webapp.bicep' = {
  name: 'webapp'
  params: {
    location: location
    customerAuthServerWebAppName: customerAuthServerWebAppName
    redisCacheName: redisCacheName
    vnetName: vnetName
    webAppName: webAppName
    webAppServicePlanName: webAppServicePlanName
    webappLinuxVersion: webappLinuxVersion
    webAppSku: webAppSku
  }
  dependsOn: [
    vnet
  ]
}

module redis 'modules/redis.bicep' = {
  name: 'redis'
  params: {
    location: location
    redisCacheName: redisCacheName 
    redisCacheSKU: redisCacheSKU
    redisCacheFamily: redisCacheFamily
    redisCacheCapacity: redisCacheCapacity
    enableNonSslPort: enableNonSslPort
    vnetName: vnetName
    webAppName: webAppName
  }
  dependsOn: [
    vnet
    webApp
  ]
}

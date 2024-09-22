@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

/*----------------------- Web App Parameters -------------------- */

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string

@description('The Name Of The Web App SKU')
param webAppSku string = 'B1'

@description('The Name Of The Web App')
param webAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string

@description('The Name Of The Customer Authorization Server Web App')
param customerAuthServerWebAppName string

/*----------------------- Redis Cache  Parameters ---------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string

/*----------------------- RESOURCES  --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2024-01-01' existing = {
  name: vnetName
}

/*----------------------- Web App  --------------------------- */

resource webAppServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: webAppServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: webAppSku 
  }
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }

  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    }
    vnetRouteAllEnabled: true
    virtualNetworkSubnetId: virtualNetwork.properties.subnets[0].id    
  }
  
  resource configuration 'config' = {
    name: 'appsettings'

    properties: {
      ASPNETCORE_ENVIRONMENT: 'Development'   
      broker__rabbitMQHost: ''
      JWTSettings__Issuer: 'https://${customerAuthServerWebAppName}.azurewebsites.net'
      JWTSettings__MetadataAddress: 'https://${customerAuthServerWebAppName}.azurewebsites.net/.well-known/openid-configuration'    
    }
  }

  resource connectionStrings 'config' = {
    name: 'connectionstrings'

    properties: {
      entraIdRedisConnectionString: {
        type:  'Custom'
        value: '${redisCacheName}.redis.cache.windows.net:6380'
      }
    }
  }
}

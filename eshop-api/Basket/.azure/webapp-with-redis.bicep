@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

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
param webappLinuxVersion string = 'DOTNETCORE|8.0'

@description('The Name Of The Customer Authorization Server Web App')
param customerAuthServerWebAppName string = 'customer-authserver-web-app-${suffix}'

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- Azure Redis Cache -------------------- */

resource redisCache 'Microsoft.Cache/redis@2024-04-01-preview' = {
  name: redisCacheName
  location: location
  
  properties: {
    enableNonSslPort: enableNonSslPort
    
    sku: {
      name: redisCacheSKU
      capacity: redisCacheCapacity
      family: redisCacheFamily
    }
  }
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

  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    } 
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
      redisConnectionString: {
        type: 'Custom'
        value: '${redisCacheName}.redis.cache.windows.net:6380,password=${redisCache.listKeys().primaryKey},ssl=True,abortConnect=False'
      }
    }
  }
}

@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Redis Cache  Parameters ---------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string

@description('The Pricing Tier Of The Azure Redis Cache')
param redisCacheSKU string = 'Basic'

@description('The Family For The SKU. C = Basic/Standard, P = Premium')
param redisCacheFamily string = 'C'

@description('The Size Of The Azurere Redis Cache instance')
param redisCacheCapacity int = 0

@description('The Boolean Value That Indicates Whether To Allow Access Via Non-SSL Ports')
param enableNonSslPort bool = false

/*----------------------- RESOURCES  --------------------------- */

resource redisCache 'Microsoft.Cache/redis@2024-03-01' = {
  name: redisCacheName
  location: location
  
  properties: {
    enableNonSslPort: enableNonSslPort
    disableAccessKeyAuthentication: true   

    sku: {
      name: redisCacheSKU
      capacity: redisCacheCapacity
      family: redisCacheFamily
    }

    redisConfiguration: {
     'aad-enabled': 'true'
    }    
  }
}

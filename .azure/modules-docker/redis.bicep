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


/*------------------- Virtual Network Parameters  --------------- */

@description('The Id Of The Subnet')
param subNetId string

@description('The Name Of The Private Endpoint')
param privateEndpointName string = '${redisCacheName}-private-endpoint'

@description('The Name Of The Network Interface For The Private Endpoint')
param privateEndpointNICName string = '${redisCacheName}-nic'

/*----------------------- Variables   --------------------------- */

var privateDnsZoneName =  'privatelink.redis.cache.windows.net'

/*----------------------- RESOURCES  --------------------------- */

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: privateDnsZoneName
}

/*----------------------- Azure Redis Cache ------------------- */

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

/*-------------------    VNET Integration  ------------------*/

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2024-01-01' = {
  name: privateEndpointName
  location: location

  properties: {
    subnet: {
      id: subNetId
    }

    customNetworkInterfaceName: privateEndpointNICName

    privateLinkServiceConnections: [
      {
        name: privateEndpointNICName
        properties: {
          privateLinkServiceId: redisCache.id
          groupIds: [
            'redisCache'
          ]
        }
      }
    ]
  }

  resource privateEndpointDnsZoneGroup 'privateDnsZoneGroups' = {
    name: 'default'

    properties: {
      privateDnsZoneConfigs: [
        {
          name: privateDnsZoneName
          properties: {
            privateDnsZoneId: privateDnsZone.id
          }
        }
      ]
    }
  }
}

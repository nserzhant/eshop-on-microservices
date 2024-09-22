@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Redis Cache  Parameters ---------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string

@description('The Pricing Tier Of The Azure Redis Cache')
param redisCacheSKU string

@description('The Family For The SKU. C = Basic/Standard, P = Premium')
param redisCacheFamily string

@description('The Size Of The Azurere Redis Cache instance')
param redisCacheCapacity int

@description('Specify a boolean value that indicates whether to allow access via non-SSL ports')
param enableNonSslPort bool

/*----------------------- Web App Parameters -------------------- */

@description('The Name Of The Web App')
param webAppName string

/*------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

/*----------------------- Variables  --------------------------- */

var privateDnsZoneName =  'privatelink.redis.cache.windows.net'
var privateEndpointName = '${redisCacheName}-private-endpoint'
var privateEndpointNICName = '${privateEndpointName}-nic'
var builtInAccessPolicyName = 'Data Contributor'

/*----------------------- RESOURCES  --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2024-01-01' existing = {
  name: vnetName
}

resource webApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: webAppName
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

  resource accessPolicyAssignment 'accessPolicyAssignments' = {
    name: 'webapppolicyAssignment'

    properties: {
      accessPolicyName: builtInAccessPolicyName
      objectId: webApp.identity.principalId
      objectIdAlias: webApp.name
    }
  }
}

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: privateDnsZoneName
  location: 'global'

  resource virtualNetworkLink 'virtualNetworkLinks' = {
    name: uniqueString(virtualNetwork.id)
    location: 'global'

    properties: {
      registrationEnabled: false

      virtualNetwork: {
        id: virtualNetwork.id
      }
    }
  }
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2024-01-01' = {
  name: privateEndpointName
  location: location

  properties: {
    subnet: {
      id: virtualNetwork.properties.subnets[1].id
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

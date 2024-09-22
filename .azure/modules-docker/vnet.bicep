@description('Location for all resources.')
param location string = resourceGroup().location

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

/*-----------------------     Variables           ----------------------- */

var containerAppEnvironmentSubnetName = 'conainerAppEnvSubnet'
var privateEndpointsSubnetName = 'privateEndpointsSubnet'
var privateDnsZoneNames = [
  'privatelink${environment().suffixes.sqlServerHostname}'
  'privatelink.blob.${environment().suffixes.storage}'
  'privatelink.redis.cache.windows.net'
]

/*----------------------- RESOURCES           --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.1.0.0/16'
      ]
    }    

    subnets : [
      {
        name: containerAppEnvironmentSubnetName
        properties: {
          addressPrefix: '10.1.0.0/23'
        }
      }
      {
        name: privateEndpointsSubnetName
        properties: {
          addressPrefix: '10.1.2.0/24'
        }
      }
    ]    
  }
}

resource privateDNSZones 'Microsoft.Network/privateDnsZones@2020-06-01' = [ for privateDNSZoneName in privateDnsZoneNames: {
  name: privateDNSZoneName
  location: 'global'

  dependsOn: [
    virtualNetwork
  ]
}]

resource virtualNetworkLinks 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01'=[for privateDNSZoneIndex in range(0, length(privateDnsZoneNames)): {
  name: uniqueString(virtualNetwork.id)
  location: 'global'
  parent: privateDNSZones[privateDNSZoneIndex]
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: virtualNetwork.id
    }
  }
  dependsOn: [
    privateDNSZones
  ]
}]

output containerAppEnvSubnetId string = virtualNetwork.properties.subnets[0].id
output privateEndpointsSubnetId string = virtualNetwork.properties.subnets[1].id

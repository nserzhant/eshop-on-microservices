
type Subnet = {
  name: string
  delegation: string?
}

@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The List Of Subnets To Create.')
param subnets Subnet[]

/*----------------------- Variables           --------------------------- */

var privateDnsZoneNames = [
  'privatelink.azurewebsites.net'
  'privatelink.blob.${environment().suffixes.storage}'
  'privatelink${environment().suffixes.sqlServerHostname}'
]

/*----------------------- RESOURCES           --------------------------- */
/*----------------------- Virtual Network     --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.3.0.0/16'
      ]
    }    
    subnets: [for (subnet,index) in subnets: {
      name: subnet.name
      properties: {
        addressPrefix: '10.3.${index + 1}.0/24'
        delegations: contains(subnet, 'delegation') ? [
          {
            name: subnet.name
            properties: {
              serviceName: subnet.delegation
            }
          }
        ] : []
      }
    }]    
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

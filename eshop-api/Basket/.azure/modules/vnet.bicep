@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Address Space Of The Virtual Network (vNet).')
param vNetAddressSpace string = '10.1.0.0/16'

@description('The Name Of The Web Api Subnet.')
param apiSubNetName string = 'basket-api'

@description('The Address Space Of The Web Api Subnet.')
param apiSubNetAddressSpace string ='10.1.0.0/24'

@description('The Name Of The Redis Cache Subnet.')
param redisSubNetName string = 'redis'

@description('The Address Space Of The Redis Cache Subnet.')
param redisSubNetAddressSpace string = '10.1.1.0/24'

/*-----------------------    Virtual Network        -------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2024-01-01' = {
  name: vnetName
  location: location
  
  properties: {
    addressSpace: {
      addressPrefixes: [
        vNetAddressSpace
      ]
    }
    subnets: [
      {
        name: apiSubNetName
        properties: {
          addressPrefix: apiSubNetAddressSpace
          delegations: [
            {
              name: 'Microsoft.Web/serverFarms'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }
          ]
        }
      }
      {
        name: redisSubNetName
        properties: {
          addressPrefix: redisSubNetAddressSpace
        }
      }
    ]
  }
}

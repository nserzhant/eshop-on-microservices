@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Address Space Of The Virtual Network (vNet).')
param vNetAddressSpace string = '10.2.0.0/16'

@description('The Auth Service Subnet Name.')
param apiSubNetName string = 'customer-authserver'

@description('The Auth Service Subnet Address Space.')
param apiSubNetAddressSpace string ='10.2.0.0/24'

@description('The Name Of The Database Subnet.')
param dbSubNetName string = 'customer-authserver-db'

@description('The Address Space Of The Database Subnet.')
param dbSubNetAddressSpace string = '10.2.1.0/24'

/*----------------------- Virtual Network  -------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' = {
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
            name: 'Microsoft.Web.serverFarms'
            properties: {
              serviceName: 'Microsoft.Web/serverFarms'
            }
          }
         ]
        }
      }
      {
        name: dbSubNetName
        properties: {
          addressPrefix: dbSubNetAddressSpace
        }
      }
    ]    
  }
}

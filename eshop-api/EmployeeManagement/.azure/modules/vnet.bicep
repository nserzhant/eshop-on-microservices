@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Address Space Of The Virtual Network (vNet).')
param vNetAddressSpace string = '10.4.0.0/16'

@description('The Name Of The Web Api Subnet.')
param apiSubNetName string = 'employees-management-api'

@description('The Address Space Of The Web Api Subnet.')
param apiSubNetAddressSpace string ='10.4.0.0/24'

@description('The Name Of The Authorization Server Subnet.')
param authServerSubNetName string = 'employees-auth-server'

@description('The Address Space Of The Authorization Server.')
param authServerSubNetAddressSpace string ='10.4.1.0/24'

@description('The Name Of The Database Subnet.')
param dbSubNetName string = 'employee-db'

@description('The Address Space Of The Database Subnet.')
param dbSubNetAddressSpace string = '10.4.2.0/24'

/*----------------------- Virtual Network          -------------------- */

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
        name: authServerSubNetName
        properties: {
         addressPrefix: authServerSubNetAddressSpace
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

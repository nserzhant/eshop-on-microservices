@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Storage Account Parameters  -------------------- */

@description('The Name Of The Storage Account')
param storageAccountName string

param storageAccountType string = 'Standard_LRS'

@description('Names Of The Containers.')
param containerNames string[]

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Name Of The Subnet.')
param subNetName string

@description('The Name Of The Private Endpoint')
param privateEndpointName string = '${storageAccountName}-private-endpoint'

@description('The Name Of The Network Interface For The Private Endpoint')
param privateEndoiuntNICName string = '${storageAccountName}-nic'

/*----------------------- Variables  --------------------------- */

var storageAccounPrivateDnsZoneName = 'privatelink.blob.${environment().suffixes.storage}'

/*-----------------------       RESOURCES        -------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' existing = {
  name: vnetName
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2023-06-01' existing = {
  name: subNetName
  parent: virtualNetwork
}

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: storageAccounPrivateDnsZoneName
}

/*----------------------- Storage Account       -------------------------- */

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'BlobStorage' 
  
  properties: {
    accessTier: 'Hot'
    publicNetworkAccess: 'Enabled'
    allowBlobPublicAccess: true
    allowSharedKeyAccess: true
    networkAcls: {
      bypass: 'None'
      defaultAction: 'Allow'
    }
  }

  resource blobServices 'blobServices' = {

    name: 'default'

    resource container 'containers' = [for containerName in containerNames: {
      name: containerName
    }]
  }
}

/*-------------------       VNET Integration -------------------*/


resource privateEndpoint 'Microsoft.Network/privateEndpoints@2023-05-01' = {
  name: privateEndpointName
  location: location

  properties: {
    subnet: {
      id: subnet.id
    }
    
    customNetworkInterfaceName: privateEndoiuntNICName

    privateLinkServiceConnections: [
      {
        name: privateEndpointName
        properties: {
         privateLinkServiceId: storageAccount.id
         groupIds: [
          'blob'
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
          name: storageAccounPrivateDnsZoneName
          properties: {
            privateDnsZoneId: privateDnsZone.id
          }
        }
      ]
    }
  }
}

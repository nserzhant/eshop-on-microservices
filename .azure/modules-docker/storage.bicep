@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Storage Account Parameters  -------------------- */

@description('The Name Of The Storage Account')
param storageAccountName string

@description('The Type Of The Storage Account')
param storageAccountType string = 'Standard_LRS'

@description('Names Of The File Share.')
param fileShareNames string[]

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Id Of The Subnet')
param subNetId string

@description('The Name Of The Private Endpoint')
param privateEndpointName string = '${storageAccountName}-private-endpoint'

@description('The Name Of The Network Interface For The Private Endpoint')
param privateEndpointNICName string = '${storageAccountName}-nic'

/*----------------------- Variables  --------------------------- */

var storageAccounPrivateDnsZoneName = 'privatelink.blob.${environment().suffixes.storage}'

/*-----------------------       RESOURCES        -------------------------- */

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

  kind: 'StorageV2'
 
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

  resource fileShare 'fileServices' = {
    name: 'default'

    resource share 'shares' = [for fileShareName in fileShareNames: {
      name: fileShareName
    }]
  }
}

/*-------------------       VNET Integration -------------------*/


resource privateEndpoint 'Microsoft.Network/privateEndpoints@2023-05-01' = {
  name: privateEndpointName
  location: location

  properties: {
    subnet: {
      id: subNetId
    }
    
    customNetworkInterfaceName: privateEndpointNICName

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

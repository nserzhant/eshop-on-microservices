@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------- SQL Server  Parameters -------------------- */

@description('The Id Of The Database Server User Assigned Identity. The Identity Should Have A Directory Readers Role')
param sqlUserAssignedIdentity string

@description('The Name Of The Database Server')
param sqlServerName string

@description('The Name Of The Database')
param dbName string

@description('The Name Of The SKU')
param dbSkuName string = 'Basic'

@description('The Tier Of The SKU')
param dbSkuTier string = 'Basic'

@description('The Capacity Of The SKU')
param dbSkuCapacity int = 5

@description('The Administrator Microsoft Entra Login Of The SQL Server')
param sqlEntraIdAdminLogin string

@description('The Administrator Microsoft Entra Object Id Of The SQL Server')
@secure()
param sqlEntraIdAdminObjectId string

/*----------------------- Virtual Network Parameters  --------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Name Of The Subnet.')
param subNetName string

@description('The Name Of The Private Endpoint')
param privateEndpointName string = '${sqlServerName}-private-endpoint'

@description('The Name Of The Network Interface For The Private Endpoint')
param privateEndoiuntNICName string = '${sqlServerName}-nic'

/*----------------------- Variables  --------------------------- */

var privateDnsZoneName =  'privatelink${environment().suffixes.sqlServerHostname}'

/*----------------------- RESOURCES  --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' existing = {
  name: vnetName
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2023-06-01' existing = {
  name: subNetName
  parent: virtualNetwork
}

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: privateDnsZoneName
}

/*----------------------- SQL Server --------------------------- */

resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' = {
  name: sqlServerName
  location: location
  identity: {
    type:'UserAssigned'
    userAssignedIdentities: {
      '${sqlUserAssignedIdentity}': {}
    }
  }
  properties: {
    primaryUserAssignedIdentityId: sqlUserAssignedIdentity
    version: '12.0'
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType:'Application'
      login: sqlEntraIdAdminLogin
      sid: sqlEntraIdAdminObjectId
      tenantId: tenant().tenantId
      azureADOnlyAuthentication: true      
    }
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-02-01-preview' = {
  name: dbName
  location: location
  parent: sqlServer

  sku: {
    name: dbSkuName
    tier: dbSkuTier
    capacity: dbSkuCapacity
  }

  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824
    zoneRedundant: false
    readScale: 'Disabled'
    autoPauseDelay: 60
    requestedBackupStorageRedundancy: 'Local'
    minCapacity: any('0.5')
  }
}

/*-------------------   VNET Integration   ---------------------*/

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
         privateLinkServiceId: sqlServer.id
         groupIds: [
          'sqlServer'
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

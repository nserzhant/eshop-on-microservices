@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------- SQL Server  Parameters -------------------- */

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

@description('The Administrator Login Of The SQL Server')
param administratorLogin string

@description('The Administrator Password Of The SQL Server')
@secure()
param administratorLoginPassword string

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- SQL Server --------------------------- */

resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    version: '12.0'
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
  
  resource firewallRules 'firewallRules@2023-05-01-preview' = {
    name: 'AllowAzureInternal'
    properties: {
      endIpAddress: '0.0.0.0'
      startIpAddress: '0.0.0.0'
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

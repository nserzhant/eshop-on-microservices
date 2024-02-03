@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Storage Account Parameters  -------------------- */

@description('The Name Of The Storage Account')
param storageAccountName string

@description('The Type Of The Storage Account')
param storageAccountType string = 'Standard_LRS'

@description('Names Of The Containers.')
param containerNames string[]

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

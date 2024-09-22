@description('Location for all resources.')
param location string = resourceGroup().location

/*------------------ Parameters --------------------------------- */

@description('The Name Of The Log Analytics Workspace')
param logAnalyticsName string

@description('The Name Of The Container App Environment.')
param containerAppEnvironmentName string

@description('The Name Of The Storage Account')
param storageAccountName string 

@description('The Names Of The File Share.')
param fileShareNames string[]

@description('The Id Of The Subnet')
param subNetId string

/*-----------------------  RESOURCES      ------------------------- */

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2020-08-01' existing = {
  name: logAnalyticsName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

/*----------------------- Container App Environment -------------- */

resource containerAppEnv 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: containerAppEnvironmentName
  location: location
  
  properties: {
    vnetConfiguration: {
      internal: false
      infrastructureSubnetId: subNetId
    }

    appLogsConfiguration: {
      destination: 'log-analytics'
      
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }    
  }
  
  resource storage 'storages' = [for fileShare in fileShareNames: {
    name: fileShare

    properties: {
      azureFile: {
        accessMode: 'ReadOnly'
        accountName: storageAccountName
        accountKey: storageAccount.listKeys().keys[0].value
        shareName: fileShare    
      }
    }
  }]
}

output Id string = containerAppEnv.id

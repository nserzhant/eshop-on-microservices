@description('Location for all resources.')
param location string = resourceGroup().location

/*-----------------------  Proxy App Parameters         -------------------- */

@description('The Id Of The Web App Service Plan')
param webAppServicePlanId string

@description('The Name Of The Proxy App')
param appName string

@description('The Reference To The Reverse Proxy Docker Image')
param envoyLinuxVersion string = 'DOCKER|envoyproxy/envoy:v1.31.1'

/*----------------------- Storage Account Parameters    -------------------- */

@description('The Name Of The Storage Account')
param storageAccountName string

@description('The Name Of The Storage Account Container')
param storageAccountContainerName string

/*----------------------- Log Analytics Workspace Parameters ----- */

@description('The Id Of The Log Analytics Workspace')
param logAnalyticsWorkspaceId string

/*-----------------------       RESOURCES        -------------------------- */

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource envoyProxy 'Microsoft.Web/sites@2022-09-01' = {
  name: appName
  location: location
  identity: {
    type: 'SystemAssigned'
  }

  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlanId
    siteConfig: {
      linuxFxVersion: envoyLinuxVersion
      alwaysOn: true
      healthCheckPath: '/'
    }     
  }

  resource configuration 'config' = {
    name: 'appsettings'

    properties: {
      WEBSITES_PORT: '8080'
    }
  }

  resource appServiceLogging  'config' = {
    name: 'logs' 

    properties: {
      applicationLogs: {
        fileSystem: {
          level: 'Warning'          
        }
      }
      httpLogs: {
        fileSystem: {
          retentionInDays: 7
          retentionInMb: 100
          enabled: true
        }
      }
      failedRequestsTracing: {
        enabled: true
      }
      detailedErrorMessages: {
        enabled: true
      }
    }
  }
}

resource mountConfiguration 'Microsoft.Web/sites/config@2022-09-01' = {
  name: 'azurestorageaccounts'
  parent: envoyProxy
  properties: {
    'config-mount':{
      type: 'AzureBlob'
      accountName: storageAccountName
      shareName: storageAccountContainerName
      mountPath: '/etc/envoy'
      accessKey: storageAccount.listKeys().keys[0].value
    }
  }
}

resource webAppDiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'application-logs'
  scope: envoyProxy 
  properties: {
    workspaceId: logAnalyticsWorkspaceId

    logs: [
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
      }
    ]
  }
}

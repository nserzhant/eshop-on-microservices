@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Web App Parameters -------------------- */

@description('The Id Of The Web App Service Plan')
param webAppServicePlanId string

@description('The Name Of The Web App')
param webAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOTNETCORE|8.0'

@description('The Configuration Of The Application')
param appConfiguration object

@description('The Connection Strings Of The Application')
@secure()
param appConnectionStrings object = {}

/*----------------------- Log Analytics Workspace Parameters ----- */

@description('The Id Of The Log Analytics Workspace')
param logAnalyticsWorkspaceId string

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- Web App     -------------------------- */

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  kind: 'linux'

  tags: {
    appName: webAppName
  }
  
  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlanId
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
      alwaysOn: true
      healthCheckPath: '/hc'
    } 
  }
  
  resource configuration 'config' = {
    name: 'appsettings'

    properties: appConfiguration
  }

  resource connectionStrings 'config' = {
    name: 'connectionstrings'

    properties: appConnectionStrings
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

resource webAppDiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'application-logs'
  scope: webApp 
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

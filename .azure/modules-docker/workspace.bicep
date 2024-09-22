@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Log Analytics Workspace And App Insights Parameters ----- */

@description('The Name Of The Log Analytics Workspace')
param logAnalyticsName string

/*-----------------------           RESOURCES           --------------------------- */

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
  name: logAnalyticsName
  location: location
  tags: {
    displayName: 'Log Analytics'
    ProjectName: 'eshop'
  }
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 120
    features: {
      searchVersion: 1
      legacy: 0
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

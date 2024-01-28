@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- Web App Parameters -------------------- */

@description('The Name Of The Web App')
param webAppName string

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string = 'client-portal-server-plan-${suffix}'

@description('The Name Of The Web App SKU')
param webAppSkuName string = 'B1'

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOCKER'

/*----------------------- Web App  -------------------- */

resource webAppServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: webAppServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: webAppSkuName
  }
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    }
  }
}

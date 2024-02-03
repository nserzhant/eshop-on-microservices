@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Statuc App Parameters ----------------------- */

@description('The Name Of The Static Web App')
param staticWebAppName string

@description('The Name Of The SKU')
param staticAppSkuName string = 'Free'

@description('The Tier Of The SKU')
param staticAppTier string = 'Free'

/*----------------------- Static Web App Resource Definition --------- */

resource staticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: staticWebAppName
  location: location
  sku: {
    name: staticAppSkuName
    tier: staticAppTier
  }

  properties: {
  }
}

output staticAppDNSName string = staticWebApp.properties.defaultHostname

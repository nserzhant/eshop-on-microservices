@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Web App Parameters -------------------- */

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string

@description('The Name Of The Web App SKU')
param webAppSkuName string = 'B1'

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- Web App     -------------------------- */

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

output servicePlanId string = webAppServicePlan.id

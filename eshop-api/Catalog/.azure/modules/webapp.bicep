@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string

@description('The Name Of The Database')
param dbName string

/*----------------------- Web App Parameters -------------------- */

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string

@description('The Name Of The Web App SKU')
param webAppSku string = 'B1'

@description('The Name Of The Web App')
param webAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string

/*----------------------- Variables  --------------------------- */

var databaseContributorRoleDefinitionResourceID = '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec'
var roleAssignmentName= guid(dbName, resourceGroup().id, databaseContributorRoleDefinitionResourceID)

/*----------------------- RESOURCES  -------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' existing = {
  name: vnetName
}

resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' existing = {
  name: sqlServerName
}

/*----------------------- Web App  -------------------- */

resource webAppServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: webAppServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: webAppSku 
  }
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    }
    vnetRouteAllEnabled: true
    virtualNetworkSubnetId: virtualNetwork.properties.subnets[0].id    
  }
  
  resource configuration 'config' = {
    name: 'appsettings'

    properties: {
      initDbOnStartup:'true'
      ASPNETCORE_ENVIRONMENT: 'Development'      
    }
  }

  resource connectionStrings 'config' = {
    name: 'connectionstrings'

    properties: {
      catalogDbConnectionString: {
        type: 'SQLAzure'
        value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      }
    }
  }
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: sqlServer
  name: roleAssignmentName
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', databaseContributorRoleDefinitionResourceID)
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

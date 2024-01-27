@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Virtual Network Parameters  -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

/*----------------------- SQL Server  Parameters  ------------------------ */

@description('The Name Of The Database Server')
param sqlServerName string

@description('The Name Of The Database')
param dbName string

/*----------------------- Web App Parameters     ------------------------- */

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string

@description('The Name Of The Web App SKU')
param webAppSkuName string = 'B1'

@description('The Name Of The Api Web App')
param apiWebAppName string

@description('The Name Of The Authorization Service Web App')
param authServerWebAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string

/*----------------------- Variables       ------------------------------- */

var databaseContributorRoleDefinitionResourceID = '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec'
var apiRoleAssignmentName= guid(dbName, resourceGroup().id, databaseContributorRoleDefinitionResourceID, apiWebAppName)
var authServerRoleAssignmentName= guid(dbName, resourceGroup().id, databaseContributorRoleDefinitionResourceID, authServerWebAppName)

/*----------------------- RESOURCES       ------------------------------ */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' existing = {
  name: vnetName
}

resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' existing = {
  name: sqlServerName
}

/*----------------------- Web Apps Service Plan -------------------------- */

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

/*----------------------- Api Web App                 -------------------- */

resource apiWebApp 'Microsoft.Web/sites@2022-09-01' = {
  name: apiWebAppName
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
      employeeDbConnectionString: {
        type: 'SQLAzure'
        value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      }
    }
  }
}

resource apiRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: sqlServer
  name: apiRoleAssignmentName
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', databaseContributorRoleDefinitionResourceID)
    principalId: apiWebApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

/*----------------------- Authorization Server App        -------------------- */

resource authServerWebApp 'Microsoft.Web/sites@2022-09-01' = {
  name: authServerWebAppName
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
    virtualNetworkSubnetId: virtualNetwork.properties.subnets[1].id    
  }
  
  resource configuration 'config' = {
    name: 'appsettings'

    properties: {
      ASPNETCORE_ENVIRONMENT: 'Development'    
    }
  }

  resource connectionStrings 'config' = {
    name: 'connectionstrings'

    properties: {
      employeeDbConnectionString: {
        type: 'SQLAzure'
        value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      }
    }
  }
}

resource authServerRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: sqlServer
  name: authServerRoleAssignmentName
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', databaseContributorRoleDefinitionResourceID)
    principalId: authServerWebApp.identity.principalId
    principalType:'ServicePrincipal'
  }
}

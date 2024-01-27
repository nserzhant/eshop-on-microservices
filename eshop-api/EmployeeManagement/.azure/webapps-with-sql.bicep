@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string = 'employee-management-sqlserver-${suffix}'

@description('The Name Of The Database')
param dbName string = 'eshop.employee.Db'

@description('The Name Of The SKU')
param dbSkuName string = 'Basic'

@description('The Tier Of The SKU')
param dbSkuTier string = 'Basic'

@description('The Capacity Of The SKU')
param dbSkuCapacity int = 5

@description('The Administrator Login Of The SQL Server')
param administratorLogin string = 'employeemanagementlogin'

@description('The Administrator Password Of The SQL Server')
@secure()
param administratorLoginPassword string

/*----------------------- Web App Parameters -------------------- */

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string = 'employee-management-splan-${suffix}'

@description('The Name Of The Web App SKU')
param webAppSkuName string = 'B1'

@description('The Api App Name')
param apiWebAppName string

@description('The Authorization Server App Name')
param authServerWebAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOTNETCORE|7.0'

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- SQL Server --------------------------- */

resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    version: '12.0'
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
  
  resource firewallRules 'firewallRules@2023-05-01-preview' = {
    name: 'AllowAzureInternal'
    properties: {
      endIpAddress: '0.0.0.0'
      startIpAddress: '0.0.0.0'
    }
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-02-01-preview' = {
  name: dbName
  location: location
  parent: sqlServer

  sku: {
    name: dbSkuName
    tier: dbSkuTier
    capacity: dbSkuCapacity
  }

  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 1073741824
    zoneRedundant: false
    readScale: 'Disabled'
    autoPauseDelay: 60
    requestedBackupStorageRedundancy: 'Local'
    minCapacity: any('0.5')
  }
}

/*-----------------------  Web Apps Service Plan------------ */

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

/*----------------------- Api Web App  ---------------------- */

resource apiWebApp 'Microsoft.Web/sites@2022-09-01' = {
  name: apiWebAppName
  location: location

  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    } 
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
        value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};User Id=${administratorLogin};Password=${administratorLoginPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
      }
    }
  }
}

/*----------------------- Auth Server Web App  ------------------ */

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: authServerWebAppName
  location: location

  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlan.id
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    } 
  }
  
  resource configuration 'config' = {
    name: 'appsettings'

    properties: {
      useEphemeralKeys: 'true'
      ASPNETCORE_ENVIRONMENT: 'Development'     
    }
  }

  resource connectionStrings 'config' = {
    name: 'connectionstrings'

    properties: {
      employeeDbConnectionString: {
        type: 'SQLAzure'
        value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};User Id=${administratorLogin};Password=${administratorLoginPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
      }
    }
  }
}

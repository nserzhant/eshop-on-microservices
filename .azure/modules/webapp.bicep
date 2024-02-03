@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string

@description('The Name Of The Database')
param dbName string

@description('The Administrator Login Of The SQL Server')
param administratorLogin string

@description('The Administrator Password Of The SQL Server')
@secure()
param administratorLoginPassword string

/*----------------------- Web App Parameters -------------------- */

@description('The Id Of The Web App Service Plan')
param webAppServicePlanId string

@description('The Name Of The Web App')
param webAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOTNETCORE|7.0'

@description('The Name Of The Connection String')
param connectionStringName string

@description('The Configuration Of The Application')
param appConfiguration object

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- Web App     -------------------------- */

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  kind: 'linux'

  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlanId
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    } 
  }
  
  resource configuration 'config' = {
    name: 'appsettings'

    properties: appConfiguration
  }

  resource connectionStrings 'config' = {
    name: 'connectionstrings'

    properties: {
      '${connectionStringName}': {
        type: 'SQLAzure'
        value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};User Id=${administratorLogin};Password=${administratorLoginPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
      }
    }
  }
}

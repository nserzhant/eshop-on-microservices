@description('Location for all resources.')
param location string = resourceGroup().location

@description('The Name Of The Api Web App')
param apiWebAppName string

@description('The Name Of The Authorization Server Web App')
param authServerWebAppName string

@description('The Administrator Microsoft Entra Login Of The SQL Server')
param sqlEntraIdAdminLogin string

@description('The Administrator Microsoft Entra Object Id Of The SQL Server')
@secure()
param sqlEntraIdAdminObjectId string

@description('The Id Of The Database Server User Assigned Identity. The Identity Should Have A Directory Readers Role')
param sqlUserAssignedIdentity string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOCKER|mcr.microsoft.com/appsvc/staticsite:latest'

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

@description('The Name Of The Virtual Network (vNet).')
param vnetName string = 'employee-management-vnet-${suffix}'

@description('The Name Of The Database Server')
param sqlServerName string = 'employee-management-sqlserver-${suffix}'

@description('The Name Of The Database')
param dbName string = 'eshop.employee.Db'

@description('The Name Of The Web App Service Plan')
param webAppServicePlanName string = 'employee-management-server-plan-${suffix}'

/*-----------------------       RESOURCES        -------------------------- */

module vnet 'modules/vnet.bicep' = {
  name: 'vnet'

  params: {
    location: location
    vnetName: vnetName
  }
}

module sql 'modules/sql.bicep' = {
  name: 'sql'

  params: {
    location: location
    vnetName: vnetName
    sqlServerName: sqlServerName
    dbName: dbName
    sqlEntraIdAdminLogin: sqlEntraIdAdminLogin
    sqlEntraIdAdminObjectId: sqlEntraIdAdminObjectId
    sqlUserAssignedIdentity: sqlUserAssignedIdentity
  }

  dependsOn: [
    vnet
  ]
}

module webApps 'modules/webapps.bicep' = {
  name: 'webapps'
  params: {
    location: location
    dbName: dbName
    sqlServerName: sqlServerName
    vnetName: vnetName
    webAppServicePlanName: webAppServicePlanName
    apiWebAppName: apiWebAppName
    authServerWebAppName: authServerWebAppName
    webappLinuxVersion: webappLinuxVersion
  }

  dependsOn: [
    sql
  ]
}

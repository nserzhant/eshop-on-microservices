@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- VNet  Parameters      -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string = 'vnet-${suffix}'

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Employees Database Server')
param employeesSqlServerName string

@description('The Name Of The Clients Database Server')
param clientsSqlServerName string

@description('The Name Of The Catalog Database Server')
param catalogSqlServerName string

@description('The Name Of The Database')
param dbName string = 'eshop-on-microservices.Db'

@description('The Id Of The Database Server User Assigned Identity. The Identity Should Have A Directory Readers Role')
param sqlUserAssignedIdentity string

@description('The Administrator Microsoft Entra Login Of The SQL Server')
param sqlEntraIdAdminLogin string

@description('The Administrator Microsoft Entra Object Id Of The SQL Server')
param sqlEntraIdAdminObjectId string

/*----------------------- Web Apps Parameters          -------------*/

@description('The Name Of The Client Apps Service Plan')
param clientAppsServicePlanName string = 'client-apps-splan-${suffix}'

@description('The Name Of The Employee Apps Service Plan')
param employeeAppsServicePlanName string = 'employee-apps-splan-${suffix}'

@description('The Name Of The Shared Services Service Plan')
param sharedServicesServicePlanName string = 'shared-services-splan-${suffix}'

@description('The Name Of The Catalog Api Web App')
param catalogApiWebAppName string = 'catalog-api-web-app-${suffix}'

@description('The Name Of The Client Authorization Server Web App')
param clientAuthServerWebAppName string = 'client-auth-web-app-${suffix}'

@description('The Name Of The Employee Api Web App')
param employeeApiWebAppName string = 'employee-api-web-app-${suffix}'

@description('The Name Of The Employee Authorization Server Web App')
param employeeAuthServerWebAppName string = 'employee-authserver-web-app-${suffix}'

@description('The Name Of The Customer Portal Static Web App')
param clientPortalWebAppName string = 'customer-portal-${suffix}'

@description('The Name Of The Employee Portal Static Web App')
param employeePortalWebAppName string = 'employee-portal-${suffix}'

@description('The Issuer Of The Employee Identity Server')
param employeeOAuthIssuer string = 'https://${employeeGatewayAppName}.azurewebsites.net/authorize'

@description('The Metadata Endpoint Of The Employee Identity Server')
param employeeOAuthMetadataEndpoint string = 'https://${employeeGatewayAppName}.azurewebsites.net/authorize/.well-known/openid-configuration'

@description('The Audience Of The Employee Identity Server Token')
param employeeOAuthAudience string = ''

/*----------------------- Storage Account Parameters  -------------*/

@description('The Name Of The Client Gateway Configuration Container')
param clientGatewayConfigContainerName string = 'clientgateway'

@description('The Name Of The Employee Gateway Configuration Container')
param employeeGatewayConfigContainerName string = 'employeegateway'

@description('The Name Of The Storage Account')
param storageAccountName string = 'storageacc${suffix}'

@description('Names Of The Containers.')
param containerNames string[] = [
  clientGatewayConfigContainerName
  employeeGatewayConfigContainerName
]

/*----------------------- NGINX Gateway Parameters    -------------*/

@description('The Name Of The Client Gateway App')
param clientGatewayAppName string = 'client-gateway-${suffix}'

@description('The Name Of The Employee Gateway App')
param employeeGatewayAppName string = 'employee-gateway-${suffix}'

/*------------------------- Variables  ----------------------------*/

var catalogApiConnectionStringName = 'catalogDbConnectionString'
var clientAuthServerConnectionString = 'clientDbConnectionString'
var employeeManagementConnectionString = 'employeeDbConnectionString'
var storageAccountSubNetName = '${storageAccountName}-subnet'
var clientsSqlServerSubNetName = '${clientsSqlServerName}-subnet'
var employeesSqlServerSubNetName = '${employeesSqlServerName}-subnet'
var catalogSqlServerSubNetName = '${catalogSqlServerName}-subnet'
var clientAppsSubnetName = 'client-apps-subnet'
var employeeAppsSubnetName = 'employee-apps-subnet'
var sharedServicesSubnetName = 'shared-services-subnet'
var clientAppsPrivateEndpointsSubnetName = 'client-apps-pe-subnet'
var employeeAppsPrivateEndpointsSubnetName = 'employee-apps-pe-subnet'
var sharedServicesPrivateEndpointsSubnetName = 'shared-services-pe-subnet'

/*----------------------- RESOURCES    --------------------------- */

module vnet 'modules-docker/vnet.bicep' = {
  name: 'vnet'
  params: {
    location: location
    vnetName: vnetName 
    subnets: [
      { name: clientsSqlServerSubNetName }
      { name: employeesSqlServerSubNetName }
      { name: catalogSqlServerSubNetName }
      { name: storageAccountSubNetName }
      { name: clientAppsPrivateEndpointsSubnetName }
      { name: employeeAppsPrivateEndpointsSubnetName }
      { name: sharedServicesPrivateEndpointsSubnetName }
      {
         name: clientAppsSubnetName
         delegation: 'Microsoft.Web/serverFarms'
      }
      {
         name: employeeAppsSubnetName
         delegation: 'Microsoft.Web/serverFarms'
      }
      {
         name: sharedServicesSubnetName
         delegation: 'Microsoft.Web/serverFarms'
      }
    ]
  }
}

module sqlClient 'modules-docker/sql.bicep' = {
 name: 'sqlClient'
 params: {
   location: location
   dbName: dbName
   sqlServerName: clientsSqlServerName 
   vnetName: vnetName
   subNetName: clientsSqlServerSubNetName
   sqlEntraIdAdminLogin: sqlEntraIdAdminLogin
   sqlEntraIdAdminObjectId: sqlEntraIdAdminObjectId
   sqlUserAssignedIdentity: sqlUserAssignedIdentity
 }
 dependsOn: [
   vnet
 ]
}

module sqlEmployee 'modules-docker/sql.bicep' = {
 name: 'sqlEmployee'
 params: {
   location: location
   dbName: dbName
   sqlServerName: employeesSqlServerName 
   vnetName: vnetName
   subNetName: employeesSqlServerSubNetName
   sqlEntraIdAdminLogin: sqlEntraIdAdminLogin
   sqlEntraIdAdminObjectId: sqlEntraIdAdminObjectId
   sqlUserAssignedIdentity: sqlUserAssignedIdentity
 }
 dependsOn: [
   vnet
 ]
}

module sqlCatalog 'modules-docker/sql.bicep' = {
 name: 'sqlCatalog'
 params: {
   location: location
   dbName: dbName
   sqlServerName: catalogSqlServerName 
   vnetName: vnetName
   subNetName: catalogSqlServerSubNetName
   sqlEntraIdAdminLogin: sqlEntraIdAdminLogin
   sqlEntraIdAdminObjectId: sqlEntraIdAdminObjectId
   sqlUserAssignedIdentity: sqlUserAssignedIdentity
 }
 dependsOn: [
   vnet
 ]
}

module storageAccount 'modules-docker/storage.bicep' = {
  name: 'storageAccount'
  params: {
    location: location
    containerNames: containerNames
    storageAccountName: storageAccountName
    vnetName: vnetName
    subNetName: storageAccountSubNetName
  }
  dependsOn: [
    vnet
  ]
}

module clientAppsSplan 'modules-docker/webappsplan.bicep' = {
  name: 'clientAppsSplan'
  params: {
    location: location
    webAppServicePlanName: clientAppsServicePlanName 
  }  
}

module employeeAppsSplan 'modules-docker/webappsplan.bicep' = {
  name: 'employeeAppsSplan'
  params: {
    location: location
    webAppServicePlanName: employeeAppsServicePlanName 
  }  
}

module sharedAppsSplan 'modules-docker/webappsplan.bicep' = {
  name: 'sharedAppsSplan'
  params: {
    location: location
    webAppServicePlanName: sharedServicesServicePlanName 
  }  
}

module clientGateway 'modules-docker/nginxGateway.bicep' = {
  name: 'clientGateway'
  params: {
    location: location
    gatewayAppName: clientGatewayAppName
    storageAccountContainerName: clientGatewayConfigContainerName
    storageAccountName: storageAccountName
    webAppServicePlanId: clientAppsSplan.outputs.servicePlanId 
    vnetName: vnetName
    subNetName: clientAppsSubnetName
  }
  dependsOn: [
    clientAppsSplan
    storageAccount
  ]
}

module employeeGateway 'modules-docker/nginxGateway.bicep' = {
  name: 'employeeGateway'
  params: {
    location: location
    gatewayAppName: employeeGatewayAppName
    storageAccountContainerName: employeeGatewayConfigContainerName
    storageAccountName: storageAccountName
    webAppServicePlanId: employeeAppsSplan.outputs.servicePlanId   
    vnetName: vnetName
    subNetName: employeeAppsSubnetName
  }
  dependsOn: [
    employeeAppsSplan
    storageAccount
  ]
}

module clientPortal 'modules-docker/webappAngular.bicep' = {
  name: 'clientPortal'
  params: {
    location: location
    vnetName: vnetName
    webAppServicePlanId: clientAppsSplan.outputs.servicePlanId 
    webAppName: clientPortalWebAppName
    webAppPrivateEndpointSubnetName: clientAppsPrivateEndpointsSubnetName
  }
  dependsOn: [
    vnet
  ]
}

module employeePortal 'modules-docker/webappAngular.bicep' = {
  name: 'employeePortal'
  params: {
    location: location
    vnetName: vnetName
    webAppServicePlanId: employeeAppsSplan.outputs.servicePlanId 
    webAppName: employeePortalWebAppName
    webAppPrivateEndpointSubnetName: employeeAppsPrivateEndpointsSubnetName
  }
  dependsOn: [
    vnet
  ]
}

module catalogApi 'modules-docker/webapp.bicep' = {
 name: 'catalogApi'
 params: {
   location: location
   vnetName: vnetName
   webAppPrivateEndpointSubnetName: sharedServicesPrivateEndpointsSubnetName
   webAppSubnetName: sharedServicesSubnetName
   connectionStringName: catalogApiConnectionStringName
   dbName: dbName
   sqlServerName: catalogSqlServerName 
   webAppName: catalogApiWebAppName
   webAppServicePlanId: sharedAppsSplan.outputs.servicePlanId 
   appConfiguration: {
    initDbOnStartup: true
    generatedItemPictureUriHost: '/catalog'
    clients: 'https://${clientGatewayAppName}.azurewebsites.net,https://${employeeGatewayAppName}.azurewebsites.net'
    EmployeeJWTSettings__Issuer: employeeOAuthIssuer
    EmployeeJWTSettings__MetadataAddress: employeeOAuthMetadataEndpoint
    EmployeeJWTSettings__Audience: employeeOAuthAudience
    ClientJWTSettings__Issuer: 'https://${clientGatewayAppName}.azurewebsites.net/authorize'
    ClientJWTSettings__MetadataAddress: 'https://${clientGatewayAppName}.azurewebsites.net/authorize/.well-known/openid-configuration'
    ASPNETCORE_ENVIRONMENT: 'Development'
   }
 }
 dependsOn: [
   sqlCatalog
   sharedAppsSplan
   clientPortal
   employeePortal
 ]
}

module clientAuthServer 'modules-docker/webapp.bicep' = {
  name: 'clientAuthServer'
  params: {
    location: location
    vnetName: vnetName
    webAppPrivateEndpointSubnetName: clientAppsPrivateEndpointsSubnetName
    webAppSubnetName: clientAppsSubnetName 
    connectionStringName: clientAuthServerConnectionString
    dbName: dbName
    sqlServerName: clientsSqlServerName 
    webAppName: clientAuthServerWebAppName
    webAppServicePlanId: clientAppsSplan.outputs.servicePlanId  
    appConfiguration: {
      initDbOnStartup: true
      useEphemeralKeys: true
      clients__0__clientOrigin: 'https://${clientGatewayAppName}.azurewebsites.net'
      clients__0__clientId: 'customer-portal'
      clients__0__displayName: 'public spa'
      ASPNETCORE_APPL_PATH: 'authorize'
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
  }
  dependsOn: [
    sqlClient
    clientAppsSplan
    clientPortal
    employeePortal
  ]
}

module employeeApi 'modules-docker/webapp.bicep' = {
  name: 'employeeApi'
  params: {
    location: location
    vnetName: vnetName
    webAppPrivateEndpointSubnetName: employeeAppsPrivateEndpointsSubnetName
    webAppSubnetName: employeeAppsSubnetName 
    connectionStringName: employeeManagementConnectionString
    dbName: dbName
    sqlServerName: employeesSqlServerName 
    webAppName: employeeApiWebAppName
    webAppServicePlanId: employeeAppsSplan.outputs.servicePlanId  
    appConfiguration: {
      initDbOnStartup: true
      clientOrigin: 'https://${employeeGatewayAppName}.azurewebsites.net'
      JWTSettings__Issuer: employeeOAuthIssuer
      JWTSettings__MetadataAddress: employeeOAuthMetadataEndpoint
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
  }
  dependsOn: [
    sqlEmployee
    employeeAppsSplan
    clientPortal
    employeePortal
  ]
}

module employeeAuthServer 'modules-docker/webapp.bicep' = {
  name: 'employeeAuthServer'
  params: {
    location: location
    vnetName: vnetName
    webAppPrivateEndpointSubnetName: employeeAppsPrivateEndpointsSubnetName
    webAppSubnetName: employeeAppsSubnetName 
    connectionStringName: employeeManagementConnectionString
    dbName: dbName
    sqlServerName: employeesSqlServerName 
    webAppName: employeeAuthServerWebAppName
    webAppServicePlanId: employeeAppsSplan.outputs.servicePlanId
    appConfiguration: {
      useEphemeralKeys: true
      clients__0__clientOrigin: 'https://${employeeGatewayAppName}.azurewebsites.net'
      clients__0__clientId: 'employee-portal'
      clients__0__displayName: 'public spa'
      ASPNETCORE_APPL_PATH: 'authorize'
      ASPNETCORE_ENVIRONMENT: 'Development'
    }  
  }
  dependsOn: [
    sqlEmployee
    employeeAppsSplan
    clientPortal
    employeePortal
  ]
}

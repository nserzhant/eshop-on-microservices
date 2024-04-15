@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string = 'eshop-on-microservices-sqlserver-${suffix}'

@description('The Name Of The Database')
param dbName string = 'eshop-on-microservices.Db'

@description('The Administrator Login Of The SQL Server')
param administratorLogin string = 'eshop-on-microservices-${suffix}'

@description('The Administrator Password Of The SQL Server')
@secure()
param administratorLoginPassword string = newGuid()

/*----------------------- Web Apps Parameters          -------------*/

@description('The Name Of The Web Apps Service Plan')
param webAppServicePlanName string = 'eshop-on-microservices-splan-${suffix}'

@description('The Name Of The Catalog Api Web App')
param catalogApiWebAppName string = 'catalog-api-web-app-${suffix}'

@description('The Name Of The Client Authorization Server Web App')
param clientAuthServerWebAppName string = 'client-auth-web-app-${suffix}'

@description('The Name Of The Employee Api Web App')
param employeeApiWebAppName string = 'employee-api-web-app-${suffix}'

@description('The Name Of The Employee Authorization Server Web App')
param employeeAuthServerWebAppName string = 'employee-authserver-web-app-${suffix}'

@description('The Name Of The Client Portal Static Web App')
param clientPortalStaticAppName string = 'client-portal-${suffix}'

@description('The Name Of The Employee Portal Static Web App')
param employeePortalStaticAppName string = 'employee-portal-${suffix}'

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

/*----------------------- RESOURCES    --------------------------- */

module sql 'modules/sql.bicep' = {
 name: 'sql'
 params: {
   location: location
   administratorLogin: administratorLogin 
   administratorLoginPassword: administratorLoginPassword
   dbName: dbName
   sqlServerName: sqlServerName 
 }
}

module storageAccount 'modules/storage.bicep' = {
  name: 'storageAccount'
  params: {
    location: location
    containerNames: containerNames
    storageAccountName: storageAccountName
  }
}


module servicePlan 'modules/webappsplan.bicep' = {
  name: 'servicePlan'
  params: {
    location: location
    webAppServicePlanName: webAppServicePlanName 
  }  
}

module clientGateway 'modules/nginxGateway.bicep' = {
  name: 'clientGateway'
  params: {
    location: location
    gatewayAppName: clientGatewayAppName
    storageAccountContainerName: clientGatewayConfigContainerName
    storageAccountName: storageAccountName
    webAppServicePlanId: servicePlan.outputs.servicePlanId  
  }
  dependsOn: [
    servicePlan
    storageAccount
  ]
}

module employeeGateway 'modules/nginxGateway.bicep' = {
  name: 'employeeGateway'
  params: {
    location: location
    gatewayAppName: employeeGatewayAppName
    storageAccountContainerName: employeeGatewayConfigContainerName
    storageAccountName: storageAccountName
    webAppServicePlanId: servicePlan.outputs.servicePlanId   
  }
  dependsOn: [
    servicePlan
    storageAccount
  ]
}

module clientPortal 'modules/staticapp.bicep' = {
  name: 'clientPortal'
  params: {
    location: location
    staticWebAppName: clientPortalStaticAppName
  }
}

module employeePortal 'modules/staticapp.bicep' = {
  name: 'employeePortal'
  params: {
    location: location
    staticWebAppName: employeePortalStaticAppName
  }
}

module catalogApi 'modules/webapp.bicep' = {
 name: 'catalogApi'
 params: {
   location: location
   administratorLogin: administratorLogin 
   administratorLoginPassword: administratorLoginPassword
   connectionStringName: catalogApiConnectionStringName
   dbName: dbName
   sqlServerName: sqlServerName 
   webAppName: catalogApiWebAppName
   webAppServicePlanId: servicePlan.outputs.servicePlanId 
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
   servicePlan
   clientPortal
   employeePortal
 ]
}

module clientAuthServer 'modules/webapp.bicep' = {
  name: 'clientAuthServer'
  params: {
    location: location
    administratorLogin: administratorLogin 
    administratorLoginPassword: administratorLoginPassword
    connectionStringName: clientAuthServerConnectionString
    dbName: dbName
    sqlServerName: sqlServerName 
    webAppName: clientAuthServerWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId  
    appConfiguration: {
      initDbOnStartup: true
      useEphemeralKeys: true
      clients__0__clientOrigin: 'https://${clientGatewayAppName}.azurewebsites.net'
      clients__0__clientId: 'client-portal'
      clients__0__displayName: 'public spa'
      ASPNETCORE_APPL_PATH: 'authorize'
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
  }
  dependsOn: [
    servicePlan
    clientPortal
  ]
}

module employeeApi 'modules/webapp.bicep' = {
  name: 'employeeApi'
  params: {
    location: location
    administratorLogin: administratorLogin 
    administratorLoginPassword: administratorLoginPassword
    connectionStringName: employeeManagementConnectionString
    dbName: dbName
    sqlServerName: sqlServerName 
    webAppName: employeeApiWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId  
    appConfiguration: {
      initDbOnStartup: true
      clientOrigin: 'https://${employeeGatewayAppName}.azurewebsites.net'
      JWTSettings__Issuer: employeeOAuthIssuer
      JWTSettings__MetadataAddress: employeeOAuthMetadataEndpoint
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
  }
  dependsOn: [
    servicePlan
    employeePortal
  ]
}

module employeeAuthServer 'modules/webapp.bicep' = {
  name: 'employeeAuthServer'
  params: {
    location: location
    administratorLogin: administratorLogin 
    administratorLoginPassword: administratorLoginPassword
    connectionStringName: employeeManagementConnectionString
    dbName: dbName
    sqlServerName: sqlServerName 
    webAppName: employeeAuthServerWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId
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
    servicePlan
    employeePortal
  ]
}

output staticAppDNSNames object = {
  clientPortal: clientPortal.outputs.staticAppDNSName
  employeePortal: employeePortal.outputs.staticAppDNSName
}

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- VNet  Parameters      -------------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string = 'vnet-${suffix}'

/*----------------------- Log Analytics Workspace Parameters -------- */

@description('The Name Of The Log Analytics Workspace')
param logAnalyticsName string = 'eshop-logs-workspace-${suffix}'

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Employee Database Server')
param employeeSqlServerName string = 'employee-sqlserver-${suffix}'

@description('The Name Of The Customer Database Server')
param customerSqlServerName string = 'customer-sqlserver-${suffix}'

@description('The Name Of The Catalog Database Server')
param catalogSqlServerName string = 'catalog-sqlserver-${suffix}'

@description('The Name Of The Ordering Database Server')
param orderingSqlServerName string = 'ordering-sqlserver-${suffix}'

@description('The Name Of The Saga Database Server')
param sagaSqlServerName string = 'saga-sqlserver-${suffix}'

@description('The Name Of The Database')
param dbName string = 'eshop.Db'

@description('The Id Of The Database Server User Assigned Identity. The Identity Should Have A Directory Readers Role')
param sqlUserAssignedIdentity string

@description('The Administrator Microsoft Entra Login Of The SQL Server')
param sqlEntraIdAdminLogin string

@description('The Administrator Microsoft Entra Object Id Of The SQL Server')
param sqlEntraIdAdminObjectId string

/*---------------- Container Apps Parameters ----------------------*/

@description('The Docker Registry Server')
param registryServer string

@description('The Docker Registry User Name')
param registryUserName string

@description('The Password Of The Container Registry Account')
@secure()
param registryPassword string

@description('Revision Suffix')
param revisionSuffix string

@description('The Name Of The Eshop Web App')
param eshopAppName string = 'eshop-${suffix}'

@description('The Name Of The Eshop Employee Portal Web App')
param eshopEmployeePortalAppName string = 'eshop-employee-portal-${suffix}'

@description('The Name Of The Container App Environment.')
param containerAppEnvironmentName string = 'eshop-app-environment-${suffix}'

@description('The Name Of The Customer Portal Container App.')
param customerPortalContainerAppName string = 'customer-portal-static-${suffix}'

@description('The Name Of The Employee Portal Container App')
param employeePortalContainerAppName string = 'employee-portal-static-${suffix}'

@description('The Name Of The Catalog Api Container App')
param catalogApiContainerAppName string = 'catalog-api-${suffix}'

@description('The Name Of The Customer Authorization Server Container App')
param customerAuthServerContainerAppName string = 'customer-authserver-${suffix}'

@description('The Name Of The Employee Api Container App')
param employeeApiContainerAppName string = 'employee-api-${suffix}'

@description('The Name Of The Employee Authorization Server Container App')
param employeeAuthServerContainerAppName string = 'employee-authserver-${suffix}'

@description('The Name Of The Basket Api Container App')
param basketApiContainerAppName string = 'basket-api-${suffix}'

@description('The Name Of The Ordering Api Container App')
param orderingApiContainerAppName string = 'ordering-api-${suffix}'

@description('The Name Of The Payment Processor Container App')
param paymentProcessorContainerAppName string = 'payment-processor-${suffix}'

@description('The Name Of The Saga Processor Container App')
param sagaProcessorContainerAppName string = 'saga-processor-${suffix}'

@description('The Issuer Of The Employee Identity Server')
param employeeOAuthIssuer string = ''

@description('The Metadata Endpoint Of The Employee Identity Server')
param employeeOAuthMetadataEndpoint string = ''

@description('The Audience Of The Employee Identity Server Token')
param employeeOAuthAudience string = ''

/*----------------------- Redis Cache  Parameters ---------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string = 'basket-rediscache-${suffix}'

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string = 'eshop-sb-${suffix}'

/*----------------------- Storage Account Parameters  -------------*/

@description('The Name Of The Eshop Web App Configuration Container')
param eshopProxyConfigFileShareName string = 'eshopproxy'

@description('The Name Of The Eshop Employee Portal Web App Configuration Container')
param eshopEmployeePortalProxyConfigFileShareName string = 'eshopemployeeportalproxy'

@description('The Name Of The Storage Account')
param storageAccountName string = 'storageacc${suffix}'

/*------------------------- Variables  ----------------------------*/

var fileShareNames  = [
  eshopProxyConfigFileShareName
  eshopEmployeePortalProxyConfigFileShareName
]

var sqlServerNames = [
  customerSqlServerName
  employeeSqlServerName
  catalogSqlServerName
  orderingSqlServerName
  sagaSqlServerName
]

var catalogAppiImageName =  '${registryServer}/${registryUserName}/eshop.catalog.api:latest'
var customerPortalImageName =  '${registryServer}/${registryUserName}/eshop.ui.customerportal:latest'
var customerAuthServerImageName =  '${registryServer}/${registryUserName}/eshop.customer.authorizationserver:latest'
var employeeAuthServerImageName =  '${registryServer}/${registryUserName}/eshop.employeemanagement.authorizationserver:latest'
var employeeApiImageName =  '${registryServer}/${registryUserName}/eshop.employeemanagement.api:latest'
var employeePortalImageName =  '${registryServer}/${registryUserName}/eshop.ui.employeeportal:latest'
var basketApiImageName = '${registryServer}/${registryUserName}/eshop.basket.api:latest'
var orderingApiImageName = '${registryServer}/${registryUserName}/eshop.ordering.api:latest'
var paymentProcessorImageName = '${registryServer}/${registryUserName}/eshop.payment.processor:latest'
var sagaProcessorImageName = '${registryServer}/${registryUserName}/eshop.saga.processor:latest'

/*----------------------- RESOURCES    --------------------------- */

module vnet 'modules-docker/vnet.bicep' = {
  name: 'vnet'
  params: {
    location: location
    vnetName: vnetName 
  }
}

module logAnalyticsWorkspace 'modules-docker/workspace.bicep' = {
  name: 'logAnalyticsWorkspace'
  params: {
    location: location
    logAnalyticsName: logAnalyticsName 
  }    
}

module redisCache 'modules-docker/redis.bicep' = {
  name: redisCacheName
  params: {
    location: location
    redisCacheName: redisCacheName
    subNetId: vnet.outputs.privateEndpointsSubnetId
  }
}

module servicebus 'modules-docker/servicebus.bicep' = {
  name: 'servicebus'
  params: {
    location: location
    serviceBusNamespaceName: serviceBusNamespaceName 
  }
}

module containerAppEnv 'modules-docker/containerAppEnv.bicep' = {
  name: 'containerAppEnv'
  params: {
    location: location
    containerAppEnvironmentName: containerAppEnvironmentName
    logAnalyticsName: logAnalyticsName
    fileShareNames: fileShareNames
    storageAccountName: storageAccountName
    subNetId: vnet.outputs.containerAppEnvSubnetId
  }
  dependsOn: [
    logAnalyticsWorkspace
    storageAccount
  ]
}

module sqlServers 'modules-docker/sql.bicep' = [for sqlServerName in sqlServerNames: {
    name: sqlServerName

    params: {
      location: location
      sqlEntraIdAdminLogin: sqlEntraIdAdminLogin
      sqlEntraIdAdminObjectId: sqlEntraIdAdminObjectId
      sqlUserAssignedIdentity: sqlUserAssignedIdentity
      dbName: dbName
      sqlServerName: sqlServerName 
      subNetId: vnet.outputs.privateEndpointsSubnetId
    }
  }
]

module storageAccount 'modules-docker/storage.bicep' = {
  name: 'storageAccount'
  params: {
    location: location
    fileShareNames: fileShareNames
    storageAccountName: storageAccountName
    subNetId: vnet.outputs.privateEndpointsSubnetId
  }
}

module eshopReverseProxy 'modules-docker/envoy.bicep' = {
  name: 'eshopReverseProxy'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: eshopAppName
    revisionSuffix: revisionSuffix
    fileShareName: eshopProxyConfigFileShareName
  }
  dependsOn: [
    storageAccount
  ]
}

module eshopEmployeePortalReverseProxy 'modules-docker/envoy.bicep' = {
  name: 'eshopEmployeePortalReverseProxy'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: eshopEmployeePortalAppName
    revisionSuffix: revisionSuffix
    fileShareName: eshopEmployeePortalProxyConfigFileShareName
  }
  dependsOn: [
    storageAccount
  ]
}

module customerPortal 'modules-docker/containerApp.bicep' = {
  name: 'customerPortal'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: customerPortalContainerAppName
    containerImage: customerPortalImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    external: false
  }
}

module employeePortal 'modules-docker/containerApp.bicep' = {
  name: 'employeePortal'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: employeePortalContainerAppName
    containerImage: employeePortalImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    external: false
  }
}

module catalogApi 'modules-docker/containerApp.bicep' = {
  name: 'catalogApi'
  params: {
    location: location
    serviceBusNamespaceName: serviceBusNamespaceName
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: catalogApiContainerAppName
    containerImage: catalogAppiImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    healthCheckPath: '/hc'
    env: {      
      initDbOnStartup: 'true'
      ASPNETCORE_ENVIRONMENT: 'Development'
      generatedItemPictureUriHost: '/catalog'
      clients: 'https://${eshopReverseProxy.outputs.fqdn},https://${ eshopEmployeePortalReverseProxy.outputs.fqdn}'
      EmployeeJWTSettings__Issuer: employeeOAuthIssuer == '' ? 'https://${ eshopEmployeePortalReverseProxy.outputs.fqdn}/authorize' : employeeOAuthIssuer
      EmployeeJWTSettings__MetadataAddress: employeeOAuthMetadataEndpoint == '' ? 'https://${eshopEmployeePortalReverseProxy.outputs.fqdn}/authorize/.well-known/openid-configuration' : employeeOAuthMetadataEndpoint
      EmployeeJWTSettings__Audience : employeeOAuthAudience
      CustomerJWTSettings__Issuer: 'https://${eshopReverseProxy.outputs.fqdn}/authorize'
      CustomerJWTSettings__MetadataAddress: 'https://${eshopReverseProxy.outputs.fqdn}/authorize/.well-known/openid-configuration'
      ConnectionStrings__catalogDbConnectionString: 'Server=tcp:${catalogSqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      broker__azureServiceBusConnectionString: 'sb://${serviceBusNamespaceName}.servicebus.windows.net'
    }
    external: false
  }
  dependsOn: [
    sqlServers
    servicebus
  ]
}

module customerAuthServer 'modules-docker/containerApp.bicep' = {
  name: 'customerAuthServer'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: customerAuthServerContainerAppName
    containerImage: customerAuthServerImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    healthCheckPath: '/hc'
    env: {
      initDbOnStartup: 'true'
      ASPNETCORE_APPL_PATH: 'authorize'
      ASPNETCORE_ENVIRONMENT: 'Development'
      useEphemeralKeys: 'true'
      clients__0__clientOrigin: 'https://${eshopReverseProxy.outputs.fqdn}'
      clients__0__clientId: 'customer-portal'
      clients__0__displayName: 'public spa'
      ConnectionStrings__openIddictDbConnectionString: 'DataSource=customer.openiddict.db'
      ConnectionStrings__customerDbConnectionString: 'Server=tcp:${customerSqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
    }
    external: false
  }
  dependsOn: [
    sqlServers
  ]
}

module employeeApi 'modules-docker/containerApp.bicep' = {
  name: 'employeeApi'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: employeeApiContainerAppName
    containerImage: employeeApiImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    healthCheckPath: '/hc'
    env: {
      initDbOnStartup: 'true'
      clientOrigin: 'https://${eshopEmployeePortalReverseProxy.outputs.fqdn}'
      JWTSettings__Issuer: employeeOAuthIssuer == '' ? 'https://${ eshopEmployeePortalReverseProxy.outputs.fqdn}/authorize' : employeeOAuthIssuer
      JWTSettings__MetadataAddress: employeeOAuthMetadataEndpoint == '' ? 'https://${eshopEmployeePortalReverseProxy.outputs.fqdn}/authorize/.well-known/openid-configuration' : employeeOAuthMetadataEndpoint
      JWTSettings__Audience: employeeOAuthAudience
      ConnectionStrings__employeeDbConnectionString: 'Server=tcp:${employeeSqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
    }
    external: false
  }
  dependsOn: [
    sqlServers
  ]
}

module employeeAuthServer 'modules-docker/containerApp.bicep' = {
  name: 'employeeAuthServer'
  params: {
    location: location  
    containerAppEnvironmentId: containerAppEnv.outputs.Id
    containerAppName: employeeAuthServerContainerAppName
    containerImage: employeeAuthServerImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    healthCheckPath: '/hc'
    env: {
      useEphemeralKeys: 'true'
      clients__0__clientOrigin: 'https://${eshopEmployeePortalReverseProxy.outputs.fqdn}'
      clients__0__clientId: 'employee-portal'
      clients__0__displayName: 'public spa'
      ASPNETCORE_APPL_PATH: 'authorize'
      ASPNETCORE_ENVIRONMENT: 'Development'
      ConnectionStrings__employeeDbConnectionString: 'Server=tcp:${employeeSqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      ConnectionStrings__openIddictDbConnectionString: 'DataSource=employee.openiddict.db'
    }
    external: false
  }
  dependsOn: [
    sqlServers
  ]
}

module basketApi 'modules-docker/containerApp.bicep' = {
  name: 'basketApi'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: basketApiContainerAppName
    containerImage: basketApiImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    redisCacheName: redisCacheName
    serviceBusNamespaceName: serviceBusNamespaceName
    healthCheckPath: '/hc'
    targetPort: 8080
    env: {
      JWTSettings__Issuer: 'https://${eshopReverseProxy.outputs.fqdn}/authorize'
      JWTSettings__MetadataAddress: 'https://${eshopReverseProxy.outputs.fqdn}/authorize/.well-known/openid-configuration'
      ASPNETCORE_ENVIRONMENT: 'Development'
      ASPNETCORE_HTTP_PORTS: '8080'
      ConnectionStrings__entraIdRedisConnectionString: '${redisCacheName}.redis.cache.windows.net:6380'
      broker__azureServiceBusConnectionString: 'sb://${serviceBusNamespaceName}.servicebus.windows.net'
    }
    external: false
  }
  dependsOn: [
    servicebus
    redisCache
  ]
}

module orderingApi 'modules-docker/containerApp.bicep' = {
  name: 'orderingApi'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: orderingApiContainerAppName
    containerImage: orderingApiImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    serviceBusNamespaceName: serviceBusNamespaceName
    healthCheckPath: '/hc'
    targetPort: 8080
    env: {
      initDbOnStartup: 'true'
      JWTSettings__Issuer: 'https://${eshopReverseProxy.outputs.fqdn}/authorize'
      JWTSettings__MetadataAddress: 'https://${eshopReverseProxy.outputs.fqdn}/authorize/.well-known/openid-configuration'
      ASPNETCORE_ENVIRONMENT: 'Development'
      ASPNETCORE_HTTP_PORTS: '8080'
      ConnectionStrings__orderingDbConnectionString: 'Server=tcp:${orderingSqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      broker__azureServiceBusConnectionString: 'sb://${serviceBusNamespaceName}.servicebus.windows.net'
    }
    external: false
  }
  dependsOn: [
    sqlServers
    servicebus
  ]
}

module paymentProcessor 'modules-docker/containerApp.bicep' = {
  name: 'paymentProcessor'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: paymentProcessorContainerAppName
    containerImage: paymentProcessorImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    serviceBusNamespaceName: serviceBusNamespaceName
    healthCheckPath: '/hc'
    targetPort: 8080
    env: {
      payment__processPayment: 'True'
      ASPNETCORE_ENVIRONMENT: 'Development'
      broker__azureServiceBusConnectionString: 'sb://${serviceBusNamespaceName}.servicebus.windows.net'
    }
    external: false
  }
  dependsOn: [
    servicebus
  ]
}

module sagaProcessor 'modules-docker/containerApp.bicep' = {
  name: 'sagaProcessor'
  params: {
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.Id 
    containerAppName: sagaProcessorContainerAppName
    containerImage: sagaProcessorImageName
    registryPassword: registryPassword
    registryServer: registryServer
    registryUserName: registryUserName
    revisionSuffix: revisionSuffix
    serviceBusNamespaceName: serviceBusNamespaceName
    healthCheckPath: '/hc'
    targetPort: 8080
    env: {
      initDbOnStartup: 'true'
      ASPNETCORE_ENVIRONMENT: 'Development'
      ConnectionStrings__sagaDbConnectionString: 'Server=tcp:${sagaSqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      broker__azureServiceBusConnectionString: 'sb://${serviceBusNamespaceName}.servicebus.windows.net'
    }
    external: false
  }
  dependsOn: [
    sqlServers
    servicebus
  ]
}

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string = 'eshop-on-microservices-sqlserver-${suffix}'

@description('The Name Of The Database')
param dbName string = 'eshop.Db'

@description('The Administrator Login Of The SQL Server')
param administratorLogin string = 'eshop-on-microservices-${suffix}'

@description('The Administrator Password Of The SQL Server')  

@secure()
param administratorLoginPassword string = newGuid()

/*----------------------- Log Analytics Workspace Parameters ----- */

@description('The Name Of The Log Analytics Workspace')
param logAnalyticsName string = 'eshop-logs-workspace-${suffix}'

/*----------------------- Redis Cache  Parameters ---------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string = 'eshop-rediscache-${suffix}'

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string = 'eshop-sb-${suffix}'

/*----------------------- Web Apps Parameters          -------------*/

@description('The Name Of The Web Apps Service Plan')
param webAppServicePlanName string = 'eshop-on-microservices-splan-${suffix}'

@description('The Name Of The Catalog Api Web App')
param catalogApiWebAppName string = 'catalog-api-web-app-${suffix}'

@description('The Name Of The Customer Authorization Server Web App')
param customerAuthServerWebAppName string = 'customer-authserver-web-app-${suffix}'

@description('The Name Of The Employee Api Web App')
param employeeApiWebAppName string = 'employee-api-web-app-${suffix}'

@description('The Name Of The Employee Authorization Server Web App')
param employeeAuthServerWebAppName string = 'employee-authserver-web-app-${suffix}'

@description('The Name Of The Basket Api Web App')
param basketApiWebAppName string = 'basket-api-web-app-${suffix}'

@description('The Name Of The Ordering Api Web App')
param orderingApiWebAppName string = 'ordering-api-web-app-${suffix}'

@description('The Name Of The Payment Processor Web App')
param paymentProcessorAppName string = 'payment-processor-${suffix}'

@description('The Name Of The Saga Processor Web App')
param sagaProcessorAppName string = 'saga-processor-${suffix}'

@description('The Name Of The Customer Portal Static Web App')
param customerPortalStaticAppName string = 'customer-portal-static-${suffix}'

@description('The Name Of The Employee Portal Static Web App')
param employeePortalStaticAppName string = 'employee-portal-static-${suffix}'

@description('The Issuer Of The Employee Identity Server')
param employeeOAuthIssuer string = 'https://${eshopEmployeePortalAppName}.azurewebsites.net/authorize'

@description('The Metadata Endpoint Of The Employee Identity Server')
param employeeOAuthMetadataEndpoint string = 'https://${eshopEmployeePortalAppName}.azurewebsites.net/authorize/.well-known/openid-configuration'

@description('The Audience Of The Employee Identity Server Token')
param employeeOAuthAudience string = ''

/*----------------------- Storage Account Parameters  -------------*/

@description('The Name Of The Eshop Web App Configuration Container')
param eshopProxyConfigContainerName string = 'eshopproxy'

@description('The Name Of The Eshop Employee Portal Web App Configuration Container')
param eshopEmployeePortalProxyConfigContainerName string = 'eshopemployeeportalproxy'

@description('The Name Of The Storage Account')
param storageAccountName string = 'storageacc${suffix}'

@description('Names Of The Containers.')
param containerNames string[] = [
  eshopProxyConfigContainerName
  eshopEmployeePortalProxyConfigContainerName
]

/*----------------------- Envoy Proxy Parameters      -------------*/

@description('The Name Of The Eshop Web App')
param eshopAppName string = 'eshop-${suffix}'

@description('The Name Of The Eshop Employee Portal Web App')
param eshopEmployeePortalAppName string = 'eshop-employee-portal-${suffix}'

/*------------------------- Variables  ----------------------------*/

var sqlConnectionString = 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};User Id=${administratorLogin};Password=${administratorLoginPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

/*----------------------- RESOURCES    --------------------------- */

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }

  properties: {   
  }
}

resource redisCache 'Microsoft.Cache/redis@2024-04-01-preview' = {
  name: redisCacheName
  location: location
  
  properties: {
    enableNonSslPort: false
    
    sku: {
      name: 'Basic'
      capacity: 0
      family: 'C'
    }
  }
}

module logAnalyticsWorkspace 'modules/workspace.bicep' = {
  name: 'logAnalyticsWorkspace'
  params: {
    location: location
    logAnalyticsName: logAnalyticsName 
  }    
}

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

module eshopApp 'modules/envoy.bicep' = {
  name: 'eshopApp'
  params: {
    location: location
    appName: eshopAppName
    storageAccountContainerName: eshopProxyConfigContainerName
    storageAccountName: storageAccountName
    webAppServicePlanId: servicePlan.outputs.servicePlanId  
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
  }
  dependsOn: [
    storageAccount
  ]
}

module eshopEmployeePortalApp 'modules/envoy.bicep' = {
  name: 'eshopEmployeePortalApp'
  params: {
    location: location
    appName: eshopEmployeePortalAppName
    storageAccountContainerName: eshopEmployeePortalProxyConfigContainerName
    storageAccountName: storageAccountName
    webAppServicePlanId: servicePlan.outputs.servicePlanId 
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id  
  }
  dependsOn: [
    storageAccount
  ]
}

module customerPortalStatic 'modules/staticapp.bicep' = {
  name: 'customerPortalStatic'
  params: {
    location: location
    staticWebAppName: customerPortalStaticAppName
  }
}

module employeePortalStatic 'modules/staticapp.bicep' = {
  name: 'employeePortalStatic'
  params: {
    location: location
    staticWebAppName: employeePortalStaticAppName
  }
}

module catalogApi 'modules/webapp.bicep' = {
 name: 'catalogApi'
 params: {
    location: location
    webAppName: catalogApiWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId 
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      initDbOnStartup: true
      generatedItemPictureUriHost: '/catalog'
      clients: 'https://${eshopAppName}.azurewebsites.net,https://${eshopEmployeePortalAppName}.azurewebsites.net'
      EmployeeJWTSettings__Issuer: employeeOAuthIssuer
      EmployeeJWTSettings__MetadataAddress: employeeOAuthMetadataEndpoint
      EmployeeJWTSettings__Audience: employeeOAuthAudience
      CustomerJWTSettings__Issuer: 'https://${eshopAppName}.azurewebsites.net/authorize'
      CustomerJWTSettings__MetadataAddress: 'https://${eshopAppName}.azurewebsites.net/authorize/.well-known/openid-configuration'
      ASPNETCORE_ENVIRONMENT: 'Development'
      broker__AzureServiceBusConnectionString: listKeys('${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBusNamespace.apiVersion).primaryConnectionString
    }
    appConnectionStrings: {
      catalogDbConnectionString: {
        type: 'SQLAzure'
        value: sqlConnectionString
      }
    }
  }
  dependsOn: [
    sql
  ]
}

module customerAuthServer 'modules/webapp.bicep' = {
  name: 'customerAuthServer'
  params: {
    location: location
    webAppName: customerAuthServerWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId  
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      initDbOnStartup: true
      useEphemeralKeys: true
      clients__0__clientOrigin: 'https://${eshopAppName}.azurewebsites.net'
      clients__0__clientId: 'customer-portal'
      clients__0__displayName: 'public spa'
      ASPNETCORE_APPL_PATH: 'authorize'
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
    appConnectionStrings: {
      customerDbConnectionString: {
        type: 'SQLAzure'
        value: sqlConnectionString
      }
      openIddictDbConnectionString : {
        type: 'Custom'
        value: 'DataSource=/home/site/customer.openiddict.db'
      }
    } 
  }
  dependsOn: [
    sql
  ]
}

module employeeApi 'modules/webapp.bicep' = {
  name: 'employeeApi'
  params: {
    location: location
    webAppName: employeeApiWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId  
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      initDbOnStartup: true
      clientOrigin: 'https://${eshopEmployeePortalAppName}.azurewebsites.net'
      JWTSettings__Issuer: employeeOAuthIssuer
      JWTSettings__MetadataAddress: employeeOAuthMetadataEndpoint
      JWTSettings__Audience: employeeOAuthAudience
      ASPNETCORE_ENVIRONMENT: 'Development'
    }
    appConnectionStrings: {
      employeeDbConnectionString: {
        type: 'SQLAzure'
        value: sqlConnectionString
      }
    } 
  }
  dependsOn: [
    sql
  ]
}

module employeeAuthServer 'modules/webapp.bicep' = {
  name: 'employeeAuthServer'
  params: {
    location: location
    webAppName: employeeAuthServerWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      useEphemeralKeys: true
      clients__0__clientOrigin: 'https://${eshopEmployeePortalAppName}.azurewebsites.net'
      clients__0__clientId: 'employee-portal'
      clients__0__displayName: 'public spa'
      ASPNETCORE_APPL_PATH: 'authorize'
      ASPNETCORE_ENVIRONMENT: 'Development'
    } 
    appConnectionStrings: {
      employeeDbConnectionString: {
        type: 'SQLAzure'
        value: sqlConnectionString
      }
      openIddictDbConnectionString : {
        type: 'Custom'
        value: 'DataSource=/home/site/employee.openiddict.db'
      }
    } 
  }
  dependsOn: [
    sql
  ]
}

module basketApi 'modules/webapp.bicep' = {
  name: 'basketApi'
  params: {
    location: location
    webAppName: basketApiWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId 
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      JWTSettings__Issuer: 'https://${eshopAppName}.azurewebsites.net/authorize'
      JWTSettings__MetadataAddress: 'https://${eshopAppName}.azurewebsites.net/authorize/.well-known/openid-configuration'
      ASPNETCORE_ENVIRONMENT: 'Development'
      broker__AzureServiceBusConnectionString: listKeys('${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBusNamespace.apiVersion).primaryConnectionString
    }
    appConnectionStrings: {
      redisConnectionString: {
        type:  'Custom'
        value: '${redisCacheName}.redis.cache.windows.net:6380,abortConnect=false,ssl=true,password=${redisCache.listKeys().primaryKey}'
      }
    }
  }
  dependsOn: [
    sql
  ]
}

module orderingApi 'modules/webapp.bicep' = {
  name: 'orderingApi'
  params: {
    location: location
    webAppName: orderingApiWebAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId 
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      initDbOnStartup: true
      JWTSettings__Issuer: 'https://${eshopAppName}.azurewebsites.net/authorize'
      JWTSettings__MetadataAddress: 'https://${eshopAppName}.azurewebsites.net/authorize/.well-known/openid-configuration'
      ASPNETCORE_ENVIRONMENT: 'Development'
      broker__AzureServiceBusConnectionString: listKeys('${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBusNamespace.apiVersion).primaryConnectionString
    }
    appConnectionStrings: {
      orderingDbConnectionString: {
        type: 'SQLAzure'
        value: sqlConnectionString
      }
    }
  }
  dependsOn: [
    sql
  ]
}

module paymentProcessor 'modules/webapp.bicep' = {
  name: 'paymentProcessor'
  params: {
    location: location
    webAppName: paymentProcessorAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId 
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      payment__processPayment: 'True'
      ASPNETCORE_ENVIRONMENT: 'Development'
      broker__AzureServiceBusConnectionString: listKeys('${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBusNamespace.apiVersion).primaryConnectionString
    }
  }
}

module sagaProcessor 'modules/webapp.bicep' = {
  name: 'sagaProcessor'
  params: {
    location: location
    webAppName: sagaProcessorAppName
    webAppServicePlanId: servicePlan.outputs.servicePlanId 
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
    appConfiguration: {
      initDbOnStartup: true
      ASPNETCORE_ENVIRONMENT: 'Development'
      broker__AzureServiceBusConnectionString: listKeys('${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBusNamespace.apiVersion).primaryConnectionString
    }
    appConnectionStrings: {
      sagaDbConnectionString: {
        type:  'SQLAzure'
        value: sqlConnectionString
      }
    }
  }
  dependsOn: [
    sql
  ]
}

output staticAppDNSNames object = {
  customerPortalStatic: customerPortalStatic.outputs.staticAppDNSName
  employeePortalStatic: employeePortalStatic.outputs.staticAppDNSName
}

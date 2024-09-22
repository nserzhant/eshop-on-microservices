@description('Required. Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*-----------------------    ACR  Parameters     -------------------- */

@description('The Name Of The Existing Azure Container Registry')
param acrName string

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

@description('The Name Of The Employee Management Database')
param employeeDbName string = 'eshop.employee.Db'

@description('The Name Of The customer Authserver Database')
param customerDbName string = 'eshop.customer.Db'

@description('The Name Of The Catalog Api Database')
param catalogDbName string = 'eshop.catalog.Db'

@description('The Name Of The Ordering Api Database')
param orderingDbName string = 'eshop.ordering.Db'

@description('The Name Of The Saga Processor Database')
param sagaDbName string = 'eshop.saga.Db'

@description('The Id Of The Database Server User Assigned Identity. The Identity Should Have A Directory Readers Role')
param sqlUserAssignedIdentity string

@description('The Administrator Microsoft Entra Login Of The SQL Server')
param sqlEntraIdAdminLogin string

@description('The Administrator Microsoft Entra Object Id Of The SQL Server')
param sqlEntraIdAdminObjectId string

/*----------------------- AKS Parameters  -------------------- */

@description('The Public IP Label Of The Eshop Web App.')
param dnsLabel string

@description('The Name Of The Managed Cluster Resource.')
param clusterName string

@description('The Number Of Nodes For The Cluster.')
@minValue(1)
@maxValue(50)
param agentCount int

@description('The Size Of The Virtual Machine.')
param agentVMSize string

@description('The Name Of The Namespace Where Service Accounts Created.')
param aksNamespace string

@description('The Name Of The Customer Authorization Server Service Account.')
param customerAuthServerServiceAccountName string = 'customer-authserver-sa'

@description('The Name Of The Employee Management Api Service Account.')
param employeeApiServiceAccountName string = 'employeemanagement-api-sa'

@description('The Name Of The Employee Authorization Server Service Account.')
param employeeAuthServerServiceAccountName string = 'employeemanagement-authserver-sa'

@description('The Name Of The Catalog Api Service Account.')
param catalogApiServiceAccountName string = 'catalog-api-sa'

@description('The Name Of The Basket Api Service Account.')
param basketApiServiceAccountName string = 'basket-api-sa'

@description('The Name Of The Ordering Api Service Account.')
param orderingApiServiceAccountName string = 'ordering-api-sa'

@description('The Name Of The Payment Processor Service Account.')
param paymentProcessirServiceAccountName string = 'payment-processor-sa'

@description('The Name Of The Saga Processor Service Account.')
param sagaProcessirServiceAccountName string = 'saga-processor-sa'

/*----------------------- Redis Cache  Parameters ---------------- */

@description('The Name Of The Azure Redis Cache')
param redisCacheName string = 'basket-rediscache-${suffix}'

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string = 'eshop-sb-${suffix}'

/*----------------------- UMI Parameters  -------------------- */

@description('The Name Of The Catalog UMI Resource.')
param catalogApiUMIName string = 'catalog-api-identity'

@description('The Name Of The Employee Management UMI Resource.')
param employeeManagementUMIName string = 'employee-management-identity'

@description('The Name Of The customer Authserver UMI Resource.')
param customerAuthServerUMIName string = 'customer-authserver-identity'

@description('The Name Of The Basket Api UMI Resource.')
param basketApiUMIName string = 'basket-api-identity'

@description('The Name Of The Ordering Api UMI Resource.')
param orderingApiUMIName string = 'ordering-api-identity'

@description('The Name Of The Payment Processor UMI Resource.')
param paymentProcessorUMIName string = 'payment-processor-identity'

@description('The Name Of The Saga Processor UMI Resource.')
param sagaProcessorUMIName string = 'saga-processor-identity'

/*----------------------- Variables    --------------------------- */

var databases = [
  {
    sqlDbName: customerDbName
    sqlServerName: customerSqlServerName
  }
  {
    sqlDbName: employeeDbName
    sqlServerName: employeeSqlServerName
  }
  {
    sqlDbName: catalogDbName
    sqlServerName: catalogSqlServerName
  }
  {
    sqlDbName: orderingDbName
    sqlServerName: orderingSqlServerName
  }
  {
    sqlDbName: sagaDbName
    sqlServerName: sagaSqlServerName
  }
]

var UMIs = [
  {
    umiName: customerAuthServerUMIName
    umiTargetServiceBusNamespaceName: ''
    umiTargetRedisCacheName: ''
    serviceAccounts: [ customerAuthServerServiceAccountName ]
  } 
  {
    umiName: employeeManagementUMIName
    umiTargetServiceBusNamespaceName: ''
    umiTargetRedisCacheName: ''
    serviceAccounts: [ employeeAuthServerServiceAccountName, employeeApiServiceAccountName ]
  }
  {
    umiName: catalogApiUMIName
    umiTargetServiceBusNamespaceName: serviceBusNamespaceName
    umiTargetRedisCacheName: ''
    serviceAccounts: [ catalogApiServiceAccountName ]
  }
  {
    umiName: orderingApiUMIName
    umiTargetServiceBusNamespaceName: serviceBusNamespaceName
    umiTargetRedisCacheName: ''
    serviceAccounts: [ orderingApiServiceAccountName ]
  }
  {
    umiName: paymentProcessorUMIName
    umiTargetServiceBusNamespaceName: serviceBusNamespaceName
    umiTargetRedisCacheName: ''
    serviceAccounts: [ paymentProcessirServiceAccountName ]
  }
  {
    umiName: basketApiUMIName
    umiTargetServiceBusNamespaceName: serviceBusNamespaceName
    umiTargetRedisCacheName: redisCacheName
    serviceAccounts: [ basketApiServiceAccountName ]
  }
  {
    umiName: sagaProcessorUMIName
    umiTargetServiceBusNamespaceName: serviceBusNamespaceName
    umiTargetRedisCacheName: ''
    serviceAccounts: [ sagaProcessirServiceAccountName ]
  }
]

/*----------------------- RESOURCES    --------------------------- */

 module aks 'modules-aks/aks.bicep' = {
  name: 'aks'

  params: {
    location: location
    acrName: acrName
    clusterName: clusterName
    dnsLabel: dnsLabel
    agentCount: agentCount
    agentVMSize: agentVMSize
  }
}
 
module redisCache 'modules-aks/redis.bicep' = {
  name: redisCacheName
  params: {
    location: location
    redisCacheName: redisCacheName
  }
}

module servicebus 'modules-aks/servicebus.bicep' = {
  name: 'servicebus'
  params: {
    location: location
    serviceBusNamespaceName: serviceBusNamespaceName 
  }
}

module sqlServers 'modules-aks/sql.bicep' = [for database in databases: {
    name: database.sqlServerName

    params: {
      location: location
      sqlServerName: database.sqlServerName
      dbName: database.sqlDbName
      sqlEntraIdAdminLogin: sqlEntraIdAdminLogin
      sqlEntraIdAdminObjectId: sqlEntraIdAdminObjectId
      sqlUserAssignedIdentity: sqlUserAssignedIdentity
    }
  }
]

module userAssignedManagedIdentities 'modules-aks/umi.bicep' = [for umi in UMIs: {
    name:  umi.umiName

    params: {
      location: location
      umiName: umi.umiName
      aksNamespace: aksNamespace
      aksOidcIssuer: aks.outputs.oidIssuer
      aksServiceAccountNames: umi.serviceAccounts
      serviceBusNamespaceName: umi.umiTargetServiceBusNamespaceName
      redisCacheName: umi.umiTargetRedisCacheName
    }

    dependsOn: [
      redisCache
      servicebus
    ]
  }
]

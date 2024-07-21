@description('Required. Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*-----------------------    ACR  Parameters     -------------------- */

@description('The Name Of The Existing Azure Container Registry')
param acrName string

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Employees Database Server')
param employeeSqlServerName string = 'employee-sqlserver-${suffix}'

@description('The Name Of The Clients Database Server')
param clientSqlServerName string = 'client-sqlserver-${suffix}'

@description('The Name Of The Catalog Database Server')
param catalogSqlServerName string = 'catalog-sqlserver-${suffix}'

@description('The Name Of The Employee Management Database')
param employeeDbName string = 'eshop.employee.Db'

@description('The Name Of The Client Authserver Database')
param clientDbName string = 'eshop.client.Db'

@description('The Name Of The Catalog Api Database')
param catalogDbName string = 'eshop.catalog.Db'

@description('The Id Of The Database Server User Assigned Identity. The Identity Should Have A Directory Readers Role')
param sqlUserAssignedIdentity string

@description('The Administrator Microsoft Entra Login Of The SQL Server')
param sqlEntraIdAdminLogin string

@description('The Administrator Microsoft Entra Object Id Of The SQL Server')
param sqlEntraIdAdminObjectId string


/*----------------------- AKS Parameters  -------------------- */

@description('The Public IP Label Of The Customer Portal Gateway.')
param dnsLabel string

@description('The Name Of The Managed Cluster Resource.')
param clusterName string

@description('The Number Of Nodes For The Cluster.')
@minValue(1)
@maxValue(50)
param agentCount int

@description('The Size Of The Virtual Machine.')
param agentVMSize string

@description('The Name Of The Namespace Where Service Account Created.')
param aksNamespace string

@description('The Name Of The Client Authorization Server Service Account.')
param clientAuthServerServiceAccountName string = 'client-authserver-sa'

@description('The Name Of The Employee Management Api Service Account.')
param employeeApiServiceAccountName string = 'employeemanagement-api-sa'

@description('The Name Of The Employee Authorization Server Service Account.')
param employeeAuthServerServiceAccountName string = 'employeemanagement-authserver-sa'

@description('The Name Of The Catalog Api Service Account.')
param catalogApiServiceAccountName string = 'catalog-api-sa'

/*----------------------- AKV Parameters  -------------------- */

@description('The Name Of The Azure Key Vault Resource.')
param akvName string = 'akv-eshop-${suffix}'

@description('The Name Of The Client Authorization Server Signing Key Akv Secret.')
param clientAuthSigningKeySecretName string = 'client-auth-signing-key'

@description('The Name Of The Employee Authorization Server Signing Key Akv Secret.')
param employeeAuthSigningKeySecretName string = 'employee-auth-signing-key'

@description('The Client Authorization Server Signing Key.')
@secure()
param clientAuthSigningKey string

@description('The Employee Authorization Server Signing Key')
@secure()
param employeeAuthSigningKey string

/*----------------------- UMI Parameters  -------------------- */

@description('The Name Of The Catalog UMI Resource.')
param catalogApiUMIName string = 'catalog-api-identity'

@description('The Name Of The Employee Management UMI Resource.')
param employeeManagementUMIName string = 'employee-management-identity'

@description('The Name Of The Client Authserver UMI Resource.')
param clientAuthServerUMIName string = 'client-authserver-identity'

/*----------------------- Variables    --------------------------- */

var databases = [
  {
    sqlDbName: clientDbName
    sqlServerName: clientSqlServerName
  }
  {
    sqlDbName: employeeDbName
    sqlServerName: employeeSqlServerName
  }
  {
    sqlDbName: catalogDbName
    sqlServerName: catalogSqlServerName
  }
]

var UMIs = [
  {
    umiName: clientAuthServerUMIName
    umiTargetSqlServerName: clientSqlServerName
    serviceAccounts: [ clientAuthServerServiceAccountName ]
  } 
  {
    umiName: employeeManagementUMIName
    umiTargetSqlServerName: employeeSqlServerName
    serviceAccounts: [ employeeAuthServerServiceAccountName, employeeApiServiceAccountName ]
  }
  {
    umiName: catalogApiUMIName
    umiTargetSqlServerName: catalogSqlServerName
    serviceAccounts: [ catalogApiServiceAccountName ]
  }
]

/*----------------------- RESOURCES    --------------------------- */
 
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

module userAssignedManagedIdentities 'modules-aks/umi.bicep' = [for umi in UMIs: {
    name:  umi.umiName

    params: {
      location: location
      umiName: umi.umiName
      aksNamespace: aksNamespace
      aksOidcIssuer: aks.outputs.oidIssuer
      aksServiceAccountNames: umi.serviceAccounts
      sqlServerName: umi.umiTargetSqlServerName
    }

    dependsOn: [
      sqlServers
    ]
  }
]

module akv 'modules-aks/akv.bicep' = {
  name: 'akv'

  params: {
    location: location
    akskeyValutProviderUMI: aks.outputs.keyValutProviderUMI
    akvName: akvName 
    clientAuthSigningKey: clientAuthSigningKey
    clientAuthSigningKeySecretName: clientAuthSigningKeySecretName
    employeeAuthSigningKey: employeeAuthSigningKey
    employeeAuthSigningKeySecretName: employeeAuthSigningKeySecretName
  }
}


@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- Container Apps Parameters ---------------- */

@description('The Name Of The Container App.')
param containerAppName string = 'saga-processor-${suffix}'

@description('The Name Of The Container App Environment.')
param containerAppEnvironmentName string = 'saga-processor-env-${suffix}'

@description('The Docker Container Image To Deploy.')
param containerImage string

@description('The Docker Registry Server')
param registryServer string = 'index.docker.io'

@description('The Docker Registry User Name')
param registryUserName string

@description('The Password Of The Container Registry Account')
@secure()
param registryPassword string

@description('Revision Suffix')
param revisionSuffix string

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Id Of The Database Server User Assigned Identity. The Identity Should Have A Directory Readers Role')
param sqlUserAssignedIdentity string

@description('The Name Of The Database Server')
param sqlServerName string = 'saga-server-${suffix}'

@description('The Name Of The Database')
param sqlDbName string = 'eshop.saga.Db'

@description('The Administrator Microsoft Entra Login Of The SQL Server')
param sqlEntraIdAdminLogin string

@description('The Administrator Microsoft Entra Object Id Of The SQL Server')
@secure()
param sqlEntraIdAdminObjectId string

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string = 'eshop-sb-${suffix}'

/*-----------------------  RESOURCES   ------------------------- */

module sql 'modules/sql.bicep' = {
  name: 'sql'

  params: {
    location: location
    sqlDbName: sqlDbName
    sqlServerName: sqlServerName
    sqlUserAssignedIdentity: sqlUserAssignedIdentity
    sqlEntraIdAdminLogin: sqlEntraIdAdminLogin
    sqlEntraIdAdminObjectId: sqlEntraIdAdminObjectId
  }
}

module servicebus 'modules/servicebus.bicep' = {
  name: 'servicebus'
  params: {
    location: location
    serviceBusNamespaceName: serviceBusNamespaceName 
  }
}

module containerapp 'modules/containerapp.bicep' = {
  name: 'containerapp'
  params: {
    location: location
    containerAppEnvironmentName: containerAppEnvironmentName 
    containerAppName: containerAppName
    registryServer: registryServer
    registryPassword: registryPassword
    registryUserName: registryUserName
    containerImage: containerImage
    revisionSuffix: revisionSuffix
    serviceBusNamespaceName: serviceBusNamespaceName
    sqlServerName: sqlServerName
    sqlDbName: sqlDbName
  }
  dependsOn: [
    sql
    servicebus
  ]
}

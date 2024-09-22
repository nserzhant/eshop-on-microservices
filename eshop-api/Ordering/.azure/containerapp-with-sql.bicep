@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)

/*----------------------- Container Apps Parameters ---------------- */

@description('The Name Of The Container App.')
param containerAppName string = 'ordering-api-${suffix}'

@description('The Name Of The Container App Environment.')
param containerAppEnvironmentName string = 'ordering-api-env-${suffix}'

@description('The Docker Container Image To Deploy.')
param containerImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

@description('The Docker Registry Server')
param registryServer string = 'index.docker.io'

@description('The Docker Registry User Name')
param registryUserName string

@description('The Password Of The Container Registry Account')
@secure()
param registryPassword string

@description('Revision Suffix')
param revisionSuffix string

@description('The Name Of The Customer Authorization Server Web App')
param customerAuthServerWebAppName string = 'customer-authserver-web-app-${suffix}'

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string = 'ordering-api-server-${suffix}'

@description('The Name Of The Database')
param sqlDbName string = 'eshop.ordering.Db'

@description('The Name Of The SKU')
param dbSkuName string = 'Basic'

@description('The Tier Of The SKU')
param dbSkuTier string = 'Basic'

@description('The Capacity Of The SKU')
param dbSkuCapacity int = 5

@description('The Administrator Login Of The SQL Server')
param administratorLogin string = 'orderingapilogin'

@description('The Administrator Password Of The SQL Server')
@secure()
param administratorLoginPassword string

/*----------------------- Variables  --------------------------- */

var registryPasswordRef = 'dockerhub-pwd'

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- Container App     -------------------- */


resource containerAppEnv 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: containerAppEnvironmentName
  location: location
  
  properties: {
  }
}

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {      
      ingress: {
        external: true
        targetPort: 8080
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: [
        {
          passwordSecretRef: registryPasswordRef
          server: registryServer
          username: registryUserName
        }
      ]
      secrets: [
        {
          name: registryPasswordRef
          value: registryPassword
        }
      ]
    }
    template: {      
      revisionSuffix: revisionSuffix
      containers: [
        {          
          name: containerAppName
          image: containerImage          
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'            
          }
          env: [
            {
              name: 'initDbOnStartup'
              value: 'true'
            }
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
              name: 'ConnectionStrings__orderingDbConnectionString'
              value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${sqlDbName};User Id=${administratorLogin};Password=${administratorLoginPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;' 
            }
            {
              name: 'JWTSettings__Issuer'
              value: 'https://${customerAuthServerWebAppName}.azurewebsites.net'
            }
            {
              name: 'JWTSettings__MetadataAddress'
              value: 'https://${customerAuthServerWebAppName}.azurewebsites.net/.well-known/openid-configuration'
            }
            {
              name: 'broker__rabbitMQHost'
              value: ''
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

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
  name: sqlDbName
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

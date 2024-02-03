@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- SQL Server  Parameters -------------------- */

@description('The Name Of The Database Server')
param sqlServerName string

@description('The Name Of The Database')
param dbName string

/*----------------------- Virtual Network Parameters  --------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Name Of The Web App Subnet.')
param webAppSubnetName string

@description('The Name Of The Web App Private Endpoint Subnet.')
param webAppPrivateEndpointSubnetName string

@description('The Name Of The Private Endpoint')
param privateEndpointName string = '${webAppName}-private-endpoint'

@description('The Name Of The Network Interface For The Private Endpoint')
param privateEndoiuntNICName string = '${webAppName}-nic'

/*----------------------- Web App Parameters    -------------------- */

@description('The Id Of The Web App Service Plan')
param webAppServicePlanId string

@description('The Name Of The Web App')
param webAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOCKER'

@description('The Name Of The Connection String')
param connectionStringName string

@description('The Configuration Of The Application')
param appConfiguration object

/*----------------------- Variables  --------------------------- */

var privateDnsZoneName =  'privatelink.azurewebsites.net'
var databaseContributorRoleDefinitionResourceID = '9b7fa17d-e63e-47b0-bb0a-15c516ac86ec'
var webAppRoleAssignmentName= guid(webAppName, resourceGroup().id, databaseContributorRoleDefinitionResourceID)

/*----------------------- RESOURCES  --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' existing = {
  name: vnetName
}

resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' existing = {
  name: sqlServerName
}

/*----------------------- Virtual Network Subnet -------------- */

resource webAppSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-06-01' existing = {
  name: webAppSubnetName
  parent: virtualNetwork
}

resource webAppPrivateEndpointSubnet 'Microsoft.Network/virtualNetworks/subnets@2023-06-01' existing = {
  name: webAppPrivateEndpointSubnetName
  parent: virtualNetwork
}

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: privateDnsZoneName
}

/*----------------------- Web App     -------------------------- */

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: webAppServicePlanId
    siteConfig: {
      linuxFxVersion: webappLinuxVersion
    }  
    vnetRouteAllEnabled: true
    virtualNetworkSubnetId: webAppSubnet.id    
    publicNetworkAccess: 'Disabled'
  }

  resource web 'config' = {
    name: 'web'

    properties: {
      httpLoggingEnabled: true
      logsDirectorySizeLimit: 100
      detailedErrorLoggingEnabled: true
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
        value: 'Server=tcp:${sqlServerName}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${dbName};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication="Active Directory Default";'
      }
    }
  }
}

resource webAppRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: sqlServer
  name: webAppRoleAssignmentName
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', databaseContributorRoleDefinitionResourceID)
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

/*-------------------       VNET Integration -------------------*/

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2023-05-01' = {
  name: privateEndpointName
  location: location

  properties: {
    subnet: {
      id: webAppPrivateEndpointSubnet.id
    }
    
    customNetworkInterfaceName: privateEndoiuntNICName

    privateLinkServiceConnections: [
      {
        name: privateEndpointName
        properties: {
         privateLinkServiceId: webApp.id
         groupIds: [
          'sites'
         ]
        }
      }
    ]
  }

  resource privateEndpointDnsZoneGroup 'privateDnsZoneGroups' = {
    name: 'default'

    properties: {
      privateDnsZoneConfigs: [
        {
          name: privateDnsZoneName
          properties: {
            privateDnsZoneId: privateDnsZone.id
          }
        }
      ]
    }
  }
}

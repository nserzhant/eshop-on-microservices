@description('Location for all resources.')
param location string = resourceGroup().location
/*----------------------- Web App Parameters    -------------------- */

@description('The Id Of The Web App Service Plan')
param webAppServicePlanId string

@description('The Name Of The Web App')
param webAppName string

@description('The Linux Version Of The Web App')
param webappLinuxVersion string = 'DOCKER'

/*----------------------- Virtual Network Parameters  --------------- */

@description('The Name Of The Virtual Network (vNet).')
param vnetName string

@description('The Name Of The Web App Private Endpoint Subnet.')
param webAppPrivateEndpointSubnetName string

@description('The Name Of The Private Endpoint')
param privateEndpointName string = '${webAppName}-private-endpoint'

@description('The Name Of The Network Interface For The Private Endpoint')
param privateEndoiuntNICName string = '${webAppName}-nic'


/*----------------------- Variables  --------------------------- */

var privateDnsZoneName =  'privatelink.azurewebsites.net'

/*----------------------- RESOURCES  --------------------------- */

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2023-05-01' existing = {
  name: vnetName
}

/*----------------------- Virtual Network Subnet -------------- */

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2023-06-01' existing = {
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
  }
}


/*-------------------       VNET Integration -------------------*/

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2023-05-01' = {
  name: privateEndpointName
  location: location

  properties: {
    subnet: {
      id: subnet.id
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

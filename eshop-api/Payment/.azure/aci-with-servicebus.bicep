@description('Location for all resources.')
param location string = resourceGroup().location

@description('Suffix. Added At The End Of The Resource Name. The Default Value Is Based On The Resource Group Id')
param suffix string = uniqueString(resourceGroup().id)


/*----------------------- ACI Parameters -------------------- */

@description('The Docker Registry Server')
param registryServer string = 'index.docker.io'

@description('The Docker Registry User Name')
param registryUserName string

@description('The Password Of The Container Registry Account')
@secure()
param registryPassword string

@description('The Name Of The Container Instance Group')
param containerInstanceGroupName string = 'payment-processor-group-${suffix}'

@description('The Name Of The Container Instance')
param containerInstanceName string = 'payment-processor-${suffix}'

@description('The Docker Container Image To Deploy.')
param containerImage string = 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'

@description('The Number Of CPU Cores To Allocate To The Container.')
param cpuCores int = 1

@description('The Amount Of Memory To Allocate To The Container In Gigabytes.')
param memoryInGb int = 1

@description('Port To Open On The Container And The Public IP Address.')
param port int = 8080

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string = 'eshop-sb-${suffix}'

@description('The Name Of The Service Bus SKU')
param serviceBusSKU string = 'Standard'

/*----------------------- Variables  --------------------------- */

var serviceBusDataOwnerResourceID = '090c5cfd-751d-490a-894a-3ce6f1109419'
var roleAssignmentName= guid(serviceBusNamespaceName, resourceGroup().id, serviceBusDataOwnerResourceID)

/*----------------------- RESOURCES  --------------------------- */
/*----------------------- Service Bus ------------------------ */

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: serviceBusSKU
    tier: serviceBusSKU
  }

  properties: {   
  }
}


/*----------------------- Azure Container Instances -------------------- */

resource containerInstance 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
  name: containerInstanceGroupName
  location: location
  identity: {
    type: 'SystemAssigned'
  }  
  properties: {
    containers: [
      { 
        name: containerInstanceName
        properties: {
          image: containerImage
            ports: [
              { 
                port: port
                protocol: 'TCP'
              }
            ]
            livenessProbe: {
              httpGet: {
                port: port
                path: '/hc'
              }
              initialDelaySeconds: 30
              periodSeconds: 30
              failureThreshold: 3
            }
            environmentVariables: [
              {
                name: 'ASPNETCORE_ENVIRONMENT'
                value: 'Development' 
              }
              {
                name: 'ASPNETCORE_HTTP_PORTS'
                value: '8080'
              }
              {
                name: 'broker__azureServiceBusConnectionString'
                value: 'sb://${serviceBusNamespace.name}.servicebus.windows.net'
              }
            ]
           resources: {
            requests: {
              cpu: cpuCores
              memoryInGB: memoryInGb
            }
          }
        }
      }      
    ]        
    osType: 'Linux'
    restartPolicy: 'Always'
    ipAddress: {
      ports: [
        {
          port: port
        }
      ] 
      dnsNameLabel: containerInstanceName
      type: 'Public'
    }
    imageRegistryCredentials: [
      {
        password: registryPassword
        server: registryServer
        username: registryUserName
      }
    ]
  }
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: roleAssignmentName
  scope: serviceBusNamespace
  properties: {
    principalId: containerInstance.identity.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', serviceBusDataOwnerResourceID)
    principalType: 'ServicePrincipal'
  }
}

output fqdn string = containerInstance.properties.ipAddress.fqdn 

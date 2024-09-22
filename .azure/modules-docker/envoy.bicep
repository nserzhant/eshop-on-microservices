@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Container Apps Parameters ---------------- */

@description('The Id Of The Container App Environment.')
param containerAppEnvironmentId string

@description('The Name Of The Container App.')
param containerAppName string

@description('Revision Suffix')
param revisionSuffix string

@description('The Names Of The File Share.')
param fileShareName string

/*----------------------- Variables  --------------------------- */

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppEnvironmentId  
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
    }
    template: {      
      revisionSuffix: revisionSuffix
      containers: [
        {          
          name: containerAppName
          image: 'envoyproxy/envoy:v1.31.1'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'                 
          }         
          volumeMounts: [
            {
              mountPath: '/etc/envoy'
              volumeName: 'configuration'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
      volumes: [
        {
          name: 'configuration'
          storageType: 'AzureFile'
          storageName: fileShareName
        }
      ]
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn

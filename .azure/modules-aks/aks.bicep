@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- AKS Parameters  -------------------- */

@description('The Public IP Label Of The Eshop Web App.')
param dnsLabel string

@description('The Name Of The Managed Cluster Resource.')
param clusterName string

@description('Optional DNS Prefix To Use With Hosted Kubernetes API Server FQDN.')
param dnsPrefix string = '${clusterName}-prefix'

@description('Disk Size (in GB) To Provision For Each Of The Agent Pool Nodes. This Value Ranges From 0 To 1023. Specifying 0 Will Apply The Default Disk Size For That AgentVMSize.')
@minValue(0)
@maxValue(1023)
param osDiskSizeGB int = 64

@description('The Number Of Nodes For The Cluster.')
@minValue(1)
@maxValue(50)
param agentCount int

@description('The Size Of The Virtual Machine.')
param agentVMSize string

/*-----------------------    ACR  Parameters     -------------------- */

@description('The Name Of The Existing Azure Container Registry')
param acrName string

/*----------------------- Variables  --------------------------- */

var acrPullRoleDefinitionResourceID = '7f951dda-4ed3-4680-a7ca-43fe172d538d'
var roleAssignmentName= guid(clusterName, resourceGroup().id, acrPullRoleDefinitionResourceID)

/*-----------------------     RESOURCES     --------------------------- */
/*-----------------------     AKS Cluster   --------------------------- */

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: acrName
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-04-02-preview' = {
  name: clusterName
  location: location

  sku: {
    name: 'Base'
    tier: 'Free'
  }

  identity: {
    type: 'SystemAssigned'
  }

  properties: {
    dnsPrefix: dnsPrefix
    enableRBAC: true

    agentPoolProfiles: [
      {
        name: 'agentpool'
        osDiskSizeGB: osDiskSizeGB
        count: agentCount
        vmSize: agentVMSize
        osType: 'Linux'
        mode: 'System'
      }
    ]

    addonProfiles: {      
      azureKeyvaultSecretsProvider: {
        enabled: true
        config: {
          enableSecretRotation: 'true'
          rotationPollInterval: '5m'
        }
      }           
    }

    networkProfile: {
      networkPlugin: 'azure'
      networkPolicy: 'calico'
    }

    securityProfile: {
      workloadIdentity: {
        enabled: true        
      }
    }

    oidcIssuerProfile: {
      enabled: true
    }  
  }
}

/*----------------------- AKS INGRESS Controller --------------------------- */

module runHelm 'br/public:deployment-scripts/aks-run-helm:2.0.3' = {
  name: 'installIngressController'
  params: {
    aksName: clusterName
    location: location
    helmRepo: 'ingress-nginx'
    helmRepoURL: 'https://kubernetes.github.io/ingress-nginx'     
    helmApps: [
      {
        helmApp: 'ingress-nginx/ingress-nginx'
        helmAppName: 'ingress-nginx'
        helmAppParams: ' --create-namespace --namespace ingress-nginx --set controller.service.annotations."service\\.beta\\.kubernetes\\.io/azure-dns-label-name"=${dnsLabel}  --set controller.service.annotations."service\\.beta\\.kubernetes\\.io/azure-load-balancer-health-probe-request-path"=/healthz'
      }
    ]
  }
  dependsOn: [
    aks
  ]
}

/*----------------------- ACR Role Assginment --------------------------- */

resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: roleAssignmentName
  scope: acr
  properties: {
    principalId: aks.properties.identityProfile.kubeletidentity.objectId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', acrPullRoleDefinitionResourceID)
    principalType: 'ServicePrincipal'
  }  
}

output keyValutProviderUMI string = aks.properties.addonProfiles.azureKeyvaultSecretsProvider.identity.objectId
output oidIssuer string = aks.properties.oidcIssuerProfile.issuerURL

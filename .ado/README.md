# Pipelines are grouped by deployment type:

>Pipelines should be executed in the sequence in which they are listed in the description.

## 1. Zip-Deployment-Based Pipelines:

### Description:

1) `eshop-on-microservices-ci.yml` - build and test. 
2) `eshop-on-microservices-cd.yml` - deploy Azure resources using Bicep artifacts and deploy Web Apps using zip artifacts from the CI pipeline. The Azure resources include Linux App Service, SQL Server, Static Apps, and a Storage Account.

### Setup:

*  Create pipelines in the ADO project. The name of each pipeline should match the file name without its extension.
*  Create the resource group (powershell):
   ```pwsh
   $resourceGroupsLocation="westeurope"
   $resourceGroupName = "eshop"

   az group create --name $resourceGroupName --location $resourceGroupsLocation
   ```
*  Create an Azure Resource Manager service connection with the name `azure-spn`. In the **New Azure service connection** pane, select `eshop` as the Resource Group parameter value.
*  eshop-on-microservices-cd: 
   1. Create the `sqlPasswordSecret` variable in the Variables section with your own value for the SQL Server password (marked as secret).
   2. Create the `subscription-id` variable in the Variables section and set its value to the subscription in which the resource group was created.

## 2. Docker Image-Based Pipelines:

### Description:

1) `eshop-on-microservices-ci-docker.yml` - run tests, build the WebApps Docker images, and push the images to the Docker Hub registry.
2) `eshop-on-microservices-cd-docker-infrastructure.yml` - Deploy Azure resources using Bicep artifacts from the CI pipeline, and set Microsoft Entra ID authorization for the WebApps to access the SQL Servers. The Azure resources include Linux App Service, SQL Server, Storage Account, and VNet.
3) `eshop-on-microservices-cd-docker.yml` - deploy WebApp Docker images from the CI pipeline to App Services.

### Setup:  
    
*  Create pipelines in the ADO project. The name of each pipeline should match the file name without its extension.
*  Create repositories in the Docker Hub registry: `eshop.catalog.api`, `eshop.employeemanagement.api`, `eshop.employeemanagement.authorizationserver`, `eshop.client.authorizationserver`, `eshop.ui.employeeportal`, `eshop.ui.clientportal`.
*  Create the Docker Registry connection,  `docker-connection`, with the following settings: 
   * Registry type: Docker Hub
   * Docker ID: Your Docker Hub registry username
   * Docker Password: Create an access token with Read, Write, and Delete access permissions in your Docker Hub account settings.

*  Create the required Azure resources:
      ```pwsh
      $subscriptionId =  $(az account show --query id --output tsv)
      $resourceGroupsLocation="westeurope"
      $appResourceGroupName = "eshop-docker"
      $umiResourceGroupName="umi-rg"
      $userAssignedIdentityName="sql-directory-reader-identity"
      $adoSPName = "ado-eshop-infrastructure-spn"

      # Create the resource group and the SQL Server user-assigned managed identity.  

      az group create --name $umiResourceGroupName --location $resourceGroupsLocation

      az identity create --name $userAssignedIdentityName --resource-group $umiResourceGroupName --location $resourceGroupsLocation

      # Create the application resource group and the SP for Azure DevOps. 

      az group create --name $appResourceGroupName --location $resourceGroupsLocation

      $appRegistration = $(az ad sp create-for-rbac --name $adoSPName --role owner --scopes /subscriptions/$subscriptionId/resourceGroups/$appResourceGroupName)

      $appId = $(az ad sp list --display-name $adoSPName --query '[0].appId')

      az role assignment create --assignee $appId --role 'Managed Identity Operator' --scope /subscriptions/$subscriptionId/resourceGroups/$umiResourceGroupName

      az role assignment create --assignee  $appId --role Reader --scope /subscriptions/$subscriptionId

      ```
*  Create an Azure Resource Manager service connection with the name `azure-docker-infrastructure-spn`. Specify the connection type as: `Azure Resource Manager using service principal (manual)`. To set up the connection properties, execute the following script using the terminal from the previous step:
      ```pwsh
      az account show --query id --output tsv   #subscription id
      az account show --query name --output tsv #subscription name
      echo $appRegistration
      ```
*  Create an Azure Resource Manager service connection with the name `azure-docker-spn`. In the **New Azure service connection** pane, select `eshop-docker` as the Resource Group parameter value.
*  Add the Directory Reader role to the user-assigned identity `sql-directory-reader-identity` in the Azure portal.
*  Create a variable group with the name `microservices-variable-group` in the ADO project with the following variables:
   * ado-service-principal-id 
      ```pwsh
      echo $appId
      ```
   * ado-service-principal-object-id 
      ```pwsh
      az ad sp list --display-name $adoSPName --query '[0].id'
      ```
   * docker-registry-read-token - create an access token with Read-only access permissions in your Docker Hub account settings (marked as secret).
   * docker-registry-username - set the Docker Hub registry username
   * sql-umi-id
      ```pwsh
      echo /subscriptions/$subscriptionId/resourcegroups/$umiResourceGroupName/providers/Microsoft.ManagedIdentity/userAssignedIdentities/$userAssignedIdentityName
      ```
   * subscription-id
      ```pwsh
      echo $subscriptionId
      ```

## 3. AKS-Based pipelines:

### Description:

1) `eshop-on-microservices-ci-aks.yml` - run tests, build the WebApps Docker Images, push images to the ACR, and create a Helm package.
2) `eshop-on-microservices-cd-aks-infrastructure.yml` - deploy Azure resources using Bicep artifacts from the CI pipeline, and set Microsoft Entra ID authorization for the WebApps to access the SQL Servers. The Azure resources include AKS, SQL Servers, Azure Key Vault, and user-managed identities.  
3) `eshop-on-microservices-cd-aks.yml` - deploy the Helm package using artifacts from the CI pipeline to AKS.

### Setup:

*  Create pipelines in the ADO project. The name of each pipeline should match the file name without its extension.
*  Create a globally unique ACR name:
      ```pwsh
      $acrName = Read-Host "Enter a unique ACR Name"
      ```

*  Create the required Azure resources:
      ```pwsh
      
      $subscriptionId =  $(az account show --query id --output tsv)
      $appResourceGroupLocation= "eastus"
      $umiResourceGroupLocation= "westeurope"
      $appResourceGroupName = "eshop-aks"
      $umiResourceGroupName="umi-rg"
      $userAssignedIdentityName="sql-directory-reader-identity"
      $adoInfraSPName = "ado-eshop-aks-infrastructure-spn"
      $adoHelmDeploySPName = "ado-eshop-helm-spn"

      # Create the application resource group and the ACR

      az group create --name $appResourceGroupName --location $appResourceGroupLocation
      
      az acr create --name $acrName `
                  --resource-group $appResourceGroupName `
                  --sku Basic
      
      $acrLoginServer = $(az acr show --name $acrName --resource-group $appResourceGroupName  --query loginServer -o tsv)
      

      # Create the resource group and the SQL Server user-assigned managed identity.  

      az group create --name $umiResourceGroupName --location $umiResourceGroupLocation

      az identity create --name $userAssignedIdentityName --resource-group $umiResourceGroupName --location $umiResourceGroupLocation      

      # Create service principals for Azure DevOps:
      # SP for infrastructure deployment
      
      $appRegistration = $(az ad sp create-for-rbac --name $adoInfraSPName --role owner --scopes /subscriptions/$subscriptionId/resourceGroups/$appResourceGroupName)

      $appId = $(az ad sp list --display-name $adoInfraSPName --query '[0].appId')

      az role assignment create --assignee $appId --role 'Managed Identity Operator' --scope /subscriptions/$subscriptionId/resourceGroups/$umiResourceGroupName

      az role assignment create --assignee  $appId --role Reader --scope /subscriptions/$subscriptionId

      # SP for deployment a Helm Chart to the AKS Cluster

      $appHelmRegistration = $(az ad sp create-for-rbac --name $adoHelmDeploySPName --role contributor --scopes /subscriptions/$subscriptionId/resourceGroups/$appResourceGroupName)
      
      ```
*  Create the Docker Registry connection,  `acr-docker-connection`, with the following settings: 
   * Registry type: Azure Container Registry
   * Azure container registry: the name of the ACR registry from the previous step.

*  Create an Azure Resource Manager service connection with the name `azure-aks-infrastructure-spn`. Specify the connection type as: `Azure Resource Manager using service principal (manual)`. To set up the connection properties, execute the following script:
      ```pwsh
      az account show --query id --output tsv   #subscription id
      az account show --query name --output tsv #subscription name
      echo $appRegistration
      ```
*  Create an Azure Resource Manager service connection with the name `azure-aks-spn`. Specify the connection type as: `Azure Resource Manager using service principal (manual)`. To set up the connection properties, execute the following script:
      ```pwsh
      az account show --query id --output tsv   #subscription id
      az account show --query name --output tsv #subscription name
      echo $appHelmRegistration
      ```
*  Add the Directory Reader Role to the user assigned identity `sql-directory-reader-identity` in the azure portal.
*  In the Variables section of the **eshop-on-microservices-cd-aks** pipeline, create the `acr-name` variable and set its value to the ACR name from the second step.
*  Create Variable Group with the name `microservices-aks-variable-group` in the ADO project with the following variables:
   * acr-name
      ```pwsh
      echo $acrName
      ```
   * ado-service-principal-id 
      ```pwsh
      echo $appId
      ```
   * ado-service-principal-object-id 
      ```pwsh
      az ad sp list --display-name $adoInfraSPName --query '[0].id'
      ```
   * sql-umi-id
      ```pwsh
      echo /subscriptions/$subscriptionId/resourcegroups/$umiResourceGroupName/providers/Microsoft.ManagedIdentity/userAssignedIdentities/$userAssignedIdentityName
      ```
   * subscription-id
      ```pwsh
      echo $subscriptionId
      ```


 

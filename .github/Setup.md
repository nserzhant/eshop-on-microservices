# Actions Description:

## Single-Service Actions:

*  `employee-portal-ci-cd.yml` - build and deploy the Angular app (Employee Portal) to the static Azure Web App.

*  `client-portal-ci-cd-docker.yml` - build and deploy the Angular app (Client Portal) as a Docker image to the Azure App Service. A Docker Hub registry is required.

*  `catalog-api-ci-cd.yml` - build, test, and deploy the Catalog API to the Azure App Service. A Windows agent is used.

*  `client-authserver-ci-cd.yml` - build and deploy the Client Authorization Server to Azure App Service. An Ubuntu agent is used.

*  `employee-management-ci-cd.yml` - build, test, and deploy the Employee Authorization Server and Employee Management API to Azure App Service. A container job and a service container are used.

## Actions For Building And Deploying All Services Grouped By The Deployment Type

>Actions should be executed in the sequence in which they are listed in the description.

### 1. Zip-Deployment-Based Actions:

1) `eshop-on-microservices-ci.yml` - build and test. 
2) `eshop-on-microservices-cd.yml` - deploy Azure resources using Bicep artifacts and deploy Web Apps using zip artifacts from the CI action. The Azure resources include Linux App Service, SQL Server, Static Apps, and a Storage Account.

### 2. Docker Image-Based Actions:

1) `eshop-on-microservices-ci-docker.yml` - run tests, build the WebApps Docker images, and push the images to the Docker Hub registry.
2) `eshop-on-microservices-cd-docker-infrastructure.yml` - Deploy Azure resources using Bicep artifacts from the CI action, and set Microsoft Entra ID authorization for the WebApps to access the SQL Servers. The Azure resources include Linux App Service, SQL Server, Storage Account, and VNet.
3) `eshop-on-microservices-cd-docker.yml` - deploy WebApp Docker images from the CI action to App Services.

### 3. AKS-Based actions:

1) `eshop-on-microservices-ci-aks.yml` - run tests, build the WebApps Docker Images, push images to the ACR, and create a Helm package.
2) `eshop-on-microservices-cd-aks-infrastructure.yml` - deploy Azure resources using Bicep artifacts from the CI action, and set Microsoft Entra ID authorization for the WebApps to access the SQL Servers. The Azure resources include AKS, SQL Servers, Azure Key Vault, and user-managed identities.  
3) `eshop-on-microservices-cd-aks.yml` - deploy the Helm package using artifacts from the CI action to AKS.

# Setup:
*  Settings -> Acions -> General -> Workflow permissions Group -> Check `Read and write permissions`  -> Save
*  Create repositories in the Docker Hub registry (Required for Docker Image-Based Actions): `eshop.catalog.api`, `eshop.employeemanagement.api`, `eshop.employeemanagement.authorizationserver`, `eshop.client.authorizationserver`, `eshop.ui.employeeportal`, `eshop.ui.clientportal`.
*  Create a globally unique ACR name:
      ```pwsh
      $acrName = Read-Host "Enter a unique ACR Name"
      ```
*  Create the required Azure resources:
      ```pwsh
      $subscriptionId =  $(az account show --query id --output tsv)

      $defaultResourceGroupsLocation   = "westeurope"      
      $aksResourceGroupLocation        = "centralus"

      $zipDeploymentResourcesRGName    = "eshop"
      $dockerDeploymentResourcesRGName = "eshop-docker"
      $aksResourcesRGName              = "eshop-aks"
      $umiRGName                       = "umi-rg"

      $userAssignedIdentityName="sql-directory-reader-identity"

      $appsDeploymentSPName = "actions-apps-deploy-sp"
      $infrastructureDeploymentSPName = "actions-infra-deploy-sp"

      # Create Resource Groups

      az group create --name $zipDeploymentResourcesRGName --location $defaultResourceGroupsLocation
      az group create --name $dockerDeploymentResourcesRGName --location $defaultResourceGroupsLocation
      az group create --name $aksResourcesRGName --location $aksResourceGroupLocation
      az group create --name $umiRGName --location $defaultResourceGroupsLocation

      # Create ACR

      az acr create --name $acrName `
                    --resource-group $aksResourcesRGName `
                    --sku Basic

      $acrLoginServer = $(az acr show --name $acrName --resource-group $aksResourcesRGName  --query loginServer -o tsv)

      $registryId = $(az acr show --name $acrName --resource-group $aksResourcesRGName --query id --output tsv)

      # Create the SQL Server user-assigned managed identity

      az identity create --name $userAssignedIdentityName --resource-group $umiRGName --location $defaultResourceGroupsLocation
      
      # Create service principal for application components deployment

      $appRegistration = $(az ad sp create-for-rbac --name $appsDeploymentSPName  --json-auth)

      $appId = $(az ad sp list --display-name $appsDeploymentSPName --query '[0].appId')

      az role assignment create --assignee  $appId --role Contributor --scope /subscriptions/$subscriptionId/resourceGroups/$zipDeploymentResourcesRGName
      az role assignment create --assignee  $appId --role Contributor --scope /subscriptions/$subscriptionId/resourceGroups/$dockerDeploymentResourcesRGName
      az role assignment create --assignee  $appId --role Contributor --scope /subscriptions/$subscriptionId/resourceGroups/$aksResourcesRGName
      az role assignment create --assignee  $appId --role AcrPush --scope $registryId
     
      # Create service principal for infrastructure deployment

      $infraAppRegistration = $(az ad sp create-for-rbac --name $infrastructureDeploymentSPName  --json-auth)

      $infraDeploymentAppId = $(az ad sp list --display-name $infrastructureDeploymentSPName --query '[0].appId')
      
      az role assignment create --assignee  $infraDeploymentAppId --role Owner --scope /subscriptions/$subscriptionId/resourceGroups/$zipDeploymentResourcesRGName
      az role assignment create --assignee  $infraDeploymentAppId --role Owner --scope /subscriptions/$subscriptionId/resourceGroups/$dockerDeploymentResourcesRGName
      az role assignment create --assignee  $infraDeploymentAppId --role Owner --scope /subscriptions/$subscriptionId/resourceGroups/$aksResourcesRGName
      az role assignment create --assignee  $infraDeploymentAppId --role 'Managed Identity Operator' --scope /subscriptions/$subscriptionId/resourceGroups/$umiRGName 
      az role assignment create --assignee  $infraDeploymentAppId --role Reader --scope /subscriptions/$subscriptionId

      ```

*  Create repository variables:
    * ACR_NAME
      ```pwsh
      echo $acrName
      ```
    * DOCKERHUB_USERNAME - set the Docker Hub registry username
    * SUFFIX - suffix is appended to resource names that must be globally unique. Enter your own value, or use the following script to generate a random value:
      ```pwsh
      openssl rand -hex 4
      ```

*  Create repository secrets:
    * AZURE_CREDENTIALS
        ```pwsh
        echo $appRegistration
        ```
    *  AZURE_SUBSCRIPTION
        ```pwsh
        echo $subscriptionId
        ```   
    *  SQL_DB_PASSWORD - set the administrator password for SQL Databases.
    *  DOCKERHUB_TOKEN - create an access token with Read, Write, and Delete access permissions in your Docker Hub account settings.
    *  AZURE_INFRASTRUCTURE_DEPLOYMENT_CREDENTIALS
        ```pwsh
        echo $infraAppRegistration
        ```
    *  AZURE_SP_APP_ID
        ```pwsh
        echo  $infraDeploymentAppId
        ```
    *  AZURE_SP_OBJECT_ID
        ```pwsh
        az ad sp list --display-name $infrastructureDeploymentSPName --query '[0].id'
        ```
    *  SQL_DIRECTORY_READER_UMI
        ```pwsh
        echo /subscriptions/$subscriptionId/resourcegroups/$umiRGName/providers/Microsoft.ManagedIdentity/userAssignedIdentities/$userAssignedIdentityName
        ```
*  Add the Directory Reader Role to the user assigned identity `sql-directory-reader-identity` in the azure portal.
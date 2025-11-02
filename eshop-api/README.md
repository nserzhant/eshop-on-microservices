# Working with the eshop-on-microservices backend implementation.

## Starting microservices:

Use the following projects as startup projects to run microservices:

|           Microservice                            |       Startup Project                         |
|---------------------------------------------------|-----------------------------------------------|
|     Basket                                        |  EShop.Basket.Api                             |
|     Catalog                                       |  EShop.Catalog.Api                            |
|     Customer Authorization Server                 |  EShop.Customer.AuthorizationServer           |
|     EmployeeManagement Api                        |  EShop.EmployeeManagement.Api                 |
|     EmployeeManagement Authorization Server       |  EShop.EmployeeManagement.AuthorizationServer |
|     Ordering                                      |  EShop.Ordering.Api                           |
|     Payment                                       |  EShop.Payment.Processor                      |
|     Saga                                          |  EShop.Saga.Components                        |


Some endpoints require identity servers for authorization. By default, services use local identity servers:

|           Microservice      |       Required Identity Server                                           |
| --------------------------- | -------------------------------------------------------------------------|
| Basket                      | Customer Authorization Server                                            |
| Catalog                     | Customer Authorization Server, EmployeeManagement Authorization Server   |
| EmployeeManagement Api      | EmployeeManagement Authorization Server                                  |
| Ordering                    | Customer Authorization Server                                            |

 Additionally, external identity providers that support OAuth 2.0 can be used. To use an external provider, modify the JWTSettings section in the `appsettings.development.json` or `lunchsettings.json` file if you start services using Docker.

### Docker support 

Each solution contains a docker-compose project to start services with all required dependencies in Docker. Additionally, the docker-compose projects include configuration to start only dependencies (SQL Server, Rabbit MQ Server, Redis Cache), without starting the services themselves.

## NSwag tool

Angular clients are generated at build time and placed in the **Client** folder within the microservice projects. To update the client, rebuild the project and copy the file to the following folders of the Angular applications:

Catalog client: eshop-ui-employee-portal\src\app\catalog\services\api and eshop-ui-customer-portal\src\app\services\api
Basket client: eshop-ui-customer-portal\src\app\services\api
Ordering client: eshop-ui-customer-portal\src\app\services\api
EmployeeManagement client: eshop-ui-employee-portal\src\app\employee-management\services\api\employee.api.client.ts


## Tests:

  All microservices contain integration and unit tests. To run integration tests, MSSQL LocalDB is used. For running Saga integration tests, all instances of the microservices should be started using the docker-compose project with the "Saga integration tests" profile.

## Infrastructure:
 All components include samples of Bicep resources in the `.azure` folder and Azure DevOps pipelines in the `.ado` folder.
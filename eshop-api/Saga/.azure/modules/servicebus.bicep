@description('Location for all resources.')
param location string = resourceGroup().location

/*----------------------- Service Bus Parameters ---------------- */

@description('The Name of the Service Bus namespace')
param serviceBusNamespaceName string

@description('The Name Of The Service Bus SKU')
param serviceBusSKU string = 'Standard'

/*-----------------------  RESOURCES   ------------------------- */

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

@description('The name of our application.')
param applicationName string = uniqueString(resourceGroup().id)

@description('The location that our resources will be deployed to. Default is location of resource group')
param location string = resourceGroup().location

@description('Name of the App Service Plan')
param appServicePlanName string = 'asp-${applicationName}'

@description('Name of the Cosmos DB account that will be deployed')
param cosmosDBAccountName string = 'cosmos-${applicationName}'

@description('Name of the App Insights instance that will be deployed')
param appInsightsName string = 'appins-${applicationName}'

@description('The name of the Key Vault that will be deployed')
param keyVaultName string = 'kv-${applicationName}'

@description('The name of the log analytics workspace that will be deployed')
param logAnalyticsWorkspaceName string = 'law-${applicationName}'

@description('The name of the Service Bus Namespace that will be deployed')
param serviceBusNamespaceName string = 'sb-${applicationName}'

@description('The time that the resource was last deployed')
param lastDeployed string = utcNow()

var cosmosDBName = 'MyHealthTrackerDB'
var cosmosContainerName = 'Records'
var tags = {
  ApplicationName: 'HealthTracker'
  Component: 'CommonInfrastructure'
  Environment: 'Production'
  LastDeployed: lastDeployed
  Owner: 'Will Velida'
}

module appServicePlan 'modules/appServicePlan.bicep' = {
  name: 'asp'
  params: {
    appServicePlanName: appServicePlanName
    location: location
    tags: tags
  }
}

module logAnalytics 'modules/logAnalytics.bicep' = {
  name: 'law'
  params: {
    location: location 
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    tags: tags
  }
}

module appInsights 'modules/appInsights.bicep' = {
  name: 'appins'
  params: {
    appInsightsName: appInsightsName 
    logAnalyticsWorkspaceId: logAnalytics.outputs.logAnalyticsId
    tags: tags
    location: location
  }
}

module keyVault 'modules/keyVault.bicep' = {
  name: 'kv'
  params: {
    keyVaultName: keyVaultName 
    location: location
    tags: tags
  }
}

module serviceBus 'modules/serviceBus.bicep' = {
  name: 'sb'
  params: {
    serviceBusNamespaceName: serviceBusNamespaceName
    location: location
    tags: tags
  }
}

module cosmos 'modules/cosmosDb.bicep' = {
  name: 'cosmos'
  params: {
    containerName: cosmosContainerName 
    cosmosDBAccountName: cosmosDBAccountName
    databaseName: cosmosDBName
    location: location
    tags: tags
    logAnalyticsWorkspaceId: logAnalytics.outputs.logAnalyticsId
  }
}

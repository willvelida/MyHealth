@description('The name of the log analytics workspace that will be deployed')
param logAnalyticsWorkspaceName string

@description('The location that our resources will be deployed to. Default is location of resource group')
param location string

@description('The tags that will be applied to the Log Analytics workspace')
param tags object

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

output logAnalyticsId string = logAnalytics.id

@description('Name of the App Insights instance that will be deployed')
param appInsightsName string

@description('The location that our App Insights instance will be deployed to')
param location string = resourceGroup().location

@description('The Log Analytics workspace Id to connect this App Insights to')
param logAnalyticsWorkspaceId string

@description('The tags that will be applied to the App Insights instance')
param tags object

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  tags: tags
  properties: {
    Application_Type: 'web'
    ImmediatePurgeDataOn30Days: true
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output appInsightsConnectionString string = appInsights.properties.ConnectionString
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey

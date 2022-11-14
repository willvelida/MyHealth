@description('Name of the Cosmos DB account that will be deployed')
param cosmosDBAccountName string

@description('The name of the database in this Cosmos DB account')
param databaseName string

@description('The name of the container in this Cosmos DB account')
param containerName string

@description('The location that our Cosmos DB resources will be deployed to')
param location string

@description('The Log Analytics workspace Id to connect this App Insights to')
param logAnalyticsWorkspaceId string

@description('The tags that will be applied to the Cosmos DB account')
param tags object

var retentionPolicy = {
  days: 30
  enabled: true 
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2022-05-15' = {
  name: cosmosDBAccountName
  location: location
  tags: tags
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2022-05-15' = {
  name: databaseName
  parent: cosmosAccount
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2022-05-15' = {
  name: containerName
  parent: database
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          '/DocumentType'
        ]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: [
          {
            path: '/*'
          }
        ]
      }
    }
  }
}

resource cosmosDiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'cosmosDiagnosticSettings'
  scope: cosmosAccount
  properties: {
    logs: [
      {
        category: 'DataPlaneRequests'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'QueryRuntimeStatistics'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'PartitionKeyStatistics'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'PartitionKeyRUConsumption'
        enabled: true
        retentionPolicy: retentionPolicy
      }
      {
        category: 'ControlPlaneRequests'
        enabled: true
        retentionPolicy: retentionPolicy
      }
    ]
    metrics: [
      {
        category: 'Requests'
        enabled: true
        retentionPolicy: retentionPolicy
      }
    ]
    workspaceId: logAnalyticsWorkspaceId
  }
}

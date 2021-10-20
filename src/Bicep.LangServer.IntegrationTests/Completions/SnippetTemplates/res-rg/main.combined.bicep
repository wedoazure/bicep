// $1 = resourceGroup
// $2 = 'name'

targetScope = 'subscription'
param location string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'name'
  location: location
  tags: {
    'tag': 'tagValue'
  }
}
// Insert snippet here

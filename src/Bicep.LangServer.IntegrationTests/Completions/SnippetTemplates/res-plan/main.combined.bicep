// $1 = appServicePlan
// $2 = 'name'
// $3 = 'name'
// $4 = 1

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: location
  sku: {
    name: 'name'
    capacity: 1
  }
}
// Insert snippet here


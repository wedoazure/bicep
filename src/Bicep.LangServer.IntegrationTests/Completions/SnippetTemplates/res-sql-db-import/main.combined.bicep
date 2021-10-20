// $1 = 'sqlDatabase/import'
// $2 = sqlDatabaseImport
// $3 = StorageAccessKey
// $4 = 'storageKey'
// $5 = 'storageUri'
// $6 = 'administratorLogin'
// $7 = 'administratorLoginPassword'

param location string

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  name: 'sqlDatabase/import'
  location: location
}

resource sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
  parent: sqlServerDatabase
  name: 'import'
  properties: {
    storageKeyType: 'StorageAccessKey'
    storageKey: 'storageKey'
    storageUri: 'storageUri'
    administratorLogin: 'administratorLogin'
    administratorLoginPassword: 'administratorLoginPassword'
    operationMode: 'Import'
  }
}
// Insert snippet here

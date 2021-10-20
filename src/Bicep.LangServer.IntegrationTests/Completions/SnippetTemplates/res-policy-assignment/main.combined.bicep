// $1 = policyAssignment
// $2 = 'name'
// $3 = 'SystemAssigned'
// $4 = 'displayName'
// $5 = 'description'
// $6 = 'Default'
// $7 = 'source'
// $8 = '0.1.0'
// $9 = 'policyDefinitionId'
// $10 = parameterName
// $11 = 'value'
// $12 = 'message'
// $13 = 'message'
// $14 = 'policyDefinitionReferenceId'

param location string

resource policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: 'name'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    displayName: 'displayName'
    description: 'description'
    enforcementMode: 'Default'
    metadata: {
      source: 'source'
      version: '0.1.0'
    }
    policyDefinitionId: 'policyDefinitionId'
    parameters: {
      parameterName: {
        value: 'value'
      }
    }
    nonComplianceMessages: [
      {
        message: 'message'
      }
      {
        message: 'message'
        policyDefinitionReferenceId: 'policyDefinitionReferenceId'
      }
    ]
  }
}
// Insert snippet here

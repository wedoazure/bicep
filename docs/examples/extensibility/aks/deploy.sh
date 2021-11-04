#!/bin/bash

# to get az cli ready for demo (run these commands in the repo root):
#
# export BICEP_IMPORTS_ENABLED_EXPERIMENTAL=true
# export BICEP_SYMBOLIC_NAME_CODEGEN_EXPERIMENTAL=true
#
# git submodule update --init --recursive
# dotnet publish --configuration Release --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true -r osx-x64 ./src/Bicep.Cli/Bicep.Cli.csproj
# 
# mv ./src/Bicep.Cli/bin/Release/net5.0/osx-x64/publish/bicep ~/.azure/bin/bicep

baseName="bicepkubedemo"
adminUsername="anthony"

# end-to-end deployment
az deployment sub create \
  -f ./docs/examples/extensibility/aks/main.bicep \
  --location 'West Central US' \
  --name $baseName \
  --parameters \
    baseName=$baseName \
    dnsPrefix=$baseName \
    linuxAdminUsername=$adminUsername \
    sshRSAPublicKey="$(cat ~/.ssh/id_rsa.pub)" \
  --query properties.outputs.webUrl

# deploy aks individually
az deployment group create \
  -f ./docs/examples/extensibility/aks/modules/aks.bicep \
  --resource-group $baseName \
  --parameters \
    baseName=$baseName \
    dnsPrefix=$baseName \
    linuxAdminUsername=$adminUsername \
    sshRSAPublicKey="$(cat ~/.ssh/id_rsa.pub)" \
  --query properties.outputs.kubeconfig

# deploy kubernetes individually
kubeConfig=<base64 encoded kubeconfig>
az deployment group create \
  -f ./docs/examples/extensibility/aks/modules/kubernetes.bicep \
  --resource-group $baseName \
  --parameters \
    kubeConfig=$kubeConfig
param(
    [Parameter(Mandatory = $false)]
    [string]$StackName = "exchange-api-dev",

    [Parameter(Mandatory = $false)]
    [string]$Region = "us-east-1",

    [Parameter(Mandatory = $false)]
    [string]$TemplateFile = ".\Infra\cloudformation\ecs-fargate.yaml",

    [Parameter(Mandatory = $false)]
    [string]$ParametersFile = ".\Infra\cloudformation\parameters.dev.json"
)

$ErrorActionPreference = "Stop"

if (-not (Get-Command aws -ErrorAction SilentlyContinue)) {
    throw "AWS CLI nao encontrada no PATH."
}

if (-not (Test-Path $TemplateFile)) {
    throw "Template nao encontrado: $TemplateFile"
}

if (-not (Test-Path $ParametersFile)) {
    throw "Arquivo de parametros nao encontrado: $ParametersFile"
}

Write-Host "Validando template CloudFormation..."
aws cloudformation validate-template --template-body "file://$TemplateFile" --region $Region | Out-Null

$parameters = Get-Content $ParametersFile -Raw | ConvertFrom-Json
$parameterOverrides = @()

foreach ($p in $parameters) {
    $parameterOverrides += "$($p.ParameterKey)=$($p.ParameterValue)"
}

Write-Host "Criando/atualizando stack '$StackName' na regiao '$Region'..."
aws cloudformation deploy `
    --stack-name $StackName `
    --template-file $TemplateFile `
    --parameter-overrides $parameterOverrides `
    --capabilities CAPABILITY_NAMED_IAM `
    --region $Region

Write-Host "Deploy finalizado."
Write-Host "Recuperando outputs..."
aws cloudformation describe-stacks --stack-name $StackName --region $Region --query "Stacks[0].Outputs" --output table

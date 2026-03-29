# Infraestrutura como Codigo (AWS ECS + CloudFormation)

Esta pasta contem o provisionamento de infraestrutura para publicar a API no AWS ECS (Fargate) com Application Load Balancer.

## Estrutura

- `cloudformation/ecs-fargate.yaml`: template principal do CloudFormation.
- `cloudformation/parameters.dev.json`: exemplo de parametros para ambiente `dev`.
- `scripts/deploy.ps1`: script para validar e aplicar stack.

## Recursos provisionados

- VPC com 2 subnets publicas em AZs diferentes
- Internet Gateway e rota publica
- Security Groups separados para ALB e ECS
- Application Load Balancer com listener HTTP na porta 80
- Target Group com health check em `/health`
- ECS Cluster
- Task Definition Fargate
- ECS Service (1 task por padrao)
- CloudWatch Log Group para logs do container
- IAM Role de execucao da task e role da aplicacao

## Pre-requisitos

1. AWS CLI configurada com credenciais validas.
2. Imagem Docker da API publicada no ECR.
3. Ajustar o arquivo `parameters.dev.json` com a imagem real e JWT.

## Como usar

No diretorio raiz do projeto:

```powershell
.\Infra\scripts\deploy.ps1 -StackName exchange-api-dev -Region us-east-1
```

## Deploy sem script (opcional)

```powershell
aws cloudformation validate-template --template-body file://Infra/cloudformation/ecs-fargate.yaml --region us-east-1

aws cloudformation deploy \
  --stack-name exchange-api-dev \
  --template-file Infra/cloudformation/ecs-fargate.yaml \
  --parameter-overrides \
    ProjectName=exchange-api \
    EnvironmentName=dev \
    ContainerImage=123456789012.dkr.ecr.us-east-1.amazonaws.com/exchange-api:latest \
    JwtKey=<chave-jwt-segura> \
  --capabilities CAPABILITY_NAMED_IAM \
  --region us-east-1
```

## Observacoes

- O template usa `AssignPublicIp: ENABLED` para simplificar ambientes de teste.
- Para producao, o recomendado e migrar tasks para subnet privada com NAT Gateway e HTTPS (ACM + listener 443).
- Este provisionamento nao altera regra de negocio da API.

# 🚀 **Exchange.API — API de Câmbio em Tempo Real**

## 📖 Descrição

API RESTful desenvolvida em .NET para conversão de moedas em tempo real, construída com **Clean Architecture** e **Domain-Driven Design (DDD)**. Utiliza **Use Cases** para garantir o isolamento das regras de negócio e o desacoplamento total entre as camadas, facilitando manutenção, testes e escalabilidade.

A API suporta:

* 💱 Conversão de valores entre diferentes moedas usando taxas de câmbio do Banco central brasileiro.
* 📜 Histórico de conversões realizadas para consulta com cachê para performance.
* 🔧 Extensibilidade para futuras integrações com  APIs de câmbio de outras instituições, agendamento de conversões e notificações.

---

## 📁 Estrutura do Projeto

```
Exchange.sln
├── Exchange.API/                # API REST, controllers, middleware e configuração
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
├── Exchange.Application/        # Casos de uso, interfaces e lógica de aplicação
│   ├── Interfaces/
    ├── Dtos/
│   └── UseCases/
├── Exchange.Domain/             # Entidades do domínio, interfaces e regras de negócio puras
│   ├── Entities/
│   └── Interfaces/
├── Exchange.Infrastructure/     # Implementações dos repositórios, serviços externos e persistência
│   ├── Repositories/
│   └── Services/
└── Exchange.Unit.Test/          # Implementações dos testes unitarios
    ├── Application/
    └── API/
```

---

## 🛠️ Tecnologias Utilizadas

* .NET 8
* Clean Architecture
* Domain-Driven Design (DDD)
* ASP.NET Core Web API
* Memory cachê
* Injeção de Dependência
* Middlewares para tratamento global de erros

---

## ▶️ Como Rodar

1. 🔽 Clone o repositório
2. 🛠️ Abra a solução `Exchange.sln` no Visual Studio ou VS Code
3. 📦 Restaure as dependências e compile o projeto
4. ▶️ Execute o projeto `Exchange.API`
   No VS Code/terminal, você pode iniciar com:
   ```bash
   dotnet run --project Exchange.API\Exchange.API.csproj --launch-profile Exchange.API
   ```
5. 🌐 Acesse a documentação Swagger em `https://localhost:{porta}/swagger` (se configurado)
6. 💸 Use o endpoint `POST /api/currency/convert` para realizar conversões

---

## 🔍 Endpoints Principais

| Método | Endpoint | Descrição |
| ------ | -------- | --------- |
| POST | `/api/currency/convert` | Converte um valor de BRL para outra moeda. |
| GET | `/api/currency/history` | Retorna o histórico de conversões com filtros e paginação. |
| GET | `/api/currency/history/{id}` | Retorna os detalhes de uma conversão específica. |
| GET | `/api/currency/rate` | Consulta cotação de compra/venda por moeda e data. |
| GET | `/api/currency/supported` | Lista as moedas suportadas pela API. |
| POST | `/api/authentication/token` | Gera um token JWT para autenticação usando `client_id` e `secret` (via header). |

---


## 📄 Exemplo de Requisição

**POST** `/api/authentication/token`  
**Headers:**

```
client_id: 3f29b6e7-1c4b-4f9a-b8b4-2f5e2f4d5c6a
secret: f8d9a7b6-2c3e-4f7a-8b1d-3e2f4a5b6c7d
```

**Resposta esperada (resumo):**

```json
{
  "success": true,
  "data": {
    "accessToken": "jwt-token",
    "expiresAt": "2026-03-07T15:00:00Z"
  },
  "error": null,
}
```

---

**POST** `/api/currency/convert`
**Headers:**

```
Authorization: Bearer {{access_token}}
Content-Type: application/json
```

**Body:**

```json
{
  "toCurrency": "EUR",
  "amountBRL": 1000,
  "dateQuotation": "2025-08-13",
  "exchangeType": 1
}
```

---

## 📈 Exemplo de Resposta

```json
{
  "success": true,
  "data": {
    "originalAmount": 1000,
    "fromCurrency": "BRL",
    "convertedAmount": 158.75,
    "toCurrency": "EUR",
    "exchangeRate": 6.30,
    "exchangeType": 1,
    "dateQuotation": "2025-08-13",
    "provider": "BACEN"
  },
  "error": null,
}
```

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "O valor deve ser maior que zero.",
    "details": null
  },
}
```

---

💡 Observações:

1. `Authorization: Bearer {{access_token}}` → o token deve ser obtido no endpoint de autenticação (`/api/authentication/token`).
2. `Content-Type: application/json` → necessário para que a API interprete corretamente o JSON.
3. `exchangeType` → pode ser usado para diferenciar tipos de câmbio (ex.: comercial, turismo).

---
## 🔑 Utilizando a API com ClientId e Secret de Teste

Para testar a API, você pode usar os seguintes valores fixos para se autenticar:

* **client_id:** `3f29b6e7-1c4b-4f9a-b8b4-2f5e2f4d5c6a`
* **secret:** `f8d9a7b6-2c3e-4f7a-8b1d-3e2f4a5b6c7d`

---

## ⚠️ Tratamento de Erros com Middleware global

* 🚫 Valores inválidos (ex.: argumentos incorretos) resultam em resposta HTTP **400 Bad Request**, com mensagens claras para facilitar o entendimento do problema.
* ❌ Erros inesperados ou internos são capturados globalmente por um **middleware de tratamento de exceções**, que garante o retorno de uma resposta HTTP **500 Internal Server Error** padronizada e evita vazamento de detalhes sensíveis.
* 💡 Esse middleware centraliza o tratamento de erros, simplificando o código dos controllers e melhorando a manutenção da aplicação.

---

## 🚀 Implantação no AWS ECS

A aplicação foi implantada com sucesso no **AWS ECS Fargate** e está disponível através do **ALB (Application Load Balancer)**.

### Infraestrutura como código (CloudFormation)

Foi adicionada a pasta `Infra/` na raiz do projeto com provisionamento via CloudFormation para ECS Fargate:

- `Infra/cloudformation/ecs-fargate.yaml`: template principal de infraestrutura.
- `Infra/cloudformation/parameters.dev.json`: parâmetros de exemplo para ambiente dev.
- `Infra/scripts/deploy.ps1`: script para validar e aplicar stack.

Comando de deploy:

```powershell
.\Infra\scripts\deploy.ps1 -StackName exchange-api-dev -Region us-east-1
```

Workflow de deploy no GitHub Actions:
- Arquivo: `.github/workflows/deploy-ecs-cloudformation.yml`
- Execucao manual via `workflow_dispatch` (nao executa automaticamente em push)

### 🌐 Endpoint
Você pode acessar o endpoint de autenticação pelo link abaixo:


[http://alb-exchange-1526545477.us-east-1.elb.amazonaws.com/api/authentication/token](http://alb-exchange-1526545477.us-east-1.elb.amazonaws.com/api/authentication/token)


### 📡 Exemplo de Requisição `POST` com `curl`

```bash
curl --location --request POST 'http://alb-exchange-1526545477.us-east-1.elb.amazonaws.com/api/authentication/token' \
--header 'client_id: 3f29b6e7-1c4b-4f9a-b8b4-2f5e2f4d5c6a' \
--header 'secret: f8d9a7b6-2c3e-4f7a-8b1d-3e2f4a5b6c7d'
````

### ✅ Passos realizados para a implantação

1. 🔹 Build da imagem Docker localmente.
2. 🔹 Push da imagem para o **ECR (Elastic Container Registry)**.
3. 🔹 Configuração da **Task Definition** no ECS.
4. 🔹 Criação do **Service** com integração ao **ALB**.
5. 🔹 Testes e validação do endpoint.

> Agora a API está rodando na nuvem com alta disponibilidade e escalabilidade! 🎉


### 🚀 Próximos Passos

* [x] ✅🔗 **Integrar API oficial do Banco Central do Brasil (Bacen)** para obter taxas de câmbio oficiais e atualizadas (DONE).  
Fonte:
[Bacen - Taxas de Câmbio - Dados Abertos](https://dadosabertos.bcb.gov.br/dataset/taxas-de-cambio-todos-os-boletins-diarios/resource/61318ccb-db9d-4d6c-87f5-d8013af7a401?inner_span=True)  
Exemplo: [Bacen - Exemplo de busca](https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/aplicacao#!/recursos/CotacaoMoedaDia#eyJmb3JtdWxhcmlvIjp7IiRmb3JtYXQiOiJqc29uIiwiJHRvcCI6MSwibW9lZGEiOiJFVVIiLCJkYXRhQ290YWNhbyI6IjA4LTE1LTIwMjUifSwicGVzcXVpc2FkbyI6dHJ1ZSwiYWN0aXZlVGFiIjoiZGFkb3MiLCJncmlkU3RhdGUiOnsDMAM6W3sDQgMiBDAEIiwDQQN9LHsDQgMiBDEEIiwDQQN9LHsDQgMiBDIEIiwDQQN9LHsDQgMiBDMEIiwDQQN9LHsDQgMiBDQEIiwDQQN9LHsDQgMiBDUEIiwDQQN9XSwDMQM6e30sAzIDOltdLAMzAzp7fSwDNAM6e30sAzUDOnt9fSwicGl2b3RPcHRpb25zIjp7A2EDOnt9LANiAzpbXSwDYwM6NTAwLANkAzpbXSwDZQM6W10sA2YDOltdLANnAzoia2V5X2FfdG9feiIsA2gDOiJrZXlfYV90b196IiwDaQM6e30sA2oDOnt9LANrAzo4NSwDbAM6ZmFsc2UsA20DOnt9LANuAzp7fSwDbwM6IkNvbnRhZ2VtIiwDcAM6IlRhYmxlIn19)
* [x] 🔐✅ **Implementar autenticação e autorização** (DONE).
* [x] 🧪✅ **Adicionar testes automatizados** (DONE).
* [x] 💱✅ **Evoluir rotas de câmbio** com `rate`, `supported` e `history/{id}` (DONE).
* [x] 📜✅ **Adicionar filtros e paginação no histórico** de conversões (DONE).
* [x] 📊✅ **Ampliar cobertura de testes unitários** com relatório de cobertura (DONE).
* [x] 🌐✅ **Buscar moedas suportadas dinamicamente no Bacen** (DONE).
* [x] 🧩✅ **Adicionar Result Pattern ao projeto** (DONE).
* [x] 📦✅ **Padronizar response envelope REST (`success/data/error`)** (DONE).
* [x] ☁️🚀 **Implantar na AWS**
* [x] ☁️🚀 **Criar Infraestrutura como codigo com CloudFormation**
* [ ] ⏰ **Adicionar agendamento de conversões** com notificação quando taxa atingir determinado valor.
* [ ] 🧪 **Adicionar testes de integração** 
### ***Indicadores de Conclusão***
 * [ ] = tarefa pendente.  
 * [x] = tarefa concluída

---

### ✨ Made with ❤️ and ☕ by Diego Fernandes Lins ✨

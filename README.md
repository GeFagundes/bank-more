🏦 BankMore
Sistema de gestão bancária modular desenvolvido em .NET 10, utilizando arquitetura de microserviços, mensageria com Kafka e persistência de dados performática.

🏗️ Arquitetura do Projeto
O projeto segue os princípios de Clean Architecture e está dividido em contextos delimitados dentro da pasta src/Services:

Account: Gestão de conta corrente, saldo e extrato.

Transfer: Orquestração de transferências entre contas.

Fees: Worker responsável pelo processamento de tarifas via consumo de eventos.

Tecnologias Utilizadas
Runtime: .NET 10

ORM/Data: Dapper (Infra)

Mensageria: Apache Kafka

Containerização: Docker & Docker Compose

Padrões: MediatR, Repository Pattern, Domain-Driven Design (DDD).

📂 Estrutura de Pastas
Plaintext
src/
 └── Services/
     ├── Account/      # API, Application, Domain e Infra de Contas
     ├── Transfer/     # API, Application, Domain e Infra de Transferências
     └── Fees/         # Worker de tarifação
tests/                 # Testes unitários e de integração
docker-compose.yml     # Orquestração de serviços (Kafka, DB, APIs)
🚀 Como Executar
Pré-requisitos
.NET 10 SDK

Docker Desktop

Passo a Passo
Clonar o repositório:

Bash
git clone https://github.com/seu-usuario/bank-more.git
cd bank-more
Subir o ambiente (Banco de dados e Mensageria):

Bash
docker-compose up -d
Restaurar dependências:

Bash
dotnet restore
Executar uma API específica (Exemplo: Account):

Bash
dotnet run --project src/Services/Account/Account.Api/Account.Api.csproj
🧪 Testes
Para rodar todos os testes do sistema:

Bash
dotnet test
🛠️ Roadmap de Desenvolvimento
[x] Estruturação dos Microserviços

[ ] Implementação do fluxo de Transferência

[ ] Integração com Kafka para o Worker de Fees

[ ] Implementação de Testes de Integração com Testcontainers
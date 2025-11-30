<img width="150" height="150" alt="SP-Studio" src="https://github.com/user-attachments/assets/30d9acf0-007e-44c4-890c-71fea32fba19" />  

# 🏦 BankDevTrail API 

API de um Banco Digital completa desenvolvida em .NET 8, simulando operações financeiras reais, arquitetura robusta e integração com Inteligência Artificial.

## 🚀 Funcionalidades

- **Gestão de Contas:** Cadastro de clientes e contas (Corrente/Poupança).
- **Operações Financeiras:** Depósito, Saque e Transferência entre contas.
- **Ledger Imutável:** Histórico de transações seguro e rastreável.
- **Segurança:** Autenticação e Autorização via JWT (JSON Web Tokens).
- **Consultor Financeiro IA:** Integração com **Azure OpenAI** para análise de extrato e dicas financeiras.
- **Soft Delete:** Exclusão lógica de clientes e contas para auditoria.

## 🛠️ Tecnologias Utilizadas

- **.NET 8** (C#)
- **Entity Framework Core** (ORM)
- **SQL Server** (Banco de Dados)
- **Azure OpenAI** (IA Generativa)
- **Swagger/OpenAPI** (Documentação)
- **xUnit & Moq** (Testes Unitários)
- **Arquitetura em Camadas** (Controller, Service, Repository)

## ⚙️ Pré-requisitos

Para rodar este projeto localmente, você precisará de:

1.  [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.
2.  [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (ou Docker image).
4.  Uma conta na **Azure** (opcional, apenas para a funcionalidade de IA).

## 🔧 Configuração e Instalação

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/felipedsanta/banksystemapi.git
    cd banksystemapi
    ```

2.  **Configuração de Segredos (Importante!):**
    O projeto utiliza dados sensíveis (Chaves de API, JWT). Você não deve colocar isso diretamente no código.
    
    Na pasta `BankSystem.Api`, execute os comandos abaixo substituindo pelos seus valores, ou crie um arquivo `appsettings.Development.json` com essas chaves:

    ```bash
    # Chave para assinar os Tokens (invente uma frase longa)
    dotnet user-secrets set
    dotnet user-secrets set "Jwt:Key" "MINHA_CHAVE_SUPER_SECRETA_LOCAL_123"

    # (Opcional) Se for testar a IA
    dotnet user-secrets set "AzureOpenAI:Endpoint" "[https://SEU-RESOURCE.openai.azure.com/](https://SEU-RESOURCE.openai.azure.com/)"
    dotnet user-secrets set "AzureOpenAI:ApiKey" "SUA_CHAVE_AZURE"
    dotnet user-secrets set "AzureOpenAI:DeploymentName" "NOME_DO_MODELO"
    ```

3.  **Banco de Dados:**
    Certifique-se que sua Connection String no `appsettings.json` aponta para seu SQL Server local. Depois, rode as migrations:

    ```bash
    cd BankSystem.Api
    dotnet ef database update
    ```
    *Isso criará o banco `BankSystemDb` e todas as tabelas automaticamente.*

4.  **Popular o Banco (Seed):**
    Ao rodar a API pela primeira vez, ela criará automaticamente um usuário **Admin** e alguns clientes de teste.

## ▶️ Como Rodar

1.  Inicie a API:
    ```bash
    dotnet run --project BankSystem.Api
    ```
2.  Acesse o **Swagger** para testar os endpoints:
    👉 `https://localhost:7092/swagger` (ou a porta indicada no seu terminal).

## 🧪 Testando o Fluxo

1.  **Login:** Use o endpoint `/api/auth/login` com:
    * Email: `admin@bank.com`
    * Senha: `123456Ff!`
2.  **Authorize:** Copie o Token gerado, clique no cadeado 🔓 no topo do Swagger e digite: `Bearer SEU_TOKEN`.
3.  **Operações:** Agora você pode criar contas, depositar e transferir dinheiro!

---
Desenvolvido por **Felipe** como parte do desafio de DevTrail.

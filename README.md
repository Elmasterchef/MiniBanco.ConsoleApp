# MiniBanco.ConsoleApp

Mini-projeto avaliativo desenvolvido em C# com .NET 10, utilizando programação orientada a objetos para simular operações bancárias e analisar a saúde financeira de contas.

## Objetivo

Implementar uma aplicação de console que permita cadastrar diferentes modalidades de contas bancárias, registrar movimentações, executar fechamentos mensais e gerar relatórios com indicadores e recomendações financeiras.

## Tecnologias

- C#
- .NET 10
- PostgreSQL 17
- Npgsql 10.0.3
- Docker Compose
- xUnit para testes automatizados
- Git e GitHub

## Funcionalidades

- Cadastro de contas correntes, poupanças e contas salário.
- Registro de depósitos e saques.
- Controle de saldo e limite de cheque especial.
- Aplicação de tarifas e rendimentos no fechamento mensal.
- Registro e consulta do extrato de transações.
- Pontuação das movimentações e cálculo do percentual de saúde financeira.
- Classificação das contas em saudável, atenção ou risco.
- Identificação de tarifas como itens críticos e saques como itens de atenção.
- Geração de recomendações financeiras.
- Relatório detalhado das contas pelo menu do console.
- Testes automatizados das regras de negócio.

## Estrutura do projeto

```text
MiniBanco.ConsoleApp/
├── src/
│   └── MiniBanco.ConsoleApp/
│       ├── Exceptions/
│       ├── Models/
│       ├── Services/
│       └── Program.cs
├── tests/
│   └── MiniBanco.Tests/
├── scripts/
│   ├── Database-Environment.ps1
│   ├── Run-MiniBanco.ps1
│   └── Test-MiniBanco.ps1
├── MiniBanco.slnx
└── README.md
```

- `Models/`: classes das contas e das transações.
- `Exceptions/`: exceções específicas das regras bancárias.
- `Services/`: serviços de análise financeira.
- `Program.cs`: menu e interação com o usuário.
- `tests/`: testes automatizados.
- `database/`: scripts de criação e evolução inicial do esquema PostgreSQL.

## Pré-requisitos

- SDK do .NET 10 instalado.
- Docker Desktop com Docker Compose.
- Git, caso deseje clonar o repositório.

## Instalação e execução local (Windows PowerShell)

Clone o repositório:

```powershell
git clone https://github.com/Elmasterchef/MiniBanco.ConsoleApp.git
cd MiniBanco.ConsoleApp
```

Crie a configuração local do banco e substitua o valor de exemplo por uma senha sua em `.env`:

```powershell
Copy-Item .env.example .env
notepad .env
```

Inicie o PostgreSQL. O Compose publica o serviço em `127.0.0.1:5434` e, em um volume novo, executa os scripts SQL em ordem:

```powershell
docker compose up -d
```

Restaure e execute a aplicação. O script lê a senha de `.env` sem exibi-la e restaura as variáveis de ambiente ao sair:

```powershell
dotnet restore .\MiniBanco.slnx
.\scripts\Run-MiniBanco.ps1
```

Siga as opções apresentadas no menu do console.

## Testes

Com o PostgreSQL ativo, execute o script na raiz do repositório. Ele lê a senha de `.env` sem exibi-la e configura as variáveis apenas durante os testes:

```powershell
.\scripts\Test-MiniBanco.ps1
```

Os testes de integração precisam alcançar o PostgreSQL. Além de verificar a conexão, comparam o extrato carregado pelo repositório com os registros existentes no banco; eles não criam nem removem dados. Os demais testes cobrem as regras de negócio.

O script restaura as variáveis ao estado anterior quando termina.

> O `.env` é lido pelo Docker Compose, não pela aplicação .NET. Em um volume já existente, alterar `POSTGRES_PASSWORD` no `.env` não troca a senha armazenada no PostgreSQL. Os scripts em `database/` são executados automaticamente somente quando o volume é inicializado vazio.

## Regras de análise financeira

A aplicação atribui pontos às transações:

- Depósito: 10 pontos.
- Saque: 5 pontos.
- Tarifa: 0 pontos.

A classificação financeira utiliza o percentual de saúde calculado pela aplicação:

- **90% a 100%:** Conta Saudável.
- **60% a menos de 90%:** Conta em Atenção.
- **Abaixo de 60%:** Conta em Risco.

O relatório também apresenta tarifas, saques e recomendações relacionadas à classificação e às movimentações.

## Controle de versão

O desenvolvimento é organizado em uma branch de funcionalidades, com commits descritivos para registrar as etapas de implementação e correção.

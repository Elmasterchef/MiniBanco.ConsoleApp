# MiniBanco.ConsoleApp

Mini-projeto avaliativo desenvolvido em C# com .NET 10, utilizando programação orientada a objetos para simular operações bancárias e analisar a saúde financeira de contas.

## Objetivo

Implementar uma aplicação de console que permita cadastrar diferentes modalidades de contas bancárias, registrar movimentações, executar fechamentos mensais e gerar relatórios com indicadores e recomendações financeiras.

## Tecnologias

- C#
- .NET 10
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
├── MiniBanco.slnx
└── README.md
```

- `Models/`: classes das contas e das transações.
- `Exceptions/`: exceções específicas das regras bancárias.
- `Services/`: serviços de análise financeira.
- `Program.cs`: menu e interação com o usuário.
- `tests/`: testes automatizados.

## Pré-requisitos

- SDK do .NET 10 instalado.
- Git, caso deseje clonar o repositório.

## Como executar

Clone o repositório:

```powershell
git clone https://github.com/Elmasterchef/MiniBanco.ConsoleApp.git
cd MiniBanco.ConsoleApp
```

Execute a aplicação a partir da raiz do projeto:

```powershell
dotnet run --project .\src\MiniBanco.ConsoleApp\MiniBanco.ConsoleApp.csproj
```

Siga as opções apresentadas no menu do console.

## Como executar os testes

Na raiz do repositório, execute:

```powershell
dotnet test .\MiniBanco.slnx
```

O comando compila a solução e executa os testes automatizados.

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

## Melhorias futuras

- Ampliar a cobertura de testes para cenários-limite.
- Avaliar a possibilidade de empacotamento e execução com Docker.
- Evoluir a persistência de dados, caso seja necessário em versões futuras.

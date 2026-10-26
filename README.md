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

## Banco de dados com Docker Compose

O Docker Compose executa o PostgreSQL 17 em um contêiner chamado `minibanco-postgres`. A aplicação de console continua sendo executada localmente pelo .NET e acessa o banco pela porta `5434`.

Na primeira execução, crie o arquivo local `.env` a partir do exemplo e defina uma senha para o PostgreSQL:

```powershell
Copy-Item .env.example .env
notepad .env
```

O arquivo `.env` é local e não deve ser enviado ao Git. Em seguida, valide a configuração e inicie o banco:

```powershell
docker compose config --quiet
docker compose up -d
```

Confira se o serviço está saudável:

```powershell
docker compose ps
```

O estado deve aparecer como `healthy`. Se precisar acompanhar a inicialização, consulte os logs:

```powershell
docker compose logs -f postgres
```

Pressione `Ctrl+C` para sair da visualização dos logs; isso não para o contêiner. O Compose cria o banco `minibanco`, o usuário `minibanco_user` e publica o acesso em `127.0.0.1:5434`. Em um volume novo, os arquivos de `database/` são executados automaticamente para criar e evoluir o esquema.

### Executar a aplicação e os testes

Com o PostgreSQL saudável, execute a aplicação:

```powershell
dotnet restore .\MiniBanco.slnx
.\scripts\Run-MiniBanco.ps1
```

O script lê `POSTGRES_PASSWORD` do `.env` sem mostrá-la e configura as variáveis de conexão apenas durante a execução. Para executar os testes unitários e de integração:

```powershell
.\scripts\Test-MiniBanco.ps1
```

Os testes de integração precisam do contêiner em execução e com a senha correspondente à já configurada no PostgreSQL.

### Parar e preservar os dados

Para parar os serviços mantendo o contêiner e os dados:

```powershell
docker compose stop
```

Para iniciar novamente:

```powershell
docker compose start
```

Também é possível remover o contêiner sem remover o volume persistente:

```powershell
docker compose down
```

Depois de `down`, recrie e inicie o contêiner mantendo o volume existente com:

```powershell
docker compose up -d
```

Os dados ficam no volume Docker `minibanco_data` e permanecem após `stop`, `start` e `down`. **Não use `docker compose down -v` se quiser preservar o banco**, pois essa opção remove o volume e os dados armazenados nele.

Os scripts de `database/` são executados automaticamente somente na primeira inicialização de um volume vazio. Alterar `POSTGRES_PASSWORD` no `.env` também não troca a senha de um usuário que já existe no volume; a senha precisa corresponder à credencial configurada no PostgreSQL.

## Instalação e execução local (Windows PowerShell)

Clone o repositório:

```powershell
git clone https://github.com/Elmasterchef/MiniBanco.ConsoleApp.git
cd MiniBanco.ConsoleApp
```

Crie a configuração local do banco e substitua o valor de exemplo por uma senha sua em `.env` (detalhes na seção [Banco de dados com Docker Compose](#banco-de-dados-com-docker-compose)):

```powershell
Copy-Item .env.example .env
notepad .env
```

Inicie o PostgreSQL:

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

> O Compose usa `.env` para configurar o contêiner; os scripts PowerShell também leem `POSTGRES_PASSWORD` desse arquivo para configurar a conexão da aplicação .NET.

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

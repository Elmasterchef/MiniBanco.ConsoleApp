# Roteiro de vídeo — MiniBanco.ConsoleApp

**Duração sugerida:** 5 a 7 minutos
**Formato:** apresentação com demonstração no terminal e no PostgreSQL
**Público:** avaliação acadêmica

## Antes de gravar

- Abra o repositório na branch `feat/postgresql-docker`.
- Deixe o Docker Desktop ativo e confirme o PostgreSQL com `docker compose ps`.
- Use uma conta de demonstração em banco descartável/isolado. O fechamento mensal processa todas as contas carregadas pela aplicação.
- Não mostre nem leia em voz alta o arquivo `.env`, senhas, dados de contas reais ou o conteúdo do backup local.
- Deixe o README aberto para mostrar os comandos de instalação e execução.

## 1. Apresentação — 0:00 a 0:30

**Fala sugerida:**

> Olá, vou apresentar o MiniBanco.ConsoleApp, um projeto acadêmico em C# e .NET 10. Ele simula contas bancárias, depósitos, saques e fechamento mensal, persiste os dados em PostgreSQL e apresenta um relatório de saúde financeira.

**Na tela:** mostre a estrutura da solução e o menu da aplicação.

## 2. Arquitetura e decisões — 0:30 a 1:30

**Fala sugerida:**

> A aplicação de console roda localmente. O Docker Compose executa o PostgreSQL 17 e publica a porta 5434 somente no endereço local. O acesso ao banco é feito com Npgsql 10.0.3.
>
> Separei modelos de domínio, repositório, serviços de análise, exceções e testes. As variáveis de conexão são fornecidas ao processo por variáveis de ambiente. O `.env` é local, ignorado pelo Git, e os scripts PowerShell carregam a senha sem exibi-la.

**Na tela:** mostre `compose.yaml`, `ConexaoBanco.cs`, `ContaRepository.cs` e, rapidamente, as pastas `Models`, `Repositories`, `Services` e `tests`. Não abra `.env`.

**Decisões importantes para explicar:**

- O Compose mantém os dados no volume `minibanco_data`, permitindo reiniciar ou recriar o contêiner sem remover o banco.
- Os scripts SQL de inicialização são aplicados automaticamente quando o volume está vazio; mudar o `.env` não troca a senha já gravada em um volume existente.
- O saldo e as movimentações de uma operação são atualizados dentro de uma transação PostgreSQL. Se uma etapa falhar, a transação não deve deixar saldo e extrato divergentes.
- A atualização verifica também o saldo anterior para detectar alteração concorrente da mesma conta.
- Ao iniciar, o repositório recupera as contas e o extrato persistido, incluindo a data original das transações.

## 3. Como iniciar — 1:30 a 2:00

**Fala sugerida:**

> Para preparar uma máquina nova, crio o `.env` a partir do exemplo, defino uma senha local e inicio o banco com Docker Compose. Depois executo a aplicação pelo script, que configura as variáveis de conexão apenas durante o processo.

**Na tela:** mostre os comandos no README, sem abrir o arquivo com a senha:

```powershell
Copy-Item .env.example .env
docker compose config --quiet
docker compose up -d
docker compose ps
dotnet restore .\MiniBanco.slnx
.\scripts\Run-MiniBanco.ps1
```

## 4. Demonstração de operações — 2:00 a 3:30

**Fala sugerida:**

> Vou usar um banco isolado para que a demonstração não altere contas reais. Abro uma conta corrente com saldo inicial de 100 reais, deposito 50 e saco 20. O saldo passa para 130 reais.
>
> Em seguida aplico o fechamento mensal. Para esta conta corrente, a tarifa de manutenção é de 10 reais, então o saldo final fica em 120 reais. Cada movimentação aparece no extrato.

**Na tela:** demonstre a abertura da conta, o depósito, o saque e o fechamento mensal. Use somente dados fictícios. Mostre as confirmações e os saldos exibidos.

> **Atenção durante a gravação:** o fechamento mensal percorre todas as contas carregadas. Faça esta etapa em um banco de demonstração isolado, nunca no banco persistente com contas que não devem ser alteradas.

## 5. Persistência e relatório — 3:30 a 4:30

**Fala sugerida:**

> Consultei o PostgreSQL e confirmei o saldo de 120 reais e três movimentações: depósito de 50, saque de 20 e tarifa de 10. Depois fechei e reabri a aplicação. O relatório recuperou a conta e as três transações do banco, mostrando também a tarifa como item crítico e o saque como item de atenção.

**Na tela:** mostre o resultado consultado no banco ou o relatório da aplicação após reiniciar. No cenário demonstrado, a pontuação foi de 15 em 30 e a classificação ficou em “Conta em Risco”.

## 6. Testes e encerramento — 4:30 a 5:30

**Fala sugerida:**

> A solução tem testes unitários para as regras de negócio e testes de integração que precisam do PostgreSQL ativo. A suíte executada neste projeto passou com 31 testes aprovados, sem falhas ou testes ignorados. O build da solução também concluiu com sucesso.
>
> O guia no README explica como iniciar, testar e parar os serviços. O comando `docker compose down` preserva o volume; `docker compose down -v` remove os dados, então não deve ser usado quando a intenção é manter o banco.
>
> Com isso, concluo a apresentação do MiniBanco.ConsoleApp.

**Na tela:** mostre o resumo do teste e finalize no README ou no menu da aplicação.

## Comandos de referência

```powershell
# Banco
docker compose up -d
docker compose ps

# Aplicação e testes
.\scripts\Run-MiniBanco.ps1
.\scripts\Test-MiniBanco.ps1

# Parar sem remover os dados
docker compose stop

# Remover o contêiner preservando o volume
docker compose down
```

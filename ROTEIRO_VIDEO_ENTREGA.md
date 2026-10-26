# Roteiro de vídeo — MiniBanco.ConsoleApp

**Duração sugerida:** 4 a 5 minutos
**Foco:** contas, movimentações, fechamento mensal e análise financeira
**Público:** avaliação acadêmica

## Antes de gravar

- Abra o repositório na branch `feat/postgresql-docker`.
- Deixe o Docker Desktop ativo e confirme o PostgreSQL com `docker compose ps`.
- Prepare o banco isolado `minibanco_demo` com os comandos da seção “Preparar o banco de demonstração”.
- Não mostre nem leia em voz alta o arquivo `.env`, senhas, dados de contas reais ou o conteúdo do backup local.
- Deixe o README aberto para mostrar os comandos de instalação e execução.

## Preparar o banco de demonstração

Faça esta preparação uma vez antes de gravar. No PowerShell, inicie o PostgreSQL e confirme que está `healthy`:

```powershell
docker compose up -d
docker compose ps
```

Depois, mantendo esse mesmo PowerShell aberto porque `$demoDb` será usado nas próximas etapas, crie um banco separado e aplique nele os mesmos scripts SQL do projeto. Isso não altera o banco principal `minibanco`.

```powershell
$demoDb = 'minibanco_demo'
docker compose exec -T postgres createdb -U minibanco_user $demoDb
Get-Content -Raw .\database\init.sql | docker compose exec -T postgres psql -U minibanco_user -d $demoDb -v ON_ERROR_STOP=1
Get-Content -Raw .\database\002_campos_especificos_contas.sql | docker compose exec -T postgres psql -U minibanco_user -d $demoDb -v ON_ERROR_STOP=1
Get-Content -Raw .\database\003_ajusta_restricao_saldo.sql | docker compose exec -T postgres psql -U minibanco_user -d $demoDb -v ON_ERROR_STOP=1
```

Se `minibanco_demo` já existir, não o exclua sem verificar se contém dados que deseja manter. Para uma gravação repetível, escolha outro nome livre e use o mesmo valor nos comandos seguintes.

## Passo a passo da gravação

### 1. Iniciar a aplicação

Depois de preparar o banco de demonstração, inicie a aplicação apontando para ele:

```powershell
dotnet restore .\MiniBanco.slnx
.\scripts\Run-MiniBanco.ps1 -DatabaseName $demoDb
```

O menu deve informar que carregou zero contas. Se a aplicação mostrar outra quantidade, confira o nome do banco antes de prosseguir.

### 2. Criar a conta de demonstração

No menu, digite estas opções e valores, pressionando `Enter` após cada linha:

| Prompt | Entrada |
|---|---:|
| Menu principal | `1` — abrir nova conta |
| Tipo da conta | `1` — conta corrente |
| Nome do titular | `Conta Demo` |
| Número da conta | `900000` |
| Saldo inicial | `100` |
| Limite do cheque especial | `0` |

A aplicação deve confirmar `Conta cadastrada com sucesso!`. Pressione uma tecla quando aparecer a pausa para retornar ao menu.

### 3. Registrar depósito e saque

No menu principal, registre o depósito:

| Prompt | Entrada |
|---|---:|
| Menu principal | `2` — registrar movimentação |
| Número da conta | `900000` |
| Operação | `1` — depósito |
| Valor | `50` |
| Descrição | `Deposito de demonstracao` |

O saldo esperado é **R$ 150,00**. Pressione uma tecla para voltar ao menu e registre o saque:

| Prompt | Entrada |
|---|---:|
| Menu principal | `2` — registrar movimentação |
| Número da conta | `900000` |
| Operação | `2` — saque |
| Valor | `20` |

O saldo esperado é **R$ 130,00**. Pressione uma tecla para voltar ao menu.

### 4. Aplicar o fechamento mensal

Digite `3` no menu. Como este banco de demonstração contém somente a conta `900000`, o fechamento não afeta nenhuma conta real. Para a conta corrente, a tarifa mensal é de R$ 10,00; o saldo esperado após o fechamento é **R$ 120,00**. Pressione uma tecla para retornar ao menu e digite `0` para sair.

> O fechamento mensal percorre todas as contas carregadas. Não execute esta opção no banco principal se não quiser aplicar as regras mensais a todas as contas dele.

### 5. Reiniciar, mostrar o extrato e conferir o banco

Inicie novamente a aplicação no mesmo banco, escolha `4` para exibir o relatório e, depois da pausa, `0` para sair:

```powershell
.\scripts\Run-MiniBanco.ps1 -DatabaseName $demoDb
```

O relatório deve carregar saldo **R$ 120,00** e três transações: depósito de R$ 50,00, saque de R$ 20,00 e tarifa de R$ 10,00. Isso demonstra a recuperação dos dados depois de encerrar e reiniciar o processo.

Também é possível conferir os registros diretamente no PostgreSQL:

```powershell
docker compose exec -T postgres psql -U minibanco_user -d $demoDb -P pager=off -c "SELECT c.numero, c.saldo, COUNT(t.id) AS transacoes, STRING_AGG(t.tipo || ':' || t.valor::text, ', ' ORDER BY t.id) AS extrato FROM public.contas c LEFT JOIN public.transacoes t ON t.conta_numero = c.numero WHERE c.numero = '900000' GROUP BY c.numero, c.saldo;"
```

Resultado esperado: saldo `120.00`, `3` transações e `Deposito:50.00, Saque:20.00, Tarifa:10.00`.

Depois da conferência, saia pelo menu com `0`. Se quiser remover somente o banco temporário após a gravação, confirme primeiro que `$demoDb` contém o nome do banco de demonstração e execute:

```powershell
docker compose exec -T postgres dropdb -U minibanco_user $demoDb
```

Não use `docker compose down -v`: isso removeria o volume persistente, não apenas o banco de demonstração.

## Roteiro falado

### 1. Apresentação — 0:00 a 0:20

**Fala sugerida:**

> Olá, vou apresentar o MiniBanco.ConsoleApp. Ele permite trabalhar com diferentes tipos de conta, registrar operações bancárias e analisar a saúde financeira de cada conta.

**Na tela:** mostre o menu principal.

### 2. Contas e regras — 0:20 a 1:10

**Fala sugerida:**

> O sistema oferece conta corrente, poupança e conta salário. Cada tipo tem suas próprias regras: a corrente pode usar cheque especial, a poupança recebe rendimento no fechamento e a conta salário controla a quantidade de saques mensais.
>
> As operações validam valores e limites antes de alterar o saldo. O extrato registra depósitos, saques, tarifas e rendimentos.

**Na tela:** mostre rapidamente os tipos de conta no código e volte ao menu.

### 3. Demonstração das operações — 1:10 a 2:50

**Fala sugerida:**

> Para demonstrar, vou abrir uma conta corrente com saldo inicial de 100 reais. Depois faço um depósito de 50 e um saque de 20. O saldo fica em 130 reais.
>
> No fechamento mensal, esta conta recebe uma tarifa de 10 reais. O saldo final passa para 120 reais. A aplicação confirma cada operação e atualiza o extrato.

**Na tela:** siga as entradas das seções “Criar a conta de demonstração”, “Registrar depósito e saque” e “Aplicar o fechamento mensal”.

> **Atenção:** faça o fechamento mensal somente no banco isolado de demonstração. Essa opção processa todas as contas carregadas.

### 4. Extrato e análise financeira — 2:50 a 4:00

**Fala sugerida:**

> Agora vou reiniciar a aplicação para mostrar que os dados permanecem salvos. No relatório, vemos o saldo de 120 reais e as três operações do extrato.
>
> O sistema calcula uma pontuação e classifica a saúde financeira como saudável, atenção ou risco. Tarifas aparecem como itens críticos, saques como itens de atenção e o relatório apresenta recomendações para a conta.

**Na tela:** reinicie no mesmo banco, escolha `4` e mostre o saldo, o extrato, a classificação e as recomendações.

### 5. Decisões e encerramento — 4:00 a 4:40

**Fala sugerida:**

> A aplicação guarda os dados no PostgreSQL e mantém o saldo e o extrato consistentes usando transações. O Docker Compose facilita iniciar o banco localmente; a apresentação de hoje ficou concentrada nas funções do MiniBanco.
>
> A solução também tem testes unitários e de integração. Na validação realizada, os 31 testes passaram. Obrigado por assistir à apresentação.

**Na tela:** mostre brevemente o resumo dos testes e finalize.

## Comandos para iniciar durante a gravação

```powershell
docker compose up -d
docker compose ps
dotnet restore .\MiniBanco.slnx
.\scripts\Run-MiniBanco.ps1 -DatabaseName $demoDb
```

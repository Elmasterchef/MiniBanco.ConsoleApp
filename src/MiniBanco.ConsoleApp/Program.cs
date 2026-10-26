
using System.Globalization;
using MiniBanco.ConsoleApp.Models;
using MiniBanco.ConsoleApp.Services;
using MiniBanco.ConsoleApp.Exceptions;
using MiniBanco.ConsoleApp.Data;
using MiniBanco.ConsoleApp.Repositories;
using Npgsql;

await using var dataSource = ConexaoBanco.CriarDataSource();
var repositorio = new ContaRepository(dataSource);

List<Conta> contas;

try
{
    bool bancoDisponivel = await repositorio.BancoDisponivelAsync();

    if (!bancoDisponivel)
    {
        Console.WriteLine("Não foi possível confirmar a conexão com o banco.");
        return;
    }

    contas = await repositorio.ObterTodasAsync();
    Console.WriteLine($"Contas carregadas do PostgreSQL: {contas.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao conectar ao PostgreSQL: {ex.Message}");
    Console.WriteLine("Verifique o banco e as variáveis de ambiente.");
    return;
}

bool executando = true;
while (executando)
{
    Console.Clear();

    Console.WriteLine("==================================");
    Console.WriteLine("         SISTEMA MINIBANCO");
    Console.WriteLine("==================================");
    Console.WriteLine("1 - Abrir nova conta");
    Console.WriteLine("2 - Registrar movimentação");
    Console.WriteLine("3 - Aplicar fechamento mensal");
    Console.WriteLine("4 - Exibir relatório das contas");
    Console.WriteLine("0 - Sair");
    Console.WriteLine("==================================");
    Console.Write("Escolha uma opção: ");

    string? opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1":
            await AbrirContaAsync(contas, repositorio);
            break;

        case "2":
            await RegistrarMovimentacaoAsync(contas, repositorio);
            break;

        case "3":
            await AplicarFechamentoMensalAsync(contas, repositorio);
            break;

        case "4":
            ExibirRelatorioContas(contas);
            break;

        case "0":
            executando = false;
            Console.WriteLine("Obrigado por utilizar o MiniBanco!");
            break;

        default:
            Console.WriteLine("Opção inválida. Escolha uma opção do menu.");
            break;
    }

    if (executando)
    {
        Console.WriteLine();
        Console.WriteLine("Pressione qualquer tecla para continuar...");
        Console.ReadKey();
    }
}

static async Task AbrirContaAsync(
    List<Conta> contas,
    ContaRepository repositorio)
{
    Console.WriteLine();
    Console.WriteLine("=== ABERTURA DE CONTA ===");
    Console.WriteLine("1 - Conta Corrente");
    Console.WriteLine("2 - Conta Poupança");
    Console.WriteLine("3 - Conta Salário");
    Console.Write("Escolha o tipo de conta: ");

    string? tipo = Console.ReadLine();

    if (tipo != "1" && tipo != "2" && tipo != "3")
    {
        Console.WriteLine("Tipo de conta inválido.");
        return;
    }

    Console.Write("Nome do titular: ");
    string titular = Console.ReadLine() ?? string.Empty;

    Console.Write("Número da conta: ");
    if (!int.TryParse(Console.ReadLine(), out int numero))
    {
        Console.WriteLine("Número da conta inválido.");
        return;
    }

    Console.Write("Saldo inicial: ");
    if (!decimal.TryParse(
            Console.ReadLine(),
            NumberStyles.Number,
            CultureInfo.GetCultureInfo("pt-BR"),
            out decimal saldoInicial))
    {
        Console.WriteLine("Saldo inicial inválido. Exemplo: 1500,50.");
        return;
    }

    try
    {
        Conta conta;

        switch (tipo)
        {
            case "1":
                Console.Write("Limite do cheque especial: ");
                if (!decimal.TryParse(
                        Console.ReadLine(),
                        NumberStyles.Number,
                        CultureInfo.GetCultureInfo("pt-BR"),
                        out decimal limite))
                {
                    Console.WriteLine("Limite do cheque especial inválido.");
                    return;
                }

                conta = new ContaCorrente(
                    titular, numero, saldoInicial, limite);
                break;

            case "2":
                Console.Write("Taxa de rendimento mensal (%): ");
                if (!double.TryParse(
                        Console.ReadLine(),
                        NumberStyles.Number,
                        CultureInfo.GetCultureInfo("pt-BR"),
                        out double taxa))
                {
                    Console.WriteLine("Taxa de rendimento inválida.");
                    return;
                }

                Console.Write("Dia do aniversário da poupança: ");
                if (!int.TryParse(Console.ReadLine(), out int dia))
                {
                    Console.WriteLine("Dia de aniversário inválido.");
                    return;
                }

                conta = new ContaPoupanca(
                    titular, numero, saldoInicial, taxa, dia);
                break;

            case "3":
                Console.Write("Nome do empregador: ");
                string empregador = Console.ReadLine() ?? string.Empty;

                Console.Write("Limite mensal de saques: ");
                if (!int.TryParse(
                        Console.ReadLine(),
                        out int limiteSaques))
                {
                    Console.WriteLine("Limite mensal de saques inválido.");
                    return;
                }

                conta = new ContaSalario(
                    titular, numero, saldoInicial,
                    empregador, limiteSaques);
                break;

            default:
                return;
        }

        await repositorio.SalvarAsync(conta);
        contas.Add(conta);
        Console.WriteLine("Conta cadastrada com sucesso!");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Não foi possível abrir a conta: {ex.Message}");
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"Formato de dado inválido: {ex.Message}");
    }
    catch (PostgresException ex) when (ex.SqlState == "23505")
    {
        Console.WriteLine("Já existe uma conta cadastrada com esse número.");
    }
    catch (PostgresException ex)
    {
        Console.WriteLine($"Erro PostgreSQL completo: {ex}");
    }
}

    

static async Task RegistrarMovimentacaoAsync(
    List<Conta> contas,
    ContaRepository repositorio)
{
    Console.WriteLine();
    Console.WriteLine("=== REGISTRAR MOVIMENTAÇÃO ===");

    if (contas.Count == 0)
    {
        Console.WriteLine("Nenhuma conta cadastrada. Abra uma conta primeiro.");
        return;
    }

    Console.Write("Número da conta: ");

    if (!int.TryParse(Console.ReadLine(), out int numero))
    {
        Console.WriteLine("Número da conta inválido.");
        return;
    }

    int indiceConta = contas.FindIndex(conta => conta.Numero == numero);

    if (indiceConta < 0)
    {
        Console.WriteLine("Conta não encontrada.");
        return;
    }

    Conta contaOriginal = contas[indiceConta];
    Conta contaEncontrada = ClonarConta(contaOriginal);

    Console.WriteLine("1 - Depósito");
    Console.WriteLine("2 - Saque");
    Console.Write("Escolha a operação: ");

    string? operacao = Console.ReadLine();

    if (operacao != "1" && operacao != "2")
    {
        Console.WriteLine("Operação inválida.");
        return;
    }

    Console.Write("Valor da movimentação: ");

    if (!decimal.TryParse(
            Console.ReadLine(),
            NumberStyles.Number,
            CultureInfo.GetCultureInfo("pt-BR"),
            out decimal valor))
    {
        Console.WriteLine("Valor inválido. Exemplo: 150,50.");
        return;
    }

    decimal saldoAnterior = contaOriginal.Saldo;
    int saquesRealizadosAnterior = contaOriginal is ContaSalario salarioOriginal
        ? salarioOriginal.SaquesRealizadosNoMes
        : 0;

    try
    {
        if (operacao == "1")
        {
            Console.Write("Descrição do depósito: ");
            string descricao = Console.ReadLine() ?? string.Empty;

            contaEncontrada.Depositar(valor, descricao);
        }
        else
        {
            contaEncontrada.Sacar(valor);
        }

        IReadOnlyList<Transacao> novasTransacoes = contaEncontrada.Extrato
            .Skip(contaOriginal.Extrato.Count)
            .ToList();

        await repositorio.RegistrarMovimentacaoAsync(
            contaEncontrada,
            saldoAnterior,
            novasTransacoes,
            saquesRealizadosAnterior);

        contas[indiceConta] = contaEncontrada;

        Console.WriteLine(
            operacao == "1"
                ? "Depósito realizado e salvo com sucesso!"
                : "Saque realizado e salvo com sucesso!");

        Console.WriteLine($"Saldo atual: {contaEncontrada.Saldo:C}");
    }
    catch (SaldoInsuficienteException ex)
    {
        Console.WriteLine($"Saque não realizado: {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Não foi possível realizar a operação: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Não foi possível realizar a operação: {ex.Message}");
    }
    catch (NpgsqlException ex)
    {
        Console.WriteLine($"Erro ao persistir a movimentação: {ex.Message}");
        Console.WriteLine("A movimentação não foi confirmada pelo banco.");
    }
}


static async Task AplicarFechamentoMensalAsync(
    List<Conta> contas,
    ContaRepository repositorio)
{
    Console.WriteLine();
    Console.WriteLine("=== FECHAMENTO MENSAL ===");

    if (contas.Count == 0)
    {
        Console.WriteLine("Nenhuma conta cadastrada para fechamento.");
        return;
    }

    bool houveFalha = false;

    for (int i = 0; i < contas.Count; i++)
    {
        Conta contaOriginal = contas[i];
        Conta conta = ClonarConta(contaOriginal);

        try
        {
            decimal saldoAnterior = contaOriginal.Saldo;
            int saquesRealizadosAnterior =
                contaOriginal is ContaSalario salarioOriginal
                    ? salarioOriginal.SaquesRealizadosNoMes
                    : 0;

            conta.AplicarFechamentoMensal();

            IReadOnlyList<Transacao> novasTransacoes = conta.Extrato
                .Skip(contaOriginal.Extrato.Count)
                .ToList();

            await repositorio.RegistrarMovimentacaoAsync(
                conta,
                saldoAnterior,
                novasTransacoes,
                saquesRealizadosAnterior);

            contas[i] = conta;

            Console.WriteLine();
            Console.WriteLine($"Conta: {conta.Numero} - {conta.Titular}");
            Console.WriteLine($"Saldo anterior: {saldoAnterior:C}");
            Console.WriteLine($"Saldo após fechamento: {conta.Saldo:C}");
            Console.WriteLine("Fechamento aplicado com sucesso.");
        }
        catch (ArgumentException ex)
        {
            houveFalha = true;
            Console.WriteLine(
                $"Erro na conta {conta.Numero}: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            houveFalha = true;
            Console.WriteLine(
                $"Erro na conta {conta.Numero}: {ex.Message}");
        }
        catch (NpgsqlException ex)
        {
            houveFalha = true;
            Console.WriteLine(
                $"Erro ao persistir o fechamento da conta {conta.Numero}: {ex.Message}");
        }
    }

    Console.WriteLine();
    Console.WriteLine(houveFalha
        ? "Fechamento mensal concluído com falhas. Consulte os erros acima."
        : "Processamento do fechamento mensal concluído.");
}

static Conta ClonarConta(Conta conta)
{
    Conta copia = conta switch
    {
        ContaCorrente corrente => new ContaCorrente(
            corrente.Titular,
            corrente.Numero,
            corrente.Saldo,
            corrente.LimiteChequeEspecial),
        ContaPoupanca poupanca => new ContaPoupanca(
            poupanca.Titular,
            poupanca.Numero,
            poupanca.Saldo,
            poupanca.TaxaRendimentoMensal,
            poupanca.DiaAniversario),
        ContaSalario salario => new ContaSalario(
            salario.Titular,
            salario.Numero,
            salario.Saldo,
            salario.Empregador,
            salario.LimiteSaquesMensais,
            salario.SaquesRealizadosNoMes),
        _ => new Conta(conta.Titular, conta.Numero, conta.Saldo)
    };

    foreach (Transacao transacao in conta.Extrato)
    {
        copia.RestaurarTransacao(
            transacao.Descricao,
            transacao.Tipo,
            transacao.Valor,
            transacao.Data);
    }

    return copia;
}

static void ExibirRelatorioContas(List<Conta> contas)
{
    Console.WriteLine();
    Console.WriteLine("==================================");
    Console.WriteLine("       RELATÓRIO DE CONTAS");
    Console.WriteLine("==================================");

    if (contas.Count == 0)
    {
        Console.WriteLine("Nenhuma conta cadastrada.");
        return;
    }
    MotorAnalise motor = new MotorAnalise();

    foreach (Conta conta in contas)
    {
        Console.WriteLine();
        Console.WriteLine("----------------------------------");
        Console.WriteLine($"Titular: {conta.Titular}");
        Console.WriteLine($"Número: {conta.Numero}");
        Console.WriteLine($"Saldo atual: {conta.Saldo:C}");

        if (conta is ContaCorrente contaCorrente)
        {
            Console.WriteLine("Tipo: Conta Corrente");
            Console.WriteLine(
                $"Limite do cheque especial: {contaCorrente.LimiteChequeEspecial:C}");
        }
        else if (conta is ContaPoupanca contaPoupanca)
        {
            Console.WriteLine("Tipo: Conta Poupança");
            Console.WriteLine(
                $"Taxa de rendimento mensal: {contaPoupanca.TaxaRendimentoMensal}%");
            Console.WriteLine(
                $"Dia do aniversário: {contaPoupanca.DiaAniversario}");
        }
        else if (conta is ContaSalario contaSalario)
        {
            Console.WriteLine("Tipo: Conta Salário");
            Console.WriteLine($"Empregador: {contaSalario.Empregador}");
            Console.WriteLine(
                $"Limite mensal de saques: {contaSalario.LimiteSaquesMensais}");
        }

            Console.WriteLine($"Transações registradas: {conta.Extrato.Count}");
            
Console.WriteLine();
Console.WriteLine("EXTRATO DE TRANSAÇÕES");

if (conta.Extrato.Count == 0)
{
    Console.WriteLine("Nenhuma transação registrada.");
}
else
{
    foreach (Transacao transacao in conta.Extrato)
    {
        Console.WriteLine("----------------------------------");
        Console.WriteLine($"Data: {transacao.Data:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine($"Descrição: {transacao.Descricao}");
        Console.WriteLine($"Tipo: {transacao.Tipo}");
        Console.WriteLine($"Valor: {transacao.Valor:C}");
        Console.WriteLine($"Pontos: {transacao.Pontos}");
    }
}

Console.WriteLine("----------------------------------");

        int pontosObtidos = motor.CalcularPontosObtidos(conta);
        int pontuacaoMaxima = motor.CalcularPontuacaoMaxima(conta);
        double percentualSaude = motor.CalcularPercentualSaude(conta);
        string classificacao = motor.ClassificarConta(conta);

            Console.WriteLine($"Pontos obtidos: {pontosObtidos}");
            Console.WriteLine($"Pontuação máxima: {pontuacaoMaxima}");
            Console.WriteLine($"Saúde financeira: {percentualSaude:F2}%");
            Console.WriteLine($"Classificação: {classificacao}");
            
Console.WriteLine();
Console.WriteLine("ITENS CRÍTICOS — TARIFAS");

List<Transacao> itensCriticos = motor.ObterItensCriticos(conta);

if (itensCriticos.Count == 0)
{
    Console.WriteLine("Nenhuma tarifa registrada.");
}
else
{
    foreach (Transacao transacao in itensCriticos)
    {
        Console.WriteLine(
            $"{transacao.Descricao}: {transacao.Valor:C}");
    }
}

Console.WriteLine();
Console.WriteLine("ITENS DE ATENÇÃO — SAQUES");

List<Transacao> itensAtencao = motor.ObterItensAtencao(conta);

if (itensAtencao.Count == 0)
{
    Console.WriteLine("Nenhum saque registrado.");
}
else
{
    foreach (Transacao transacao in itensAtencao)
    {
        Console.WriteLine(
            $"{transacao.Descricao}: {transacao.Valor:C}");
    }
}

Console.WriteLine();
Console.WriteLine("RECOMENDAÇÕES FINANCEIRAS");

List<string> recomendacoes = motor.GerarRecomendacoes(conta);

foreach (string recomendacao in recomendacoes)
{
    Console.WriteLine($"- {recomendacao}");
}

    }

            Console.WriteLine();
            Console.WriteLine("==================================");
            Console.WriteLine($"Total de contas: {contas.Count}");
            Console.WriteLine("==================================");
}


using System.Globalization;
using MiniBanco.ConsoleApp.Models;
using MiniBanco.ConsoleApp.Services;

List<Conta> contas = new();
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
            AbrirConta(contas);
            break;

        case "2":
            RegistrarMovimentacao(contas);
            break;

        case "3":
            AplicarFechamentoMensal(contas);
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

static void AbrirConta(List<Conta> contas)
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
    
}

    
static void RegistrarMovimentacao(List<Conta> contas)
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

    Conta? contaEncontrada = null;

    foreach (Conta conta in contas)
    {
        if (conta.Numero == numero)
        {
            contaEncontrada = conta;
            break;
        }
    }

    if (contaEncontrada is null)
    {
        Console.WriteLine("Conta não encontrada.");
        return;
    }

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

    try
    {
        if (operacao == "1")
        {
            Console.Write("Descrição do depósito: ");
            string descricao = Console.ReadLine() ?? string.Empty;

            contaEncontrada.Depositar(valor, descricao);
            Console.WriteLine("Depósito realizado com sucesso!");
        }
        else
        {
            contaEncontrada.Sacar(valor);
            Console.WriteLine("Saque realizado com sucesso!");
        }

        Console.WriteLine($"Saldo atual: {contaEncontrada.Saldo:C}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Não foi possível realizar a operação: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Não foi possível realizar a operação: {ex.Message}");
    }
}

static void AplicarFechamentoMensal(List<Conta> contas)
{
    Console.WriteLine();
    Console.WriteLine("=== FECHAMENTO MENSAL ===");

    if (contas.Count == 0)
    {
        Console.WriteLine("Nenhuma conta cadastrada para fechamento.");
        return;
    }

    foreach (Conta conta in contas)
    {
        try
        {
            decimal saldoAnterior = conta.Saldo;

            conta.AplicarFechamentoMensal();

            Console.WriteLine();
            Console.WriteLine($"Conta: {conta.Numero} - {conta.Titular}");
            Console.WriteLine($"Saldo anterior: {saldoAnterior:C}");
            Console.WriteLine($"Saldo após fechamento: {conta.Saldo:C}");
            Console.WriteLine("Fechamento aplicado com sucesso.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(
                $"Erro na conta {conta.Numero}: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(
                $"Erro na conta {conta.Numero}: {ex.Message}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Processamento do fechamento mensal concluído.");
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
        int pontosObtidos = motor.CalcularPontosObtidos(conta);
int pontuacaoMaxima = motor.CalcularPontuacaoMaxima(conta);
double percentualSaude = motor.CalcularPercentualSaude(conta);
string classificacao = motor.ClassificarConta(conta);

Console.WriteLine($"Pontos obtidos: {pontosObtidos}");
Console.WriteLine($"Pontuação máxima: {pontuacaoMaxima}");
Console.WriteLine($"Saúde financeira: {percentualSaude:F2}%");
Console.WriteLine($"Classificação: {classificacao}");
    }

    Console.WriteLine();
    Console.WriteLine("==================================");
    Console.WriteLine($"Total de contas: {contas.Count}");
    Console.WriteLine("==================================");
}

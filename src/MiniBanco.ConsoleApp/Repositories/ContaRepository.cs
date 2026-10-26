
using MiniBanco.ConsoleApp.Data;
using MiniBanco.ConsoleApp.Models;
using Npgsql;
using NpgsqlTypes;

namespace MiniBanco.ConsoleApp.Repositories;

public sealed class ContaRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ContaRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<bool> BancoDisponivelAsync(
        CancellationToken cancellationToken = default)
    {
        await using var conexao =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var comando = new NpgsqlCommand("SELECT 1;", conexao);

        object? resultado =
            await comando.ExecuteScalarAsync(cancellationToken);

        return resultado is int valor && valor == 1;
    }
    public async Task<List<Conta>> ObterTodasAsync(
        CancellationToken cancellationToken = default)
    {
        var contas = new List<Conta>();

        const string sqlContas = """
            SELECT
                numero,
                titular,
                tipo,
                saldo,
                limite_cheque_especial,
                taxa_rendimento_mensal,
                dia_aniversario,
                empregador,
                limite_saques_mensais,
                saques_realizados_no_mes
            FROM public.contas
            ORDER BY numero;
            """;

        await using var conexao =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        await using (var comandoContas = new NpgsqlCommand(sqlContas, conexao))
        await using (var leitor =
            await comandoContas.ExecuteReaderAsync(cancellationToken))
        {
            while (await leitor.ReadAsync(cancellationToken))
            {
                string numeroTexto = leitor.GetString(0);

                if (!int.TryParse(numeroTexto, out int numero))
                {
                    throw new InvalidOperationException(
                        $"Número de conta inválido no banco: '{numeroTexto}'.");
                }

                string titular = leitor.GetString(1);
                string tipo = leitor.GetString(2);
                decimal saldo = leitor.GetDecimal(3);

                Conta conta = tipo switch
                {
                    "Corrente" => new ContaCorrente(
                        titular, numero, saldo, leitor.GetDecimal(4)),
                    "Poupanca" => new ContaPoupanca(
                        titular, numero, saldo,
                        leitor.GetDouble(5), leitor.GetInt32(6)),
                    "Salario" => new ContaSalario(
                        titular, numero, saldo,
                        leitor.GetString(7), leitor.GetInt32(8),
                        leitor.GetInt32(9)),
                    _ => throw new InvalidOperationException(
                        $"Tipo de conta desconhecido no banco: '{tipo}'.")
                };

                contas.Add(conta);
            }
        }

        var contasPorNumero = contas.ToDictionary(conta => conta.Numero);

        const string sqlTransacoes = """
            SELECT conta_numero, descricao, tipo, valor, data
            FROM public.transacoes
            ORDER BY id;
            """;

        await using var comandoTransacoes =
            new NpgsqlCommand(sqlTransacoes, conexao);
        await using var leitorTransacoes =
            await comandoTransacoes.ExecuteReaderAsync(cancellationToken);

        while (await leitorTransacoes.ReadAsync(cancellationToken))
        {
            string numeroTexto = leitorTransacoes.GetString(0);

            if (!int.TryParse(numeroTexto, out int numero))
            {
                throw new InvalidOperationException(
                    $"Número de conta inválido no extrato: '{numeroTexto}'.");
            }

            if (contasPorNumero.TryGetValue(numero, out Conta? conta))
            {
                conta.RestaurarTransacao(
                    leitorTransacoes.GetString(1),
                    leitorTransacoes.GetString(2),
                    leitorTransacoes.GetDecimal(3),
                    leitorTransacoes.GetDateTime(4));
            }
        }

        return contas;
    }

    public async Task SalvarAsync(
        Conta conta,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(conta);

        string tipo;
        decimal? limiteChequeEspecial = null;
        double? taxaRendimentoMensal = null;
        int? diaAniversario = null;
        string? empregador = null;
        int? limiteSaquesMensais = null;
        int saquesRealizadosNoMes = 0;

        switch (conta)
        {
            case ContaCorrente corrente:
                tipo = "Corrente";
                limiteChequeEspecial = corrente.LimiteChequeEspecial;
                break;

            case ContaPoupanca poupanca:
                tipo = "Poupanca";
                taxaRendimentoMensal = poupanca.TaxaRendimentoMensal;
                diaAniversario = poupanca.DiaAniversario;
                break;

            case ContaSalario salario:
                tipo = "Salario";
                empregador = salario.Empregador;
                limiteSaquesMensais = salario.LimiteSaquesMensais;
		saquesRealizadosNoMes = salario.SaquesRealizadosNoMes;
                break;

            default:
                throw new NotSupportedException(
                    $"Tipo de conta não suportado: {conta.GetType().Name}.");
        }

        await using var conexao =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var transacao =
            await conexao.BeginTransactionAsync(cancellationToken);

        const string sql = """
            INSERT INTO public.contas (
                numero,
                titular,
                tipo,
                saldo,
                limite_cheque_especial,
                taxa_rendimento_mensal,
                dia_aniversario,
                empregador,
                limite_saques_mensais,
                saques_realizados_no_mes
            )
            VALUES (
                @numero,
                @titular,
                @tipo,
                @saldo,
                @limite_cheque_especial,
                @taxa_rendimento_mensal,
                @dia_aniversario,
                @empregador,
                @limite_saques_mensais,
                @saques_realizados_no_mes
            );
            """;

        await using var comando = new NpgsqlCommand(sql, conexao, transacao);

        comando.Parameters.AddWithValue("numero", conta.Numero.ToString());
        comando.Parameters.AddWithValue("titular", conta.Titular);
        comando.Parameters.AddWithValue("tipo", tipo);
        comando.Parameters.AddWithValue("saldo", conta.Saldo);

        comando.Parameters.AddWithValue(
            "limite_cheque_especial",
            (object?)limiteChequeEspecial ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "taxa_rendimento_mensal",
            (object?)taxaRendimentoMensal ?? DBNull.Value);
        comando.Parameters.AddWithValue(
            "dia_aniversario",
            (object?)diaAniversario ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "empregador",
            (object?)empregador ?? DBNull.Value);

        comando.Parameters.AddWithValue(
            "limite_saques_mensais",
            (object?)limiteSaquesMensais ?? DBNull.Value);
        comando.Parameters.AddWithValue(
            "saques_realizados_no_mes",
            saquesRealizadosNoMes);
            await comando.ExecuteNonQueryAsync(cancellationToken);
            await transacao.CommitAsync(cancellationToken);

    }

    public async Task RegistrarMovimentacaoAsync(
        Conta conta,
        decimal saldoAnterior,
        IReadOnlyList<Transacao> transacoes,
        int saquesRealizadosAnterior = 0,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(conta);
        ArgumentNullException.ThrowIfNull(transacoes);

        await using var conexao =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var transacaoBanco =
            await conexao.BeginTransactionAsync(cancellationToken);

        const string sqlAtualizarSaldo = """
            UPDATE public.contas
            SET saldo = @saldo,
                saques_realizados_no_mes = @saquesRealizados
            WHERE numero = @numero
              AND saldo = @saldoAnterior
              AND saques_realizados_no_mes = @saquesRealizadosAnterior;
            """;

        await using (var comandoSaldo = new NpgsqlCommand(
            sqlAtualizarSaldo, conexao, transacaoBanco))
        {
            comandoSaldo.Parameters.AddWithValue("saldo", conta.Saldo);
            comandoSaldo.Parameters.AddWithValue(
                "numero", conta.Numero.ToString());
            comandoSaldo.Parameters.AddWithValue("saldoAnterior", saldoAnterior);
            int saquesRealizados = conta is ContaSalario contaSalario
                ? contaSalario.SaquesRealizadosNoMes
                : 0;
            comandoSaldo.Parameters.AddWithValue(
                "saquesRealizados", saquesRealizados);
            comandoSaldo.Parameters.AddWithValue(
                "saquesRealizadosAnterior", saquesRealizadosAnterior);

            int linhasAfetadas =
                await comandoSaldo.ExecuteNonQueryAsync(cancellationToken);

            if (linhasAfetadas != 1)
            {
                throw new InvalidOperationException(
                    $"O saldo da conta {conta.Numero} mudou no banco ou a conta não existe. " +
                    "A movimentação não foi persistida.");
            }
        }

        const string sqlInserirTransacao = """
            INSERT INTO public.transacoes
                (conta_numero, descricao, tipo, valor, data)
            VALUES
                (@numero, @descricao, @tipo, @valor, @data);
            """;

        foreach (Transacao item in transacoes)
        {
            await using var comandoTransacao = new NpgsqlCommand(
                sqlInserirTransacao, conexao, transacaoBanco);

            comandoTransacao.Parameters.AddWithValue(
                "numero", conta.Numero.ToString());
            comandoTransacao.Parameters.AddWithValue(
                "descricao", item.Descricao);
            comandoTransacao.Parameters.AddWithValue(
                "tipo", item.Tipo);
            comandoTransacao.Parameters.AddWithValue(
                "valor", item.Valor);
            comandoTransacao.Parameters.AddWithValue(
                "data", item.Data);

            await comandoTransacao.ExecuteNonQueryAsync(cancellationToken);
        }

        await transacaoBanco.CommitAsync(cancellationToken);
    }
}

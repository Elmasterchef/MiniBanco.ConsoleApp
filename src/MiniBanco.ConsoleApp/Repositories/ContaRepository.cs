
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
            INSERT INTO contas (
                numero,
                titular,
                tipo,
                saldo,
                limite_cheque_especial,
                taxa_rendimento_mensal,
                dia_aniversario,
                empregador,
                limite_saques_mensais
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
                @limite_saques_mensais
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

        await comando.ExecuteNonQueryAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);
    }
}

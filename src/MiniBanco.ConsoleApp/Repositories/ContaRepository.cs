
using MiniBanco.ConsoleApp.Data;
using Npgsql;

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

        await using var comando = new NpgsqlCommand(
            "SELECT 1;",
            conexao);

        object? resultado =
            await comando.ExecuteScalarAsync(cancellationToken);

        return resultado is int valor && valor == 1;
    }
}

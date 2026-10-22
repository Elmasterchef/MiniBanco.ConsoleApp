
using Npgsql;

namespace MiniBanco.ConsoleApp.Data;

public static class ConexaoBanco
{
    public static NpgsqlDataSource CriarDataSource()
    {
        string host = ObterVariavel("MINIBANCO_DB_HOST");
        string porta = ObterVariavel("MINIBANCO_DB_PORT");
        string banco = ObterVariavel("MINIBANCO_DB_NAME");
        string usuario = ObterVariavel("MINIBANCO_DB_USER");
        string senha = ObterVariavel("MINIBANCO_DB_PASSWORD");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = int.Parse(porta),
            Database = banco,
            Username = usuario,
            Password = senha
        };

        return NpgsqlDataSource.Create(builder.ConnectionString);
    }

    private static string ObterVariavel(string nome)
    {
        string? valor = Environment.GetEnvironmentVariable(nome);

        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new InvalidOperationException(
                $"A variável de ambiente '{nome}' não foi configurada.");
        }

        return valor;
    }
}

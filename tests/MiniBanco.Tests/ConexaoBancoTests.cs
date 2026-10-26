using MiniBanco.ConsoleApp.Data;
using MiniBanco.ConsoleApp.Models;
using MiniBanco.ConsoleApp.Repositories;
using Npgsql;
using Xunit;

namespace MiniBanco.Tests;

public class ConexaoBancoTests
{
    [Fact]
    public async Task BancoDisponivelAsync_ComPostgreSqlAtivo_DeveRetornarVerdadeiro()
    {
        await using var dataSource = ConexaoBanco.CriarDataSource();
        var repositorio = new ContaRepository(dataSource);

        bool disponivel = await repositorio.BancoDisponivelAsync();

        Assert.True(disponivel);
    }

    [Fact]
    public async Task ObterTodasAsync_DeveRecuperarTransacoesPersistidas()
    {
        await using var dataSource = ConexaoBanco.CriarDataSource();
        var repositorio = new ContaRepository(dataSource);
        var contas = await repositorio.ObterTodasAsync();

        await using var conexao = await dataSource.OpenConnectionAsync();

        foreach (var conta in contas)
        {
            const string sql = """
                SELECT descricao, tipo, valor, data
                FROM public.transacoes
                WHERE conta_numero = @numero
                ORDER BY id;
                """;

            await using var comando = new NpgsqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("numero", conta.Numero.ToString());

            var transacoesPersistidas = new List<Transacao>();
            await using var leitor = await comando.ExecuteReaderAsync();

            while (await leitor.ReadAsync())
            {
                transacoesPersistidas.Add(new Transacao(
                    leitor.GetString(0),
                    leitor.GetString(1),
                    leitor.GetDecimal(2),
                    leitor.GetDateTime(3)));
            }

            Assert.Equal(transacoesPersistidas.Count, conta.Extrato.Count);

            for (int i = 0; i < transacoesPersistidas.Count; i++)
            {
                Assert.Equal(
                    transacoesPersistidas[i].Descricao,
                    conta.Extrato[i].Descricao);
                Assert.Equal(
                    transacoesPersistidas[i].Tipo,
                    conta.Extrato[i].Tipo);
                Assert.Equal(
                    transacoesPersistidas[i].Valor,
                    conta.Extrato[i].Valor);
                Assert.Equal(
                    transacoesPersistidas[i].Data,
                    conta.Extrato[i].Data);
            }
        }
    }
}

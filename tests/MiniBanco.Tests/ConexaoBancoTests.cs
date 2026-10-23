
using MiniBanco.ConsoleApp.Data;
using MiniBanco.ConsoleApp.Repositories;
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
}

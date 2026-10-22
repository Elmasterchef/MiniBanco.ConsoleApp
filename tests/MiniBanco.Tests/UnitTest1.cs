
using MiniBanco.ConsoleApp.Models;
using MiniBanco.ConsoleApp.Exceptions;
using Xunit;

namespace MiniBanco.Tests;

public class ContaTests
{
    [Fact]
    public void Depositar_DeveAumentarOSaldo()
    {
        // Arrange (preparação)
        var conta = new Conta("Rafael", 1, 0);

        // Act (ação)
        conta.Depositar(100m, "Depósito de teste");

        // Assert (verificação)
        Assert.Equal(100m, conta.Saldo);
    }
    
    [Fact]
    public void Depositar_ComValorZero_DeveLancarExcecao()
    {
        // Arrange
        var conta = new Conta("Rafael", 1, 0);

        // Act e Assert
        Assert.Throws<ArgumentException>(
    () => conta.Depositar(0m, "Teste de depósito inválido"));

        // Confirma que o saldo permaneceu inalterado.
        Assert.Equal(0m, conta.Saldo);
    }
    
    [Fact]
    public void Sacar_ComSaldoInsuficiente_DeveLancarExcecao()
    {
        // Arrange
        var conta = new Conta("Rafael", 1, 0);

        conta.Depositar(50m, "Depósito para teste");

        // Act e Assert
        Assert.Throws<SaldoInsuficienteException>(
    () => conta.Sacar(100m));

        // O saldo deve permanecer intacto.
        Assert.Equal(50m, conta.Saldo);
    }
    
    [Fact]
    public void DepositarESacar_DeveRegistrarMovimentacoesNoExtrato()
    {
        // Arrange
        var conta = new Conta("Rafael", 1, 0);

        // Act
        conta.Depositar(100m, "Depósito inicial");
        conta.Sacar(30m);

        // Assert
        Assert.Equal(70m, conta.Saldo);
        Assert.Equal(2, conta.Extrato.Count);

        Assert.Equal("Deposito", conta.Extrato[0].Tipo);
        Assert.Equal(100m, conta.Extrato[0].Valor);

        Assert.Equal("Saque", conta.Extrato[1].Tipo);
        Assert.Equal(30m, conta.Extrato[1].Valor);
    }

    
[Fact]
public void ContaCorrente_AplicarFechamentoMensal_DeveDescontarTarifa()
{
    // Arrange
    var conta = new ContaCorrente(
        titular: "Rafael",
        numero: 10,
        saldoInicial: 100m);

    // Act
    conta.AplicarFechamentoMensal();

    // Assert
    Assert.Equal(90m, conta.Saldo);
    Assert.Single(conta.Extrato);
    Assert.Equal("Tarifa", conta.Extrato[0].Tipo);
    Assert.Equal(10m, conta.Extrato[0].Valor);
}

[Fact]
public void ContaPoupanca_AplicarFechamentoMensal_DeveAplicarRendimento()
{
    // Arrange
    var conta = new ContaPoupanca(
        titular: "Rafael",
        numero: 20,
        saldoInicial: 1000m,
        taxaRendimentoMensal: 1.0,
        diaAniversario: 10);

    // Act
    conta.AplicarFechamentoMensal();

    // Assert
    Assert.Equal(1010m, conta.Saldo);
    Assert.Single(conta.Extrato);
    Assert.Equal("Deposito", conta.Extrato[0].Tipo);
    Assert.Equal(10m, conta.Extrato[0].Valor);
}

[Fact]
public void ContaCorrente_SacarDentroDoLimite_DevePermitirSaque()
{
    // Arrange
    var conta = new ContaCorrente(
        titular: "Rafael",
        numero: 30,
        saldoInicial: 100m,
        limiteChequeEspecial: 200m);

    // Act
    conta.Sacar(250m);

    // Assert
    Assert.Equal(-150m, conta.Saldo);
    Assert.Single(conta.Extrato);
    Assert.Equal("Saque", conta.Extrato[0].Tipo);
    Assert.Equal(250m, conta.Extrato[0].Valor);
}

[Fact]
public void ContaCorrente_SacarAcimaDoLimite_DeveLancarExcecao()
{
    // Arrange
    var conta = new ContaCorrente(
        titular: "Rafael",
        numero: 31,
        saldoInicial: 100m,
        limiteChequeEspecial: 200m);

    // Act e Assert
    Assert.Throws<InvalidOperationException>(
        () => conta.Sacar(301m));

    Assert.Equal(100m, conta.Saldo);
    Assert.Empty(conta.Extrato);
}

[Fact]
public void ContaSalario_SaqueAcimaDoLimite_DeveCobrarTarifa()
{
    // Arrange
    var conta = new ContaSalario(
        titular: "Rafael",
        numero: 40,
        saldoInicial: 100m,
        empregador: "Empresa Teste",
        limiteSaquesMensais: 2);

    // Act
    conta.Sacar(10m);
    conta.Sacar(10m);
    conta.Sacar(10m);

    // Assert
    Assert.Equal(65m, conta.Saldo);
    Assert.Equal(4, conta.Extrato.Count);

    Assert.Equal("Tarifa", conta.Extrato[3].Tipo);
    Assert.Equal(5m, conta.Extrato[3].Valor);
}

[Fact]
public void ContaSalario_FechamentoMensal_DeveReiniciarContagemDeSaques()
{
    // Arrange
    var conta = new ContaSalario(
        titular: "Rafael",
        numero: 41,
        saldoInicial: 100m,
        empregador: "Empresa Teste",
        limiteSaquesMensais: 1);

    // Act
    conta.Sacar(10m);
    conta.Sacar(10m); // Ultrapassa o limite: tarifa de R$ 5.

    conta.AplicarFechamentoMensal();

    conta.Sacar(10m); // Primeiro saque do novo período.

    // Assert
    Assert.Equal(65m, conta.Saldo);
    Assert.Equal(4, conta.Extrato.Count);
    Assert.Equal("Saque", conta.Extrato[3].Tipo);
    Assert.Equal(10m, conta.Extrato[3].Valor);
}

[Fact]
public void Transacao_Deposito_DeveAtribuir10Pontos()
{
    Transacao transacao = new Transacao("Depósito inicial", "Deposito", 100m);

    Assert.Equal(10, transacao.Pontos);
}

[Fact]
public void Transacao_Saque_DeveAtribuir5Pontos()
{
    Transacao transacao = new Transacao("Saque", "Saque", 50m);

    Assert.Equal(5, transacao.Pontos);
}

[Fact]
public void Transacao_Tarifa_DeveAtribuirZeroPontos()
{
    Transacao transacao = new Transacao("Tarifa mensal", "Tarifa", 10m);

    Assert.Equal(0, transacao.Pontos);
}


}

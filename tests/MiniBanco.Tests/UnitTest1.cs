
using MiniBanco.ConsoleApp.Models;
using Xunit;

namespace MiniBanco.Tests;

public class ContaTests
{
    [Fact]
    public void Depositar_DeveAumentarOSaldo()
    {
        // Arrange (preparação)
        var conta = new Conta(
            titular: "Rafael",
            numero: "001");

        // Act (ação)
        conta.Depositar(100m);

        // Assert (verificação)
        Assert.Equal(100m, conta.Saldo);
    }
    
    [Fact]
    public void Depositar_ComValorZero_DeveLancarExcecao()
    {
        // Arrange
        var conta = new Conta(
            titular: "Rafael",
            numero: "001");

        // Act e Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => conta.Depositar(0m));

        // Confirma que o saldo permaneceu inalterado.
        Assert.Equal(0m, conta.Saldo);
    }
    
    [Fact]
    public void Sacar_ComSaldoInsuficiente_DeveLancarExcecao()
    {
        // Arrange
        var conta = new Conta(
            titular: "Rafael",
            numero: "001");

        conta.Depositar(50m);

        // Act e Assert
        Assert.Throws<InvalidOperationException>(
            () => conta.Sacar(100m));

        // O saldo deve permanecer intacto.
        Assert.Equal(50m, conta.Saldo);
    }
    
    [Fact]
    public void DepositarESacar_DeveRegistrarMovimentacoesNoExtrato()
    {
        // Arrange
        var conta = new Conta(
            titular: "Rafael",
            numero: "001");

        // Act
        conta.Depositar(100m);
        conta.Sacar(30m);

        // Assert
        Assert.Equal(70m, conta.Saldo);
        Assert.Equal(2, conta.Extrato.Count);

        Assert.Equal("Crédito", conta.Extrato[0].Tipo);
        Assert.Equal(100m, conta.Extrato[0].Valor);

        Assert.Equal("Débito", conta.Extrato[1].Tipo);
        Assert.Equal(30m, conta.Extrato[1].Valor);
    }



}

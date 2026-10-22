
using MiniBanco.ConsoleApp.Models;
using MiniBanco.ConsoleApp.Exceptions;
using MiniBanco.ConsoleApp.Services;
using System.Collections.Generic;
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
    Assert.Throws<SaldoInsuficienteException>(() => conta.Sacar(301m));

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
public void ContaSalario_SaldoInsuficienteParaTarifa_NaoDeveFicarNegativo()
{
    // Arrange
    var conta = new ContaSalario(
        titular: "Rafael",
        numero: 41,
        saldoInicial: 12m,
        empregador: "Empresa Teste",
        limiteSaquesMensais: 1);

    // Act
    conta.Sacar(10m);
    conta.Sacar(1m);

    // Assert
    Assert.Equal(1m, conta.Saldo);
    Assert.Equal(2, conta.Extrato.Count);
    Assert.DoesNotContain(
        conta.Extrato,
        transacao => transacao.Tipo == "Tarifa");
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


[Fact]
public void MotorAnalise_DeveCalcularPontosObtidos()
{
    Conta conta = new Conta("Rafael", 101);
    conta.Depositar(100m, "Depósito");
    conta.Sacar(20m);

    MotorAnalise motor = new MotorAnalise();

    Assert.Equal(15, motor.CalcularPontosObtidos(conta));
}

[Fact]
public void MotorAnalise_DeveCalcularPontuacaoMaxima()
{
    Conta conta = new Conta("Rafael", 101);
    conta.Depositar(100m, "Depósito");
    conta.Sacar(20m);

    MotorAnalise motor = new MotorAnalise();

    Assert.Equal(20, motor.CalcularPontuacaoMaxima(conta));
}


[Fact]
public void MotorAnalise_PercentualAlto_DeveClassificarComoContaSaudavel()
{
    Conta conta = new Conta("Rafael", 201);

    for (int i = 0; i < 9; i++)
    {
        conta.Depositar(10m, $"Depósito {i + 1}");
    }

    conta.Sacar(10m);

    MotorAnalise motor = new MotorAnalise();

    Assert.Equal("Conta Saudável", motor.ClassificarConta(conta));
}

[Fact]
public void MotorAnalise_PercentualIntermediario_DeveClassificarComoContaEmAtencao()
{
    Conta conta = new Conta("Rafael", 202);
    conta.Depositar(100m, "Depósito");
    conta.Sacar(10m);
    conta.Sacar(10m);

    MotorAnalise motor = new MotorAnalise();

    Assert.Equal("Conta em Atenção", motor.ClassificarConta(conta));
}

[Fact]
public void MotorAnalise_SemTransacoes_DeveClassificarComoContaEmRisco()
{
    Conta conta = new Conta("Rafael", 203);
    MotorAnalise motor = new MotorAnalise();

    Assert.Equal("Conta em Risco", motor.ClassificarConta(conta));
}

[Fact]
public void MotorAnalise_DeveIdentificarTarifasComoItensCriticos()
{
    Conta conta = new ContaCorrente("Rafael", 301, 100m, 0m);
    conta.Depositar(100m, "Depósito");
    conta.Sacar(10m);
    conta.AplicarFechamentoMensal();

    MotorAnalise motor = new MotorAnalise();

    List<Transacao> itensCriticos = motor.ObterItensCriticos(conta);

    Assert.Single(itensCriticos);
    Assert.Equal("Tarifa", itensCriticos[0].Tipo);
}

[Fact]
public void MotorAnalise_DeveIdentificarSaquesComoItensDeAtencao()
{
    Conta conta = new Conta("Rafael", 302);
    conta.Depositar(100m, "Depósito");
    conta.Sacar(10m);

    MotorAnalise motor = new MotorAnalise();

    List<Transacao> itensAtencao = motor.ObterItensAtencao(conta);

    Assert.Single(itensAtencao);
    Assert.Equal("Saque", itensAtencao[0].Tipo);
}

[Fact]
public void MotorAnalise_ContaSaudavel_DeveRecomendarInvestimentos()
{
    Conta conta = new Conta("Rafael", 401);

    for (int i = 0; i < 9; i++)
    {
        conta.Depositar(10m, $"Depósito {i + 1}");
    }

    conta.Sacar(10m);

    MotorAnalise motor = new MotorAnalise();

    List<string> recomendacoes = motor.GerarRecomendacoes(conta);

    Assert.Contains(
        "Oferecer opções de investimento.",
        recomendacoes);

    Assert.Contains(
        "Avaliar a possibilidade de aumento de limite.",
        recomendacoes);
}

[Fact]
public void MotorAnalise_ContaEmAtencao_DeveRecomendarOrientacaoFinanceira()
{
    Conta conta = new Conta("Rafael", 402);
    conta.Depositar(100m, "Depósito");
    conta.Sacar(10m);
    conta.Sacar(10m);

    MotorAnalise motor = new MotorAnalise();

    List<string> recomendacoes = motor.GerarRecomendacoes(conta);

    Assert.Contains(
        "Oferecer orientação financeira.",
        recomendacoes);

    Assert.Contains(
        "Revisar as tarifas cobradas.",
        recomendacoes);

    Assert.Contains(
        "Revisar a frequência dos saques e avaliar alternativas para movimentar a conta.",
        recomendacoes);
}

[Fact]
public void MotorAnalise_ContaEmRisco_DeveRecomendarAnaliseDeCredito()
{
    Conta conta = new Conta("Rafael", 403);
    MotorAnalise motor = new MotorAnalise();

    List<string> recomendacoes = motor.GerarRecomendacoes(conta);

    Assert.Contains(
        "Bloquear aumento de limite.",
        recomendacoes);

    Assert.Contains(
        "Realizar uma análise de crédito.",
        recomendacoes);
}
[Fact]
public void ContaCorrente_ComSaldoNegativo_DevePermitirReconstrucao()
{
    var conta = new ContaCorrente(
        titular: "Rafael",
        numero: 999,
        saldoInicial: -150m,
        limiteChequeEspecial: 200m);

    Assert.Equal(-150m, conta.Saldo);
}
[Fact]
public void ContaPoupanca_ComSaldoInicialNegativo_DeveLancarExcecao()
{
    Assert.Throws<ArgumentOutOfRangeException>(() =>
        new ContaPoupanca(
            titular: "Rafael",
            numero: 998,
            saldoInicial: -100m,
            taxaRendimentoMensal: 1.0,
            diaAniversario: 10));
}
[Fact]
public void ContaSalario_ComSaldoInicialNegativo_DeveLancarExcecao()
{
    Assert.Throws<ArgumentOutOfRangeException>(() =>
        new ContaSalario(
            titular: "Rafael",
            numero: 997,
            saldoInicial: -100m,
            empregador: "Empresa Teste",
            limiteSaquesMensais: 5));
}

}

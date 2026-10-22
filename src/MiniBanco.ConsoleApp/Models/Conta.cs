
using System;
using System.Collections.Generic;
using MiniBanco.ConsoleApp.Exceptions;

namespace MiniBanco.ConsoleApp.Models;

public class Conta
{
    private readonly List<Transacao> _extrato = new();

    public string Titular { get; }
    public int Numero { get; }
    public decimal Saldo { get; protected set; }

    public IReadOnlyList<Transacao> Extrato => _extrato.AsReadOnly();

    public Conta(string titular, int numero, decimal saldoInicial = 0)
    {
        if (string.IsNullOrWhiteSpace(titular))
            throw new ArgumentException(
                "O titular da conta é obrigatório.",
                nameof(titular));

        if (numero <= 0)
            throw new ArgumentException(
                "O número da conta deve ser maior que zero.",
                nameof(numero));

        if (saldoInicial < 0)
            throw new ArgumentException(
                "O saldo inicial não pode ser negativo.",
                nameof(saldoInicial));

        Titular = titular;
        Numero = numero;
        Saldo = saldoInicial;
    }

    public void Depositar(decimal valor, string descricao)
    {
        if (valor <= 0)
            throw new ArgumentException(
                "O valor do depósito deve ser maior que zero.",
                nameof(valor));

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException(
                "A descrição do depósito é obrigatória.",
                nameof(descricao));

        Saldo += valor;

        RegistrarTransacao(
            descricao,
            "Deposito",
            valor);
    }

    
public virtual void Sacar(decimal valor)
{
    if (valor <= 0)
        throw new ArgumentException(
            "O valor do saque deve ser maior que zero.",
            nameof(valor));

    if (valor > Saldo)
        throw new SaldoInsuficienteException(
            "Saldo insuficiente para realizar o saque.");

    Saldo -= valor;

        RegistrarTransacao(
        "Saque",
        "Saque",
        valor);
}

public virtual void AplicarFechamentoMensal()
    {
        // Cada tipo de conta poderá definir sua própria regra.
    }

    protected void RegistrarTransacao(
        string descricao,
        string tipo,
        decimal valor)
    {
        Transacao transacao = new Transacao(descricao, tipo, valor);
        _extrato.Add(transacao);
    }
}

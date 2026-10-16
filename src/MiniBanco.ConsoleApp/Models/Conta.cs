using System;
using System.Collections.Generic;

namespace MiniBanco.ConsoleApp.Models;

public class Conta
{
    private readonly List<Transacao> _extrato = new();

    public string Titular { get; }
    public string Numero { get; }
    public decimal Saldo { get; protected set; }

    public IReadOnlyList<Transacao> Extrato => _extrato.AsReadOnly();

    public Conta(string titular, string numero, decimal saldoInicial = 0)
    {
        if (string.IsNullOrWhiteSpace(titular))
            throw new ArgumentException(
                "O titular da conta é obrigatório.",
                nameof(titular));

        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException(
                "O número da conta é obrigatório.",
                nameof(numero));

        if (saldoInicial < 0)
            throw new ArgumentException(
                "O saldo inicial não pode ser negativo.",
                nameof(saldoInicial));

        Titular = titular;
        Numero = numero;
        Saldo = saldoInicial;
    }
    public void Depositar(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException(
                "O valor do depósito deve ser maior que zero.",
                nameof(valor));

        Saldo += valor;
        _extrato.Add(new Transacao("Depósito", "Crédito", valor));
    }
    public virtual void Sacar(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException(
                "O valor do saque deve ser maior que zero.",
                nameof(valor));

        if (valor > Saldo)
            throw new InvalidOperationException(
                "Saldo insuficiente para realizar o saque.");

        Saldo -= valor;
        _extrato.Add(new Transacao("Saque", "Débito", valor));
    }
    public virtual void AplicarFechamentoMensal()
    {
        // Cada tipo de conta poderá definir sua própria regra.
    }

    protected void RegistrarTransacao(Transacao transacao)
    {
        _extrato.Add(transacao);
    }
}

namespace MiniBanco.ConsoleApp.Models;

public class ContaSalario : Conta
{
    public string Empregador { get; }
    public int LimiteSaquesMensais { get; }
public int SaquesRealizadosNoMes => _saquesRealizadosNoMes;

private int _saquesRealizadosNoMes;

    public ContaSalario(
    string titular,
    int numero,
    decimal saldoInicial = 0,
    string empregador = "",
    int limiteSaquesMensais = 5,
    int saquesRealizadosNoMes = 0)
    : base(titular, numero, saldoInicial)
    {
        if (saldoInicial < 0)
    throw new ArgumentOutOfRangeException(
        nameof(saldoInicial),
        "O saldo inicial da conta-salário não pode ser negativo.");

        if (string.IsNullOrWhiteSpace(empregador))
            throw new ArgumentException(
                "O empregador é obrigatório.",
                nameof(empregador));

        if (limiteSaquesMensais < 0)
            throw new ArgumentOutOfRangeException(
                nameof(limiteSaquesMensais),
                "O limite de saques não pode ser negativo.");
        if (saquesRealizadosNoMes < 0)
    throw new ArgumentOutOfRangeException(
        nameof(saquesRealizadosNoMes),
        "A quantidade de saques realizados não pode ser negativa.");

        Empregador = empregador;
        LimiteSaquesMensais = limiteSaquesMensais;
        _saquesRealizadosNoMes = saquesRealizadosNoMes;
    }

    
public override void Sacar(decimal valor)
{
    if (valor <= 0)
        throw new ArgumentException(
            "O valor do saque deve ser maior que zero.",
            nameof(valor));

    if (valor > Saldo)
        throw new MiniBanco.ConsoleApp.Exceptions.SaldoInsuficienteException(
            "Saldo insuficiente para realizar o saque.");

    base.Sacar(valor);

    _saquesRealizadosNoMes++;

    
if (_saquesRealizadosNoMes > LimiteSaquesMensais)
{
    if (Saldo >= 5m)
    {
        Saldo -= 5m;

        RegistrarTransacao(
            "Tarifa por exceder o limite mensal de saques",
            "Tarifa",
            5m);
    }
}

}


public override void AplicarFechamentoMensal()
    {
        _saquesRealizadosNoMes = 0;
    }
}

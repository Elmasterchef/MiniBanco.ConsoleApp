
namespace MiniBanco.ConsoleApp.Models;

public class ContaSalario : Conta
{
    public string Empregador { get; }
    public int LimiteSaquesMensais { get; }
    private int _saquesRealizadosNoMes;

    public ContaSalario(
    string titular,
    int numero,
    decimal saldoInicial = 0,
    string empregador = "",
    int limiteSaquesMensais = 5)
    : base(titular, numero, saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(empregador))
            throw new ArgumentException(
                "O empregador é obrigatório.",
                nameof(empregador));

        if (limiteSaquesMensais < 0)
            throw new ArgumentOutOfRangeException(
                nameof(limiteSaquesMensais),
                "O limite de saques não pode ser negativo.");

        Empregador = empregador;
        LimiteSaquesMensais = limiteSaquesMensais;
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
        Saldo -= 5m;

        RegistrarTransacao(
            "Tarifa por exceder o limite mensal de saques",
            "Tarifa",
            5m);
    }
    }


public override void AplicarFechamentoMensal()
    {
        _saquesRealizadosNoMes = 0;
    }
}

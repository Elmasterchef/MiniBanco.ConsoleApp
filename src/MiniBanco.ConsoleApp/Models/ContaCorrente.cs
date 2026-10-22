
namespace MiniBanco.ConsoleApp.Models;

public class ContaCorrente : Conta
{
    public decimal LimiteChequeEspecial { get; }

    public ContaCorrente(
    string titular,
    int numero,
    decimal saldoInicial = 0,
    decimal limiteChequeEspecial = 0)
    : base(titular, numero, saldoInicial)
    {
        if (limiteChequeEspecial < 0)
            throw new ArgumentOutOfRangeException(
                nameof(limiteChequeEspecial),
                "O limite não pode ser negativo.");

        LimiteChequeEspecial = limiteChequeEspecial;
    }

    public override void Sacar(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(valor),
                "O saque deve ser maior que zero.");

        if (valor > Saldo + LimiteChequeEspecial)
            throw new InvalidOperationException(
                "Saldo e limite de cheque especial insuficientes.");

        Saldo -= valor;

      RegistrarTransacao(
    "Saque em conta corrente",
    "Saque",
    valor);
    }

    public override void AplicarFechamentoMensal()
{
    Saldo -= 10m;

    RegistrarTransacao(
        "Tarifa de manutenção mensal",
        "Tarifa",
        10m);
}
}


namespace MiniBanco.ConsoleApp.Models;

public class ContaCorrente : Conta
{
    public decimal LimiteChequeEspecial { get; }

    public ContaCorrente(
        string titular,
        string numero,
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
            new Transacao("Saque em conta corrente", "Saque", valor));
    }

    public override void AplicarFechamentoMensal()
    {
        
    }
}

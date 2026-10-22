
namespace MiniBanco.ConsoleApp.Models;

public class ContaPoupanca : Conta
{
    public double TaxaRendimentoMensal { get; }
    public int DiaAniversario { get; }

    public ContaPoupanca(
    string titular,
    int numero,
    decimal saldoInicial = 0,
    double taxaRendimentoMensal = 0,
    int diaAniversario = 1)
    : base(titular, numero, saldoInicial)
    {
        if (taxaRendimentoMensal < 0)
            throw new ArgumentOutOfRangeException(
                nameof(taxaRendimentoMensal),
                "A taxa de rendimento não pode ser negativa.");

        if (diaAniversario < 1 || diaAniversario > 31)
            throw new ArgumentOutOfRangeException(
                nameof(diaAniversario),
                "O dia de aniversário deve estar entre 1 e 31.");

        TaxaRendimentoMensal = taxaRendimentoMensal;
        DiaAniversario = diaAniversario;
    }

    public override void AplicarFechamentoMensal()
{
    decimal rendimento = Saldo *
        (decimal)(TaxaRendimentoMensal / 100.0);

    if (rendimento > 0)
    {
        Saldo += rendimento;

        RegistrarTransacao(
            "Rendimento mensal da poupança",
            "Deposito",
            rendimento);
    }
}
}

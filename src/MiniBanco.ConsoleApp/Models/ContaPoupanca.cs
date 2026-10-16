
namespace MiniBanco.ConsoleApp.Models;

public class ContaPoupanca : Conta
{
    public decimal TaxaRendimentoMensal { get; }
    public int DiaAniversario { get; }

    public ContaPoupanca(
        string titular,
        string numero,
        decimal saldoInicial = 0,
        decimal taxaRendimentoMensal = 0,
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
        // Implementaremos o rendimento após confirmar a regra do enunciado.
    }
}

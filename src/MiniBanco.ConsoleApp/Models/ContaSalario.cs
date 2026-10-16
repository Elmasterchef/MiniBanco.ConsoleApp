
namespace MiniBanco.ConsoleApp.Models;

public class ContaSalario : Conta
{
    public string Empregador { get; }
    public int LimiteSaquesMensais { get; }

    public ContaSalario(
        string titular,
        string numero,
        string empregador,
        decimal saldoInicial = 0,
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
        // A validação do limite mensal será implementada
        // na próxima etapa, junto com o controle dos saques.
        base.Sacar(valor);
    }

    public override void AplicarFechamentoMensal()
    {
        // Implementaremos conforme as regras do enunciado.
    }
}

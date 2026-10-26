
namespace MiniBanco.ConsoleApp.Models;

public class Transacao
{
    public string Descricao { get; }
    public string Tipo { get; }
    public decimal Valor { get; }
    public DateTime Data { get; }

    public Transacao(
        string descricao,
        string tipo,
        decimal valor)
        : this(descricao, tipo, valor, DateTime.Now)
    {
    }

    public Transacao(
        string descricao,
        string tipo,
        decimal valor,
        DateTime data)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException(
                "A descrição da transação é obrigatória.",
                nameof(descricao));

        if (tipo != "Deposito" &&
            tipo != "Saque" &&
            tipo != "Tarifa" &&
            tipo != "Rendimento")
        {
            throw new ArgumentException(
                "O tipo deve ser Deposito, Saque, Tarifa ou Rendimento.",
                nameof(tipo));
        }

        if (valor <= 0)
            throw new ArgumentException(
                "O valor da transação deve ser maior que zero.",
                nameof(valor));

        Descricao = descricao;
        Tipo = tipo;
        Valor = valor;
        Data = data;
    }

    public int Pontos
    {
        get
        {
            if (Tipo == "Deposito")
                return 10;

            if (Tipo == "Saque")
                return 5;

            return 0;
        }
    }
}

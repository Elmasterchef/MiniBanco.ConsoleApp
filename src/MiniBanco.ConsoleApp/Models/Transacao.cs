namespace MiniBanco.ConsoleApp.Models;

public class Transacao
{
    public string Descricao { get; }
    public string Tipo { get; }
    public decimal Valor { get; }
    public DateTime Data { get; }
    public Transacao(string descricao, string tipo, decimal valor)
    {
        if(string.IsNullOrWhiteSpace(descricao))
          throw new ArgumentException(
            "A descrição da transação é obrigatoria",
          nameof(descricao));

          if(string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException(
              "O tipo da transação é obrigatório.",
              nameof(tipo));

          if(valor <= 0)
            throw new ArgumentException(
              "O valor da transação deve ser maior que zero.",
              nameof(valor));
          Descricao = descricao;
          Tipo = tipo;
          Valor = valor;
          Data = DateTime.Now;
       
    }
}
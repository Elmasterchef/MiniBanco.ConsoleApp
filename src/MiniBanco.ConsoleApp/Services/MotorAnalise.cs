
using MiniBanco.ConsoleApp.Models;

namespace MiniBanco.ConsoleApp.Services;

public class MotorAnalise
{
    public int CalcularPontosObtidos(Conta conta)
    {
        int pontos = 0;

        foreach (Transacao transacao in conta.Extrato)
        {
            pontos += transacao.Pontos;
        }

        return pontos;
    }

    public int CalcularPontuacaoMaxima(Conta conta)
    {
        return conta.Extrato.Count * 10;
    }

    public double CalcularPercentualSaude(Conta conta)
    {
        int pontuacaoMaxima = CalcularPontuacaoMaxima(conta);

        if (pontuacaoMaxima == 0)
        {
            return 0;
        }

        int pontosObtidos = CalcularPontosObtidos(conta);

        return (double)pontosObtidos / pontuacaoMaxima * 100;
    }
}

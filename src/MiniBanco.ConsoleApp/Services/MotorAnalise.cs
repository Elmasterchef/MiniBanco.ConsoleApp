
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
    
public string ClassificarConta(Conta conta)
{
    double percentual = CalcularPercentualSaude(conta);

    if (percentual >= 90)
    {
        return "Conta Saudável";
    }

    if (percentual >= 60)
    {
        return "Conta em Atenção";
    }

    return "Conta em Risco";
}

public List<Transacao> ObterItensCriticos(Conta conta)
{
    List<Transacao> itensCriticos = new List<Transacao>();

    foreach (Transacao transacao in conta.Extrato)
    {
        if (transacao.Tipo == "Tarifa")
        {
            itensCriticos.Add(transacao);
        }
    }

    return itensCriticos;
}

public List<Transacao> ObterItensAtencao(Conta conta)
{
    List<Transacao> itensAtencao = new List<Transacao>();

    foreach (Transacao transacao in conta.Extrato)
    {
        if (transacao.Tipo == "Saque")
        {
            itensAtencao.Add(transacao);
        }
    }

    return itensAtencao;
}

}

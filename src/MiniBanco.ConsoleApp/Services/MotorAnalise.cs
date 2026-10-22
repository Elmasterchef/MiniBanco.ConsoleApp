
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

public List<string> GerarRecomendacoes(Conta conta)
{
    List<string> recomendacoes = new List<string>();

    string classificacao = ClassificarConta(conta);
    List<Transacao> itensCriticos = ObterItensCriticos(conta);
    List<Transacao> itensAtencao = ObterItensAtencao(conta);

    if (classificacao == "Conta Saudável")
    {
        recomendacoes.Add("Oferecer opções de investimento.");
        recomendacoes.Add("Avaliar a possibilidade de aumento de limite.");
    }
    else if (classificacao == "Conta em Atenção")
    {
        recomendacoes.Add("Entrar em contato com a gestão da conta.");
        recomendacoes.Add("Oferecer orientação financeira.");
        recomendacoes.Add("Revisar as tarifas cobradas.");
    }
    else
    {
        recomendacoes.Add("Bloquear aumento de limite.");
        recomendacoes.Add("Realizar uma análise de crédito.");
    }

    if (itensCriticos.Count > 0)
    {
        recomendacoes.Add(
            "Analisar as tarifas registradas e avaliar como reduzir esses custos.");
    }

    if (itensAtencao.Count > 0)
    {
        recomendacoes.Add(
            "Revisar a frequência dos saques e avaliar alternativas para movimentar a conta.");
    }

    if (recomendacoes.Count == 0)
    {
        recomendacoes.Add("Nenhuma recomendação disponível.");
    }

    return recomendacoes;
}

}

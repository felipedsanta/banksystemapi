using Azure;
using BankSystem.Api.Repositories;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text;

namespace BankSystem.Api.Services;

public class FinancialAdvisorService(
    ITransacaoRepository transacaoRepo,
    IConfiguration config,
    IContaRepository contaRepo) : IFinancialAdvisorService
{
    public async Task<string?> GerarAnaliseFinanceiraAsync(Guid contaId)
    {
        var conta = await contaRepo.GetByIdAsync(contaId);
        if (conta == null)
        {
            return null;
        }

        var transacoes = (await transacaoRepo.GetExtratoByContaIdAsync(contaId)).Take(20);

        if (!transacoes.Any()) return "Você ainda não possui transações para analisar.";

        var sb = new StringBuilder();
        sb.AppendLine("Você é um consultor financeiro experiente e amigável de um banco digital.");
        sb.AppendLine("Analise o seguinte extrato bancário e forneça:");
        sb.AppendLine("1. Um resumo dos gastos.");
        sb.AppendLine("2. Dicas para economizar baseadas nos padrões de compra.");
        sb.AppendLine("3. Um tom de voz encorajador.");
        sb.AppendLine("Não invente dados. Use apenas o que está abaixo:");
        sb.AppendLine("--- EXTRATO ---");

        foreach (var t in transacoes)
        {
            sb.AppendLine($"{t.DataHora:dd/MM} | {t.Tipo} | R$ {t.Valor:F2}");
        }

        var endpoint = new Uri(config["AzureOpenAI:Endpoint"]!);
        var key = new AzureKeyCredential(config["AzureOpenAI:ApiKey"]!);
        var azureClient = new AzureOpenAIClient(endpoint, key);

        var chatClient = azureClient.GetChatClient(config["AzureOpenAI:DeploymentName"]!);
        
        var messages = new List<ChatMessage>
        {
        new SystemChatMessage("Você é um assistente bancário útil."),
        new UserChatMessage(sb.ToString())
        };

        try
        {
            ChatCompletion completion = await chatClient.CompleteChatAsync(messages);

            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            return "No momento nosso consultor inteligente está indisponível. Tente mais tarde." + ex;
        }
    }
}
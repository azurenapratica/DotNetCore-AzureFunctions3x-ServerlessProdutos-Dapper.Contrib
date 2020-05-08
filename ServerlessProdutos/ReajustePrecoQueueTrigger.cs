using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServerlessProdutos.Business;

namespace ServerlessProdutos
{
    public static class ReajustePrecoQueueTrigger
    {
        [FunctionName("ReajustePrecoQueueTrigger")]
        public static void Run([QueueTrigger("queue-reajustepreco", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation("Acessada a Function ReajustePrecoQueueTrigger");
            log.LogInformation($"Dados: {myQueueItem}");

            try
            {
                if (ProdutoServices.UpdateReajustePreco(myQueueItem))
                    log.LogInformation($"ReajustePrecoQueueTrigger - Ajuste efetuado com sucesso");
                else
                    log.LogError($"ReajustePrecoQueueTrigger - Erro de validação");
            }
            catch
            {
                log.LogError($"ReajustePrecoQueueTrigger - Erro durante o processamento");
            }
        }
    }
}
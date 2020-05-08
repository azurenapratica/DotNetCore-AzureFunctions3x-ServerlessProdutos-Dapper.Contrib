using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServerlessProdutos.Business;

namespace ServerlessProdutos
{
    public static class Produtos
    {
        [FunctionName("Produtos")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Acessada a Function Produto");
            log.LogInformation($"Operação: {req.Method}");

            switch (req.Method)
            {
                case "GET":
                    return ProdutoServices.Get(req.Query["codigo"]);
                case "POST":
                    return ProdutoServices.Insert(new StreamReader(req.Body).ReadToEndAsync().Result);
                case "PUT":
                    return ProdutoServices.Update(new StreamReader(req.Body).ReadToEndAsync().Result);
                case "DELETE":
                    return ProdutoServices.Delete(req.Query["codigo"]);
            }

            return new BadRequestResult();
        }
    }
}
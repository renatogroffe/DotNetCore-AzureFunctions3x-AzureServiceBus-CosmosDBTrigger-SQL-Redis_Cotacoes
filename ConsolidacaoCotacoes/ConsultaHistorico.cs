using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ConsolidacaoCotacoes.Data;

namespace ConsolidacaoCotacoes
{
    public class ConsultaHistorico
    {
        private readonly CotacoesRepository _repository;

        public ConsultaHistorico(CotacoesRepository repository)
        {
            _repository = repository;
        }

        [FunctionName("ConsultaHistorico")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var historico = _repository.ListHistoricoDolar();
            log.LogInformation($"HistoricoDolar - HTTP trigger: {historico.Count()} registro(s) encontrado(s)");

            return new OkObjectResult(historico);
        }
    }
}
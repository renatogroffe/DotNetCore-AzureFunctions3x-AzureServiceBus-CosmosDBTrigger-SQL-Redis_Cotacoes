using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ConsolidacaoCotacoes.Data;

namespace ConsolidacaoCotacoes
{
    public class ConsolidarDadosCotacao
    {
        private readonly CotacoesRepository _repository;

        public ConsolidarDadosCotacao(CotacoesRepository repository)
        {
            _repository = repository;
        }

        [FunctionName("ConsolidarDadosCotacao")]
        public void Run([CosmosDBTrigger(
            databaseName: "DBCotacoes",
            collectionName: "HistoricoMoedas",
            ConnectionStringSetting = "CosmosDBConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var doc in input)
                {
                    var document = _repository.GetCotacao(doc.Id);
                    log.LogInformation($"Dados: {JsonSerializer.Serialize(document)}");

                    _repository.Save(document);
                    if (document.id.StartsWith("USD"))
                        _repository.SaveHistoricoDolar(document);

                    log.LogInformation("Cotação registrada com sucesso");
                }
            }
        }
    }
}
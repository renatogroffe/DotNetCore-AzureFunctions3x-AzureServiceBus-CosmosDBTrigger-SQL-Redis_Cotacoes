using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Dapper.Contrib.Extensions;
using StackExchange.Redis;
using ConsolidacaoCotacoes.Models;
using ConsolidacaoCotacoes.Documents;

namespace ConsolidacaoCotacoes.Data
{
    public class CotacoesRepository
    {
        private readonly ConnectionMultiplexer _conexaoRedis;
        private readonly string _prefixoCotacaoRedis;
        private readonly string _baseCotacoesConnectionString;
        private readonly string _DBCotacoesEndpointUri;
        private readonly string _DBCotacoesEndpointPrimaryKey;

        public CotacoesRepository(IConfiguration configuration)
        {
            _conexaoRedis = ConnectionMultiplexer
                .Connect(configuration["RedisConnectionString"]);
            _prefixoCotacaoRedis = configuration["PrefixoCotacaoRedis"];
            _baseCotacoesConnectionString = configuration["BaseCotacoesConnectionString"];
            _DBCotacoesEndpointUri = configuration["DBCotacoesEndpointUri"];
            _DBCotacoesEndpointPrimaryKey = configuration["DBCotacoesEndpointPrimaryKey"];
        }

        public CotacaoMoedaDocument GetCotacao(string id)
        {
            using (var client = new DocumentClient(
                new Uri(_DBCotacoesEndpointUri), _DBCotacoesEndpointPrimaryKey))
            {
                FeedOptions queryOptions =
                    new FeedOptions { MaxItemCount = -1 };

                return client.CreateDocumentQuery<CotacaoMoedaDocument>(
                        UriFactory.CreateDocumentCollectionUri(
                            "DBCotacoes", "HistoricoMoedas"),
                            "SELECT * FROM HistoricoMoedas h " +
                           $"WHERE h.id = '{id}'", queryOptions)
                        .ToArray()[0];
            }
        }

        public void Save(CotacaoMoedaDocument document)
        {
            var dbRedis = _conexaoRedis.GetDatabase();
            dbRedis.StringSet(
                $"{_prefixoCotacaoRedis}-{document.Sigla}",
                JsonSerializer.Serialize(document, new JsonSerializerOptions()
                {
                    IgnoreNullValues = true
                }),
                expiry: null);
        }

        public void SaveHistoricoDolar(CotacaoMoedaDocument document)
        {
            using (var conexao = new SqlConnection(_baseCotacoesConnectionString))
            {
                conexao.Insert(new HistoricoDolar()
                {
                    CodReferencia = document.id,
                    DataReferencia = document.DataReferencia,
                    Valor = document.Valor
                });
            }
        }

        public IEnumerable<HistoricoDolar> ListHistoricoDolar()
        {
            using (var conexao = new SqlConnection(_baseCotacoesConnectionString))
            {
                return conexao.Query<HistoricoDolar>(
                    "SELECT * FROM dbo.HistoricoDolar " +
                    "ORDER BY  DataReferencia DESC");
            }
        }
    }
}
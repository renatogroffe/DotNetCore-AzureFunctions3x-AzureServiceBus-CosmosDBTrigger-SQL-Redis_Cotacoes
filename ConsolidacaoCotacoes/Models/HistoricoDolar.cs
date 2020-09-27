using Dapper.Contrib.Extensions;

namespace ConsolidacaoCotacoes.Models
{
    [Table("dbo.HistoricoDolar")]
    public class HistoricoDolar
    {
        [Key]
        public int Id { get; set; }
        public string CodReferencia { get; set; }
        public string DataReferencia { get; set; }
        public double Valor { get; set; }
    }
}
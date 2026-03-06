namespace Exchange.Infrastructure.Services.Bacen.Responses
{
    public class CurrencyBacenResponse
    {
        public List<CurrencyItemBacen> Value { get; set; } = new();
    }

    public class CurrencyItemBacen
    {
        public string Simbolo { get; set; } = string.Empty;
        public decimal ParidadeCompra { get; set; }
        public decimal ParidadeVenda { get; set; }
        public decimal CotacaoCompra { get; set; }
        public decimal CotacaoVenda { get; set; }
        public string DataHoraCotacao { get; set; } = string.Empty;
        public string TipoBoletim { get; set; } = string.Empty;
    }
}

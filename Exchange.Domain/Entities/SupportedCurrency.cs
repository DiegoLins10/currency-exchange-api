namespace Exchange.Domain.Entities
{
    public class SupportedCurrency
    {
        public string Symbol { get; }
        public string Name { get; }

        public SupportedCurrency(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }
}

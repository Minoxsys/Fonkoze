using System;

namespace Web.ReceiveSmsUseCase.Models
{
    public class ParsedProduct : IEquatable<ParsedProduct>, IParsedProduct
    {
        public string ProductGroupCode { get; set; }
        public string ProductCode { get; set; }
        public int StockLevel { get; set; }
        public string IsClientIdentifier { get; set; }

        public bool Equals(ParsedProduct other)
        {
            return ProductGroupCode == other.ProductGroupCode && ProductCode == other.ProductCode && StockLevel == other.StockLevel &&
                   IsClientIdentifier == other.IsClientIdentifier;
        }
    }
}

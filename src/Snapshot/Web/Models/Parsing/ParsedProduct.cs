using System;

namespace Web.Models.Parsing
{
    public class ParsedProduct : IParsedProduct
    {
        public string ProductGroupCode { get; set; }
        public string ProductCode { get; set; }
        public int StockLevel { get; set; }
        public string ClientIdentifier { get; set; }

        public override bool Equals(Object other)
        {
            // Performs an equality check on two points (integer pairs). 
            if (other == null || GetType() != other.GetType()) return false;
            var p = (ParsedProduct) other;
            return ProductGroupCode == p.ProductGroupCode && ProductCode == p.ProductCode && StockLevel == p.StockLevel &&
                   ClientIdentifier == p.ClientIdentifier;
        }

        public override int GetHashCode()
        {
            return ProductCode.GetHashCode() ^ StockLevel.GetHashCode();
        }
    }
}

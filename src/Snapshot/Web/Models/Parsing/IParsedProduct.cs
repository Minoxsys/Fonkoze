namespace Web.Models.Parsing
{
    public interface IParsedProduct
    {
        string ProductGroupCode { get; set; }
        string ProductCode { get; set; }
        int StockLevel { get; set; }
        string ClientIdentifier { get; set; }
    }
}
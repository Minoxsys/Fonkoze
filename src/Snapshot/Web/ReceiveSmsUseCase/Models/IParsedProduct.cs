namespace Web.ReceiveSmsUseCase.Models
{
    public interface IParsedProduct
    {
        string ProductGroupCode { get; set; }
        string ProductCode { get; set; }
        int StockLevel { get; set; }
    }
}
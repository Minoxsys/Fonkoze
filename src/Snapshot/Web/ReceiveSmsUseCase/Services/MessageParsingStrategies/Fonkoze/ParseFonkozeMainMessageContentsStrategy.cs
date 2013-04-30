namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies.Fonkoze
{
    public class ParseFonkozeMainMessageContentsStrategy : ParseMainMessageContentsStrategy
    {
        private const string GenericProductGroupCode = "ALL";
        private const int ProductCodeLength = 2;

        protected override string GetProductGroupCode(string message)
        {
            return GenericProductGroupCode;
        }

        protected override string GetProductCode(string message)
        {
            return message.Substring(0, ProductCodeLength);
        }

        protected override string GetStockLevelString(string message)
        {
            return message.Substring(ProductCodeLength, message.Length - 1 - ProductCodeLength);
        }
    }
}
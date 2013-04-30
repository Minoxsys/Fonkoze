namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies.Fonkoze
{
    public class ParseFonkozeMainMessageContentsStrategy : ParseMainMessageContentsStrategy
    {
        private const string GenericProductGroupCode = "ALL";

        protected override string GetProductGroupCode(string message)
        {
            return GenericProductGroupCode;
        }

        protected override string GetProductCode(string message)
        {
            return message.Substring(0, 2);
        }

        protected override string GetStockLevelString(string message)
        {
            return message.Substring(2, message.Length - 1 - 2);
        }
    }
}
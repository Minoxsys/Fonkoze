using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies.Fonkoze
{
    public class ParseFonkozeMainMessageContentsWithGroupsAndTwoLetterProductCodeStrategy : ParseMainMessageContentsStrategy
    {
        private const int ProductCodeStartIndex = 3;
        private const int ProductCodeLength = 2;

        protected override string GetProductCode(string message)
        {
            return message.Substring(ProductCodeStartIndex, ProductCodeLength);
        }

        protected override string GetStockLevelString(string message)
        {
            return message.Substring(ProductCodeLength + ProductCodeStartIndex, message.Length - 1 - (ProductCodeLength + ProductCodeStartIndex));
        }
    }
}
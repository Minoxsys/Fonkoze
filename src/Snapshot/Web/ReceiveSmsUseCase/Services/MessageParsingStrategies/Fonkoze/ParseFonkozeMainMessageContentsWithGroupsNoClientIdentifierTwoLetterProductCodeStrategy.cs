using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies.Fonkoze
{
    public class ParseFonkozeMainMessageContentsWithGroupsNoClientIdentifierTwoLetterProductCodeStrategy : ParseMainMessageContentsStrategy
    {
        private const int ProductCodeStartIndex = 3;
        private const int ProductCodeLength = 2;

        protected override string GetProductCode(string message)
        {
            return message.Substring(ProductCodeStartIndex, ProductCodeLength);
        }

        protected override string GetStockLevelString(string message)
        {
            return message.Substring(ProductCodeLength + ProductCodeStartIndex, message.Length - (ProductCodeLength + ProductCodeStartIndex));
        }

        protected override bool IsValidClientIdefifier(string identifier)
        {
            return string.IsNullOrEmpty(identifier);
        }

        protected override string GetClientIdentifier(string message)
        {
            return string.Empty;
        }

        protected override bool TryParseToken(string token, out Web.Models.Parsing.ParsedProduct parsedProduct)
        {
            var parseResult = base.TryParseToken(token, out parsedProduct);
            if (ContainsClientIdentifier(token))
            {
                parseResult = false;
            }
            return parseResult;
        }

        private bool ContainsClientIdentifier(string token)
        {
            var lastChar = token.Substring(token.Length - 1, 1)[0];
            return lastChar < '0' || lastChar > '9';
        }
    }
}
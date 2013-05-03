using System;
using System.Text.RegularExpressions;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    public class ParseMainMessageContentsStrategy : ISmsParsingStrategy
    {
        private readonly MessageParsingHelpers _parsingHelper = new MessageParsingHelpers();

        public SmsParseResult Parse(string message)
        {
            var result = new SmsParseResult();
            var tokens = message.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                ParsedProduct parsedProduct;
                if (!TryParseToken(token, out parsedProduct))
                {
                    return _parsingHelper.CreateInvalidMessageFormatResponse();
                }

                result.ParsedProducts.Add(parsedProduct);
            }

            result.Success = true;
            return result;
        }

        protected bool TryParseToken(string token, out ParsedProduct parsedProduct)
        {
            bool parseResult = true;
            ParsedProduct product = null;
            try
            {
                product = new ParsedProduct
                    {
                        ProductGroupCode = GetProductGroupCode(token),
                        ProductCode = GetProductCode(token),
                        ClientIdentifier = GetClientIdentifier(token),
                    };

                int stockLevel;
                if (ValidContents(token, product, out stockLevel))
                {
                    product.StockLevel = stockLevel;
                }
                else
                {
                    parseResult = false;
                    parsedProduct = null;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                parseResult = false;
            }

            parsedProduct = product;
            return parseResult;
        }

        protected virtual bool ValidContents(string token, ParsedProduct product, out int stockLevel)
        {
            return int.TryParse(GetStockLevelString(token), out stockLevel) && ContainsOnlyLetters(product.ProductGroupCode) &&
                   ContainsOnlyLetters(product.ProductCode) && IsValidClientIdefifier(product.ClientIdentifier);
        }

        protected virtual string GetClientIdentifier(string message)
        {
            return message.Substring(message.Length - 1, 1);
        }

        protected virtual string GetProductCode(string message)
        {
            return message.Substring(3, 1);
        }

        protected virtual string GetProductGroupCode(string message)
        {
            return message.Substring(0, 3);
        }

        protected virtual bool IsValidClientIdefifier(string identifier)
        {
            var regex = new Regex("^[f,n,F,N]{1}$");
            return regex.IsMatch(identifier);
        }

        protected bool ContainsOnlyLetters(string code)
        {
            var regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(code);
        }

        protected virtual string GetStockLevelString(string message)
        {
            return message.Substring(4, message.Length - 1 - 4);
        }
    }
}
using System;
using System.Text.RegularExpressions;
using Web.LocalizationResources;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    internal class ParseStockSaleMessageStrategy : ISmsParsingStrategy
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
                    if (result.ParsedProducts.Count > 0)
                    {
                        return CreateAtLeastOneProductWrongResponse();
                    }
                    return _parsingHelper.CreateInvalidMessageFormatResponse();
                }

                result.ParsedProducts.Add(parsedProduct);
            }

            result.Success = true;
            result.MessageType = MessageType.StockSale;
            return result;
        }

        private bool TryParseToken(string token, out ParsedProduct parsedProduct)
        {
            bool parseResult = true;
            ParsedProduct product = null;
            try
            {
                product = new ParsedProduct
                    {
                        ProductGroupCode = GetProductGroupCode(token),
                        ProductCode = GetProductCode(token),
                        IsClientIdentifier = GetIsClientIdentifier(token),
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

        private bool ValidContents(string token, ParsedProduct product, out int stockLevel)
        {
            return int.TryParse(GetStockLevelString(token), out stockLevel) && ContainsOnlyLetters(product.ProductGroupCode) &&
                   ContainsOnlyLetters(product.ProductCode) && IsValidClientIdefifier(product.IsClientIdentifier);
        }

        private SmsParseResult CreateAtLeastOneProductWrongResponse()
        {
            return _parsingHelper.CreateErrorMessage(Strings.At_least_one_product_specification_is_invalid);
        }

        private string GetIsClientIdentifier(string message)
        {
            return message.Substring(message.Length - 1, 1);
        }

        private string GetProductCode(string message)
        {
            return message.Substring(3, 1);
        }

        private string GetProductGroupCode(string message)
        {
            return message.Substring(0, 3);
        }

        private bool IsValidClientIdefifier(string identifier)
        {
            var regex = new Regex("^[f,n,F,N]{1}$");
            return regex.IsMatch(identifier);
        }

        private bool ContainsOnlyLetters(string code)
        {
            var regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(code);
        }

        private string GetStockLevelString(string message)
        {
            return message.Substring(4, message.Length - 1 - 4);
        }
    }
}
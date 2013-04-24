using System;
using System.Text.RegularExpressions;
using Web.LocalizationResources;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public class SmsTextParserService : ISmsTextParserService
    {
        private const int ValidMessageMinimumLength = 6;
        private const string ActivationMessage = "activate";

        public SmsParseResult Parse(string message)
        {
            message = message.Trim();
            var result = new SmsParseResult();

            if (string.IsNullOrEmpty(message))
            {
                return CreateInvalidMessageFormatResponse();
            }
            if (message.Length < ValidMessageMinimumLength)
            {
                return CreateInvalidMessageFormatResponse();
            }

            if (string.Compare(message.Trim(new[] {'.', ',', ';'}), ActivationMessage, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return CreateActivationMessageResponse();
            }

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
                    return CreateInvalidMessageFormatResponse();
                }

                result.ParsedProducts.Add(parsedProduct);
            }

            result.Success = true;
            result.MessageType = MessageType.StockUpdate;
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

        private SmsParseResult CreateActivationMessageResponse()
        {
            return new SmsParseResult
                {
                    Success = true,
                    MessageType = MessageType.Activation
                };
        }

        private SmsParseResult CreateInvalidMessageFormatResponse()
        {
            return CreateErrorMessage(Strings.InvalidMessageFormat);
        }

        private SmsParseResult CreateAtLeastOneProductWrongResponse()
        {
            return CreateErrorMessage(Strings.At_least_one_product_specification_is_invalid);
        }

        private SmsParseResult CreateErrorMessage(string msg)
        {
            return new SmsParseResult {Message = msg};
        }

        private string GetStockLevelString(string message)
        {
            return message.Substring(4, message.Length - 1 - 4);
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
    }
}
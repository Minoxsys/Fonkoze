using System;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services.MessageParsingStrategies
{
    internal class ParseActivationMessageStrategy : ISmsParsingStrategy
    {
        private const string ActivationMessage = "activate";

        public SmsParseResult Parse(string message)
        {
            if (string.Compare(message.Trim(new[] {'.', ',', ';'}), ActivationMessage, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return CreateActivationMessageResponse();
            }
            return new SmsParseResult {Success = false};
        }

        private SmsParseResult CreateActivationMessageResponse()
        {
            return new SmsParseResult
                {
                    Success = true,
                    MessageType = MessageType.Activation
                };
        }
    }
}
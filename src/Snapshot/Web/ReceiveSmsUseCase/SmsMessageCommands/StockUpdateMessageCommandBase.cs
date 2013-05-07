﻿using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Web.LocalizationResources;
using Web.Models.Parsing;
using Web.ReceiveSmsUseCase.Models;
using Web.Services;
using Web.Services.SendEmail;
using Web.Services.StockUpdates;

namespace Web.ReceiveSmsUseCase.SmsMessageCommands
{
    public abstract class StockUpdateMessageCommandBase : ISmsMessageCommand
    {
        protected readonly IUpdateStockService UpdateStockService;
        private readonly ISendSmsService _sendSmsService;
        private readonly ISaveOrUpdateCommand<Alert> _saveOrUpdateAlertCommand;
        private readonly IPreconfiguredEmailService _emailSendingService;
        private readonly IQueryService<RawSmsReceived> _rawSmsReceivedQueryService;

        protected StockUpdateMessageCommandBase(IUpdateStockService updateStockService, ISendSmsService sendSmsService,
                                         ISaveOrUpdateCommand<Alert> saveOrUpdateAlertCommand, IPreconfiguredEmailService emailSendingService,
                                         IQueryService<RawSmsReceived> rawSmsReceived)
        {
            _rawSmsReceivedQueryService = rawSmsReceived;
            _emailSendingService = emailSendingService;
            UpdateStockService = updateStockService;
            _sendSmsService = sendSmsService;
            _saveOrUpdateAlertCommand = saveOrUpdateAlertCommand;
        }

        public void Execute(ReceivedSmsInputModel smsData, ISmsParseResult parseResult, Outpost outpost)
        {
            if (!IsActiveSender(outpost, smsData.Sender))
            {
                _sendSmsService.SendSms(smsData.Sender, Strings.PhoneNumberNotActive, false);
                return;
            }

            if (parseResult.Success)
            {
                StockUpdateResult result = UpdateProductStocksForOutpost(parseResult, outpost);
                if (result != null && !result.Success)
                {
                    _sendSmsService.SendSms(smsData.Sender, ComposeUpdateStockFailMessage(result.FailedProducts), true);
                   
                    DoAfterStockUpdate(parseResult,outpost);
                }
                else
                {
                    _sendSmsService.SendSms(smsData.Sender, Strings.StockUpdateSuccessConfirmation, true);
                }
            }
            else
            {
                if (IsSecondOrMoreConsecutiveMistake(smsData))
                {
                    var msg = CreateMailMessage(smsData, outpost);
                    _emailSendingService.SendEmail(msg);

                    var phoneNumber = outpost.GetDistrictManagersPhoneNumberAsString();
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        _sendSmsService.SendSms(phoneNumber, ComposeSecondMistakeSms(smsData, outpost), true);
                    }
                }

                var alert = new Alert
                    {
                        AlertType = AlertType.Error,
                        Client = outpost.Client,
                        OutpostId = outpost.Id,
                        Contact = smsData.Sender,
                        OutpostName = outpost.Name,
                        ProductGroupName = "-",
                        LowLevelStock = "-",
                        LastUpdate = null
                    };
                _saveOrUpdateAlertCommand.Execute(alert);

                _sendSmsService.SendSms(smsData.Sender, ComposeFailedParsingMessage(parseResult.Message), true);
            }
        }

        internal abstract StockUpdateResult UpdateProductStocksForOutpost(ISmsParseResult parseResult, Outpost outpost);

        internal virtual void DoAfterStockUpdate(ISmsParseResult parseResult, Outpost outpost)
        {

        }

        private string ComposeSecondMistakeSms(ReceivedSmsInputModel smsData, Outpost outpost)
        {
            return string.Format(Strings._2ndMistakeSmsToManager, outpost.Name, smsData.Sender, smsData.Content);
        }

        private bool IsSecondOrMoreConsecutiveMistake(ReceivedSmsInputModel smsData)
        {
            var beforePreviousMessage =
                _rawSmsReceivedQueryService.Query()
                                           .OrderByDescending(m => m.Created).Where(m => m.Sender == smsData.Sender).Skip(1)
                                           .FirstOrDefault();
            return beforePreviousMessage != null && !beforePreviousMessage.ParseSucceeded;
        }

        private string ComposeFailedParsingMessage(string detailsMessage)
        {
            return string.Format(Strings.InvalidSmsReceived, detailsMessage, Environment.NewLine);
        }

        private string ComposeUpdateStockFailMessage(IEnumerable<IParsedProduct> failedProducts)
        {
            var sb = new StringBuilder(string.Format(Strings.InvalidCodesPart1, Environment.NewLine));
            foreach (var failedProduct in failedProducts)
            {
                sb.AppendLine(string.Format("({0},{1})", failedProduct.ProductGroupCode, failedProduct.ProductCode));
            }
            sb.AppendLine(Strings.InvalidCodesPart2);
            return sb.ToString();
        }

        private MailMessage CreateMailMessage(ReceivedSmsInputModel smsData, Outpost outpost)
        {
            var msg = _emailSendingService.CreatePartialMailMessageFromConfig();
            msg.Subject = Strings.IncorrectSMSAlertSubject;
            msg.Body = string.Format(Strings.TwoConsecutiveInvalidSMSEmailBody, outpost.Name,
                                     smsData.Sender);
            return msg;
        }

        private bool IsActiveSender(Outpost outpost, string senderPhoneNumber)
        {
            return
                outpost.Contacts.FirstOrDefault(
                    c => c.ContactDetail.Contains(senderPhoneNumber) && c.ContactType == Contact.MOBILE_NUMBER_CONTACT_TYPE && c.IsMainContact) != null;
        }
    }
}
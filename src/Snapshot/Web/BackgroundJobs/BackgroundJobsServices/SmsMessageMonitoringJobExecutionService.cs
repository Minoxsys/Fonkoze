using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Persistence;
using Domain;
using Web.LocalizationResources;
using Web.Services;
using Web.Services.Helper;

namespace Web.BackgroundJobs.BackgroundJobsServices
{
    public class SmsMessageMonitoringJobExecutionService : IJobExecutionService
    {
        private readonly IQueryService<RawSmsReceived> _rawSmsQueryService;
        private readonly ISendSmsService _sendSmsService;
        private readonly ISenderInformationService _senderInformationService;

        public SmsMessageMonitoringJobExecutionService(IQueryService<RawSmsReceived> rawSmsQueryService, ISendSmsService sendSmsService,
                                        ISenderInformationService senderInformationService)
        {
            _senderInformationService = senderInformationService;
            _sendSmsService = sendSmsService;
            _rawSmsQueryService = rawSmsQueryService;
        }

        public void ExecuteJob()
        {
            var latestSmsBySender =
                  _rawSmsQueryService.Query()
                         .Where(sms => sms.Created > DateTime.UtcNow.AddHours(-24))
                         .OrderByDescending(r => r.Created)
                         .GroupBy(r => r.Sender).Select(g => g);

            foreach (IGrouping<string, RawSmsReceived> smsGroup in latestSmsBySender)
            {
                Outpost senderOutpost = _senderInformationService.GetOutpostWithActiveSender(smsGroup.Key);
                if (senderOutpost == null)
                    continue;

                var latestSmsMessage = smsGroup.FirstOrDefault();
                if (latestSmsMessage == null)
                    continue;

                if (!latestSmsMessage.ParseSucceeded && latestSmsMessage.Created < DateTime.UtcNow.AddHours(-4))
                {
                    var phoneNumber = senderOutpost.GetDistrictManagersPhoneNumberAsString();
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        _sendSmsService.SendSms(phoneNumber, ComposeMessage(senderOutpost, latestSmsMessage.Created), true);
                    }
                }
            }
        }

        private string ComposeMessage(Outpost senderOutpost, DateTime? msgDateTime)
        {
            return string.Format(Strings.LastSmsInvalidNoFollowUpSmsMessage, senderOutpost.Name,
                                 senderOutpost.DetailMethod, msgDateTime);
        }
    }
}
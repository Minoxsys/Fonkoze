using Core.Persistence;
using Domain;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.LocalizationResources;
using Web.Services;
using Web.Services.Helper;
using Web.Utils;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class SmsMessagesMonitoringJob: IJob
    {
        private readonly IQueryService<RawSmsReceived> _rawSmsQueryService;
        private readonly ISendSmsService _sendSmsService;
        private readonly ISenderInformationService _senderInformationService;
        private readonly ILogger _logger;
        private const string JobName = "SmsMessagesMonitoringJob";

        public TimeSpan Interval
        {
            //2 hour 1 minute
            get { return TimeSpan.FromMinutes(15); }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(14); }
        }

        public string Name
        {
            get { return JobName; }
        }

        public SmsMessagesMonitoringJob(IQueryService<RawSmsReceived> rawSmsQueryService, ISendSmsService sendSmsService,
                                        ISenderInformationService senderInformationService, ILogger logger)
        {
            _logger = logger;
            _senderInformationService = senderInformationService;
            _sendSmsService = sendSmsService;
            _rawSmsQueryService = rawSmsQueryService;
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    try
                    {
                        var latestSmsBySender =
                            _rawSmsQueryService.Query()
                                               .Where(sms => sms.Created > DateTime.UtcNow.AddHours(-6))
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sms monitorign job has failed");
                        throw;
                    }
                });
        }

        private string ComposeMessage(Outpost senderOutpost, DateTime? msgDateTime)
        {
            return string.Format(Strings.LastSmsInvalidNoFollowUpSmsMessage, senderOutpost.Name,
                                 senderOutpost.DetailMethod, msgDateTime);
        }
    }
}
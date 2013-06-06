using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.BackgroundJobs
{
    public enum JobExecutionType 
    { 
        EmptyJobType, 
        CampaignJobType, 
        OutpostInactivityType, 
        SmsMessageMonitoringType, 
        StockLevelMonitoringType, 
        TrimLogType 
    }
}
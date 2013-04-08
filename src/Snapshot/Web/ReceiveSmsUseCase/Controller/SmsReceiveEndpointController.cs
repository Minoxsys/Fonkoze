using System.Web.Mvc;
using Web.Models;
using Web.ReceiveSmsUseCase.Models;
using Web.ReceiveSmsUseCase.Services;
using Web.Services;

namespace Web.ReceiveSmsUseCase.Controller
{
    public class SmsReceiveEndpointController : System.Web.Mvc.Controller
    {
        private readonly IReceiveSmsWorkflowService _receiveSmsWorkflowService;

        public SmsReceiveEndpointController(IReceiveSmsWorkflowService receiveSmsWorkflowService)
        {
            _receiveSmsWorkflowService = receiveSmsWorkflowService;
        }

        [HttpGet]
        public ActionResult ReceiveSms(ReceivedSmsInputModel smsData)
        {
            _receiveSmsWorkflowService.ProcessSms(smsData);
            return new EmptyResult();
        }
    }
}
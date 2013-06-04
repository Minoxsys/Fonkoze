using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Controllers;
using Web.Areas.MessagesManagement.Models.SentMessages;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.SentMessagesControllerTests
{
    public class ObjectMother
    {
        public SentMessagesController Controller;
        public IQueryService<SentSms> QuerySms;

        public const string Phonenumber = "+123456789";
        public const string Message = "Some message";
        public const string Response = "OK";

        public void Init()
        {
            MockServices();
            Setup_Controller();
        }

        private void Setup_Controller()
        {
            Controller = new SentMessagesController {QuerySms = QuerySms};
        }

        private void MockServices()
        {
            QuerySms = MockRepository.GenerateMock<IQueryService<SentSms>>();
        }

        public IQueryable<SentSms> PageOfData(IndexTableInputModel indexModel)
        {
            var smsList = new List<SentSms>();

            Debug.Assert(indexModel.start != null, "IndexTableInputModel.start != null");
            Debug.Assert(indexModel.limit != null, "IndexTableInputModel.limit != null");
            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                smsList.Add(new SentSms
                    {
                        PhoneNumber = Phonenumber,
                        Message = Message + i,
                        SentDate = DateTime.UtcNow,
                        Response = Response

                    });
            }
            return smsList.AsQueryable();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.SendMessagesControllerTests
{

    
        
    [TestFixture]
    public class SendMessageToOutpostsMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            objectMother.SetupSmsService_and_MockServices();

        }

        public void Should_Return_JSonSuccessMessage_And_SaveIn_SentSmssTable()
        {
          //  objectMother.saveOrUpdateProduct.Expect(it => it.Execute(Arg<Product>.Matches(st => st.Name.Equals(objectMother.product.Name))));    
        
        }


    }
}

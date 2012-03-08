using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Models.EmailRequest;
using Persistence.Queries.Contacts;

namespace Tests.Unit.Controllers.EmailRequestControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        private ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Setup_Controller_and_MockServices();
            objectMother.SetUp_StubData();
        }

        [Test]
        public void Get_Create_Returns_View_With_A_ListOfOutpost_And_A_ListOfProductGroups()
        {
            // Arange
            objectMother.queryServiceOutpost.Expect(call => call.Query()).Return(new Outpost[] { objectMother.outpost }.AsQueryable());
            objectMother.queryServiceProductGroup.Expect(call => call.Query()).Return(new ProductGroup[] { objectMother.productGroup }.AsQueryable());

            //Act
            var result = (ViewResult)objectMother.controller.Create();

            //Assert
            objectMother.queryServiceOutpost.VerifyAllExpectations();
            objectMother.queryServiceProductGroup.VerifyAllExpectations();

            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOf<EmailCreateModel>(result.Model);
        }

        [Test]
        public void Post_Create_OnSuccess_SavesTheEmailRequest_And_SendsEmail()
        {
            //Arange
            EmailCreateModel model = new EmailCreateModel()
            {
                Outpost = new ReferenceModel() { Id = objectMother.outpostId, Name = objectMother.outpost.Name },
                ProductGroup = new ReferenceModel() { Id = objectMother.productGroup.Id, Name = objectMother.productGroup.Name }
            };
            objectMother.queryServiceContact.Expect(call => call.Query(Arg<ContactByOutpostIdAndMainMethod>.Is.Anything)).Return(new Contact[] { objectMother.contact }.AsQueryable());

            objectMother.saveCommandEmailRequest.Expect(it => it.Execute(Arg<EmailRequest>.Matches(
               c => c.Date.ToShortDateString() == DateTime.UtcNow.ToShortDateString() &&
                    c.OutpostId == objectMother.outpost.Id &&
                    c.ProductGroupId == objectMother.productGroup.Id
            )));

            objectMother.queryServiceProductGroup.Expect(call => call.Load(objectMother.productGroupId)).Return(objectMother.productGroup);
            objectMother.urlService.Stub(call => call.GetEmailLinkUrl(Arg<string>.Is.Anything)).Return("LINKURL");

            //Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.Create(model);

            //Assert
            objectMother.queryServiceContact.VerifyAllExpectations();
            objectMother.saveCommandEmailRequest.VerifyAllExpectations();

            Assert.That(objectMother.emailService._sendMailMessageCalled, Is.True);

            Assert.AreEqual("Overview", redirectResult.RouteValues["Action"]);
            Assert.AreEqual("EmailRequest", redirectResult.RouteValues["Controller"]);
        }

        [Test]
        public void Post_Create_OnFail_RedirectTo_HomeIndex()
        {
            //Arrange
            EmailCreateModel model = new EmailCreateModel();
            objectMother.queryServiceContact.Expect(call => call.Query(Arg<ContactByOutpostIdAndMainMethod>.Is.Anything)).Return(new Contact[] { null }.AsQueryable());

            //Act
            var redirectResult = (RedirectToRouteResult)objectMother.controller.Create(model);

            //Asser
            objectMother.queryServiceContact.VerifyAllExpectations();

            Assert.That(objectMother.emailService._sendMailMessageCalled, Is.False);

            Assert.AreEqual("Index", redirectResult.RouteValues["Action"]);
            Assert.AreEqual("Home", redirectResult.RouteValues["Controller"]);
        }
    }
}

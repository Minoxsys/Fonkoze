using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Rhino.Mocks;
using System.Web.Mvc;
using Web.Models.UserManager;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class GetListOfClientsMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Clients()
        {
            //Arange
            objectMother.QueryClient.Expect(call => call.Query()).Return(new Client[] {objectMother.Client}.AsQueryable());

            //Act
            var jsonResult = objectMother.Controller.GetListOfClients();

            //Assert
            objectMother.QueryClient.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<ClientsIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as ClientsIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }
    }
}

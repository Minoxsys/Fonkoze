using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Persistence;
using Core.Domain;
using Domain;
using Rhino.Mocks;
using NUnit.Framework;
using System.Web.Mvc;
using Web.Models.Shared;
using Web.Models.UserManager;

namespace Tests.Unit.Controllers.UserMangerControllerTests
{
    [TestFixture]
    public class GetListOfRolesMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_List_Of_Roles()
        {
            //Arange
            objectMother.QueryRole.Expect(call => call.Query()).Return(new Role[] {objectMother.Role}.AsQueryable());

            //Act
            var jsonResult = objectMother.Controller.GetListOfRoles();

            //Assert
            objectMother.QueryRole.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            Assert.IsInstanceOf<StoreOutputModel<ReferenceModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ReferenceModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(1, jsonData.TotalItems);
        }

    }
}

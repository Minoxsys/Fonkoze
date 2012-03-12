using System;
using System.Linq;
using System.Web.Mvc;
using Domain;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Country;
using Rhino.Mocks;
using Persistence.Queries.Functions;
using Core.Domain;
using Persistence.Queries.Employees;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.CountryControllerTests
{
    [TestFixture]
    public class OverviewMethod
    {
        readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void GetsTheCurrentUserAndItsClient()
        {
            objectMother.queryUsers.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());

            objectMother.QueryCountriesToReturnsEmptyResult();
            objectMother.QueryWorldCountryRecordsReturnsEmptyResult();

           objectMother.controller.Overview();



        }

        [Test]
        public void Get_ReturnsTheViewModel_WithTheWorldCountriesLoaded()
        {
            objectMother.queryUsers.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());
            objectMother.QueryCountriesToReturnsEmptyResult();

            objectMother.queryWorldCountryRecords.Expect(call => call.Query()).Return( objectMother.WorldCountryRecords());
           // // Act
            var viewResult = (ViewResult)objectMother.controller.Overview();

           //// Assert
            objectMother.queryWorldCountryRecords.VerifyAllExpectations();

            Assert.IsNotNull(viewResult.Model);

            Assert.AreEqual(ObjectMother.DEFAULT_VIEW_NAME, viewResult.ViewName);
        
        }

        [Test]
        public void Get_Returns_OnlyTheCountries_ThatDoNetBelong_ToTheUser_Currently()
        {
            var currentUserCountries = objectMother.CurrentUserCountries();
            var worldCountryRecord = objectMother.WorldCountryRecords();

            objectMother.queryUsers.Expect(bt => bt.Query(Arg<UserByUserName>.Is.Anything)).Return(new User[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());
            objectMother.queryPermission.Expect(it => it.Query(Arg<FunctionByName>.Is.Anything)).Return(new Permission[] { }.AsQueryable());

            objectMother.queryWorldCountryRecords.Expect(call => call.Query()).Return(worldCountryRecord);
            objectMother.queryCountry.Expect(call => call.Query()).Return(currentUserCountries);

            var viewResult = (ViewResult)objectMother.controller.Overview();

            objectMother.queryCountry.VerifyAllExpectations();

            var model = viewResult.Model as CountryOverviewModel;

            Assert.IsNotNull(model);

            Assert.That(model.WorldRecords, Is.EqualTo("[]"));
        }
    }
}

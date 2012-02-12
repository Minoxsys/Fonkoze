using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Fact = NUnit.Framework.TestAttribute;
using BeforeEach = NUnit.Framework.SetUpAttribute;
using Web.Models.Shared;
using Domain;
using Web.Areas.OutpostManagement.Models.Country;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.CountryControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();
        private string PHONE_PREFIX = "0040";
        private string NAME = "Romania";
        private string ISO_CODE = "RO";

        [BeforeEach]
        public void BeforeEach()
        {
            objectMother.Init();
        }

        [Fact]
        public void Creates_Country_From_InputModel()
        {
            var inputModel = new CountryInputModel()
            {
                ISOCode = ISO_CODE,
                Name = NAME,
                PhonePrefix = PHONE_PREFIX
            };

            objectMother.saveCommand.Expect(call => call.Execute(Arg<Country>.Matches(c => c.ISOCode == ISO_CODE &&
                                                                                           c.Name == NAME && c.PhonePrefix == PHONE_PREFIX)));

            objectMother.controller.Create(inputModel);


            objectMother.saveCommand.VerifyAllExpectations();
        }
    }
}
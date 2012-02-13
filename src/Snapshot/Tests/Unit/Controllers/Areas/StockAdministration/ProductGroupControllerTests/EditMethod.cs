using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.StockAdministration.Models.ProductGroup;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ProductGroup_Has_No_Id()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new ProductGroupInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a prouctGroupId in order to edit the region."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Region_Has_Been_Saved()
        {
            //Arrange
            ProductGroupInputModel productGroupInputModel = new ProductGroupInputModel()
            {
                Id = objectMother.productGroup.Id,
                Name = objectMother.productGroup.Name,
                Description = objectMother.productGroup.Description,
                ReferenceCode = objectMother.productGroup.ReferenceCode
            };
            objectMother.saveCommand.Expect(call => call.Execute(Arg<ProductGroup>.Matches(p => p.Name == objectMother.productGroup.Name && p.Description == objectMother.productGroup.Description)));

            //Act
            var jsonResult = objectMother.controller.Edit(productGroupInputModel);

            //Assert
            objectMother.queryProductGroup.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Product Group Malaria has been saved."));
        }
    }
}

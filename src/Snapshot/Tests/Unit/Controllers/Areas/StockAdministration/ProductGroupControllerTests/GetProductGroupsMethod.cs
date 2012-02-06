using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.StockAdministration.Models.ProductGroup;
using System.Web.Mvc;
using Domain;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
{
    [TestFixture]
    public class GetProductGroupsMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            //Arrange
            var indexModel = new ProductGroupIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };
            var pageOfData = objectMother.PageOfProductGroupData(indexModel);
            objectMother.queryProductGroup.Expect(call => call.Query()).Return(pageOfData);
            objectMother.queryProduct.Expect(call => call.Query()).Return(new Product[] {objectMother.product}.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetProductGroups(indexModel);

            //Assert
            objectMother.queryProductGroup.VerifyAllExpectations();
            objectMother.queryProduct.VerifyAllExpectations();

            Assert.That(jsonResult, Is.InstanceOfType<JsonResult>());
            Assert.That(jsonResult.Data, Is.InstanceOfType<ProductGroupIndexOutputModel>());
            var jsonData = jsonResult.Data as ProductGroupIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_ProductGroups_Order_ByName_DESC()
        {
            //Arrange
            var indexModel = new ProductGroupIndexModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };
            var pageOfData = objectMother.PageOfProductGroupData(indexModel);
            objectMother.queryProductGroup.Expect(call => call.Query()).Return(pageOfData);
            objectMother.queryProduct.Expect(call => call.Query()).Return(new Product[] { objectMother.product }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetProductGroups(indexModel);

            //Assert
            objectMother.queryProductGroup.VerifyAllExpectations();
            objectMother.queryProduct.VerifyAllExpectations();

            var jsonData = jsonResult.Data as ProductGroupIndexOutputModel;

            Assert.That(jsonData.ProductGroups[0].Name, Is.EqualTo("Malaria9"));
            Assert.That(jsonData.ProductGroups[0].Description, Is.EqualTo("9 Descriere pentru malaria"));

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.StockAdministration.Models.ProductGroup;
using System.Web.Mvc;
using Domain;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
{
    [TestFixture]
    public class GetProductGroupsMethod
    {
        public ObjectMother objectMother = new ObjectMother();
        private IQueryable<ProductGroup> pageOfData;

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();

        }

        [Test]
        public void Loads_The_UserAndClient()
        {
            var indexModel = new IndexTableInputModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };

            MockPageOfData(indexModel);
            var jsonResult = objectMother.controller.GetProductGroups(indexModel);
            objectMother.VerifyUserAndClientQueries();
        }

        [Test]
        public void Returns_The_Data_Paginated_BasedOnTheInputValues()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };

            MockPageOfData(indexModel);

            //Act
            var jsonResult = objectMother.controller.GetProductGroups(indexModel);

            //Assert
            objectMother.queryProductGroup.VerifyAllExpectations();
            objectMother.queryProduct.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ProductGroupModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ProductGroupModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_ProductGroups_Order_ByName_DESC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name"
            };
            MockPageOfData(indexModel);

            //Act
            var jsonResult = objectMother.controller.GetProductGroups(indexModel);

            //Assert
            objectMother.queryProductGroup.VerifyAllExpectations();
            objectMother.queryProduct.VerifyAllExpectations();

            var jsonData = jsonResult.Data as StoreOutputModel<ProductGroupModel>;

            Assert.That(jsonData.Items[0].Name, Is.EqualTo("Malaria9"));
            Assert.That(jsonData.Items[0].Description, Is.EqualTo("9 Descriere pentru malaria"));

        }

        private void MockPageOfData(IndexTableInputModel indexModel)
        {
            pageOfData = objectMother.PageOfProductGroupData(indexModel);
            objectMother.queryProductGroup.Expect(call => call.Query()).Return(pageOfData);
            objectMother.queryProduct.Expect(call => call.Query()).Return(new Product[] { objectMother.product }.AsQueryable());
        }
    }
}

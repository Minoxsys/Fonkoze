using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{
    [TestFixture]
    public class GetProductsMethod
    {
        ObjectMother _ = new ObjectMother();
        [SetUp]
        public void Init()
        {
            _.Init();

        }

        [Test]
        public void LoadsUserAndClient()
        {
            _.controller.GetProducts(_.ProductsInput());

            _.VerifyUserAndClientExpectations();
        }

        [Test]
        public void QueriesForProductsWithTheGivenProductId()
        {
            _.controller.GetProducts(_.ProductsInput());

            _.VerifyProductsQueried();
        }

        [Test]
        public void ReturnsAListOfProducts_BasedOnThe_productGroupId()
        {
            var result = _.controller.GetProducts(_.ProductsInput());

            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<ProductModel[]>(result.Data);
        }
    }
}

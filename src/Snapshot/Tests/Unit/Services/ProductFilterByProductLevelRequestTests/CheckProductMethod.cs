using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;

namespace Tests.Unit.Services.ProductFilterByProductLevelRequestTests
{
    [TestFixture]
    public class CheckProductMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            _.Setup_ProductLevelRequest_And_InputData();
        }

        [Test]
        public void Receives_A_Product_That_Is_Selected_And_Returns_True()
        {
            // Arrange

            // Act
            var result = _.filter.CheckProduct(_.selectedProduct);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Receives_A_Product_That_Is_Not_Selected_And_Returns_False()
        {
            // Arrange

            // Act
            var result = _.filter.CheckProduct(_.unselectedProduct);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Receives_A_Product_That_Does_Not_Exist_In_The_Products_List_For_The_ProductLevelRequest()
        {
            // Arrange

            // Act
            var result = _.filter.CheckProduct(_.secondProduct);

            // Assert
            Assert.IsFalse(result);
        }
    }
}

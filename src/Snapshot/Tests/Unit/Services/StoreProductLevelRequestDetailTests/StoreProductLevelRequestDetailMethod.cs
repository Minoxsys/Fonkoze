using System;
using System.Linq;
using NUnit.Framework;

namespace Tests.Unit.Services.StoreProductLevelRequestDetailTests
{
    [TestFixture]
    public class StoreProductLevelRequestDetailMethod
    {

        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        [Test]
        public void SavesTheRequestDetailToDatabase()
        {
            var input = _.ValidMessageInput();
            _.service.StoreProductLevelRequestDetail(
               input);

            _.ExpectProductLevelRequestDetailsToBeSaved(input);
            
        }

        [Test]
        public void RemovesProductLevelRequestDetails_IfOldValuesExist()
        {

            var input = _.ValidMessageInput();
            _.SetUpOldValue(input);

            _.service.StoreProductLevelRequestDetail(
               input);

            _.ExpectOld_ProductLevelRequestDetailsToBeRemoved(input);
            
        }

        [Test]
        public void SavesRequestDetailsWith_NoContactDefined_WhenContact_IsNull()
        {

            var input = _.ValidMessageInputWithNullContact();

            _.service.StoreProductLevelRequestDetail(
               input);

            _.ExpectProductLevelRequestDetailsToBeSaved(input);

        }

        [Test]
        public void Stores_SavesRequestDetailsWith_EmptyProductsArray()
        {

            var input = _.ValidMessageInputWithEmptyProducts();

            _.service.StoreProductLevelRequestDetail(
               input);

            _.ExpectProductLevelRequestDetailsToBeSaved(input);

        }

        
    }
}

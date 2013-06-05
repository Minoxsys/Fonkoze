using Core.Domain;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using Web.Models.ClientManager;
using Web.Models.Shared;

namespace Tests.Unit.Controllers.ClientManagerControllerTests
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
            var pageOfData = objectMother.PageOfClientsData(indexModel);
            objectMother.queryClient.Expect(call => call.Query()).Return(pageOfData);
            objectMother.queryUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetListOfClients(indexModel);

            //Assert
            objectMother.queryClient.VerifyAllExpectations();
            objectMother.queryUsers.VerifyAllExpectations();

            Assert.IsInstanceOf<StoreOutputModel<ClientManagerOutputModel>>(jsonResult.Data);
            var jsonData = jsonResult.Data as StoreOutputModel<ClientManagerOutputModel>;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(pageOfData.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_Clients_With_ShearchValue_And_Order_ByName_DESC()
        {
            //Arrange
            var indexModel = new IndexTableInputModel
            {
                dir = "DESC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "Name",
                searchValue = "E"
            };

            var pageOfData = objectMother.PageOfClientsData(indexModel);
            objectMother.queryClient.Expect(call => call.Query()).Return(pageOfData);
            objectMother.queryUsers.Expect(call => call.Query()).Return(new User[] { objectMother.user }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.GetListOfClients(indexModel);

            //Assert
            objectMother.queryClient.VerifyAllExpectations();
            objectMother.queryUsers.VerifyAllExpectations();

            var jsonData = jsonResult.Data as StoreOutputModel<ClientManagerOutputModel>;

            Assert.That(jsonData.Items[0].Name, Is.EqualTo("9Edgard"));
        }
    }
}

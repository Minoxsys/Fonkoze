using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;
using Web.Services;

namespace Tests.Unit.Services.ProductLevelRequestMessagesDispatcherTests
{
    [TestFixture]
    public class DispatchMessagesForProductLevelRequestMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            _.Setup_Dispatcher_Service_And_QueryServices();
            _.Setup_Stub_Data();
        }

        [Test]
        public void Given_A_ProductLevelRequest_With_A_Selected_Product_Found_In_DB_Will_Call_The_Provided_Sender_Service()
        {
            // Arrange
            _.queryServiceContact.Expect(call => call.Query()).Return(new Contact[] { _.contact }.AsQueryable());
            _.queryServiceStockLevel.Expect(call => call.Query()).Return(new OutpostStockLevel[] { _.outpostStockLevel1, _.outpostStockLevel2 }.AsQueryable());
            _.queryServiceOutpost.Expect(call => call.Load(_.outpostId)).Return(_.outpost);
            _.senderService.Expect(call => call.SendProductLevelRequestMessage(Arg<ProductLevelRequestMessageInput>.Matches(m =>
                m.Contact.Equals(_.contact) &&
                m.Outpost.Equals(_.outpost) &&
                m.ProductGroup.Equals(_.productGroup) &&
                m.Client.Equals(_.client) && 
                m.ByUser.Equals(_.byUser) &&
                m.Products.Count == 1 &&
                m.Products[0].Equals(_.outpostStockLevel1.Product)
            ))).Return(true);
            _.saveOrUpdateRequestRecord.Expect(call => call.Execute(Arg<RequestRecord>.Matches(r => 
                r.CampaignId.Equals(_.campaign.Id) &&
                r.CampaignName.Equals(_.campaign.Name) &&
                r.Client.Equals(_.client) &&
                r.OutpostId.Equals(_.outpost.Id) &&
                r.OutpostName.Equals(_.outpost.Name) &&
                r.ProductGroupId.Equals(_.productGroup.Id) &&
                r.ProductGroupName.Equals(_.productGroup.Name) &&
                r.ProductsNo.Equals(1)
            )));

            // Act
            _.dispatcherService.DispatchMessagesForProductLevelRequest(_.productLevelRequestWithOneProduct);

            // Assert
            _.queryServiceContact.VerifyAllExpectations();
            _.queryServiceStockLevel.VerifyAllExpectations();
            _.senderService.VerifyAllExpectations();
            _.saveOrUpdateRequestRecord.VerifyAllExpectations();
        }
    }
}

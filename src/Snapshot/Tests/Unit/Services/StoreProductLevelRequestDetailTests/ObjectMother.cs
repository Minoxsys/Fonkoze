using System;
using System.Linq;
using Web.Services;
using AutofacContrib.Moq;
using Moq;
using Domain;
using System.Collections.Generic;
using Core.Domain;
using Core.Persistence;

namespace Tests.Unit.Services.StoreProductLevelRequestDetailTests
{
    public class ObjectMother
    {
        AutoMock autoMock;
        public StoreProductLevelRequestDetailService service;

        private User _user;
        private Client _client;
        private ProductGroup _productGroup;
        private Outpost _outpost;
        private ProductLevelRequestMessageInput _input;

        private IDeleteCommand<ProductLevelRequestDetail> deleteCommand;
        private ISaveOrUpdateCommand<ProductLevelRequestDetail> saveOrUpdateCommand;
        private IQueryService<ProductLevelRequestDetail> queryService;

        public ObjectMother()
        {
            autoMock = AutoMock.GetLoose();
        }

        public void Init()
        {
            deleteCommand = autoMock.Mock<IDeleteCommand<ProductLevelRequestDetail>>().Object;
            saveOrUpdateCommand = autoMock.Mock<ISaveOrUpdateCommand<ProductLevelRequestDetail>>().Object;
            queryService = autoMock.Mock<IQueryService<ProductLevelRequestDetail>>().Object;

            autoMock.Provide(deleteCommand);
            autoMock.Provide(saveOrUpdateCommand);
            autoMock.Provide(queryService);

            service = autoMock.Create<StoreProductLevelRequestDetailService>();
        }

        internal ProductLevelRequestMessageInput ValidMessageInput()
        {
            _input = new ProductLevelRequestMessageInput();

            _input.ByUser = FakeUser();
            _input.Client = FakeClient();
            _input.Contact = FakeContact();
            _input.Outpost = FakeOutpost();
            _input.ProductGroup = FakeProductGroup();
            _input.Products = FakeProducts(3);
            _input.ProductLevelRequest = FakeProductLevelRequest();

            return _input;
        }

        internal ProductLevelRequestMessageInput ValidMessageInputWithNullContact()
        {
            _input = new ProductLevelRequestMessageInput();

            _input.ByUser = FakeUser();
            _input.Client = FakeClient();

            _input.Contact = null;

            _input.Outpost = FakeOutpost();
            _input.ProductGroup = FakeProductGroup();
            _input.Products = FakeProducts(3);
            _input.ProductLevelRequest = FakeProductLevelRequest();

            return _input;
        }

        internal ProductLevelRequestMessageInput ValidMessageInputWithEmptyProducts()
        {
            _input = new ProductLevelRequestMessageInput();

            _input.ByUser = FakeUser();
            _input.Client = FakeClient();

            _input.Contact = null;

            _input.Outpost = FakeOutpost();
            _input.ProductGroup = FakeProductGroup();
            _input.Products = FakeProducts(0);
            _input.ProductLevelRequest = FakeProductLevelRequest();

            return _input;
        }

        private ProductLevelRequest FakeProductLevelRequest()
        {
            var productLevelRequest = new Mock<ProductLevelRequest>();
            productLevelRequest.SetupGet(p => p.Id).Returns(Guid.NewGuid());

            return productLevelRequest.Object;
        }

        private Client FakeClient()
        {
            var clientMock = new Mock<Client>();
            clientMock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            clientMock.SetupGet(p => p.Name).Returns("minoxsys");

            return clientMock.Object;
        }

        private System.Collections.Generic.List<Domain.Product> FakeProducts(int productsNo)
        {
            var list = new List<Product>();

            for (int i = 0; i < productsNo; i++)
            {
                list.Add(FakeProduct(i));
            }

            return list;
        }

        private Product FakeProduct(int productNo)
        {
            var productMock = new Mock<Product>();
            productMock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            productMock.SetupGet(p => p.Name).Returns("Product " + productNo);
            productMock.SetupGet(p => p.SMSReferenceCode).Returns(Char.ConvertFromUtf32(productNo + 94));

            return productMock.Object;
        }

        private Domain.ProductGroup FakeProductGroup()
        {
            var productGroup = new Mock<ProductGroup>();
            productGroup.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            productGroup.SetupGet(p => p.Name).Returns("Product Group");
            productGroup.SetupGet(p => p.Description).Returns("Product Group");
            productGroup.SetupGet(p => p.ReferenceCode).Returns("PRG");

            return productGroup.Object;
        }

        private Domain.Outpost FakeOutpost()
        {
            var outpost = new Mock<Outpost>();

            outpost.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            outpost.SetupGet(p => p.Name).Returns("Outpost name");

            outpost.SetupGet(p => p.Country).Returns(FakeCountry());
            outpost.SetupGet(p => p.Region).Returns(FakeRegion());
            outpost.SetupGet(p => p.District).Returns(FakeDistrict());

            return outpost.Object;
        }

        private Country FakeCountry()
        {
            var country = new Mock<Country>();

            country.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            country.SetupGet(p => p.Name).Returns("Country name");

            return country.Object;
        }

        private Region FakeRegion()
        {
            var region = new Mock<Region>();

            region.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            region.SetupGet(p => p.Name).Returns("Region name");

            return region.Object;
        }

        private District FakeDistrict()
        {
            var district = new Mock<District>();

            district.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            district.SetupGet(p => p.Name).Returns("District name");

            return district.Object;
        }

        private Domain.Contact FakeContact()
        {
            var contact = new Mock<Contact>();

            contact.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            contact.SetupGet(p => p.ContactType).Returns(Contact.EMAIL_CONTACT_TYPE);
            contact.SetupGet(p => p.ContactDetail).Returns("someone@example.com");

            return contact.Object;
        }

        private Core.Domain.User FakeUser()
        {
            var contact = new Mock<User>();

            contact.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            contact.SetupGet(p => p.UserName).Returns("fakeusername");
            contact.SetupGet(p => p.Email).Returns("someone@example.com");

            return contact.Object;
        }

        internal void ExpectProductLevelRequestDetailsToBeSaved(ProductLevelRequestMessageInput input)
        {
            var saveOrUpdate = Mock.Get(saveOrUpdateCommand);

            saveOrUpdate.Verify(call => call.Execute(It.Is<ProductLevelRequestDetail>(match =>
                                                                                              match.OutpostName == GetExpectedOutpostName(input) &&
                                                                                              match.ProductGroupName == input.ProductGroup.Name &&
                                                                                              match.ProductLevelRequestId == input.ProductLevelRequest.Id &&
                                                                                              match.Method == GetContactMethod(input) &&
                                                                                              match.RequestMessage == GetReqMessage(input))));
        }

        internal void ExpectProductLevelRequestDetailsTo_NOT_BeSaved(ProductLevelRequestMessageInput input)
        {
            var saveOrUpdate = Mock.Get(saveOrUpdateCommand);

            saveOrUpdate.Verify(call => call.Execute(It.IsAny<ProductLevelRequestDetail>()),Times.Never());
        }

        private string GetContactMethod(ProductLevelRequestMessageInput input)
        {
            var contactNethod = input.Contact == null ? "No main contact method" : string.Format("{0} ({1})", input.Contact.ContactType, input.Contact.ContactDetail);

            if (input.Products == null || input.Products.Count == 0)
            {
                return "None";

            }

            return contactNethod;
        }

        private string GetReqMessage(ProductLevelRequestMessageInput input)
        {
            if (input.Contact == null)
            {
                return "-";
            }

            switch (input.Contact.ContactType)
            {
                case Contact.EMAIL_CONTACT_TYPE:
                    return new FormattingStrategy().FormatEmail(input.ProductGroup.Name, "[link will be generated]");
                case Contact.MOBILE_NUMBER_CONTACT_TYPE:
                    return new FormattingStrategy().FormatSms(input);
            }
            return string.Empty;
        }

        internal void SetUpOldValue(ProductLevelRequestMessageInput input)
        {
            var queryMock = Mock.Get(queryService);
            queryMock.Setup(call => call.Query()).Returns(new ProductLevelRequestDetail[]
            {
                new ProductLevelRequestDetail
                {
                    ProductGroupName = input.ProductGroup.Name,
                    OutpostName = GetExpectedOutpostName(input),
                    ProductLevelRequestId = input.ProductLevelRequest.Id
                }
            }.AsQueryable());
        }

        internal void ExpectOld_ProductLevelRequestDetailsToBeRemoved(ProductLevelRequestMessageInput input)
        {
            var deleteCommandMock = Mock.Get(deleteCommand);
            deleteCommandMock.Verify(call => call.Execute(
                It.Is<ProductLevelRequestDetail>(match =>
                                                         match.ProductLevelRequestId == input.ProductLevelRequest.Id &&
                                                         match.ProductGroupName == input.ProductGroup.Name)));
        }

        private static string GetExpectedOutpostName(ProductLevelRequestMessageInput input)
        {
            return string.Format("{0} ( {1}/{2}/{3} )", input.Outpost.Name,
                input.Outpost.Country.Name,
                input.Outpost.Region.Name,
                input.Outpost.District.Name);
        }
    }
}
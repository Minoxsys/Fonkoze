using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.StockAdministration.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Areas.StockAdministration.Models.ProductGroup;
using Core.Domain;
using MvcContrib.TestHelper.Fakes;
using Core.Security;
using Persistence.Security;

namespace Tests.Unit.Controllers.Areas.StockAdministration.ProductGroupControllerTests
{
    public class ObjectMother
    {

        public ProductGroupController controller;

        public IQueryService<Product> queryProduct;
        public IQueryService<ProductGroup> queryProductGroup;
        public ISaveOrUpdateCommand<ProductGroup> saveCommand;
        public IDeleteCommand<ProductGroup> deleteCommand;

        public IQueryService<User> queryUsers;

        public IQueryService<Client> queryClients;
        public IQueryService<Permission> queryPermission;

        public IPermissionsService PermissionService;

        public ProductGroup productGroup;
        public Product product;

        public Guid productGroupId;
        public Guid productId;

        private Client client;
        private User user;
        private const string FAKE_USERNAME = "fake-user-name";

        private const string PRODUCTGROUP_NAME = "Malaria";
        private const string PRODUCTGROUP_DESCRIPTION = "Descriere pentru malaria";
        private const string PRODUCTGROUP_REFERENCECODE = "MAL";
        private const string PRODUCT_NAME = "Produs1";
        
        public void Init()
        {
            MockServices();
            Setup_Controller();
            StubUserAndClient();
            SetUp_StubData();
        }

        private void StubUserAndClient()
        {
            client = MockRepository.GeneratePartialMock<Client>();
            user = MockRepository.GeneratePartialMock<User>();

            client.Stub(p=>p.Id ).Return( Guid.NewGuid() );
            client.Name = "Minoxsys";

            user.Stub(p=>p.Id ).Return( Guid.NewGuid() );
            user.UserName = FAKE_USERNAME;
            user.ClientId = client.Id;

            queryClients.Expect(call => call.Load(client.Id)).Return(client);
            queryUsers.Expect(call => call.Query()).Return(new User[] { user }.AsQueryable());


        }

        private void MockServices()
        {
            queryProduct = MockRepository.GenerateMock<IQueryService<Product>>();
            queryProductGroup = MockRepository.GenerateMock<IQueryService<ProductGroup>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<ProductGroup>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<ProductGroup>>();

            queryClients = MockRepository.GenerateMock<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryPermission = MockRepository.GenerateMock<IQueryService<Permission>>();
        }

        private void Setup_Controller()
        {
            controller = new ProductGroupController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);
            PermissionService = new FunctionRightsService(queryPermission, queryUsers);

            controller.QueryService = queryProductGroup;
            controller.QueryProduct = queryProduct;
            controller.SaveOrUpdateProductGroup = saveCommand;
            controller.DeleteCommand = deleteCommand;
            controller.PermissionService = PermissionService;
            controller.QueryClients = queryClients;
            controller.QueryUsers = queryUsers;
        }

        private void SetUp_StubData()
        {
            productGroupId = Guid.NewGuid();
            productGroup = MockRepository.GeneratePartialMock<ProductGroup>();
            productGroup.Stub(c => c.Id).Return(productGroupId);
            productGroup.Name = PRODUCTGROUP_NAME;
            productGroup.Description = PRODUCTGROUP_DESCRIPTION;
            productGroup.ReferenceCode = PRODUCTGROUP_REFERENCECODE;
            productGroup.Client = client;

            productId = Guid.NewGuid();
            product = MockRepository.GeneratePartialMock<Product>();
            product.Stub(c => c.Id).Return(productId);
            product.Name = PRODUCT_NAME;
            product.ProductGroup = productGroup;
            productGroup.Client = client;
        }

        public IQueryable<ProductGroup> PageOfProductGroupData(ProductGroupIndexModel indexModel)
        {
            List<ProductGroup> productGroupPageList = new List<ProductGroup>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                productGroupPageList.Add(new ProductGroup
                {
                    Client = client,
                    ByUser = user,
                    Name = "Malaria" + i,
                    Description = i + " " + PRODUCTGROUP_DESCRIPTION
                });
            }
            return productGroupPageList.AsQueryable();
        }

        internal void VerifyUserAndClientQueries()
        {
            controller.QueryUsers.VerifyAllExpectations();
            controller.QueryClients.VerifyAllExpectations();
        }

    }
}

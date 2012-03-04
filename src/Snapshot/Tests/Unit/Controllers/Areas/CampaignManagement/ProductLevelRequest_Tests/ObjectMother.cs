using System;
using System.Linq;
using Core.Services;
using Web.Areas.CampaignManagement.Controllers;
using AutofacContrib.Moq;
using Moq;
using Core.Domain;
using Domain;
using Autofac;
using MvcContrib.TestHelper.Fakes;
using System.Collections.Generic;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{
    public class ObjectMother
    {
        const string FAKE_USERNAME = "fake.username";

        internal ProductLevelRequestController controller;

        internal AutoMock autoMock;
        private Guid clientId = Guid.NewGuid();
        private Mock<User> userMock;
        public Mock<Client> clientMock;

        internal void Init()
        {
            autoMock = AutoMock.GetLoose();

            InitializeController();
            StubUserAndItsClient();
        }

        internal void StubUserAndItsClient()
        {
            var loadClient = Mock.Get(this.controller.LoadClient);
            var queryUser = Mock.Get(this.controller.QueryUsers);

            this.clientMock = new Mock<Client>();
            clientMock.Setup(c => c.Id).Returns(this.clientId);
            clientMock.Setup(c => c.Name).Returns("minoxsys");

            this.userMock = new Mock<User>();
            userMock.Setup(c => c.Id).Returns(Guid.NewGuid());
            userMock.Setup(c => c.ClientId).Returns(clientMock.Object.Id);
            userMock.Setup(c => c.UserName).Returns(FAKE_USERNAME);
            userMock.Setup(c => c.Password).Returns("asdf");

            loadClient.Setup(c => c.Load(this.clientId)).Returns(clientMock.Object);
            queryUser.Setup(c => c.Query()).Returns(new[] { userMock.Object }.AsQueryable());

            controller.LoadClient = loadClient.Object;
            controller.QueryUsers = queryUser.Object;
        }

        private void InitializeController()
        {
            controller = new ProductLevelRequestController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(FAKE_USERNAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            autoMock.Container.InjectUnsetProperties(controller);
        }

        internal void VerifyUserAndClientExpectations()
        {
            var queryUser = Mock.Get(this.controller.QueryUsers);
            var loadClient = Mock.Get(this.controller.LoadClient);

            queryUser.Verify(call => call.Query());
            loadClient.Verify(call => call.Load(this.clientId));
        }




        internal void VerifySchedulesQueried()
        {
            var querySchedules = Mock.Get(this.controller.QuerySchedules);

            querySchedules.Verify(call => call.Query());
        }

        internal void StubSchedulesData()
        {
            
            var querySchedules = Mock.Get(this.controller.QuerySchedules);

            querySchedules.Setup(c => c.Query()).Returns(ListOfSchedules());
        }

        private IQueryable<Schedule> ListOfSchedules()
        {
            var schedules = new List<Schedule>();

            for (int i = 0; i < 10; i++)
            {
                schedules.Add(ScheduleMock(i));
            }

            return schedules.AsQueryable();
        }

        private Schedule ScheduleMock(int i)
        {
            var schedule = new Mock<Schedule>();

            schedule.SetupGet(x => x.Id).Returns(Guid.NewGuid());
            schedule.SetupGet(x => x.Name).Returns("Schedule " + i);
            schedule.SetupGet(x => x.ScheduleBasis).Returns("On a schedule");
            schedule.SetupGet(x => x.FrequencyType).Returns("Daily");
            schedule.SetupGet(x => x.FrequencyValue).Returns(2);

            schedule.Setup(x => x.Reminders).Returns(Reminders(schedule.Object));

            schedule.SetupGet(x => x.Client).Returns(clientMock.Object);
            return schedule.Object;
        }

        private  IList<RequestReminder> Reminders(Schedule schedule)
        {
            var reminder = new Mock<RequestReminder>();

            reminder.SetupGet(x => x.Id).Returns(Guid.NewGuid());
            reminder.SetupGet(x => x.PeriodType).Returns("Day");
            reminder.SetupGet(x => x.PeriodValue).Returns(5);
            reminder.SetupGet(x => x.Schedule).Returns(schedule);
            reminder.SetupGet(x => x.ByUser).Returns(userMock.Object);

            return new List<RequestReminder>{ reminder.Object};
        }

        private Guid productGroupId = Guid.NewGuid();
        private Guid campaignId;
        private Guid scheduleId;

        internal GetProductsInput ProductsInput()
        {

            var productsService = Mock.Get(controller.QueryProducts);
            productsService.Setup(c => c.Query()).Returns(ListOfProducts());
            return new GetProductsInput
            {
                ProductGroupId = productGroupId
            };
        }

        private  IQueryable<Product> ListOfProducts()
        {
            var products = new List<Product>();
            var productGroup = new Mock<ProductGroup>();
            productGroup.SetupGet(p => p.Id).Returns(productGroupId);
            productGroup.SetupGet(p => p.Name).Returns("Product Group");

            products.Add(MakeProduct(0, productGroup.Object));

            return products.AsQueryable();
        }

        private Product MakeProduct(int idx, ProductGroup productGroup)
        {
            var product = new Mock<Product>();
            product.SetupGet(c => c.Id).Returns(Guid.NewGuid());
            product.SetupGet(c => c.Name).Returns("Product_" + idx);
            product.SetupGet(c => c.SMSReferenceCode).Returns(Char.ConvertFromUtf32(idx));
            product.SetupGet(c => c.Client).Returns(clientMock.Object);
            product.SetupGet(c => c.ByUser).Returns(userMock.Object);
            product.SetupGet(c => c.ProductGroup).Returns(productGroup);

            return product.Object;
        }

        internal void VerifyProductsQueried()
        {
            var productsService = Mock.Get(controller.QueryProducts);
            productsService.Verify(call => call.Query());
        }

        internal void VerifyCampaignsQueried()
        {
            var campaignsService = Mock.Get(controller.QueryCampaigns);
            campaignsService.Verify(call => call.Query());
        }

        internal CreateProductLevelRequestInput CreateInput()
        {
            productGroupId = Guid.NewGuid();
            campaignId = Guid.NewGuid();
            scheduleId = Guid.NewGuid();

            return new CreateProductLevelRequestInput
            {
                ProductGroupId = productGroupId,
                CampaignId = campaignId,
                ScheduleId = scheduleId,
                Products = new ProductModel[] {
                    new ProductModel{
                        Id = Guid.NewGuid(),
                        ProductItem = "Orange",
                        Selected= false,
                        SmsCode = "O"
                    } 
                }
            };
        }

        internal void VerifyCreateExpectations()
        {
            
            var campaignsService = Mock.Get(controller.QueryCampaigns);
            var productGroupsService = Mock.Get(controller.LoadProductGroup);
            var schedulesService = Mock.Get(controller.QuerySchedules);
            var saveProductLevelRequest = Mock.Get(controller.SaveProductLevelRequest);

            campaignsService.Verify(c => c.Load(campaignId));
            productGroupsService.Verify(c => c.Load(productGroupId));
            schedulesService.Verify(c => c.Load(scheduleId));

            saveProductLevelRequest.Verify(c => c.Execute(It.IsAny<ProductLevelRequest>()));
        }

        internal GetProductLevelRequestInput GetProductLevelRequestsInput()
        {
            var model = new GetProductLevelRequestInput
            {
                dir="ASC",
                limit = 10,
                start = 0,
                page =1 ,
                sort="Campaign"
            };

            FakeQueryProductLevelRequest();

            return model;
        }

        private void FakeQueryProductLevelRequest()
        {
            var productLevelRequestService = Mock.Get(controller.QueryProductLevelRequests);

            productLevelRequestService.Setup(c => c.Query()).Returns(ListOfProductLevelRequests());

        }

        private  IQueryable<ProductLevelRequest> ListOfProductLevelRequests()
        {
            var list = new List<ProductLevelRequest>();

            for (int i = 0; i < 200; i++)
            {

                var plr = new Mock<ProductLevelRequest>();

                plr.SetupGet(c => c.Id).Returns(Guid.NewGuid());
                plr.SetupGet(c => c.Campaign).Returns(MockCampaign(i));
                plr.SetupGet(c => c.ProductGroup).Returns(MockProductGroup(i));
                plr.Setup(c => c.RestoreProducts<ProductModel[]>()).Returns(MockProducts(i));
                plr.SetupGet(c => c.Schedule).Returns(MockSchedule(i));


                plr.SetupGet(c => c.Client).Returns(clientMock.Object);
                plr.SetupGet(c => c.ByUser).Returns(userMock.Object);

                list.Add(plr.Object);
            }

            return list.AsQueryable();
        }

        private Schedule MockSchedule(int i)
        {
            var schedule = new Mock<Schedule>();

            schedule.SetupGet(c => c.Id).Returns(Guid.NewGuid());
            schedule.SetupGet(c => c.Name).Returns("Product Group " + i);
            schedule.SetupGet(c => c.FrequencyType).Returns("Daily");
            schedule.SetupGet(c => c.FrequencyValue).Returns(2);
            schedule.SetupGet(c => c.ByUser).Returns(userMock.Object);
            schedule.SetupGet(c => c.Client).Returns(clientMock.Object);

            return schedule.Object;
        }

        private ProductModel[] MockProducts(int i)
        {
            var products = new List<ProductModel>();

            for (int j = 0; i < 2; i++)
            {
                products.Add(new ProductModel
                {
                    Id = Guid.NewGuid(),
                    ProductItem = "Product " + (i*j*100),
                    Selected = false,
                    SmsCode = Char.ConvertFromUtf32(i+97)
                });
            }

            return products.ToArray();
        }

        private ProductGroup MockProductGroup(int i)
        {
            var productGroup = new Mock<ProductGroup>();

            productGroup.SetupGet(c => c.Id).Returns(Guid.NewGuid());
            productGroup.SetupGet(c => c.Name).Returns("Product Group " + i);
            productGroup.SetupGet(c => c.ReferenceCode).Returns("PG"+ (i%10));

            return productGroup.Object;
        }

        private Campaign MockCampaign(int i)
        {
            var campaign = new Mock<Campaign>();

            campaign.SetupGet(c => c.Id).Returns(Guid.NewGuid());
            campaign.SetupGet(c => c.Name).Returns("Campaign " + i);
            campaign.SetupGet(c => c.StartDate).Returns(DateTime.UtcNow);
            campaign.SetupGet(c => c.EndDate).Returns(DateTime.UtcNow.AddDays(7));

            return campaign.Object;
        }

        internal void VerifyProductLevelRequestsQueried()
        {
            var productLevelRequestService = Mock.Get(controller.QueryProductLevelRequests);

            productLevelRequestService.Verify(c => c.Query());
        }
    }
}

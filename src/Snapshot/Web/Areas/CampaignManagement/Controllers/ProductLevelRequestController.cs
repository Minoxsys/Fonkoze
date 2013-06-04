using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Core.Domain;
using Core.Persistence;
using Core.Security;
using Domain;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
using Web.Models.Shared;
using Web.Security;
using Web.Services;
using Web.Areas.StockAdministration.Models.ProductGroup;

namespace Web.Areas.CampaignManagement.Controllers
{
    public class ProductLevelRequestController : Controller
    {
        private Core.Domain.User _user;
        private Client _client;

        public IQueryService<Client> LoadClient { get; set; }

        public IQueryService<User> QueryUsers { get; set; }

        public IQueryService<Schedule> QuerySchedules { get; set; }

        public IQueryService<Product> QueryProducts { get; set; }

        public IQueryService<Campaign> QueryCampaigns { get; set; }

        public IQueryService<ProductGroup> LoadProductGroup { get; set; }

        public IQueryService<ProductLevelRequest> QueryProductLevelRequests { get; set; }

        public IQueryService<ProductLevelRequestDetail> QueryProductLevelRequestDetails { get; set; }

        public ISaveOrUpdateCommand<ProductLevelRequest> SaveProductLevelRequest { get; set; }

        public ISaveOrUpdateCommand<Campaign> SaveCampaign { get; set; }

        public IProductLevelRequestMessagesDispatcherService DispatcherService { get; set; }
        public IPermissionsService PermissionService { get; set; }

        public GenerateProductLevelDetailsCommand GenerateProductLevelRequestDetails { get; set; }

        private const String PRODUCTLEVELREQUEST_ADD_PERMISSION = "ProductLevelRequest.Edit";
        private const String PRODUCTLEVELREQUEST_STOP_PERMISSION = "ProductLevelRequest.Stop";

        [Requires(Permissions = "ProductLevelRequest.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(PRODUCTLEVELREQUEST_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            ViewBag.HasNoRightsToStop = (PermissionService.HasPermissionAssigned(PRODUCTLEVELREQUEST_STOP_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();           

            return View();
        }

        public JsonResult Create(CreateProductLevelRequestInput createProductLevelRequestInput)
        {
            LoadUserAndClient();
            var productGroup = LoadProductGroup.Load(createProductLevelRequestInput.ProductGroupId.Value);
            var schedule = QuerySchedules.Load(createProductLevelRequestInput.ScheduleId.Value);
            var campaign = QueryCampaigns.Load(createProductLevelRequestInput.CampaignId.Value);

            var productLevelRequest = new ProductLevelRequest
            {
                ProductGroup = productGroup,
                Schedule = schedule,
                Campaign = campaign,

                ByUser = _user,
                Client = _client
            };

            productLevelRequest.StoreProducts<ProductModel[]>(createProductLevelRequestInput.Products);

            SaveProductLevelRequest.Execute(productLevelRequest);

            campaign.Opened = true;
            SaveCampaign.Execute(campaign);

            if (createProductLevelRequestInput.ScheduleId.Equals(Guid.Empty))
            {
                DispatcherService.DispatchMessagesForProductLevelRequest(productLevelRequest);
            }

            GenerateProductLevelRequestDetails.Execute(productLevelRequest);

            return Json(new JsonActionResponse
            {
                Status="Success",
                Message = "Saved Product Level Request"
            });

        }


		public JsonResult Edit(EditProductLevelRequestInput editProductLevelRequestInput)
		{
			LoadUserAndClient();
			var productGroup = LoadProductGroup.Load(editProductLevelRequestInput.ProductGroupId.Value);
			var schedule = QuerySchedules.Load(editProductLevelRequestInput.ScheduleId.Value);
			var campaign = QueryCampaigns.Load(editProductLevelRequestInput.CampaignId.Value);

			var productLevelRequest = QueryProductLevelRequests.Load(editProductLevelRequestInput.Id.Value);
				productLevelRequest.ProductGroup = productGroup;
				productLevelRequest.Schedule = schedule;
				productLevelRequest.Campaign = campaign;

				productLevelRequest.ByUser = _user;
				productLevelRequest.Client = _client;

			productLevelRequest.StoreProducts<ProductModel[]>(editProductLevelRequestInput.Products);

			SaveProductLevelRequest.Execute(productLevelRequest);
            GenerateProductLevelRequestDetails.Execute(productLevelRequest);

			return Json(new JsonActionResponse
			{
				Status = "Success",
				Message = "Saved Product Level Request"
			});

		}

        public JsonResult GetProductLevelRequests(IndexTableInputModel input)
        {

            LoadUserAndClient();
            var productLevelRequestData = QueryProductLevelRequests.Query().Where(c => c.Client == _client);

            if (!string.IsNullOrWhiteSpace(input.searchValue))
            {
                productLevelRequestData = productLevelRequestData.Where(c => c.Campaign.Name.Contains(input.searchValue));
            }

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<ProductLevelRequest>>>()
			{
                { "ScheduleName-ASC", () => productLevelRequestData.OrderBy(c => c.Schedule.Name) },
                { "ScheduleName-DESC", () => productLevelRequestData.OrderByDescending(c => c.Schedule.Name) },
                { "Campaign-ASC", () => productLevelRequestData.OrderBy(c => c.Campaign.Name) },
                { "Campaign-DESC", () => productLevelRequestData.OrderByDescending(c => c.Campaign.Name) }
			};
            var totalItems = productLevelRequestData.Count();
            productLevelRequestData = productLevelRequestData.Take(input.limit.Value).Skip(input.start.Value);


            var nowSchedule = new Schedule
            {
                Name = "Now",
                FrequencyType = "Now",
                FrequencyValue = 0

            };

            var productLevelRequests = productLevelRequestData.ToList().Select(req =>
                new GetProductLevelRequestModel
                {
					Id= req.Id.ToString(),
					CampaignId = req.Campaign.Id.ToString(),
					ProductGroupId = req.ProductGroup.Id.ToString(),
					ScheduleId =  (req.Schedule??nowSchedule).Id.ToString(),
					ProductIds = GetProductIds(req),

                    IsStopped = req.IsStopped,

                    Campaign = req.Campaign.Name,
                    StartDate = req.Campaign.StartDate.HasValue ? req.Campaign.StartDate.Value.ToString("dd-MMM-yyyy") : "-",
                    EndDate = req.Campaign.EndDate.HasValue ? req.Campaign.EndDate.Value.ToString("dd-MMM-yyyy") : "-",
                    ProductGroup = req.ProductGroup.Name,
                    ScheduleName = (req.Schedule??nowSchedule).Name,
                    ProductSmsCodes = req.Products != null ? GetSmsCodesRepresentation(req) : string.Empty,
                    Frequency = (req.Schedule??nowSchedule).FrequencyType ?? "Now",
                    Editable = req.Campaign.StartDate > DateTime.UtcNow && req.Schedule!=null

                }).ToArray();



            return Json(new GetProductLevelRequestResponse
            {
                TotalItems = totalItems,
                ProductLevelRequests = productLevelRequests
            }, JsonRequestBehavior.AllowGet);
        }

		private string[] GetProductIds(ProductLevelRequest req)
		{
            var products = req.RestoreProducts<ProductModel[]>();

			return products.Where(p => p.Selected).Select(p => p.Id.ToString()).ToArray();
		}

        private string GetSmsCodesRepresentation(ProductLevelRequest req)
        {
            var sb = new StringBuilder();

            var products = req.RestoreProducts<ProductModel[]>(); 
            if (products == null) return "--";

            for (int i = 0; i < products.Length; i++)
            {
                if (products[i].Selected)
                {
                    string format = "{0}/";
                    sb.AppendFormat(format, products[i].SmsCode);
                }
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        public JsonResult GetSchedules()
        {
            LoadUserAndClient();

            var schedulesData = QuerySchedules.Query().Where(c => c.Client == _client).ToList();

            var schedules = schedulesData.Select(schedule =>
                new ScheduleModel
                {
                    Basis = schedule.ScheduleBasis,
                    Frequency = schedule.FrequencyType ?? "-",
                    ScheduleName = schedule.Name,
                    Id = schedule.Id.ToString(),
                    Reminders = schedule.Reminders.Select(reminder => new RequestReminderModel
                    {
                        PeriodType = reminder.PeriodType,
                        PeriodValue = reminder.PeriodValue
                    }).ToArray(),
                    Selected = false
                }).ToArray();

            return Json(schedules, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProducts(GetProductsInput input)
        {
            LoadUserAndClient();
            var productsDataQry = QueryProducts.Query().Where(p => p.Client == _client);

            if (input.ProductGroupId.HasValue && input.ProductGroupId.Value != Guid.Empty)
            {
                productsDataQry = productsDataQry.Where(it => it.ProductGroup.Id == input.ProductGroupId.Value);
            }

            var products = productsDataQry.Select(product =>
                new ProductModel
                {
                    Id = product.Id,
                    ProductItem = product.Name,
                    SmsCode = product.SMSReferenceCode,
                    Selected = false 

                }).ToArray();


            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCampaigns()
        {
            LoadUserAndClient();
            var campaignsDataQry = QueryCampaigns.Query().Where(p => p.Client == _client);
            List<CampaignModel> campaigns = new List<CampaignModel>();

            foreach (var campaign in campaignsDataQry)
            {
                var model = new CampaignModel
                {
                    Id = campaign.Id.ToString(),
                    Name = campaign.Name
                };
                campaigns.Add(model);
            }


            return Json(campaigns.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public void StopProductLevelRequest(StopProductLevelRequestInput stopProductLevelRequestInput)
        {
            var productLevelRequest = QueryProductLevelRequests.Load(stopProductLevelRequestInput.Id.Value);

            productLevelRequest.IsStopped = true;

            SaveProductLevelRequest.Execute(productLevelRequest);

        }

        public JsonResult GetProductGroups()
        {
            LoadUserAndClient();

            var productGroups = LoadProductGroup.Query().Where(p => p.Client == _client).ToList();
            var productGroupModelList = new List<ProductGroupModel>();

            foreach (var productGroup in productGroups)
            {
                var productGroupModel = new ProductGroupModel();
                productGroupModel.Id = productGroup.Id;
                productGroupModel.Name = productGroup.Name;
                productGroupModelList.Add(productGroupModel);
            }

            return Json(new
            {
                productGroups = productGroupModelList.ToArray(),
                TotalItems = productGroupModelList.Count
            }, JsonRequestBehavior.AllowGet);
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = LoadClient.Load(clientId);
        }


        public JsonResult GetProductLevelRequestDetails(Guid? productLevelRequestId)
        {
            var results = QueryProductLevelRequestDetails.Query().Where(p => p.ProductLevelRequestId == productLevelRequestId.Value).Select(it=>
               new ProductLevelDetailModel
               {
                   OutpostName=it.OutpostName,
                   ProductGroupName = it.ProductGroupName,
                   Method = it.Method,
                   RequestMessage= it.RequestMessage,
                   Updated = it.Updated.Value.ToString("dd-MMM-yyyy HH:mm:ss")
                   
               }
                ).ToArray();

            return Json(new GetProductLevelRequestDetailsModel{ ProductLevelRequestDetails = results }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Requires(Permissions="ProductLevelRequest.View")]
        public EmptyResult RecalculateProductLevelRequestDetails(Guid? productLevelRequestId)
        {

            var productLevelRequest = QueryProductLevelRequests.Load(productLevelRequestId.Value);

            GenerateProductLevelRequestDetails.Execute(productLevelRequest);

            return new EmptyResult();
        }


    }
}

using System;
using System.Linq;
using System.Web.Mvc;
using Core.Services;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
using Web.Models.Shared;
using System.Collections.Generic;
using System.Text;

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

        public ISaveOrUpdateCommand<ProductLevelRequest> SaveProductLevelRequest { get; set; }


        public ActionResult Overview()
        {
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

			return Json(new JsonActionResponse
			{
				Status = "Success",
				Message = "Saved Product Level Request"
			});

		}

        public JsonResult GetProductLevelRequests(GetProductLevelRequestInput input)
        {

            LoadUserAndClient();
            var productLevelRequestData = QueryProductLevelRequests.Query().Where(c => c.Client == _client);

            if (!string.IsNullOrWhiteSpace(input.search))
            {
                productLevelRequestData = productLevelRequestData.Where(c => c.Campaign.Name.Contains(input.search));
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


            var productLevelRequests = productLevelRequestData.ToList().Select(req =>
                new GetProductLevelRequestModel
                {
					Id= req.Id.ToString(),
					CampaignId = req.Campaign.Id.ToString(),
					ProductGroupId = req.ProductGroup.Id.ToString(),
					ScheduleId = req.Schedule.Id.ToString(),
					ProductIds = GetProductIds(req),

                    IsStopped = req.IsStopped,

                    Campaign = req.Campaign.Name,
                    StartDate = req.Campaign.StartDate.HasValue ? req.Campaign.StartDate.Value.ToString("dd-MMM-yyyy") : "-",
                    EndDate = req.Campaign.EndDate.HasValue ? req.Campaign.EndDate.Value.ToString("dd-MMM-yyyy") : "-",
                    ProductGroup = req.ProductGroup.Name,
                    ScheduleName = req.Schedule.Name,
                    ProductSmsCodes = req.Products != null ? GetSmsCodesRepresentation(req) : string.Empty,
                    Frequency = req.Schedule.FrequencyType ?? "Now",
                    Editable = req.Campaign.StartDate > DateTime.UtcNow

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
                string format = "{0}/";
                if (products[i].Selected)
                {
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
            if (input.ProductGroupId.HasValue == false)
            {
                throw new ArgumentNullException("No product group id specified");
            }

            var productsDataQry = QueryProducts.Query().Where(p => p.Client == _client && p.ProductGroup.Id == input.ProductGroupId.Value).ToList();

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
            var campaignsDataQry = QueryCampaigns.Query().Where(p => p.Client == _client).ToList();

            var campaigns = campaignsDataQry.Select(campaign =>
                new CampaignModel
                {
                    Id = campaign.Id.ToString(),
                    Name = campaign.Name

                }).ToArray();


            return Json(campaigns, JsonRequestBehavior.AllowGet);
        }

        public void StopProductLevelRequest(StopProductLevelRequestInput stopProductLevelRequestInput)
        {
            var productLevelRequest = QueryProductLevelRequests.Load(stopProductLevelRequestInput.Id.Value);

            productLevelRequest.IsStopped = true;

            SaveProductLevelRequest.Execute(productLevelRequest);

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



    }
}

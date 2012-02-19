using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Core.Domain;

namespace Web.Areas.OutpostManagement.Controllers
{
    public class AssignProductGroupController : Controller
    {
        private Core.Domain.User _user;
        private Client _client;


        public IQueryService<OutpostStockLevel> QueryOutpostStockLevel { get; set; }
        public IQueryService<Product> QueryProducts { get; set; }

		public IQueryService<Outpost> LoadOutpost { get; set; }
		public IQueryService<ProductGroup> LoadProductGroup { get; set; }

        public IQueryService<Client> LoadClient { get; set; }
        public IQueryService<User> QueryUsers { get; set; }


        public class GetProductsInput
        {
			public Guid? OutpostId{get;set;}
            public Guid? ProductGroupId { get; set; }
        }

		public class GetProductsOutputModel{
			public string Id { get; set; }
			public string Name { get; set; }
			public string SmsCode { get; set; }
			public bool Selected { get; set; }
		}
        public JsonResult GetProducts(GetProductsInput input)
        {
			LoadUserAndClient();

			var outpost = LoadOutpost.Load(input.OutpostId.Value);
			var productGroup = LoadProductGroup.Load(input.ProductGroupId.Value);

			var outpostStockLevels = QueryOutpostStockLevel.Query()
				.Where(o=>o.Client == _client)
				.Where(o=>o.ProductGroup == productGroup)
				.Where(o=>o.Outpost == outpost);

			var products = QueryProducts.Query();
			// todo add product.Client in the where clause, after Elena fixes the issues
			var selectedProducts = (from p in products
									select new GetProductsOutputModel
									{
										Id = p.Id.ToString(),
										Name = p.Name,
										SmsCode =p.SMSReferenceCode,
										Selected = outpostStockLevels.Any(s => s.Product == p)
									}).ToArray();


            return Json(selectedProducts, JsonRequestBehavior.AllowGet);
        }

        public class GetOutpostStockLevelInput
        {
            public Guid? OutpostId { get; set; }
        }

        public class OutpostStockLevelModel
        {
            public string Id { get; set; }
            public string Group { get; set; }
            public string ProductItem { get; set; }
            public string SmsCode { get; set; }
            public int ProductLevel { get; set; }
            public string Update { get; set; }
        }

        public class GetOutpostStockLevelOutpost
        {
            public OutpostStockLevelModel[] OutpostStockLevels { get; set; }
           
        }
        public JsonResult GetOutpostStockLevels(GetOutpostStockLevelInput input)
        {
            LoadUserAndClient();
            if (input.OutpostId.HasValue == null)
            {
                throw new ArgumentNullException("outpostId");
            }

            var outpostStockLevels = QueryOutpostStockLevel.Query().Where(o=>o.Client == _client && o.Outpost.Id == input.OutpostId).ToList();


            var response = (from o in outpostStockLevels.ToList()
                            select new OutpostStockLevelModel
                            {
                                Id = o.Id.ToString(),
                                Group = o.ProductGroup.Name,
                                ProductItem = o.Product.Name,
                                SmsCode = o.Product.SMSReferenceCode,
                                ProductLevel = o.StockLevel,
                                Update = o.Updated.Value.ToString("dd-MMM-yyyy")
                            }).ToArray();

            return Json(response, JsonRequestBehavior.AllowGet);
        
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

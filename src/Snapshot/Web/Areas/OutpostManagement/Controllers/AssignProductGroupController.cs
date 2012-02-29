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
        public IQueryService<OutpostHistoricalStockLevel> QueryOutpostHistoricalStockLevel { get; set; }

        public IQueryService<Product> QueryProducts { get; set; }

		public IQueryService<Outpost> LoadOutpost { get; set; }
		public IQueryService<ProductGroup> LoadProductGroup { get; set; }

        public IQueryService<Client> LoadClient { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        public IDeleteCommand<OutpostStockLevel> DeleteOutpostStockLevelCommand { get; set; }
        public ISaveOrUpdateCommand<OutpostStockLevel> SaveOutpostStockLevelCommand { get; set; }


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

            public bool HasStockLevels { get; set; }
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

            var historicalStockLevels = QueryOutpostHistoricalStockLevel.Query()
                .Where(o=>o.ProductGroupId == productGroup.Id)
                .Where(o=>o.OutpostId == outpost.Id);

			var products = QueryProducts.Query();
            products = products.Where(p => p.Client == _client && p.ProductGroup == productGroup);

			var selectedProducts = (from p in products
									select new GetProductsOutputModel
									{
										Id = p.Id.ToString(),
										Name = p.Name,
										SmsCode =p.SMSReferenceCode,
										Selected = outpostStockLevels.Any(s => s.Product == p),
                                        HasStockLevels = outpostStockLevels.Any(s=>s.Product == p && (s.PrevStockLevel > 0 || s.StockLevel > 0 )) || historicalStockLevels.Any(hs=> hs.ProductId == p.Id)
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

        public class ProductAssignmentDetail
        {
            public Guid Id { get; set; }
            public bool Selected { get; set; }
        }
        public class ModifyProductAssignmentsInput
        {
            public Guid OutpostId { get; set; }
            public Guid ProductGroupId { get; set; }
            public ProductAssignmentDetail[] Assignments { get; set; }

        }

        public EmptyResult ModifyProductAssignments(ModifyProductAssignmentsInput input )
        {
            LoadUserAndClient();

            var stockLevels = QueryOutpostStockLevel.Query()
                .Where(c=>c.Outpost.Id == input.OutpostId)
                .Where(c=>c.ProductGroup.Id == input.ProductGroupId)
                .ToList();

            var assignments = input.Assignments.ToList();

            stockLevels.ForEach(stockItem =>
                {
                    if (assignments.Any(asgn => asgn.Id== stockItem.Product.Id && !asgn.Selected))
                    {
                        DeleteOutpostStockLevelCommand.Execute(stockItem);
                    }

                });

            assignments.Where(it => it.Selected).ToList().ForEach(product =>
            {
                var outpostStockItem = new OutpostStockLevel
                {
                    Product= QueryProducts.Load(product.Id),
                    Outpost = LoadOutpost.Load(input.OutpostId),
                    ProductGroup = LoadProductGroup.Load(input.ProductGroupId),
                    UpdateMethod= OutpostStockLevel.MANUAL_UPDATE,
                    Client = _client,
                    ByUser = _user
                };

                SaveOutpostStockLevelCommand.Execute(outpostStockItem);

            });

            




            return new EmptyResult();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Models.Contact;
using AutoMapper;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Models.Shared;

namespace Web.Areas.OutpostManagement.Controllers
{
	public class OutpostController : Controller
	{
		public OutpostModel OutpostModel { get; set; }
		public OutpostOutputModel OutpostModelOutput { get; set; }

		public IQueryService<Outpost> QueryWarehouse { get; set; }
		public IQueryService<Outpost> QueryService { get; set; }
		public IQueryService<Country> QueryCountry { get; set; }
		public IQueryService<Region> QueryRegion { get; set; }
		public IQueryService<District> QueryDistrict { get; set; }
		public IQueryService<Client> LoadClient { get; set; }
		public IQueryService<User> QueryUsers { get; set; }
		public IQueryService<Product> QueryProduct { get; set; }
		public IQueryService<Contact> QueryContact { get; set; }

		public ISaveOrUpdateCommand<Outpost> SaveOrUpdateCommand { get; set; }
		public ISaveOrUpdateCommand<Contact> SaveOrUpdateCommandContact { get; set; }

		public IDeleteCommand<Outpost> DeleteCommand { get; set; }
		public IDeleteCommand<Contact> DeleteContactCommand { get; set; }

		public OutpostOutputModel OutpostOutputModel { get; set; }

		public OutpostInputModel OutpostInputModel { get; set; }

		public OutpostOutputModel CreateOutpost { get; set; }

		private const string TEMPDATA_ERROR_KEY = "error";
		private Core.Domain.User _user;
		private Client _client;


		[HttpPost]
		public RedirectToRouteResult DeleteContact(Guid outpostID, Guid contactId)
		{
			var outpost = QueryService.Load(outpostID);
			var contact = QueryContact.Load(contactId);

			if (contact != null)
			{
				DeleteContactCommand.Execute(contact);
			}

			return RedirectToAction("Edit", "Outpost", new
			{
				outpostId = outpostID
			});
		}

		[HttpGet]
		public JsonResult GetProductsList(Guid? productId)
		{
			List<SelectListItem> Outposts = new List<SelectListItem>();

			var outposts = QueryService.Query().Where(it => it.District.Id == productId.Value);

			if (outposts.ToList().Count > 0)
			{
				foreach (var item in outposts)
				{
					Outposts.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
				}
			}
			var jsonResult = new JsonResult();
			jsonResult.Data = Outposts;

			return Json(Outposts, JsonRequestBehavior.AllowGet);
		}

		public ActionResult CreateContact(Guid outpostId)
		{
			var model = new ContactModel();
			model.OutpostId = outpostId;
			model.ContactType = "Mobile Number";
			return View("CreateContact", model);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult CreateContact(ContactModel contactModel)
		{
			var model = new ContactModel();

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var contact = new Contact();

			Outpost outpost = QueryService.Load(contactModel.OutpostId);
			var contacts = QueryContact.Query().Where(m => m.Outpost.Id == outpost.Id);

			contact.Client = LoadClient.Load(Client.DEFAULT_ID);
			;

			Mapper.Map(contactModel, contact);
			if (outpost != null)
			{
				if (contacts.Count() == 0)
				{
					contact.IsMainContact = true;
				}
				outpost.Contacts.Add(contact);
			}

			SaveOrUpdateCommand.Execute(outpost);

			return RedirectToAction("Edit", "Outpost", new { outpostId = contactModel.OutpostId });
		}


		[HttpGet]
		public ActionResult Overview()
		{
			OutpostOverviewModel model = new OutpostOverviewModel();

			return View(model);
		}

		[HttpPost]
		public JsonResult Delete(Guid outpostId)
		{
			var outpost = QueryService.Load(outpostId);

			if (outpost != null)
			{
				DeleteCommand.Execute(outpost);
			}

			return Json(new JsonActionResponse
			{
				Status = "Success",
				Message = string.Format("Successfully removed outpost {0}", outpost.Name)
			});
		}

		public JsonResult GetOutposts(GetOutpostsInputModel input)
		{
			var model = new GetOutpostsOutputModel();
			LoadUserAndClient();

			var outpostsQueryData = QueryService.Query().
			Where(c => c.Client == this._client);
			if (input.districtId.HasValue)
			{
				outpostsQueryData = outpostsQueryData.Where(o => o.District.Id == input.districtId.Value);
			}

			var orderByColumnDirection = new Dictionary<string, Func<IQueryable<Outpost>>>()
			{
				{ "Name-ASC", () => outpostsQueryData.OrderBy(c => c.Name) },
				{ "Name-DESC", () => outpostsQueryData.OrderByDescending(c => c.Name) }
			};

			Func<IQueryable<Outpost>> orderOutposts;
			if (orderByColumnDirection.TryGetValue(String.Format("{0}-{1}", input.sort, input.dir), out orderOutposts))
			{
				outpostsQueryData = orderOutposts.Invoke();
			}

			model.TotalItems = outpostsQueryData.Count();
			outpostsQueryData = outpostsQueryData.Take(input.limit.Value).Skip(input.start.Value);
			model.Outposts = (from o in outpostsQueryData.ToList() select new GetOutpostsOutputModel.OutpostModel
			{
				Id = o.Id.ToString(),
				Name = o.Name,
				IsWarehouse = o.IsWarehouse,
				WarehouseName = o.Warehouse != null ? o.Warehouse.Name : string.Empty,
				Coordinates = o.Latitude + "" + o.Longitude, // TODO drop this and just add a simple Coordintates property, the client should have accepted it in the first place
				ContactMethod = o.DetailMethod,
				CountryId = o.Country.Id.ToString(),
				RegionId = o.Region.Id.ToString(),
				DistrictId = o.District.Id.ToString(),
				WarehouseId = o.Warehouse != null ? o.Warehouse.Id.ToString() : string.Empty
			}).ToArray();

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetDistricts(Guid? regionId)
		{
			LoadUserAndClient();
			var model = new GetDistrictsOutputModel();

			if (regionId.HasValue && regionId.Value != Guid.Empty)
			{
				model.Districts = this.QueryDistrict.Query().Where(m => m.Region.Id == regionId &&
																		m.Client == _client).
				Select(district => new GetDistrictsOutputModel.DistrictModel
				{
					Id = district.Id,
					Name = district.Name
				}).ToArray();
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetWarehouses()
		{
			var model = new GetWarehousesOutputModel();
			LoadUserAndClient();

			var warehouseQueryData = QueryService.Query()
												 .Where(c => c.Client == this._client && c.IsWarehouse == true)
												 .OrderBy(w => w.Name);
			model.Warehouses = (from w in warehouseQueryData.ToList() select new GetWarehousesOutputModel.WarehouseModel
			{
				Id = w.Id.ToString(),
				Name = w.Name
			}).ToArray();

			return Json(model, JsonRequestBehavior.AllowGet);
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

		[HttpPost]
		public JsonResult Create(CreateOutpostInputModel model)
		{
			LoadUserAndClient();
			var outpost = new Outpost();
			MapInputToOutpost(model, ref outpost);

			SaveOrUpdateCommand.Execute(outpost);

			return Json(new JsonActionResponse
			{
				Message = string.Format("Created successfully outpost {0}", outpost.Name),
				Status = "Success"
			});
		}

		[HttpPost]
		public JsonResult Edit(EditOutpostInputModel model)
		{
			LoadUserAndClient();

			var outpost = QueryService.Load(model.EntityId.Value);
			MapInputToOutpost(model, ref outpost);

			SaveOrUpdateCommand.Execute(outpost);

			return Json(new JsonActionResponse
			{
				Message = string.Format("Saved successfully outpost {0}", outpost.Name),
				Status = "Success"
			});
		}

		private void MapInputToOutpost(CreateOutpostInputModel model, ref Outpost outpost)
		{
			outpost. Name = model.Name;
			outpost. IsWarehouse = model.IsWarehouse.Value;
			outpost. Latitude = model.Coordinates;
			outpost. Client = _client;
			outpost. ByUser = _user;

			outpost.Country = QueryCountry.Load(model.CountryId.Value);
			outpost.Region = QueryRegion.Load(model.RegionId.Value);
			outpost.District = QueryDistrict.Load(model.DistrictId.Value);

			outpost.Warehouse = null;
			if (model.WarehouseId.HasValue)
			{
				outpost.Warehouse = QueryService.Load(model.WarehouseId.Value);
			}
		}
	}
}
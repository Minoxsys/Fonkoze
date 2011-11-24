using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Web.Models.RoleManager;
using Core.Domain;
using Persistence.Commands;
using Persistence.Queries;
using Persistence;
using Core.Persistence;
using Web.Security;

namespace Web.Controllers
{
	public class RoleManagerController :Controller	{
		[Dependency]
		public RoleManagerListOutputModel ListOutputModel
		{
			get;
			set;
		}

		[Dependency]
		public RoleManagerCreateOutputModel CreateOutputModel
		{
			get;
			set;
		}

		[Dependency]
		public ISaveOrUpdateCommand<Role> SaveOrUpdate
		{
			get;
			set;
		}


		[Dependency]
		public RoleManagerEditOutput EditOutputModel
		{
			get;
			set;
		}

		[Dependency]
		public RoleManagerAssignModel AssignModel
		{
			get;
			set;
		}
	
		[Dependency]
		public RoleManagerUnAssignModel UnAssignModel
		{
			get;
			set;
		}

        [Requires(Permissions = "RoleManager.Overview")]
		public ActionResult List()
		{

			ListOutputModel.InfoMessage = string.Empty + (string)TempData["info"];
			return View(ListOutputModel);
		}

        [Requires(Permissions = "RoleManager.CRUD")]
		public ActionResult Create()
		{

			return View(CreateOutputModel);
		}

		[HttpPost]
        [Requires(Permissions = "RoleManager.CRUD")]
		public ActionResult Create( RoleManagerCreateInputModel input )
		{
			if (!this.ModelState.IsValid)
				return View(CreateOutputModel);

			SaveOrUpdate.Execute(new Role
			{
				Name = input.Role.Name,
				Description = input.Role.Description
			});
			TempData.Add("info", "Your Role has been saved");

			return RedirectToAction("List");
		}

        [Requires(Permissions = "RoleManager.CRUD")]
		public ActionResult Edit( Guid id )
		{
			if (id == Guid.Empty)
				return RedirectToAction("Index", "Home");

			EditOutputModel.Load(id);

			if (EditOutputModel.Role == null || EditOutputModel.Role.Id == Guid.Empty)
			{

				return RedirectToAction("Index", "Home");

			}

            EditOutputModel.Info = (string)TempData["info"] ?? "";

			return View(EditOutputModel);
		}
        [HttpPost]
        [Requires(Permissions = "RoleManager.CRUD")]
		public ActionResult Edit( RoleManagerEditInput model )
		{

            if (model.Role.Id != Guid.Empty && this.ModelState.IsValid == false)
                return Edit(model.Role.Id);

			EditOutputModel.Load(model.Role.Id);
            if (EditOutputModel.Role == null)
            {
				return RedirectToAction("List");
            }

            EditOutputModel.Role.Description = model.Role.Description;
            EditOutputModel.Role.Name = model.Role.Name;

            SaveOrUpdate.Execute(EditOutputModel.Role);

			TempData.Add("info", "Your Role has been saved");

            return RedirectToAction("Edit", new { id = EditOutputModel.Role.Id });
		}

		[HttpPost]
        [Requires(Permissions = "RoleManager.CRUD")]
		public EmptyResult Assign( Guid functionId, Guid roleId )
		{

			AssignModel.LinkFunctionToRole(functionId, roleId);
			return new EmptyResult();
		}


        [Requires(Permissions = "RoleManager.CRUD")]
		public EmptyResult UnAssign( Guid functionId, Guid roleId )
		{
			UnAssignModel.RemoveFunctionFromRole(functionId, roleId);

			return new EmptyResult();
		}
	}
}

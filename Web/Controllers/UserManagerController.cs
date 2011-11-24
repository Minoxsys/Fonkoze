﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Web.Models.UserManager;
using Core.Domain;
using Core.Persistence;
using Web.Security;

namespace Web.Controllers
{
	public class UserManagerController :
		Controller
	{
		[Dependency]
		public UserManagerListModel ListModel
		{
			get;
			set;
		}


		[Dependency]
		public UserManagerCreateModel CreateModel
		{
			get;
			set;
		}

		[Dependency]
		public UserManagerEditModel EditModel
		{
			get;
			set;
		}

		[Dependency]
		public UserManagerAssignModel AssignModel
		{
			get;
			set;
		}
		[Dependency]
		public UserManagerUnAssignModel UnAssignModel
		{
			get;
			set;
		}

		[Dependency]
		public ISaveOrUpdateCommand<User> SaveOrUpdate { get; set; }

        [Requires(Permissions = "UserManager.Overview")]
		public ViewResult List()
		{
			ListModel.InfoMessage = string.Empty + (string)TempData["info"];
			return View(ListModel);
		}

        [Requires(Permissions = "UserManager.CRUD")]
		public ViewResult Create()
		{
			return View(CreateModel);
		}

		[HttpPost]
        [Requires(Permissions = "UserManager.CRUD")]
		public ActionResult Create( [Bind(Exclude = "Id")]User employee )
		{
			if (!this.ModelState.IsValid)
				return View(CreateModel);

			SaveOrUpdate.Execute(employee);
			TempData.Add("info", employee.UserName + " has been saved");


			return RedirectToAction("List");
		}

        [Requires(Permissions = "UserManager.CRUD")]
		public ActionResult Edit( Guid id )
		{
			EditModel.Load(id);


			return View(EditModel);

		}

        [Requires(Permissions = "UserManager.CRUD")]
		public EmptyResult Assign( Guid employeeId, Guid roleId )
		{
			AssignModel.LinkUserToRole(employeeId, roleId);

			return new EmptyResult();
		}


        [Requires(Permissions = "UserManager.CRUD")]
		public EmptyResult UnAssign( Guid employeeId, Guid roleId )
		{
			UnAssignModel.RemoveRole(employeeId, roleId);

			return new EmptyResult();
		}
	}
}
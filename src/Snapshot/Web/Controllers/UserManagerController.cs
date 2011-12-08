using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.UserManager;
using Core.Domain;
using Core.Persistence;
using Web.Security;
using AutoMapper;
using Core.Services;

namespace Web.Controllers
{
	public class UserManagerController :
		Controller
	{
		
		public UserManagerListModel ListModel
		{
			get;
			set;
		}


		
		public UserManagerCreateModel CreateModel
		{
			get;
			set;
		}

		
		public UserManagerEditModel EditModel
		{
			get;
			set;
		}

		
		public UserManagerAssignModel AssignModel
		{
			get;
			set;
		}
		
		public UserManagerUnAssignModel UnAssignModel
		{
			get;
			set;
		}

		
		public ISaveOrUpdateCommand<User> SaveOrUpdate { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IDeleteCommand<User> DeleteUser { get; set; }

        public ISecurePassword SecurePassword { get; set; }

        //[Requires(Permissions = "UserManager.Overview")]
		public ViewResult List()
		{
			ListModel.InfoMessage = string.Empty + (string)TempData["info"];
			return View(ListModel);
		}

        //[Requires(Permissions = "UserManager.CRUD")]
		public ViewResult Create()
		{
			return View(CreateModel);
		}

		[HttpPost]
        //[Requires(Permissions = "UserManager.CRUD")][Bind(Exclude = "Employee.Id")]
        public ActionResult Create( UserManagerCreateInputModel model)
		{           
			if (!this.ModelState.IsValid)
				return View(CreateModel);

            if (!model.ConfirmedPassword.Equals(model.Employee.Password))
            {
                ViewData.ModelState.AddModelError("Password", "Passwords does not match!");
                return View(CreateModel);
            }

            var user = new User();
            CreateMapping();

            model.Employee.Password = SecurePassword.EncryptPassword(model.Employee.Password);

            Mapper.Map(model.Employee, user);

			SaveOrUpdate.Execute(user);
			TempData.Add("info", model.Employee.UserName + " has been saved");


			return RedirectToAction("List");
		}

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var user = QueryUsers.Load(id);

            if (user.Id != Guid.Empty)
                DeleteUser.Execute(user);

            return RedirectToAction("List");
 
        }
        private void CreateMapping()
        {
            Mapper.CreateMap<UserModel, User>();

            Mapper.CreateMap<User, UserModel>();
        }

        //[Requires(Permissions = "UserManager.CRUD")]
		public ActionResult Edit( Guid id )
		{
			EditModel.Load(id);


			return View(EditModel);

		}

        //[Requires(Permissions = "UserManager.CRUD")]
		public EmptyResult Assign( Guid employeeId, Guid roleId )
		{
			AssignModel.LinkUserToRole(employeeId, roleId);

			return new EmptyResult();
		}


        //[Requires(Permissions = "UserManager.CRUD")]
		public EmptyResult UnAssign( Guid employeeId, Guid roleId )
		{
			UnAssignModel.RemoveRole(employeeId, roleId);

			return new EmptyResult();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain;
using System.Web.Mvc;
using Core.Persistence;
using Domain;

namespace Web.Models.UserManager
{
	public class UserManagerCreateModel
	{
		public  UserModel Employee
		{
			get;
			set;
		}
		public string InfoMessage
		{
			get;
			set;
		}
         public List<SelectListItem> Clients
        { 
            get;
            set;
        }
         public string ConfirmedPassword { get; set; }

        private IQueryService<Client> queryClients;

      
        public UserManagerCreateModel(IQueryService<Client> queryClients)
        {
            this.queryClients = queryClients;
            Clients = new List<SelectListItem>();
            Employee = new UserModel();

            var result = queryClients.Query().ToList();

            foreach(Client item in result)
            {
                SelectListItem selectItem = new SelectListItem();
                selectItem.Text = item.Name;
                selectItem.Value = item.Id.ToString();
                Clients.Add(selectItem);
                
            }        
        }
	}
}
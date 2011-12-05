using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Services;

namespace Web.Security
{
    public class FreeToAllMembershipService:IMembershipService
    {
        const bool COME_IN_PLEASE = true;
        public bool ValidateUser(string userName, string password)
        {
            return COME_IN_PLEASE;
        }
    }
}
using Core.Domain;
using Domain;

namespace Web.Security
{
    public class UserAndClientIdentity
    {
        public User User { get; set; }
        public Client Client { get; set; }
    }
}

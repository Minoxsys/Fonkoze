using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Core.Domain;
using Persistence;
using Persistence.Queries.Employees;
using Persistence.Conventions;
using NHibernate.Linq;

namespace IntegrationTests
{
	[TestFixture]

	public class When_WeWantToPersist_A_User : GivenAPersistenceSpecification<User>
	{
        const string PASSWORD = "123adn";
        const string EMAIL = "some@evoz.com";
        Guid CLIENT_ID = Guid.Empty;
        const string USERNAME = "mihai.lazar";

		[Test]
		public void It_ShouldSuccessfullyPersist_AnEmployee()
		{          

            var employee = Specs
                .CheckProperty(e => e.UserName, USERNAME)
                .CheckList(e => e.Roles, new List<Role> { new Role { Name = "Admnistrator", Description = "The users with this role will " } })
                .CheckProperty(e => e.ClientId, CLIENT_ID)
                .CheckProperty(e => e.Email, EMAIL)
                .CheckProperty(e => e.Password, PASSWORD)
                .VerifyTheMappings();

			Assert.IsNotNull(employee);
			Assert.AreEqual("mihai.lazar", employee.UserName);
            Assert.AreEqual(PASSWORD, employee.Password);
            Assert.AreEqual(EMAIL, employee.Email);
            Assert.AreEqual(CLIENT_ID, employee.ClientId);

            Assert.IsInstanceOf<Guid>(employee.ClientId);
           	Assert.IsNotNull(employee.Roles);
			Assert.AreEqual(1, employee.Roles.Count);

            
			session.Delete(employee);            
            session.Flush();

		}

		[Test]
		public void It_ShouldQueryFor_EmployeeByName()
		{
			var employee = Specs
				.CheckProperty(e => e.UserName, USERNAME)
                .CheckProperty(e=> e.ClientId,CLIENT_ID)
                .CheckProperty( e => e.Password,PASSWORD)
                .CheckProperty(e=>e.Email,EMAIL)
				.CheckList(e => e.Roles, new List<Role> { new Role { Name = "Admnistrator", Description = "The users with this role will " } })
				.VerifyTheMappings();

			Assert.IsNotNull(employee);
			Assert.AreEqual(USERNAME, employee.UserName);
            Assert.AreEqual(PASSWORD, employee.Password);
            Assert.AreEqual(EMAIL, employee.Email);
            Assert.AreEqual(CLIENT_ID, employee.ClientId);

			Assert.IsNotNull(employee.Roles);
			Assert.AreEqual(1, employee.Roles.Count);


			var unitOfWork = new NHibernateUnitOfWork(_sessionFactory);
			unitOfWork.Initialize();


			var s = new Persistence.Queries.NHibernateQueryService<User>(unitOfWork);
			;
			var emplByName = new UserByUserName(USERNAME);

			var xx = s.Query(emplByName);
			var x = xx.ToList();
			Assert.IsNotNull(x);

			session.Delete(employee);

			session.Flush();

			unitOfWork.Close();
		}
	}
}

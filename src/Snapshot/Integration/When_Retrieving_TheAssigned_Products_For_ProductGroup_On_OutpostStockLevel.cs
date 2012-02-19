using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Core.Domain;
using NHibernate.Linq;

namespace IntegrationTests
{
	[TestFixture]
	public class When_Retrieving_TheAssigned_Products_For_ProductGroup_On_OutpostStockLevel
		: GivenAPersistenceSpecification<ProductGroup>
	{
		Client client;
		User user;

		Outpost outpost;

		ProductGroup productGroup;
		List<Product> productsCache = new List<Product>();
		List<OutpostStockLevel> oslCache = new List<OutpostStockLevel>();

		[Test]
		public void Foobar()
		{
			var pg = session.Load<ProductGroup>(productGroup.Id);
			var outp = session.Load<Outpost>(outpost.Id);

			var products = session.Query<Product>().Where(p=>p.ProductGroup == pg);
			var stockLevels = session.Query<OutpostStockLevel>()
				.Where(o=>o.Client == client)
				.Where(o=>o.ProductGroup == pg)
				.Where(o=>o.Outpost == outp);

			var selectedProducts = (from p in products
									select new
									{
										product = p,
										selected = stockLevels.Any(s=>s.Product == p)
									}).ToList();

			var x = selectedProducts;
		}

		[SetUp]
		public void BeforeEach()
		{
			InitDbData();
		}
		[TearDown]
		public void AfterEach()
		{
			ClearDbData();
		}

		private void ClearDbData()
		{
			using (var clearSession = _sessionFactory.CreateSession())
			{

				oslCache.ForEach(osl =>
					clearSession.Delete(clearSession.Load<OutpostStockLevel>(osl.Id)));

				productsCache.ForEach(p =>
					clearSession.Delete(clearSession.Load<Product>(p.Id)));

				clearSession.Delete(clearSession.Load<ProductGroup>(productGroup.Id));
				clearSession.Delete(clearSession.Load<Outpost>(outpost.Id));
				clearSession.Delete(clearSession.Load<Client>(client.Id));
				clearSession.Delete(clearSession.Load<User>(user.Id));

				clearSession.Flush();
			}
		}
		private void InitDbData()
		{
			var buildSession = _sessionFactory.CreateSession();
			buildSession.FlushMode = NHibernate.FlushMode.Always;


            user = new User{
                Email = "user@project.com",
                Password = "Darn",
                FirstName = "David",
                LastName = "Jones",
                UserName ="david.jones"
            };

            buildSession.Save(user);

            client = new Client
            {
                ByUser = user,
                Name = "Univers"
            };

            buildSession.Save(client);

            user.ClientId = client.Id;
            buildSession.Update(user);

            outpost = new Outpost
            {
                ByUser = user,
                Client = client,
                Name = "Sri Lanka",
                DetailMethod = "0741 551 994"

            };
            buildSession.Save(outpost);

            productGroup = new ProductGroup
            {
                ByUser = user,
                Name = "Malaria",
                ReferenceCode = "MAL",
                Description = "Contains all drugs within the Malaria Group"
            };

            buildSession.Save(productGroup);

			Create10Products(buildSession);

			Assign3ProductsToOutpost(buildSession);

			buildSession.Flush();
			buildSession.Dispose();
		}

		private void Assign3ProductsToOutpost(NHibernate.ISession buildSession)
		{
			for (int i = 0; i < 3; i++)
			{
				var osl = new OutpostStockLevel
				{
					ByUser = user,
					Client = client,

					Outpost = outpost,
					ProductGroup = productGroup,
					Product = productsCache[i],

					StockLevel= 10,
					UpdateMethod = "manual"
				};
				buildSession.Save(osl);
				oslCache.Add(osl);

			}
		}

		private void Create10Products(NHibernate.ISession buildSession)
		{
			for (int i = 0; i < 10; i++)
			{
				var product = new Product
				{
					ByUser = user,

					Name = "Product_"+i,
					Description = "Product_"+i+"_Description",
					LowerLimit = 1,
					UpperLimit = 10,
					SMSReferenceCode = "Z",
					ProductGroup = productGroup
				};
				buildSession.Save(product);
				productsCache.Add(product);
			}
		}
	}
}

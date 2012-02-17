using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using FluentNHibernate.Testing;
using Core.Domain;

namespace IntegrationTests
{
    [TestFixture]
    public class When_WeWantToPersiste_An_OutpostStockLevel : GivenAPersistenceSpecification<OutpostStockLevel>
    {
        Client client;
        User user;

        Outpost outpost;

        ProductGroup productGroup;
        Product product;
        [SetUp]
        public void Before()
        {
            StoreReferenceData();

        }

        private void StoreReferenceData()
        {
            user = new User{
                Email = "user@project.com",
                Password = "Darn",
                FirstName = "David",
                LastName = "Jones",
                UserName ="david.jones"
            };

            session.Save(user);

            client = new Client
            {
                ByUser = user,
                Name = "Univers"
            };

            session.Save(client);

            user.ClientId = client.Id;
            session.Update(user);

            outpost = new Outpost
            {
                ByUser = user,
                Client = client,
                Name = "Sri Lanka",
                DetailMethod = "0741 551 994"

            };
            session.Save(outpost);

            productGroup = new ProductGroup
            {
                ByUser = user,
                Name = "Malaria",
                ReferenceCode = "MAL",
                Description = "Contains all drugs within the Malaria Group"
            };

            session.Save(productGroup);

            product = new Product
            {
                ByUser = user,
                Name = "Malaria Cure",
                Description = "This is the universal cure for malaria",
                LowerLimit = 10,
                UpperLimit = 700,
                SMSReferenceCode = "Y",
                ProductGroup = productGroup

            };

            session.Save(product);

        }



        [TearDown]
        public void After()
        {
            var cleanup = _sessionFactory.CreateSession();

            cleanup.Delete(productGroup);
            cleanup.Delete(product);
            cleanup.Delete(outpost);

            cleanup.Delete(client);
            cleanup.Delete(user);
            cleanup.Flush();

            cleanup.Dispose();
        }


        [Test]
        public void OutpostStockLevel_is_Saved_and_Only_one_query_is_sent_to_the_DB_when_accesing_Product_ProductGroup_And_Outpost()
        {
            session.Flush();


            var outpostStockLevel = Specs.CheckProperty(p => p.StockLevel, 200)
                .CheckProperty(p => p.UpdateMethod, "SMS")
                .CheckReference(p=>p.Client, client)
                .CheckReference(p=>p.ByUser, user)
                .CheckReference(p => p.ProductGroup, productGroup)
                .CheckReference(p => p.Product, product)
                .CheckReference(p => p.Outpost, outpost)
                .VerifyTheMappings();

            session.Flush();
            session.Dispose();

            using (var request = _sessionFactory.CreateSession())
            {
                var checkSelectStatementForOutpostStockLvl = request.Load<OutpostStockLevel>(outpostStockLevel.Id);

                var productName = checkSelectStatementForOutpostStockLvl.Product.Name;
                var productGroupName = checkSelectStatementForOutpostStockLvl.ProductGroup.Name;
                var outpostName = checkSelectStatementForOutpostStockLvl.Outpost.Name;

                request.Delete(checkSelectStatementForOutpostStockLvl);
                request.Flush();
            }


        }
    }
}

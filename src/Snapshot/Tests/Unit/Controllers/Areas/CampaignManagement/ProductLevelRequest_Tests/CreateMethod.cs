﻿using System;
using System.Linq;
using NUnit.Framework;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Tests.Unit.Controllers.Areas.CampaignManagement.ProductLevelRequest_Tests
{

    [TestFixture]
    public class CreateMethod
    {
        ObjectMother _ = new ObjectMother();

        [SetUp]
        public void BeforeEach()
        {
            _.Init();
        }

        public void CreatesANewProductLevelRequest()
        {
            _.controller.Create(new CreateProductLevelRequestInput());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Controllers;
using System.Web.Mvc;
using Web.Models.UnauthorizedAccess;

namespace Tests.Unit.Controllers.Areas
{
    [TestFixture]
    public class UnautorizedAccessController_Tests
    {
        private string DEFAULT_VIEW_NAME = "";
        private UnauthorizedAccessController noAccessController;

        [Test]
        public void Get_Overview_Returns_UnauthorizedAccess_OverviewModel_with_Default_ViewName()
        {
            //Arange
            noAccessController = new UnauthorizedAccessController();

            //Act
            var result = noAccessController.Index();

            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;

            Assert.IsInstanceOf<Index>(viewResult.Model);
            Assert.AreEqual(DEFAULT_VIEW_NAME, viewResult.ViewName);
        }
    }
}

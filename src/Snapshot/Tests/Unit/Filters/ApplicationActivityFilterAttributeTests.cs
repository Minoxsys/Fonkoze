using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.CustomFilters;
using Web.Models.Shared;

namespace Tests.Unit.Filters
{
    [TestFixture]
    public class ApplicationActivityFilterAttributeTests
    {
        private ApplicationActivityFilterAttribute _sut;
        private Mock<ISaveOrUpdateCommand<ApplicationActivity>> _appActivitySaveCmd;

        [SetUp]
        public void PerTestSetup()
        {
            _appActivitySaveCmd = new Mock<ISaveOrUpdateCommand<ApplicationActivity>>();
            _sut = new ApplicationActivityFilterAttribute {ApplicationActivitySaveCommand = _appActivitySaveCmd.Object};
        }

        [Test]
        public void RecordsActivityWithDataFromActionResultModel_WhenActionMethodCodeExecutedSuccesfully()
        {
            var ctx = new ActionExecutedContext {Result = new JsonResult {Data = new JsonActionResponse {Status = "Success", Message = "a"}}};

            _sut.OnActionExecuted(ctx);

            _appActivitySaveCmd.Verify(c => c.Execute(It.Is<ApplicationActivity>(a => a.Message == "a")));
        }

        [Test]
        public void RecordsActivityWithDataFromActionResultModel_WhenActionMethodCodeDidNotExecuteProperly()
        {
            var ctx = new ActionExecutedContext { Result = new JsonResult { Data = new JsonActionResponse { Status = "Error"} } };

            _sut.OnActionExecuted(ctx);

            _appActivitySaveCmd.Verify(c => c.Execute(It.IsAny<ApplicationActivity>()), Times.Never());
        }

        [Test]
        public void DoesNotRecordsActivity_WhenActionResultIsNotAsExpected()
        {
            var ctx = new ActionExecutedContext { Result = new ViewResult () };

            _sut.OnActionExecuted(ctx);

            _appActivitySaveCmd.Verify(c => c.Execute(It.IsAny<ApplicationActivity>()), Times.Never());
        }

        [Test]
        public void DoesNotRecordsActivity_WhenActionResultModelIsNotAsExpected()
        {
            var ctx = new ActionExecutedContext { Result = new JsonResult { Data = new { } } };

            _sut.OnActionExecuted(ctx);

            _appActivitySaveCmd.Verify(c => c.Execute(It.IsAny<ApplicationActivity>()), Times.Never());
        }


    }
}

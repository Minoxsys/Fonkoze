using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NUnit.Framework;
using Rhino.Mocks;
using Web.BackgroundJobs.BackgroundJobsServices;
using Web.Services;

namespace Tests.Unit.BackgroundJobs
{
    [TestFixture]
    class CampaignJobExecutionTests
    {
        private CampaignJobExecutionService _sut;
        IQueryService<ProductLevelRequest> _queryProductLevelRequests;
        IQueryService<RequestRecord> _queryExistingRequests;
        IProductLevelRequestMessagesDispatcherService _dispatcherService;

        [SetUp]
        public void PerTestSetup()
        {
            _queryProductLevelRequests = MockRepository.GenerateMock<IQueryService<ProductLevelRequest>>();
            _queryExistingRequests = MockRepository.GenerateMock<IQueryService<RequestRecord>>();
            _dispatcherService = MockRepository.GenerateMock<IProductLevelRequestMessagesDispatcherService>();

            _sut = new CampaignJobExecutionService(_queryProductLevelRequests, _queryExistingRequests, _dispatcherService);
        }

        [Test]
        public void ExecuteJob_DispatchesMessagesForProductLevelRequest_WhenProductLevelRequestActiveNeverExecutedAndNeedsToBeExecuted()
        {
            // Arrange
            var productLevelRequest = CreateProductLevelRequestThatNeedsToBeExecuted();
            _queryProductLevelRequests.Stub(s => s.Query()).Return(new List<ProductLevelRequest> { productLevelRequest }.AsQueryable());
            _queryExistingRequests.Stub(s => s.Query()).Return(new List<RequestRecord>().AsQueryable());

            _dispatcherService.Expect(call => call.DispatchMessagesForProductLevelRequest(Arg<ProductLevelRequest>.Matches(p => p == productLevelRequest)));

            // Act
            _sut.ExecuteJob();

            // Assert
            _dispatcherService.VerifyAllExpectations();
        }

        [Test]
        public void ExecuteJob_DispatchesMessagesForProductLevelRequest_WhenProductLevelRequestActiveAndNeedsToBeExecuted()
        {
            // Arrange
            var productLevelRequest = CreateProductLevelRequestThatNeedsToBeExecuted();
            _queryProductLevelRequests.Stub(s => s.Query()).Return(new List<ProductLevelRequest> { productLevelRequest }.AsQueryable());
            _queryExistingRequests.Stub(s => s.Query()).Return(new List<RequestRecord> { CreateExistingRequest() }.AsQueryable());

            _dispatcherService.Expect(call => call.DispatchMessagesForProductLevelRequest(Arg<ProductLevelRequest>.Matches(p => p == productLevelRequest)));

            // Act
            _sut.ExecuteJob();

            // Assert
            _dispatcherService.VerifyAllExpectations();
        }

        [Test]
        public void ExecuteJob_DoesNothing_WhenNoActiveProductLevelRequests()
        {
            // Arrange
            _queryProductLevelRequests.Stub(s => s.Query()).Return(new List<ProductLevelRequest>().AsQueryable());
            _queryExistingRequests.Stub(s => s.Query()).Return(new List<RequestRecord> { CreateExistingRequest() }.AsQueryable());

            _dispatcherService.Expect(call => call.DispatchMessagesForProductLevelRequest(Arg<ProductLevelRequest>.Is.Anything)).Repeat.Never();

            // Act
            _sut.ExecuteJob();

            // Assert
            _dispatcherService.VerifyAllExpectations();
        }

        #region Helpers
        private RequestRecord CreateExistingRequest()
        {
            return new RequestRecord
            {
                Created = DateTime.Now.AddDays(-1)
            };
        }

        private ProductLevelRequest CreateProductLevelRequestThatNeedsToBeExecuted()
        {
            return new ProductLevelRequest
            {
                Created = DateTime.MinValue,
                Campaign = new Campaign { StartDate = DateTime.MinValue, Opened = true },
                IsStopped = false,
                Schedule = new Schedule { FrequencyValue = 1 },
            };
        }
        #endregion
    }
}

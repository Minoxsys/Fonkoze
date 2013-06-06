using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.BackgroundJobs;
using Web.BackgroundJobs.BackgroundJobsServices;

namespace Tests.Unit.BackgroundJobs
{
    [TestFixture]
    class EmptyJobExecutionTests
    {
        private EmptyJobExecutionService _sut;
        Mock<IQueryService<WorkItem>> _queryWorkItems;
        Mock<IDeleteCommand<WorkItem>> _deleteWorkItems;

        [SetUp]
        public void PerTestSetup()
        {
            _queryWorkItems = new Mock<IQueryService<WorkItem>>();
            _deleteWorkItems = new Mock<IDeleteCommand<WorkItem>>();

            _sut = new EmptyJobExecutionService(_queryWorkItems.Object, _deleteWorkItems.Object);
        }

        [Test]
        public void ExecuteJob_DeletesWorkItems_WhenWorkItemsHaveBeenCompletedMoreThan4HoursAgo()
        {
            // Arrange
            var workItem = CreateMoreThan4HoursCompletedWorkItem();
            _queryWorkItems.Setup(s => s.Query()).Returns(new List<WorkItem> { workItem }.AsQueryable());

            // Act
            _sut.ExecuteJob();

            // Assert
            _deleteWorkItems.Verify(s => s.Execute(It.Is<WorkItem>(w => w == workItem)));
        }

        [Test]
        public void ExecuteJob_DoesNothing_WhenWorkItemsHaveBeenCompletedLessThan4HoursAgo()
        {
            // Arrange
            _queryWorkItems.Setup(s => s.Query()).Returns(CreateLessThen4HoursCompletedWorkItemList().AsQueryable());

            // Act
            _sut.ExecuteJob();

            // Assert
            _deleteWorkItems.Verify(s => s.Execute(It.IsAny<WorkItem>()), Times.Never());
        }

        [Test]
        public void ExecuteJob_DoesNothing_WhenWorkItemsHaveNotBeenCompleted()
        {
            // Arrange
            _queryWorkItems.Setup(s => s.Query()).Returns(CreateNotCompletedWorkItemList().AsQueryable());

            // Act
            _sut.ExecuteJob();

            // Assert
            _deleteWorkItems.Verify(s => s.Execute(It.IsAny<WorkItem>()), Times.Never());
        }

        #region Helpers
        private WorkItem CreateMoreThan4HoursCompletedWorkItem()
        {
            return new WorkItem { Completed = DateTime.MinValue };
        }

        private IList<WorkItem> CreateLessThen4HoursCompletedWorkItemList()
        {
            return new List<WorkItem>
            { 
                new WorkItem { Completed = DateTime.Now }
            };
        }

        private IList<WorkItem> CreateNotCompletedWorkItemList()
        {
            return new List<WorkItem> { new WorkItem() };
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.OutpostControllerTests
{
	[TestFixture]
	public class DeleteMethod
	{
		readonly ObjectMother _ = new ObjectMother();
		[SetUp]
		public void BeforeEach()
		{
			_.Init();
		}

		[Test]
		public void Invokes_Load()
		{
			var outpostId = Guid.NewGuid();

			_.EnsureThatQueryLoadReturnsAValidDomainEntity(outpostId);
			// act
			_.controller.Delete(outpostId);

			_.VerifyThatLoadWasInvokedWithTheGiven(outpostId);

		}

		[Test]
		public void Invokes_DeleteCommand()
		{
			var outpostId = Guid.NewGuid();

			_.EnsureThatQueryLoadReturnsAValidDomainEntity(outpostId);
			// act
			_.controller.Delete(outpostId);

			_.VerifyThatDelectCommandWasExecutedWith(outpostId);

		}

	}
}

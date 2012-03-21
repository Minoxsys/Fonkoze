using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.OutpostManagement.ContactControllerTests
{
    [TestFixture]
    public class IndexMethod
    {
        readonly ObjectMother _ = new ObjectMother();
        [SetUp]
        public void BeforeEach()
        {
            _.Init();
            _.StubUserAndItsClient();
        }

        [Test]
        public void LoadsUserAndClient()
        {
            var indexModel = _.GetIndexMethodInput();

            _.controller.Index(indexModel);

            _.VerifyUserAndClientExpectations();
        }

        [Test]
        public void LoadsOutpost()
        {
            var indexModel = _.GetIndexMethodInput();

            _.controller.Index(indexModel);

            _.VerifyOutpostLoaded();
        }

        [Test]
        public void QueriesContacts()
        {
            var indexModel = _.GetIndexMethodInput();

            _.controller.Index(indexModel);

            _.VerifyContactsQueried();
        }

        [Test]
        public void SavesANewContactOnPost()
        {
            var postModel = _.GetPostModel();
            _.StubLoadOutpost();

            _.controller.Index(postModel);

            _.VerifySaveCommandCalledWithDataFrom(postModel);
        }
        [Test]
        public void PutUpdates()
        {
            var putModel = _.GetPutModel();
            _.StubLoadOutpost(putModel.ContactDetail);
            _.StubLoadContact(putModel);

            _.controller.Index(putModel);

            _.VerifySaveCommandCalledWithDataFrom(putModel);
            _.VerifyUpdateOutpostCommandCalledWithDataFrom(putModel);
        }

        [Test]
        public void Put_CreatesNewContact_WhenIdIsEmpty_AndContactLoadReturnsNull()
        {
            var putModel = _.GetPutModel_WithEmptyId();
            _.StubLoadOutpost(putModel.ContactDetail);
          
            _.controller.Index(putModel);

            _.VerifySaveCommandCalledWithDataFrom(putModel);
            _.VerifyUpdateOutpostCommandCalledWithDataFrom(putModel);
 
        }
        [Test]
        public void RemoveContactOnDelete()
        {
            var deleteModel = _.GetDeleteModel();
            _.StubLoadContact(deleteModel);

            _.controller.Index(deleteModel);

            _.VerifyDeleteCommandExecuted();
        }
    }
}

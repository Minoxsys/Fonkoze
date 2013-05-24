using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Services;
using Web.Models.Parsing.Outpost;
using Web.Security;

namespace Tests.Unit.Services.OutpostsService
{
    [TestFixture]
    public class ContactsUpdateServiceTests
    {
        private IContactsUpdateService _sut;
        private IParsedOutpost _parsedOutpost;
        private Mock<IQueryService<Contact>> _queryContactService;
        private Mock<ISaveOrUpdateCommand<Contact>> _saveOrUpdateCommandContactstMock;
        private Mock<Outpost> _outpostMock;

        private Mock<UserAndClientIdentity> _loggedUserMock;

        [SetUp]
        public void PerTestSetup()
        {
            _queryContactService = new Mock<IQueryService<Contact>>();
            _saveOrUpdateCommandContactstMock = new Mock<ISaveOrUpdateCommand<Contact>>();

            _sut = new ContactsUpdateService(_queryContactService.Object, _saveOrUpdateCommandContactstMock.Object);
            _parsedOutpost = CreateParsedOutpost();
            _outpostMock = new Mock<Outpost>();
            _loggedUserMock = new Mock<UserAndClientIdentity>();
        }

        [Test]
        public void ManageOutpostsContact_AddsTheContact_WhenTheContactIsNotAlreadyExistent()
        {
            _outpostMock.Setup(s => s.Contacts).Returns(new List<Contact> { new Contact { ContactDetail = "566897813156" } });

            _queryContactService.Setup(s => s.Query()).Returns((new List<Contact> { new Contact { ContactDetail = _parsedOutpost.ContactDetail } }).AsQueryable());

            _sut.ManageOutpostContact(_loggedUserMock.Object, _outpostMock.Object, _parsedOutpost);

            _saveOrUpdateCommandContactstMock.Verify(
                cmd => cmd.Execute(It.Is<Contact>(ctact => ctact.ByUser == _loggedUserMock.Object.User && ctact.Client == _loggedUserMock.Object.Client &&
                   ctact.ContactDetail == _parsedOutpost.ContactDetail)));
        }

        [Test]
        public void ManageOutpostsContact_DoesNotAddTheContact_WhenTheContactIsAlreadyExistent()
        {
            var contact = new Contact { ContactDetail = _parsedOutpost.ContactDetail };

            _outpostMock.Setup(s => s.Contacts).Returns(new List<Contact> { contact });

            _sut.ManageOutpostContact(_loggedUserMock.Object, _outpostMock.Object, _parsedOutpost);

            _saveOrUpdateCommandContactstMock.Verify(
                cmd => cmd.Execute(It.IsAny<Contact>()), Times.Never());
        }

        private ParsedOutpost CreateParsedOutpost()
        {
            return new ParsedOutpost { Country = "Country1", Region = "Region1", District = "District1", Name = "Name1", Longitude = "1.1", Latitude = "0.1", ContactDetail = "898989898" };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Moq;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Services;
using Web.Models.Parsing.Outpost;
using Web.Security;

namespace Tests.Unit.Services.OutpostsAddCSVFileParserService
{
    [TestFixture]
    public class OutpostsUpdateServiceTests
    {
        private IOutpostsUpdateService _sut;
        private IParsedOutpost _parsedOutpost;

        private Mock<IQueryService<Outpost>> _queryServiceMock;
        private Mock<IQueryService<Country>> _queryCountryMock;
        private Mock<IQueryService<Region>> _queryRegionMock;
        private Mock<IQueryService<District>> _queryDistrictMock;
        private Mock<ISaveOrUpdateCommand<Outpost>> _saveOrUpdateCommandOutpostMock;
        private Mock<IQueryService<Contact>> _queryContactMock;

        private Mock<IContactsUpdateService> _contactsUpdateService;
        private Mock<UserAndClientIdentity> _loggedUserMock;

        [SetUp]
        public void PerTestSetup()
        {
            _parsedOutpost = CreateParsedOutpost("Haiti", "TestRegion", "TestDistrict", "outpostUnitTest", "3.2345", "1.4321", "800800200");

            _queryServiceMock = new Mock<IQueryService<Outpost>>();
            _queryCountryMock = new Mock<IQueryService<Country>>();
            _queryRegionMock = new Mock<IQueryService<Region>>();
            _queryDistrictMock = new Mock<IQueryService<District>>();
            _queryContactMock = new Mock<IQueryService<Contact>>();
            _saveOrUpdateCommandOutpostMock = new Mock<ISaveOrUpdateCommand<Outpost>>();

            _contactsUpdateService = new Mock<IContactsUpdateService>();
            _loggedUserMock = new Mock<UserAndClientIdentity>();

            _sut = new OutpostsUpdateService(_contactsUpdateService.Object, _queryServiceMock.Object, _queryCountryMock.Object, _queryRegionMock.Object,
                _queryDistrictMock.Object, _saveOrUpdateCommandOutpostMock.Object, _queryContactMock.Object);
        }

        [Test]
        public void ManageParseOutposts_AddsTheOutpost_WhenTheOutpostIsNotExistent()
        {
            DoQueryMocksSetup();

            _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult { Success = true, ParsedOutposts = new List<IParsedOutpost> { _parsedOutpost } });

            _saveOrUpdateCommandOutpostMock.Verify(
                cmd => cmd.Execute(It.Is<Outpost>(outpost => outpost.ByUser == _loggedUserMock.Object.User && outpost.Client == _loggedUserMock.Object.Client &&
                    outpost.Country.Name == _parsedOutpost.Country && outpost.Region.Name == _parsedOutpost.Region && outpost.District.Name == _parsedOutpost.District &&
                    outpost.DetailMethod == _parsedOutpost.ContactDetail && outpost.Latitude == "(1.4321,3.2345)" && outpost.Name == _parsedOutpost.Name)));
        }

        [Test]
        public void ManageParseOutposts_UpdatesTheOutpost_WhenTheOutpostIsExistent()
        {
            _queryServiceMock.Setup(s => s.Query()).Returns((new List<Outpost> { new Outpost 
                { 
                    Name = _parsedOutpost.Name, 
                    District = new District { Name = _parsedOutpost.District } } 
                }).AsQueryable());

            DoQueryMocksSetup();

            _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult { Success = true, ParsedOutposts = new List<IParsedOutpost> { _parsedOutpost } });

            _saveOrUpdateCommandOutpostMock.Verify(
                cmd => cmd.Execute(It.Is<Outpost>(outpost => outpost.ByUser == _loggedUserMock.Object.User && outpost.Client == _loggedUserMock.Object.Client &&
                    outpost.Country.Name == _parsedOutpost.Country && outpost.Region.Name == _parsedOutpost.Region && outpost.District.Name == _parsedOutpost.District &&
                    outpost.DetailMethod == _parsedOutpost.ContactDetail && outpost.Latitude == "(1.4321,3.2345)" && outpost.Name == _parsedOutpost.Name)));
        }

        [Test]
        public void ManageParseOutposts_UpdatesTheOutpost_WhenInputCoordonatesAreExistentAndBelongToTheInputOutpost()
        {
            _queryServiceMock.Setup(s => s.Query()).Returns((new List<Outpost> { new Outpost 
                { 
                    Name = _parsedOutpost.Name, 
                    Longitude = _parsedOutpost.Longitude,
                    Latitude = _parsedOutpost.Latitude,
                    District = new District { Name = _parsedOutpost.District } } 
                }).AsQueryable());

            DoQueryMocksSetup();

            _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult { Success = true, ParsedOutposts = new List<IParsedOutpost> { _parsedOutpost } });

            _saveOrUpdateCommandOutpostMock.Verify(
                cmd => cmd.Execute(It.Is<Outpost>(outpost => outpost.ByUser == _loggedUserMock.Object.User && outpost.Client == _loggedUserMock.Object.Client &&
                    outpost.Country.Name == _parsedOutpost.Country && outpost.Region.Name == _parsedOutpost.Region && outpost.District.Name == _parsedOutpost.District &&
                    outpost.DetailMethod == _parsedOutpost.ContactDetail && outpost.Latitude == "(1.4321,3.2345)" && outpost.Name == _parsedOutpost.Name)));
        }

        [Test]
        public void ManageParseOutposts_DoesNotUpdateTheOutpost_WhenInputCoordonatesAreAlreadyExistent()
        {
            _queryServiceMock.Setup(s => s.Query()).Returns((new List<Outpost> { new Outpost 
                { 
                    Name = "randomName", 
                    Latitude = "(3.2345,1.4321)",
                    District = new District { Name = _parsedOutpost.District } } 
                }).AsQueryable());

            DoQueryMocksSetup();

            _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult { Success = true, ParsedOutposts = new List<IParsedOutpost> { _parsedOutpost } });

            _saveOrUpdateCommandOutpostMock.Verify(
                cmd => cmd.Execute(It.IsAny<Outpost>()), Times.Never());
        }

        [Test]
        public void ManageparseOutposts_DoesNotAddOrUpdateTheOutpost_WhenOneOfTheFieldsAreEmpty()
        {
            DoQueryMocksSetup();

            _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult { Success = true, ParsedOutposts = new List<IParsedOutpost> { CreateEmptyParsedOutpost() } });

            _saveOrUpdateCommandOutpostMock.Verify(
                 cmd => cmd.Execute(It.IsAny<Outpost>()), Times.Never());
        }

        [Test]
        public void ManageParseOutposts_DoesNotAddOrUpdateTheOutpost_WhenFieldNotEmptyButCountryRegionOrDistrictAreNonExistent()
        {
            _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult { Success = true, ParsedOutposts = new List<IParsedOutpost> { _parsedOutpost } });

            _saveOrUpdateCommandOutpostMock.Verify(
                 cmd => cmd.Execute(It.IsAny<Outpost>()), Times.Never());
        }

        [Test]
        public void ManageParseOutposts_ReturnsSuccessAndNoFailedProducts_WhenOutpostsAreValid()
        {
            var parsedOutpost2 = CreateParsedOutpost("Haiti", "TestRegion", "TestDistrict", "outpostUnitTest22", "3.23452", "", "8435008030200");

            DoQueryMocksSetup();

            var result = _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult
            {
                Success = true,
                ParsedOutposts = new List<IParsedOutpost>
                {
                    _parsedOutpost, 
                    parsedOutpost2
                }
            });

            Assert.IsTrue(result.Success);
            Assert.IsNull(result.FailedOutposts);
        }

        [Test]
        public void ManageParseOutposts_ReturnsSuccessFalseAndFailedProducts_WhenOutpostsAreNotValid()
        {
            var parsedOutpost = CreateParsedOutpost("Haiti", "TestRegion", "", "outpostUnitTest11", "3.2345998", "1.43213321", "820208030200");
            var parsedOutpost2 = CreateParsedOutpost("", "TestRegion", "TestDistrict", "", "3.23452", "", "8002803440200");

            DoQueryMocksSetup();

            var result = _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult
            {
                Success = true,
                ParsedOutposts = new List<IParsedOutpost>
                {
                    parsedOutpost,
                    parsedOutpost2
                }
            });

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.FailedOutposts);
        }

        [Test]
        public void ManageParseOutposts_ReturnsSuccessFalseAndFailedProducts_WhenContactDetailsAlreadyExist()
        {
            var parsedOutpost = CreateParsedOutpost("Haiti", "TestRegion", "TestDistrict", "outpostUnitTest11", "3.2345998", "1.43213321", "800");
            var parsedOutpost2 = CreateParsedOutpost("Haiti", "TestRegion", "TestDistrict", "outpostUnitTest113", "3.23452", "5.43213321", "800");

            DoQueryMocksSetup();

            _queryContactMock.Setup(s => s.Query()).Returns(new List<Contact> { new Contact { ContactDetail = parsedOutpost.ContactDetail, IsMainContact = true } }.AsQueryable());

            var result = _sut.ManageParseOutposts(_loggedUserMock.Object, new OutpostsParseResult
            {
                Success = true,
                ParsedOutposts = new List<IParsedOutpost>
                {
                    parsedOutpost,
                    parsedOutpost2
                }
            });

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.FailedOutposts);
        }


        private void DoQueryMocksSetup()
        {
            var country = new Country { Name = _parsedOutpost.Country };
            var region = new Region { Name = _parsedOutpost.Region, Country = country };

            _queryCountryMock.Setup(s => s.Query()).Returns((new List<Country> { country }).AsQueryable());
            _queryRegionMock.Setup(s => s.Query()).Returns((new List<Region> { region }).AsQueryable());
            _queryDistrictMock.Setup(s => s.Query()).Returns((new List<District> { new District { Name = _parsedOutpost.District, Region=region } }).AsQueryable());
        }

        private ParsedOutpost CreateParsedOutpost(string country, string region, string district, string name, string longitude, string latitude, string phoneNo)
        {
            return new ParsedOutpost { Country = country, Region = region, District = district, Name = name, Longitude = longitude, Latitude = latitude, ContactDetail = phoneNo };
        }

        private ParsedOutpost CreateEmptyParsedOutpost()
        {
            return new ParsedOutpost { Country = "", Region = "", District = "", Name = "", Longitude = "", Latitude = "", ContactDetail = "" };
        }
    }
}

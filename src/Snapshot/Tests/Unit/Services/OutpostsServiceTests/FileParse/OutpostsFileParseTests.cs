using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.OutpostManagement.Models.Outpost;
using Web.Areas.OutpostManagement.Services;
using Web.Models.Parsing.Outpost;

namespace Tests.Unit.Services.OutpostAddCSVFileParserService
{
    [TestFixture]
    class OutpostsAddCsvFileParserTests
    {
        private IOutpostsFileParseService _sut;

        [SetUp]
        public void PerTestSetup()
        {
            _sut = new OutpostsFileParseService();
        }

        [Test]
        public void ParseStream_ReturnsTheListOfParsedObjects_WhenParsingWasSuccessful()
        {
            //arrange
            OutpostsParseResult result;
            var list = CreateListOfOutposts();
            var parsedOutposts = list as IList<IParsedOutpost> ?? list.ToList();

            using (var stream = CreateStreamWithValidValues(parsedOutposts))
            {
                stream.Seek(0, SeekOrigin.Begin);

                //act
                result = _sut.ParseStream(stream);
            }

            //assert
            Assert.That(result.Success, Is.EqualTo(true));
            CollectionAssert.AreEquivalent(parsedOutposts, result.ParsedOutposts);
        }

        [Test]
        public void ParseStream_ReturnAnEmptyList_WhenParsingWasSuccessfullButThereWereNoItems()
        {
            //arrange
            OutpostsParseResult result;
            var parsedOutposts = new List<IParsedOutpost>();

            using (var stream = CreateStreamWithValidValues(parsedOutposts))
            {
                stream.Seek(0, SeekOrigin.Begin);

                result = _sut.ParseStream(stream);
            }

            Assert.That(result.Success, Is.EqualTo(true));
            Assert.That(result.ParsedOutposts.Count, Is.EqualTo(0));
        }
        [Test]
        public void ParseStream_ReturnsFailedParse_WhenTheContentsOfTheFileAreTrashed()
        {
            //arrange
            OutpostsParseResult result;

            using (var stream = CreateStreamWithTrashContent())
            {
                stream.Seek(0, SeekOrigin.Begin);

                //act
                result = _sut.ParseStream(stream);
            }

            //assert
            Assert.That(result.Success, Is.EqualTo(false));
        }

        private Stream CreateStreamWithTrashContent()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            for (int i = 0; i < 20; i++)
            {
                if (i % 2 == 0)
                {
                    writer.WriteLine("hdjdjdj  dsa {0}, ..f.f..df.d.fd.f.dfs.fdsfd.sfsd.{1}", i, i + 1);
                }
                else
                {
                    writer.WriteLine("1 1");
                }
            }
            writer.Flush();
            return stream;
        }

        private Stream CreateStreamWithValidValues(IEnumerable<IParsedOutpost> parsedOutposts)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            foreach (var outpost in parsedOutposts)
            {
                writer.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", outpost.Country, outpost.Region, outpost.District, outpost.Name, outpost.Longitude, outpost.Latitude, outpost.ContactDetail);
            }

            writer.Flush();
            return stream;
        }

        private IEnumerable<IParsedOutpost> CreateListOfOutposts()
        {
            var list = new List<IParsedOutpost>();
            for (int i = 'A'; i < 'Z'; i++)
            {
                var stringToAdd = i.ToString(CultureInfo.InvariantCulture);
                list.Add(new ParsedOutpost { Country=stringToAdd, Region = stringToAdd, District = stringToAdd, Name = stringToAdd, Longitude = stringToAdd, Latitude = stringToAdd, ContactDetail = stringToAdd });
            }
            return list;
        }

    }
}

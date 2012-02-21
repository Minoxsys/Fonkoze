using System;
using NUnit.Framework;
using Web.Helpers;

namespace Tests.Unit.Helpers
{
    [TestFixture]
    public class DateFormatter_Tests
    {
        private static DateTime _date = new DateTime(2011,1,1);
        private const string _dateString = "01-Jan-2011";
        private static DateTime? _nullDate = null;

        [Test]
        public void DateToShortString_ShouldReturn_TheStringRepresentation_OfTheDate()
        {
            //Act
            string result = DateFormatter.DateToShortString(_date);


            //Assert
            Assert.AreEqual(_dateString, result);
        }

        [Test]
        public void DateToShortString_With_NullDate_ShouldReturn_AnEmpty_String()
        {
            //Act
            string result = DateFormatter.DateToShortString(_nullDate);


            //Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void StringToDate_Should_Return_TheDateValue_Of_The_String()
        {
            //Act
            DateTime result = DateFormatter.StringToDate(_dateString);


            //Assert
            Assert.AreEqual(_date, result);
        }

        [Test]
        public void StringToDate_WithAnEmptyString_Should_Return_TheDateValue_Of_Today()
        {
            //Act
            DateTime result = DateFormatter.StringToDate(string.Empty);


            //Assert
            Assert.AreEqual(DateTime.Today, result);
        }

        [Test]
        public void StringToDate_WithInvalidString_ShouldReturn_DateTimeToday()
        {
            //Act
            DateTime result = DateFormatter.StringToDate("1");

            //Assert
            Assert.AreEqual(DateTime.Today, result);
        }


        [Test]
        public void StringToDate_WithFlag_WithAnEmptyString_Should_Return_Null()
        {
            //Act
            DateTime? result = DateFormatter.StringToDate(string.Empty, true);


            //Assert
            Assert.AreEqual(null, result);
        }


        [Test]
        public void StringToDate_WithFlag_Should_Return_TheDateValue_Of_The_String()
        {
            //Act
            DateTime? result = DateFormatter.StringToDate(_dateString, true);


            //Assert
            Assert.AreEqual(_date, result);
        }

        [Test]
        public void StringToDate_WithInvalidString_ShouldReturn_NULL()
        {
            //Act
            DateTime? result = DateFormatter.StringToDate("1", true);

            //Assert
            Assert.AreEqual(null, result);
        }
    }
}

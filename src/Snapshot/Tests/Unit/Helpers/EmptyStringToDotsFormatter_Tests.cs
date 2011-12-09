using NUnit.Framework;
using Web.Helpers;

namespace Tests.Unit.Helpers
{
    [TestFixture]
    public class EmptyStringToDotsFormatter_Tests
    {
        private static string _placeholder = StringPlaceholder.Placeholder;

        private const string STRING = "random";
        private const int INT = 1;

        private class StringTestClass
        {
            public string Dots1 { get; set; }
            public string Dots2 { get; set; }
            public string Empty1 { get; set; }
            public string Empty2 { get; set; }
            public string String { get; set; }
            public int Int { get; set; }
            public int? NullInt = null;
            public string EmptyField = string.Empty;

            public StringTestClass()
            {
                Dots1 = _placeholder;
                Dots2 = _placeholder;
                Empty1 = string.Empty;
                Empty2 = string.Empty;
                String = STRING;
                Int = INT;
            }
        }

        [Test]
        public void It_Should_Change_All_The_String_Properties_Of_A_Class_That_Are_Empty_To_Dots()
        {
            //Arrange 
            var testClass = new StringTestClass();


            //Act
            EmptyStringToDotsFormatter.ConvertModelForView(testClass);


            //Assert
            Assert.AreEqual(_placeholder, testClass.Empty1);
            Assert.AreEqual(_placeholder, testClass.Empty2);
            Assert.AreNotEqual(_placeholder, testClass.NullInt);
            Assert.AreNotEqual(_placeholder, testClass.Int);
            Assert.AreNotEqual(_placeholder, testClass.String);
            Assert.AreEqual(_placeholder, testClass.Dots1);
            Assert.AreEqual(_placeholder, testClass.Dots2);
            Assert.AreEqual(null, testClass.NullInt);
            Assert.AreEqual(STRING, testClass.String);
            Assert.AreEqual(string.Empty, testClass.EmptyField); //fields are not changed to dots, only properties
        }

    }
}

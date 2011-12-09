using NUnit.Framework;
using Web.Helpers;

namespace Tests.Unit.Helpers
{
    [TestFixture]
    public class NullableDoubleFormatter_Tests
    {
        private static double? _nullDouble = null;

        private static double? _notNullDouble = 1.2;

        private static string _stringInt = "1.2";

        [Test]
        public void It_Should_Convert_A_Null_Double_To_An_Empty_String()
        {
            //Act
            string result = NullableDoubleFormatter.DoubleToString(_nullDouble);

            //Assert
            Assert.AreEqual(string.Empty, result);

        }


        [Test]
        public void It_Should_Convert_A_NonNull_Double_To_The_String_With_Its_Value()
        {
            //Act
            string result = NullableDoubleFormatter.DoubleToString(_notNullDouble);


            //Assert
            Assert.AreEqual(_notNullDouble.ToString(), result);

        }

        [Test]
        public void It_Should_Convert_An_Empty_String_To_A_Null_Double()
        {

            //Act
            double? result = NullableDoubleFormatter.StringToDouble(string.Empty);

            //Assert
            Assert.AreEqual(null, result);

        }

        [Test]
        public void It_Should_Convert_A_String_Number_To_The_Double_With_Its_Value()
        {
            //Act
            double? result = NullableDoubleFormatter.StringToDouble(_stringInt);

            //Assert
            Assert.AreEqual(double.Parse(_stringInt), result);

        }

        [Test]
        public void It_Should_Return_Null_If_String_Cannot_be_Converted()
        {
            //Act
            double? result = NullableDoubleFormatter.StringToDouble("-");

            //Assert
            Assert.IsNull(result);
        }
    }
}

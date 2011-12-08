using NUnit.Framework;
using Web.Helpers;

namespace Tests.Unit.Helpers
{
    [TestFixture]
    public class NullableIntFormatter_Tests
    {
        private static int? _nullInt = null;

        private static int? _notNullInt = 1;

        private static string _stringInt = "1";

        [Test]
        public void It_Should_Convert_A_Null_Int_To_An_Empty_String()
        {
            //Act
            string result = NullableIntFormatter.IntToString(_nullInt);

            //Assert
            Assert.AreEqual(string.Empty, result);

        }


        [Test]
        public void It_Should_Convert_A_NonNull_Int_To_The_String_With_Its_Value()
        {
            //Act
            string result = NullableIntFormatter.IntToString(_notNullInt);


            //Assert
            Assert.AreEqual(_notNullInt.ToString(), result);

        }

        [Test]
        public void It_Should_Convert_An_Empty_String_To_A_Null_Int()
        {

            //Act
            int? result = NullableIntFormatter.StringToInt(string.Empty);

            //Assert
            Assert.AreEqual(null, result);

        }

        [Test]
        public void It_Should_Convert_A_String_Number_To_The_Int_With_Its_Value()
        {
            //Act
            int? result = NullableIntFormatter.StringToInt(_stringInt);

            //Assert
            Assert.AreEqual(int.Parse(_stringInt), result);

        }

        [Test]
        public void It_Should_Return_Null_If_String_Cannot_be_Converted()
        {
            //Act
            int? result = NullableIntFormatter.StringToInt("-");

            //Assert
            Assert.IsNull(result);

        }
    }
}

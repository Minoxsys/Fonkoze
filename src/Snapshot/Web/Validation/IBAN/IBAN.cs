using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Validation.IBAN
{

    public class IBAN : ValidationAttribute
    {


        public IBAN()
            : base("IBAN")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }

            var iban = value as string;

            if (!IsAValidIBAN_ForRomania(iban) && !IsAValidIBAN_ForUK(iban))
                return new ValidationResult("The IBAN is not valid for Romania or UK!");

            return ValidationResult.Success;     
        }        

        public bool IsAValidIBAN_ForRomania(string iban)
        {
            iban = iban.Replace(" ", "");

            if (iban.Length != 24)
                return false;
            if (GetModulusOfIBAN(iban, 97) != 1)
                return false;
            return true;
        }

        public bool IsAValidIBAN_ForUK(string iban)
        {
            iban = iban.Replace(" ", "");

            if (iban.Length != 22)
                return false;
            if (GetModulusOfIBAN(iban, 97) != 1)
                return false;
            return true;
        }

        private int GetModulusOfIBAN(string iban, int modulus)
        {
            string transformedIBAN = TransformAndRearrangeIBAN(iban);

            return GetModulusOfLargeNumber(transformedIBAN, modulus);
        }

        private string TransformAndRearrangeIBAN(string iban)
        {
            string rearrangedIban = iban.Substring(4, iban.Length - 4) + iban.Substring(0, 4);

            rearrangedIban = rearrangedIban.ToUpper();

            for (int i = 10; i < 36; i++)
            {
                rearrangedIban = rearrangedIban.Replace(Char.ConvertFromUtf32(55 + i), i.ToString());
            }

            char[] arr = rearrangedIban.ToCharArray();
            Array.Reverse(arr);
            string reversedIban = new string(arr);

            return reversedIban;

        }

        private int GetModulusOfLargeNumber(string largeNumber, int modulo)
        {
            int result = 0;
            int lastRowValue = 1;

            for (int i = 0; i < largeNumber.Length; i++)
            {
                result += lastRowValue * int.Parse(largeNumber[i].ToString());
                lastRowValue = (lastRowValue * 10) % modulo;

            }

            return result = result % modulo;
        }
    }
}
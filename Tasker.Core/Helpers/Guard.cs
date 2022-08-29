using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tasker.Core.Helpers
{
    public class Guard
    {
        public static void AgainstNull<T>(T param) where T : class
        { 
            if(param == null)
                throw new ArgumentNullException();
        }

        public static void AgainstEmptyOrWhiteSpace(string s)
        {
            AgainstNull(s);
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException();
        }

        public static void AgainstInvalidEmail(string email)
        {
            AgainstNull(email);
            if
            (
                !Regex.IsMatch
                (
                    email
                    , @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b"
                    , RegexOptions.IgnoreCase
                    )
            )
                throw new ArgumentException();
        }

        public static void AgainstNegative(double number)
        {
            if (number < 0)
                throw new ArgumentException();
        }

        public static void AgainstDefault(double number)
        {
            if (number == default(double))
                throw new ArgumentException();
        }

        public static void AgainstDefault<T>(T number)
        {
            if (number.Equals(default(T)))
                throw new ArgumentException();
        }

        public static void AgainstEmpty<T>(IEnumerable<T> collection)
        {
            if (collection.Count() == 0)
                throw new ArgumentException();
        }

        //public static void AgainstNegative(decimal number)
        //{
        //    if (number == 0m)
        //        throw new ArgumentException();
        //}
    }
}

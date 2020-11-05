using System;

namespace Dumplingram.API.Helpers
{
    public static class Extensions
    {
        public static int CalculateAge(this DateTime date)
        {
            var age = DateTime.Today.Year - date.Year;
            if (date.AddYears(age) > DateTime.Today)
                age--;
            
            return age;
        }
    }
}
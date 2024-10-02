using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        //  public static int CalculateAge(this DateOnly dob)
        //  {
        //     var today = DateOnly.FromDateTime(DateTime.UtcNow);
        //     var age = today.Year - dob.Year; //checking user age
        //     if(dob > today.AddYears(-age)) age--; //this will give us someone age
        //     return age;
        // }


            public static int CalculateAge(this DateTime dob)
            {
            var today = DateTime.Today;
            var age = today.Year - dob.Year; //checking user age
            if(dob.Date > today.AddYears(-age)) age--; //this will give us someone age
            return age;
           }
        
    }
}
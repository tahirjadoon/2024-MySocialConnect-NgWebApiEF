using System;

namespace MSC.Core.Extensions;

public static class DateTimeExtensions
{

    public static int CalculateAge(this DateOnly dob)
    {
        //todays date
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        //calculate the age
        var age = today.Year - dob.Year;

        //go back to the year in which the person was in case of a leap year
        if(dob > today.AddYears(-age))
            age--;
            
        return age;
    }
}

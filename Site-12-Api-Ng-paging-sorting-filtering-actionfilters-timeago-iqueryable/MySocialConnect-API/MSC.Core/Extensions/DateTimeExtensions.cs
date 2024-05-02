using System;
using MSC.Core.Constants;

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

    /// <summary>
    /// The oldest the person can be
    /// </summary>
    /// <param name="maxAge">where dob >= this date</param>
    /// <returns></returns>
    public static DateOnly CalculateMinDob(this int maxAge)
    {
        if (maxAge <= 0) maxAge = DataConstants.MaxAge;
        var dob = DateOnly.FromDateTime(DateTime.Today.AddYears(-maxAge - 1));
        return dob;
    }

    /// <summary>
    /// The youngest the person can be
    /// </summary>
    /// <param name="minAge">where dob <= this date</param>
    /// <returns></returns>
    public static DateOnly CalculateMaxDob(this int minAge)
    {
        if (minAge <= 0) minAge = DataConstants.MinAge;
        var dob = DateOnly.FromDateTime(DateTime.Today.AddYears(-minAge));
        return dob;
    }
}

namespace CoursesAPI.Services.Utilities
{
	public class DateTimeUtils
	{
        /// <summary>
        /// Function that tells if a year is a leap year or not.
        /// If the function returns true than the year is leap year
        /// but false otherwise.
        /// </summary>
        /// <param name="year">The year</param>
        /// <returns>True if the year is leap year but false otherwise</returns>
		public static bool IsLeapYear(int year)
		{
            return ((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0);
		}
	}
}

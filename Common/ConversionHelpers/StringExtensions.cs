namespace FreshBake.API.Common.ConversionHelpers
{
    public class StringExtensions
    {
        public static int ToInt32 ( object value )
        {
            if (value == null)
                return 0;

            string valueString = value.ToString();

            if (string.IsNullOrWhiteSpace(valueString))
                return 0;

            return int.TryParse(valueString, out int result) ? result : 0;
        }
    }
}

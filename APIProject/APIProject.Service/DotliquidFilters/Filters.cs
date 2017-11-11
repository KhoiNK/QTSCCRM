using System;

namespace APIProject.Service.DotliquidFilters
{
    public static class CustomFilters
    {
        public static string Money(int? input)
        {
            if (input == null)
            {
                return null;
            }
            return $"{input.Value:C}".Replace(".00","").Replace("$","");
        }
    }
}
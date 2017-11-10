using System;

namespace APIProject.Service.DotliquidFilters
{
    public static class CustomFilters
    {
        public static string Money(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }
            return $"Order Total: {input:C}";
        }
    }
}
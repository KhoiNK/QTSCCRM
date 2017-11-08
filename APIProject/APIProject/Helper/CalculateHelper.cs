using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProject.Helper
{
    public class CalculateHelper
    {
        public int Sum(params int[] customerssalary)
        {
            int result = 0;

            for (int i = 0; i < customerssalary.Length; i++)
            {
                result += customerssalary[i];
            }

            return result;
        }
        public decimal Average(params int[] customerssalary)
        {
            int sum = Sum(customerssalary);
            decimal result = (decimal)sum / customerssalary.Length;
            return result;
        }
    }
}
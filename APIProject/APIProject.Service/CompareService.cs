using APIProject.Data.Repositories;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface ICompareService
    {
        IEnumerable<Customer> GetSimilarCustomers(Customer customer);
        double StringCompare(string a, string b);
    }
    public class CompareService:ICompareService
    {
        private readonly ICustomerRepository _customerRepository;
        public CompareService(ICustomerRepository _customerRepository)
        {
            this._customerRepository = _customerRepository;
        }
        public double StringCompare(string a, string b)
        {
            if (a == b) //Same string, no iteration needed.
                return 100;
            if ((a.Length == 0) || (b.Length == 0)) //One is empty, second is not
            {
                return 0;
            }
            double maxLen = a.Length > b.Length ? a.Length : b.Length;
            int minLen = a.Length < b.Length ? a.Length : b.Length;
            int sameCharAtIndex = 0;
            for (int i = 0; i < minLen; i++) //Compare char by char
            {
                if (a[i] == b[i])
                {
                    sameCharAtIndex++;
                }
            }
            return sameCharAtIndex / maxLen * 100;
        }

        public IEnumerable<Customer> GetSimilarCustomers(Customer customer)
        {
            int percentage = 70;
            var customerEntities = _customerRepository.GetAll().Where(c => c.IsDelete == false
            && StringCompare(customer.Name,c.Name)>=percentage);
            return customerEntities;
        }
    }
}

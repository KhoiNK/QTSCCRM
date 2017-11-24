using APIProject.Data.Repositories;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface ICompareService
    {
        IEnumerable<Customer> GetSimilarCustomers(Customer customer);
    }

    public class CompareService : ICompareService
    {
        private readonly ICustomerRepository _customerRepository;

        private readonly string[] irrelevantWords =
        {
            "CÔNG TY", "CONG TY", "TNHH", "TRÁCH NHIỆM", "HỮU HẠN", "CTY", "COMPANY", "INC", "CỔ PHẦN",
            "CP", "THƯƠNG MẠI", "THUONG MAI", "TM", "DỊCH VỤ", "DICH VU", "DV", "XUẤT NHẬP KHẨU",
            "XUAT NHAP KHAU", "XNK", "TM&DV", "MỘT THÀNH VIÊN", "MOT THANH VIEN", "MTV", "ĐẦU TƯ",
            "DAU TU", "PHÁT TRIỂN", "PHAT TRIEN", "VÀ", "NỘI THẤT", "NOI THAT", "KINH DOANH",
            "KINH DOANH", "XÂY DỰNG", "XAY DUNG", "DU LỊCH", "DU LICH", "TRUYỀN THÔNG", "TRUYEN THONG",
            "CÔNG NGHỆ", "CONG NGHE", "TƯ NHÂN", "PHẦN MỀM", "PM", "PHAN MEM"
        };

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

        public string FilterIrrelevantWords(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }
            input = input.ToUpper();
            input = Regex.Replace(input, @"\s+", " ");
            foreach (string irrelevantWord in irrelevantWords)
            {
                input = input.Replace(irrelevantWord, "");
            }
            return input.Trim();
        }

        public IEnumerable<Customer> GetSimilarCustomers(Customer customer)
        {
            int percentage = 70;
            var entities = _customerRepository.GetAll().Where(c => c.IsDelete == false);
            var response = new List<Customer>();
            foreach(var entity in entities)
            {
                var matchingPercentage = StringCompare(FilterIrrelevantWords(customer.Name), FilterIrrelevantWords(entity.Name));
                if (matchingPercentage >= percentage)
                {
                    response.Add(entity);
                }
            }
            var customerEntities = _customerRepository.GetAll()
                .Where(c => c.IsDelete == false &&
                            StringCompare(FilterIrrelevantWords(customer.Name), FilterIrrelevantWords(c.Name)) >=
                            percentage);
            //return customerEntities;
            return response;
        }
    }
}
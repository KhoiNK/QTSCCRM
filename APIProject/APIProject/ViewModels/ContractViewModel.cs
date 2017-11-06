using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIProject.ViewModels
{
    public class ContractViewModel
    {
        public int ID { get; set; }
        public string ContractCode { get; set; }
        public string CustomerName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
    public class PostContractsResponseViewModel
    {
        public bool OppotunityUpdated { get; set; }
        public bool CustomerConverted { get; set; }
        public bool ContractsCreated { get; set; }
        public List<int> ContractIDs { get; set; }
    }
    public class PostContractsViewModel
    {
        [Required]
        public int StaffID { get; set; }
        [Required]
        public int OpportunityID { get; set; }
        [Required]
        public List<PostContractViewModel> Contracts { get; set; }
    }
    public class PostContractViewModel
    {
        [Required]
        public int QuoteItemID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }

    public class ContractDetailsViewModel
    {
        public int ID { get; set; }
        public string ContractCode { get; set; }
        public CustomerDetailViewModel CustomerDetail { get; set; }
        public ContactViewModel ContactDetail { get; set; }
        public StaffDetailViewModel StaffDetail { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        public ContractDetailsViewModel(Contract contractDto,
            Customer customerDto,
            Contact contactDto,
            Staff staffDto)
        {
            ID = contractDto.ID;
            ContractCode = contractDto.ContractCode;
            Status = contractDto.Status;
            StartDate = contractDto.StartDate;
            EndDate = contractDto.EndDate;
            Name = contractDto.Name;
            Price = contractDto.Price;
            Unit = contractDto.Unit;
            if (customerDto != null)
            {
                CustomerDetail = new CustomerDetailViewModel(customerDto);
            }
            if (contactDto != null)
            {
                ContactDetail = new ContactViewModel(contactDto);
            }
            if (staffDto != null)
            {
                StaffDetail = new StaffDetailViewModel(staffDto);
            }
        }
    }

    
}
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
        public string SalesCategoryName { get; set; }
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
        public List<PostContractViewModel> Categories { get; set; }
    }
    public class PostContractViewModel
    {
        [Required]
        public int SalesCagetogyID { get; set; }
        [Required]
        public List<PostContractItemViewModel> QuoteItems { get; set; }
    }
    public struct PostContractItemViewModel
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
        public string CategoryName { get; set; }
        public string Status { get; set; }
        public List<ContractItemDetailViewModel> ContractItems { get; set; }
        public CustomerDetailViewModel CustomerDetail { get; set; }
        public ContactViewModel ContactDetail { get; set; }
        public StaffDetailViewModel StaffDetail { get; set; }

        public ContractDetailsViewModel(Contract contractDto,
            SalesCategory salesCategoryDto,
            List<ContractItem> contractItemDtos,
            Customer customerDto,
            Contact contactDto,
            Staff staffDto)
        {
            ID = contractDto.ID;
            ContractCode = contractDto.ContractCode;
            CategoryName = salesCategoryDto.Name;
            Status = contractDto.Status;
            ContractItems = contractItemDtos.Select(c => new ContractItemDetailViewModel(c)).ToList();
            CustomerDetail = new CustomerDetailViewModel(customerDto);
            ContactDetail = new ContactViewModel(contactDto);
            StaffDetail = new StaffDetailViewModel(staffDto);
        }
    }

    public class ContractItemDetailViewModel
    {
        public int ID { get; set; }
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string Unit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ContractItemDetailViewModel(ContractItem dto)
        {
            ID = dto.ID;
            ItemCode = dto.ItemCode;
            Name = dto.Name;
            Quantity = dto.Quantity;
            Price = dto.Price;
            Unit = dto.Unit;
            StartDate = dto.StartDate;
            EndDate = dto.EndDate;
        }
    }
}
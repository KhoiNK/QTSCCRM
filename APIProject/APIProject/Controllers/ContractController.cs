using APIProject.GlobalVariables;
using APIProject.Model.Models;
using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/contract")]
    public class ContractController : ApiController
    {
        private readonly IContractService _contractService;
        private readonly IQuoteService _quoteService;
        private readonly IStaffService _staffService;
        private readonly IContactService _contactService;
        private readonly IQuoteItemMappingService _quoteItemMappingService;
        private readonly ICustomerService _customerService;
        private readonly ISalesItemService _salesItemService;
        private readonly IOpportunityService _opportunityService;
        private readonly IUploadNamingService _uploadNamingService;
        private readonly IContractItemService _contractItemService;
        private readonly ISalesCategoryService _salesCategoryService;

        public ContractController(IContractService _contractService,
            IQuoteItemMappingService _quoteItemMappingService,
            ICustomerService _customerService, ISalesCategoryService _salesCategoryService,
            IUploadNamingService _uploadNamingService,
            IQuoteService _quoteService,
            ISalesItemService _salesItemService,
            IStaffService _staffService,
            IContractItemService _contractItemService,
            IContactService _contactService,
            IOpportunityService _opportunityService)
        {
            this._uploadNamingService = _uploadNamingService;
            this._salesCategoryService = _salesCategoryService;
            this._customerService = _customerService;
            this._quoteItemMappingService = _quoteItemMappingService;
            this._contractItemService = _contractItemService;
            this._salesItemService = _salesItemService;
            this._opportunityService = _opportunityService;
            this._contractService = _contractService;
            this._quoteService = _quoteService;
            this._staffService = _staffService;
            this._contactService = _contactService;
        }

        [Route("PostContracts")]
        [ResponseType(typeof(PostContractsResponseViewModel))]
        public IHttpActionResult PostContracts(PostContractsViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            #region validate
            if (!request.Categories.Any())
            {
                return BadRequest(message: CustomError.ContractRequired);
            }
            foreach (var contract in request.Categories)
            {
                if (!contract.QuoteItems.Any())
                {
                    return BadRequest(message: CustomError.ContractItemRequired);
                }
                foreach (var contractItem in contract.QuoteItems)
                {
                    if (DateTime.Compare(DateTime.Now, contractItem.StartDate) > 0)
                    {
                        return BadRequest(message: CustomError.ContractItemStartDateMustPassCurrent);
                    }
                    if (DateTime.Compare(contractItem.StartDate, contractItem.EndDate) > 0)
                    {
                        return BadRequest(message: CustomError.ContractItemStartDateMustNotPassEndDate);
                    }
                }
            }
            #endregion
            try
            {
                var responseResult = new PostContractsResponseViewModel();
                var foundOpp = _opportunityService.Get(request.OpportunityID);
                if (foundOpp.StageName != OpportunityStage.Negotiation)
                {
                    throw new Exception(CustomError.OppStageRequired
                        + OpportunityStage.Negotiation);
                }
                var foundStaff = _staffService.Get(request.StaffID);
                var ContractIDs = new List<int>();
                foreach (var category in request.Categories)
                {
                    Contract contract = new Contract();
                    contract.CreatedStaffID = request.StaffID;
                    contract.CreatedDate = DateTime.Now;
                    contract.ContactID = foundOpp.ContactID.Value;
                    contract.CustomerID = foundOpp.CustomerID.Value;
                    contract.SalesCategoryID = category.SalesCagetogyID;
                    contract.Status = ContractStatus.Waiting;
                    contract.UpdatedDate = DateTime.Now;

                    var insertedContract = _contractService.Add(contract);
                    ContractIDs.Add(insertedContract.ID);
                    var contractItems = new List<ContractItem>();
                    foreach (var quoteItem in category.QuoteItems)
                    {
                        var quoteItemMapping = _quoteItemMappingService.Get(quoteItem.QuoteItemID);
                        ContractItem contractItem = new ContractItem
                        {
                            ContractID = insertedContract.ID,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            StartDate = quoteItem.StartDate,
                            EndDate = quoteItem.EndDate,
                            ItemCode = Guid.NewGuid().ToString(),
                            SalesItemID = quoteItemMapping.SalesItemID,
                            Name = quoteItemMapping.SalesItemName,
                            Price = quoteItemMapping.Price.Value,
                            Unit = quoteItemMapping.Unit,
                            Quantity = quoteItem.Quantity
                        };
                        contractItems.Add(contractItem);
                    }
                    _contractItemService.AddRange(insertedContract,contractItems);
                }
                responseResult.ContractsCreated = true;
                responseResult.ContractIDs = ContractIDs;

                _opportunityService.SetWon(foundOpp);
                _opportunityService.SaveChanges();
                responseResult.OppotunityUpdated = true;

                var foundCustomer = _customerService.Get(foundOpp.CustomerID.Value);
                if (foundCustomer.CustomerType == CustomerType.Lead)
                {
                    _customerService.ConvertToCustomer(foundCustomer);
                    responseResult.CustomerConverted = true;
                }
                _customerService.SaveChanges();
                #region debug return result
                //var response = new List<ContractDetailsViewModel>();
                //foreach (var contractID in ContractIDs)
                //{
                //    var foundContract = _contractService.Get(contractID);
                //    var contractItems = _contractItemService.GetByContract(contractID).ToList();
                //    var contractContact = _contactService.Get(foundContract.ContactID);
                //    _uploadNamingService.ConcatContactAvatar(contractContact);
                //    var contractCustomer = _customerService.Get(foundContract.CustomerID);
                //    _uploadNamingService.ConcatCustomerAvatar(contractCustomer);
                //    var contractStaff = _staffService.Get(foundContract.CreatedStaffID);
                //    _uploadNamingService.ConcatStaffAvatar(contractStaff);
                //    var contractCategory = _salesCategoryService.Get(foundContract.SalesCategoryID);
                //    response.Add(new ContractDetailsViewModel(
                //        foundContract,
                //        contractCategory,
                //        contractItems,
                //        contractCustomer,
                //        contractContact,
                //        contractStaff));
                //}
                #endregion

                return Ok(responseResult);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Route("GetContracts")]
        [ResponseType(typeof(ContractViewModel))]
        public IHttpActionResult GetContracts()
        {
            var entities = _contractService.GetAll();
            var response = new List<ContractViewModel>();
            foreach (var entity in entities)
            {
                var contractCustomerName = _customerService.Get(entity.CustomerID).Name;
                var contractCategoryName = _salesCategoryService.Get(entity.SalesCategoryID).Name;
                response.Add(new ContractViewModel
                {
                    ID = entity.ID,
                    ContractCode = entity.ContractCode,
                    CustomerName = contractCustomerName,
                    SalesCategoryName = contractCategoryName,
                    Status = entity.Status
                });
            }
            return Ok(response);
        }

        [Route("GetContract")]
        [ResponseType(typeof(ContractDetailsViewModel))]
        public IHttpActionResult GetContract(int id = 0)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            try
            {
                var foundContract = _contractService.Get(id);
                var contractItems = _contractItemService.GetByContract(id).ToList();
                var contractContact = _contactService.Get(foundContract.ContactID);
                _uploadNamingService.ConcatContactAvatar(contractContact);
                var contractCustomer = _customerService.Get(foundContract.CustomerID);
                _uploadNamingService.ConcatCustomerAvatar(contractCustomer);
                var contractStaff = _staffService.Get(foundContract.CreatedStaffID);
                _uploadNamingService.ConcatStaffAvatar(contractStaff);
                var contractCategory = _salesCategoryService.Get(foundContract.SalesCategoryID);
                var response = new ContractDetailsViewModel(
                    foundContract,
                        contractCategory,
                        contractItems,
                        contractCustomer,
                        contractContact,
                        contractStaff);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

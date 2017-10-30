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
        private readonly ISalesItemService _salesItemService;
        private readonly IOpportunityService _opportunityService;
        private readonly IContractItemService _contractItemService;

        public ContractController(IContractService _contractService,
            IQuoteItemMappingService _quoteItemMappingService,
            IQuoteService _quoteService,
            ISalesItemService _salesItemService,
            IStaffService _staffService,
            IContractItemService _contractItemService,
            IContactService _contactService,
            IOpportunityService _opportunityService)
        {
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
                var foundOpp = _opportunityService.Get(request.OpportunityID);
                var foundStaff = _staffService.Get(request.StaffID);
                var ContractIDs = new List<int>();
                foreach (var category in request.Categories)
                {
                    Contract contract = new Contract();
                    contract.CreatedStaffID = request.StaffID;
                    contract.CreatedDate = DateTime.Now;
                    contract.SalesCategoryID = category.SalesCagetogyID;
                    contract.Status = ContractStatus.Waiting;
                    contract.UpdatedDate = DateTime.Now;

                    var insertedContract = _contractService.Add(contract);
                    ContractIDs.Add(insertedContract.ID);
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
                        _contractItemService.Add(contractItem);
                    }
                }
                //todo
                return Ok(ContractIDs);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}

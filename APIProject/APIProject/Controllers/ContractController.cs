using APIProject.GlobalVariables;
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

        public ContractController(IContractService _contractService,
            IQuoteService _quoteService,
            IStaffService _staffService,
            IContactService _contactService)
        {
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
            if (!request.Contracts.Any())
            {
                return BadRequest(message: CustomError.ContractRequired);
            }
            foreach(var contract in request.Contracts)
            {
                if (!contract.ContractItems.Any())
                {
                    return BadRequest(message: CustomError.ContractItemRequired);
                }
                foreach(var contractItem in contract.ContractItems)
                {
                    if (DateTime.Compare(DateTime.Now, contractItem.StartDate) > 0)
                    {
                        return BadRequest(message: CustomError.ContractItemStartDateMustPassCurrent);
                    }
                    if(DateTime.Compare(contractItem.StartDate,contractItem.EndDate) > 0)
                    {
                        return BadRequest(message: CustomError.ContractItemStartDateMustNotPassEndDate);
                    }
                }
            }
            #endregion
            try
            {
                var foundQuote = _quoteService.Get(request.QuoteID);
                var foundStaff = _staffService.Get(request.StaffID);
                var foundContact = _contactService.Get(request.ContactID);
                //todo
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}

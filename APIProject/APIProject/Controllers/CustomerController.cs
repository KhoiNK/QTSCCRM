﻿using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APIProject.Controllers
{
    [RoutePrefix("api/customer")]
    public class CustomerController : ApiController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService _customerService)
        {
            this._customerService = _customerService;
        }

        [Route("PostLeadCustomer")]
        public IHttpActionResult PostLeadCustomer(PostLeadCustomerViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            string avatarB64 = null;
            string avatarName = null;
            if(request.Avatar != null)
            {
                avatarName = request.Avatar.Name;
                avatarB64 = request.Avatar.Base64Content;
            }

            return Ok(_customerService.CreateNewLead(request.ToCustomerModel(), avatarName ,avatarB64));
        }

        [Route("PutLeadInformation")]
        public IHttpActionResult PutLeadInformation(PutLeadInformationViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            string avatarB64 = null;
            string avatarName = null;
            if (request.Avatar != null)
            {
                avatarName = request.Avatar.Name;
                avatarB64 = request.Avatar.Base64Content;
            }

            return Ok(_customerService.EditLead(request.ToCustomerModel(), avatarName, avatarB64));
        }

        [Route("PutCustomerInformation")]
        public IHttpActionResult PutCustomerInformation(PutCustomerInformationViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(_customerService.EditCustomer(request.ToCustomerModel()));
        }

        [Route("GetCustomerList")]
        public IHttpActionResult GetCustomerList()
        {
            return Ok(_customerService.GetCustomerList().Select(x => new CustomerViewModel(x)));
        }
        [Route("GetCustomerDetail")]
        public IHttpActionResult GetCustomerDetail(int ID)
        {
            return Ok(_customerService.GetCustomerList().Where(x => x.ID == ID).Select(x => new CustomerDetailViewModel(x)));
        }
    }
}

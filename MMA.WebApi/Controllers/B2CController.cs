using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MMA.WebApi.Core.Services;
using MMA.WebApi.Models;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Models.B2C;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [ApiController]
    public class B2CController : BaseController
    {
        private readonly CustomerService _customerService;
        private readonly SupplierService _supplierService;
        private readonly UserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ILogger _logger;

        public B2CController(CustomerService customerService, SupplierService supplierService, ILogger<B2CController> logger, UserService userService, ICompanyService companyService, IApplicationUserService applicationUserService)
        {
            _logger = logger;
            _supplierService = supplierService;
            _customerService = customerService;
            _applicationUserService = applicationUserService;
            _userService = userService;
            _companyService = companyService;
        }


        /// <summary>
        /// B2C_1_Customer_Email_Phone_SignIn_SignUP
        /// </summary>
        /// <param name="apiConnectorData"></param>
        /// <returns></returns>
        [HttpPost("customer-b2c-api-connector")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateCustomerRegistration(ApiConnectorDataCustomer apiConnectorData)
        {
            try
            {
                return await RegistrationValidationCustomer(apiConnectorData);
            }
            catch (Exception e)
            {
                return BadRequest(ApiConnectorResponse.ValidationErrorResponse(e.ToString()));
            }
        }

        [HttpPost("supplier-b2c-api-connector")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateSupplierRegistration([FromBody] Dictionary<string, object> apiConnectorData)
        {
            try
            {
                ApiConnectorDataSupplier supplierData = new ApiConnectorDataSupplier();

                foreach (string key in apiConnectorData.Keys)
                {
                    if (key.Contains("_CompanyName"))
                    {
                        supplierData.CompanyName = apiConnectorData[key].ToString();
                    }
                    if (key.Contains("_CompanyDescription"))
                    {
                        supplierData.CompanyDescription = apiConnectorData[key].ToString();
                    }
                }

                List<ApiConnectorDataIdentities> identities = JsonConvert.DeserializeObject<List<ApiConnectorDataIdentities>>(apiConnectorData["identities"].ToString());
                supplierData.Identities.AddRange(identities);

                return await RegistrationValidationSupplier(supplierData);
            }
            catch (Exception e)
            {
                return BadRequest(ApiConnectorResponse.ValidationErrorResponse(e.ToString()));
            }
        }

        private async Task<IActionResult> RegistrationValidationCustomer(ApiConnectorDataCustomer apiConnectorData)
        {
            bool isAllowRegister = false;
            // Microsoft rule -> Maximum number of characters for string field is 256
            if (apiConnectorData.GivenName.Length > 256) return BadRequest(ApiConnectorResponse.ValidationErrorResponse($"Maximum number of characters for Given Name field is 256."));
            if (apiConnectorData.Surname.Length > 256) return BadRequest(ApiConnectorResponse.ValidationErrorResponse($"Maximum number of characters for Surname field is 256."));

            var identifier = apiConnectorData.Identities.Select(x => x.IssuerAssignedId).FirstOrDefault();

            // Supplier
            if (await _userService.UserExists(identifier) && await _companyService.CheckIfUserExistsAsCompanySupplier(identifier))
            {
                // Supplier is already created through UI by SupplierAdmin, it only needs to register to B2C
                isAllowRegister = true;
                return Ok(ApiConnectorResponse.ContinuationResponse($"Registration as Supplier successfull for {identifier}"));
            }

            // Customer
            // TODO: Add another check -> check if username exists in AllowedEmailsForRegistration table
            //if (await _userService.UserExists(identifier))
            //{
            //    await _customerService.UpdateUserFirstAndLastName(identifier, apiConnectorData.GivenName, apiConnectorData.Surname);
            //    return Ok(ApiConnectorResponse.ContinuationResponse($"Registration as Customer successfull for {identifier}"));
            //}

            // TODO: Uncomment this line after all domains are correctly added and remove the line below this one
            // (Difference between this 2 methods -> 
            // 1. Checks if is in invited OR accepted domains -> Old feature 
            // 2. Checks if is in invited AND user accepted domains -> New feature 
            if (await _applicationUserService.CheckIfUserIsInvitedOrIsInUserDomains(identifier))
            {
                isAllowRegister = true;
            }

            if (await _applicationUserService.CheckIfUserIsInvitedOrIsInAcceptedDomains(identifier))
            {
                isAllowRegister = true;
            }

            if (isAllowRegister)
            {
                await _customerService.CreateCustomer(apiConnectorData);
                return Ok(ApiConnectorResponse.ContinuationResponse($"Registration as Customer successfull for {identifier}"));
            }
            else
            {
                return BadRequest(ApiConnectorResponse.ValidationErrorResponse($"User {identifier} is not allowed to be registered."));
            }
        }

        private async Task<IActionResult> RegistrationValidationSupplier(ApiConnectorDataSupplier apiConnectorData)
        {
            // Microsoft rule -> Maximum number of characters for string field is 256
            if (apiConnectorData.CompanyName.Length > 256) return BadRequest(ApiConnectorResponse.ValidationErrorResponse($"Maximum number of characters for Company Name field is 256."));
            if (apiConnectorData.CompanyDescription.Length > 256) return BadRequest(ApiConnectorResponse.ValidationErrorResponse($"Maximum number of characters for Company Description field is 256."));

            var identifier = apiConnectorData.Identities.Select(x => x.IssuerAssignedId).FirstOrDefault();

            if (await _userService.UserExists(identifier)) return BadRequest(ApiConnectorResponse.ValidationErrorResponse($"User {identifier} already exists."));

            if (await _companyService.CheckIfCompanyWithSameNameExits(apiConnectorData.CompanyName))
            {
                await _supplierService.AttachSupplierToExistingCompany(apiConnectorData);
                return Ok(ApiConnectorResponse.ContinuationResponse($"Registration Successfull for {identifier}"));
            }

            await _supplierService.CreateSupplier(apiConnectorData);
            var response = ApiConnectorResponse.ContinuationResponse($"Registration Successfull for {identifier}");

            return Ok(response);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Image;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Image;
using MMA.WebApi.Shared.Monads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class CompaniesController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ICompanyService _companyService;
        private readonly IRoleService _roleService;
        private readonly IImageUtilsService _imageUtilsService;

        public CompaniesController(
            IConfiguration configuration,
            ICompanyService companyService,
            IRoleService roleService,
            IImageUtilsService imageUtilsService
        )
        {
            _configuration = configuration;
            _companyService = companyService;
            _roleService = roleService;
            _imageUtilsService = imageUtilsService;
        }

        [HttpPost("all/page/{pageNumber}")]
        public async Task<IActionResult> GetAllCompaniesPage(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                queryModel.PaginationParameters.PageSize = 10;

                return Ok(await _companyService.GetAllCompaniesPage(queryModel));
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost("suppliers-missing")]
        public async Task<IActionResult> GetAllMissingLicenseCompanies(QueryModel queryModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var missingLicenseComapnies = await _companyService.GetAllMissingLicenseCompanies(
                    queryModel
                );
                if (missingLicenseComapnies == null)
                    return NotFound();
                return Ok(missingLicenseComapnies);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost("suppliers-inactive")]
        public async Task<IActionResult> GetAllRejectedAndDeactivatedCompaniesPage(
            QueryModel queryModel,
            int pageNumber
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var inactiveCompanies =
                    await _companyService.GetAllRejectedAndDeactivatedCompaniesPage(queryModel);
                if (inactiveCompanies == null)
                    return NotFound();
                return Ok(inactiveCompanies);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCompaniesCard()
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
                return Ok(await _companyService.GetAllCompaniesCard());

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpGet("get-company-id/{companyName}")]
        public async Task<IActionResult> CheckIfCompanyWithSameNameExits(string companyName)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
                return Ok(await _companyService.GetCompanyIdForCompanyName(companyName));

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost("suppliers")]
        public async Task<IActionResult> CreateOrUpdateFocalPoint(ApplicationUserModel model)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplierAdmin(UserId))
            {
                model.ImageSets = new List<ImageModel>();
                if (model.Image != null)
                    model.ImageSets.Add(model.Image);
                var croppedImages = await _imageUtilsService.PrepareImagesForUpload(
                    model.ImageSets
                );
                model.ImageSets = croppedImages;
                var result = await _companyService.CreateOrUpdateFocalPoint(model, UserId);

                // Handle image upload in background
                //BackgroundJob.Enqueue(() => _imageUtilsService.CreateImages(croppedImages, Declares.ImageForType.User));
                new ImageUploadHelper(_configuration).UploadImagesInBackground(
                    croppedImages,
                    Declares.ImageForType.User
                );

                if (result is Maybe<ApplicationUserModel>.None)
                    return NotFound();

                if (result == null)
                    return BadRequest("One focal point must be Supplier Admin.");

                return Ok(result.Value);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        //Get all Focal Points for my company
        //Including Supplier Admin
        [HttpPost("suppliers/page/{pageNumber}")]
        public async Task<IActionResult> GetMyFocalPointsPage(QueryModel queryModel, int pageNumber)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplierAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                queryModel.PaginationParameters.PageSize = 15;
                var suppliers = await _companyService.GetMyFocalPointsPage(queryModel, UserId);

                await AddRolesToSupplier(suppliers);

                return Ok(suppliers);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost("suppliers/page/{pageNumber}/{companyId}")]
        public async Task<IActionResult> GetAllSuppliersForCompany(
            QueryModel queryModel,
            int pageNumber,
            int companyId
        )
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplierAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                queryModel.PaginationParameters.PageSize = 15;

                var supplierUsers = await _companyService.GetAllAdminSupliersAndSuppliersPage(
                    queryModel,
                    companyId
                );

                await AddRolesToSupplier(supplierUsers);

                return Ok(supplierUsers);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpDelete]
        [Route("suppliers/{id}")]
        public async Task<IActionResult> DeleteMyFocalPoint(string id)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplierAdmin(UserId))
            {
                if (id == UserId)
                    return BadRequest("Cannot delete logged in user.");
                await _companyService.DeleteFocalPoint(id, UserId);
                return Ok(
                    JsonConvert.SerializeObject(
                        MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Focal Point")
                    )
                );
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost("suppliers-pending")]
        public async Task<IActionResult> GetAllPendingCompanies(QueryModel queryModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = queryModel.Page
                };
                var pendingCompanies = await _companyService.GetAllPendingCompanies(queryModel);
                if (pendingCompanies == null)
                    return NotFound();
                return Ok(pendingCompanies);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpGet("{id}/offers")]
        public async Task<IActionResult> GetCompanyOffers(int id)
        {
            return Ok(_companyService.GetCompanyOffers(id));
        }

        [HttpGet("rating/{id}")]
        public async Task<IActionResult> GetCompanyRating(int id)
        {
            return Ok(await _companyService.GetCompanyRating(id));
        }

        [HttpPost("pending/page/{pageNumber}")]
        public async Task<IActionResult> GetAllPendingSuppliersPage(
            QueryModel queryModel,
            int pageNumber
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                queryModel.PaginationParameters.PageSize = 10;

                return Ok(await _companyService.GetAllPendingCompaniesPage(queryModel));
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpGet("suppliers/{id}")]
        public async Task<IActionResult> GetFocalPointById(string id)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplierAdmin(UserId))
            {
                var focalPoint = await _companyService.GetFocalPointById(id, UserId);
                if (focalPoint == null)
                    return NotFound("There was an error. Please contact system administrator.");
                return Ok(focalPoint);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(CompanyModel model)
        {
            var roles = await _roleService.GetUserRoles(UserId);

            if (_companyService.CanManageCompany(roles, model, UserId))
            {
                var logo = new List<ImageModel>();
                var croppedImages = new List<ImageModel>();
                if (!string.IsNullOrWhiteSpace(model.Logo))
                {
                    logo = PrepareLogo(model.Logo);
                    croppedImages = await _imageUtilsService.PrepareImagesForUpload(logo);
                }
                var result = await _companyService.CreateOrUpdateAsync(model, UserId);

                if (croppedImages.Count > 0)
                    // Handle image upload in background
                    //BackgroundJob.Enqueue(() => _imageUtilsService.CreateImages(croppedImages, Declares.ImageForType.Company));
                    new ImageUploadHelper(_configuration).UploadImagesInBackground(
                        croppedImages,
                        Declares.ImageForType.Company
                    );

                if (result is Maybe<CompanyModel>.None)
                    return NotFound();

                return Ok(result.Value);
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        private List<ImageModel> PrepareLogo(string logo)
        {
            return new List<ImageModel>()
            {
                new ImageModel()
                {
                    Id = logo,
                    OriginalImageId = Guid.Parse(logo),
                    Cover = true,
                    Type = Shared.Enums.Document.DocumentDeclares.OfferDocumentType.Thumbnail,
                    CropCoordinates = new CropCoordinates
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = 0,
                        Y2 = 0
                    },
                    CropNGXCoordinates = new CropCoordinates
                    {
                        X1 = 0,
                        X2 = 0,
                        Y1 = 0,
                        Y2 = 0
                    }
                }
            };
        }

        [HttpGet("getCompanyByUser")]
        public async Task<IActionResult> GetMySupplierCompany()
        {
            //API SECURITY: CHECK THIS ONE!!!
            var roleContain = await _roleService.CheckIfUserIsNotBuyer(UserId);

            if (roleContain.IsBuyer)
                return BadRequest(
                    "You do not have permission to do this action. Please contact system administartor."
                );

            var companies = await _companyService.GetMySupplierCompany(UserId);

            if (companies == null)
                return NotFound();

            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //API SECURITY: CHECK THIS ONE!!!
            // ADMINI, SUPPLIERI ALI SAMO SVOJU, REVIEWER
            var roles = await _roleService.GetUserRoles(UserId);
            var result = await _companyService.GetCompanyById(id, roles, UserId);

            if (result is Maybe<CompanyModel>.None)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Soft Delete of company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _companyService.DeleteCompany(id);
                return Ok(
                    JsonConvert.SerializeObject(
                        MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Company")
                    )
                );
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        /// <summary>
        /// Delete of company
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("remove/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _companyService.HardDeleteCompany(id);
                return Ok(
                    JsonConvert.SerializeObject(
                        MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Company")
                    )
                );
            }

            return BadRequest(
                "You do not have permission to do this action. Please contact system administartor."
            );
        }

        #region private methods

        private async Task AddRolesToSupplier(PaginationListModel<ApplicationUserModel> suppliers)
        {
            // get all suppliers roles
            var userRoles = await _roleService.GetUsersRoles(
                suppliers.List.Select(x => x.Id).ToList()
            );
            // add roles to supplier

            foreach (var supplier in suppliers.List)
            {
                supplier.Role = String.Join(",", userRoles[supplier.Id]);
            }
        }

        private string isModelValidSupplier(CompanyRegistrationModel model)
        {
            StringBuilder sb = new StringBuilder();

            if (model.TradeLicence == null)
            {
                sb.AppendLine("Upload Trade Licence file");
            }
            if (model.TradeLicenceExpiryDate < DateTime.UtcNow)
            {
                sb.AppendLine("Trade Licence Expiry Date is not valid");
            }
            if (!model.TermsAndConditions)
            {
                sb.AppendLine("Please accept Terms And Conditions");
            }
            return sb.ToString();
        }

        [HttpPost("update-trade-licence")]
        public async Task<IActionResult> UpdateTeradeLicence(CompanyRegistrationModel data)
        {
            var errorMessage = isModelValidSupplier(data);
            if (!string.IsNullOrEmpty(errorMessage))
                return BadRequest(errorMessage);

            await _companyService.UpdateTeradeLicence(data, UserId);
            //   var createdUser = await _userManager.FindByNameAsync(applicationUser.UserName);

            /*    CompanyModel company = new CompanyModel()
                {
                    NameEnglish = data.CompanyName,
                    OfficialEmail = data.Email,
                    TradeLicence = data.TradeLicence,
                    TradeLicenceExpiryDate = data.TradeLicenceExpiryDate,
                    CreatedBy = createdUser.Id,
                    UpdatedBy = createdUser.Id,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    EstablishDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(365),
                    ApproveStatus = SupplierStatus.PendingApproval.ToString(),
                };

                await _companyService.RegisterCompanyAsync(company, createdUser.Id);

                if (coordinators.Count > 0)
                {
                    var messageTemplate = Declares.MessageTemplateList.Supplier_Registration_Notify_Coordinator;
                    var emailData = _mailStorageServiceService.CreateMailData(coordinators.FirstOrDefault().Id, null,
                                                                                company.NameEnglish, messageTemplate, false);
                    await _mailStorageServiceService.CreateMail(emailData);
                }*/

            return Ok(
                JsonConvert.SerializeObject(
                    MessageConstants.SaveMessages.ItemSuccessfullySaved("Registration Ok")
                )
            );
        }
        #endregion
    }
}

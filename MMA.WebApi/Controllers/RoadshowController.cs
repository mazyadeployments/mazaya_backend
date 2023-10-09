using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Constants;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowEvent;
using MMA.WebApi.Shared.Interfaces.RoadshowInvite;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.Roadshow;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class RoadshowController : BaseController
    {
        private readonly IRoadshowService _roadshowService;
        private readonly IRoadshowInviteService _roadshowInviteService;
        private readonly IRoadshowEventService _roadshowEventService;
        private readonly ICompanyService _companyService;
        private readonly IRoleService _roleService;

        public RoadshowController(
            IRoadshowService roadshowService,
            IRoadshowInviteService roadshowInviteService,
            IRoleService roleService,
            IRoadshowEventService roadshowEventService,
            ICompanyService companyService
        )
        {
            _roadshowService = roadshowService;
            _companyService = companyService;
            _roleService = roleService;
            _roadshowInviteService = roadshowInviteService;
            _roadshowEventService = roadshowEventService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (
                _roleService.CheckIfUserIsAdminOrSupplier(UserId)
                || _roleService.CheckIfUserIsRoadshowFocalPoint(UserId)
            )
            {
                var result = await _roadshowService.GetRoadshow(id);

                if (result is null)
                    return NotFound("Roadshow not found");

                return Ok(result);
            }

            return BadRequest("You don't have permission to do see this page.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoadshow(RoadshowModel model)
        {
            if (_roleService.CheckIfUserIsSupplierOrSupplierAdmin(UserId))
            {
                var result = await _roadshowService.CreateRoadshow(model, UserId);
                if (result is null)
                    return NotFound("You don't have permission to do this action.");

                await _roadshowService.SendMailNotificationIfRoadshowStatusChanged(
                    result.Status,
                    result.Status,
                    result
                );
                return Ok(result);
            }
            return Unauthorized("You don't have permission to do this action.");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateRoadshow(RoadshowModel model)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                var oldStatus = await _roadshowService.GetRoadshowStatusById(model.Id);
                var result = await _roadshowService.UpdateRoadshow(model, UserId);
                if (result is null)
                    return NotFound("You don't have permission to do this action.");

                await _roadshowService.SendMailNotificationIfRoadshowStatusChanged(
                    oldStatus,
                    result.Status,
                    result
                );
                return Ok(result);
            }
            return Unauthorized("You don't have permission to do this action.");
        }

        [HttpPost("{roadshowId}/unpublish")]
        public async Task<IActionResult> Unpublish(int roadshowId)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _roadshowService.UnpublishRoadshow(roadshowId);
                return Ok();
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!_roleService.CheckIfUserIsRoadshowFocalPoint(UserId))
            {
                return Ok(await _roadshowService.GetRoadshows());
            }
            else
            {
                return Ok(await _roadshowService.GetConfirmedRoadshows());
            }
        }

        [HttpPost("{id}/suppliers/forinvite/page/{pageNumber}")]
        public async Task<IActionResult> GetSuppliersForRoadshowInviteModal(
            QueryModel queryModel,
            int pageNumber,
            int id
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                return Ok(await _companyService.GetSuppliersForRoadshowInviteModal(queryModel, id));
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("{id}/invites/add")]
        public async Task<IActionResult> SendRoadshowInvitesFromModal(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            int id
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _roadshowInviteService.SendRoadshowInvitesFromModal(
                    roadshowInvitesQueryModel,
                    id,
                    UserId
                );
                return Ok();
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("{id}/invites/sendinvite")]
        public async Task<IActionResult> SendRoadshowInvitesFromForm(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel,
            int id
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                await _roadshowInviteService.SendRoadshowInvitesFromForm(
                    roadshowInvitesQueryModel,
                    id,
                    UserId
                );
                return Ok();
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("{id}/invites/{idinvite}/events")]
        public IActionResult AddOrUpdateRoadshowEventToRoadshowInvite(
            RoadshowEventModel roadshowEventModel,
            int id,
            int idinvite
        )
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var eventId = _roadshowInviteService.AddOrUpdateRoadshowEventToRoadshowInvite(
                    roadshowEventModel,
                    id,
                    idinvite
                );
                return Ok(eventId);
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpDelete]
        [Route("{id}/invites/{idinvite}/events/{idevent}")]
        public async Task<IActionResult> DeleteRoadshowEventToRoadshowInvite(int idevent)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                var response = await _roadshowInviteService.DeleteRoadshowEventToRoadshowInvite(
                    idevent
                );

                if (response)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Error while deleting event");
                }
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost]
        [Route("{id}/invites/{idinvite}/delete")]
        public IActionResult DeleteRoadshowInvites(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel
        )
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                _roadshowInviteService.DeleteRoadshowInvites(roadshowInvitesQueryModel);
                return Ok();
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost]
        [Route("{id}/invites/{idinvite}/change")]
        public IActionResult UpdateRoadshowInvites(
            RoadshowInvitesQueryModel roadshowInvitesQueryModel
        )
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                _roadshowInviteService.UpdateRoadshowInvites(roadshowInvitesQueryModel);
                return Ok();
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("{id}/invites")]
        public async Task<IActionResult> GetAllRoadshowInvitesForRoadshow(
            QueryModel queryModel,
            int id
        )
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = queryModel.Page,
                    PageSize = 10
                };
                return Ok(
                    await _roadshowInviteService.GetAllRoadshowInvitesForRoadshow(
                        queryModel,
                        UserId,
                        id
                    )
                );
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpGet("{roadshowId}/invites/{inviteId}")]
        public async Task<IActionResult> GetRoadshowInviteDetails(int roadshowId, int inviteId)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
                return Ok(
                    await _roadshowInviteService.GetRoadshowInviteDetails(
                        roadshowId,
                        inviteId,
                        UserId
                    )
                );

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("{roadshowId}/invites/{inviteId}")]
        public async Task<IActionResult> UpdateOfRoadshowInviteDetails(
            RoadshowInviteDetailsModel roadshowInviteDetailsModel
        )
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                await _roadshowInviteService.UpdateOfRoadshowInviteDetails(
                    roadshowInviteDetailsModel,
                    UserId
                );
                return Ok(
                    JsonConvert.SerializeObject(
                        MessageConstants.UpdateMessages.ItemSuccessfullyUpdated(
                            "Roadshow Invite Details"
                        )
                    )
                );
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("event/{eventId}")]
        public async Task<IActionResult> GetOffersForSelectedEvent(
            QueryModel queryModel,
            int eventId
        )
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = queryModel.Page
                };
                return Ok(
                    await _roadshowEventService.GetOffersForSelectedEvent(queryModel, eventId)
                );
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("planning/page/{pageNumber}")]
        public async Task<IActionResult> GetAllRoadshowsPage(QueryModel queryModel, int pageNumber)
        {
            var isUserAdminOrSupplier = _roleService.CheckIfUserIsAdminOrSupplier(UserId);
            var isUserRoadShowFocalPoint = _roleService.CheckIfUserIsRoadshowFocalPoint(UserId);
            //API SECURITY: CHECK THIS ONE!!!
            if (isUserAdminOrSupplier || isUserRoadShowFocalPoint)
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                return Ok(await _roadshowService.GetAllRoadshows(queryModel, UserId));
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("calendar")]
        public async Task<IActionResult> GetAllRoadshowsForCalendar(CalendarQueryModel queryModel)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                return Ok(await _roadshowService.GetAllRoadshowsForCalendar(queryModel, UserId));
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("calendar/edit")]
        public async Task<IActionResult> EditRoadshowForCalendar(RoadshowModel model)
        {
            if (_roleService.CheckIfUserIsAdmin(UserId))
            {
                // fix this to call CreateOrUpdate on service
                var result = await _roadshowService.EditRoadshowForCalendar(model, UserId);
                if (result is null)
                {
                    return BadRequest("Error updating roadshow.");
                }
                return Ok(result);
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                await _roadshowService.DeleteRoadshow(id);
                return Ok(
                    JsonConvert.SerializeObject(
                        MessageConstants.DeleteMessages.ItemSuccessfullyDeleted("Roadshow")
                    )
                );
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpPost("calendar/events")]
        public async Task<IActionResult> GetAllEventsForCalendar(
            QueryModel queryModel,
            int pageNumber
        )
        {
            if (
                _roleService.CheckIfUserIsAdmin(UserId)
                || _roleService.CheckIfUserIsRoadshowFocalPoint(UserId)
            )
            {
                queryModel.PaginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber
                };
                return Ok(await _roadshowService.GetAllEventsForCalendar(queryModel, UserId));
            }

            return BadRequest("You don't have permission to do this action.");
        }

        [HttpGet("specific/{id}")]
        public async Task<IActionResult> GetSpecificRoadshowById(int id)
        {
            if ((await _roleService.CheckIfUserIsNotBuyer(UserId)).IsBuyer)
            {
                var result = await _roadshowService.GetSpecificRoadshowOfferById(id);

                if (result == null)
                    return NotFound("Roadshow not found");

                return Ok(result.Value);
            }
            return BadRequest("You don't have permission to do this action.");
        }
    }
}

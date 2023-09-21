using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using System;
using System.Threading.Tasks;

namespace MMA.WebApi.Shared.Hubs
{
    [Authorize("Bearer")]
    public class NotificationHub : Hub
    {
        private readonly IRoleService _roleService;
        private readonly IOfferRepository _offerRepository;
        public NotificationHub(IRoleService roleService, IOfferRepository offerRepository)
        {
            _roleService = roleService;
            _offerRepository = offerRepository;
        }
        public async Task<int> GetAssignedOffersCount()
        {
            var userId = Context.User.Identity.Name;

            var roles = await _roleService.GetUserRoles(userId);
            if (roles.Contains(Enums.Declares.Roles.Admin) || roles.Contains(Enums.Declares.Roles.AdnocCoordinator))
            {
                return await _offerRepository.GetAssignedOffersCountForAdmin();
            }

            return 0;
        }

        public string GetConnectionId() => Context.ConnectionId;

        // Called when a connection with the hub is created.
        public async override Task OnConnectedAsync()
        {
            var userId = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;

            var roles = await _roleService.GetUserRoles(userId);
            if (roles.Contains(Enums.Declares.Roles.Admin) || roles.Contains(Enums.Declares.Roles.AdnocCoordinator))
            {
                await Groups.AddToGroupAsync(connectionId, Enums.Declares.Roles.Admin.ToString());
            }

            await Groups.AddToGroupAsync(connectionId, userId);
            await Task.CompletedTask;
        }

        // Called when a connection with the hub is terminated.
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;

            var roles = await _roleService.GetUserRoles(userId);
            if (roles.Contains(Enums.Declares.Roles.Admin) || roles.Contains(Enums.Declares.Roles.AdnocCoordinator))
            {
                await Groups.RemoveFromGroupAsync(connectionId, Enums.Declares.Roles.Admin.ToString());
            }

            await Groups.RemoveFromGroupAsync(connectionId, userId);
            await Task.FromResult(0);
        }
    }
}

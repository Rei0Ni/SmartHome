using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Hubs;

namespace SmartHome.Application.Hubs
{
    [Authorize]
    public class OverviewHub : Hub
    {
        private readonly IDashboardService _dashboardService;
        private readonly IHubState _hubState;

        public OverviewHub(IDashboardService dashboardService, IHubState hubState)
        {
            _dashboardService = dashboardService;
            _hubState = hubState;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _hubState.AddConnectedUser(userId);
                await SendOverviewUpdateToUser(userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                _hubState.RemoveConnectedUser(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendOverviewUpdateToUser(string userId)
        {
            try
            {
                var overviewData = await _dashboardService.GetDashboardOverview(userId);
                await Clients.Group(userId).SendAsync("ReceiveOverviewData", overviewData);
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending overview update to user {userId}: {ex.Message}");
            }
        }

        // Method to send overview update to all connected users
        public async Task SendOverviewUpdateToAll()
        {
            List<string> currentConnectedUserIds = _hubState.GetConnectedUsers();
            foreach (var userId in currentConnectedUserIds)
            {
                await SendOverviewUpdateToUser(userId);
            }
        }
    }
}
using SmartHome.Dto.Dashboard;

namespace SmartHome.Application.Interfaces.Hubs
{
    public interface IOverviewClient
    {
        Task ReceiveOverviewData(OverviewDto overviewData);
        Task ReceiveRealTimeData(string areaId, object data);
        Task SendOverviewUpdateToAll();
    }
}
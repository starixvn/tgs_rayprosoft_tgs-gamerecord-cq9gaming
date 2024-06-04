using Hangfire.Dashboard;

namespace Tgs.GameRecord.CQ9Gaming;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
using Tgs.GameRecord.CQ9Gaming.Core.JobApplicationServices;

namespace Tgs.GameRecord.CQ9Gaming.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
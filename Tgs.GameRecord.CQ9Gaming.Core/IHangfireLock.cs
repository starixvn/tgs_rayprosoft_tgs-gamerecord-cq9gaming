namespace Tgs.GameRecord.CQ9Gaming.Core;

public interface IHangfireLock
{
	public Task<bool> LockingAsync(string application, CancellationToken cancellationToken = default);
}
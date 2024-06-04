using Tgs.GameRecord.CQ9Gaming.Core.ApplicationServices;

namespace Tgs.GameRecord.CQ9Gaming.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
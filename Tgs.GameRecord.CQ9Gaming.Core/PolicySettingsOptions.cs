namespace Tgs.GameRecord.CQ9Gaming.Core;

public record PolicySettingsOptions
{
	public int RetryCount { get; init; } = 3;
	public int RetrySeconds { get; init; } = 2;
}

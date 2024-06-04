namespace Tgs.GameRecord.CQ9Gaming.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderConfiguration { get; init; } = default!;
	public string HangfirePrefix { get; set; } = "gamerecord-cq9-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-cq9-locking:";
	public double IntervalMinutes { get; init; } = 10;

	public double IntervalMinutesMax { get; init; } = 1440;

	public double IntervalMinutesMinimum { get; init; } = 1;
}
namespace Tgs.GameRecord.CQ9Gaming.Core.ApplicationServices;
public record PullRecordCommand(
	Uri ApiUrl,
	string ApiToken,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
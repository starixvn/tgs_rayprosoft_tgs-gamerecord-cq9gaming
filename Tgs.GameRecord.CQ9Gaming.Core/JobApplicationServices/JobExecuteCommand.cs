namespace Tgs.GameRecord.CQ9Gaming.Core.JobApplicationServices;

public record JobExecuteCommand(
	Uri ApiUrl,
	string ApiToken);
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.CQ9Gaming.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var betState = (command.GameRecord.WinLoss.ToDecimal().GetValueOrDefault(0) > 0) ? Handle.Common.Protos.BetState.Win : Handle.Common.Protos.BetState.Loss;
		var freeGameState = command.GameRecord.IsFreegame ? Handle.Common.Protos.FreeGameState.FreeGame : Handle.Common.Protos.FreeGameState.None;

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.GameRecord.PlayerCode,
			ExternalRoundId = command.GameRecord.ExternalRoundId,
			ExternalBetId = command.GameRecord.ExternalBetId,
			ExternalGameId = command.GameRecord.ExternalGameId,
			ExternalCreatedOn = command.GameRecord.ExternalCreatedOn,
			ExternalClosedOn = command.GameRecord.ExternalClosedOn,
			ExternalPayoffOn = command.GameRecord.ExternalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = freeGameState,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = Handle.Common.Protos.CashoutState.None,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = command.GameRecord.Bet.ToDecimal().GetValueOrDefault(0),
			ValidBet = command.GameRecord.ValidBet.ToDecimal().GetValueOrDefault(0),
			Settle = command.GameRecord.Settle.ToDecimal().GetValueOrDefault(0),
			Commission = 0.0m,
			Turnover = command.GameRecord.ValidBet.ToDecimal().GetValueOrDefault(0),
			WinLoss = command.GameRecord.WinLoss.ToDecimal().GetValueOrDefault(0),
			Jackpot = command.GameRecord.Jackpot.ToDecimal().GetValueOrDefault(0),
			AdditionalData = command.GameRecord.AdditionalData
		};

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
				.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
				DateTime.Now.ToString("o"),
				Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
				new
				{
					request.GameProviderCode,
					request.PlayerCode,
					request.ExternalRoundId,
					request.ExternalBetId,
					request.ExternalGameId,
					ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
					ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
					request.BetState
				});
	}
}
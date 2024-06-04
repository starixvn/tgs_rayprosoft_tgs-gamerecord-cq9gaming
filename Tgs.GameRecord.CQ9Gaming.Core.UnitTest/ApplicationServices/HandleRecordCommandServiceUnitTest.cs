using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.CQ9Gaming.Protos;
using Tgs.GameRecord.CQ9Gaming.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.CQ9Gaming.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "CQ9",
			GameProviderConfiguration = "CQ9Configuration",
			HangfirePrefix = "gamerecord-cq9-hangfire:",
			HangfireLocking = "gamerecord-cq9-locking:",
			IntervalMinutes = 10d,
			IntervalMinutesMax = 60d,
			IntervalMinutesMinimum = 1d
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordCommand(
			GameRecord: new GameRecordInfo
			{
				PlayerCode = "4ucmqplxb029",
				ExternalRoundId = "10014289",
				ExternalBetId = "GINKGO240528220000007",
				ExternalGameId = "GINKGO01",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-05-28T22:48:03.802-04:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-05-28T22:48:20.441-04:00").ToTimestamp(),
				IsFreegame = false,
				Bet = 10.0m,
				ValidBet = 10.0m,
				Settle = 20.0m,
				WinLoss = 10.0m,
				Jackpot = 0.0M,
				AdditionalData = new JsonObject
				{
					["gamehall"] = "cq9",
					["gametype"] = "live",
					["gameplat"] = "web",
					["gamecode"] = "GINKGO01",
					["account"] = "4ucmqplxb029",
					["round"] = "GINKGO240528220000007",
					["balance"] = 149,
					["win"] = 10,
					["bet"] = 10,
					["validbet"] = 10,
					["jackpot"] = 0,
					["jackpotcontribution"] = new JsonArray(),
					["jackpottype"] = "",
					["status"] = "complete",
					["endroundtime"] = "2024-05-28T22:48:20.441-04:00",
					["createtime"] = "2024-05-28T22:48:20.401-04:00",
					["bettime"] = "2024-05-28T22:48:03.802-04:00",
					["detail"] = new JsonArray(),
					["singlerowbet"] = false,
					["gamerole"] = "player",
					["bankertype"] = "pc",
					["rake"] = 0,
					["roomfee"] = 0,
					["tabletype"] = 1,
					["tableid"] = "AET02",
					["roundnumber"] = "10014289",
					["bettype"] = new JsonArray { "2" },
					["gameresult"] = new JsonObject
					{
						["points"] = new JsonArray { "2", "8" },
						["cards"] = new JsonArray{
							new JsonObject
							{
								["poker"] = "H13",
								["tag"] = 2,
							},
							new JsonObject
							{
								["poker"] = "D3",
								["tag"] = 1,
							},
							new JsonObject
							{
								["poker"] = "D8",
								["tag"] = 2,
							},
							new JsonObject
							{
								["poker"] = "C9",
								["tag"] = 1,
							}
						},
					},
					["points"] = new JsonArray{
						new JsonObject{
							["bettype"] = "2",
							["win"] = 20,
							["winlose"] = 10,
							["bet"] = 10,
							["validbet"] = 10,
							["odds"] = 1,
							["matchresult"] = "2",
						}
					},
					["currency"] = "CNY",
					["donate"] = 0,
					["isdonate"] = false,
				}.ToJsonString()
			});

		var service = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
				.PublishAsync(
				subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
				data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
				{
					GameProviderCode = gameRecordInfo.Value.GameProviderCode,
					PlayerCode = command.GameRecord.PlayerCode,
					ExternalRoundId = command.GameRecord.ExternalRoundId,
					ExternalBetId = command.GameRecord.ExternalBetId,
					ExternalGameId = command.GameRecord.ExternalGameId,
					ExternalCreatedOn = command.GameRecord.ExternalCreatedOn,
					ExternalClosedOn = command.GameRecord.ExternalClosedOn,
					ExternalPayoffOn = command.GameRecord.ExternalClosedOn,
					ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
					BetState = Handle.Common.Protos.BetState.Win,
					BetType = Handle.Common.Protos.BetType.Normal,
					FreeGameState = Handle.Common.Protos.FreeGameState.None,
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
				}),
				cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Freegame()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "CQ9",
			GameProviderConfiguration = "CQ9Configuration",
			HangfirePrefix = "gamerecord-cq9-hangfire:",
			HangfireLocking = "gamerecord-cq9-locking:",
			IntervalMinutes = 10d,
			IntervalMinutesMax = 60d,
			IntervalMinutesMinimum = 1d
		});
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordCommand(
			GameRecord: new GameRecordInfo
			{
				PlayerCode = "4ucmqplxb029",
				ExternalRoundId = "6230452578",
				ExternalBetId = "6230452578",
				ExternalGameId = "160",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-05-29T02:37:50.907-04:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-05-29T02:39:40.143-04:00").ToTimestamp(),
				IsFreegame = true,
				Bet = 0.76m,
				ValidBet = 0.76m,
				Settle = 192.4m,
				WinLoss = 191.64m,
				Jackpot = 0.0M,
				AdditionalData = new JsonObject
				{
					["gamehall"] = "cq9",
					["gametype"] = "slot",
					["gameplat"] = "web",
					["gamecode"] = "160",
					["account"] = "4ucmqplxb029",
					["round"] = "6230414220",
					["balance"] = 1142.98,
					["win"] = 192.4,
					["bet"] = 0.76,
					["validbet"] = 0,
					["jackpot"] = 0,
					["jackpotcontribution"] = new JsonArray(),
					["jackpottype"] = "",
					["status"] = "complete",
					["endroundtime"] = "2024-05-29T02:39:40.143-04:00",
					["createtime"] = "2024-05-29T02:39:40.099-04:00",
					["bettime"] = "2024-05-29T02:37:50.907-04:00",
					["detail"] = new JsonArray{
						new JsonObject{
							["freegame"] = 18,
						},
						new JsonObject{
							[ "luckydraw"] = 0,
						},
						new JsonObject{
							[ "bonus"] = 0,
						}
					},
					["singlerowbet"] = false,
					["gamerole"] = "",
					["bankertype"] = "",
					["rake"] = 0,
					["roomfee"] = 0,
					["tabletype"] = "",
					["tableid"] = "",
					["roundnumber"] = "",
					["bettype"] = new JsonArray(),
					["gameresult"] = new JsonObject
					{
						["points"] = new JsonArray(),
						["cards"] = new JsonArray()
					},
					["currency"] = "CNY",
					["donate"] = 0,
					["isdonate"] = false,
				}.ToJsonString()
			});

		var service = new HandleRecordCommandService(
			faceLogger,
			gameRecordInfo,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
				.PublishAsync(
				subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
				data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
				{
					GameProviderCode = gameRecordInfo.Value.GameProviderCode,
					PlayerCode = command.GameRecord.PlayerCode,
					ExternalRoundId = command.GameRecord.ExternalRoundId,
					ExternalBetId = command.GameRecord.ExternalBetId,
					ExternalGameId = command.GameRecord.ExternalGameId,
					ExternalCreatedOn = command.GameRecord.ExternalCreatedOn,
					ExternalClosedOn = command.GameRecord.ExternalClosedOn,
					ExternalPayoffOn = command.GameRecord.ExternalClosedOn,
					ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
					BetState = Handle.Common.Protos.BetState.Win,
					BetType = Handle.Common.Protos.BetType.Normal,
					FreeGameState = Handle.Common.Protos.FreeGameState.FreeGame,
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
				}),
				cancellationToken: Arg.Any<CancellationToken>());
	}
}
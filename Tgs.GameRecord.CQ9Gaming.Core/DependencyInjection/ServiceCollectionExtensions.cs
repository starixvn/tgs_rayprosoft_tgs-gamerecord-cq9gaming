using System.Reflection;

using Tgs.GameCommand.Client;
using Tgs.GameInterface.CQ9Gaming.Client;
using Tgs.GameRecord.CQ9Gaming.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordCore(
		this IServiceCollection services,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceCQ9GamingClientOptions, IServiceProvider> gameInterfaceCQ9GamingClientOptions)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddGameCommandClient(gameCommandClientOptions)
		.AddGameInterfaceCQ9GamingClient(gameInterfaceCQ9GamingClientOptions)
		.AddSingleton(TimeProvider.System)
		.AddSingleton<IPullRecordDispatcher, PullRecordDispatcher>()
		.AddTransient<IHangfireUtensil, HangfireUtensil>();
}
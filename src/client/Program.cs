using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OwlDomain.Owlish;

public static class Program
{
	#region Functions
	public static async Task Main()
	{
		HostApplicationBuilderSettings settings = new()
		{
			ApplicationName = "owlish",
			DisableDefaults = true,
			ContentRootPath = AppContext.BaseDirectory,
#if DEBUG
			EnvironmentName = Environments.Development,
#endif
		};

		HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings);
		builder.Services.AddHostedService<OwlishWorker>();

		IHost host = builder.Build();
		try
		{
			await host.StartAsync();
		}
		finally
		{
			await host.StopAsync();
		}
	}
	#endregion
}

class OwlishWorker : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		Console.WriteLine("Welcome to owlish.");
		return Task.CompletedTask;
	}
}

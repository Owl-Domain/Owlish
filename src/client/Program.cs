using Microsoft.Extensions.Configuration;
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
			ApplicationName = "Owlish",
			DisableDefaults = true,
			ContentRootPath = AppContext.BaseDirectory,
#if DEBUG
			EnvironmentName = Environments.Development,
#endif
		};

		HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings);
		builder.Services.AddHostedService<OwlishWorker>();

		IHost host = builder.Build();

		await host.StartAsync();
		await host.WaitForShutdownAsync();
	}
	#endregion
}

class OwlishWorker(IHostApplicationLifetime lifetime, IConfiguration configuration) : BackgroundService
{
	#region Nested types
	private readonly record struct Position(int Left, int Top)
	{
		#region Properties
		public bool IsStartOfLine => Left is 0;
		#endregion

		#region Functions
		public static Position GetCurrent()
		{
			(int left, int top) = Console.GetCursorPosition();
			return new(left, top);
		}
		#endregion

		#region Methods
		public void Set() => Console.SetCursorPosition(Left, Top);
		#endregion
	}
	#endregion

	#region Properties
	private Position PromptStart { get; set; }
	#endregion

	#region Lifetime methods
	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		await EnsureStateAsync(cancellationToken);

		string? appName = configuration[nameof(HostApplicationBuilderSettings.ApplicationName)];
		Console.WriteLine($"Welcome to {appName ?? "Owlish"}.");

		await LoopAsync(cancellationToken);
	}
	private async Task LoopAsync(CancellationToken cancellationToken)
	{
		await DrawPromptAsync(cancellationToken);
		while (cancellationToken.IsCancellationRequested is false)
		{
			if (await WaitForInputAsync(cancellationToken) is false)
				return;

			ConsoleKeyInfo key = Console.ReadKey(true);
			await HandleInputAsync(key, cancellationToken);
			await RedrawPromptAsync(cancellationToken);
		}
	}
	#endregion

	#region Methods
	private Task EnsureStateAsync(CancellationToken cancellationToken)
	{
		if (Position.GetCurrent().IsStartOfLine is false)
			Console.WriteLine();

		// Note(Nightowl): Initial printing shouldn't show the cursor;
		Console.CursorVisible = false;

		return Task.CompletedTask;
	}
	private Task DrawPromptAsync(CancellationToken cancellationToken)
	{
		PromptStart = Position.GetCurrent();
		Console.Write($"{DateTime.Now} > ");

		Console.CursorVisible = true;

		return Task.CompletedTask;
	}
	private async Task HandleInputAsync(ConsoleKeyInfo key, CancellationToken cancellationToken)
	{
		if (key.Modifiers is ConsoleModifiers.Control && key.Key is ConsoleKey.C)
		{
			lifetime.StopApplication();
			return;
		}
	}
	#endregion

	#region Helpers
	private async Task RedrawPromptAsync(CancellationToken cancellationToken)
	{
		Console.CursorVisible = false;
		try
		{
			PromptStart.Set();
			await DrawPromptAsync(cancellationToken);
		}
		finally
		{
			Console.CursorVisible = true;
		}
	}
	private async Task<bool> WaitForInputAsync(CancellationToken cancellationToken)
	{
		while (Console.KeyAvailable is false)
		{
			if (cancellationToken.IsCancellationRequested)
				return false;

			await Task.Delay(50);
		}

		return true;
	}
	#endregion
}

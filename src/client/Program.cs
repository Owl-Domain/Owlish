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
	private List<char> InputBuffer { get; } = [];
	private int InputIndex { get; set; } = 0;
	private Position PromptStart { get; set; }
	private Position CaretPosition { get; set; }
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
		// Note(Nightowl): Initial printing shouldn't show the cursor;
		Console.CursorVisible = false;

		if (Position.GetCurrent().IsStartOfLine is false)
			Console.WriteLine();

		InputBuffer.Clear();
		InputIndex = 0;

		return Task.CompletedTask;
	}
	private Task DrawPromptAsync(CancellationToken cancellationToken)
	{
		PromptStart = Position.GetCurrent();
		Console.Write($"{DateTime.Now} > ");

		for (int i = 0; i < InputIndex; i++)
			Console.Write(InputBuffer[i]);

		CaretPosition = Position.GetCurrent();

		for (int i = InputIndex; i < InputBuffer.Count; i++)
			Console.Write(InputBuffer[i]);

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

		if (key.Key is ConsoleKey.Enter)
		{
			// ensure potential output starts on a new line.
			Console.WriteLine();

			string input = new(InputBuffer.ToArray());
			await ExecuteAsync(input, cancellationToken);
			await EnsureStateAsync(cancellationToken);

			return;
		}

		if (char.IsControl(key.KeyChar))
			return;

		InputBuffer.Insert(InputIndex, key.KeyChar);
		InputIndex++;
	}
	private async Task ExecuteAsync(string input, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(input))
			return;

		string[] parts = input.Split();
		string command = parts[0]; // One part is always guaranteed.
		string[] args = parts.Skip(1).ToArray();

		ProcessStartInfo startInfo = new(command, args)
		{
			// Note(Nightowl): Pretty sure this happens anyway but let's be explicit about it;
			WorkingDirectory = Environment.CurrentDirectory
		};

		Process? process = null;
		try
		{
			process = Process.Start(startInfo);
			if (process is null)
			{
				// Todo(Nightowl): Fail somehow?;
				return;
			}

			await process.WaitForExitAsync(cancellationToken);
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine(exception.Message);
		}
		finally
		{
			if (process?.ExitCode is not 0 and not null)
			{
				Console.Error.WriteLine($"Processes failed with code: {process.ExitCode}");
			}
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

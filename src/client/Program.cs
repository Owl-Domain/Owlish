using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OwlDomain.Owlish.Engine.Input;

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
	private ConsoleTextInput ConsoleInput { get; } = new(new TextInput(), new ConsoleKeyConfiguration());
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

			bool needsRedraw = false;

			while (Console.KeyAvailable)
			{
				// Note(Nightowl):
				// This seems to work properly and it doesn't starve the prompt drawing even if
				// I hold down a single key, I'm hoping this isn't just a thing for me though;
				ConsoleKeyInfo key = Console.ReadKey(true);
				TextInputResult result = ConsoleInput.Handle(key);

				if (result is TextInputResult.Complete)
				{
					/// Ensure prompt and input is fully drawn before we get potential output.
					await RedrawPromptAsync(cancellationToken);

					// ensure potential output starts on a new line.
					Console.WriteLine();
					string input = ConsoleInput.Input.ToString();

					await EnsureStateAsync(cancellationToken);

					if (string.IsNullOrWhiteSpace(input) is false)
						await ExecuteAsync(input, cancellationToken);

					needsRedraw = true;
					break;
				}

				if (result is TextInputResult.Exit)
				{
					lifetime.StopApplication();
					return;
				}

				if (result is not TextInputResult.None)
					needsRedraw = true;
			}

			if (needsRedraw)
				await RedrawPromptAsync(cancellationToken);
		}
	}
	#endregion

	#region Methods
	private Task EnsureStateAsync(CancellationToken cancellationToken)
	{
		// Note(Nightowl): Skip everything before actual input, prevents accidentally sending a command without knowing;
		while (Console.KeyAvailable)
		{
			if (cancellationToken.IsCancellationRequested)
				return Task.CompletedTask;

			_ = Console.ReadKey(true);
		}

		if (Position.GetCurrent().IsStartOfLine is false)
			Console.WriteLine();

		ConsoleInput.Input.Reset();

		return Task.CompletedTask;
	}
	private Task DrawPromptAsync(CancellationToken cancellationToken)
	{
		Console.CursorVisible = false;

		try
		{
			PromptStart = Position.GetCurrent();
			Console.Write($"{DateTime.Now} > ");

			for (int i = 0; i < ConsoleInput.Input.Position; i++)
				Console.Write(ConsoleInput.Input[i]);

			Position caret = Position.GetCurrent();

			for (int i = ConsoleInput.Input.Position; i < ConsoleInput.Input.Length; i++)
				Console.Write(ConsoleInput.Input[i]);

			// Note(Nightowl): VT100 code for clearing from the caret position until the end of the display;
			Console.Write("\e[0J");

			caret.Set();
		}
		finally
		{
			Console.CursorVisible = true;
		}

		return Task.CompletedTask;
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

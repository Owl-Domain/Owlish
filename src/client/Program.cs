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
		Console.WriteLine($"Welcome to {GetAppName()}.");

		Console.TreatControlCAsInput = true;
		try
		{
			await LoopAsync(cancellationToken);
		}
		finally
		{
			Console.TreatControlCAsInput = false;
		}
	}
	private async Task LoopAsync(CancellationToken cancellationToken)
	{
		while (cancellationToken.IsCancellationRequested is false)
		{
			string? result = await HandleSingleIterationAsync(cancellationToken);
			if (result is null)
				return;

			if (result.IsWhiteSpace())
				continue;

			// Note(Nightowl):
			// This seems to be required to make sure the app receives the SIG-INT signal,
			// however this also seems to cancel this app as well because of the hosting lifetime;
			Console.TreatControlCAsInput = false;
			try
			{
				Console.WriteLine(); // Note(Nightowl): Ensure output begins at start of line;
				ClearUntilEnd(); // Note(Nightowl): Ensure there's no existing text after the prompt;

				await ExecuteAsync(result, cancellationToken);
			}
			finally
			{
				Console.TreatControlCAsInput = true;
			}
		}
	}
	#endregion

	#region Methods
	private async Task<string?> HandleSingleIterationAsync(CancellationToken cancellationToken)
	{
		await EnsureStateAsync(cancellationToken);
		await DrawPromptAsync(cancellationToken);

		List<ConsoleKeyInfo> toHandle = [];
		while (true) // Cancellation is handled during waiting for input to keep it simpler.
		{
			toHandle.Clear();

			if (await WaitForInputAsync(cancellationToken) is false)
				return null; // Operation was cancelled during waiting for input.

			// Note(Nightowl):
			// Batch input keys in preparation for the future, to simplify pasting so we can handle
			// line breaks and treat them properly without accidentally running the command too early
			//
			// This might have to ensure there's no refresh starving while holding down a key,
			// but for me it worked perfectly fine;
			while (Console.KeyAvailable)
			{
				ConsoleKeyInfo key = Console.ReadKey(true);
				toHandle.Add(key);
			}

			for (int i = 0; i < toHandle.Count; i++)
			{
				ConsoleKeyInfo key = toHandle[i];
				TextInputResult result = ConsoleInput.Handle(key);

				if (result is TextInputResult.Exit)
				{
					lifetime.StopApplication();
					return null;
				}

				if (result is TextInputResult.Complete)
				{
					if (i < toHandle.Count - 1)
						continue; // Note(Nightowl): This is where we'd be handling line breaks from pasting;

					return ConsoleInput.Input.ToString();
				}

				if (result is not TextInputResult.None)
					await RedrawPromptAsync(cancellationToken);
			}
		}
	}
	private Task EnsureStateAsync(CancellationToken cancellationToken)
	{
		Console.Title = GetAppName();

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

			ClearUntilEnd();

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
				// Todo(Nightowl): Fail somehow?
				return;
			}

			await WaitForProcessAsync(process, cancellationToken);
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine(exception.Message);
		}
		finally
		{
			if (process?.ExitCode is not 0 and not null)
				Console.Error.WriteLine($"Processes failed with code: {process.ExitCode}");
		}
	}
	private async Task WaitForProcessAsync(Process process, CancellationToken appToken)
	{
		while (process.HasExited is false)
		{
			if (appToken.IsCancellationRequested)
			{
				process.Kill();
				return;
			}

			try
			{
				await process.WaitForExitAsync(appToken);
			}
			catch (OperationCanceledException) { /* Will hit the check at the start of the loop. */ }
		}
	}
	#endregion

	#region Helpers
	private void ClearUntilEnd()
	{
		// Note(Nightowl): VT100 code for clearing from the caret position until the end of the display;
		Console.Write("\e[0J");
	}
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
	private string GetAppName()
	{
		string? appName = configuration[nameof(HostApplicationBuilderSettings.ApplicationName)];
		return appName ?? "Owlish";
	}
	#endregion
}

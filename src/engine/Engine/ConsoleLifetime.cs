// Most of this implementation has been taken from the Microsoft.Extensions.Hosting.ConsoleLifetime
// class which is licensed under the MIT license.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace OwlDomain.Owlish.Engine;

/// <summary>
/// 	An application lifetime that listens for Ctrl+C or SIGTERM and initiates shutdown when appropriate.
/// </summary>
public sealed class ConsoleLifetime : IHostLifetime, IDisposable
{
	#region Fields
	private CancellationTokenRegistration _applicationStartedRegistration;
	private CancellationTokenRegistration _applicationStoppingRegistration;

	private PosixSignalRegistration? _sigIntRegistration;
	private PosixSignalRegistration? _sigQuitRegistration;
	private PosixSignalRegistration? _sigTermRegistration;
	#endregion

	#region Properties
	private ConsoleLifetimeOptions Options { get; }
	private IHostEnvironment Environment { get; }
	private IHostApplicationLifetime ApplicationLifetime { get; }
	private HostOptions HostOptions { get; }
	private ILogger Logger { get; }
	#endregion

	#region Constructors
	/// <summary>
	/// Initializes a <see cref="ConsoleLifetime"/> instance using the specified console lifetime options, host environment, host application lifetime, and host options.
	/// </summary>
	/// <param name="options">An object used to retrieve <see cref="ConsoleLifetimeOptions"/> instances.</param>
	/// <param name="environment">Information about the hosting environment an application is running in.</param>
	/// <param name="applicationLifetime">An object that allows consumers to be notified of application lifetime events.</param>
	/// <param name="hostOptions">An object used to retrieve internal host options instances.</param>
	/// <exception cref="ArgumentNullException"><paramref name="options"/> or <paramref name="environment"/> or <paramref name="applicationLifetime"/> or <paramref name="hostOptions"/> is <see langword="null"/>.</exception>
	public ConsoleLifetime(IOptions<ConsoleLifetimeOptions> options, IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, IOptions<HostOptions> hostOptions)
		 : this(options, environment, applicationLifetime, hostOptions, NullLoggerFactory.Instance) { }

	/// <summary>
	/// Initializes a <see cref="ConsoleLifetime"/> instance using the specified console lifetime options, host environment, host options, and logger factory.
	/// </summary>
	/// <param name="options">An object used to retrieve <see cref="ConsoleLifetimeOptions"/> instances.</param>
	/// <param name="environment">Information about the hosting environment an application is running in.</param>
	/// <param name="applicationLifetime">An object that allows consumers to be notified of application lifetime events.</param>
	/// <param name="hostOptions">An object used to retrieve <see cref="HostOptions"/> instances.</param>
	/// <param name="loggerFactory">An object to configure the logging system and create instances of <see cref="ILogger"/> from the registered <see cref="ILoggerProvider"/>.</param>
	/// <exception cref="ArgumentNullException"><paramref name="options"/> or <paramref name="environment"/> or <paramref name="applicationLifetime"/> or <paramref name="hostOptions"/> or <paramref name="loggerFactory"/> is <see langword="null"/>.</exception>
	public ConsoleLifetime(IOptions<ConsoleLifetimeOptions> options, IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, IOptions<HostOptions> hostOptions, ILoggerFactory loggerFactory)
	{
		ArgumentNullException.ThrowIfNull(options?.Value, nameof(options));
		ArgumentNullException.ThrowIfNull(applicationLifetime);
		ArgumentNullException.ThrowIfNull(environment);
		ArgumentNullException.ThrowIfNull(hostOptions?.Value, nameof(hostOptions));
		ArgumentNullException.ThrowIfNull(loggerFactory);

		Options = options.Value;
		Environment = environment;
		ApplicationLifetime = applicationLifetime;
		HostOptions = hostOptions.Value;
		Logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
	}
	#endregion

	#region Methods
	/// <summary>
	/// Registers the application start, application stop and shutdown handlers for this application.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
	/// <returns>A <see cref="Task"/> that represents the asynchronous registration operation.</returns>
	public Task WaitForStartAsync(CancellationToken cancellationToken)
	{
		if (!Options.SuppressStatusMessages)
		{
			_applicationStartedRegistration = ApplicationLifetime.ApplicationStarted.Register(state =>
			{
				((ConsoleLifetime)state!).OnApplicationStarted();
			},
			this);
			_applicationStoppingRegistration = ApplicationLifetime.ApplicationStopping.Register(state =>
			{
				((ConsoleLifetime)state!).OnApplicationStopping();
			},
			this);
		}

		RegisterShutdownHandlers();

		// Console applications start immediately.
		return Task.CompletedTask;
	}
	private void OnApplicationStarted()
	{
		if (Logger.IsEnabled(LogLevel.Information))
		{
			Logger.LogInformation("Application started. Press Ctrl+C to shut down.");
			Logger.LogInformation("Hosting environment: {EnvName}", Environment.EnvironmentName);
			Logger.LogInformation("Content root path: {ContentRoot}", Environment.ContentRootPath);
		}
	}

	/// <summary>
	/// This method does nothing.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token instance.</param>
	/// <returns>A <see cref="Task"/> that represents a completed task.</returns>
	public Task StopAsync(CancellationToken cancellationToken)
	{
		// There's nothing to do here
		return Task.CompletedTask;
	}
	private void OnApplicationStopping()
	{
		Logger.LogInformation("Application is shutting down...");
	}

	/// <summary>
	/// Unregisters the shutdown handlers and disposes the application start and application stop registrations.
	/// </summary>
	public void Dispose()
	{
		UnregisterShutdownHandlers();

		_applicationStartedRegistration.Dispose();
		_applicationStoppingRegistration.Dispose();
	}
	#endregion

	#region Helpers
	private void HandlePosixSignal(PosixSignalContext context)
	{
		Debug.Assert(context.Signal == PosixSignal.SIGINT || context.Signal == PosixSignal.SIGQUIT || context.Signal == PosixSignal.SIGTERM);

		context.Cancel = true;
		ApplicationLifetime.StopApplication();
	}
	private void HandleWindowsShutdown(PosixSignalContext context)
	{
		// for SIGTERM on Windows we must block this thread until the application is finished
		// otherwise the process will be killed immediately on return from this handler

		// don't allow Dispose to unregister handlers, since Windows has a lock that prevents the unregistration while this handler is running
		// just leak these, since the process is exiting
		_sigIntRegistration = null;
		_sigQuitRegistration = null;
		_sigTermRegistration = null;

		ApplicationLifetime.StopApplication();

		// We could wait for a signal here, like Dispose as is done in non-netcoreapp case, but those inevitably could have user
		// code that runs after them in the user's Main. Instead we just block this thread completely and let the main routine exit.
		Thread.Sleep(HostOptions.ShutdownTimeout);
	}
	private void RegisterShutdownHandlers()
	{
		if (!OperatingSystem.IsWasi())
		{
			Action<PosixSignalContext> handler = HandlePosixSignal;
			_sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);
			_sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);
			_sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, OperatingSystem.IsWindows() ? HandleWindowsShutdown : handler);
		}
	}
	private void UnregisterShutdownHandlers()
	{
		_sigIntRegistration?.Dispose();
		_sigQuitRegistration?.Dispose();
		_sigTermRegistration?.Dispose();
	}
	#endregion
}

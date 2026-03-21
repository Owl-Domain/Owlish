namespace OwlDomain.Owlish.Engine.Execution;

/// <summary>
/// 	Represents a service for controlling the execution of child processes.
/// </summary>
public sealed class ExecutionService : IExecutionService
{
	#region Fields
	private readonly List<Process> _active = [];
	private readonly List<Process> _main = [];
	private readonly ReaderWriterLockSlim _lock = new();
	#endregion

	#region Properties
	/// <inheritdoc/>
	public IReadOnlyCollection<Process> Active
	{
		get
		{
			// Note(Nightowl): Wasteful but simpler so I don't care right now;
			using (_lock.ReadLock())
				return _active.ToArray();
		}
	}

	/// <inheritdoc/>
	public IReadOnlyCollection<Process> Main
	{
		get
		{
			// Note(Nightowl): Wasteful but simpler so I don't care right now;
			using (_lock.ReadLock())
				return _main.ToArray();
		}
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public Process Execute(ProcessStartInfo startInfo, bool isMain)
	{
		Process? process = Process.Start(startInfo);
		if (process is null)
			ThrowHelper.ThrowArgumentException(nameof(startInfo), "The process couldn't be executed, something was probably wrong with the start info.");

		Debug.WriteLine($"Executed process '{startInfo.FileName}' PID: {process.Id}.");

		void Handler(object? sender, EventArgs args)
		{
			Debug.WriteLine($"Execution process stopped PID: {process.Id}.");
			process.Exited -= Handler;

			using (_lock.WriteLock())
			{
				_active.Remove(process);

				if (isMain)
					_main.Remove(process);
			}
		}

		process.Exited += Handler;
		process.EnableRaisingEvents = true;

		if (process.HasExited is false)
		{
			using (_lock.WriteLock())
			{
				_active.Add(process);

				if (isMain)
					_main.Add(process);
			}
		}

		return process;
	}
	#endregion
}

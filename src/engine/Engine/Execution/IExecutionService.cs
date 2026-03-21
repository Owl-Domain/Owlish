namespace OwlDomain.Owlish.Engine.Execution;

/// <summary>
/// 	Represents a service for controlling the execution of child processes.
/// </summary>
public interface IExecutionService
{
	#region Properties
	/// <summary>The collection of the active processes.</summary>
	IReadOnlyCollection<Process> Active { get; }

	/// <summary>The main processes that are being executed.</summary>
	/// <remarks>This represents the processes being ran directly by the user through commands.</remarks>
	IReadOnlyCollection<Process> Main { get; }
	#endregion

	#region Methods
	/// <summary>Executes a new process.</summary>
	/// <param name="startInfo">The information to use to start the process.</param>
	/// <param name="isMain">Whether the started process should be considered as one of the main processes.</param>
	/// <returns>The started process.</returns>
	/// <remarks>
	/// 	This method will not perform any validation if the process fails to start,
	/// 	and instead it'll forward the exceptions to the caller to handle.
	/// </remarks>
	Process Execute(ProcessStartInfo startInfo, bool isMain);
	#endregion
}

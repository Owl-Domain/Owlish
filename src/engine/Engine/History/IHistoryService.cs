namespace OwlDomain.Owlish.Engine.History;

/// <summary>
/// 	Represents a service used for controlling shell command history.
/// </summary>
public interface IHistoryService
{
	#region Properties
	/// <summary>The items in the history.</summary>
	/// <remarks>The last item is the latest one.</remarks>
	IReadOnlyList<string> Items { get; }
	#endregion

	#region Methods
	/// <summary>Adds the given <paramref name="input"/> to the shell command history.</summary>
	/// <param name="input"></param>
	void Add(string input);
	#endregion
}

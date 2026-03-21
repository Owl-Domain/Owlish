namespace OwlDomain.Owlish.Engine.History;

/// <summary>
/// 	Represents a service used for controlling shell command history.
/// </summary>
public class HistoryService : IHistoryService
{
	#region Constants
	private const int MaximumItems = 100;
	#endregion

	#region Fields
	private readonly List<string> _items = [];
	#endregion

	#region Properties
	/// <inheritdoc/>
	public IReadOnlyList<string> Items => _items; // Note(Nightowl): Locking probably isn't necessary, at least not yet;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Add(string input)
	{
		// Note(Nightowl): Don't add duplicate items when they're consecutive;
		if (string.IsNullOrWhiteSpace(input) || _items.LastOrDefault() == input)
			return;

		if (_items.Count >= MaximumItems)
			_items.RemoveRange(0, MaximumItems - _items.Count + 1);

		_items.Add(input);
	}
	#endregion
}

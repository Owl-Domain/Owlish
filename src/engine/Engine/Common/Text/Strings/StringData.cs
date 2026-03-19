namespace OwlDomain.Owlish.Engine.Common.Text.Strings;

/// <summary>
/// 	Represents information about a string made up of individual fragments.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class StringData : IReadOnlyList<IStringFragment>
{
	#region Fields
	private readonly IStringFragment[] _fragments = [];
	#endregion

	#region Properties
	/// <inheritdoc/>
	public int Count => _fragments.Length;
	#endregion

	#region Indexers
	/// <inheritdoc/>
	public IStringFragment this[int index] => _fragments[index];
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="StringData"/>.</summary>
	/// <param name="fragments">The fragments that make up the string.</param>
	public StringData(params IEnumerable<IStringFragment> fragments) => _fragments = fragments.ToArray();
	#endregion

	#region Methods
	/// <inheritdoc/>
	public override string ToString() => string.Concat(_fragments);

	/// <inheritdoc/>
	public IEnumerator<IStringFragment> GetEnumerator() => ((IEnumerable<IStringFragment>)_fragments).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	#region Helpers
	private string DebuggerDisplay() => $"{nameof(StringData)} {{ Fragments = ({Count}) }}";
	#endregion
}

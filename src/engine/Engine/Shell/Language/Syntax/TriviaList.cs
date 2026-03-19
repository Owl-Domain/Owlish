namespace OwlDomain.Owlish.Engine.Shell.Language.Syntax;

/// <summary>
/// 	Represents a list of sequential trivia nodes.
/// </summary>
[CollectionBuilder(typeof(TriviaList), nameof(Create))]
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class TriviaList : IReadOnlyList<TriviaNode>
{
	#region Fields
	private readonly TriviaNode[] _nodes;
	#endregion

	#region Properties
	/// <summary>Represents an empty list of trivia nodes.</summary>
	public static TriviaList Empty { get; } = new();

	/// <inheritdoc/>
	public int Count => _nodes.Length;

	/// <summary>Whether the trivia list is empty.</summary>
	public bool IsEmpty => _nodes.Length is 0;

	/// <summary>Gets the position of the trivia nodes.</summary>
	/// <remarks>If there are no trivia nodes, then the default value is returned.</remarks>
	public TextSpan Position => IsEmpty ? default : new(_nodes.First().Position.Start, _nodes.Last().Position.End);

	/// <summary>Gets the position of the trivia nodes.</summary>
	/// <remarks>If there are no trivia nodes, then a <see langword="null"/> value is returned.</remarks>
	public TextSpan? PositionOrNull => IsEmpty ? null : new(_nodes.First().Position.Start, _nodes.Last().Position.End);
	#endregion

	#region Indexers
	/// <inheritdoc/>
	public TriviaNode this[int index] => _nodes[index];
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="TriviaList"/>.</summary>
	/// <param name="nodes">The nodes that will make up the trivia list.</param>
	public TriviaList(params IEnumerable<TriviaNode> nodes)
	{
		_nodes = nodes.ToArray();

		if (_nodes.Length >= 2)
		{
			for (int i = 0; i < _nodes.Length - 1; i++)
			{
				if (_nodes[i].Position.Start > _nodes[i + 1].Position.Start)
					ThrowHelper.ThrowArgumentException(nameof(nodes), "The given nodes were not sorted properly.");
			}
		}
	}

	/// <summary>Creates a new <see cref="TriviaList"/>.</summary>
	/// <param name="nodes">The nodes that will make up the trivia list.</param>
	public TriviaList(params ReadOnlySpan<TriviaNode> nodes)
	{
		if (nodes.Length >= 2)
		{
			for (int i = 0; i < nodes.Length - 1; i++)
			{
				if (nodes[i].Position.Start > nodes[i + 1].Position.Start)
					ThrowHelper.ThrowArgumentException(nameof(nodes), "The given nodes were not sorted properly.");
			}
		}

		_nodes = nodes.ToArray();
	}
	#endregion

	#region Functions
	/// <summary>Creates a new <see cref="TriviaList"/>.</summary>
	/// <param name="nodes">The nodes that will make up the trivia list.</param>
	/// <returns>The created trivia list.</returns>
	public static TriviaList Create(ReadOnlySpan<TriviaNode> nodes) => new(nodes);
	#endregion

	#region Methods
	/// <inheritdoc/>
	public IEnumerator<TriviaNode> GetEnumerator() => ((IEnumerable<TriviaNode>)_nodes).GetEnumerator(); // Note(Nightowl): Yuck;
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	#region Helpers
	private string DebuggerDisplay() => $"{nameof(TriviaList)} {{ Count = ({Count}), Position = ({PositionOrNull}) }}";
	#endregion
}

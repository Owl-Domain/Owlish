namespace OwlDomain.Owlish.Engine.Input;

/// <summary>
///  Represents a manager for handling textual input.
/// </summary>
public class TextInput : ITextInput
{
	#region Fields
	private readonly List<char> _characters = [];
	#endregion

	#region Properties
	/// <inheritdoc/>
	public IReadOnlyList<char> Characters => _characters;

	/// <inheritdoc/>
	public int Position { get; private set; }

	/// <inheritdoc/>
	public bool IsAtStart => Position <= 0;

	/// <inheritdoc/>
	public bool IsAtEnd => Position >= Characters.Count;

	/// <inheritdoc/>
	public bool IsEmpty => _characters.Count is 0;

	/// <inheritdoc/>
	public int Length => _characters.Count;
	#endregion

	#region Indexers
	/// <inheritdoc/>
	public char this[int index] => _characters[index];
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Reset()
	{
		_characters.Clear();
		Position = 0;
	}

	/// <inheritdoc/>
	public override string ToString() => new(_characters.ToArray());
	#endregion

	#region Edit methods
	/// <inheritdoc/>
	public void Add(char character)
	{
		_characters.Insert(Position, character);
		Position++;
	}

	/// <inheritdoc/>
	public bool DeleteBefore()
	{
		if (IsAtStart)
			return false;

		Debug.Assert(Position > 0);
		_characters.RemoveAt(Position - 1);
		Position--;

		return true;
	}

	/// <inheritdoc/>
	public bool DeleteAfter()
	{
		if (IsAtEnd)
			return false;

		_characters.RemoveAt(Position);
		return true;
	}
	#endregion

	#region Movement methods
	/// <inheritdoc/>
	public bool MoveLeft()
	{
		if (IsAtStart)
			return false;

		Position--;
		return true;
	}

	/// <inheritdoc/>
	public bool MoveRight()
	{
		if (IsAtEnd)
			return false;

		Position++;
		return true;
	}

	/// <inheritdoc/>
	public bool MoveToStart()
	{
		if (IsAtStart)
			return false;

		Position = 0;
		return true;
	}

	/// <inheritdoc/>
	public bool MoveToEnd()
	{
		if (IsAtEnd)
			return false;

		Position = Characters.Count;
		return true;
	}
	#endregion
}

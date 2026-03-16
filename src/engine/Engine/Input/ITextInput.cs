namespace OwlDomain.Owlish.Engine.Input;

/// <summary>
///  Represents a manager for handling textual input.
/// </summary>
public interface ITextInput
{
	#region Properties
	/// <summary>The currently typed characters.</summary>
	IReadOnlyList<char> Characters { get; }

	/// <summary>The caret position inside of the <see cref="Characters"/> collection.</summary>
	/// <remarks>This position represents that the caret is <i>before</i> the character with the same index.</remarks>
	int Position { get; }

	/// <summary>Checks whether the <see cref="Position"/> is currently at the start of the input.</summary>
	bool IsAtStart { get; }

	/// <summary>Checks whether the <see cref="Position"/> is currently at the end of the input.</summary>
	bool IsAtEnd { get; }

	/// <summary>Checks whether the text input is empty.</summary>
	bool IsEmpty { get; }

	/// <summary>The length of the text input.</summary>
	int Length { get; }
	#endregion

	#region Indexers
	/// <summary>Gets the character at the given <paramref name="index"/> of the input.</summary>
	/// <param name="index">The index to get the character at.</param>
	/// <returns>The character at the given <paramref name="index"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the given <paramref name="index"/> points outside of the allowed input range.</exception>
	char this[int index] { get; }
	#endregion

	#region Methods
	/// <summary>Resets the state of the text input.</summary>
	void Reset();

	/// <summary>Gets the <see cref="Characters"/> collection as a <see langword="string"/>.</summary>
	string ToString();
	#endregion

	#region Edit methods
	/// <summary>Adds a new <paramref name="character"/> at the current position.</summary>
	/// <param name="character">The character to add.</param>
	void Add(char character);

	/// <summary>Deletes one character before the caret.</summary>
	/// <returns><see langword="true"/> if the character was deleted and the caret was moved, <see langword="false"/> if there was nothing to delete.</returns>
	/// <remarks>This will move the caret one character to the left.</remarks>
	bool DeleteBefore();

	/// <summary>Deletes one character after the caret.</summary>
	/// <returns><see langword="true"/> if the character was deleted and the caret was moved, <see langword="false"/> if there was nothing to delete.</returns>
	/// <remarks>This will keep the caret in its current position.</remarks>
	bool DeleteAfter();
	#endregion

	#region Movement methods
	/// <summary>Moves the caret one space to the left (towards the start).</summary>
	/// <returns><see langword="true"/> if the caret was moved, <see langword="false"/> if it was already at the start.</returns>
	bool MoveLeft();

	/// <summary>Moves the caret one space to the right (towards the end).</summary>
	/// <returns><see langword="true"/> if the caret was moved, <see langword="false"/> if it was already at the end.</returns>
	bool MoveRight();

	/// <summary>Moves the caret to the start.</summary>
	/// <returns><see langword="true"/> if the caret was moved, <see langword="false"/> if it was already at the start.</returns>
	bool MoveToStart();

	/// <summary>Moves the caret to the end.</summary>
	/// <returns><see langword="true"/> if the caret was moved, <see langword="false"/> if it was already at the end.</returns>
	bool MoveToEnd();
	#endregion
}

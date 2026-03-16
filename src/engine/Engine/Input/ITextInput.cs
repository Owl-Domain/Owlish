namespace OwlDomain.Owlish.Engine.Input;

/// <summary>
/// 	Represents the result of the text input handling a key.
/// </summary>
public enum TextInputResult
{
	/// <summary>No difference was made to the input.</summary>
	None,

	/// <summary>The caret has been moved.</summary>
	/// <remarks>If a caret was moved and the text has changed then <see cref="TextChanged"/> will be returned instead.</remarks>
	CaretMoved,

	/// <summary>The text has been changed and needs to be re-rendered.</summary>
	TextChanged,

	/// <summary>The user indicated for the application to stop.</summary>
	Exit,

	/// <summary>The text input has been completed.</summary>
	Complete,
}

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
	#endregion

	#region Methods
	/// <summary>Handles the given <paramref name="key"/> input.</summary>
	/// <param name="key">The key to handle.</param>
	/// <returns>The result of handling the <paramref name="key"/>.</returns>
	TextInputResult Handle(ConsoleKeyInfo key);

	/// <summary>Resets the state of the text input.</summary>
	void Reset();

	/// <summary>Gets the <see cref="Characters"/> collection as a <see langword="string"/>.</summary>
	string ToString();
	#endregion
}

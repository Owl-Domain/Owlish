namespace OwlDomain.Owlish.Engine.Common.Text.Parsing;

/// <summary>
/// 	Represents a general text parser.
/// </summary>
public interface ITextParser
{
	#region Properties
	/// <summary>The current text element.</summary>
	/// <remarks>If the current text element is not available, an empty text element will be returned.</remarks>
	TextElement Current { get; }

	/// <summary>The next text element.</summary>
	/// <remarks>If the next text element is not available, an empty text element will be returned.</remarks>
	TextElement Next { get; }

	/// <summary>The current position in the text.</summary>
	TextPosition Position { get; }

	/// <summary>Whether the parser has reached the end of the available text.</summary>
	bool IsAtEnd { get; }

	/// <summary>Whether the parser has remaining text elements that can be parsed.</summary>
	bool HasRemaining { get; }

	/// <summary>Whether the parser is at the beginning of the current line.</summary>
	bool IsAtStartOfLine { get; }
	#endregion

	#region Methods
	/// <summary>The amount of text elements to advance the parser's position by.</summary>
	/// <param name="amount">The amount of text elements to advance the parser's position by.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if given <paramref name="amount"/> is less than one.</exception>
	void Advance(int amount = 1);

	/// <summary>Tries to peek at the text <paramref name="element"/> at the given relative <paramref name="offset"/> from the current position.</summary>
	/// <param name="offset">The relative offset (in terms of text elements) from the current position.</param>
	/// <param name="element">The text element at the given relative <paramref name="offset"/>.</param>
	/// <returns>
	/// 	<see langword="true"/> if the text <paramref name="element"/> at the given relative
	/// 	<paramref name="offset"/> was available, <see langword="false"/> otherwise.
	/// </returns>
	bool TryPeek(int offset, out TextElement element);

	/// <summary>Peeks at the text element at the given relative <paramref name="offset"/> from the current position.</summary>
	/// <param name="offset">The relative offset (in terms of text elements) from the current position.</param>
	/// <returns>The text element at the given relative <paramref name="offset"/>.</returns>
	/// <remarks>If the text element at the given offset is not available, an empty text element will be returned.</remarks>
	TextElement Peek(int offset);

	/// <summary>Gets a slice at the given <paramref name="offset"/>, with the given amount of text elements.</summary>
	/// <param name="offset">The relative offset (in terms of text elements) from the current position to start the slice at.</param>
	/// <param name="amount">The amount of text elements that the slice should include.</param>
	/// <returns>The requested slice.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 	Thrown if either the given <paramref name="offset"/>, or the
	/// 	given <paramref name="amount"/> (of text elements) is invalid.
	/// </exception>
	ReadOnlySpan<char> GetSlice(int offset, int amount);

	/// <summary>Advances the position by one if the given text <paramref name="element"/> matches the current text element.</summary>
	/// <param name="element">The text element to check against the current text element.</param>
	/// <returns>
	/// 	<see langword="true"/> if the text elements matched and the
	/// 	position was advanced, <see langword="false"/> otherwise.
	/// </returns>
	bool Match(TextElement element);

	/// <summary>Advances the position by one if the given <paramref name="character"/> matches the current text element.</summary>
	/// <param name="character">The character to check against the current text element.</param>
	/// <returns>
	/// 	<see langword="true"/> if the given <paramref name="character"/> matched the current
	/// 	text element and the position was advanced, <see langword="false"/> otherwise.
	/// </returns>
	bool Match(char character);

	/// <summary>Advances the position by one if the given <see langword="char"/> <paramref name="span"/> matches the next text elements.</summary>
	/// <param name="span">The span to compare against the next text elements.</param>
	/// <returns>
	/// 	<see langword="true"/> if the given <paramref name="span"/> matched the next text
	/// 	elements and the position was advanced, <see langword="false"/> otherwise.
	/// </returns>
	bool Match(ReadOnlySpan<char> span);

	/// <summary>Marks the current position as the start of a new line.</summary>
	/// <remarks>This should be done after consuming the line break characters.</remarks>
	void MarkNewLine();

	/// <summary>Returns the current text element and advances the parser.</summary>
	/// <returns>The current text element.</returns>
	TextElement Consume();

	/// <summary>Tries to provide the current text <paramref name="element"/> and advance the parser.</summary>
	/// <param name="element">The current text element.</param>
	/// <returns>Whether the current text <paramref name="element"/> could be provided, <see langword="false"/> otherwise.</returns>
	bool TryConsume(out TextElement element);
	#endregion
}

using System.Globalization;
using System.Text;

namespace OwlDomain.Owlish.Engine.Common.Text;

/// <summary>
/// 	Represents a single text element (extended grapheme cluster) in a piece of text.
/// </summary>
public readonly ref struct TextElement
{
	#region Properties
	/// <summary>The span of characters that represents the text element.</summary>
	public ReadOnlySpan<char> Span { get; }

	/// <summary>checks whether the current element represents a whitespace character.</summary>
	public bool IsWhitespace => Span.Length is 1 && char.IsWhiteSpace(Span[0]);

	/// <summary>checks whether the current element represents a control character.</summary>
	public bool IsControl => Span.Length is 1 && char.IsControl(Span[0]);
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="TextElement"/>.</summary>
	/// <param name="span">The span of characters that represents the text element.</param>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="span"/> does not contain exactly 1 text element.</exception>
	public TextElement(ReadOnlySpan<char> span)
	{
		if (IsValid(span) is false)
			ThrowHelper.ThrowArgumentException(nameof(span), "The given span should contain exactly 1 text element (extended grapheme cluster).");

		Span = span;
	}
	#endregion

	#region Methods
	/// <summary>Compares the current text element with the <paramref name="other"/> given text element.</summary>
	/// <param name="other">The other text element to check for equality.</param>
	/// <param name="comparison">The type of string comparison to use for the equality check.</param>
	/// <returns>
	/// 	<see langword="true"/> if the current text element is equal to the
	/// 	<paramref name="other"/> given text element, <see langword="false"/> otherwise.
	/// </returns>
	public bool Equals(TextElement other, StringComparison comparison = StringComparison.Ordinal) => Span.Equals(other.Span, comparison);

	/// <summary>Compares the current text element with the <paramref name="other"/> given <see langword="char"/> span.</summary>
	/// <param name="other">The other <see langword="char"/> span to check for equality.</param>
	/// <param name="comparison">The type of string comparison to use for the equality check.</param>
	/// <returns>
	/// 	<see langword="true"/> if the current text element is equal to the
	/// 	<paramref name="other"/> given <see langword="char"/> span, <see langword="false"/> otherwise.
	/// </returns>
	public bool Equals(ReadOnlySpan<char> other, StringComparison comparison = StringComparison.Ordinal) => Span.Equals(other, comparison);

	/// <summary>Compares the current text element with the <paramref name="other"/> given <see langword="char"/>.</summary>
	/// <param name="other">The other <see langword="char"/> to check for equality.</param>
	/// <param name="comparison">The type of string comparison to use for the equality check.</param>
	/// <returns>
	/// 	<see langword="true"/> if the current text element is equal to the
	/// 	<paramref name="other"/> given <see langword="char"/>, <see langword="false"/> otherwise.
	/// </returns>
	public bool Equals(char other, StringComparison comparison = StringComparison.Ordinal) => Span.Equals([other], comparison);

	/// <inheritdoc/>
	public override string ToString() => Span.ToString();
	#endregion

	#region Obsolete methods
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

	/// <summary>This method is not supported as text elements cannot be boxed, either use the equality operator or a different overload.</summary>
	/// <exception cref="NotSupportedException">Will always be thrown.</exception>
	[Obsolete($"{nameof(Equals)}(object) is not supported, either use a different overload or the equality operator.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		ThrowHelper.ThrowNotSupportedException($"{nameof(Equals)}(object) is not supported, use either the equality operator or a different overload.");
		return default;
	}

	/// <summary>This method is not supported as text elements cannot be boxed.</summary>
	/// <exception cref="NotSupportedException">Will always be thrown.</exception>
	[Obsolete($"{nameof(GetHashCode)}() is not supported as text elements cannot be boxed.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int GetHashCode()
	{
		ThrowHelper.ThrowNotSupportedException<int>($"{nameof(GetHashCode)} is not supported.");
		return default;
	}

#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
	#endregion

	#region Helpers
	private static bool IsValid(ReadOnlySpan<char> span)
	{
		int length = StringInfo.GetNextTextElementLength(span);

		return length > 0 && length == span.Length;
	}
	#endregion

	#region Operators
	/// <summary>Compares two values to determine equality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(TextElement left, TextElement right) => left.Equals(right);

	/// <summary>Compares two values to determine inequality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(TextElement left, TextElement right) => left.Equals(right) is false;

	/// <summary>Compares two values to determine equality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(TextElement left, ReadOnlySpan<char> right) => left.Equals(right);

	/// <summary>Compares two values to determine inequality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(TextElement left, ReadOnlySpan<char> right) => left.Equals(right) is false;

	/// <summary>Compares two values to determine equality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(TextElement left, char right) => left.Equals(right);

	/// <summary>Compares two values to determine inequality.</summary>
	/// <param name="left">The value to compare with <paramref name="right"/>.</param>
	/// <param name="right">The value to compare with <paramref name="left"/>.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(TextElement left, char right) => left.Equals(right) is false;
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="TextElement"/>.
/// </summary>
public static class TextElementExtensions
{
	extension(ReadOnlySpan<char> span)
	{
		#region Methods
		/// <summary>Gets the next text element in the given <see langword="char"/> span.</summary>
		/// <returns>The next text element in the given <see langword="char"/> span.</returns>
		public TextElement GetNextTextElement()
		{
			int length = StringInfo.GetNextTextElementLength(span);
			ReadOnlySpan<char> slice = span[..length];

			return new(slice);
		}

		/// <summary>Gets the next text element in the given <see langword="char"/> span.</summary>
		/// <param name="remaining">The span of the remaining <see langword="char"/> values.</param>
		/// <returns>The next text element in the given <see langword="char"/> span.</returns>
		public TextElement GetNextTextElement(out ReadOnlySpan<char> remaining)
		{
			int length = StringInfo.GetNextTextElementLength(span);

			ReadOnlySpan<char> slice = span[..length];
			remaining = span[length..];

			return new(slice);
		}

		/// <summary>Gets the next text <paramref name="element"/> in the given <see langword="char"/> span.</summary>
		/// <param name="element">The next text element in the given <see langword="char"/> span.</param>
		/// <returns>The span of the remaining <see langword="char"/> values.</returns>
		public ReadOnlySpan<char> GetNextTextElement(out TextElement element)
		{
			int length = StringInfo.GetNextTextElementLength(span);

			ReadOnlySpan<char> slice = span[..length];
			element = new(slice);

			return span[length..];
		}
		#endregion
	}

	extension(StringBuilder builder)
	{
		#region Methods
		/// <summary>Appends the given text <paramref name="element"/> to the string builder.</summary>
		/// <param name="element">The text element to append to the builder.</param>
		/// <returns>The used builder instance.</returns>
		public StringBuilder Append(TextElement element) => builder.Append(element.Span);
		#endregion
	}
}

namespace OwlDomain.Owlish.Engine.Common.Text;

/// <summary>
/// 	Represents a span location inside of a piece of text.
/// </summary>
public readonly struct TextSpan :
#if NET7_0_OR_GREATER
	IEqualityOperators<TextSpan, TextSpan, bool>,
#endif
	IEquatable<TextSpan>
{
	#region Properties
	/// <summary>The inclusive start of the span.</summary>
	public TextPosition Start { get; }

	/// <summary>The exclusive start of the span.</summary>
	public TextPosition End { get; }

	/// <summary>The length of the span.</summary>
	public int Length => checked(End.Offset - Start.Offset); // Should always be zero or positive so use checked to make sure nothing goes wrong.
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="TextSpan"/>.</summary>
	/// <param name="start">The start position of the span. This should be earlier in the text than the <paramref name="end"/> position, or they should represent the same position.</param>
	/// <param name="end">The start position of the span. This should be later in the text than the <paramref name="start"/> position, or they should represent the same position.</param>
	public TextSpan(TextPosition start, TextPosition end)
	{
		Guard.IsLessThanOrEqualTo(start, end);

		Start = start;
		End = end;
	}
	#endregion

	#region Methods
	/// <summary>Checks whether the given <paramref name="offset"/> occurs in the span.</summary>
	/// <param name="offset">The offset to check.</param>
	/// <returns><see langword="true"/> if the given <paramref name="offset"/> occurs in the span, <see langword="false"/> otherwise.</returns>
	public bool Contains(int offset) => Start.Offset <= offset && End.Offset > offset;

	/// <summary>checks whether the given <paramref name="position"/> occurs in the span.</summary>
	/// <param name="position">The position to check.</param>
	/// <returns><see langword="true"/> if the given <paramref name="position"/> occurs in the span, <see langword="false"/> otherwise.</returns>
	public bool Contains(TextPosition position) => Contains(position.Offset);

	/// <inheritdoc/>
	public bool Equals(TextSpan other)
	{
		return
			Start == other.Start &&
			End == other.End;
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is TextSpan other)
			return Equals(other);

		return false;
	}

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Start, End);

	/// <inheritdoc/>
	public override string ToString() => $"{Start} -> {End}, {Length}";
	#endregion

	#region Operators
	/// <summary>Checks whether the two values represent the same span.</summary>
	/// <param name="left">The first span.</param>
	/// <param name="right">The second span.</param>
	/// <returns><see langword="true"/> if the two values represent the same span, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(TextSpan left, TextSpan right) => left.Equals(right);

	/// <summary>Checks whether the two values represent different spans.</summary>
	/// <param name="left">The first span.</param>
	/// <param name="right">The second span.</param>
	/// <returns><see langword="true"/> if the two values represent different spans, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(TextSpan left, TextSpan right) => left.Equals(right) is false;
	#endregion
}

namespace OwlDomain.Owlish.Engine.Common.Text;

/// <summary>
/// 	Represents a single position inside of a piece of text.
/// </summary>
public readonly struct TextPosition :
#if NET7_0_OR_GREATER
	IEqualityOperators<TextPosition, TextPosition, bool>,
	IComparisonOperators<TextPosition, TextPosition, bool>,
#endif
	IEquatable<TextPosition>,
	IComparable<TextPosition>
{
	#region Properties
	/// <summary>Represents an empty position.</summary>
	/// <remarks>This generally means an invalid position.</remarks>
	public static TextPosition Empty { get; } = new(0, 0, 0);

	/// <summary>The offset inside of the text.</summary>
	/// <remarks>This should be zero indexed.</remarks>
	public int Offset { get; }

	/// <summary>The line inside of the text.</summary>
	/// <remarks>This should be one indexed.</remarks>
	public int Line { get; }

	/// <summary>The column on the <see cref="Line"/> inside of the text..</summary>
	/// <remarks>This should be one indexed.</remarks>
	public int Column { get; }

	/// <summary>Checks whether this value is equal to <see cref="Empty"/>.</summary>
	public bool IsEmpty => this == Empty;
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="TextPosition"/>.</summary>
	/// <param name="offset">The offset inside of the text. Should be greater than or equal to zero.</param>
	/// <param name="line">The line inside of the text. Should be greater than zero.</param>
	/// <param name="column">The column on the <paramref name="line"/> inside of the text. Should be greater than zero.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// 	Thrown if either the <paramref name="offset"/>, <paramref name="line"/> or <paramref name="column"/> is invalid.
	/// </exception>
	public TextPosition(int offset, int line, int column)
	{
		Guard.IsGreaterThanOrEqualTo(offset, 0);
		Guard.IsGreaterThanOrEqualTo(line, 1);
		Guard.IsGreaterThanOrEqualTo(column, 1);

		Offset = offset;
		Line = line;
		Column = column;
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public int CompareTo(TextPosition other) => Offset.CompareTo(other.Offset);

	/// <inheritdoc/>
	public bool Equals(TextPosition other)
	{
		return
			Offset == other.Offset &&
			Line == other.Line &&
			Column == other.Column;
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is TextPosition other)
			return Equals(other);

		return false;
	}

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Offset, Line, Column);

	/// <inheritdoc/>
	public override string ToString() => $"({Offset}, {Line}, {Column})";
	#endregion

	#region Operators
	/// <summary>Checks whether the two values represent the same position.</summary>
	/// <param name="left">The first value.</param>
	/// <param name="right">The second value.</param>
	/// <returns><see langword="true"/> if the two values represent the same position, <see langword="false"/> otherwise.</returns>
	public static bool operator ==(TextPosition left, TextPosition right) => left.Equals(right);

	/// <summary>Checks whether the two values represent different positions.</summary>
	/// <param name="left">The first value.</param>
	/// <param name="right">The second value.</param>
	/// <returns><see langword="true"/> if the two values represent different positions, <see langword="false"/> otherwise.</returns>
	public static bool operator !=(TextPosition left, TextPosition right) => left.Equals(right) is false;

	/// <summary>Checks whether the <paramref name="left"/> position appears earlier in the text than the <paramref name="right"/> position.</summary>
	/// <param name="left">The left position.</param>
	/// <param name="right">The right position.</param>
	/// <returns>
	/// 	<see langword="true"/> if the <paramref name="left"/> position appears in the text
	/// 	earlier than the <paramref name="right"/> position, <see langword="false"/> otherwise.
	/// </returns>
	public static bool operator <(TextPosition left, TextPosition right) => left.CompareTo(right) < 0;

	/// <summary>Checks whether the <paramref name="left"/> position appears later in the text than the <paramref name="right"/> position.</summary>
	/// <param name="left">The left position.</param>
	/// <param name="right">The right position.</param>
	/// <returns>
	/// 	<see langword="true"/> if the <paramref name="left"/> position appears in the text
	/// 	later than the <paramref name="right"/> position, <see langword="false"/> otherwise.
	/// </returns>
	public static bool operator >(TextPosition left, TextPosition right) => left.CompareTo(right) > 0;

	/// <summary>
	/// 	Checks whether the <paramref name="left"/> position appears earlier in the text
	/// 	than the <paramref name="right"/> position, or if they represent the same position.
	/// </summary>
	/// <param name="left">The left position.</param>
	/// <param name="right">The right position.</param>
	/// <returns>
	/// 	<see langword="true"/> if the <paramref name="left"/> position appears in the text earlier than the
	/// 	<paramref name="right"/> position, or if they represent the same position, <see langword="false"/> otherwise.
	/// </returns>
	public static bool operator <=(TextPosition left, TextPosition right) => left.CompareTo(right) <= 0;

	/// <summary>
	/// 	Checks whether the <paramref name="left"/> position appears later in the text
	/// 	than the <paramref name="right"/> position, or if they represent the same position.
	/// </summary>
	/// <param name="left">The left position.</param>
	/// <param name="right">The right position.</param>
	/// <returns>
	/// 	<see langword="true"/> if the <paramref name="left"/> position appears in the text later than the
	/// 	<paramref name="right"/> position, or if they represent the same position, <see langword="false"/> otherwise.
	/// </returns>
	public static bool operator >=(TextPosition left, TextPosition right) => left.CompareTo(right) >= 0;
	#endregion
}

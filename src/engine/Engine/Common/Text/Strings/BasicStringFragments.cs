namespace OwlDomain.Owlish.Engine.Common.Text.Strings;

/// <summary>
/// 	Represents a string fragment that contains regular text.
/// </summary>
public sealed class TextStringFragment : BaseStringFragment
{
	/// <summary>Creates a new instance of the <see cref="TextStringFragment"/>.</summary>
	/// <param name="value">The value of the fragment.</param>
	/// <param name="position">The position of the fragment.</param>
	public TextStringFragment(string value, TextSpan position) : base(value, position)
	{
	}
}

/// <summary>
/// 	Represents a string fragment that contains the opening quote.
/// </summary>
public sealed class OpeningQuoteStringFragment : BaseStringFragment
{
	/// <summary>Creates a new instance of the <see cref="OpeningQuoteStringFragment"/>.</summary>
	/// <param name="value">The value of the fragment.</param>
	/// <param name="position">The position of the fragment.</param>
	public OpeningQuoteStringFragment(string value, TextSpan position) : base(value, position)
	{
	}
}

/// <summary>
/// 	Represents a string fragment that contains the closing quote.
/// </summary>
public sealed class ClosingQuoteStringFragment : BaseStringFragment
{
	/// <summary>Creates a new instance of the <see cref="ClosingQuoteStringFragment"/>.</summary>
	/// <param name="value">The value of the fragment.</param>
	/// <param name="position">The position of the fragment.</param>
	public ClosingQuoteStringFragment(string value, TextSpan position) : base(value, position)
	{
	}
}


/// <summary>
/// 	Represents a string fragment that contains an escape sequence.
/// </summary>
public sealed class EscapeStringFragment : BaseStringFragment
{
	/// <summary>Creates a new instance of the <see cref="EscapeStringFragment"/>.</summary>
	/// <param name="value">The value of the fragment.</param>
	/// <param name="position">The position of the fragment.</param>
	public EscapeStringFragment(string value, TextSpan position) : base(value, position)
	{
	}
}

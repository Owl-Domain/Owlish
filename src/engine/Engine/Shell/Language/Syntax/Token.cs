namespace OwlDomain.Owlish.Engine.Shell.Language.Syntax;

/// <summary>
/// 	Represents a token node.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class Token : ISyntaxNode
{
	#region Properties
	/// <inheritdoc/>
	public SyntaxKind Kind { get; }

	/// <inheritdoc/>
	public TextSpan Position { get; }

	/// <inheritdoc/>
	public TextSpan FullPosition => GetFullPosition();

	/// <summary>The trivia nodes that came before the token.</summary>
	public TriviaList LeadingTrivia { get; }

	/// <summary>The trivia nodes that came after the token.</summary>
	public TriviaList TrailingTrivia { get; }

	/// <summary>Whether this token was fabricated.</summary>
	/// <remarks>A fabricated token is a token that is faked for the purposes of error recovery during parsing.</remarks>
	public bool IsFabricated { get; }

	/// <summary>The value that the token holds.</summary>
	public object? Value { get; }
	#endregion

	#region Constructors
	private Token(SyntaxKind kind, TextSpan position, bool isFabricated, object? value = null, TriviaList? leadingTrivia = null, TriviaList? trailingTrivia = null)
	{
		Guard.IsToken(kind);

		if (leadingTrivia?.IsEmpty is false)
			Guard.IsGreaterThanOrEqualTo(position.Start, leadingTrivia.Position.End);

		if (trailingTrivia?.IsEmpty is false)
			Guard.IsLessThanOrEqualTo(position.End, trailingTrivia.Position.Start);

		Kind = kind;
		Position = position;
		IsFabricated = isFabricated;
		Value = value;
		LeadingTrivia = leadingTrivia ?? TriviaList.Empty;
		TrailingTrivia = trailingTrivia ?? TriviaList.Empty;
	}

	/// <summary>Creates a new syntax <see cref="Token"/>.</summary>
	/// <param name="kind">The kind of the syntax node. This must represent a token.</param>
	/// <param name="position">The position of the token inside of the text.</param>
	/// <param name="value">The value that token holds.</param>
	/// <param name="leadingTrivia">The list of the leading trivia nodes.</param>
	/// <param name="trailingTrivia">The list of the trailing trivia nodes.</param>
	/// <remarks>To create a fabricated token use <see cref="Fabricated(SyntaxKind, TextPosition)"/>.</remarks>
	public Token(SyntaxKind kind, TextSpan position, object? value = null, TriviaList? leadingTrivia = null, TriviaList? trailingTrivia = null)
		: this(kind, position, false, value, leadingTrivia, trailingTrivia)
	{
	}
	#endregion

	#region Functions
	/// <summary>Creates a new fabricated <see cref="Token"/>.</summary>
	/// <param name="kind">The kind of the syntax node. This must represent a token.</param>
	/// <param name="position">The position of the fabricated token inside of the text.</param>
	/// <returns>A new fabricated token.</returns>
	/// <remarks>A fabricated token is a token that is faked for the purposes of error recovery during parsing.</remarks>
	public static Token Fabricated(SyntaxKind kind, TextPosition position)
	{
		Guard.IsToken(kind);

		return new(kind, new(position, position), true);
	}
	#endregion

	#region Helpers
	private string DebuggerDisplay()
	{
		if (Value is not null)
			return $"{Kind} {{ Position = ({Position}), Value = ({Value}) }}";

		return $"{Kind} {{ Position = ({Position}) }}";
	}
	private TextSpan GetFullPosition()
	{
		TextPosition start = (LeadingTrivia.PositionOrNull ?? Position).Start;
		TextPosition end = (TrailingTrivia.PositionOrNull ?? Position).End;

		return new(start, end);
	}
	#endregion
}

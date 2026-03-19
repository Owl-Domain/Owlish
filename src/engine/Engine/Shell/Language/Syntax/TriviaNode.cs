namespace OwlDomain.Owlish.Engine.Shell.Language.Syntax;

/// <summary>
/// 	Represents a trivia node.
/// </summary>
public sealed class TriviaNode : ISyntaxNode
{
	#region Properties
	/// <inheritdoc/>
	public SyntaxKind Kind { get; }

	/// <inheritdoc/>
	public TextSpan Position { get; }

	/// <inheritdoc/>
	public TextSpan FullPosition => Position;

	/// <summary>The value of the trivia.</summary>
	public string Value { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="TriviaNode"/>.</summary>
	/// <param name="kind">The kind of the syntax node. This should be a type of trivia.</param>
	/// <param name="position">The position that the node takes up in the text.</param>
	/// <param name="value">The value of the trivia node.</param>
	public TriviaNode(SyntaxKind kind, TextSpan position, string value)
	{
		Guard.IsTrivia(kind);

		Kind = kind;
		Position = position;
		Value = value;
	}
	#endregion
}

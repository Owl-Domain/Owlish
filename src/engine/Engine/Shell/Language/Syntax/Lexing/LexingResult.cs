namespace OwlDomain.Owlish.Engine.Shell.Language.Syntax.Lexing;

/// <summary>
/// 	Represents the result of a lexing operation.
/// </summary>
public sealed class LexingResult
{
	#region Properties
	/// <summary>The text that was lexed.</summary>
	public string Text { get; }

	/// <summary>The diagnostics that occurred during lexing.</summary>
	public DiagnosticBag Diagnostics { get; }

	/// <summary>The lexed tokens.</summary>
	public IReadOnlyList<Token> Tokens { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="LexingResult"/>.</summary>
	/// <param name="text">The text that was lexed.</param>
	/// <param name="diagnostics">The diagnostics that occurred during lexing.</param>
	/// <param name="tokens">The lexed tokens. There should be at least one token representing the <see cref="SyntaxKind.EndOfFileToken"/>.</param>
	public LexingResult(string text, DiagnosticBag diagnostics, IReadOnlyList<Token> tokens)
	{
		if (tokens.Count is 0 || tokens[^1].Kind is not SyntaxKind.EndOfFileToken)
			ThrowHelper.ThrowArgumentException(nameof(tokens), "The last token should represent an end-of-file token.");

		Text = text;
		Diagnostics = diagnostics;
		Tokens = tokens;
	}
	#endregion
}

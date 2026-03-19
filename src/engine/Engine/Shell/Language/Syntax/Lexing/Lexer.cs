using System.Text;
using OwlDomain.Owlish.Engine.Common.Text.Parsing;

namespace OwlDomain.Owlish.Engine.Shell.Language.Syntax.Lexing;

/// <summary>
/// 	Represents a lexer for the shell REPL language.
/// </summary>
public sealed class Lexer
{
	#region Properties
	private StringTextParser Text { get; }
	private DiagnosticBag Diagnostics { get; } = [];
	private StringBuilder Builder { get; } = new();
	#endregion

	#region Constructors
	private Lexer(StringTextParser text)
	{
		Text = text;
	}
	#endregion

	#region Functions
	/// <summary>Lexes the given <paramref name="text"/>.</summary>
	/// <param name="text">The input text to lex.</param>
	/// <returns>The result of the lexing operation.</returns>
	public static LexingResult Lex(string text)
	{
		using StringTextParser textParser = new(text);
		Lexer lexer = new(textParser);

		IReadOnlyList<Token> tokens = lexer.Lex();
		LexingResult result = new(text, lexer.Diagnostics, tokens);

		return result;
	}
	#endregion

	#region Methods
	/// <summary>Lexes the entire input text for tokens.</summary>
	/// <returns>A list of the lexed tokens.</returns>
	/// <remarks>This method will also convert any bad tokens into trivia.</remarks>
	private IReadOnlyList<Token> Lex()
	{
		List<Token> tokens = [];
		List<Token> badTokens = [];

		Token token;
		do
		{
			token = LexToken();
			Debug.Assert(token.IsFabricated is false, "The lexer should not fabricate any tokens.");

			if (token.Kind is SyntaxKind.BadToken)
			{
				badTokens.Add(token);
				continue;
			}

			if (badTokens.Count is 0)
			{
				tokens.Add(token);
				continue;
			}

			List<TriviaNode> leading = [.. token.LeadingTrivia];
			int index = 0;

			foreach (Token badToken in badTokens)
			{
				Debug.Assert(badToken.Value is string, "Bad tokens should include the bad character as their value (as a string).");

				leading.InsertRange(index, badToken.LeadingTrivia);
				index += badToken.LeadingTrivia.Count;

				TriviaNode newTrivia = new(SyntaxKind.BadTokenTrivia, badToken.Position, (string)badToken.Value);
				leading.Insert(index++, newTrivia);

				leading.InsertRange(index, badToken.TrailingTrivia);
				index += badToken.TrailingTrivia.Count;
			}

			badTokens.Clear();
			token = new(token.Kind, token.Position, token.Value, new(leading), token.TrailingTrivia);
			tokens.Add(token);
		}
		while (token.Kind is not SyntaxKind.EndOfFileToken);

		return tokens;
	}
	#endregion

	#region Token methods
	private Token LexToken()
	{
		TriviaList leadingTrivia = LexLeadingTrivia();

		TextPosition start = Text.Position;
		if (Text.IsAtEnd)
			return new(SyntaxKind.EndOfFileToken, new(start, start), null, leadingTrivia);

		SyntaxKind kind = LexToken(out object? value);
		Debug.Assert(Builder.Length is 0);

		TextPosition end = Text.Position;
		TriviaList trailingTrivia = LexTrailingTrivia();

		return new(kind, new(start, end), value, leadingTrivia, trailingTrivia);
	}
	private SyntaxKind LexToken(out object? value)
	{
		value = default;

		return SyntaxKind.BadToken;
	}
	#endregion

	#region Trivia methods
	private TriviaList LexLeadingTrivia()
	{
		List<TriviaNode>? list = null;

		while (TryLexTrivia(out TriviaNode? trivia))
		{
			Debug.Assert(Builder.Length is 0);

			list ??= [];
			list.Add(trivia);
		}

		return list is null ? TriviaList.Empty : new(list);
	}
	private TriviaList LexTrailingTrivia()
	{
		List<TriviaNode>? list = null;

		while (TryLexTrivia(out TriviaNode? trivia))
		{
			Debug.Assert(Builder.Length is 0);

			list ??= [];
			list.Add(trivia);

			// Note(Nightowl): Normally trailing trivia only goes until a line break but I'm not sure if the REPL will allow line breaks yet;
		}

		return list is null ? TriviaList.Empty : new(list);
	}
	private bool TryLexTrivia([NotNullWhen(true)] out TriviaNode? trivia)
	{
		if (TryLexWhitespaceTrivia(out trivia))
			return true;

		return false;
	}
	private bool TryLexWhitespaceTrivia([NotNullWhen(true)] out TriviaNode? trivia)
	{
		if (IsWhitespaceTrivia(Text.Current) is false)
		{
			trivia = null;
			return false;
		}

		TextPosition start = Text.Position;

		while (IsWhitespaceTrivia(Text.Current))
			Builder.Append(Text.Consume());

		trivia = new(SyntaxKind.WhitespaceTrivia, new(start, Text.Position), Builder.ToStringAndClear());
		return true;
	}
	private static bool IsWhitespaceTrivia(TextElement element)
	{
		// Note(Nightowl): Line breaks shouldn't be mistaken for normal whitespace;
		if (element == '\r' || element == '\n')
			return false;

		return element.IsWhitespace;
	}
	#endregion
}

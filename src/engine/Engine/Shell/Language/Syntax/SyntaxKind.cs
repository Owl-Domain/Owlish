namespace OwlDomain.Owlish.Engine.Shell.Language.Syntax;

/// <summary>
/// 	Represents the available types of syntax nodes.
/// </summary>
public enum SyntaxKind
{
	/// <summary>An invalid syntax node.</summary>
	Unknown,

	#region Special
	/// <summary>A token that represents invalid input.</summary>
	/// <remarks>I don't even know if anything will actually count as bad input.</remarks>
	BadToken,

	/// <summary>Marks the end of the parsed input.</summary>
	/// <remarks>Not actually gonna be a 'file' but calling it something else feels weird.</remarks>
	EndOfFileToken,
	#endregion

	#region Trivia
	/// <summary>A collection of <see cref="BadToken"/> tokens converted to trivia to ease processing.</summary>
	BadTokenTrivia,

	/// <summary>A trivia token that represents whitespace characters that will not be passed to the called program.</summary>
	WhitespaceTrivia,
	#endregion

	#region Tokens
	/// <summary>A piece of regular text.</summary>
	TextToken,

	/// <summary>A string value token enclosed in either <c>""</c> (double quotes) or <c>''</c> (single quotes).</summary>
	StringToken,
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="SyntaxKind"/>.
/// </summary>
public static class SyntaxKindExtensions
{
	#region Properties
	private static HashSet<SyntaxKind> Tokens { get; } = [];
	private static HashSet<SyntaxKind> Trivia { get; } = [];
	#endregion
	static SyntaxKindExtensions()
	{
		foreach (SyntaxKind kind in Enum.GetValues<SyntaxKind>())
		{
			string name = kind.ToString();
			if (name.EndsWith("Token"))
				Tokens.Add(kind);
			else if (name.EndsWith("Trivia"))
				Trivia.Add(kind);
		}
	}

	extension(SyntaxKind kind)
	{
		#region Methods
		/// <summary>Checks whether the syntax kind represents a token node.</summary>
		/// <returns><see langword="true"/> if the syntax <paramref name="kind"/> represents a token node, <see langword="false"/> otherwise.</returns>
		public bool IsToken => Tokens.Contains(kind);

		/// <summary>Checks whether the syntax kind represents a trivia node.</summary>
		/// <returns><see langword="true"/> if the syntax <paramref name="kind"/> represents a trivia node, <see langword="false"/> otherwise.</returns>
		public bool IsTrivia => Trivia.Contains(kind);
		#endregion
	}

	extension(Guard)
	{
		#region Functions
		/// <summary>Asserts that the given syntax <paramref name="kind"/> represents a token node.</summary>
		/// <param name="kind">The syntax kind to check.</param>
		/// <param name="name">The name of the parameter that is passed in as the syntax <paramref name="kind"/> value.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the given syntax <paramref name="kind"/> does not represent a token node.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IsToken(SyntaxKind kind, [CallerArgumentExpression(nameof(kind))] string name = "")
		{
			if (kind.IsToken is false)
				ThrowHelper.ThrowArgumentOutOfRangeException(name, kind, "Expected the given syntax kind to represent a token.");
		}

		/// <summary>Asserts that the given syntax <paramref name="kind"/> represents a trivia node.</summary>
		/// <param name="kind">The syntax kind to check.</param>
		/// <param name="name">The name of the parameter that is passed in as the syntax <paramref name="kind"/> value.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the given syntax <paramref name="kind"/> does not represent a trivia node.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IsTrivia(SyntaxKind kind, [CallerArgumentExpression(nameof(kind))] string name = "")
		{
			if (kind.IsTrivia is false)
				ThrowHelper.ThrowArgumentOutOfRangeException(name, kind, "Expected the given syntax kind to represent a trivia node.");
		}
		#endregion
	}
}

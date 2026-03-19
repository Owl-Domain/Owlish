namespace OwlDomain.Owlish.Engine.Shell.Language;

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
}

namespace OwlDomain.Owlish.Engine.Shell.Language.Syntax;

/// <summary>
/// 	Represents a syntax node.
/// </summary>
public interface ISyntaxNode
{
	#region Properties
	/// <summary>The kind of the syntax node.</summary>
	SyntaxKind Kind { get; }

	/// <summary>The position that the node takes up in the text.</summary>
	/// <remarks>This position will exclude the start and end trivia.</remarks>
	TextSpan Position { get; }

	/// <summary>The position that the node takes up in the text.</summary>
	/// <summary>This position will include the start and end trivia.</summary>
	TextSpan FullPosition { get; }
	#endregion
}

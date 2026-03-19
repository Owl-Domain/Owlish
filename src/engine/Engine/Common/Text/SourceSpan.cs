namespace OwlDomain.Owlish.Engine.Common.Text;

/// <summary>
/// 	Represents a span in a piece of text, along with optional source information.
/// </summary>
public readonly struct SourceSpan
{
	#region Properties
	/// <summary>The position inside of a piece of text.</summary>
	public TextSpan Position { get; }

	/// <summary>The source information.</summary>
	/// <remarks>This should be the name/path of the source file, rather than the direct source itself.</remarks>
	public string? Source { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="SourceSpan"/>.</summary>
	/// <param name="span">The span inside of a piece of text.</param>
	/// <param name="source">The optional source information. This should be the name/path of the source file, rather than the direct source itself.</param>
	public SourceSpan(TextSpan span, string? source = null)
	{
		Position = span;
		Source = source;
	}
	#endregion
}

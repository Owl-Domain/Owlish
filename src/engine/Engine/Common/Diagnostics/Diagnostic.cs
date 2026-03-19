namespace OwlDomain.Owlish.Engine.Common.Diagnostics;

/// <summary>
/// 	Represents a diagnostic.
/// </summary>
public sealed class Diagnostic
{
	#region Properties
	/// <summary>The severity of the diagnostic.</summary>
	public Severity Severity { get; }

	/// <summary>The source position that the diagnostic is about.</summary>
	public SourceSpan Source { get; }

	/// <summary>The diagnostic message.</summary>
	public string Message { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="Diagnostic"/>.</summary>
	/// <param name="severity">The severity of the diagnostic.</param>
	/// <param name="source">The source position that the diagnostic is about.</param>
	/// <param name="message">The diagnostic message.</param>
	public Diagnostic(Severity severity, SourceSpan source, string message)
	{
		Severity = severity;
		Source = source;
		Message = message;
	}
	#endregion
}

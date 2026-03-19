namespace OwlDomain.Owlish.Engine.Common.Diagnostics;

/// <summary>
/// 	Represents a diagnostic severity.
/// </summary>
public enum Severity
{
	/// <summary>An error that should stop further processing.</summary>
	Error,

	/// <summary>A warning that something could be going wrong.</summary>
	Warning,

	/// <summary>A hint to the user about something.</summary>
	Hint,
}

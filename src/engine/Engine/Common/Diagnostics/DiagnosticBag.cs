namespace OwlDomain.Owlish.Engine.Common.Diagnostics;

/// <summary>
/// 	Represents a collection of diagnostics.
/// </summary>
public sealed class DiagnosticBag : ICollection<Diagnostic>
{
	#region Fields
	private readonly List<Diagnostic> _diagnostics = [];
	#endregion

	#region Properties
	/// <inheritdoc/>
	public int Count => _diagnostics.Count;
	bool ICollection<Diagnostic>.IsReadOnly => false;

	/// <summary>Whether there are any errors in the bag.</summary>
	public bool HasErrors => _diagnostics.Any(diagnostic => diagnostic.Severity is Severity.Error);
	#endregion

	#region Methods
	/// <inheritdoc/>
	public bool Contains(Diagnostic item) => _diagnostics.Contains(item);

	/// <inheritdoc/>
	public void CopyTo(Diagnostic[] array, int arrayIndex) => _diagnostics.CopyTo(array, arrayIndex);

	/// <inheritdoc/>
	public bool Remove(Diagnostic item) => _diagnostics.Remove(item);

	/// <inheritdoc/>
	public void Clear() => _diagnostics.Clear();

	/// <inheritdoc/>
	public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	#region Add methods
	/// <inheritdoc/>
	public void Add(Diagnostic item) => _diagnostics.Add(item);

	/// <summary>Creates a new diagnostic with the severity of an error.</summary>
	/// <param name="source">The source position that the diagnostic is about.</param>
	/// <param name="message">The diagnostic message.</param>
	/// <returns>The diagnostic that was created.</returns>
	public Diagnostic AddError(SourceSpan source, string message)
	{
		Diagnostic diagnostic = new(Severity.Error, source, message);
		Add(diagnostic);

		return diagnostic;
	}

	/// <summary>Creates a new diagnostic with the severity of a warning.</summary>
	/// <param name="source">The source position that the diagnostic is about.</param>
	/// <param name="message">The diagnostic message.</param>
	/// <returns>The diagnostic that was created.</returns>
	public Diagnostic AddWarning(SourceSpan source, string message)
	{
		Diagnostic diagnostic = new(Severity.Warning, source, message);
		Add(diagnostic);

		return diagnostic;
	}

	/// <summary>Creates a new diagnostic with the severity of a hint.</summary>
	/// <param name="source">The source position that the diagnostic is about.</param>
	/// <param name="message">The diagnostic message.</param>
	/// <returns>The diagnostic that was created.</returns>
	public Diagnostic AddHint(SourceSpan source, string message)
	{
		Diagnostic diagnostic = new(Severity.Hint, source, message);
		Add(diagnostic);

		return diagnostic;
	}
	#endregion
}

namespace OwlDomain.Owlish.Engine.Common.Text.Strings;

/// <summary>
/// 	Represents a fragment of a string.
/// </summary>
public interface IStringFragment
{
	#region Properties
	/// <summary>The value of the fragment.</summary>
	string Value { get; }

	/// <summary>The position of the fragment inside of the piece of text.</summary>
	/// <summary>This is relative to the overall source text, not relative to the string.</summary>
	TextSpan Position { get; }
	#endregion

	#region Methods
	/// <summary>Converts the string fragment to it's <see langword="string"/> representation.</summary>
	/// <returns>The fragment's value.</returns>
	string ToString();
	#endregion
}

/// <summary>
/// 	Represents the base implementation for an <see cref="IStringFragment"/>.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public abstract class BaseStringFragment : IStringFragment
{
	#region Properties
	/// <inheritdoc/>
	public string Value { get; }

	/// <inheritdoc/>
	public TextSpan Position { get; }
	#endregion

	#region Constructors
	/// <summary>Populates the <see cref="BaseStringFragment"/> properties.</summary>
	/// <param name="value">The value of the fragment.</param>
	/// <param name="position">The position of the fragment.</param>
	protected BaseStringFragment(string value, TextSpan position)
	{
		Position = position;
		Value = value;
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public override string ToString() => Value;
	#endregion

	#region Helpers
	private string DebuggerDisplay()
	{
		string type = GetType().Name;

		return $"{type} {{ Value = ({Value}), Position = ({Position}) }}";
	}
	#endregion
}

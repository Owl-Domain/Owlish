namespace OwlDomain.Owlish.Engine.Input;

/// <summary>
/// 	Represents the result of the text input handling a key.
/// </summary>
public enum TextInputResult
{
	/// <summary>No difference was made to the input.</summary>
	None,

	/// <summary>The caret has been moved.</summary>
	/// <remarks>If a caret was moved and the text has changed then <see cref="TextChanged"/> will be returned instead.</remarks>
	CaretMoved,

	/// <summary>The text has been changed and needs to be re-rendered.</summary>
	TextChanged,

	/// <summary>The user indicated for the application to stop.</summary>
	Exit,

	/// <summary>The text input has been completed.</summary>
	Complete,
}

/// <summary>
/// 	Represents a wrapper around <see cref="ITextInput"/> for console
/// </summary>
public class ConsoleTextInput
{
	#region Properties
	/// <summary>The text input that is being wrapped.</summary>
	public ITextInput Input { get; }

	/// <summary>The configuration that dictates which console keys do what.</summary>
	public ConsoleKeyConfiguration Configuration { get; }
	#endregion

	#region Constructors
	/// <summary>Creates a new <see cref="ConsoleTextInput"/>.</summary>
	/// <param name="input">The text input to wrap.</param>
	/// <param name="configuration">The configuration that dictates which console keys do what.</param>
	public ConsoleTextInput(ITextInput input, ConsoleKeyConfiguration configuration)
	{
		Input = input;
		Configuration = configuration;
	}
	#endregion

	#region Methods
	/// <summary>Handles the given <paramref name="key"/> input.</summary>
	/// <param name="key">The key to handle.</param>
	/// <returns>The result of handling the <paramref name="key"/>.</returns>
	public TextInputResult Handle(ConsoleKeyInfo key)
	{
		#region Movement
		if (key.IsMatch(Configuration.MoveLeft))
			return Input.MoveLeft() ? TextInputResult.CaretMoved : TextInputResult.None;

		if (key.IsMatch(Configuration.MoveRight))
			return Input.MoveRight() ? TextInputResult.CaretMoved : TextInputResult.None;

		if (key.IsMatch(Configuration.MoveToStart))
			return Input.MoveToStart() ? TextInputResult.CaretMoved : TextInputResult.None;

		if (key.IsMatch(Configuration.MoveToEnd))
			return Input.MoveToEnd() ? TextInputResult.CaretMoved : TextInputResult.None;
		#endregion

		#region Editing
		if (key.IsMatch(Configuration.DeleteBefore))
			return Input.DeleteBefore() ? TextInputResult.TextChanged : TextInputResult.None;

		if (key.IsMatch(Configuration.DeleteAfter))
			return Input.DeleteAfter() ? TextInputResult.TextChanged : TextInputResult.None;
		#endregion

		if (key.IsMatch(Configuration.Accept))
			return TextInputResult.Complete;

		if (key.IsMatch(Configuration.ResetOrCancel))
		{
			if (Input.IsEmpty)
				return TextInputResult.Exit;

			Input.Reset();
			return TextInputResult.TextChanged;
		}

		if (Configuration.AddCheck.Invoke(key))
		{
			Input.Add(key.KeyChar);
			return TextInputResult.TextChanged;
		}

		return TextInputResult.None;
	}
	#endregion
}

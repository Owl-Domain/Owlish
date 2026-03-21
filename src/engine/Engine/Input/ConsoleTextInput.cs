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

	/// <summary>A registered keybind has been invoked.</summary>
	KeybindInvoked,

	/// <summary>The user indicated for the application to stop.</summary>
	Exit,

	/// <summary>The text input has been completed.</summary>
	Complete,
}

/// <summary>
/// 	A delegate that represents the callback invoked from a keybind.
/// </summary>
/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
/// <returns>A task that represents the asynchronous operation.</returns>
public delegate Task KeybindCallback(CancellationToken cancellationToken = default);

/// <summary>
/// 	Represents a wrapper around <see cref="ITextInput"/> for console
/// </summary>
public class ConsoleTextInput
{
	#region Nested types
	private sealed class KeyComparer : EqualityComparer<ConsoleKeyInfo>
	{
		#region Methods
		public override bool Equals(ConsoleKeyInfo x, ConsoleKeyInfo y) => x.IsMatch(y);
		public override int GetHashCode([DisallowNull] ConsoleKeyInfo obj) => HashCode.Combine(obj.Key, obj.Modifiers);
		#endregion
	}
	#endregion

	#region Fields
	private readonly Dictionary<ConsoleKeyInfo, KeybindCallback> _keybinds = new(new KeyComparer());
	#endregion

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
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
	/// <returns>The result of handling the <paramref name="key"/>.</returns>
	public async Task<TextInputResult> HandleAsync(ConsoleKeyInfo key, CancellationToken cancellationToken = default)
	{
		if (_keybinds.TryGetValue(key, out KeybindCallback? callback))
		{
			await callback.Invoke(cancellationToken);
			return TextInputResult.KeybindInvoked;
		}

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

	/// <summary>Registers a <paramref name="keybind"/> that will invoke the given <paramref name="callback"/>.</summary>
	/// <param name="keybind">The keybind to check for.</param>
	/// <param name="callback">The callback to invoke when the keybind activates.</param>
	/// <exception cref="ArgumentException">Thrown if the given <paramref name="keybind"/> has already been registered.</exception>
	public void RegisterKeybind(ConsoleKeyInfo keybind, KeybindCallback callback)
	{
		if (_keybinds.TryAdd(keybind, callback) is false)
			ThrowHelper.ThrowArgumentException(nameof(keybind), "The given keybind has already been registered.");
	}

	/// <summary>Unregisters the given <paramref name="keybind"/>.</summary>
	/// <param name="keybind">The keybind to unregister.</param>
	/// <returns><see langword="true"/> if the given <paramref name="keybind"/> was unregistered, <see langword="false"/> if there was nothing to unregister.</returns>
	public bool UnregisterKeybind(ConsoleKeyInfo keybind) => _keybinds.Remove(keybind);
	#endregion
}

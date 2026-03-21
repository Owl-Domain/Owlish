namespace OwlDomain.Owlish.Engine.Input;

/// <summary>
/// 	Represents a configuration that matches common console keys to their respective actions.
/// </summary>
public class ConsoleKeyConfiguration
{
	#region Properties
	/// <summary>The key for moving the caret one character to the left.</summary>
	public ConsoleKeyInfo MoveLeft { get; set; } = New(ConsoleKey.LeftArrow);

	/// <summary>The key for moving the caret one character to the right.</summary>
	public ConsoleKeyInfo MoveRight { get; set; } = New(ConsoleKey.RightArrow);

	/// <summary>The key for moving the caret to the start.</summary>
	public ConsoleKeyInfo MoveToStart { get; set; } = New(ConsoleKey.Home);

	/// <summary>The key for moving the caret to the end.</summary>
	public ConsoleKeyInfo MoveToEnd { get; set; } = New(ConsoleKey.End);

	/// <summary>The key for delete one character before the caret.</summary>
	public ConsoleKeyInfo DeleteBefore { get; set; } = New(ConsoleKey.Backspace);

	/// <summary>The key for delete one character after the caret.</summary>
	public ConsoleKeyInfo DeleteAfter { get; set; } = New(ConsoleKey.Delete);

	/// <summary>The key for accepting the input.</summary>
	public ConsoleKeyInfo Accept { get; set; } = New(ConsoleKey.Enter);

	/// <summary>The key for resetting the current input or cancelling the overall input.</summary>
	public ConsoleKeyInfo ResetOrCancel { get; set; } = New(ConsoleModifiers.Control, ConsoleKey.C);

	/// <summary>The predicate used to check whether the character from a key should be added to the input.</summary>
	public Predicate<ConsoleKeyInfo> AddCheck { get; set; } = (key) => char.IsControl(key.KeyChar) is false;

	/// <summary>The key for navigating up in the history.</summary>
	public ConsoleKeyInfo UpHistory { get; } = New(ConsoleKey.UpArrow);

	/// <summary>The key for navigating down in the history.</summary>
	public ConsoleKeyInfo DownHistory { get; } = New(ConsoleKey.DownArrow);
	#endregion

	#region Helpers
	private static ConsoleKeyInfo New(ConsoleKey key) => New(ConsoleModifiers.None, key);
	private static ConsoleKeyInfo New(ConsoleModifiers modifiers, ConsoleKey key)
	{
		bool shift = modifiers.HasFlag(ConsoleModifiers.Shift);
		bool alt = modifiers.HasFlag(ConsoleModifiers.Alt);
		bool ctrl = modifiers.HasFlag(ConsoleModifiers.Control);

		return new('\0', key, shift, alt, ctrl);
	}
	#endregion
}

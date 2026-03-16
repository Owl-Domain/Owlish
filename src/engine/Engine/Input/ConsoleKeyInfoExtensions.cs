namespace OwlDomain.Owlish.Engine.Input;

/// <summary>
/// 	Contains various extension methods related to the <see cref="ConsoleKeyInfo"/>.
/// </summary>
public static class ConsoleKeyInfoExtensions
{
	extension(ConsoleKeyInfo info)
	{
		/// <summary>
		/// 	Checks whether the key <paramref name="info"/> has the given
		/// 	<paramref name="modifiers"/> and is for the given <paramref name="key"/>.
		/// </summary>
		/// <param name="modifiers">The key modifiers to check for.</param>
		/// <param name="key">The key to check for.</param>
		/// <returns>
		/// 	<see langword="true"/> if the given key <paramref name="info"/> has the expected
		/// 	<paramref name="modifiers"/> and <paramref name="key"/>, <see langword="false"/> otherwise.
		/// </returns>
		public bool IsMatch(ConsoleModifiers modifiers, ConsoleKey key)
		{
			return
				info.Modifiers == modifiers &&
				info.Key == key;
		}

		/// <summary>
		/// 	Checks whether the key <paramref name="info"/> has the given
		/// 	<paramref name="key"/> without any key modifiers.
		/// </summary>
		/// <param name="key">The key to check for.</param>
		/// <returns>
		/// 	<see langword="true"/> if the given key <paramref name="info"/> has
		/// 	the expected <paramref name="key"/> and no key modifiers.
		/// </returns>
		public bool IsMatch(ConsoleKey key)
		{
			return
				info.Modifiers is ConsoleModifiers.None &&
				info.Key == key;
		}
	}
}

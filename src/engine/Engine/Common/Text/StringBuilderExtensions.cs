using System.Text;

namespace OwlDomain.Owlish.Engine.Common.Text;

/// <summary>
/// 	Contains various extension methods related to the <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
	extension(StringBuilder builder)
	{
		#region Methods
		/// <summary>Converts the builder to a string and then clears it.</summary>
		/// <returns>The string that the builder created.</returns>
		public string ToStringAndClear()
		{
			string result = builder.ToString();
			builder.Clear();

			return result;
		}
		#endregion
	}
}

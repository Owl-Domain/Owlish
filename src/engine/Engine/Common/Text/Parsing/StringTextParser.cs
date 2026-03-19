using System.Buffers;
using System.Globalization;

namespace OwlDomain.Owlish.Engine.Common.Text.Parsing;

/// <summary>
/// 	Represents a general text parser for <see langword="string"/> values.
/// </summary>
public sealed class StringTextParser : BaseTextParser, IDisposable
{
	#region Fields
	private readonly string _text;
	private readonly ReadOnlyMemory<int> _lookup;
	private readonly int[] _rentedBuffer;
	private int _lookupIndex = 0;
	#endregion

	#region Properties
	/// <inheritdoc/>
	public override bool IsAtEnd => _lookupIndex >= _lookup.Length;
	#endregion

	#region Constructors
	/// <summary>Creates a new instance of the <see cref="StringTextParser"/>.</summary>
	/// <param name="text">The text to parse.</param>
	public StringTextParser(string text)
	{
		_text = text;
		_lookup = ParseCombiningCharacters(text, out _rentedBuffer);
	}
	#endregion

	#region Methods
	/// <inheritdoc/>
	public override bool TryPeek(int offset, out TextElement element)
	{
		int lookupIndex = _lookupIndex + offset;

		if (lookupIndex < 0 || lookupIndex >= _lookup.Length)
		{
			element = default;
			return false;
		}

		int stringIndex = _lookup.Span[lookupIndex];
		ReadOnlySpan<char> slice;

		if (lookupIndex == _lookup.Length - 1)
			slice = _text.AsSpan(stringIndex);
		else
		{
			int end = _lookup.Span[lookupIndex + 1];
			slice = _text.AsSpan(stringIndex, end - stringIndex);
		}

		element = new(slice);
		return true;
	}

	/// <inheritdoc/>
	public override ReadOnlySpan<char> GetSlice(int offset, int amount)
	{
		int lookupIndex = _lookupIndex + offset;
		int endIndex = lookupIndex + amount;

		Guard.IsInRange(lookupIndex, 0, _lookup.Length);
		Guard.IsGreaterThanOrEqualTo(amount, 0);
		Guard.IsLessThanOrEqualTo(endIndex, _lookup.Length);

		int stringIndex = _lookup.Span[lookupIndex];
		if (endIndex == _lookup.Length)
			return _text.AsSpan(stringIndex);

		int end = _lookup.Span[lookupIndex + amount];
		return _text.AsSpan(stringIndex, end - stringIndex);
	}

	/// <inheritdoc/>
	protected override int AdvanceCore(int amount)
	{
		Debug.Assert(amount > 0);

		int remaining = _lookup.Length - _lookupIndex;
		int actual = Math.Min(amount, remaining);

		_lookupIndex += actual;
		return actual;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		ArrayPool<int>.Shared.Return(_rentedBuffer);
	}
	#endregion

	#region Helpers
	private static ReadOnlyMemory<int> ParseCombiningCharacters(ReadOnlySpan<char> span, out int[] rentedBuffer)
	{
		int used = 0;
		int offset = 0;

		rentedBuffer = ArrayPool<int>.Shared.Rent(span.Length);
		Memory<int> buffer = rentedBuffer;

		while (span.IsEmpty is false)
		{
			buffer.Span[used++] = offset;
			int length = StringInfo.GetNextTextElementLength(span);

			offset += length;
			span = span[length..];
		}

		return buffer[..used];
	}
	#endregion
}

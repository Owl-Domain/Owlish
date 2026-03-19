namespace OwlDomain.Owlish.Engine.Common.Text.Parsing;

/// <summary>
/// 	Represents the base implementation for a general text parser.
/// </summary>
public abstract class BaseTextParser : ITextParser
{
	#region Fields
	private int _offset, _line = 1, _column = 1;
	#endregion

	#region Properties
	/// <inheritdoc/>
	public TextElement Current => Peek(0);

	/// <inheritdoc/>
	public TextElement Next => Peek(1);

	/// <inheritdoc/>
	public TextPosition Position => new(_offset, _line, _column);

	/// <inheritdoc/>
	public abstract bool IsAtEnd { get; }

	/// <inheritdoc/>
	public virtual bool HasRemaining => IsAtEnd is false;

	/// <inheritdoc/>
	public bool IsAtStartOfLine => _column is 1;
	#endregion

	#region Methods
	/// <inheritdoc/>
	public void Advance(int amount = 1)
	{
		Guard.IsGreaterThanOrEqualTo(amount, 1);

		if (IsAtEnd)
			return;

		int actualAmount = AdvanceCore(amount);

		_offset += actualAmount;
		_column += actualAmount;
	}

	/// <summary>The amount of text elements to advance the parser's position by.</summary>
	/// <param name="amount">The amount of text elements to advance the parser's position by.</param>
	/// <returns>The actual amount of text elements that the parser was advanced by.</returns>
	/// <remarks>The given <paramref name="amount"/> will never be less than one.</remarks>
	protected abstract int AdvanceCore(int amount);

	/// <inheritdoc/>
	public TextElement Peek(int offset)
	{
		if (TryPeek(offset, out TextElement element))
			return element;

		return default;
	}

	/// <inheritdoc/>
	public abstract bool TryPeek(int offset, out TextElement element);

	/// <inheritdoc/>
	public abstract ReadOnlySpan<char> GetSlice(int offset, int amount);

	/// <inheritdoc/>
	public virtual bool Match(TextElement element)
	{
		if (Current == element)
		{
			Advance();
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public virtual bool Match(char character)
	{
		if (Current == character)
		{
			Advance();
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public virtual bool Match(ReadOnlySpan<char> span)
	{
		int offset = 0;

		while (span.IsEmpty is false)
		{
			TextElement current = span.GetNextTextElement(out ReadOnlySpan<char> remaining);

			if (Peek(offset) != current)
				return false;

			span = remaining;
			offset++;
		}

		Advance(offset);
		return true;
	}

	/// <inheritdoc/>
	public virtual void MarkNewLine()
	{
		_line++;
		_column = 1;
	}

	/// <inheritdoc/>
	public TextElement Consume()
	{
		TextElement current = Current;

		Advance();

		return current;
	}

	/// <inheritdoc/>
	public bool TryConsume(out TextElement element)
	{
		if (IsAtEnd)
		{
			element = default;
			return false;
		}

		element = Current;
		Advance();

		return true;
	}
	#endregion
}

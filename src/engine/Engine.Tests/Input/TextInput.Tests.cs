using OwlDomain.Owlish.Engine.Input;

namespace OwlDomain.Owlish.Engine.Tests.Input;

[TestClass]
public class TextInputTests
{
	#region Tests
	[TestMethod]
	public void Constructor_ExpectedInitialState()
	{
		// Act
		TextInput sut = new();

		// Assert
		Assert.IsEmpty(sut.Characters);
		Assert.AreEqual(0, sut.Position);
		Assert.IsTrue(sut.IsAtStart);
		Assert.IsTrue(sut.IsAtEnd);
		Assert.IsTrue(sut.IsEmpty);
	}

	[TestMethod]
	public void ToString_Empty_ReturnsEmptyString()
	{
		// Arrange
		TextInput sut = new();
		IsEmpty(sut);

		// Act
		string result = sut.ToString();

		// Assert
		Assert.AreEqual(string.Empty, result);
	}

	[TestMethod]
	public void ToString_WithCharacters_ReturnsExpected()
	{
		// Arrange
		const string expected = "123";
		TextInput sut = new();
		sut.Add('1');
		sut.Add('2');
		sut.Add('3');

		IsNotEmpty(sut);

		// Act
		string result = sut.ToString();

		// Assert
		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	public void Reset_IsEmpty_DoesNothing()
	{
		// Arrange
		TextInput sut = new();

		IsAtStart(sut);
		IsAtEnd(sut);
		IsEmpty(sut);

		// Act
		sut.Reset();

		// Assert
		Assert.IsTrue(sut.IsAtStart);
		Assert.IsTrue(sut.IsAtEnd);
		Assert.IsTrue(sut.IsEmpty);
	}
	#endregion

	#region Movement tests
	[TestMethod]
	public void MoveLeft_AtStart_ReturnsFalse()
	{
		// Arrange
		TextInput sut = new();
		int startPosition = sut.Position;

		IsAtStart(sut);

		// Act
		bool result = sut.MoveLeft();

		// Assert
		Assert.IsFalse(result);
		Assert.AreEqual(startPosition, sut.Position);
	}

	[TestMethod]
	public void MoveLeft_NotAtStart_ReturnsTrue()
	{
		// Arrange
		TextInput sut = new();
		sut.Add('1'); // Ensure input is not empty.

		int startPosition = sut.Position;
		IsNotAtStart(sut);

		// Act
		bool result = sut.MoveLeft();

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(startPosition - 1, sut.Position);
		Assert.IsTrue(sut.IsAtStart);
	}

	[TestMethod]
	public void MoveRight_AtEnd_ReturnsFalse()
	{
		// Arrange
		TextInput sut = new();
		int startPosition = sut.Position;

		IsAtEnd(sut);

		// Act
		bool result = sut.MoveRight();

		// Assert
		Assert.IsFalse(result);
		Assert.AreEqual(startPosition, sut.Position);
		Assert.IsTrue(sut.IsAtEnd);
	}

	[TestMethod]
	public void MoveRight_NotAtEnd_ReturnsTrue()
	{
		// Arrange
		TextInput sut = new();
		sut.Add('1'); // Ensure input is not empty.
		sut.MoveLeft(); // Ensure input is no longer at the end.

		int startPosition = sut.Position;
		IsNotAtEnd(sut);

		// Act
		bool result = sut.MoveRight();

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(startPosition + 1, sut.Position);
		Assert.IsTrue(sut.IsAtEnd);
	}

	[TestMethod]
	public void MoveToStart_AtStart_ReturnsFalse()
	{
		// Arrange
		TextInput sut = new();
		int startPosition = sut.Position;

		IsAtStart(sut);

		// Act
		bool result = sut.MoveToStart();

		// Assert
		Assert.IsFalse(result);
		Assert.AreEqual(startPosition, sut.Position);
	}

	[TestMethod]
	public void MoveToStart_NotAtStart_ReturnsTrue()
	{
		// Arrange
		TextInput sut = new();
		sut.Add('1'); // ensure input is not at the start.

		IsNotAtStart(sut);

		// Act
		bool result = sut.MoveToStart();

		// Assert
		Assert.IsTrue(result);
		Assert.IsTrue(sut.IsAtStart);
		Assert.AreEqual(0, sut.Position);
	}

	[TestMethod]
	public void MoveToEnd_AtEnd_ReturnsFalse()
	{
		// Arrange
		TextInput sut = new();
		int startPosition = sut.Position;

		IsAtEnd(sut);

		// Act
		bool result = sut.MoveToEnd();

		// Assert
		Assert.IsFalse(result);
		Assert.AreEqual(startPosition, sut.Position);
	}

	[TestMethod]
	public void MoveToEnd_NotAtEnd_ReturnsTrue()
	{
		// Arrange
		TextInput sut = new();
		sut.Add('1'); // ensure input is not empty.
		sut.MoveLeft(); // ensure input is not at the end.

		IsNotAtEnd(sut);

		// Act
		bool result = sut.MoveToEnd();

		// Assert
		Assert.IsTrue(result);
		Assert.IsTrue(sut.IsAtEnd);
		Assert.AreEqual(sut.Length, sut.Position);
	}
	#endregion

	#region Edit tests
	[TestMethod]
	public void Add_AddsCharactersAtPosition()
	{
		// Arrange
		const char character = '1';

		TextInput sut = new();

		int startPosition = sut.Position;
		IsEmpty(sut);
		IsAtStart(sut);

		// Act
		sut.Add(character);

		// Assert
		Assert.AreEqual(startPosition + 1, sut.Position);
		Assert.AreEqual(1, sut.Length);
		Assert.AreEqual(character, sut[0]);
		Assert.IsTrue(sut.IsAtEnd);
		Assert.IsFalse(sut.IsEmpty);
		Assert.IsFalse(sut.IsAtStart);
	}

	[TestMethod]
	public void DeleteBefore_AtStart_ReturnsFalse()
	{
		// Arrange
		TextInput sut = new();
		IsAtStart(sut);

		// Act
		bool result = sut.DeleteBefore();

		// Assert
		Assert.IsFalse(result);
		Assert.IsTrue(sut.IsAtStart);
	}

	[TestMethod]
	public void DeleteBefore_NotAtStart_ReturnsTrue()
	{
		// Arrange
		TextInput sut = new();
		sut.Add('1'); // Ensure input is not at start.

		int startPosition = sut.Position;
		IsNotAtStart(sut);

		// Act
		bool result = sut.DeleteBefore();

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(startPosition - 1, sut.Position);
	}

	[TestMethod]
	public void DeleteAfter_IsAtEnd_ReturnsFalse()
	{
		// Arrange
		TextInput sut = new();
		IsAtEnd(sut);

		// Act
		bool result = sut.DeleteAfter();

		// Assert
		Assert.IsFalse(result);
		Assert.IsTrue(sut.IsAtEnd);
	}

	[TestMethod]
	public void DeleteAfter_NotAtEnd_ReturnsTrue()
	{
		// Arrange
		TextInput sut = new();
		sut.Add('1'); // Ensure input is not empty.
		sut.MoveLeft(); // Ensure input is not at the end.

		int startPosition = sut.Position;
		IsNotAtEnd(sut);

		// Act
		bool result = sut.DeleteAfter();

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(startPosition, sut.Position);
	}
	#endregion

	#region Helpers
	[ExcludeFromCodeCoverage]
	private void IsAtStart(TextInput input)
	{
		if (input.IsAtStart is false)
			Assert.Inconclusive("The text input should be at the start.");
	}

	[ExcludeFromCodeCoverage]
	private void IsNotAtStart(TextInput input)
	{
		if (input.IsAtStart)
			Assert.Inconclusive("The text input should not be at the start.");
	}

	[ExcludeFromCodeCoverage]
	private void IsAtEnd(TextInput input)
	{
		if (input.IsAtEnd is false)
			Assert.Inconclusive("The text input should be at the end.");
	}

	[ExcludeFromCodeCoverage]
	private void IsNotAtEnd(TextInput input)
	{
		if (input.IsAtEnd)
			Assert.Inconclusive("The text input should not be at the end.");
	}

	[ExcludeFromCodeCoverage]
	private void IsEmpty(TextInput input)
	{
		if (input.IsEmpty is false)
			Assert.Inconclusive("The text input should be empty.");
	}

	[ExcludeFromCodeCoverage]
	private void IsNotEmpty(TextInput input)
	{
		if (input.IsEmpty)
			Assert.Inconclusive("The text input should not be empty.");
	}
	#endregion
}

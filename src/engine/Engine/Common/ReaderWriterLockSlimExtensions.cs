namespace OwlDomain.Owlish.Engine.Common;

/// <summary>
/// 	Represents a read lock around the given <paramref name="readerWriterLock"/>.
/// </summary>
/// <param name="readerWriterLock">The reader/writer lock.</param>
public readonly ref struct ReadLock(ReaderWriterLockSlim readerWriterLock) : IDisposable
{
	#region Properties
	/// <inheritdoc/>
	public void Dispose() => readerWriterLock.ExitReadLock();
	#endregion
}

/// <summary>
/// 	Represents a write lock around the given <paramref name="readerWriterLock"/>.
/// </summary>
/// <param name="readerWriterLock">The reader/writer lock.</param>
public readonly ref struct WriteLock(ReaderWriterLockSlim readerWriterLock) : IDisposable
{
	#region Properties
	/// <inheritdoc/>
	public void Dispose() => readerWriterLock.ExitWriteLock();
	#endregion
}

/// <summary>
/// 	Represents an upgradeable read lock around the given <paramref name="readerWriterLock"/>.
/// </summary>
/// <param name="readerWriterLock">The reader/writer lock.</param>
public readonly ref struct UpgradeableReadLock(ReaderWriterLockSlim readerWriterLock) : IDisposable
{
	#region Properties
	/// <inheritdoc/>
	public void Dispose() => readerWriterLock.ExitUpgradeableReadLock();
	#endregion
}

/// <summary>
/// 	Contains various extension methods related to the <see cref="ReaderWriterLockSlim"/>.
/// </summary>
public static class ReaderWriterLockSlimExtensions
{
	extension(ReaderWriterLockSlim slim)
	{
		#region Methods
		/// <summary>Enters a read lock.</summary>
		/// <returns>A disposable which will exit the read lock.</returns>
		public ReadLock ReadLock()
		{
			slim.EnterReadLock();
			return new(slim);
		}

		/// <summary>Enters a write lock.</summary>
		/// <returns>A disposable which will exit the write lock.</returns>
		public WriteLock WriteLock()
		{
			slim.EnterWriteLock();
			return new(slim);
		}

		/// <summary>Enters an upgradeable read lock.</summary>
		/// <returns>A disposable which will exit the upgradeable read lock.</returns>
		public UpgradeableReadLock UpgradeableReadLock()
		{
			slim.EnterUpgradeableReadLock();
			return new(slim);
		}
		#endregion
	}
}

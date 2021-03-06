﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SimpleMvvm.Common
{
	public static class ReaderWriterLockExt
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDisposable ReadLock(this ReaderWriterLockSlim readerWriterLock)
		{
			Guard.NotNull(readerWriterLock);

			readerWriterLock.EnterReadLock();

			return new DisposeAction(readerWriterLock.ExitReadLock);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDisposable UpgradeableReadLock(this ReaderWriterLockSlim readerWriterLock)
		{
			Guard.NotNull(readerWriterLock);

			readerWriterLock.EnterUpgradeableReadLock();

			return new DisposeAction(readerWriterLock.ExitUpgradeableReadLock);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDisposable WriteLock(this ReaderWriterLockSlim readerWriterLock)
		{
			Guard.NotNull(readerWriterLock);

			readerWriterLock.EnterWriteLock();

			return new DisposeAction(readerWriterLock.ExitWriteLock);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IDisposable WriteLockIfRequired(this ReaderWriterLockSlim readerWriterLock, ref IDisposable release)
		{
			Guard.NotNull(readerWriterLock);

			if (release != null)
			{
				if (!readerWriterLock.IsWriteLockHeld)
				{
					throw new Exception("Release handler is not null but the write lock did not held");
				}

				return release;
			}

			release = readerWriterLock.WriteLock();
			return release;
		}
	}
}

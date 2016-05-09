using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleMvvm
{
	public static class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInRange(this int value, int min, int max)
		{
			return value >= min && value <= max;
		}

		/// <summary>
		/// Join two lines into one string if necessary, the null or whitespace line will be skipped.
		/// </summary>
		/// <param name="firstLine">first line</param>
		/// <param name="secondLine">second line</param>
		/// <returns>The result of the concat, if the both parameters were skipped a null will be returned</returns>
		public static string JoinLine(this string firstLine, string secondLine)
		{
			var lines = new List<string>();

			if(!string.IsNullOrWhiteSpace(firstLine))
			{
				lines.Add(firstLine);
			}

			if(!string.IsNullOrWhiteSpace(secondLine))
			{
				lines.Add(secondLine);
			}

			if(lines.Count == 0)
			{
				return null;
			}

			return string.Join(Environment.NewLine, lines);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ExecuteSafe(this ICommand command, object parameter = null)
		{
			if (command == null)
			{
				return;
			}

			if (command.CanExecute(parameter))
			{
				command.Execute(parameter);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TOut As<TOut>(this object obj)
			where TOut : class
		{
			return obj as TOut;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TOut With<TIn, TOut>(this TIn obj, Func<TIn, TOut> func, TOut defaultValue = default(TOut))
		   where TIn : class
		{
			Guard.NotNull(func, nameof(func));

			return obj != null
				? func(obj)
				: defaultValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Exception Unwrap(this Exception ex)
		{
			var aggregateException = ex as AggregateException;
			if (aggregateException != null)
			{
				return aggregateException.InnerException.Unwrap();
			}

			return ex;
		}

		public static async void LogAsyncError(this Task task)
		{
			if(task == null)
			{
				return;
			}

			try
			{
				await task;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

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

using System;
using System.Runtime.CompilerServices;

namespace SimpleMvvm
{
	public static class Guard
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotNull<T>(T value, string paramName = "", string message = "")
			where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(paramName, message);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotNullAndEmpty(string value, string paramName = "", string message = "")
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(paramName, message);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NotNullAndWhiteSpace(string value, string paramName = "", string message = "")
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException(paramName, message);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InRange<T>(T value, T minValue, T maxValue, string paramName = "", string message = "")
			where T : struct, IComparable<T>
		{
			if(value.CompareTo(minValue) < 0
				|| value.CompareTo(maxValue) > 0)
			{
				throw new ArgumentOutOfRangeException(paramName, value, message);
			}
		}
	}
}

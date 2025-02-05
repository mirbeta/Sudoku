﻿namespace System;

/// <summary>
/// Provides methods for pointer handling.
/// </summary>
public static unsafe class PointerOperations
{
	/// <summary>
	/// To swap the two variables using pointers when the pointee is an <see langword="unmanaged"/> type.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of the variable.</typeparam>
	/// <param name="left">The left variable.</param>
	/// <param name="right">The right variable.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when <paramref name="left"/> or <paramref name="right"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<TUnmanaged>(TUnmanaged* left, TUnmanaged* right) where TUnmanaged : unmanaged
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		var temp = *left;
		*left = *right;
		*right = temp;
	}

	/// <summary>
	/// Get the length of the specified string which is represented by a <see cref="char"/>*.
	/// </summary>
	/// <param name="ptr">The pointer.</param>
	/// <returns>The total length.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// In C#, this function is unsafe because the implementation of
	/// <see cref="string"/> types between C and C# is totally different.
	/// In C, <see cref="string"/> is like a <see cref="char"/>* or a
	/// <see cref="char"/>[], they ends with the terminator symbol <c>'\0'</c>.
	/// However, C# not.
	/// </remarks>
	public static int StringLengthOf(char* ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		var result = 0;
		for (var p = ptr; *p != '\0'; p++)
		{
			result++;
		}

		return result;
	}

	/// <summary>
	/// Get the length of the specified string which is represented by a <see cref="Utf8Char"/>*.
	/// </summary>
	/// <param name="ptr">The pointer.</param>
	/// <returns>The total length.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// In C#, this function is unsafe because the implementation of
	/// <see cref="Utf8String"/> types between C and C# is totally different.
	/// In C, <see cref="Utf8String"/> is like a <see cref="Utf8Char"/>* or a
	/// <see cref="Utf8Char"/>[], they ends with the terminator symbol <c>'\0'</c>.
	/// However, C# not.
	/// </remarks>
	public static int StringLengthOf(Utf8Char* ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		var result = 0;
		for (var p = ptr; *p != (Utf8Char)'\0'; p++)
		{
			result++;
		}

		return result;
	}

	/// <summary>
	/// Get the new array from the pointer, with the specified start index.
	/// </summary>
	/// <typeparam name="TUnmanaged">
	/// The type of the pointer element. Note that the type should be <see langword="unmanaged"/>
	/// in order to use pointer handling. Therefore, <see langword="managed"/> types shouldn't be allowed.
	/// </typeparam>
	/// <param name="ptr">The pointer.</param>
	/// <param name="length">The length of the array that pointer points to.</param>
	/// <param name="index">The start index that you want to pick from.</param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// For example, the pointer is the address of the first element in an array <c>{ 0, 1, 3, 6, 10 }</c>,
	/// if parameter <paramref name="index"/> is 2, the return array will be <c>{ 3, 6, 10 }</c>. Note that
	/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
	/// 5 elements in this case.
	/// </remarks>
	public static TUnmanaged[] GetArrayFromStart<TUnmanaged>(TUnmanaged* ptr, int length, int index)
		where TUnmanaged : unmanaged
	{
		ArgumentNullException.ThrowIfNull(ptr);

		var result = new TUnmanaged[length - index];
		for (var i = index; i < length; i++)
		{
			result[i - index] = ptr[i];
		}

		return result;
	}

	/// <summary>
	/// Get the new array from the pointer, with the specified start index.
	/// </summary>
	/// <param name="ptr">The pointer.</param>
	/// <param name="length">The length of the array that pointer points to.</param>
	/// <param name="index">The start index that you want to pick from.</param>
	/// <param name="removeTrailingZeros">
	/// Indicates whether the method will remove the trailing zeros. If <see langword="false"/>,
	/// the method will be same as <see cref="GetArrayFromStart{TUnmanaged}(TUnmanaged*, int, int)"/>.
	/// </param>
	/// <returns>The array of elements.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// For example, the pointer is the address of the first element in an array <c>{ 0, 1, 3, 6, 10 }</c>,
	/// if parameter <paramref name="index"/> is 2, the return array will be <c>{ 3, 6, 10 }</c>. Note that
	/// the parameter <paramref name="length"/> should keep the value 5 because the array contains
	/// 5 elements in this case.
	/// </remarks>
	/// <seealso cref="GetArrayFromStart{TUnmanaged}(TUnmanaged*, int, int)"/>
	public static int[] GetArrayFromStart(int* ptr, int length, int index, bool removeTrailingZeros)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		if (removeTrailingZeros)
		{
			var count = 0;
			var p = ptr + length - 1;
			for (var i = length - 1; i >= 0; i--, p--, count++)
			{
				if (*p != 0)
				{
					break;
				}
			}

			var result = new int[length - count - index];
			for (int i = index, iterationLength = length - count; i < iterationLength; i++)
			{
				result[i - index] = ptr[i];
			}

			return result;
		}
		else
		{
			return GetArrayFromStart(ptr, length, index);
		}
	}
}

﻿namespace System;

/// <summary>
/// Provides with extension methods on generic type arguments.
/// </summary>
public static class GenericExtensions
{
	/// <summary>
	/// Covers the string value with the current <see cref="Span{T}"/> instance, at the specified index as start.
	/// </summary>
	/// <typeparam name="TStruct">
	/// The type of the element. Due to the limit of C# language, the type must be a <see langword="struct"/>.
	/// </typeparam>
	/// <param name="start">The reference to the space that the string <paramref name="s"/> will be covered.</param>
	/// <param name="s">The string to cover.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Covers<TStruct>(this scoped ref TStruct start, string s) where TStruct : struct
		=> Unsafe.CopyBlock(
			ref Unsafe.As<TStruct, byte>(ref start),
			ref Unsafe.As<char, byte>(ref Unsafe.AsRef(s[0])),
			(uint)(sizeof(char) * s.Length)
		);
}

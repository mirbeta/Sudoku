﻿namespace System.Linq;

/// <summary>
/// Defines a type that supports
/// <see langword="join"/>-<see langword="in"/>-<see langword="on"/>-<see langword="equals"/> clauses.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public interface IJoinClauseProvider<T> : ILinqProvider<T>
{
	/// <summary>
	/// Correlates the elements of two sequences based on matching keys.
	/// </summary>
	/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
	/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
	/// <typeparam name="TResult">The type of the result elements.</typeparam>
	/// <param name="inner">The sequence to join to the first sequence.</param>
	/// <param name="outerKeySelector">
	/// A function to extract the join key from each element of the first sequence.
	/// </param>
	/// <param name="innerKeySelector">
	/// A function to extract the join key from each element of the second sequence.
	/// </param>
	/// <param name="resultSelector">A function to create a result element from two matching elements.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> that has elements of type TResult that are obtained
	/// by performing an inner join on two sequences.
	/// </returns>
	public abstract IEnumerable<TResult> Join<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		Func<T, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<T, TInner, TResult> resultSelector);

	/// <inheritdoc cref="Join{TInner, TKey, TResult}(IEnumerable{TInner}, Func{T, TKey}, Func{TInner, TKey}, Func{T, TInner, TResult})"/>
	public sealed unsafe IEnumerable<TResult> JoinUnsafe<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		delegate*<T, TKey> outerKeySelector,
		delegate*<TInner, TKey> innerKeySelector,
		delegate*<T, TInner, TResult> resultSelector)
		=> Join(inner, e => outerKeySelector(e), e => innerKeySelector(e), (outer, inner) => resultSelector(outer, inner));
}

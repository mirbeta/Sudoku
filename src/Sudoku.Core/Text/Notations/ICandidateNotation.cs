﻿namespace Sudoku.Text.Notations;

/// <summary>
/// Defines a type that can convert a <see cref="Candidates"/> instance into a result <see cref="string"/>
/// representation to describe the candidate collection.
/// </summary>
/// <typeparam name="TSelf">The base type that applies the interface.</typeparam>
/// <typeparam name="TOptions">The type that is used as the provider for extra options.</typeparam>
public interface ICandidateNotation<[Self] TSelf, TOptions>
	where TSelf : class, ICandidateNotation<TSelf, TOptions>
	where TOptions : struct, INotationOptions<TOptions>
{
	/// <summary>
	/// Indicates the candidate notation kind that the current type supports.
	/// </summary>
	public static abstract CandidateNotation CandidateNotation { get; }


	/// <summary>
	/// <para>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="Candidates"/>
	/// instance.
	/// </para>
	/// <para>
	/// Different with the method <see cref="ParseCandidates(string)"/>, the method will return a
	/// <see cref="bool"/> value instead, indicating whether the operation is successful. Therefore,
	/// the method won't throw <see cref="FormatException"/>.
	/// </para>
	/// </summary>
	/// <param name="str">The <see cref="string"/> value.</param>
	/// <param name="result">
	/// The <see cref="Candidates"/> result. If the return value is <see langword="false"/>,
	/// this argument will be a discard and cannot be used.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the parsing operation is successful.</returns>
	/// <seealso cref="ParseCandidates(string)"/>
	public static abstract bool TryParseCandidates(string str, out Candidates result);

	/// <summary>
	/// Gets the <see cref="string"/> representation of a list of candidates.
	/// </summary>
	/// <param name="candidates">The candidate list.</param>
	/// <returns>The <see cref="string"/> representation describe the candidate list.</returns>
	public static abstract string ToCandidatesString(scoped in Candidates candidates);

	/// <summary>
	/// Gets the <see cref="string"/> representation of a list of candidates.
	/// </summary>
	/// <param name="candidates">The candidates list.</param>
	/// <param name="options">The extra options to control the output style.</param>
	/// <returns>The <see cref="string"/> representation describe the candidate list.</returns>
	public static abstract string ToCandidatesString(scoped in Candidates candidates, scoped in TOptions options);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="Candidates"/>
	/// instance.
	/// </summary>
	/// <param name="str">The <see cref="string"/> value.</param>
	/// <returns>The <see cref="Candidates"/> result.</returns>
	/// <exception cref="FormatException">
	/// Throws when the parsing operation is failed due to invalid characters or invalid operation.
	/// </exception>
	public static abstract Candidates ParseCandidates(string str);
}

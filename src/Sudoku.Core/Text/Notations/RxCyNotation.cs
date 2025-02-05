﻿namespace Sudoku.Text.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using RxCy notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
/// <remarks>
/// <para>
/// The <b>RxCy notation</b> is a notation to describe a set of cells that uses letter
/// <c>R</c> (or its lower case <c>r</c>) to describe a row label, and uses the other letter
/// <c>C</c> (or its lower case <c>c</c>) to describe a column label. For example,
/// <c>R4C2</c> means the cell at row 4 and column 2.
/// </para>
/// <para>
/// For more information about this concept, please visit
/// <see href="http://sudopedia.enjoysudoku.com/Rncn.html">this link</see>.
/// </para>
/// <para>
/// Please note that the type is an <see langword="abstract"/> type,
/// which means you cannot instantiate any objects. In addition, the type contains
/// a <see langword="private"/> instance constructor, which disallows you deriving any types.
/// </para>
/// </remarks>
public sealed partial class RxCyNotation :
	ICellNotation<RxCyNotation, RxCyNotationOptions>,
	ICandidateNotation<RxCyNotation, RxCyNotationOptions>
{
	/// <summary>
	/// Indicates a pattern that matches for a pre-positional formed candidate list.
	/// </summary>
	[StringSyntax(StringSyntaxAttribute.Regex)]
	private const string PrepositionalFormCandidateList = """[1-9]{1,9}(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}|\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}),\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})*\s*\})""";

	/// <summary>
	/// Indicates a pattern that matches for a post-positional formed candidate list.
	/// </summary>
	[StringSyntax(StringSyntaxAttribute.Regex)]
	private const string PostpositionalFormCandidateList = """\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}),\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})*\s*\}\([1-9]{1,9}\)""";


	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, false, DiagnosticId = "SCA0108", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0108")]
	private RxCyNotation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static CellNotation CellNotation => CellNotation.RxCy;

	/// <inheritdoc/>
	public static CandidateNotation CandidateNotation => CandidateNotation.RxCy;


	/// <summary>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="int"/>
	/// instance, as the cell value.
	/// </summary>
	/// <param name="str">The <see cref="string"/> value.</param>
	/// <param name="result">
	/// The <see cref="int"/> result. If the return value is <see langword="false"/>,
	/// this argument will be a discard and cannot be used.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the parsing operation is successful.</returns>
	public static bool TryParseCell(string str, out int result)
	{
		(result, var @return) = str.Trim() switch
		{
			['R' or 'r', var r and >= '1' and <= '9', 'C' or 'c', var c and >= '1' and <= '9'] => ((r - '1') * 9 + (c - '1'), true),
			_ => (0, false)
		};

		return @return;
	}

	/// <inheritdoc/>
	public static bool TryParseCells(string str, out CellMap result)
	{
		try
		{
			result = ParseCells(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <summary>
	/// Gets the <see cref="string"/> representation of a cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The <see cref="string"/> representation of a cell.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCellString(int cell) => ToCellsString(CellsMap[cell]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCellsString(scoped in CellMap cells) => ToCellsString(cells, RxCyNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCellsString(scoped in CellMap cells, scoped in RxCyNotationOptions options)
	{
		var upperCasing = options.UpperCasing;
		return cells switch
		{
			[] => string.Empty,
			[var p] => $"{rowLabel(upperCasing)}{p / 9 + 1}{columnLabel(upperCasing)}{p % 9 + 1}",
			_ => r(cells, options) is var a && c(cells, options) is var b && a.Length <= b.Length ? a : b
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char rowLabel(bool upperCasing) => upperCasing ? 'R' : 'r';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static char columnLabel(bool upperCasing) => upperCasing ? 'C' : 'c';

		static string i(int v) => (v + 1).ToString();

		static unsafe string r(scoped in CellMap cells, scoped in RxCyNotationOptions options)
		{
			scoped var sbRow = new StringHandler(50);
			var dic = new Dictionary<int, List<int>>(9);
			var (upperCasing, separator) = options;
			foreach (var cell in cells)
			{
				if (!dic.ContainsKey(cell / 9))
				{
					dic.Add(cell / 9, new(9));
				}

				dic[cell / 9].Add(cell % 9);
			}
			foreach (var row in dic.Keys)
			{
				sbRow.Append(rowLabel(upperCasing));
				sbRow.Append(row + 1);
				sbRow.Append(columnLabel(upperCasing));
				sbRow.AppendRange(dic[row], &i);
				sbRow.Append(separator);
			}
			sbRow.RemoveFromEnd(options.Separator.Length);

			return sbRow.ToStringAndClear();
		}

		static unsafe string c(scoped in CellMap cells, scoped in RxCyNotationOptions options)
		{
			var dic = new Dictionary<int, List<int>>(9);
			scoped var sbColumn = new StringHandler(50);
			var (upperCasing, separator) = options;
			foreach (var cell in cells)
			{
				if (!dic.ContainsKey(cell % 9))
				{
					dic.Add(cell % 9, new(9));
				}

				dic[cell % 9].Add(cell / 9);
			}

			foreach (var column in dic.Keys)
			{
				sbColumn.Append(rowLabel(upperCasing));
				sbColumn.AppendRange(dic[column], &i);
				sbColumn.Append(columnLabel(upperCasing));
				sbColumn.Append(column + 1);
				sbColumn.Append(separator);
			}
			sbColumn.RemoveFromEnd(options.Separator.Length);

			return sbColumn.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	public static unsafe CellMap ParseCells(string str)
	{
		// Check whether the match is successful.
		if (CellOrCellListRegex().Matches(str) is not { Count: not 0 } matches)
		{
			throw new FormatException("The specified string can't match any cell instance.");
		}

		// Declare the buffer.
		scoped Span<int> bufferRows = stackalloc int[9], bufferColumns = stackalloc int[9];

		// Declare the result variable.
		var result = CellMap.Empty;

		// Iterate on each match instance.
		foreach (var match in matches.Cast<Match>())
		{
			var value = match.Value;
			char* anchorR, anchorC;
			fixed (char* pValue = value)
			{
				anchorR = anchorC = pValue;

				// Find the index of the character 'C'.
				// The regular expression guaranteed the string must contain the character 'C' or 'c',
				// so we don't need to check '*p != '\0''.
				while (*anchorC is not ('C' or 'c'))
				{
					anchorC++;
				}
			}

			// Stores the possible values into the buffer.
			int rIndex = 0, cIndex = 0;
			for (var p = anchorR + 1; *p is not ('C' or 'c'); p++, rIndex++)
			{
				bufferRows[rIndex] = *p - '1';
			}
			for (var p = anchorC + 1; *p != '\0'; p++, cIndex++)
			{
				bufferColumns[cIndex] = *p - '1';
			}

			// Now combine two buffers.
			for (var i = 0; i < rIndex; i++)
			{
				for (var j = 0; j < cIndex; j++)
				{
					result.Add(bufferRows[i] * 9 + bufferColumns[j]);
				}
			}
		}

		// Returns the result.
		return result;
	}

	/// <inheritdoc/>
	public static bool TryParseCandidates(string str, out Candidates result)
	{
		try
		{
			result = ParseCandidates(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <summary>
	/// Gets the <see cref="string"/> representation of a candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <returns>The <see cref="string"/> representation of a candidate.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCandidateString(int candidate) => $"{ToCellString(candidate / 9)}({candidate % 9 + 1})";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCandidatesString(scoped in Candidates candidates)
		=> ToCandidatesString(candidates, RxCyNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCandidatesString(scoped in Candidates candidates, scoped in RxCyNotationOptions options)
	{
		return candidates switch
		{
			[] => "{ }",
			[var a] when (a / 9, a % 9) is (var c, var d) => $"r{c / 9 + 1}c{c % 9 + 1}({d + 1})",
			_ => f(candidates.ToArray(), options)
		};


		static string f(int[] offsets, scoped in RxCyNotationOptions options)
		{
			scoped var sb = new StringHandler(50);

			foreach (var digitGroup in
				from candidate in offsets
				group candidate by candidate % 9 into digitGroups
				orderby digitGroups.Key
				select digitGroups)
			{
				var cells = CellMap.Empty;
				foreach (var candidate in digitGroup)
				{
					cells.Add(candidate / 9);
				}

				sb.Append(ToCellsString(cells, options));
				sb.Append('(');
				sb.Append(digitGroup.Key + 1);
				sb.Append(')');
				sb.Append(options.Separator);
			}

			sb.RemoveFromEnd(options.Separator.Length);
			return sb.ToStringAndClear();
		}
	}

	/// <inheritdoc/>
	public static unsafe Candidates ParseCandidates(string str)
	{
		// Check whether the match is successful.
		var matches = CandidateOrCandidateListRegex().Matches(str);
		if (matches.Count == 0)
		{
			throw new FormatException("The specified string can't match any candidate instance.");
		}

		var result = Candidates.Empty;

		// Iterate on each match item.
		var bufferDigits = stackalloc int[9];
		foreach (var match in matches.Cast<Match>())
		{
			var value = match.Value;
			if (value.SatisfyPattern(PrepositionalFormCandidateList))
			{
				var cells = CellMap.Parse(value);
				var digitsCount = 0;
				fixed (char* pValue = value)
				{
					for (var ptr = pValue; *ptr is not ('{' or 'R' or 'r'); ptr++)
					{
						bufferDigits[digitsCount++] = *ptr - '1';
					}
				}

				foreach (var cell in cells)
				{
					for (var i = 0; i < digitsCount; i++)
					{
						result.AddAnyway(cell * 9 + bufferDigits[i]);
					}
				}
			}
			else if (value.SatisfyPattern(PostpositionalFormCandidateList))
			{
				var cells = CellMap.Parse(value);
				var digitsCount = 0;
				for (int i = value.IndexOf('(') + 1, length = value.Length; i < length; i++)
				{
					bufferDigits[digitsCount++] = value[i] - '1';
				}

				foreach (var cell in cells)
				{
					for (var i = 0; i < digitsCount; i++)
					{
						result.AddAnyway(cell * 9 + bufferDigits[i]);
					}
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Indicates the regular expression matching a cell or cell-list.
	/// </summary>
	[GeneratedRegex("""(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})""", RegexOptions.Compiled, 5000)]
	private static partial Regex CellOrCellListRegex();

	/// <summary>
	/// Indicates the regular expression matching a candidate or candidate-list.
	/// </summary>
	[GeneratedRegex("""([1-9]{3}|[1-9]{1,9}(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}|\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}),\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})*\s*\})|\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}),\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})*\s*\}\([1-9]{1,9}\))""", RegexOptions.Compiled, 5000)]
	private static partial Regex CandidateOrCandidateListRegex();
}

﻿namespace Sudoku.Text.Parsing;

/// <summary>
/// Encapsulates a grid parser that can parse a <see cref="Utf8String"/> value and convert it
/// into a valid <see cref="Grid"/> instance as the result.
/// </summary>
public unsafe ref struct Utf8GridParser
{
	/// <summary>
	/// The list of all methods to parse.
	/// </summary>
	private static readonly delegate*<ref Utf8GridParser, Grid>[] ParseFunctions;

	/// <summary>
	/// The list of all methods to parse multiple-line grid.
	/// </summary>
	private static readonly delegate*<ref Utf8GridParser, Grid>[] MultilineParseFunctions;


	/// <summary>
	/// Initializes an instance with parsing data.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	public Utf8GridParser(Utf8String parsingValue) : this(parsingValue, false, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <seealso cref="CompatibleFirst"/>
	public Utf8GridParser(Utf8String parsingValue, bool compatibleFirst) : this(parsingValue, compatibleFirst, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <param name="shortenSusser">Indicates the parser will shorten the susser format result.</param>
	/// <seealso cref="CompatibleFirst"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8GridParser(Utf8String parsingValue, bool compatibleFirst, bool shortenSusser)
		=> (ParsingValue, CompatibleFirst, ShortenSusserFormat) = (parsingValue, compatibleFirst, shortenSusser);


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static Utf8GridParser()
	{
		ParseFunctions = new delegate*<ref Utf8GridParser, Grid>[]
		{
			&OnParsingSimpleTable,
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked,
			&onParsingSusser_1,
			&onParsingSusser_2,
			&OnParsingExcel,
			&OnParsingOpenSudoku,
			&onParsingSukaku_1,
			&onParsingSukaku_2
		};

#if GITHUB_ISSUE_216
		// Cannot apply Range syntax '1..3' onto pointer-typed array.
		// Array slicing on pointer type cannot be available for AnyCPU.
		MultilineParseFunctions = ParseFunctions[1..3];
#else
		MultilineParseFunctions = new delegate*<ref Utf8GridParser, Grid>[]
		{
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked
		};
#endif

		static Grid onParsingSukaku_1(ref Utf8GridParser @this) => OnParsingSukaku(ref @this, @this.CompatibleFirst);
		static Grid onParsingSukaku_2(ref Utf8GridParser @this) => OnParsingSukaku(ref @this, !@this.CompatibleFirst);
		static Grid onParsingSusser_1(ref Utf8GridParser @this) => OnParsingSusser(ref @this, !@this.ShortenSusserFormat);
		static Grid onParsingSusser_2(ref Utf8GridParser @this) => OnParsingSusser(ref @this, @this.ShortenSusserFormat);
	}


	/// <summary>
	/// The string value to parse.
	/// </summary>
	public Utf8String ParsingValue { get; private set; }

	/// <summary>
	/// Indicates whether the parser will change the execution order of PM grid.
	/// If the value is <see langword="true"/>, the parser will check compatible one
	/// first, and then check recommended parsing plan ('<c><![CDATA[<d>]]></c>' and '<c>*d*</c>').
	/// </summary>
	public bool CompatibleFirst { get; }

	/// <summary>
	/// Indicates whether the parser will use shorten mode to parse a susser format grid.
	/// If the value is <see langword="true"/>, the parser will omit the continuous empty notation
	/// <c>.</c>s or <c>0</c>s to a <c>*</c>.
	/// </summary>
	public bool ShortenSusserFormat { get; private set; }


	/// <summary>
	/// To parse the value.
	/// </summary>
	/// <returns>The grid.</returns>
	public Grid Parse()
	{
		if (ParsingValue.Length == 729)
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else if (ParsingValue.Contains("-+-"u8))
		{
			foreach (var parseMethod in MultilineParseFunctions)
			{
				if (parseMethod(ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}
		else if (ParsingValue.Contains((Utf8Char)'\t'))
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else
		{
			for (int trial = 0, length = ParseFunctions.Length; trial < length; trial++)
			{
				if (ParseFunctions[trial](ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}

		return Grid.Undefined;
	}

	/// <summary>
	/// To parse the value with a specified grid parsing type.
	/// </summary>
	/// <param name="gridParsingOption">A specified parsing type.</param>
	/// <returns>The grid.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="gridParsingOption"/> is not defined.
	/// </exception>
	public Grid Parse(GridParsingOption gridParsingOption)
		=> gridParsingOption switch
		{
			GridParsingOption.Susser => OnParsingSusser(ref this, false),
			GridParsingOption.ShortenSusser => OnParsingSusser(ref this, true),
			GridParsingOption.Table => OnParsingSimpleMultilineGrid(ref this),
			GridParsingOption.PencilMarked => OnParsingPencilMarked(ref this),
			GridParsingOption.SimpleTable => OnParsingSimpleTable(ref this),
			GridParsingOption.Sukaku => OnParsingSukaku(ref this, false),
			GridParsingOption.SukakuSingleLine => OnParsingSukaku(ref this, true),
			GridParsingOption.Excel => OnParsingExcel(ref this),
			GridParsingOption.OpenSudoku => OnParsingOpenSudoku(ref this),
			_ => throw new ArgumentOutOfRangeException(nameof(gridParsingOption))
		};


	/// <summary>
	/// Parse the value using multi-line simple grid (without any candidates).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSimpleMultilineGrid(ref Utf8GridParser parser)
	{
		var matches = parser.ParsingValue.MatchAll("""(\+?\d|\.)"""u8);
		var length = matches.Length;
		if (length is not (81 or 85))
		{
			// Subtle grid outline will bring 2 '.'s on first line of the grid.
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (var i = 0; i < 81; i++)
		{
			var currentMatch = matches[length - 81 + i];
			switch (currentMatch)
			{
				case [var match and not ('.' or '0')]:
				{
					result[i] = match - '1';
					result.SetStatus(i, CellStatus.Given);

					break;
				}
				case [_]:
				{
					continue;
				}
				case [_, var match]:
				{
					if (match is '.' or '0')
					{
						// '+0' or '+.'? Invalid combination.
						return Grid.Undefined;
					}
					else
					{
						result[i] = match - '1';
						result.SetStatus(i, CellStatus.Modifiable);
					}

					break;
				}
				default:
				{
					// The sub-match contains more than 2 characters or empty string,
					// which is invalid.
					return Grid.Undefined;
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Parse the Excel format.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingExcel(ref Utf8GridParser parser)
	{
		var parsingValue = parser.ParsingValue;
		if (!parsingValue.Contains((Utf8Char)'\t'))
		{
			return Grid.Undefined;
		}

		var values = ((string)parsingValue).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		if (values.Length != 9)
		{
			return Grid.Undefined;
		}

		scoped var sb = new StringHandler(81);
		foreach (var value in values)
		{
			foreach (var digitString in value.Split(new[] { '\t' }))
			{
				sb.Append(string.IsNullOrEmpty(digitString) ? '.' : digitString[0]);
			}
		}

		return Grid.Parse(sb.ToStringAndClear());
	}

	/// <summary>
	/// Parse the open sudoku format grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingOpenSudoku(ref Utf8GridParser parser)
	{
		if (parser.ParsingValue.Match("""\d(\|\d){242}"""u8) is not { } match)
		{
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (var i = 0; i < 81; i++)
		{
			switch (match[i * 6])
			{
				case '0' when whenClause(i * 6, match, "|0|1", "|0|1|"):
				{
					continue;
				}
				case not '0' and var ch when whenClause(i * 6, match, "|0|0", "|0|0|"):
				{
					result[i] = ch - '1';
					result.SetStatus(i, CellStatus.Given);

					break;
				}
				default:
				{
					// Invalid string status.
					return Grid.Undefined;
				}
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool whenClause(int i, string match, string pattern1, string pattern2)
			=> i == 80 * 6 ? match[(i + 1)..(i + 5)] == pattern1 : match[(i + 1)..(i + 6)] == pattern2;
	}

	/// <summary>
	/// Parse the PM grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingPencilMarked(ref Utf8GridParser parser)
	{
		// Older regular expression pattern:
		if (parser.ParsingValue.MatchAll("""(\<\d\>|\*\d\*|\d*[\+\-]?\d+)"""u8) is not { Length: 81 } matches)
		{
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			if (matches[cell] is not { Length: var length and <= 9 } s)
			{
				// More than 9 characters.
				return Grid.Undefined;
			}

			if (s.Contains('<'))
			{
				// All values will be treated as normal characters:
				// '<digit>', '*digit*' and 'candidates'.

				// Givens.
				if (length == 3)
				{
					if (s[1] is var c and >= '1' and <= '9')
					{
						result[cell] = c - '1';
						result.SetStatus(cell, CellStatus.Given);
					}
					else
					{
						// Illegal characters found.
						return Grid.Undefined;
					}
				}
				else
				{
					// The length is not 3.
					return Grid.Undefined;
				}
			}
			else if (s.Contains('*'))
			{
				// Modifiables.
				if (length == 3)
				{
					if (s[1] is var c and >= '1' and <= '9')
					{
						result[cell] = c - '1';
						result.SetStatus(cell, CellStatus.Modifiable);
					}
					else
					{
						// Illegal characters found.
						return Grid.Undefined;
					}
				}
				else
				{
					// The length is not 3.
					return Grid.Undefined;
				}
			}
			else if (s.SatisfyPattern("""[1-9]{1,9}"""))
			{
				// Candidates.
				// Here don't need to check the length of the string,
				// and also all characters are digit characters.
				short mask = 0;
				foreach (var c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return Grid.Undefined;
				}

				if ((mask & mask - 1) == 0)
				{
					result[cell] = TrailingZeroCount(mask);
					result.SetStatus(cell, CellStatus.Given);
				}
				else
				{
					for (var digit = 0; digit < 9; digit++)
					{
						result[cell, digit] = (mask >> digit & 1) != 0;
					}
				}
			}
			else
			{
				// All conditions can't match.
				return Grid.Undefined;
			}
		}

		return result;
	}

	/// <summary>
	/// Parse the simple table format string (Sudoku explainer format).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The grid.</returns>
	private static Grid OnParsingSimpleTable(ref Utf8GridParser parser)
	{
		if (parser.ParsingValue.Match("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}"""u8) is not { } match)
		{
			return Grid.Undefined;
		}

		// Remove all '\r's and '\n's.
		scoped var sb = new StringHandler(81 + (9 << 1));
		sb.AppendCharacters(from @char in match where @char is not ('\r' or '\n') select @char);
		parser.ParsingValue = (Utf8String)sb.ToStringAndClear();
		return OnParsingSusser(ref parser, false);
	}

	/// <summary>
	/// Parse the susser format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="shortenSusser">Indicates whether the parser will shorten the susser format.</param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSusser(ref Utf8GridParser parser, bool shortenSusser)
	{
		var match = shortenSusser
			? parser.ParsingValue.Match("""[\d\.\*]{1,9}(,[\d\.\*]{1,9}){8}"""u8)
			: parser.ParsingValue.Match("""[\d\.\+]{80,}(\:(\d{3}\s+)*\d{3})?"""u8);

		switch (shortenSusser)
		{
			case false when match is not { Length: <= 405 }:
			case true when match is not { Length: <= 81 } || !expandCode(match, out match):
			{
				return Grid.Undefined;
			}
		}

		// Step 1: fills all digits.
		var result = Grid.Empty;
		int i = 0, length = match.Length;
		for (var realPos = 0; i < length && match[i] != ':'; realPos++)
		{
			var c = match[i];
			switch (c)
			{
				case '+':
				{
					// Plus sign means the character after it is a digit,
					// which is modifiable value in the grid in its corresponding position.
					if (i < length - 1)
					{
						if (match[i + 1] is var nextChar and >= '1' and <= '9')
						{
							// Set value.
							// Note that the subtractor is character '1', not '0'.
							result[realPos] = nextChar - '1';

							// Add 2 on iteration variable to skip 2 characters
							// (A plus sign '+' and a digit).
							i += 2;
						}
						else
						{
							// Why isn't the character a digit character?
							return Grid.Undefined;
						}
					}
					else
					{
						return Grid.Undefined;
					}

					break;
				}
				case '.':
				case '0':
				{
					// A placeholder.
					// Do nothing but only move 1 step forward.
					i++;

					break;
				}
				case >= '1' and <= '9':
				{
					// Is a digit character.
					// Digits are representing given values in the grid.
					// Not the plus sign, but a placeholder '0' or '.'.
					// Set value.
					result[realPos] = c - '1';

					// Set the cell status as 'CellStatus.Given'.
					// If the code below doesn't make sense to you,
					// you can see the comments in method 'OnParsingSusser(string)'
					// to know the meaning also.
					result.SetStatus(realPos, CellStatus.Given);

					// Finally moves 1 step forward.
					i++;

					break;
				}
				default:
				{
					// Other invalid characters. Throws an exception.
					//throw Throwing.ParsingError<Grid>(nameof(ParsingValue));
					return Grid.Undefined;
				}
			}
		}

		// Step 2: eliminates candidates if exist.
		// If we have met the colon sign ':', this loop would not be executed.
		if (Grid.ExtendedSusserEliminationsPattern().Match(match) is { Success: true, Value: var elimMatch })
		{
			foreach (var candidate in EliminationNotation.ParseCandidates(elimMatch))
			{
				// Set the candidate with false to eliminate the candidate.
				result[candidate / 9, candidate % 9] = false;
			}
		}

		return result;


		static bool expandCode(string? original, [NotNullWhen(true)] out string? result)
		{
			// We must the string code holds 8 ','s and is with no ':' or '+'.
			if (original is null || original.Contains(':') || original.Contains('+') || original.CountOf(',') != 8)
			{
				result = null;
				return false;
			}

			scoped var resultSpan = (stackalloc char[81]);
			var lines = original.Split(',');
			if (lines.Length != 9)
			{
				result = null;
				return false;
			}

			// Check per line, and expand it.
			var placeholder = original.IndexOf('0') == -1 ? '.' : '0';
			for (var i = 0; i < 9; i++)
			{
				var line = lines[i];
				switch (line.CountOf('*'))
				{
					case 1 when (9 + 1 - line.Length, 0, 0) is var (empties, j, k):
					{
						foreach (var c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, empties).Fill(placeholder);

								j++;
								k += empties;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}

					case var n when (9 + n - line.Length, 0, 0) is var (empties, j, k):
					{
						var emptiesPerStar = empties / n;
						foreach (var c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, emptiesPerStar).Fill(placeholder);

								j++;
								k += emptiesPerStar;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}
				}
			}

			result = resultSpan.ToString();
			return true;
		}
	}

	/// <summary>
	/// Parse the sukaku format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the algorithm uses compatibility mode to check and parse sudoku grid.
	/// </param>
	/// <returns>The result.</returns>
	private static Grid OnParsingSukaku(ref Utf8GridParser parser, bool compatibleFirst)
	{
		const int candidatesCount = 729;
		if (compatibleFirst)
		{
			var parsingValue = parser.ParsingValue;
			if (parsingValue.Length < candidatesCount)
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var i = 0; i < candidatesCount; i++)
			{
				char c = parsingValue[i];
				if (c is not (>= '0' and <= '9' or '.'))
				{
					return Grid.Undefined;
				}

				if (c is '0' or '.')
				{
					result[i / 9, i % 9] = false;
				}
			}

			return result;
		}
		else
		{
			var matches = parser.ParsingValue.MatchAll("""\d*[\-\+]?\d+"""u8);
			if (matches is { Length: not 81 })
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var offset = 0; offset < 81; offset++)
			{
				var s = matches[offset].Reserve(@"\d");
				if (s.Length > 9)
				{
					// More than 9 characters.
					return Grid.Undefined;
				}

				short mask = 0;
				foreach (var c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return Grid.Undefined;
				}

				// We don't need to set the value as a given because the current parsing
				// if for sukakus, rather than normal sudokus.
				//if (IsPow2(mask))
				//{
				//	result[offset] = TrailingZeroCount(mask);
				//	result.SetStatus(offset, CellStatus.Given);
				//}

				for (var digit = 0; digit < 9; digit++)
				{
					result[offset, digit] = (mask >> digit & 1) != 0;
				}
			}

			return result;
		}
	}
}

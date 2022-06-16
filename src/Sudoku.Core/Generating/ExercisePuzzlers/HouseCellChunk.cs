﻿namespace Sudoku.Generating.ExercisePuzzlers;

/// <summary>
/// Defines a chunk that stores the cells in a house.
/// </summary>
public readonly unsafe struct HouseCellChunk :
	IComparable<HouseCellChunk>,
	IComparisonOperators<HouseCellChunk, HouseCellChunk>,
	IEquatable<HouseCellChunk>,
	IEqualityOperators<HouseCellChunk, HouseCellChunk>
{
	/// <summary>
	/// Indicates the mask.
	/// </summary>
	private readonly long _mask;


	/// <summary>
	/// Initializes a <see cref="HouseCellChunk"/> instance via the chunk of 9 values. 0 is for empty cell.
	/// </summary>
	/// <param name="house">Indicates the house used.</param>
	/// <param name="chunkValues">A chunk of values. This argument must hold 9 elements.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HouseCellChunk(int house, int* chunkValues)
		=> _mask = (long)house << 36
			| (long)chunkValues[0] | (long)chunkValues[1] << 4 | (long)chunkValues[2] << 8
			| (long)chunkValues[3] << 12 | (long)chunkValues[4] << 16 | (long)chunkValues[5] << 20
			| (long)chunkValues[6] << 24 | (long)chunkValues[7] << 28 | (long)chunkValues[8] << 32;


	/// <summary>
	/// Indicates the house used.
	/// </summary>
	public int House
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)(_mask >> 36) & 15;
	}

	/// <summary>
	/// Indicates the result candidate.
	/// </summary>
	public int ResultCandidate
	{
		get
		{
			bool* status = stackalloc bool[9];
			Unsafe.InitBlock(status, 0, sizeof(bool) * 9);

			int resultCell = 0;
			for (byte i = 0; i < 9; i++)
			{
				if ((byte)(_mask >> (i << 2) & 15) is var mask and not 0xF)
				{
					status[mask] = true;
				}
				else
				{
					resultCell = i;
				}
			}

			int resultDigit = -1;
			for (byte i = 0; i < 9; i++)
			{
				if (!status[i])
				{
					resultDigit = i;
					break;
				}
			}

			return HouseCells[House][resultCell] * 9 + resultDigit;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is HouseCellChunk comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(HouseCellChunk other) => _mask == other._mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => (int)_mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(object? obj)
	{
		ArgumentNullException.ThrowIfNull(obj);
		Argument.ThrowIfFalse(obj is HouseCellChunk, $"The type must be '{nameof(HouseCellChunk)}'.");

		return CompareTo((HouseCellChunk)obj);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(HouseCellChunk other) => _mask.CompareTo(other._mask);

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		int resultCand = ResultCandidate;
		int cell = resultCand / 9, digit = resultCand % 9;
		string houseStr = new HouseCollection(House).ToString();
		string cellStr = RxCyNotation.ToCellString(cell);
		string digitStr = (digit + 1).ToString();
		return $$"""{{nameof(HouseCellChunk)}} { {{nameof(House)}} = {{houseStr}}, {{nameof(ResultCandidate)}} = {{cellStr}}({{digitStr}}) }""";
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(HouseCellChunk left, HouseCellChunk right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(HouseCellChunk left, HouseCellChunk right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(HouseCellChunk left, HouseCellChunk right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(HouseCellChunk left, HouseCellChunk right) => left.CompareTo(right) >= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(HouseCellChunk left, HouseCellChunk right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(HouseCellChunk left, HouseCellChunk right) => left.CompareTo(right) <= 0;
}
﻿namespace Sudoku.Checking;

/// <summary>
/// Defines a searcher that searches for the true candidates of the current sudoku grid.
/// </summary>
public sealed class TrueCandidatesSearcher
{
	/// <summary>
	/// Initializes an instance with the specified grid.
	/// </summary>
	/// <param name="grid">The current puzzle grid.</param>
	/// <exception cref="InvalidOperationException">Throws when the puzzle is invalid.</exception>
	public TrueCandidatesSearcher(scoped in Grid grid)
	{
		Argument.ThrowIfInvalid(grid.IsValid(), "The puzzle must contain unique solution before checking.");

		Puzzle = grid;
	}


	/// <summary>
	/// Indicates the current grid is a BUG+n pattern.
	/// </summary>
	public bool IsBugPattern => TrueCandidates is not [];

	/// <summary>
	/// The grid.
	/// </summary>
	public Grid Puzzle { get; }

	/// <summary>
	/// Indicates all true candidates (non-BUG candidates).
	/// </summary>
	public Candidates TrueCandidates => GetAllTrueCandidates(81 - 17);


	/// <summary>
	/// Get all true candidates when the number of empty cells
	/// is below than the argument.
	/// </summary>
	/// <param name="maximumEmptyCells">The maximum number of the empty cells.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>All true candidates.</returns>
	public unsafe Candidates GetAllTrueCandidates(int maximumEmptyCells, CancellationToken cancellationToken = default)
	{
		// Get the number of multi-value cells.
		// If the number of that is greater than the specified number,
		// here will return the default list directly.
		var multivalueCellsCount = 0;
		foreach (var value in Puzzle.EmptyCells)
		{
			switch (PopCount((uint)Puzzle.GetCandidates(value)))
			{
				case 1:
				case > 2 when ++multivalueCellsCount > maximumEmptyCells:
				{
					return Array.Empty<int>();
				}
			}
		}

		// Store all bi-value cells and construct the relations.
		var peerHouses = stackalloc int[3];
		var stack = new CellMap[multivalueCellsCount + 1, 9];
		foreach (var cell in Puzzle.BivalueCells)
		{
			foreach (var digit in Puzzle.GetCandidates(cell))
			{
				scoped ref var map = ref stack[0, digit];
				map.Add(cell);

				cell.CopyHouseInfo(peerHouses);
				for (var i = 0; i < 3; i++)
				{
					if ((map & HousesMap[peerHouses[i]]).Count > 2)
					{
						// The specified house contains at least three positions to fill with the digit,
						// which is invalid in any BUG + n patterns.
						return Array.Empty<int>();
					}
				}
			}
		}

		// Store all multi-value cells.
		// Suppose the pattern is the simplest BUG + 1 pattern (i.e. Only one multi-value cell).
		// The comments will help you to understand the processing.
		SkipInit(out short mask);
		var pairs = new short[multivalueCellsCount, 37]; // 37 == (1 + 8) * 8 / 2 + 1
		var multivalueCells = (Puzzle.EmptyCells - Puzzle.BivalueCells).ToArray();
		for (int i = 0, length = multivalueCells.Length; i < length; i++)
		{
			// e.g. { 2, 4, 6 } (42)
			mask = Puzzle.GetCandidates(multivalueCells[i]);

			// e.g. { 2, 4 }, { 4, 6 }, { 2, 6 } (10, 40, 34)
			var pairList = GetMaskSubsets(mask, 2);

			// e.g. pairs[i, ..] = { 3, { 2, 4 }, { 4, 6 }, { 2, 6 } } ({ 3, 10, 40, 34 })
			pairs[i, 0] = (short)pairList.Length;
			for (int z = 1, pairListLength = pairList.Length; z <= pairListLength; z++)
			{
				pairs[i, z] = pairList[z - 1];
			}
		}

		// Now check the pattern.
		// If the pattern is a valid BUG + n, the processing here will give you one plan of all possible
		// combinations; otherwise, none will be found.
		scoped var playground = (stackalloc int[3]);
		var currentIndex = 1;
		var chosen = new int[multivalueCellsCount + 1];
		var resultMap = new CellMap[9];
		var result = Candidates.Empty;
		do
		{
			int i;
			var currentCell = multivalueCells[currentIndex - 1];
			var @continue = false;
			for (i = chosen[currentIndex] + 1; i <= pairs[currentIndex - 1, 0]; i++)
			{
				@continue = true;
				mask = pairs[currentIndex - 1, i];
				foreach (var digit in pairs[currentIndex - 1, i])
				{
					var temp = stack[currentIndex - 1, digit];
					temp.Add(currentCell);

					fixed (int* p = playground)
					{
						currentCell.CopyHouseInfo(p);
					}
					foreach (var houseIndex in playground)
					{
						if ((temp & HousesMap[houseIndex]).Count > 2)
						{
							@continue = false;
							break;
						}
					}

					if (!@continue) { break; }
				}

				if (@continue) { break; }
			}

			if (@continue)
			{
				for (int z = 0, stackLength = stack.GetLength(1); z < stackLength; z++)
				{
					stack[currentIndex, z] = stack[currentIndex - 1, z];
				}

				chosen[currentIndex] = i;
				var pos1 = TrailingZeroCount(mask);

				stack[currentIndex, pos1].Add(currentCell);
				stack[currentIndex, mask.GetNextSet(pos1)].Add(currentCell);
				if (currentIndex == multivalueCellsCount)
				{
					// Iterate on each digit.
					for (var digit = 0; digit < 9; digit++)
					{
						// Take the cell that doesn't contain in the map above.
						// Here, the cell is the "true candidate cell".
						scoped ref var map = ref resultMap[digit];
						map = Puzzle.CandidatesMap[digit] - stack[currentIndex, digit];
						foreach (var cell in map)
						{
							result.AddAnyway(cell * 9 + digit);
						}
					}

					return result;
				}
				else
				{
					currentIndex++;
				}
			}
			else
			{
				chosen[currentIndex--] = 0;
			}
		} while (currentIndex > 0);

		return result;
	}
}

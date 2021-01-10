﻿using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Miscellaneous
{
	partial class BivalueOddagonStepSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">(<see langword="in"/> parameter) The loop.</param>
		/// <param name="links">The links.</param>
		/// <param name="extraCellsMap">(<see langword="in"/> parameter) The extra cells map.</param>
		partial void CheckType1(
			IList<BivalueOddagonStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop,
			IReadOnlyList<Link> links, in Cells extraCellsMap)
		{
			int extraCell = extraCellsMap[0];
			var conclusions = new List<Conclusion>();
			if (grid.Exists(extraCell, d1) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, extraCell, d1));
			}
			if (grid.Exists(extraCell, d2) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, extraCell, d2));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in loop - extraCell)
			{
				candidateOffsets.Add(new(0, cell * 9 + d1));
				candidateOffsets.Add(new(0, cell * 9 + d2));
			}

			accumulator.AddIfDoesNotContain(
				new BivalueOddagonType1StepInfo(
					conclusions,
					new View[] { new() { Candidates = candidateOffsets, Links = links } },
					loop,
					d1,
					d2,
					extraCell));
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">(<see langword="in"/> parameter) The loop.</param>
		/// <param name="links">The links.</param>
		/// <param name="extraCellsMap">(<see langword="in"/> parameter) The extra cells map.</param>
		/// <param name="comparer">The comparer mask (equals to <c>1 &lt;&lt; d1 | 1 &lt;&lt; d2</c>).</param>
		partial void CheckType2(
			IList<BivalueOddagonStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
			in Cells loop, IReadOnlyList<Link> links, in Cells extraCellsMap, short comparer)
		{
			short mask = 0;
			foreach (int cell in extraCellsMap)
			{
				mask |= grid.GetCandidates(cell);
			}
			mask &= (short)~comparer;

			if (!mask.IsPowerOfTwo())
			{
				return;
			}

			int extraDigit = TrailingZeroCount(mask);
			var elimMap = (extraCellsMap & CandMaps[extraDigit]).PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(ConclusionType.Elimination, cell, extraDigit));
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in loop)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(new(digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}

			accumulator.AddIfDoesNotContain(
				new BivalueOddagonType2StepInfo(
					conclusions,
					new View[] { new() { Candidates = candidateOffsets, Links = links } },
					loop,
					d1,
					d2,
					extraDigit));
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">(<see langword="in"/> parameter) The loop.</param>
		/// <param name="links">The links.</param>
		/// <param name="extraCellsMap">(<see langword="in"/> parameter) The extra cells map.</param>
		/// <param name="comparer">The comparer mask (equals to <c>1 &lt;&lt; d1 | 1 &lt;&lt; d2</c>).</param>
		partial void CheckType3(
			IList<BivalueOddagonStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
			in Cells loop, IReadOnlyList<Link> links, in Cells extraCellsMap, short comparer)
		{
			bool notSatisfiedType3 = false;
			foreach (int cell in extraCellsMap)
			{
				short mask = grid.GetCandidates(cell);
				if (!mask.Overlaps(comparer) || mask == comparer)
				{
					notSatisfiedType3 = true;
					break;
				}
			}

			if (!extraCellsMap.InOneRegion || notSatisfiedType3)
			{
				return;
			}

			short m = 0;
			foreach (int cell in extraCellsMap)
			{
				m |= grid.GetCandidates(cell);
			}
			if (!m.Covers(comparer))
			{
				return;
			}

			short otherDigitsMask = (short)(m & ~comparer);
			foreach (int region in extraCellsMap.CoveredRegions)
			{
				if ((ValueMaps[d1] | ValueMaps[d2]).Overlaps(RegionMaps[region]))
				{
					return;
				}

				int[] otherCells = ((RegionMaps[region] & EmptyMap) - loop).ToArray();
				for (int size = PopCount((uint)otherDigitsMask) - 1, count = otherCells.Length; size < count; size++)
				{
					foreach (int[] cells in otherCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidates(cell);
						}

						if (PopCount((uint)mask) != size + 1 || !mask.Covers(otherDigitsMask))
						{
							continue;
						}

						var elimMap = (RegionMaps[region] & EmptyMap) - cells - loop;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask)
						{
							foreach (int cell in elimMap & CandMaps[digit])
							{
								conclusions.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cell in loop)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(otherDigitsMask.ContainsBit(digit) ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						accumulator.AddIfDoesNotContain(
							new BivalueOddagonType3StepInfo(
								conclusions,
								new View[]
								{
									new()
									{
										Candidates = candidateOffsets,
										Regions = new DrawingInfo[] { new(0, region) },
										Links = links
									}
								},
								loop,
								d1,
								d2,
								mask,
								cells));
					}
				}
			}
		}
	}
}

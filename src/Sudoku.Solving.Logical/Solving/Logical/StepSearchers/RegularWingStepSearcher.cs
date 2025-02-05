﻿namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
internal sealed unsafe partial class RegularWingStepSearcher : IRegularWingStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public int MaxSize { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		// Iterate on the size.
		// Note that the greatest size is determined by two factors: the size that you specified
		// and the number of bi-value cells in the grid.
		scoped ref readonly var grid = ref context.Grid;
		for (int size = 3, count = Min(MaxSize, BivalueCells.Count); size <= count; size++)
		{
			// Iterate on each pivot cell.
			foreach (var pivot in EmptyCells)
			{
				var mask = grid.GetCandidates(pivot);
				var candsCount = PopCount((uint)mask);
				if (candsCount != size && candsCount != size - 1)
				{
					// Candidates are not enough.
					continue;
				}

				var map = PeersMap[pivot] & BivalueCells;
				if (map.Count < size - 1)
				{
					// Bi-value cells are not enough.
					continue;
				}

				// Iterate on each cell combination.
				foreach (var cells in map & size - 1)
				{
					// Check duplicate.
					// If two cells contain same candidates, the wing can't be formed.
					var flag = false;
					for (int i = 0, length = cells.Count, outerLength = length - 1; i < outerLength; i++)
					{
						for (var j = i + 1; j < length; j++)
						{
							if (grid.GetMask(cells[i]) == grid.GetMask(cells[j]))
							{
								flag = true;
								goto CheckWhetherTwoCellsContainSameCandidateKind;
							}
						}
					}

				CheckWhetherTwoCellsContainSameCandidateKind:
					if (flag)
					{
						continue;
					}

					short union = mask, inter = (short)(Grid.MaxCandidatesMask & mask);
					foreach (var cell in cells)
					{
						var m = grid.GetCandidates(cell);
						union |= m;
						inter &= m;
					}

					if (PopCount((uint)union) != size || inter != 0 && !IsPow2(inter))
					{
						continue;
					}

					// Get the Z digit (The digit to be removed).
					var isIncomplete = inter == 0;
					var interWithoutPivot = (short)(union & ~grid.GetCandidates(pivot));
					var maskToCheck = isIncomplete ? interWithoutPivot : inter;
					if (!IsPow2(maskToCheck))
					{
						continue;
					}

					// The pattern should be "az, bz, cz, dz, ... , abcd(z)".
					var zDigit = TrailingZeroCount(maskToCheck);
					var petals = cells;
					if ((petals + pivot & CandidatesMap[zDigit]).Count != (isIncomplete ? size - 1 : size))
					{
						continue;
					}

					// Check elimination map.
					var elimMap = petals.PeerIntersection;
					if (!isIncomplete)
					{
						elimMap &= PeersMap[pivot];
					}
					elimMap &= CandidatesMap[zDigit];
					if (!elimMap)
					{
						continue;
					}

					// Gather highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>(6);
					foreach (var cell in cells)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(
								new(
									digit == zDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
									cell * 9 + digit
								)
							);
						}
					}
					foreach (var digit in grid.GetCandidates(pivot))
					{
						candidateOffsets.Add(
							new(
								digit == zDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								pivot * 9 + digit
							)
						);
					}

					var step = new RegularWingStep(
						from cell in elimMap select new Conclusion(Elimination, cell, zDigit),
						ImmutableArray.Create(View.Empty | candidateOffsets),
						pivot,
						PopCount((uint)mask),
						union,
						petals
					);

					if (context.OnlyFindOne)
					{
						return step;
					}

					context.Accumulator.Add(step);
				}
			}
		}

		return null;
	}
}

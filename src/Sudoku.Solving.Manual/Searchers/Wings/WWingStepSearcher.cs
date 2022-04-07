﻿namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>W-Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>W-Wing (George Woods' Wing)</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class WWingStepSearcher : IIregularWingStepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// In fact, <c>Hybrid-Wing</c>s, <c>Local-Wing</c>s, <c>Split-Wing</c>s and <c>M-Wing</c>s can
	/// be found in another searcher. In addition, these wings are not elementary and necessary techniques
	/// so we doesn't need to list them.
	/// </remarks>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		// The grid with possible W-Wing structure should contain
		// at least two empty cells (start and end cell).
		if (BivalueMap.Count < 2)
		{
			return null;
		}

		// Iterate on each cells.
		for (int c1 = 0; c1 < 72; c1++)
		{
			if (!BivalueMap.Contains(c1))
			{
				// The cell isn't a bi-value cell.
				continue;
			}

			// Iterate on each cells which are not peers in 'c1'.
			var digits = grid.GetCandidates(c1).GetAllSets();
			foreach (int c2 in BivalueMap - new Cells(c1))
			{
				if (c2 < c1)
				{
					// To avoid duplicate structures found.
					continue;
				}

				if (grid.GetCandidates(c1) != grid.GetCandidates(c2))
				{
					// Two cells may contain different kinds of digits.
					continue;
				}

				var intersection = PeerMaps[c1] & PeerMaps[c2];
				if ((EmptyMap & intersection) is [])
				{
					// The structure doesn't contain any possible eliminations.
					continue;
				}

				// Iterate on each house.
				for (int house = 0; house < 27; house++)
				{
					if (house == c1.ToHouseIndex(HouseType.Block)
						|| house == c1.ToHouseIndex(HouseType.Row)
						|| house == c1.ToHouseIndex(HouseType.Column)
						|| house == c2.ToHouseIndex(HouseType.Block)
						|| house == c2.ToHouseIndex(HouseType.Row)
						|| house == c2.ToHouseIndex(HouseType.Column))
					{
						// The house to search for conjugate pairs shouldn't
						// be the same as those two cells' houses.
						continue;
					}

					// Iterate on each digit to search for the conjugate pair.
					foreach (int digit in digits)
					{
						// Now search for conjugate pair.
						if ((CandMaps[digit] & HouseMaps[house]) is not [var a, var b] conjugate)
						{
							// The current house doesn't contain the conjugate pair of this digit.
							continue;
						}

						// Check whether the cells are the same house as the head and the tail cell.
						if (!(Cells.Empty + c1 + a).InOneHouse || !(Cells.Empty + c2 + b).InOneHouse
							&& !(Cells.Empty + c1 + b).InOneHouse || !(Cells.Empty + c2 + a).InOneHouse)
						{
							continue;
						}

						// Check for eliminations.
						int anotherDigit = TrailingZeroCount(grid.GetCandidates(c1) & ~(1 << digit));
						if ((CandMaps[anotherDigit] & !(Cells.Empty + c1 + c2)) is not { Count: not 0 } elimMap)
						{
							continue;
						}

						// Now W-Wing found. Store it into the accumulator.
						var step = new WWingStep(
							ImmutableArray.Create(
								Conclusion.ToConclusions(elimMap, anotherDigit, ConclusionType.Elimination)
							),
							ImmutableArray.Create(
								View.Empty
									+ new CandidateViewNode[]
									{
										new(0, c1 * 9 + anotherDigit),
										new(0, c2 * 9 + anotherDigit),
										new(1, c1 * 9 + digit),
										new(1, c2 * 9 + digit),
										new(1, a * 9 + digit),
										new(1, b * 9 + digit)
									}
									+ new HouseViewNode(0, house)),
							a,
							b,
							new(conjugate, digit)
						);

						if (onlyFindOne)
						{
							return step;
						}

						accumulator.Add(step);
					}
				}
			}
		}

		return null;
	}
}

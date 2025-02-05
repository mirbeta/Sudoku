﻿namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
internal sealed unsafe partial class EmptyRectangleIntersectionPairStepSearcher : IEmptyRectangleIntersectionPairStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		for (int i = 0, length = BivalueCells.Count, iterationLength = length - 1; i < iterationLength; i++)
		{
			var c1 = BivalueCells[i];

			var mask = grid.GetCandidates(c1);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			for (var j = i + 1; j < length; j++)
			{
				var c2 = BivalueCells[j];

				// Check the candidates that cell holds is totally same with 'c1'.
				if (grid.GetCandidates(c2) != mask)
				{
					continue;
				}

				// Check the two cells are not in same house index.
				if ((CellsMap[c1] + c2).InOneHouse)
				{
					continue;
				}

				var block1 = c1.ToHouseIndex(HouseType.Block);
				var block2 = c2.ToHouseIndex(HouseType.Block);
				if (block1 % 3 == block2 % 3 || block1 / 3 == block2 / 3)
				{
					continue;
				}

				// Check the block that two cells both see.
				var interMap = (CellsMap[c1] + c2).PeerIntersection;
				var unionMap = (PeersMap[c1] | PeersMap[c2]) + c1 + c2;
				foreach (var interCell in interMap)
				{
					var block = interCell.ToHouseIndex(HouseType.Block);
					var houseMap = HousesMap[block];
					var checkingMap = houseMap - unionMap & houseMap;
					if (checkingMap & CandidatesMap[d1] || checkingMap & CandidatesMap[d2])
					{
						continue;
					}

					// Check whether two digits are both in the same empty rectangle.
					var b1 = c1.ToHouseIndex(HouseType.Block);
					var b2 = c2.ToHouseIndex(HouseType.Block);
					var erMap = unionMap & HousesMap[b1] - interMap | unionMap & HousesMap[b2] - interMap;
					var erCellsMap = houseMap & erMap;
					var m = grid.GetDigitsUnion(erCellsMap);
					if ((m & mask) != mask)
					{
						continue;
					}

					// Check eliminations.
					var conclusions = new List<Conclusion>();
					var z = (interMap & houseMap)[0];
					var c1Map = HousesMap[(CellsMap[z] + c1).CoveredLine];
					var c2Map = HousesMap[(CellsMap[z] + c2).CoveredLine];
					foreach (var elimCell in (c1Map | c2Map) - c1 - c2 - erMap)
					{
						if (CandidatesMap[d1].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, d1));
						}
						if (CandidatesMap[d2].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, d2));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (var digit in grid.GetCandidates(c1))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, c1 * 9 + digit));
					}
					foreach (var digit in grid.GetCandidates(c2))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, c2 * 9 + digit));
					}
					foreach (var cell in erCellsMap)
					{
						foreach (var digit in grid.GetCandidates(cell))
						{
							if (digit != d1 && digit != d2)
							{
								continue;
							}

							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new EmptyRectangleIntersectionPairStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, block)
						),
						c1,
						c2,
						block,
						d1,
						d2
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

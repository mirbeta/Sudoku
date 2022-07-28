﻿namespace Sudoku.Solving.Manual.Searchers;

[StepSearcher]
[StepSearcherOptions(IsOptionsFixed = true, IsDirect = true)]
internal sealed unsafe partial class BruteForceStepSearcher : IBruteForceStepSearcher
{
	/// <inheritdoc/>
	public Grid Solution { get; set; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		if (Solution.IsUndefined)
		{
			goto ReturnNull;
		}

		foreach (int offset in BruteForceTryAndErrorOrder)
		{
			if (grid.GetStatus(offset) == CellStatus.Empty)
			{
				int cand = offset * 9 + Solution[offset];
				var step = new BruteForceStep(
					ImmutableArray.Create(new Conclusion(ConclusionType.Assignment, cand)),
					ImmutableArray.Create(View.Empty | new CandidateViewNode(DisplayColorKind.Normal, cand))
				);
				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}
		}

	ReturnNull:
		return null;
	}
}

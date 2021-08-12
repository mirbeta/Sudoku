﻿namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>brute force</b> (<b>BF</b>) technique searcher.
	/// The searcher is not executed until all searchers whose priority
	/// is higher than this one can't find out any technique steps.
	/// </summary>
	/// <remarks>
	/// This searcher is a trick, because it will check the assignments on
	/// the terminal grid (I mean, the answer grid).
	/// </remarks>
	[DirectSearcher, IsOptionsFixed]
	public sealed class BfStepSearcher : LastResortStepSearcher
	{
		/// <summary>
		/// The order of cell offsets to get values.
		/// </summary>
		private static readonly int[] TryAndErrorOrder =
		{
			40, 41, 32, 31, 30, 39, 48, 49, 50,
			51, 42, 33, 24, 23, 22, 21, 20, 29,
			38, 47, 56, 57, 58, 59, 60, 61, 52,
			43, 34, 25, 16, 15, 14, 13, 12, 11,
			10, 19, 28, 37, 46, 55, 64, 65, 66,
			67, 68, 69, 70, 71, 62, 53, 44, 35,
			26, 17,  8,  7,  6,  5,  4,  3,  2,
			 1,  0,  9, 18, 27, 36, 45, 54, 63,
			72, 73, 74, 75, 76, 77, 78, 79, 80
		};


		/// <summary>
		/// Initializes a <see cref="BfStepSearcher"/> with no arguments.
		/// </summary>
		public BfStepSearcher()
		{
		}

		/// <summary>
		/// A trick. Initializes an instance with the solution grid.
		/// This searcher will try to extract a value from the
		/// solution.
		/// </summary>
		/// <param name="solution">The solution.</param>
		public BfStepSearcher(in SudokuGrid solution) => Solution = solution;


		/// <summary>
		/// Indicates the solution grid.
		/// </summary>
		public SudokuGrid Solution { get; set; }

		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(
			38, DisplayingLevel.E,
			EnabledAreas: EnabledAreas.Default
		);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(38, nameof(Technique.BruteForce))
		{
			IsReadOnly = true,
			DisplayLevel = 10,
			OnlyEnableInAnalysis = true
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			if (Solution == SudokuGrid.Undefined)
			{
				return;
			}

			foreach (int offset in TryAndErrorOrder)
			{
				if (grid.GetStatus(offset) == CellStatus.Empty)
				{
					int cand = offset * 9 + Solution[offset];
					accumulator.Add(
						new BfStepInfo(
							new Conclusion[] { new(ConclusionType.Assignment, cand) },
							new View[] { new() { Candidates = new DrawingInfo[] { new(0, cand) } } }
						)
					);
				}
			}
		}
	}
}
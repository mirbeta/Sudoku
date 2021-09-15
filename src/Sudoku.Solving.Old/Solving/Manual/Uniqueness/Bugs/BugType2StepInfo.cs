﻿namespace Sudoku.Solving.Manual.Uniqueness.Bugs;

/// <summary>
/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 2 technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Digit">The digit.</param>
/// <param name="Cells">All cell offsets.</param>
public sealed record class BugType2StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, IReadOnlyList<int> Cells
) : BugStepInfo(Conclusions, Views)
{
	/// <summary>
	/// The table of extra difficulty values.
	/// </summary>
	private static readonly decimal[] ExtraDifficulty =
	{
		.1M, .2M, .2M, .3M, .3M, .3M, .4M, .4M, .4M, .4M,
		.5M, .5M, .5M, .5M, .5M, .6M, .6M, .6M, .6M, .6M
	};


	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + ExtraDifficulty[Cells.Count - 1];

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BugType2;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells(Cells).ToString();
	}
}

﻿namespace Sudoku.Solving.Manual.Alses;

/// <summary>
/// Provides a usage of <b>almost locked set</b> (ALS) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
public abstract record class AlsStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
	: StepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.Als;
}

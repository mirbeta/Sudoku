﻿namespace Sudoku.Solving.Manual.RankTheory;

/// <summary>
/// Provides a usage of <b>rank theory</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
public abstract record class RankTheoryStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
	: StepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory;
}
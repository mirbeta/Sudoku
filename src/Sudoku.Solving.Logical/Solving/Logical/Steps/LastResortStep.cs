﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Last Resort</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record LastResortStep(ConclusionList Conclusions, ViewList Views) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LastResort;

	/// <inheritdoc/>
	public override Stableness Stableness => Stableness.LessUnstable;
}

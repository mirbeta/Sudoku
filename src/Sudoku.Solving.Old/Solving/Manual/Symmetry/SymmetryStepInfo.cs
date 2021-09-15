﻿namespace Sudoku.Solving.Manual.Symmetry;

/// <summary>
/// Provides a usage of <b>symmetry</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
public abstract record class SymmetryStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
	: StepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => false;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Symmetry;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Symmetry;
}

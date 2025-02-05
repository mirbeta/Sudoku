﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-sector Locked Sets</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
internal sealed record MultisectorLockedSetsStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells
) : NonnegativeRankStep(Conclusions, Views), IStepWithRank, IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 9.4M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.Size, A002024(Cells.Count) * .1M) };

	/// <inheritdoc/>
	public int Rank => 0;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Msls;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.MultisectorLockedSets;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.MultisectorLockedSets;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[ResourceTextFormatter]
	internal string CellsCountStr() => Cells.Count.ToString();

	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();
}

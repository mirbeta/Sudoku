﻿using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Candidates</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">Indicates the mask that contains the digits used.</param>
/// <param name="BaseCells">Indicates the base cells.</param>
/// <param name="TargetCells">Indicates the target cells.</param>
/// <param name="HasValueCell">Indicates whether the step contains value cells.</param>
public sealed record AlmostLockedCandidatesStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	short DigitsMask,
	in Cells BaseCells,
	in Cells TargetCells,
	bool HasValueCell
) : IntersectionStep(Conclusions, Views)
{
	/// <summary>
	/// Indicates the size.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public override decimal Difficulty =>
		Size switch { 2 => 4.5M, 3 => 5.2M, 4 => 5.7M } // Base difficulty.
		+ (HasValueCell ? Size switch { 2 => .1M, 3 => .1M, 4 => .2M } : 0); // Extra difficulty.

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Size switch
	{
		2 => Technique.AlmostLockedPair,
		3 => Technique.AlmostLockedTriple,
		4 => Technique.AlmostLockedQuadruple
	};

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlmostLockedCandidates;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	private string BaseCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BaseCells.ToString();
	}

	[FormatItem]
	private string TargetCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TargetCells.ToString();
	}
}

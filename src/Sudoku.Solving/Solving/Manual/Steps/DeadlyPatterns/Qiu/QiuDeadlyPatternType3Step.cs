﻿namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Qiu;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ExtraDigitsMask">Indicates the extra digits used to form the subset.</param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="IsNaked">Indicates whether the subset is a naked subset.</param>
public sealed record QiuDeadlyPatternType3Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in QiuDeadlyPattern Pattern,
	short ExtraDigitsMask,
	Cells ExtraCells,
	bool IsNaked
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + PopCount((uint)ExtraDigitsMask) * .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.QiuDeadlyPatternType3;

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraDigitsMask).ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ExtraCells.ToString();
	}

	[FormatItem]
	private string SubsetName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current[$"SubsetNamesSize{ExtraCells.Count + 1}"];
	}
}
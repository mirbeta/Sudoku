﻿namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Extended;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit used.</param>
public sealed record ExtendedRectangleType2Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Cells,
	short DigitsMask,
	int ExtraDigit
) : ExtendedRectangleStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.XrType2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}

﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraDigit">The extra digit.</param>
internal sealed record UniquePolygonType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Map,
	short DigitsMask,
	int ExtraDigit
) : UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.4M;

	/// <inheritdoc/>
	public override int Type => 2;

	[ResourceTextFormatter]
	internal string ExtraDigitStr() => (ExtraDigit + 1).ToString();
}

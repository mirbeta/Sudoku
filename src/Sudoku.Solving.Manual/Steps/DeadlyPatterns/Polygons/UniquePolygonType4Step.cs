﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ConjugateHouse">Indicates the cells that forms the conjugate house.</param>
/// <param name="ExtraMask">Indicates the extra digits mask.</param>
public sealed record class UniquePolygonType4Step(
	ImmutableArray<Conclusion> Conclusions, ImmutableArray<View> Views,
	in Cells Map, short DigitsMask, in Cells ConjugateHouse, short ExtraMask) :
	UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.5M;

	/// <inheritdoc/>
	public override int Type => 4;

	[FormatItem]
	internal string ExtraCombStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraMask).ToString();
	}

	[FormatItem]
	internal string ConjHouseStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugateHouse.ToString();
	}
}

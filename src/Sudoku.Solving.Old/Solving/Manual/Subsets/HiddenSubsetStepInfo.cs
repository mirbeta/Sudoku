﻿namespace Sudoku.Solving.Manual.Subsets;

/// <summary>
/// Provides a usage of <b>hidden subset</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Region">The region that structure lies in.</param>
/// <param name="Cells">All cells used.</param>
/// <param name="Digits">All digits used.</param>
public sealed record class HiddenSubsetStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	int Region, in Cells Cells, IReadOnlyList<int> Digits
) : SubsetStepInfo(Conclusions, Views, Region, Cells, Digits)
{
	/// <inheritdoc/>
	public override decimal Difficulty => Size switch { 2 => 3.4M, 3 => 4.0M, 4 => 5.4M };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Size switch
	{
		2 => Technique.HiddenPair,
		3 => Technique.HiddenTriple,
		4 => Technique.HiddenQuadruple
	};

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(Digits).ToString();
	}

	[FormatItem]
	private string RegionStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new RegionCollection(Region).ToString();
	}
}
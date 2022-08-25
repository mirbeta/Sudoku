﻿namespace Sudoku.Solving.Data.Representation;

/// <summary>
/// Represents for a data set that describes the complete information about a bi-value oddagon technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole bi-value oddagon loop.</param>
/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="short"/>.</param>
public readonly record struct BivalueOddagonDataInfo(scoped in Cells Loop, short DigitsMask) :
	IEquatable<BivalueOddagonDataInfo>,
	IEqualityOperators<BivalueOddagonDataInfo, BivalueOddagonDataInfo>,
	ITechniqueDataInfo<BivalueOddagonDataInfo>
{
	/// <inheritdoc/>
	Cells ITechniqueDataInfo<BivalueOddagonDataInfo>.Map => Loop;
}
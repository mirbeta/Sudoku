﻿using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>naked subset</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region that structure lies in.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Digits">All digits used.</param>
	/// <param name="IsLocked">Indicates whether the subset is locked.</param>
	public sealed record NakedSubsetStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Region, in Cells Cells, IReadOnlyList<int> Digits, bool? IsLocked
	) : SubsetStepInfo(Conclusions, Views, Region, Cells, Digits)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + ExtraDifficulty;

		/// <inheritdoc/>
		public override Technique TechniqueCode => (IsLocked, DigitsCount: Digits.Count) switch
		{
			(IsLocked: true, DigitsCount: 2) => Technique.LockedPair,
			(IsLocked: false, DigitsCount: 2) => Technique.NakedPairPlus,
			(IsLocked: null, DigitsCount: 2) => Technique.NakedPair,
			(IsLocked: true, DigitsCount: 3) => Technique.LockedTriple,
			(IsLocked: false, DigitsCount: 3) => Technique.NakedTriplePlus,
			(IsLocked: null, DigitsCount: 3) => Technique.NakedTriple,
			(IsLocked: false, DigitsCount: 4) => Technique.NakedQuadruplePlus,
			(IsLocked: null, DigitsCount: 4) => Technique.NakedQuadruple
		};

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => Size switch { 2 => 3.0M, 3 => 3.6M, 4 => 5.0M };

		/// <summary>
		/// Indicates the extra difficulty.
		/// </summary>
		private decimal ExtraDifficulty =>
			IsLocked switch { null => 0, true => Size switch { 2 => -1.0M, 3 => -1.1M }, false => .1M };


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitStr = new DigitCollection(Digits).ToString();
			string regionStr = new RegionCollection(Region).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitStr} in {regionStr} => {elimStr}";
		}
	}
}

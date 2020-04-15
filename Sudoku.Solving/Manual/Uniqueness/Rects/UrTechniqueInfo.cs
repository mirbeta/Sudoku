﻿using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle</b> (UR) or
	/// <b>avoidable rectangle</b> (AR) technique.
	/// </summary>
	public abstract class UrTechniqueInfo : RectangleTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="typeName">The type name.</param>
		/// <param name="digit1">The digit 1.</param>
		/// <param name="digit2">The digit 2.</param>
		/// <param name="cells">All cells.</param>
		/// <param name="isAr">Indicates whether the structure is an AR.</param>
		public UrTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, string typeName,
			int digit1, int digit2, int[] cells, bool isAr) : base(conclusions, views) =>
			(TypeName, Digit1, Digit2, Cells, IsAr) = (typeName, digit1, digit2, cells, isAr);


		/// <summary>
		/// Indicates the type name of this pattern.
		/// </summary>
		public string TypeName { get; }

		/// <summary>
		/// Indicates the digit 1.
		/// </summary>
		public int Digit1 { get; }

		/// <summary>
		/// Indicates the digit 2.
		/// </summary>
		public int Digit2 { get; }

		/// <summary>
		/// Indicates the cells.
		/// </summary>
		public int[] Cells { get; }

		/// <summary>
		/// Indicates the current structure is UR or AR.
		/// </summary>
		public bool IsAr { get; }

		/// <inheritdoc/>
		public sealed override string Name => $"{(IsAr ? "Avoidable" : "Unique")} Rectangle Type {TypeName}";

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => true;


		/// <inheritdoc/>
		public override string ToString()
		{
			int d1 = Digit1 + 1;
			int d2 = Digit2 + 1;
			string cellsStr = CellCollection.ToString(Cells);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {d1}, {d2} in {cellsStr} with {GetAdditional()} => {elimStr}";
		}

		/// <summary>
		/// Get additional string.
		/// </summary>
		/// <returns>The additional string.</returns>
		protected abstract string GetAdditional();
	}
}

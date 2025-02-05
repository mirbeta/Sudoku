﻿namespace Sudoku.Solving.Algorithms.DancingLinx;

/// <summary>
/// Represents a column node.
/// </summary>
internal sealed class ColumnNode : DataNode
{
	/// <summary>
	/// Initializes a <see cref="ColumnNode"/> instance via the specified ID value.
	/// </summary>
	/// <param name="id">The ID value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ColumnNode(int id) : base(id) => (Column, Size) = (this, 0);


	/// <summary>
	/// Indicates the size of the node.
	/// </summary>
	public int Size { get; set; }
}

﻿namespace Sudoku.Solving.Logical.Patterns;

/// <summary>
/// Defines a pattern that is a unique polygon technique structure in theory. The sketch is like:
/// <code><![CDATA[
/// .-------.-------.-------.
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// :-------+-------+-------:
/// | . . . | . . . | . . . |
/// | . . . | . . . | . . . |
/// | . . . | . . . | P P . |
/// :-------+-------+-------:
/// | . . . | . . Q | S S . |
/// | . . . | . . Q | S(S). |
/// | . . . | . . . | . . . |
/// '-------'-------'-------'
/// ]]></code>
/// Where:
/// <list type="table">
/// <item><term>P</term><description>The first group of cells.</description></item>
/// <item><term>Q</term><description>The second group of cells.</description></item>
/// <item>
/// <term>S</term>
/// <description>
/// The square cells of size 3 or 4. The cell with the bracket (r8c8 in the picture)
/// is optional.
/// </description>
/// </item>
/// </list>
/// </summary>
/// <param name="Mask">The mask that forms a structure.</param>
/// <remarks>
/// <para>
/// This <see cref="Mask"/> is of type <see cref="long"/>,
/// where the distribution of each bit is as follows:
/// <code><![CDATA[
/// 0      7     14     21     28     35     42     49     56
/// ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓      ↓
/// |-------|-------|-------|-------|-------|-------|-------|-------|
/// ↑       ↑       ↑       ↑       ↑       ↑       ↑       ↑       ↑
/// 0       8      16      24      32      40      48      56      64
/// ]]></code>
/// where the bit <c>[0..56]</c> is for 8 cells, the last 7 bits determine the pattern is a
/// heptagon or a octagon. If the value is 127 (not available), the pattern will be a heptagon.
/// </para>
/// <para>
/// Due to the rendering engine, you have to check this file rather than the tip window.
/// </para>
/// </remarks>
public readonly record struct UniquePolygonPattern(long Mask) : ITechniquePattern<UniquePolygonPattern>
{
	/// <summary>
	/// Indicates whether the specified pattern is a heptagon.
	/// </summary>
	public bool IsHeptagon
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Mask >> 28 & 127) == 127;
	}

	/// <summary>
	/// Indicates the map of pair 1 cells.
	/// </summary>
	public CellMap Pair1Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CellsMap[Pair1.A] + Pair1.B;
	}

	/// <summary>
	/// Indicates the map of pair 2 cells.
	/// </summary>
	public CellMap Pair2Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => CellsMap[Pair2.A] + Pair2.B;
	}

	/// <summary>
	/// The map of other three (or four) cells.
	/// </summary>
	public CellMap CenterCellsMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var (a, b, c, d) = CenterCells;
			return IsHeptagon ? CellsMap[a] + b + c : CellsMap[a] + b + c + d;
		}
	}

	/// <inheritdoc/>
	public CellMap Map
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Pair1Map | Pair2Map | CenterCellsMap;
	}

	/// <summary>
	/// Indicates the pair 1.
	/// </summary>
	private (int A, int B) Pair1
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((int)(Mask >> 7 & 127), (int)(Mask & 127));
	}

	/// <summary>
	/// Indicates the pair 2.
	/// </summary>
	private (int A, int B) Pair2
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((int)(Mask >> 21 & 127), (int)(Mask >> 14 & 127));
	}

	/// <summary>
	/// Indicates the other three (or four) cells.
	/// </summary>
	/// <remarks>
	/// <b>If and only if</b> the fourth value in the returned quadruple is available.
	/// </remarks>
	private (int A, int B, int C, int D) CenterCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((int)(Mask >> 49 & 127), (int)(Mask >> 42 & 127), (int)(Mask >> 35 & 127), (int)(Mask >> 28 & 127));
	}


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out (int A, int B) pair1, out (int A, int B) pair2, out (int A, int B, int C, int D) centerCells)
		=> (pair1, pair2, centerCells) = (Pair1, Pair2, CenterCells);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => (int)Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $$"""{{nameof(UniquePolygonPattern)}} { {{nameof(Map)}} = {{Map}} }""";
}

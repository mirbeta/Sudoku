﻿namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a candidate line.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
public sealed partial class CandidateLine : DrawingElement
{
	/// <summary>
	/// Indicates the order.
	/// </summary>
	private readonly byte _order;

	/// <summary>
	/// The inner line.
	/// </summary>
	private readonly Line _line = new();

	/// <summary>
	/// Indicates the pane size, which is the backing field of the property <see cref="PaneSize"/>.
	/// </summary>
	/// <seealso cref="PaneSize"/>
	private double _paneSize;

	/// <summary>
	/// Indicates the outside offset, which is the backing field of the property <see cref="OutsideOffset"/>.
	/// </summary>
	/// <seealso cref="OutsideOffset"/>
	private double _outsideOffset;


	/// <summary>
	/// The order of the block line. The value must be between 0 and 27.
	/// <list type="table">
	/// <listheader>
	/// <term>Range</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><![CDATA[>= 0 and < 28]]></term>
	/// <description>The block line is horizontal.</description>
	/// </item>
	/// <item>
	/// <term><![CDATA[>= 28 and < 56]]></term>
	/// <description>The block line is vertical.</description>
	/// </item>
	/// </list>
	/// </summary>
	public required byte Order
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _order;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init
		{
			_order = value;

			if (_paneSize == 0 || _outsideOffset == 0)
			{
				return;
			}

			var ((x1, y1), (x2, y2)) = PointConversions.GetCandidateLine(_paneSize, _outsideOffset, value);
			_line.X1 = x1;
			_line.X2 = x2;
			_line.Y1 = y1;
			_line.Y2 = y2;
		}
	}

	/// <summary>
	/// The stroke thickness of the block line.
	/// </summary>
	public required double StrokeThickness
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _line.StrokeThickness;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _line.StrokeThickness = value;
	}

	/// <summary>
	/// The pane size.
	/// </summary>
	public required double PaneSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _paneSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var ((x1, y1), (x2, y2)) = PointConversions.GetCandidateLine(_paneSize = value, _outsideOffset, Order);
			_line.X1 = x1;
			_line.X2 = x2;
			_line.Y1 = y1;
			_line.Y2 = y2;
		}
	}

	/// <summary>
	/// The outside offset.
	/// </summary>
	public required double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var ((x1, y1), (x2, y2)) = PointConversions.GetCandidateLine(_paneSize, _outsideOffset = value, Order);
			_line.X1 = x1;
			_line.X2 = x2;
			_line.Y1 = y1;
			_line.Y2 = y2;
		}
	}

	/// <summary>
	/// The stroke color of the block line.
	/// </summary>
	public required Color StrokeColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((SolidColorBrush)_line.Stroke).Color;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _line.Stroke = new SolidColorBrush(value);
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CandidateLine);

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private string DebuggerDisplayView
	{
		get
		{
			var (x1, x2, y1, y2) = _line;
			return $"{nameof(CandidateLine)} {{ Start = {(x1, y1)}, End = {(x2, y2)} }}";
		}
	}
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is CandidateLine comparer
		&& _line.X1.NearlyEquals(comparer._line.X1, 1E-2)
		&& _line.X2.NearlyEquals(comparer._line.X2, 1E-2)
		&& _line.Y1.NearlyEquals(comparer._line.Y1, 1E-2)
		&& _line.Y2.NearlyEquals(comparer._line.Y2, 1E-2);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
		=> HashCode.Combine(TypeIdentifier, HashCode.Combine(_line.X1, _line.X2, _line.Y1, _line.Y2));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Line GetControl() => _line;
}

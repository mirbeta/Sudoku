﻿namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a cell view node shape.
/// </summary>
public sealed class CellViewNodeShape : DrawingElement
{
	/// <summary>
	/// The inner control.
	/// </summary>
	private readonly Rectangle _rectangle = new Rectangle()
		.WithMargin(0)
		.WithCanvasZIndex(-2);

	/// <summary>
	/// Indicates whether the node shape is displayed.
	/// </summary>
	private bool _isVisible;

	/// <summary>
	/// Indicates the identifier.
	/// </summary>
	private Identifier _identifier;


	/// <summary>
	/// Indicates the visible.
	/// </summary>
	public required bool IsVisible
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isVisible;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_isVisible = value;
			_rectangle.Visibility = _isVisible ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	/// <summary>
	/// The color identifier.
	/// </summary>
	public Identifier ColorIdentifier
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _identifier;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_identifier = value;

			// Temporary solution:
			// If we use a color with full alpha value (255), due to the priority of the rendering,
			// the cell rectangles will cover a part of grid or cell border lines.
			// Therefore, we should set the alpha value to 64.
			var color = _identifier.AsColor(Preference) with { A = 64 };
			_rectangle.Fill = new SolidColorBrush(color);
		}
	}

	/// <summary>
	/// The user preference instance.
	/// </summary>
	public required IDrawingPreference Preference { get; init; }


	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CellViewNodeShape);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is CellViewNodeShape comparer && _identifier == comparer._identifier;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _identifier);

	/// <inheritdoc/>
	public override Rectangle GetControl() => _rectangle;
}

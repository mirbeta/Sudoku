﻿namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a chute view node shape.
/// </summary>
public sealed class ChuteViewNodeShape : DrawingElement
{
	/// <summary>
	/// Indicates the visible table.
	/// </summary>
	private readonly bool[] _isVisibleTable = new bool[6];

	/// <summary>
	/// Indicates the color identifiers.
	/// </summary>
	private readonly Identifier[] _identifiers = new Identifier[6];

	/// <summary>
	/// Indicates the grid layout.
	/// </summary>
	private readonly GridLayout _gridLayout = new GridLayout()
		.WithRowDefinitionsCount(9)
		.WithColumnDefinitionsCount(9);

	/// <summary>
	/// Indicates the inner rectangles.
	/// </summary>
	private readonly Rectangle[] _rectangles = new Rectangle[6];

	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;


	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	public IDrawingPreference Preference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference;

		init
		{
			_preference = value;

			var i = 0;
			while (i < 3)
			{
				_gridLayout.AddChildren(
					_rectangles[i] = new Rectangle()
						.WithGridLayout(row: i * 3, rowSpan: 3, columnSpan: 9)
						.WithOpacity(0)
						.WithOpacityTransition(TimeSpan.FromMilliseconds(500))
				);

				i++;
			}
			while (i < 6)
			{
				_gridLayout.AddChildren(
					_rectangles[i] = new Rectangle()
						.WithGridLayout(column: (i - 3) * 3, rowSpan: 9, columnSpan: 3)
						.WithOpacity(0)
						.WithOpacityTransition(TimeSpan.FromMilliseconds(500))
				);

				i++;
			}
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(ChuteViewNodeShape);


	/// <summary>
	/// Sets the visibility at the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <param name="isVisible">The visibility.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetIsVisible(int house, bool isVisible)
	{
		_isVisibleTable[house] = isVisible;
		_rectangles[house].Opacity = isVisible ? 1 : 0;
	}

	/// <summary>
	/// Sets the color identifier at the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <param name="colorIdentifier">The color identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetIdentifier(int house, Identifier colorIdentifier)
	{
		var targetColor = colorIdentifier.AsColor(_preference);

		_identifiers[house] = colorIdentifier;
		_rectangles[house].Fill = new SolidColorBrush(targetColor with { A = 64 });
		_rectangles[house].Stroke = new SolidColorBrush(targetColor);
		_rectangles[house].StrokeThickness = _preference.HouseViewNodeStrokeThickness;
	}

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is ChuteViewNodeShape comparer && Enumerable.SequenceEqual(comparer._identifiers, _identifiers)
		&& Enumerable.SequenceEqual(comparer._identifiers, _identifiers);

	/// <summary>
	/// Gets the visibility at the specified house.
	/// </summary>
	/// <param name="house">The house.</param>
	/// <returns>The visibility.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool GetVisible(int house) => _isVisibleTable[house];

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		result.Add(TypeIdentifier);

		for (var i = 0; i < 6; i++)
		{
			result.Add(_rectangles[i]);
		}

		return result.ToHashCode();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;
}

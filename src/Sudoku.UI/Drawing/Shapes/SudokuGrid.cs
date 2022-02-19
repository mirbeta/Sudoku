﻿using Windows.UI;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a sudoku grid.
/// </summary>
public sealed class SudokuGrid : DrawingElement
{
	/// <summary>
	/// Indicates the status for displaying candidates.
	/// </summary>
	private bool _showCandidates;

	/// <summary>
	/// Indicates the font size.
	/// </summary>
	private double _valueFontSize, _candidateFontSize;

	/// <summary>
	/// Indicates the pane size.
	/// </summary>
	private double _paneSize;

	/// <summary>
	/// Indicates the outside offset.
	/// </summary>
	private double _outsideOffset;

	/// <summary>
	/// Indicates the font name.
	/// </summary>
	private string _valueFontName, _candidateFontName;

	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	private Grid _grid;

	/// <summary>
	/// Indicates the inner grid layout control.
	/// </summary>
	private readonly GridLayout _gridLayout;

	/// <summary>
	/// Indicates the cell digits.
	/// </summary>
	private readonly CellDigit[] _cellDigits = new CellDigit[81];

	/// <summary>
	/// Indicates the candidate digits.
	/// </summary>
	private readonly CandidateDigit[] _candidateDigits = new CandidateDigit[81];


	/// <summary>
	/// Initializes a <see cref="SudokuGrid"/> instance via the details.
	/// </summary>
	/// <param name="grid">The <see cref="Grid"/> instance.</param>
	/// <param name="showCandidates">Whether the grid displays candidates.</param>
	/// <param name="paneSize">The pane size.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	/// <param name="givenColor">The given text color.</param>
	/// <param name="modifiableColor">The modifiable text color.</param>
	/// <param name="candidateColor">The candidate text color.</param>
	/// <param name="valueFontName">The given or modifiable font name.</param>
	/// <param name="candidateFontName">The candidate font name.</param>
	/// <param name="valueFontSize">The value font size.</param>
	/// <param name="candidateFontSize">The candidate font size.</param>
	public SudokuGrid(
		in Grid grid, bool showCandidates, double paneSize, double outsideOffset,
		Color givenColor, Color modifiableColor, Color candidateColor,
		string valueFontName, string candidateFontName, double valueFontSize, double candidateFontSize)
	{
		// Assign the basic info.
		_grid = grid;
		_showCandidates = showCandidates;
		_paneSize = paneSize;
		_outsideOffset = outsideOffset;
		_valueFontName = valueFontName;
		_candidateFontName = candidateFontName;
		_valueFontSize = valueFontSize;
		_candidateFontSize = candidateFontSize;

		// Initializes the grid layout.
		_gridLayout = new()
		{
			Width = paneSize,
			Height = paneSize,
			Padding = new(outsideOffset),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};
		for (int i = 0; i < 9; i++)
		{
			_gridLayout.RowDefinitions.Add(new());
			_gridLayout.ColumnDefinitions.Add(new());
		}

		// Initializes values.
		for (int i = 0; i < 81; i++)
		{
			_cellDigits[i] = new(givenColor, modifiableColor, valueFontName, valueFontSize);
			var p = _cellDigits[i].GetControl();
			GridLayout.SetRow(p, i / 9);
			GridLayout.SetColumn(p, i % 9);
			_gridLayout.Children.Add(p);

			_candidateDigits[i] = new(showCandidates, candidateFontName, candidateFontSize, candidateColor);
			var q = _candidateDigits[i].GetControl();
			GridLayout.SetRow(q, i / 9);
			GridLayout.SetColumn(q, i % 9);
			_gridLayout.Children.Add(q);
		}

		// Then initialize other items.
		CoverGrid(grid);
	}


	/// <summary>
	/// Indicates whether the grid displays for candidates.
	/// </summary>
	public bool ShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_showCandidates == value)
			{
				return;
			}

			_showCandidates = value;
			Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.ShowDigits = value);
		}
	}

	/// <summary>
	/// Gets or sets the outside offset.
	/// </summary>
	public double OutsideOffset
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _outsideOffset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_outsideOffset.NearlyEquals(value))
			{
				return;
			}

			_outsideOffset = value;
			_gridLayout.Padding = new(value);
		}
	}

	/// <summary>
	/// Gets or sets the pane size.
	/// </summary>
	public double PaneSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _paneSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_paneSize.NearlyEquals(value))
			{
				return;
			}

			_paneSize = value;
			_gridLayout.Width = value;
			_gridLayout.Height = value;
		}
	}

	/// <summary>
	/// Gets or sets the given or modifiable font size.
	/// </summary>
	public double ValueFontSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _valueFontSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_valueFontSize = value;
			Array.ForEach(_cellDigits, cellDigit => cellDigit.FontSize = value);
		}
	}

	/// <summary>
	/// Gets or sets the candidate font size.
	/// </summary>
	public double CandidateFontSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateFontSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_candidateFontSize = value;
			Array.ForEach(_candidateDigits, candidateDIgit => candidateDIgit.FontSize = value);
		}
	}

	/// <summary>
	/// Gets or sets the given or modifiable font name.
	/// </summary>
	public string ValueFontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _valueFontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_valueFontName = value;
			Array.ForEach(_cellDigits, cellDigit => cellDigit.FontName = value);
		}
	}

	/// <summary>
	/// Gets or sets the candidate font name.
	/// </summary>
	public string CandidateFontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateFontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_candidateFontName = value;
			Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.FontName = value);
		}
	}

	/// <summary>
	/// Gets or sets the given color.
	/// </summary>
	public Color GivenColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cellDigits[0].GivenColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_cellDigits, cellDigit => cellDigit.GivenColor = value);
	}

	/// <summary>
	/// Gets or sets the modifiable color.
	/// </summary>
	public Color ModifiableColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cellDigits[0].ModifiableColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_cellDigits, cellDigit => cellDigit.ModifiableColor = value);
	}

	/// <summary>
	/// Gets or sets the candidate color.
	/// </summary>
	public Color CandidateColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateDigits[0].Color;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Array.ForEach(_candidateDigits, candidateDigit => candidateDigit.Color = value);
	}

	/// <summary>
	/// Gets or sets the grid.
	/// </summary>
	public Grid Grid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _grid;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => CoverGrid(_grid = value);
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is SudokuGrid comparer && _grid == comparer._grid;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => _grid.GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl() => _gridLayout;

	/// <summary>
	/// To cover the grid info.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void CoverGrid(in Grid grid)
	{
		for (int i = 0; i < 81; i++)
		{
			switch (grid.GetStatus(i))
			{
				case CellStatus.Empty:
				{
					_cellDigits[i].Digit = byte.MaxValue;
					_cellDigits[i].IsGiven = false;
					_candidateDigits[i].CandidateMask = grid.GetCandidates(i);

					break;
				}
				case var status and (CellStatus.Given or CellStatus.Modifiable):
				{
					_cellDigits[i].Digit = (byte)grid[i];
					_cellDigits[i].IsGiven = status == CellStatus.Given;
					_candidateDigits[i].CandidateMask = 0;

					break;
				}
			}
		}
	}
}
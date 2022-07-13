﻿namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the candidate-leveled digit.
/// The data structure represents for all possible candidates in a cell.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
internal sealed class CandidateDigit : DrawingElement
{
	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	private readonly GridLayout? _grid;

	/// <summary>
	/// Indicates the digit blocks.
	/// </summary>
	private readonly TextBlock[] _digitBlocks = new TextBlock[9];

	/// <summary>
	/// Indicates the user preference.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;

	/// <summary>
	/// Indicates whether the current mode is mask mode.
	/// </summary>
	private bool _isMaskMode;

	/// <summary>
	/// Indicates whether the user controls on showing candidates.
	/// </summary>
	private bool _showsCandidates;

	/// <summary>
	/// Indicates the candidate mask.
	/// </summary>
	private short _candidateMask;

	/// <summary>
	/// Indicates the wrong digit mask.
	/// </summary>
	private short _wrongDigitMask;


	/// <summary>
	/// Gets or sets the value indicating whether the candidate block shows digits.
	/// </summary>
	public bool ShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.ShowCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_preference.ShowCandidates == value || _grid is null)
			{
				return;
			}

			_preference.ShowCandidates = value;
			_grid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	/// <summary>
	/// Gets or sets the value indicating whether user controls on displaying candidates.
	/// </summary>
	public bool UserShowCandidates
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _showsCandidates;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_showsCandidates == value || _grid is null)
			{
				return;
			}

			_showsCandidates = value;
			_grid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
		}
	}

	/// <summary>
	/// Gets or sets the value indicating whether the current mode is mask mode.
	/// </summary>
	public required bool IsMaskMode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isMaskMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_isMaskMode == value || _grid is null)
			{
				return;
			}

			if (_isMaskMode && !ShowCandidates)
			{
				return;
			}

			_isMaskMode = value;
			_grid.Visibility = value || !_showsCandidates ? Visibility.Collapsed : Visibility.Visible;
		}
	}

	/// <summary>
	/// Indicates the candidate mask.
	/// </summary>
	public required short CandidateMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _candidateMask;

		set
		{
			Argument.ThrowIfFalse(value is >= 0 and <= 511);

			_candidateMask = value;
			for (byte digit = 0; digit < 9; digit++)
			{
				_digitBlocks[digit].Visibility =
					((value | _wrongDigitMask) >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed;
			}
		}
	}

	/// <summary>
	/// Indicates the wrong digits mask.
	/// </summary>
	public required short WrongDigitMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _wrongDigitMask;

		set
		{
			Argument.ThrowIfFalse(value is >= 0 and <= 511);

			_wrongDigitMask = value;
			for (byte digit = 0; digit < 9; digit++)
			{
				scoped ref var current = ref _digitBlocks[digit];
				current.Visibility =
					((_candidateMask | value) >> digit & 1) != 0 ? Visibility.Visible : Visibility.Collapsed;
				current.Foreground = new SolidColorBrush(
					(_wrongDigitMask >> digit & 1) != 0
						? _preference.CandidateDeltaColor
						: _preference.CandidateColor
				);
			}
		}
	}

	/// <summary>
	/// Indicates the user preference.
	/// </summary>
	public required IDrawingPreference UserPreference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference;

		[MemberNotNull(nameof(_grid))]
		init
		{
			_preference = value;

			_showsCandidates = _preference.ShowCandidates;
			_grid ??= createGrid(_digitBlocks, value);

			static GridLayout createGrid(TextBlock[] textBlocks, IDrawingPreference value)
			{
				var grid = new GridLayout()
					.WithVisibility(value.ShowCandidates ? Visibility.Visible : Visibility.Collapsed)
					.WithRowDefinitionsCount(3)
					.WithColumnDefinitionsCount(3);

				for (byte digit = 0; digit < 9; digit++)
				{
					var digitBlock = new TextBlock
					{
						Text = (digit + 1).ToString(),
						FontFamily = new(value.CandidateFont.FontName),
						FontSize = 60 * value.CandidateFont.FontScale,
						TextAlignment = TextAlignment.Center,
						HorizontalTextAlignment = TextAlignment.Center
					};

					GridLayout.SetRow(digitBlock, digit / 3);
					GridLayout.SetColumn(digitBlock, digit % 3);
					textBlocks[digit] = digitBlock;
					grid.Children.Add(digitBlock);
				}

				return grid;
			}
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CandidateDigit);

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
			int c = PopCount((uint)_candidateMask);
			scoped var sb = new StringHandler();

			byte i = 0;
			foreach (int digit in _candidateMask)
			{
				sb.Append(digit);
				if (++i != c)
				{
					sb.Append(", ");
				}
			}

			return $"{nameof(CandidateDigit)} {{ Digits = {sb.ToStringAndClear()} }}";
		}
	}
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is CandidateDigit comparer && _candidateMask == comparer._candidateMask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _candidateMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override GridLayout GetControl()
		=> _grid ?? throw new InvalidOperationException("The value cannot be null.");
}

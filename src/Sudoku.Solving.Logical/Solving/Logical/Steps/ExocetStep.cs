﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet</b> technique.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Exocet">INdicates the exocet pattern.</param>
/// <param name="DigitsMask">Indicates the mask that holds all possible digits used.</param>
/// <param name="Eliminations">Indicates all possible eliminations.</param>
internal abstract record ExocetStep(
	ViewList Views,
	scoped in ExocetPattern Exocet,
	short DigitsMask,
	ImmutableArray<ExocetElimination> Eliminations
) : Step(GatherConclusions(Eliminations), Views), IStepWithRank
{
	/// <inheritdoc/>
	public virtual int Rank => 0;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Exocet;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Exocet;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.HardlyEver;

	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	/// <summary>
	/// Indicates the base map string.
	/// </summary>
	[ResourceTextFormatter]
	internal string BaseCellsStr() => BaseMap.ToString();

	/// <summary>
	/// Indicates the target map string.
	/// </summary>
	[ResourceTextFormatter]
	internal string TargetCellsStr() => TargetMap.ToString();

	/// <summary>
	/// Indicates the map of the base cells.
	/// </summary>
	private CellMap BaseMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.BaseCellsMap;
	}

	/// <summary>
	/// Indicates the map of the target cells.
	/// </summary>
	private CellMap TargetMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.TargetCellsMap;
	}


	/// <inheritdoc/>
	public sealed override string ToFullString()
	{
		scoped var sb = new StringHandler(100);
		sb.Append(base.ToFullString());
		sb.AppendLine();
		sb.AppendRangeWithLines(Eliminations);

		return sb.ToStringAndClear();
	}


	/// <summary>
	/// Gather conclusions.
	/// </summary>
	/// <returns>The gathered result.</returns>
	private static ConclusionList GatherConclusions(ImmutableArray<ExocetElimination> eliminations)
	{
		var result = new List<Conclusion>();
		foreach (var eliminationInstance in eliminations)
		{
			result.AddRange(eliminationInstance.Conclusions);
		}

		return ImmutableArray.CreateRange(result);
	}
}

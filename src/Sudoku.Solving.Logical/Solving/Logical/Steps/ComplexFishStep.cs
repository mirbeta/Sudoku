﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Fish</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="BaseSetsMask"><inheritdoc/></param>
/// <param name="CoverSetsMask"><inheritdoc/></param>
/// <param name="Exofins">The exo-fins.</param>
/// <param name="Endofins">The endo-fins.</param>
/// <param name="IsFranken">Indicates whether the fish is a Franken fish.</param>
/// <param name="IsSashimi">Indicates whether the fish is a Sashimi fish.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record ComplexFishStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit,
	int BaseSetsMask,
	int CoverSetsMask,
	scoped in CellMap Exofins,
	scoped in CellMap Endofins,
	bool IsFranken,
	bool? IsSashimi
) :
	FishStep(Conclusions, Views, Digit, BaseSetsMask, CoverSetsMask),
	IDistinctableStep<ComplexFishStep>,
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => stackalloc[] { 0, 0, 3.2M, 3.8M, 5.2M, 6.0M, 6.0M, 6.6M, 7.0M }[Size];

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			(
				PhasedDifficultyRatingKinds.Sashimi,
				IsSashimi switch
				{
					false => stackalloc[] { 0, 0, .2M, .2M, .2M, .3M, .3M, .3M, .4M }[Size],
					true => stackalloc[] { 0, 0, .3M, .3M, .4M, .4M, .5M, .6M, .7M }[Size],
					_ => 0
				}
			),
			(
				PhasedDifficultyRatingKinds.FishShape,
				IsFranken
					? stackalloc[] { 0, 0, .2M, 1.2M, 1.2M, 1.3M, 1.3M, 1.3M, 1.4M }[Size]
					: stackalloc[] { 0, 0, .3M, 1.4M, 1.4M, 1.5M, 1.5M, 1.5M, 1.6M }[Size]
			)
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel
		=> Size switch
		{
			2 => DifficultyLevel.Hard,
			3 or 4 => DifficultyLevel.Fiendish,
			_ => DifficultyLevel.Nightmare
		};

	/// <inheritdoc/>
	public override Technique TechniqueCode => GetComplexFishTechniqueCodeFromName(InternalName);

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.ComplexFish;

	/// <inheritdoc/>
	public override Rarity Rarity
		=> (Size, ShapeModifier, FinModifier) switch
		{
			(3, _, _) => Rarity.Sometimes,
			(4, ComplexFishShapeKind.Franken, _) => Rarity.Sometimes,
			(4, ComplexFishShapeKind.Mutant, _) => Rarity.Seldom,
			(5, ComplexFishShapeKind.Franken, ComplexFishFinKind.Sashimi) => Rarity.Seldom,
			(5, ComplexFishShapeKind.Mutant, _) => Rarity.Seldom,
			_ => Rarity.HardlyEver
		};

	/// <summary>
	/// The internal name.
	/// </summary>
	private string InternalName
	{
		get
		{
			var fin = FinModifier == ComplexFishFinKind.Normal ? null : $"{FinModifier} ";
			var shape = ShapeModifier == ComplexFishShapeKind.Basic ? null : $"{ShapeModifier} ";
			var sizeName = Size switch
			{
				2 => "X-Wing",
				3 => "Swordfish",
				4 => "Jellyfish",
				5 => "Squirmbag",
				6 => "Whale",
				7 => "Leviathan"
			};
			return $"{fin}{shape}{sizeName}";
		}
	}

	/// <summary>
	/// Indicates the fin modifier.
	/// </summary>
	private ComplexFishFinKind FinModifier
		=> IsSashimi switch
		{
			true => ComplexFishFinKind.Sashimi,
			false => ComplexFishFinKind.Finned,
			_ => ComplexFishFinKind.Normal
		};

	/// <summary>
	/// The shape modifier.
	/// </summary>
	private ComplexFishShapeKind ShapeModifier
		=> IsFranken ? ComplexFishShapeKind.Franken : ComplexFishShapeKind.Mutant;

	/// <summary>
	/// Indicates the base houses.
	/// </summary>
	private int[] BaseHouses
	{
		get
		{
			var result = new int[PopCount((uint)BaseSetsMask)];
			var i = 0;
			foreach (var house in BaseSetsMask)
			{
				result[i++] = house;
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the cover houses.
	/// </summary>
	private int[] CoverHouses
	{
		get
		{
			var result = new int[PopCount((uint)CoverSetsMask)];
			var i = 0;
			foreach (var house in CoverSetsMask)
			{
				result[i++] = house;
			}

			return result;
		}
	}

	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();

	[ResourceTextFormatter]
	internal string BaseSetsStr() => HouseFormatter.Format(BaseHouses);

	[ResourceTextFormatter]
	internal string CoverSetsStr() => HouseFormatter.Format(CoverHouses);

	[ResourceTextFormatter]
	internal string ExofinsStr() => Exofins ? $"f{Exofins} " : string.Empty;

	[ResourceTextFormatter]
	internal string EndofinsStr() => Endofins ? $"ef{Endofins} " : string.Empty;


	/// <inheritdoc/>
	public static bool Equals(ComplexFishStep left, ComplexFishStep right)
		=> left.Digit == right.Digit
		&& left.BaseSetsMask == right.BaseSetsMask
		&& left.CoverSetsMask == right.CoverSetsMask
		&& left.Exofins == right.Exofins
		&& left.Endofins == right.Endofins;
}

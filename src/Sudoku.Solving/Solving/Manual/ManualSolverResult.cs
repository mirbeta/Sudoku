﻿namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides the solver result after <see cref="ManualSolver"/> solves a puzzle.
/// </summary>
/// <param name="OriginalPuzzle"><inheritdoc/></param>
[AutoDeconstruct(nameof(IsSolved), nameof(SolvingStepsCount), nameof(Steps))]
[AutoDeconstruct(nameof(IsSolved), nameof(TotalDifficulty), nameof(MaxDifficulty), nameof(PearlDifficulty), nameof(DiamondDifficulty), nameof(OriginalPuzzle), nameof(Solution), nameof(ElapsedTime), nameof(SolvingStepsCount), nameof(Steps), nameof(StepGrids))]
[AutoGetEnumerator(nameof(Steps), ExtraNamespaces = new[] { "System", "Sudoku.Solving.Manual" }, ReturnType = typeof(ImmutableArray<Step>.Enumerator), MemberConversion = "@.*")]
[AutoFormattable]
public sealed partial record ManualSolverResult(in Grid OriginalPuzzle) : ISolverResult, IFormattable
{
	/// <inheritdoc/>
	public bool IsSolved { get; init; }

	/// <inheritdoc/>
	public FailedReason FailedReason { get; init; }

	/// <inheritdoc/>
	public Grid Solution { get; init; }

	/// <inheritdoc/>
	public TimeSpan ElapsedTime { get; init; }

	/// <summary>
	/// <para>
	/// Indicates the wrong step found. In general cases, if the property <see cref="IsSolved"/> keeps
	/// <see langword="false"/> value, it'll mean the puzzle is invalid to solve, or the solver has found
	/// one error step to apply, that causes the original puzzle <see cref="OriginalPuzzle"/> become invalid.
	/// In this case we can check this property to get the wrong information to debug the error,
	/// or tell the author himself directly, with the inner value of this property held.
	/// </para>
	/// <para>
	/// However, if the puzzle is successful to be solved, the property won't contain any value,
	/// so it'll keep the <see langword="null"/> reference. Therefore, please check the nullability
	/// of this property before using.
	/// </para>
	/// <para>
	/// In general, this table will tell us the nullability of this property:
	/// <list type="table">
	/// <listheader>
	/// <term>Nullability</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Not <see langword="null"/></term>
	/// <description>The puzzle is failed to solve, and the solver has found an invalid step to apply.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>Other cases.</description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	/// <seealso cref="IsSolved"/>
	/// <seealso cref="OriginalPuzzle"/>
	public Step? WrongStep { get; init; }

	/// <summary>
	/// Indicates a list, whose element is the intermediate grid for each step.
	/// </summary>
	/// <seealso cref="Steps"/>
	public ImmutableArray<Grid> StepGrids { get; init; }

	/// <summary>
	/// Indicates all solving steps that the solver has recorded.
	/// </summary>
	/// <seealso cref="StepGrids"/>
	public ImmutableArray<Step> Steps { get; init; }

	/// <summary>
	/// <para>Indicates the maximum difficulty of the puzzle.</para>
	/// <para>
	/// When the puzzle is solved by <see cref="ManualSolver"/>,
	/// the value will be the maximum value among all difficulty
	/// ratings in solving steps. If the puzzle has not been solved,
	/// or else the puzzle is solved by other solvers, this value will
	/// be always <c>20.0M</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public unsafe decimal MaxDifficulty => Evaluator(&Enumerable.Max<Step>, 20.0M);

	/// <summary>
	/// <para>Indicates the total difficulty rating of the puzzle.</para>
	/// <para>
	/// When the puzzle is solved by <see cref="ManualSolver"/>,
	/// the value will be the sum of all difficulty ratings of steps. If
	/// the puzzle has not been solved, the value will be the sum of all
	/// difficulty ratings of steps recorded in <see cref="Steps"/>.
	/// However, if the puzzle is solved by other solvers, this value will
	/// be <c>0</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	/// <seealso cref="Steps"/>
	public unsafe decimal TotalDifficulty => Evaluator(&Enumerable.Sum<Step>, 0);

	/// <summary>
	/// <para>
	/// Indicates the pearl difficulty rating of the puzzle, calculated
	/// during only by <see cref="ManualSolver"/>.
	/// </para>
	/// <para>
	/// When the puzzle is solved, the value will be the difficulty rating
	/// of the first solving step. If the puzzle has not solved or
	/// the puzzle is solved by other solvers, this value will be always <c>0</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public decimal PearlDifficulty => Steps.IsDefaultOrEmpty
		? 0
		: Steps.FirstOrDefault(static info => info.ShowDifficulty)?.Difficulty ?? 0;

	/// <summary>
	/// <para>
	/// Indicates the pearl difficulty rating of the puzzle, calculated
	/// during only by <see cref="ManualSolver"/>.
	/// </para>
	/// <para>
	/// When the puzzle is solved, the value will be the difficulty rating
	/// of the first step before the first one whose conclusion is
	/// <see cref="ConclusionType.Assignment"/>. If the puzzle has not solved
	/// or solved by other solvers, this value will be <c>20.0M</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	/// <seealso cref="ConclusionType"/>
	public decimal DiamondDifficulty
	{
		get
		{
			if (!Steps.IsDefaultOrEmpty)
			{
				for (int i = 0, length = Steps.Length; i < length; i++)
				{
					var step = Steps[i];
					if (step.HasTag(TechniqueTags.Singles))
					{
						if (i == 0)
						{
							return step.Difficulty;
						}
						else
						{
							decimal max = 0;
							for (int j = 0; j < i; j++)
							{
								decimal difficulty = Steps[j].Difficulty;
								if (difficulty >= max)
								{
									max = difficulty;
								}
							}

							return max;
						}
					}
				}
			}

			return 20.0M;
		}
	}

	/// <summary>
	/// Indicates the number of all solving steps recorded.
	/// </summary>
	public int SolvingStepsCount => Steps.IsDefault ? 1 : Steps.Length;

	/// <summary>
	/// Indicates the difficulty level of the puzzle.
	/// If the puzzle has not solved or solved by other solvers,
	/// this value will be <see cref="DifficultyLevel.Unknown"/>.
	/// </summary>
	public DifficultyLevel DifficultyLevel
	{
		get
		{
			var maxLevel = DifficultyLevel.Unknown;
			if (IsSolved)
			{
				foreach (var step in Steps)
				{
					if (step.ShowDifficulty && step.DifficultyLevel > maxLevel)
					{
						maxLevel = step.DifficultyLevel;
					}
				}
			}

			return maxLevel;
		}
	}

	/// <summary>
	/// Gets the bottleneck during the whole grid solving. Returns <see langword="null"/> if the property
	/// <see cref="Steps"/> is default case (not initialized or empty).
	/// </summary>
	/// <seealso cref="Steps"/>
	public Step? Bottleneck
	{
		get
		{
			if (Steps.IsDefaultOrEmpty)
			{
				return null;
			}

			for (int i = Steps.Length - 1; i >= 0; i--)
			{
				var step = Steps[i];
				if (step.ShowDifficulty && !step.HasTag(TechniqueTags.Singles))
				{
					return step;
				}
			}

			// If code goes to here, all steps are more difficult than single techniques.
			// Get the first one is okay.
			return Steps[0];
		}
	}


	/// <summary>
	/// Gets the <see cref="Step"/> instance at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The step information.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the <see cref="Steps"/> is <see langword="null"/> or empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	/// <seealso cref="Steps"/>
	public Step this[int index] => Steps is not { IsDefaultOrEmpty: false, Length: not 0 }
		? throw new InvalidOperationException("You can't extract any elements because of being null or empty.")
		: index >= Steps.Length || index < 0
			? throw new IndexOutOfRangeException($"Parameter '{nameof(index)}' is out of range.")
			: Steps[index];

	/// <summary>
	/// Gets the first <see cref="Step"/> instance that matches the specified technique.
	/// </summary>
	/// <param name="code">The technique code to check and fetch.</param>
	/// <returns>The step information instance as the result.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the list doesn't contain any valid instance to get.
	/// </exception>
	public Step this[Technique code] => IsSolved
		? Steps.First(step => step.TechniqueCode == code)
		: throw new InvalidOperationException("The specified instance can't get the result.");


	/// <summary>
	/// Get the analysis result string using the specified format and the country code.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <param name="countryCode">The country code.</param>
	/// <returns>The result string.</returns>
	public string ToString(string format, CountryCode countryCode) =>
		new Formatter(this).ToString(format, null, countryCode);

	/// <inheritdoc/>
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		new Formatter(this).ToString(format, formatProvider);

	/// <inheritdoc cref="Formatter.ToString(SolverResultFormattingOptions)"/>
	public string ToString(SolverResultFormattingOptions options) =>
		new Formatter(this).ToString(options);

	/// <inheritdoc cref="Formatter.ToString(SolverResultFormattingOptions, CountryCode)"/>
	public string ToString(SolverResultFormattingOptions options, CountryCode countryCode) =>
		new Formatter(this).ToString(options, countryCode);

	/// <inheritdoc/>
	public string ToDisplayString() => ToDisplayString(CountryCode.Default);

	/// <inheritdoc/>
	public string ToDisplayString(CountryCode countryCode) => new Formatter(this).ToString(null, null, countryCode);

	/// <summary>
	/// The inner executor to get the difficulty value (total, average).
	/// </summary>
	/// <param name="executor">The execute method.</param>
	/// <param name="d">
	/// The default value as the return value when <see cref="Steps"/> is <see langword="null"/> or empty.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Steps"/>
	private unsafe decimal Evaluator(delegate*<IEnumerable<Step>, Func<Step, decimal>, decimal> executor, decimal d) =>
		!Steps.IsDefaultOrEmpty && Steps.Length != 0
			? executor(Steps, static step => step.ShowDifficulty ? step.Difficulty : 0)
			: d;
}
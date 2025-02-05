﻿namespace Sudoku.Solving;

/// <summary>
/// Defines an instance that describes the result after executed the method
/// <see cref="IComplexSolver{TSolver, TSolverResult}.Solve(in Grid, IProgress{double}, CancellationToken)"/>.
/// </summary>
/// <typeparam name="TSolver">The solver's type.</typeparam>
/// <typeparam name="TSolverResult">The type of the target result.</typeparam>
/// <seealso cref="IComplexSolver{TSolver, TSolverResult}.Solve(in Grid, IProgress{double}, CancellationToken)"/>
public interface IComplexSolverResult<in TSolver, out TSolverResult>
	where TSolver : IComplexSolver<TSolver, TSolverResult>
	where TSolverResult : IComplexSolverResult<TSolver, TSolverResult>
{
	/// <summary>
	/// Indicates whether the solver has solved the puzzle.
	/// </summary>
	public abstract bool IsSolved { get; init; }

	/// <summary>
	/// Indicates the elapsed time used during solving the puzzle. The value may not be an useful value.
	/// Some case if the puzzle doesn't contain a valid unique solution, the value may be
	/// <see cref="TimeSpan.Zero"/>.
	/// </summary>
	/// <seealso cref="TimeSpan.Zero"/>
	public abstract TimeSpan ElapsedTime { get; init; }

	/// <summary>
	/// Indicates the original puzzle to be solved.
	/// </summary>
	public abstract Grid Puzzle { get; init; }

	/// <summary>
	/// Indicates the result sudoku grid solved. If the solver can't solve this puzzle, the value will be
	/// <see cref="Grid.Undefined"/>.
	/// </summary>
	/// <seealso cref="Grid.Undefined"/>
	public abstract Grid Solution { get; init; }

	/// <summary>
	/// Indicates the unhandled exception thrown.
	/// </summary>
	public abstract Exception? UnhandledException { get; init; }
}

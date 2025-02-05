﻿namespace Sudoku.Solving;

/// <summary>
/// Defines a complex solver, that uses complex algorithm to solve a puzzle, returning
/// a complex result type instead of an only <see cref="bool"/> value
/// indicating whether the puzzle is successfully solved.
/// </summary>
/// <typeparam name="TSolver">The solver's type.</typeparam>
/// <typeparam name="TSolverResult">The type of the target result.</typeparam>
public interface IComplexSolver<in TSolver, out TSolverResult>
	where TSolver : IComplexSolver<TSolver, TSolverResult>
	where TSolverResult : IComplexSolverResult<TSolver, TSolverResult>
{
	/// <summary>
	/// To solve the specified puzzle.
	/// </summary>
	/// <param name="puzzle">The puzzle to be solved.</param>
	/// <param name="progress">The progress instance that is used for reporting the status.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The solver result that provides the information after solving.</returns>
	public abstract TSolverResult Solve(scoped in Grid puzzle, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}

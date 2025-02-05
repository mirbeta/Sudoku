﻿namespace Sudoku.Solving.Logical.Steps.Specialized;

/// <summary>
/// Defines a step type that contains the property <c>Size</c>.
/// </summary>
public interface IStepWithSize : IStep
{
	/// <summary>
	/// Indicates the size of the current step instance related to the specified technique.
	/// </summary>
	public abstract int Size { get; }
}

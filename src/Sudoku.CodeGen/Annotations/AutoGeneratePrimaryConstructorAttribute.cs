﻿using System;

namespace Sudoku.CodeGen.Annotations
{
	/// <summary>
	/// Indicates the marked <see langword="class"/> should generate primary constructor automatically.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AutoGeneratePrimaryConstructorAttribute : Attribute
	{
	}
}

﻿using System;

namespace Sudoku.CodeGen.Annotations
{
	/// <summary>
	/// Indicates the member should be ignored while generating primary constructors.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class IgnoredMemberWhileGeneratingPrimaryConstructorAttribute : Attribute
	{
	}
}

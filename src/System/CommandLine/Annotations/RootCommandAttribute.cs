﻿namespace System.CommandLine.Annotations;

/// <summary>
/// Represents a root command description.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RootCommandAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="RootCommandAttribute"/> instance via the name and its description.
	/// </summary>
	/// <param name="name">The name of the command.</param>
	/// <param name="description">The description of the command.</param>
	public RootCommandAttribute(string name, string description) => (Name, Description) = (name, description);


	/// <summary>
	/// <para>Indicates whether the command is special.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool IsSpecial { get; init; }

	/// <summary>
	/// Indicates the name of the command.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Indicates the description of the command.
	/// </summary>
	public string Description { get; }
}

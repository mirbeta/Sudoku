﻿namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines the incremental source generator that is used for the generation on sync the solution version.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class GlobalConfigValueGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context) =>
		context.RegisterSourceOutput(
			context.AdditionalTextsProvider
				.Where(static file => file.Path.EndsWith("Directory.Build.props"))
				.Select((text, _) => VersionXmlNodeDeterminer(text)),
			static (spc, v) => spc.AddSource("Constants.Version.g.cs", GetSource(v))
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string VersionXmlNodeDeterminer(AdditionalText text) =>
		new XmlDocument()
			.OnLoading(text.Path)
			.DocumentElement
			.SelectNodes("descendant::PropertyGroup")
			.Cast<XmlNode>()
			.FirstOrDefault()
			.ChildNodes
			.OfType<XmlNode>()
			.Where(static element => element.Name == "Version")
			.Select(static element => element.InnerText)
			.First()
			.ToString();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GetSource(string versionValue) =>
		$$"""
		namespace Sudoku.Diagnostics.CodeGen;
		
		partial class Constants
		{
			/// <summary>
			/// Indicates the version of this project.
			/// </summary>
			public const string VersionValue = "{{versionValue}}";
		}
		""";
}

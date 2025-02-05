﻿namespace Global.CodeGen;

/// <summary>
/// Defines the incremental source generator that is used for the generation on sync the solution version.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class VersionValueGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.AdditionalTextsProvider
				.Where(static file => file.Path.EndsWith("Directory.Build.props", StringComparison.Ordinal))
				.Select(
					static (text, _)
						=> new XmlDocument()
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
							.ToString()
				),
			static (spc, v)
				=> spc.AddSource(
					"Constants.Version.g.cs",
					$$"""
					namespace CodeGen;
					
					partial class Constants
					{
						/// <summary>
						/// Indicates the version of this project.
						/// </summary>
						public const string VersionValue = "{{v}}";
					}
					"""
				)
		);
}

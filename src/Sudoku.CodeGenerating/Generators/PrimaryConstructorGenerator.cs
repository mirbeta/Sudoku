﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates a generator that generates primary constructors for <see langword="class"/>es
	/// when they're marked <see cref="AutoGeneratePrimaryConstructorAttribute"/>.
	/// </summary>
	/// <remarks>
	/// This generator can <b>only</b> support non-nested <see langword="class"/>es.
	/// </remarks>
	/// <seealso cref="AutoGeneratePrimaryConstructorAttribute"/>
	[Generator]
	public sealed partial class PrimaryConstructorGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
			var compilation = context.Compilation;
			foreach (var type in
				from candidate in receiver.CandidateClasses
				let model = compilation.GetSemanticModel(candidate.SyntaxTree)
				select model.GetDeclaredSymbol(candidate)! into type
				where type.Marks<AutoGeneratePrimaryConstructorAttribute>()
				select (INamedTypeSymbol)type)
			{
				type.DeconstructInfo(
					false, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out _, out _, out _, out _
				);

				var attributeSymbol = compilation.GetTypeByMetadataName<AutoGeneratePrimaryConstructorAttribute>();
				var baseClassCtorArgs =
					type.BaseType is { } baseType
					&& baseType.GetAttributes().Any(
						a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)
					) ? GetMembers(baseType, handleRecursively: true) : null;
				/*length-pattern*/
				string? baseCtorInheritance = baseClassCtorArgs is not { Count: not 0 }
					? null
					: $" : base({string.Join(", ", from x in baseClassCtorArgs select x.ParameterName)})";

				var members = GetMembers(type, handleRecursively: false);
				string parameterList = string.Join(
					", ",
					from x in baseClassCtorArgs is null ? members : members.Concat(baseClassCtorArgs)
					select $"{x.Type} {x.ParameterName}"
				);
				string memberAssignments = string.Join(
					"\r\n\t\t\t",
					from member in members select $"{member.Name} = {member.ParameterName};"
				);

				context.AddSource(
					type.ToFileName(),
					"PrimaryConstructor",
					$@"#pragma warning disable 1591

using System.Runtime.CompilerServices;

#nullable enable

namespace {namespaceName}
{{
	partial class {type.Name}{genericParametersList}
	{{
		[CompilerGenerated]
		public {type.Name}({parameterList}){baseCtorInheritance}
		{{
			{memberAssignments}
		}}
	}}
}}"
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();


		/// <summary>
		/// Try to get all possible fields or properties in the specified <see langword="class"/> type.
		/// </summary>
		/// <param name="classSymbol">The specified class symbol.</param>
		/// <param name="handleRecursively">
		/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.</param>
		/// <returns>The result list that contains all member symbols.</returns>
		private static IReadOnlyList<SymbolInfo> GetMembers(INamedTypeSymbol classSymbol, bool handleRecursively)
		{
			var result = new List<SymbolInfo>(
				(
					from x in classSymbol.GetMembers().OfType<IFieldSymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&& !x.HasInitializer()
							|| x.Marks<PrimaryConstructorIncludedMemberAttribute>()
						)
						&& !x.Marks<PrimaryConstructorIgnoredMemberAttribute>()
					select new SymbolInfo(
						x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				).Concat(
					from x in classSymbol.GetMembers().OfType<IPropertySymbol>()
					where x is { CanBeReferencedByName: true, IsStatic: false }
						&& (
							x.IsReadOnly
							&& !x.HasInitializer()
							|| x.Marks<PrimaryConstructorIncludedMemberAttribute>()
						)
						&& !x.Marks<PrimaryConstructorIgnoredMemberAttribute>()
					select new SymbolInfo(
						x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
						toCamelCase(x.Name),
						x.Name,
						x.GetAttributes()
					)
				)
			);

			if (handleRecursively
				&& classSymbol.BaseType is { } baseType
				&& baseType.Marks<AutoGeneratePrimaryConstructorAttribute>())
			{
				result.AddRange(GetMembers(baseType, true));
			}

			return result;


			static string toCamelCase(string name)
			{
				name = name.TrimStart('_');
				return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
			}
		}
	}
}

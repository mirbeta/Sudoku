﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that checks the dynamically invocation of the <see langword="dynamic"/>
	/// field <c>TextResources.Current</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class DynamicallyInvocationOfCurrentAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the method name that is called in order to change the language of that resource dictionary.
		/// </summary>
		private const string ChangeLanguageMethodName = "ChangeLanguage";

		/// <summary>
		/// Indicates the method name that is called in order to serialize the object.
		/// </summary>
		private const string SerializeMethodName = "Serialize";

		/// <summary>
		/// Indicates the method name that is called in order to deserialize the object.
		/// </summary>
		private const string DeserializeMethodName = "Deserialize";

		/// <summary>
		/// Indicates the country code type name.
		/// </summary>
		private const string CountryCodeTypeName = "Sudoku.Globalization.CountryCode";

		/// <summary>
		/// Indicates the text resources class name.
		/// </summary>
		private const string TextResourcesClassName = "TextResources";

		/// <summary>
		/// Indicates that field dynamically bound.
		/// </summary>
		private const string TextResourcesStaticReadOnlyFieldName = "Current";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterOperationAction(
				AnalyzeDynamicInvocationOperation,
				new OperationKind[] { OperationKind.DynamicInvocation }
			);
		}

		private static void AnalyzeDynamicInvocationOperation(OperationAnalysisContext context)
		{
			if (context.Compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
			{
				// We don't check on those two WPF projects, because those two projects has already used
				// their own resource dictionary called 'MergedDictionary'.
				return;
			}

			var semanticModel = context.Operation.SemanticModel;
			if (semanticModel is null)
			{
				return;
			}

			var compilation = context.Compilation;
			var symbol = context.ContainingSymbol;
			foreach (var syntaxReference in symbol.DeclaringSyntaxReferences)
			{
				if (
					syntaxReference.GetSyntax() is not InvocationExpressionSyntax
					{
						Expression: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: MemberAccessExpressionSyntax
							{
								RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
								Expression: IdentifierNameSyntax
								{
									Identifier: { ValueText: TextResourcesClassName }
								},
								Name: IdentifierNameSyntax
								{
									Identifier: { ValueText: TextResourcesStaticReadOnlyFieldName }
								}
							},
							Name: IdentifierNameSyntax
							{
								Identifier: { ValueText: var methodName }
							} identifierNameNode
						},
						ArgumentList: var argumentListNode
					} node
				)
				{
					continue;
				}

				if (
					methodName is not (
						ChangeLanguageMethodName or SerializeMethodName or DeserializeMethodName
					)
				)
				{
					ReportSudoku009(context, identifierNameNode, methodName);

					goto CheckSudoku012;
				}

				int actualParamsCount = argumentListNode.Arguments.Count;
				int requiredParamsCount = methodName switch
				{
					ChangeLanguageMethodName => 1,
					SerializeMethodName => 2,
					DeserializeMethodName => 2
				};
				if (actualParamsCount != requiredParamsCount)
				{
					ReportSudoku010(context, methodName, identifierNameNode, actualParamsCount, requiredParamsCount);

					goto CheckSudoku012;
				}

				switch (methodName)
				{
					case ChangeLanguageMethodName:
					{
						if (
							!SymbolEqualityComparer.Default.Equals(
								semanticModel.GetOperation(argumentListNode.Arguments[0].Expression)!.Type,
								compilation.GetTypeByMetadataName(CountryCodeTypeName)
							)
						)
						{
							ReportSudoku011_Case1(context, semanticModel, identifierNameNode, methodName, argumentListNode);
						}

						break;
					}
					case SerializeMethodName:
					case DeserializeMethodName:
					{
						var expectedTypeSymbol = compilation.GetSpecialType(SpecialType.System_String);
						for (int i = 0; i < 2; i++)
						{
							if (
								!SymbolEqualityComparer.Default.Equals(
									semanticModel.GetOperation(argumentListNode.Arguments[i].Expression)!.Type,
									expectedTypeSymbol
								)
							)
							{
								ReportSudoku011_Case2(context, semanticModel, methodName, argumentListNode, i);
							}
						}

						break;
					}
				}

			CheckSudoku012:
				if (node.Parent is not ExpressionStatementSyntax)
				{
					ReportSudoku012(context, node, methodName);
				}
			}
		}


		private static void ReportSudoku009(
			OperationAnalysisContext context, IdentifierNameSyntax identifierNameNode, string methodName) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku009,
						title: Titles.Sudoku009,
						messageFormat: Messages.Sudoku009,
						category: Categories.ResourceDictionary,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku009
					),
					location: identifierNameNode.GetLocation(),
					messageArgs: new[] { methodName }
				)
			);

		private static void ReportSudoku010(
			OperationAnalysisContext context, string methodName, IdentifierNameSyntax nameNode,
			int actualParamsCount, int requiredParamsCount) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku010,
						title: Titles.Sudoku010,
						messageFormat: Messages.Sudoku010,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku010
					),
					location: nameNode.GetLocation(),
					messageArgs: new object[] { methodName, requiredParamsCount, actualParamsCount }
				)
			);

		private static void ReportSudoku011_Case1(
			OperationAnalysisContext context, SemanticModel semanticModel,
			IdentifierNameSyntax identifierNameNode, string? methodName,
			ArgumentListSyntax argListNode) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku011,
						title: Titles.Sudoku011,
						messageFormat: Messages.Sudoku011,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku011
					),
					location: identifierNameNode.GetLocation(),
					messageArgs: new object?[]
					{
						methodName,
						CountryCodeTypeName,
						argListNode.Arguments[0].GetParamFullName(semanticModel)
					}
				)
			);

		private static void ReportSudoku011_Case2(
			OperationAnalysisContext context, SemanticModel semanticModel, string? methodName,
			ArgumentListSyntax argListNode, int i) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku011,
						title: Titles.Sudoku011,
						messageFormat: Messages.Sudoku011,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku011
					),
					location: argListNode.Arguments[i].GetLocation(),
					messageArgs: new object?[]
					{
						methodName,
						"string",
						argListNode.Arguments[i].GetParamFullName(semanticModel)
					}
				)
			);

		private static void ReportSudoku012(
			OperationAnalysisContext context, InvocationExpressionSyntax invocationNode, string methodName) =>
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.Sudoku012,
						title: Titles.Sudoku012,
						messageFormat: Messages.Sudoku012,
						category: Categories.Usage,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.Sudoku012
					),
					location: invocationNode.GetLocation(),
					messageArgs: new[] { methodName }
				)
			);
	}
}

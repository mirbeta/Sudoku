﻿namespace Sudoku.Diagnostics.CodeGen.SyntaxContextReceivers;

/// <summary>
/// Defines a syntax context receiver that provides the gathered node for the usages on the source generator
/// <see cref="AutoOverridesEqualsGenerator"/>.
/// </summary>
/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
/// <seealso cref="AutoOverridesEqualsGenerator"/>
internal sealed record class AutoOverridesEqualsReceiver(CancellationToken CancellationToken) :
	IResultCollectionReceiver<(INamedTypeSymbol Symbol, AttributeData AttributeData)>
{
	/// <inheritdoc/>
	public ICollection<(INamedTypeSymbol Symbol, AttributeData AttributeData)> Collection { get; } =
		new List<(INamedTypeSymbol, AttributeData)>();


	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		const string attributeFullName = "System.Diagnostics.CodeGen.AutoOverridesEqualsAttribute";

		if (
			context is not
			{
				Node: TypeDeclarationSyntax { Modifiers: var modifiers and not [] } n and (
					ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax
				),
				SemanticModel: { Compilation: { } compilation } semanticModel
			}
		)
		{
			return;
		}

		if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { ContainingType: null } typeSymbol)
		{
			return;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(attributeFullName);
		var attributeData = (
			from a in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
			select a
		).FirstOrDefault();
		if (attributeData is null)
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.PartialKeyword))
		{
			return;
		}

		if (!Collection.Any(t => SymbolEqualityComparer.Default.Equals(t.Symbol, typeSymbol)))
		{
			Collection.Add((typeSymbol, attributeData));
		}
	}
}

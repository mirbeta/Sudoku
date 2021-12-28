﻿namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

[SyntaxChecker("SCA0305")]
public sealed partial class PublicFunctionPointerDataMembersSyntaxChecker : ISyntaxContextReceiver
{
	/// <inheritdoc/>
	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (
			context is not
			{
				Node: var memberDeclaration and (FieldDeclarationSyntax or PropertyDeclarationSyntax),
				SemanticModel: var semanticModel
			}
		)
		{
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(memberDeclaration, _cancellationToken);
		if (
			symbol is not
			{
				ContainingType.TypeKind: not TypeKind.Interface,
				DeclaredAccessibility: Accessibility.Public
			}
		)
		{
			return;
		}

		if (
			symbol is not (
				IFieldSymbol { Type: IFunctionPointerTypeSymbol }
				or IPropertySymbol { Type: IFunctionPointerTypeSymbol, IsAbstract: false }
			)
		)
		{
			return;
		}

		Diagnostics.Add(Diagnostic.Create(SCA0305, memberDeclaration.GetLocation(), messageArgs: null));
	}
}

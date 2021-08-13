﻿namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0701")]
public sealed partial class SS0701CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0701));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, leftExprSpan) = diagnostic.AdditionalLocations[0];
		var leftExpr = (ExpressionSyntax)root.FindNode(leftExprSpan, getInnermostNodeForTie: true);
		var (_, rightExprSpan) = diagnostic.AdditionalLocations[1];
		var rightExpr = (ExpressionSyntax)root.FindNode(rightExprSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0701,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactory.ExpressionStatement(
							SyntaxFactory.AssignmentExpression(
								SyntaxKind.CoalesceAssignmentExpression,
								leftExpr,
								rightExpr
							)
						)
						.NormalizeWhitespace()
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0701)
			),
			diagnostic
		);
	}
}

﻿namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0604")]
public sealed partial class SS0604CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0604));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, exprSpan) = diagnostic.AdditionalLocations[0];
		var expr = (ExpressionSyntax)root.FindNode(exprSpan, getInnermostNodeForTie: true);
		var (_, constantSpan) = diagnostic.AdditionalLocations[1];
		var constant = (ExpressionSyntax)root.FindNode(constantSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0604,
				createChangedDocument: async c =>
				{
					string tokenStr = diagnostic.Properties["EqualityToken"]!;

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactory.BinaryExpression(
							tokenStr == "==" ? SyntaxKind.EqualsExpression : SyntaxKind.NotEqualsExpression,
							expr,
							constant
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0604)
			),
			diagnostic
		);
	}
}

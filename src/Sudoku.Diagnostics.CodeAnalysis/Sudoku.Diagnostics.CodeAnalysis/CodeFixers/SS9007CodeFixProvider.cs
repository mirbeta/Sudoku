﻿namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS9007")]
public sealed partial class SS9007CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS9007));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, span) = diagnostic.AdditionalLocations[0];
		var node = (AccessorDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS9007,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						node.WithKeyword(
							SyntaxFactory.Token(
								SyntaxKind.InitKeyword
							)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS9007)
			),
			diagnostic
		);
	}
}

﻿using Sudoku.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.ViewManagement;
using static Sudoku.UI.StringResource;

namespace Sudoku.UI.Views.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SudokuPage : Page
{
	/// <summary>
	/// Initializes a <see cref="SudokuPage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SudokuPage() => InitializeComponent();


	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool EnsureUnsnapped()
	{
		// FilePicker APIs will not work if the application is in a snapped state.
		// If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
		bool unsnapped = ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();
		if (!unsnapped)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, Get("SudokuPage_InfoBar_CannotSnapTheSample"));
		}

		return unsnapped;
	}

	/// <summary>
	/// Asynchronously opening the file, and get the inner content to be parsed to a <see cref="Grid"/> result
	/// to display.
	/// </summary>
	/// <returns>The typical awaitable instance that holds the task to open the file.</returns>
	private async Task OpenFileAsync()
	{
		if (!EnsureUnsnapped())
		{
			return;
		}

		var fop = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
		fop.FileTypeFilter.Add(CommonFileExtensions.Text);
		fop.AwareHandleOnWin32();

		var file = await fop.PickSingleFileAsync();
		if (file is not { Path: var filePath })
		{
			return;
		}

		if (new FileInfo(filePath).Length == 0)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_FileIsEmpty"));
			return;
		}

		// Checks the validity of the file, and reads the whole content.
		string content = await FileIO.ReadTextAsync(file);
		if (string.IsNullOrWhiteSpace(content))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_FileIsEmpty"));
			return;
		}

		// Checks the file content.
		if (!Grid.TryParse(content, out var grid))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_FileIsInvalid"));
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, Get("SudokuPage_InfoBar_FilePuzzleIsNotUnique"));
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, Get("SudokuPage_InfoBar_FileOpenSuccessfully"));
	}

	/// <summary>
	/// Asynchronously saving the file using the current sudoku grid as the base content.
	/// </summary>
	/// <returns>The typical awaitable instance that holds the task to open the file.</returns>
	private async Task SaveFileAsync()
	{
		if (!EnsureUnsnapped())
		{
			return;
		}

		var fsp = new FileSavePicker
		{
			SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
			SuggestedFileName = "Sudoku"
		};
		fsp.FileTypeChoices.Add(Get("FileExtension_TextDescription"), new List<string> { CommonFileExtensions.Text });
		fsp.AwareHandleOnWin32();

		var file = await fsp.PickSaveFileAsync();
		if (file is not { Name: var fileName })
		{
			return;
		}

		// Prevent updates to the remote version of the file until we finish making changes
		// and call CompleteUpdatesAsync.
		CachedFileManager.DeferUpdates(file);

		// Writes to the file.
		await FileIO.WriteTextAsync(file, _cPane.Grid.ToString("#"));

		// Let Windows know that we're finished changing the file so the other app can update
		// the remote version of the file.
		// Completing updates may require Windows to ask for user input.
		var status = await CachedFileManager.CompleteUpdatesAsync(file);
		if (status == FileUpdateStatus.Complete)
		{
			string a = Get("SudokuPage_InfoBar_SaveSuccessfully1");
			string b = Get("SudokuPage_InfoBar_SaveSuccessfully2");
			_cInfoBoard.AddMessage(InfoBarSeverity.Success, $"{a}{fileName}{b}");
		}
		else
		{
			string a = Get("SudokuPage_InfoBar_SaveFailed1");
			string b = Get("SudokuPage_InfoBar_SaveFailed2");
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, $"{a}{fileName}{b}");
		}
	}

	/// <summary>
	/// To paste the text via the clipboard asynchonously.
	/// </summary>
	/// <returns>The typical awaitable instance that holds the task to open the file.</returns>
	private async Task PasteAsync()
	{
		var dataPackageView = Clipboard.GetContent();
		if (!dataPackageView.Contains(StandardDataFormats.Text))
		{
			return;
		}

		string gridStr = await dataPackageView.GetTextAsync();
		if (!Grid.TryParse(gridStr, out var grid))
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_PasteIsInvalid"));
			return;
		}

		// Checks the validity of the parsed grid.
		if (!grid.IsValid)
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Warning, Get("SudokuPage_InfoBar_PastePuzzleIsNotUnique"));
		}

		// Loads the grid.
		_cPane.Grid = grid;
		_cInfoBoard.AddMessage(InfoBarSeverity.Success, Get("SudokuPage_InfoBar_PasteSuccessfully"));
	}


	/// <summary>
	/// Triggers when the current page is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Page_Loaded([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		_cInfoBoard.AddMessage(
			InfoBarSeverity.Informational, Get("SudokuPage_InfoBar_Welcome"),
			Get("Link_SudokuTutorial"), Get("Link_SudokuTutorialDescription"));

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void OpenAppBarButton_ClickAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		await OpenFileAsync();

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ClearAppBarButton_Click(object sender, RoutedEventArgs e)
	{
		_cPane.Grid = Grid.Empty;
		_cInfoBoard.AddMessage(InfoBarSeverity.Informational, Get("SudokuPage_InfoBar_ClearSuccessfully"));
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void CopyAppBarButton_Click([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e)
	{
		ref readonly var grid = ref _cPane.GridByReference();
		if (grid is { IsUndefined: true } or { IsEmpty: true })
		{
			_cInfoBoard.AddMessage(InfoBarSeverity.Error, Get("SudokuPage_InfoBar_CopyFailedDueToEmpty"));
			return;
		}

		new DataPackage { RequestedOperation = DataPackageOperation.Copy }.SetText(grid.ToString("#"));
	}

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void PasteAppBarButton_ClickAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		await PasteAsync();

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void SaveAppBarButton_ClickAsync([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		await SaveFileAsync();

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ClearInfoBarsAppBarButton_Click([IsDiscard] object sender, [IsDiscard] RoutedEventArgs e) =>
		_cInfoBoard.ClearMessages();

	/// <summary>
	/// Triggers when the button is clicked.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void UndoOrRedo_Click(object sender, [IsDiscard] RoutedEventArgs e) =>
		(
			sender is AppBarButton { Tag: var tag }
				? tag switch { "Undo" => _cPane.UndoStep, "Redo" => _cPane.RedoStep }
				: default(Action)!
		)();
}

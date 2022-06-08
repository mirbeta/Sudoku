﻿namespace Sudoku.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// <para>Initializes the singleton application object.</para>
	/// <para>
	/// This is the first line of authored code executed,
	/// and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App() => InitializeComponent();


	/// <summary>
	/// Indicates the preloading sudoku grid.
	/// </summary>
	internal Grid? PreloadingGrid { get; set; } = null;

	/// <summary>
	/// Indicates the main window in this application in the current interaction logic.
	/// </summary>
	internal Window MainWindow { get; private set; } = null!;

	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	internal Preference UserPreference { get; } = new();


	/// <summary>
	/// <para>Invoked when the application is launched normally by the end user.</para>
	/// <para>
	/// Other entry points will be used such as when the application is launched to open a specific file.
	/// </para>
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected override async void OnLaunched(MicrosoftLaunchActivatedEventArgs args)
	{
		// Binds the resource fetcher on type 'MergedResources'.
		R.AddExternalResourceFetecher(GetType().Assembly, static key => Current.Resources[key] as string);

		// Activate the main window.
		if (
			AppInstance.GetCurrent().GetActivatedEventArgs() is
			{
				Kind: ExtendedActivationKind.File,
				Data: IFileActivatedEventArgs { Files: [StorageFile file, ..] }
			}
		)
		{
			PreloadingGrid = Grid.Parse(await FileIO.ReadTextAsync(file));
		}

		(MainWindow = new MainWindow()).Activate();
	}
}

﻿namespace Sudoku.UI.Pages.MainWindow;

/// <summary>
/// Indicates the page that contains the settings.
/// </summary>
public sealed partial class SettingsPage : Page
{
	/// <summary>
	/// Indicates whether the <see cref="Page"/> instance has been initialized.
	/// </summary>
	/// <remarks>
	/// After the constructor called, the value is always <see langword="true"/>.
	/// This field is only used for distinguishing what phase the program executed to.
	/// </remarks>
	private readonly bool _pageIsInitialized;

	/// <summary>
	/// Indicates the list of possible searching option values that is provided to be searched for later.
	/// </summary>
	/// <remarks>
	/// The collection is <see cref="ICollection{T}"/> of type (<see cref="string"/>, <see cref="string"/>[]).
	/// Each items will be a <see cref="string"/>[] because the details will be searched by each word.
	/// </remarks>
	private readonly List<(string Key, string[] Keywords)> _valuesToBeSearched = new();

	/// <summary>
	/// Indicates the queue of steps used as temporary records.
	/// </summary>
	private readonly NotifyChangedList<PreferenceBinding> _boundSteps = new();

	/// <summary>
	/// Indicates the preferences.
	/// </summary>
	private readonly Preference _preference = new();


	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage()
	{
		InitializeComponent();
		InitializeFields();
		InitializeControls();
		addPossibleSearchValues(_valuesToBeSearched);

		_pageIsInitialized = true;


		static void addPossibleSearchValues(ICollection<(string, string[])> collection)
		{
			const string p = "SettingsPage_Option_"; // Of length 20.
			const string q = "_Intro"; // Of length 6.
			const int l = 20, m = 6; // The length of 'p' and 'q'.

			foreach (var mergedDictionary in UiResources.Dictionaries)
			{
				foreach (var kvp in mergedDictionary)
				{
					if (kvp is (key: string { Length: >= l } k, value: string v) && k[..l] == p && k[^m..] != q)
					{
						collection.Add((v, v.Contains(' ') ? v.Split(' ') : new[] { v }));
					}
				}
			}
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ApplicationTheme Route_ToggleSwitch_ApplicationTheme(bool b) =>
		_preference.ApplicationTheme = b ? ApplicationTheme.Dark : ApplicationTheme.Light;


	/// <summary>
	/// Triggers when the specified <see cref="AutoSuggestBox"/> instance is submitted the input.
	/// </summary>
	/// <param name="sender">The instance to trigger that event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void AutoSuggestBox_OptionSearcher_QuerySubmitted(
		AutoSuggestBox sender,
		AutoSuggestBoxQuerySubmittedEventArgs args
	)
	{
		string queryText = args.QueryText;
		if (string.IsNullOrWhiteSpace(queryText))
		{
			return;
		}

		sender.ItemsSource = (
			from pair in _valuesToBeSearched
			where Array.Exists(pair.Keywords, k => k.Contains(queryText, StringComparison.OrdinalIgnoreCase))
			select pair.Key
		).ToArray();
	}

	/// <summary>
	/// Triggers when the one element found is chosen.
	/// </summary>
	/// <param name="sender">The <see cref="AutoSuggestBox"/> instance that triggers this event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void AutoSuggestBox_OptionSearcher_SuggestionChosen(
		AutoSuggestBox sender,
		AutoSuggestBoxSuggestionChosenEventArgs args
	)
	{
		switch ((Sender: sender, Args: args, SettingsPanel))
		{
			case (
				Sender: { Text: var senderText },
				Args: { SelectedItem: string itemValue },
				SettingsPanel: { Children: var controls }
			)
			when !string.IsNullOrWhiteSpace(senderText):
			{
				foreach (var control in controls)
				{
					switch (control)
					{
						case TextBlock { Text: var text, FocusState: FocusState.Unfocused } tb
						when text == itemValue:
						{
							sender.Text = string.Empty;
							tb.Focus(FocusState.Programmatic);

							continue;
						}

						// TODO: Wait for other controls adding.
					}
				}

				break;
			}
		}
	}


	private partial void ToggleSwitch_SashimiFishContainsKeywordFinned_Toggled(object sender, RoutedEventArgs e);
	private partial void ToggleSwitch_UseSizedFishName_Toggled(object sender, RoutedEventArgs e);
	private partial void ToggleSwitch_ApplicationTheme_Toggled(object sender, RoutedEventArgs e);
}

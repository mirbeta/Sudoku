﻿using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.UI.Views.Pages;

/// <summary>
/// Indicates the main page of the window. The page is used for navigation to other pages.
/// </summary>
public sealed partial class MainPage : Page
{
	/// <summary>
	/// Indicates the navigation pairs that controls to route pages.
	/// </summary>
	private readonly (string ViewItemTag, Type PageType)[] _navigationPairs =
	{
		(nameof(SettingsPage), typeof(SettingsPage)),
		(nameof(AboutPage), typeof(AboutPage)),
		(nameof(SudokuPage), typeof(SudokuPage))
	};


	/// <summary>
	/// Initializes a <see cref="MainPage"/> instance.
	/// </summary>
	public MainPage() => InitializeComponent();


	/// <summary>
	/// Try to navigate the pages.
	/// </summary>
	/// <param name="tag">The specified tag of the navigate page item.</param>
	/// <param name="transitionInfo">The transition information.</param>
	private void OnNavigate(string tag, NavigationTransitionInfo transitionInfo)
	{
		var (_, pageType) = _navigationPairs.FirstOrDefault(p => p.ViewItemTag == tag);

		// Get the page type before navigation so you can prevent duplicate entries in the backstack.
		// Only navigate if the selected page isn't currently loaded.
		var preNavPageType = _viewRouterFrame.CurrentSourcePageType;
		if (pageType is not null && preNavPageType != pageType)
		{
			_viewRouterFrame.Navigate(pageType, null, transitionInfo);
		}
	}

	/// <summary>
	/// Triggers when the navigation is failed. The method will be invoked if and only if the routing is invalid.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	/// <exception cref="InvalidOperationException">
	/// Always throws. Because the method is handled with the failure of the navigation,
	/// the throwing is expected.
	/// </exception>
	[DoesNotReturn]
	private void ViewRouterFrame_NavigationFailed([IsDiscard] object sender, NavigationFailedEventArgs e) =>
		throw new InvalidOperationException($"Cannot find the page '{e.SourcePageType.FullName}'.");

	/// <summary>
	/// Triggers when the frame of the navigation view control has navigated to a certain page.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ViewRouterFrame_Navigated(object sender, NavigationEventArgs e)
	{
		if (
			(Sender: sender, EventArg: e, Router: _viewRouter) is not (
				Sender: Frame { SourcePageType: not null },
				EventArg: { SourcePageType: var sourcePageType },
				Router: { MenuItems: var menuItems, FooterMenuItems: var footerMenuItems }
			)
		)
		{
			return;
		}

		var (tag, _) = _navigationPairs.FirstOrDefault(tagSelector);
		var item = menuItems.Concat(footerMenuItems).OfType<NavigationViewItem>().First(itemSelector);
		_viewRouter.SelectedItem = item;
		_viewRouter.Header = item.Content?.ToString();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool tagSelector((string, Type PageType) p) => p.PageType == sourcePageType;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool itemSelector(NavigationViewItem n) => n.Tag as string == tag;
	}

	/// <summary>
	/// Triggers when a page-related navigation item is clicked and selected.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void ViewRouter_ItemInvoked([IsDiscard] NavigationView sender, NavigationViewItemInvokedEventArgs args)
	{
		if (args is { InvokedItemContainer.Tag: string tag, RecommendedNavigationTransitionInfo: var info })
		{
			OnNavigate(tag, info);
		}
	}

	/// <summary>
	/// Triggers when the page-related navigation item, as the selection, is changed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void ViewRouter_SelectionChanged([IsDiscard] NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		if (args is { SelectedItemContainer.Tag: string tag, RecommendedNavigationTransitionInfo: var info })
		{
			OnNavigate(tag, info);
		}
	}
}

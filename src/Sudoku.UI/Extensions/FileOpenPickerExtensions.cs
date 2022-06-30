﻿namespace Windows.Storage.Pickers;

/// <summary>
/// Provides extension methods on <see cref="FileOpenPicker"/>.
/// </summary>
/// <seealso cref="FileOpenPicker"/>
internal static class FileOpenPickerExtensions
{
	/// <summary>
	/// To aware the handle of the window, and apply to the <see cref="FileOpenPicker"/> instance.
	/// </summary>
	/// <param name="this">The instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AwareHandleOnWin32(this FileOpenPicker @this)
	{
		if (Window.Current is null)
		{
			var initializeWithWindowWrapper = @this.As<IInitializeWithWindow>();
			nint hwnd = getActiveWindow();
			initializeWithWindowWrapper.Initialize(hwnd);
		}


		[DllImport("user32", EntryPoint = "GetActiveWindow", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true)]
		static extern nint getActiveWindow();
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileOpenPicker AddFileTypeFilter(this FileOpenPicker @this, string item)
	{
		@this.FileTypeFilter.Add(item);
		return @this;
	}
}

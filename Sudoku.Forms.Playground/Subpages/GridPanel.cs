﻿using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Forms.Subpages
{
	public partial class GridPanel : UserControl
	{
		/// <summary>
		/// Indicates the settings.
		/// </summary>
		private readonly Settings _settings = Settings.DefaultSetting.Clone();

		/// <summary>
		/// Indicates the layer collection.
		/// </summary>
		private LayerCollection _layerCollection;

		/// <summary>
		/// The sudoku grid.
		/// </summary>
		private Grid _grid;

		/// <summary>
		/// The point converter.
		/// </summary>
		private PointConverter _pointConverter;


		public GridPanel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initialization after the initializer <see cref="MainForm.MainForm"/>.
		/// </summary>
		/// <seealso cref="MainForm.MainForm"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializeAfterBase()
		{
			_pointConverter = new PointConverter(_pictureBoxGrid.Width, _pictureBoxGrid.Height);
			_grid = ((Grid)Grid.Empty).Clone();
			_layerCollection = new LayerCollection
			{
				new BackLayer(_pointConverter, _settings.BackgroundColor),
				new GridLineLayer(
					_pointConverter, _settings.GridLineWidth, _settings.GridLineColor),
				new BlockLineLayer(
					_pointConverter, _settings.BlockLineWidth, _settings.BlockLineColor),
				new ValueLayer(
					_pointConverter, _settings.ValueScale, _settings.CandidateScale,
					_settings.GivenColor, _settings.ModifiableColor, _settings.CandidateColor,
					_settings.GivenFontName, _settings.ModifiableFontName,
					_settings.CandidateFontName, _grid)
			};
		}

		/// <summary>
		/// To show the specified form.
		/// </summary>
		/// <typeparam name="TForm">The form type.</typeparam>
		/// <param name="byDialog">Indicates whether the form is shown by dialog.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ShowForm<TForm>(bool byDialog)
			where TForm : Form, new()
		{
			var form = new TForm();
			if (byDialog)
			{
				form.ShowDialog();
			}
			else
			{
				form.Show();
			}
		}

		/// <summary>
		/// To show the image.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ShowImage()
		{
			var bitmap = new Bitmap(_pointConverter.PanelSize.Width, _pointConverter.PanelSize.Height);
			_layerCollection.IntegrateTo(bitmap);
			_pictureBoxGrid.Image = bitmap;

			GC.Collect();
		}

		/// <summary>
		/// Rearrange the location of the control.
		/// </summary>
		/// <param name="sender">The sender triggered the event.</param>
		/// <param name="control">The control to rearrange the location.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RearrangeLocationOf(object sender, Control control)
		{
			if (sender is Control senderControl)
			{
				control.Top = senderControl.Top;
			}
		}

		/// <summary>
		/// To get the mouse point at present.
		/// </summary>
		/// <returns>The point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Point GetMousePoint() => _pictureBoxGrid.PointToClient(MousePosition);

		/// <summary>
		/// To get the snapshot of this form.
		/// </summary>
		/// <returns>The image.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Image GetWindowSnapshot()
		{
			var bitmap = new Bitmap(Width, Height);
			using (var g = Graphics.FromImage(bitmap))
			{
				g.CopyFromScreen(Location, Point.Empty, bitmap.Size);
				return bitmap;
			}
		}

		private void GridPanel_Load(object sender, EventArgs e) =>
			InitializeAfterBase();

		private void GridPanel_SizeChanged(object sender, EventArgs e) =>
			_pictureBoxGrid.Width = _pictureBoxGrid.Height;
	}
}

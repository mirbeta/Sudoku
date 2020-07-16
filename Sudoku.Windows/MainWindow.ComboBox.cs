﻿using System.Windows.Controls;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void ComboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// While initializing, the selection index will be changed to 0.
			// During changing, the label or combo box may be null in this case.
			// So here need null checking.
			if (sender is ComboBox comboBox
				&& !(_labelSymmetry is null) && !(_comboBoxSymmetry is null)
				&& !(_labelBackdoorFilteringDepth is null) && !(_comboBoxBackdoorFilteringDepth is null))
			{
				Settings.GeneratingModeComboBoxSelectedIndex = comboBox.SelectedIndex;
				SwitchOnGeneratingComboBoxesDisplaying();
			}
		}

		private void ComboBoxSymmetry_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When initializing, 'Settings' may be null.
			if (sender is ComboBox comboBox && !(Settings is null))
			{
				Settings.GeneratingSymmetryModeComboBoxSelectedIndex = comboBox.SelectedIndex;
			}
		}

		private void ComboBoxBackdoorFilteringDepth_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When initializing, 'Settings' may be null.
			if (sender is ComboBox comboBox && !(Settings is null))
			{
				Settings.GeneratingBackdoorSelectedIndex = comboBox.SelectedIndex;
			}
		}

		private void ComboBoxDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When initializing, 'Settings' may be null.
			if (sender is ComboBox comboBox && !(Settings is null))
			{
				Settings.GeneratingDifficultyLevelSelectedIndex = comboBox.SelectedIndex;
			}
		}
	}
}

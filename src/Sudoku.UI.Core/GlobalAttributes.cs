﻿[assembly: InternalsVisibleTo("Sudoku.UI")]
[assembly: AutoExtensionDeconstruction(typeof(Line), nameof(Line.X1), nameof(Line.X2), nameof(Line.Y1), nameof(Line.Y2))]
[assembly: AutoExtensionDeconstruction(typeof(Point), nameof(Point.X), nameof(Point.Y), EmitsInKeyword = true)]
[assembly: AutoExtensionDeconstruction(typeof(Color), nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B))]

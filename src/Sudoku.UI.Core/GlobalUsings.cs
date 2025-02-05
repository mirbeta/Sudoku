﻿global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Diagnostics.CodeGen;
global using System.Linq;
global using System.Numerics;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.InteropServices.WindowsRuntime;
global using System.Runtime.Messages;
global using System.Text;
global using System.Threading.Tasks;
global using Microsoft.UI.Input;
global using Microsoft.UI.Text;
global using Microsoft.UI.Windowing;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Data;
global using Microsoft.UI.Xaml.Documents;
global using Microsoft.UI.Xaml.Markup;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Media.Animation;
global using Microsoft.UI.Xaml.Media.Imaging;
global using Microsoft.UI.Xaml.Shapes;
global using Sudoku.Concepts;
global using Sudoku.Presentation;
global using Sudoku.Presentation.Nodes;
global using Sudoku.Solving;
global using Sudoku.UI.Data;
global using Sudoku.UI.Data.Configuration;
global using Sudoku.UI.Interop;
global using Windows.Foundation;
global using Windows.Graphics.Display;
global using Windows.Graphics.Imaging;
global using Windows.Storage;
global using Windows.Storage.Streams;
global using Windows.System;
global using Windows.UI;
global using Windows.UI.Core;
global using Windows.UI.Text;
global using WinRT;
global using WinRT.Interop;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Resources.MergedResources;
global using static Sudoku.SolutionWideReadOnlyFields;
global using GridLayout = Microsoft.UI.Xaml.Controls.Grid;
global using Grid = Sudoku.Concepts.Grid;
global using Point = Windows.Foundation.Point;
global using WinsysDispatcherQueue = Windows.System.DispatcherQueue;
global using WinsysDispatcherQueueController = Windows.System.DispatcherQueueController;
global using Key = Windows.System.VirtualKey;

﻿using System;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using HuajiTech.Mirai.Messaging;
using Sudoku.Bot.Extensions;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual;
using R = Sudoku.Bot.Resources;

namespace Sudoku.Bot
{
	/// <summary>
	/// Encapsulates a plugin for sudoku.
	/// </summary>
	public sealed class SudokuPlugin
	{
		/// <summary>
		/// Indicates the drawing size.
		/// </summary>
		private const int Size = 800;


		/// <summary>
		/// The inner grid painter.
		/// </summary>
		private static GridPainter? _painter;


		/// <summary>
		/// Initializes an instance with the specified user event source.
		/// </summary>
		/// <param name="currentUserEventSource">The current user event source.</param>
		public SudokuPlugin(CurrentUserEventSource currentUserEventSource) =>
			currentUserEventSource.AddMessageReceivedEventHandler(OnReceivingMessageAsync);

		/// <summary>
		/// The method that invoked on receiving messages.
		/// </summary>
		/// <param name="session">The current session.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task OnReceivingMessageAsync(Session session, MessageReceivedEventArgs e)
		{
			var complexStr = e.Message.Content;
			if (complexStr is not { Count: not 0 } || complexStr[0] is not PlainText pl)
			{
				return;
			}

			string info = pl.ToString();
			switch (info)
			{
				case var _ when info.Contains(R.GetValue("MyName")) && info.Contains(R.GetValue("Morning")):
				{
					await GreetingAsync(e);
					break;
				}
				default:
				{
					switch (info.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
					{
						case { Length: >= 1 } s:
						{
							switch (s[0])
							{
								case "！帮助": await ShowHelperTextAsync(e); break;
								case "！分析": await AnalysisAsync(info, e); break;
								case "！生成图片": await DrawImageAsync(info, e); break;
								case "！生成空盘": await GenerateEmptyGridAsync(e); break;
								case "！清盘": await CleanGridAsync(info, e); break;
								case "！关于": await IntroduceAsync(e); break;
								case "！开始绘图": await StartDrawingAsync(info, s, e); break;
								case "！结束绘图": await DisposePainterAsync(e); break;
								case "！填入" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "提示数": await FillAsync(s, e, true); break;
										case "填入数": await FillAsync(s, e, false); break;
									}
									break;
								}
								case "！画" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "单元格": await DrawCellAsync(s, e, false); break;
										case "候选数": await DrawCandidateAsync(s, e, false); break;
										case "行": await DrawRowAsync(s, e, false); break;
										case "列": await DrawColumnAsync(s, e, false); break;
										case "宫": await DrawBlockAsync(s, e, false); break;
										case "链": await DrawChainAsync(s, e, false); break;
										case "叉叉": await DrawCrossAsync(s, e, false); break;
										case "圆圈": await DrawCircleAsync(s, e, false); break;
									}
									break;
								}
								case "！去除" when s.Length >= 2:
								{
									switch (s[1])
									{
										case "单元格": await DrawCellAsync(s, e, true); break;
										case "候选数": await DrawCandidateAsync(s, e, true); break;
										case "行": await DrawRowAsync(s, e, true); break;
										case "列": await DrawColumnAsync(s, e, true); break;
										case "宫": await DrawBlockAsync(s, e, true); break;
										case "链": await DrawChainAsync(s, e, true); break;
										case "叉叉": await DrawCrossAsync(s, e, true); break;
										case "圆圈": await DrawCircleAsync(s, e, true); break;
									}
									break;
								}
							}
							break;
						}
					}
					break;
				}
			}
		}

		/// <summary>
		/// Greet with somebody.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of the method.</returns>
		private static async Task GreetingAsync(MessageReceivedEventArgs e) =>
			await e.Reply(R.GetValue("MorningToo"));

		/// <summary>
		/// Show helper text.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task ShowHelperTextAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder(R.GetValue("Help1"))
				.AppendLine()
				.AppendLine(R.GetValue("Help2"))
				.AppendLine()
				.AppendLine(R.GetValue("HelpHelp"))
				.AppendLine(R.GetValue("HelpAnalyze"))
				.AppendLine(R.GetValue("HelpGeneratePicture"))
				.AppendLine(R.GetValue("HelpClean"))
				.AppendLine(R.GetValue("HelpEmpty"))
				.AppendLine(R.GetValue("HelpStartDrawing"))
				.AppendLine(R.GetValue("HelpEndDrawing"))
				.AppendLine(R.GetValue("HelpIntroduceMyself"))
				.AppendLine()
				.AppendLine(R.GetValue("Help3"))
				.AppendLine(R.GetValue("Help4"))
				.AppendLine(R.GetValue("Help5"))
				.AppendLine()
				.Append(R.GetValue("MyName"))
				.ToString());

		/// <summary>
		/// Introduce myself.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task IntroduceAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder(R.GetValue("Introduce1"))
				.AppendLine()
				.Append(R.GetValue("Introduce2"))
				.ToString());

		/// <summary>
		/// Analyze the puzzle.
		/// </summary>
		/// <param name="info">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task AnalysisAsync(string info, MessageReceivedEventArgs e)
		{
			string analysisCommand = R.GetValue("AnalysisCommand");
			if (!SudokuGrid.TryParse(info[analysisCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			var analysisResult = await new ManualSolver().SolveAsync(grid, null);
			await e.Source.SendAsync(analysisResult.ToString("-!", CountryCode.ZhCn));

			var painter = new GridPainter(new(Size, Size), new(), analysisResult.Solution!);
			using var image = painter.Draw();
			await e.ReplyImageWithTextAsync(image, $"{R.GetValue("Analysis1")}{Environment.NewLine}");
		}

		/// <summary>
		/// Draw the image.
		/// </summary>
		/// <param name="info">The information.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawImageAsync(string info, MessageReceivedEventArgs e)
		{
			if (e.Message.Content.Count != 1)
			{
				return;
			}

			string GeneratePictureCommand = R.GetValue("GeneratePictureCommand");
			if (!SudokuGrid.TryParse(info[GeneratePictureCommand.Length..].Trim(), out var grid)
				|| !grid.IsValid())
			{
				return;
			}

			var painter = new GridPainter(new(Size, Size), new(), grid);
			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Generate an empty grid.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task GenerateEmptyGridAsync(MessageReceivedEventArgs e)
		{
			var painter = new GridPainter(new(Size, Size), new(), SudokuGrid.Undefined);
			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Clean the grid, and send the grid picture.
		/// </summary>
		/// <param name="info">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task CleanGridAsync(string info, MessageReceivedEventArgs e)
		{
			string cleanCommand = R.GetValue("CleaningGridCommand");
			if (!SudokuGrid.TryParse(info[cleanCommand.Length..].Trim(), out var grid) || !grid.IsValid())
			{
				return;
			}

			grid.Clean();
			var painter = new GridPainter(new(Size, Size), new(), grid);

			using var image = painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Start drawing picture.
		/// </summary>
		/// <param name="info">The command arguments.</param>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task StartDrawingAsync(string info, string[] s, MessageReceivedEventArgs e)
		{
			// 开始绘图 [大小 <大小>] [盘面 <盘面>]
			int size = Size;
			var grid = SudokuGrid.Undefined;
			int pos = info.IndexOf(R.GetValue("Size"));
			if (pos != -1)
			{
				int startPos = pos + R.GetValue("Size").Length;
				if (info[startPos] != ' ')
				{
					return;
				}

				int index = startPos + 1;
				for (; info[index] is >= '0' and <= '9'; index++) ;

				string sizeStr = info[startPos..index];
				if (!int.TryParse(sizeStr, out int result))
				{
					return;
				}

				size = result;
			}

			pos = info.IndexOf(R.GetValue("Grid"));
			if (pos != -1)
			{
				int startPos = pos + R.GetValue("Grid").Length;
				if (info[startPos] != ' ')
				{
					return;
				}

				int index = startPos + 1;
				string gridStr = info[startPos..];
				if (!SudokuGrid.TryParse(gridStr, out var result))
				{
					return;
				}

				grid = result;
			}

			var pointConverter = new PointConverter(size, size);
			_painter = new(pointConverter, new(), grid);

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Fill the given and modifiable.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="appendGiven">Indicates whether we should add given values.</param>
		/// <returns>The task of this method.</returns>
		private static async Task FillAsync(string[] s, MessageReceivedEventArgs e, bool appendGiven)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 5)
			{
				return;
			}

			if (s[3] != R.GetValue("To"))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte digit) || digit is <= 0 or > 9)
			{
				return;
			}

			if (!Parser.TryParseCell(s[4], out byte row, out byte column))
			{
				return;
			}

			int cell = row * 9 + column;
			_painter.Grid[cell] = digit - 1;
			if (appendGiven)
			{
				_painter.Grid.SetStatus(cell, CellStatus.Given);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a cell.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCellAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!Parser.TryParseCell(s[2], out byte row, out byte column))
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], out colorId))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			int cell = row * 9 + column;
			if (remove)
			{
				_painter.CustomView.RemoveCell(cell);
			}
			else
			{
				_painter.CustomView.AddCell(colorId, cell);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a candidate.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCandidateAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 5 - (remove ? 1 : 0))
			{
				return;
			}

			if (!Parser.TryParseCell(s[2], out byte row, out byte column))
			{
				return;
			}

			if (!byte.TryParse(s[3], out byte digit) || digit is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[4], out colorId))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			int candidate = row * 81 + column * 9 + digit - 1;
			if (remove)
			{
				_painter.CustomView.RemoveCandidate(candidate);
			}
			else
			{
				_painter.CustomView.AddCandidate(colorId, candidate);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a row.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawRowAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte region) || region is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], out colorId))
			{
				return;
			}

			await DrawRegionAsync(RegionLabel.Row, region - 1, colorId, e, remove);
		}

		/// <summary>
		/// Draw a column.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawColumnAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte region) || region is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], out colorId))
			{
				return;
			}

			await DrawRegionAsync(RegionLabel.Column, region - 1, colorId, e, remove);
		}

		/// <summary>
		/// Draw a block.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawBlockAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (s.Length != 4 - (remove ? 1 : 0))
			{
				return;
			}

			if (!byte.TryParse(s[2], out byte region) || region is <= 0 or > 9)
			{
				return;
			}

			long colorId = default;
			if (!remove && !Parser.TryParseColorId(s[3], out colorId))
			{
				return;
			}

			await DrawRegionAsync(RegionLabel.Block, region - 1, colorId, e, remove);
		}

		/// <summary>
		/// Draw a region.
		/// </summary>
		/// <param name="label">The region label.</param>
		/// <param name="region">The region.</param>
		/// <param name="colorId">The color ID.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawRegionAsync(
			RegionLabel label, int region, long colorId, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			int r = (int)label * 9 + region;
			if (remove)
			{
				_painter.CustomView.RemoveRegion(r);
			}
			else
			{
				_painter.CustomView.AddRegion(colorId, r);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a chain.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawChainAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 8)
			{
				return;
			}

			if (s[2] != R.GetValue("From"))
			{
				return;
			}

			if (!Parser.TryParseCell(s[3], out byte r1, out byte c1)
				|| !Parser.TryParseCell(s[6], out byte r2, out byte c2))
			{
				return;
			}

			if (!byte.TryParse(s[4], out byte d1) || !byte.TryParse(s[7], out byte d2)
				|| d1 is <= 0 or > 9 || d2 is <= 0 or > 9)
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			var link = new Link(r1 * 81 + c1 * 9 + (d1 - 1), r2 * 81 + c2 * 9 + (d2 - 1), LinkType.Default);
			if (remove)
			{
				_painter.CustomView.RemoveLink(link);
			}
			else
			{
				_painter.CustomView.AddLink(link);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a cross sign.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCrossAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 3)
			{
				return;
			}

			if (!Parser.TryParseCell(s[2], out byte row, out byte column))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			if (remove)
			{
				_painter.CustomView.RemoveDirectLine(GridMap.Empty, new() { row * 9 + column });
			}
			else
			{
				_painter.CustomView.AddDirectLine(GridMap.Empty, new() { row * 9 + column });
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Draw a circle sign.
		/// </summary>
		/// <param name="s">The command arguments.</param>
		/// <param name="e">The event arguments.</param>
		/// <param name="remove">Indicates whether the current operation is removing the element.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DrawCircleAsync(string[] s, MessageReceivedEventArgs e, bool remove)
		{
			if (_painter is null)
			{
				return;
			}

			if (s.Length != 3)
			{
				return;
			}

			if (!Parser.TryParseCell(s[2], out byte row, out byte column))
			{
				return;
			}

			if (_painter.CustomView is null)
			{
				_painter = _painter with { CustomView = new() };
			}

			if (remove)
			{
				_painter.CustomView.RemoveDirectLine(new() { row * 9 + column }, GridMap.Empty);
			}
			else
			{
				_painter.CustomView.AddDirectLine(new() { row * 9 + column }, GridMap.Empty);
			}

			using var image = _painter.Draw();
			await e.ReplyImageAsync(image);
		}

		/// <summary>
		/// Dispose the painter.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		/// <returns>The task of this method.</returns>
		private static async Task DisposePainterAsync(MessageReceivedEventArgs e)
		{
			_painter = null;

			await e.Reply(R.GetValue("Success"));
		}
	}
}

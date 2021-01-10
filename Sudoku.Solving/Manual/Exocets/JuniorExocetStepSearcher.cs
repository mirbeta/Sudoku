﻿using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets.Eliminations;
using static System.Numerics.BitOperations;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Encapsulates a <b>junior exocet</b> (JE) technique searcher.
	/// </summary>
	[Obsolete("Please use '" + nameof(JeStepSearcher) + "' instead.", true)]
	public sealed class JuniorExocetStepSearcher : ExocetStepSearcher
	{
		/// <summary>
		/// The iterator for Bi-bi pattern.
		/// </summary>
		private static readonly int[,] BibiIter =
		{
			{ 4, 5, 7, 8 }, { 3, 5, 6, 8 }, { 3, 4, 6, 7 },
			{ 1, 2, 7, 8 }, { 0, 2, 6, 8 }, { 0, 1, 6, 7 },
			{ 1, 2, 4, 5 }, { 0, 2, 3, 5 }, { 0, 1, 3, 4 }
		};


		/// <inheritdoc/>
		public JuniorExocetStepSearcher(bool checkAdvanced) : base(checkAdvanced)
		{
		}


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(34, nameof(TechniqueCode.Je))
		{
			IsEnabled = false,
			DisabledReason = DisabledReason.HasBugs | DisabledReason.Deprecated
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			foreach (var exocet in Patterns)
			{
				var (baseMap, targetMap, _) = exocet;
				var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2) = exocet;

				// The base cells can't be given or modifiable.
				if (baseMap > EmptyMap)
				{
					continue;
				}

				// Base cells should be empty.
				if (grid.GetStatus(b1) != CellStatus.Empty || grid.GetStatus(b2) != CellStatus.Empty)
				{
					continue;
				}

				// The number of different candidates in base cells can't be greater than 5.
				short baseCandidatesMask = (short)(grid.GetCandidates(b1) | grid.GetCandidates(b2));
				if (PopCount((uint)baseCandidatesMask) > 5)
				{
					continue;
				}

				// At least one cell should be empty.
				if ((targetMap & EmptyMap).IsEmpty)
				{
					continue;
				}

				// Then check target eliminations.
				// Here 'nonBaseQ' and 'nonBaseR' are the conjugate pair in target Q and target R cells pair,
				// different with 'lockedMemberQ' and 'lockedMemberR'.
				if (!CheckTarget(grid, tq1, tq2, baseCandidatesMask, out short nonBaseQ, out _)
					|| !CheckTarget(grid, tr1, tr2, baseCandidatesMask, out short nonBaseR, out _))
				{
					continue;
				}

				// Get all locked members.
				int[] mq1o = mq1.ToArray(), mq2o = mq2.ToArray(), mr1o = mr1.ToArray(), mr2o = mr2.ToArray();
				int v1 = grid.GetCandidates(mq1o[0]) | grid.GetCandidates(mq1o[1]);
				int v2 = grid.GetCandidates(mq2o[0]) | grid.GetCandidates(mq2o[1]);
				short temp = (short)(v1 | v2);
				short needChecking = (short)(baseCandidatesMask & temp);
				short lockedMemberQ = (short)(baseCandidatesMask & ~needChecking);

				v1 = grid.GetCandidates(mr1o[0]) | grid.GetCandidates(mr1o[1]);
				v2 = grid.GetCandidates(mr2o[0]) | grid.GetCandidates(mr2o[1]);
				temp = (short)(v1 | v2);
				needChecking &= temp;
				short lockedMemberR = (short)(baseCandidatesMask & ~(baseCandidatesMask & temp));

				// Check crossline.
				if (!CheckCrossline(s, needChecking))
				{
					continue;
				}

				// Gather highlight cells and candidates.
				var cellOffsets = new List<DrawingInfo> { new(0, b1), new(0, b2) };
				var candidateOffsets = new List<DrawingInfo>();
				foreach (int digit in grid.GetCandidates(b1))
				{
					candidateOffsets.Add(new(0, b1 * 9 + digit));
				}
				foreach (int digit in grid.GetCandidates(b2))
				{
					candidateOffsets.Add(new(0, b2 * 9 + digit));
				}

				// Check target eliminations.
				// '|' first, '&&' second. (Do you know my meaning?)
				var targetElims = new Target();
				temp = (short)(nonBaseQ != 0 ? baseCandidatesMask | nonBaseQ : baseCandidatesMask);
				if (GatheringTargetEliminations(tq1, grid, baseCandidatesMask, temp, targetElims)
					| GatheringTargetEliminations(tq2, grid, baseCandidatesMask, temp, targetElims)
					&& nonBaseQ != 0
					&& grid.GetStatus(tq1) == CellStatus.Empty ^ grid.GetStatus(tq2) == CellStatus.Empty)
				{
					int conjugatPairDigit = TrailingZeroCount(nonBaseQ);
					if (grid.Exists(tq1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tq1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tq2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tq2 * 9 + conjugatPairDigit));
					}
				}

				temp = (short)(nonBaseR != 0 ? baseCandidatesMask | nonBaseR : baseCandidatesMask);
				if (GatheringTargetEliminations(tr1, grid, baseCandidatesMask, temp, targetElims)
					| GatheringTargetEliminations(tr2, grid, baseCandidatesMask, temp, targetElims)
					&& nonBaseR != 0
					&& grid.GetStatus(tr1) == CellStatus.Empty ^ grid.GetStatus(tr2) == CellStatus.Empty)
				{
					int conjugatPairDigit = TrailingZeroCount(nonBaseR);
					if (grid.Exists(tr1, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tr1 * 9 + conjugatPairDigit));
					}
					if (grid.Exists(tr2, conjugatPairDigit) is true)
					{
						candidateOffsets.Add(new(1, tr2 * 9 + conjugatPairDigit));
					}
				}

				var (tar1, mir1) =
					GatheringMirrorEliminations(
						tq1, tq2, tr1, tr2, mq1, mq2, nonBaseQ, 0, grid,
						baseCandidatesMask, cellOffsets, candidateOffsets);
				var (tar2, mir2) =
					GatheringMirrorEliminations(
						tr1, tr2, tq1, tq2, mr1, mr2, nonBaseR, 1, grid,
						baseCandidatesMask, cellOffsets, candidateOffsets);
				var targetEliminations = (Target)(targetElims | tar1 | tar2);
				var mirrorEliminations = (Mirror)(mir1 | mir2);
				var bibiEliminations = new BiBiPattern();
				var targetPairEliminations = new TargetPair();
				var swordfishEliminations = new Swordfish();
				if (CheckAdvanced && PopCount((uint)baseCandidatesMask) > 2)
				{
					CheckBibiPattern(
						grid, baseCandidatesMask, b1, b2, tq1, tq2, tr1, tr2, s,
						baseMap.CoveredLine < 18, nonBaseQ, nonBaseR, targetMap, out bibiEliminations,
						out targetPairEliminations, out swordfishEliminations);
				}

				if (CheckAdvanced
					&& (targetEliminations.Count, mirrorEliminations.Count, bibiEliminations.Count) == (0, 0, 0)
					|| !CheckAdvanced && targetEliminations.Count == 0)
				{
					continue;
				}

				cellOffsets.Add(new(1, tq1));
				cellOffsets.Add(new(1, tq2));
				cellOffsets.Add(new(1, tr1));
				cellOffsets.Add(new(1, tr2));
				foreach (int cell in s)
				{
					cellOffsets.Add(new(2, cell));
				}

				accumulator.Add(
					new JeStepInfo(
						new List<Conclusion>(),
						new View[] { new() { Cells = cellOffsets, Candidates = candidateOffsets } },
						exocet,
						baseCandidatesMask.GetAllSets().ToArray(),
						lockedMemberQ == 0 ? null : lockedMemberQ.GetAllSets().ToArray(),
						lockedMemberR == 0 ? null : lockedMemberR.GetAllSets().ToArray(),
						targetEliminations,
						CheckAdvanced ? mirrorEliminations : null,
						bibiEliminations,
						targetPairEliminations,
						swordfishEliminations));
			}
		}

		/// <summary>
		/// Gathering mirror eliminations. This method is an entry for the method check mirror in base class.
		/// </summary>
		/// <param name="tq1">The target Q1 cell.</param>
		/// <param name="tq2">The target Q2 cell.</param>
		/// <param name="tr1">The target R1 cell.</param>
		/// <param name="tr2">The target R2 cell.</param>
		/// <param name="m1">(<see langword="in"/> parameter) The mirror 1 cell.</param>
		/// <param name="m2">(<see langword="in"/> parameter) The mirror 2 cell.</param>
		/// <param name="lockedNonTarget">The locked digits that is not the target digits.</param>
		/// <param name="x">The X digit.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="cellOffsets">The highlight cells.</param>
		/// <param name="candidateOffsets">The highliht candidates.</param>
		/// <returns>The result.</returns>
		private (Target, Mirror) GatheringMirrorEliminations(
			int tq1, int tq2, int tr1, int tr2, in Cells m1, in Cells m2, short lockedNonTarget,
			int x, in SudokuGrid grid, short baseCandidatesMask, List<DrawingInfo> cellOffsets,
			List<DrawingInfo> candidateOffsets)
		{
			if ((grid.GetCandidates(tq1) & baseCandidatesMask) != 0)
			{
				short mask1 = grid.GetCandidates(tr1), mask2 = grid.GetCandidates(tr2);
				short m1d = (short)(mask1 & baseCandidatesMask), m2d = (short)(mask2 & baseCandidatesMask);
				return CheckMirror(
					grid, tq1, tq2, lockedNonTarget != 0 ? lockedNonTarget : 0,
					baseCandidatesMask, m1, x,
					(m1d, m2d) switch { (not 0, 0) => tr1, (0, not 0) => tr2, _ => -1 },
					cellOffsets, candidateOffsets);
			}
			else if ((grid.GetCandidates(tq2) & baseCandidatesMask) != 0)
			{
				short mask1 = grid.GetCandidates(tq1), mask2 = grid.GetCandidates(tq2);
				short m1d = (short)(mask1 & baseCandidatesMask), m2d = (short)(mask2 & baseCandidatesMask);
				return CheckMirror(
					grid, tq2, tq1, lockedNonTarget != 0 ? lockedNonTarget : 0,
					baseCandidatesMask, m2, x,
					(m1d, m2d) switch { (not 0, 0) => tr1, (0, not 0) => tr2, _ => -1 },
					cellOffsets, candidateOffsets);
			}
			else
			{
				return default;
			}
		}

		/// <summary>
		/// The method for gathering target eliminations.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="baseCandidatesMask">The base candidates mask.</param>
		/// <param name="temp">The temp mask.</param>
		/// <param name="targetElims">The target eliminations.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether this method has been found eliminations.
		/// </returns>
		private static bool GatheringTargetEliminations(
			int cell, in SudokuGrid grid, short baseCandidatesMask, short temp, Target targetElims)
		{
			short candidateMask = (short)(grid.GetCandidates(cell) & ~temp);
			if (grid.GetStatus(cell) == CellStatus.Empty && candidateMask != 0
				&& (grid.GetCandidates(cell) & baseCandidatesMask) != 0)
			{
				foreach (int digit in candidateMask)
				{
					targetElims.Add(new(ConclusionType.Elimination, cell, digit));
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Check the cross-line cells.
		/// </summary>
		/// <param name="crossline">(<see langword="in"/> parameter) The cross line cells.</param>
		/// <param name="digitsNeedChecking">The digits that need checking.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the structure passed the validation.
		/// </returns>
		private bool CheckCrossline(in Cells crossline, short digitsNeedChecking)
		{
			foreach (int digit in digitsNeedChecking)
			{
				var crosslinePerCandidate = crossline & DigitMaps[digit];
				int r = crosslinePerCandidate.RowMask, c = crosslinePerCandidate.ColumnMask;
				if ((PopCount((uint)r), PopCount((uint)c)) is not ( > 2, > 2))
				{
					continue;
				}

				bool flag = false;
				foreach (int d1 in r)
				{
					foreach (int d2 in c)
					{
						if (crosslinePerCandidate < (RegionMaps[d1 + 9] | RegionMaps[d2 + 18]))
						{
							flag = true;
							goto FinalCheck;
						}
					}
				}

			FinalCheck:
				if (!flag)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check the target cells.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="pos1">The cell 1 to determine.</param>
		/// <param name="pos2">The cell 2 to determine.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="otherCandidatesMask">
		/// (<see langword="out"/> parameter) The other candidate mask.
		/// </param>
		/// <param name="otherRegion">(<see langword="out"/> parameter) The other region.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		private bool CheckTarget(
			in SudokuGrid grid, int pos1, int pos2, int baseCandidatesMask,
			out short otherCandidatesMask, out int otherRegion)
		{
			otherRegion = -1;
			otherCandidatesMask = -1;

			short m1 = grid.GetCandidates(pos1), m2 = grid.GetCandidates(pos2);
			if ((baseCandidatesMask & m1) != 0 ^ (baseCandidatesMask & m2) != 0)
			{
				// One cell contains the digit that base candidate holds,
				// and another one doesn't contain.
				return true;
			}

			if ((m1 & baseCandidatesMask, m2 & baseCandidatesMask) == (0, 0)
				|| (m1 & ~baseCandidatesMask, m2 & ~baseCandidatesMask) == (0, 0))
			{
				// Two cells don't contain any digits in the base cells neither,
				// or both contains only digits from base cells,
				// which is not allowed in the exocet rule (or doesn't contain
				// any eliminations).
				return false;
			}

			// Now we check the special cases, in other words, the last two cells both contain
			// digits from base cells, and at least one cell contains non-base digits.
			// Therefore, we should check on non-base digits, whether the non-base digits
			// covers only one of two last cells; otherwise, false.
			short candidatesMask = (short)((m1 | m2) & ~baseCandidatesMask);
			var span = (Span<int>)stackalloc[]
			{
				RegionLabel.Block.ToRegion(pos1),
				RegionLabel.Row.ToRegion(pos1) == RegionLabel.Row.ToRegion(pos2)
				? RegionLabel.Row.ToRegion(pos1)
				: RegionLabel.Column.ToRegion(pos1)
			};
			foreach (short mask in Algorithms.GetMaskSubsets(candidatesMask))
			{
				for (int i = 0; i < 2; i++)
				{
					int count = 0;
					for (int j = 0; j < 9; j++)
					{
						int p = RegionCells[span[i]][j];
						if (p == pos1 || p == pos2 || grid.GetStatus(p) != CellStatus.Empty
							|| (grid.GetCandidates(p) & mask) == 0)
						{
							continue;
						}

						count++;
					}

					if (count == PopCount((uint)mask) - 1)
					{
						for (int j = 0; j < 9; j++)
						{
							int p = RegionCells[span[i]][j];
							if (grid.GetStatus(p) != CellStatus.Empty || (grid.GetCandidates(p) & mask) == 0
								|| (grid.GetCandidates(p) & ~mask) == 0 || p == pos1 || p == pos2)
							{
								continue;
							}
						}

						otherCandidatesMask = mask;
						otherRegion = span[i];
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check Bi-bi pattern eliminations.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="baseCandidatesMask">The base candidate mask.</param>
		/// <param name="b1">The base cell 1.</param>
		/// <param name="b2">The base cell 2.</param>
		/// <param name="tq1">The target Q1 cell.</param>
		/// <param name="tq2">The target Q2 cell.</param>
		/// <param name="tr1">The target R1 cell.</param>
		/// <param name="tr2">The target R2 cell.</param>
		/// <param name="crossline">(<see langword="in"/> parameter) The cross-line cells.</param>
		/// <param name="isRow">
		/// Indicates whether the specified exocet is in the horizontal direction.
		/// </param>
		/// <param name="lockedQ">The locked member Q.</param>
		/// <param name="lockedR">The locked member R.</param>
		/// <param name="targetMap">(<see langword="in"/> parameter) The target map.</param>
		/// <param name="bibiElims">
		/// (<see langword="out"/> parameter) The Bi-bi pattern eliminations.
		/// </param>
		/// <param name="targetPairElims">
		/// (<see langword="out"/> parameter) The target pair eliminations.
		/// </param>
		/// <param name="swordfishElims">
		/// (<see langword="out"/> parameter) The swordfish eliminations.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating whether the pattern exists.</returns>
		private bool CheckBibiPattern(
			in SudokuGrid grid, short baseCandidatesMask, int b1, int b2,
			int tq1, int tq2, int tr1, int tr2, in Cells crossline, bool isRow,
			short lockedQ, short lockedR, in Cells targetMap,
			out BiBiPattern bibiElims, out TargetPair targetPairElims, out Swordfish swordfishElims)
		{
			bibiElims = new();
			targetPairElims = new();
			swordfishElims = new();
			var playground = (stackalloc short[3]);
			int block = RegionLabel.Block.ToRegion(b1);
			short[] temp = new short[4];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int p = RegionCells[BibiIter[block, j]][i];
					if (grid.GetCandidates(p).IsPowerOfTwo())
					{
						temp[j] |= grid.GetCandidates(p);
					}
				}
			}

			short commonCandidatesMask = (short)((temp[0] | temp[3]) & (temp[1] | temp[2]) & baseCandidatesMask);
			playground[1] = (short)(temp[0] & temp[3] & baseCandidatesMask & ~commonCandidatesMask & baseCandidatesMask);
			playground[2] = (short)(temp[1] & temp[2] & baseCandidatesMask & ~commonCandidatesMask & baseCandidatesMask);
			if ((playground[1], playground[2]) is not (not 0, not 0))
			{
				// Does not contain Bi-Bi pattern.
				return false;
			}

			var dic = new Dictionary<int, short>
			{
				[b1] = grid.GetCandidates(b1),
				[b2] = grid.GetCandidates(b2)
			};
			for (int i = 1; i <= 2; i++)
			{
				for (int j = 1; j <= 2; j++)
				{
					var (pos1, pos2) = j == 1 ? (b1, b2) : (b2, b1);
					short ck = (short)(grid.GetCandidates(pos1) & playground[i]);
					if (ck != 0 && !ck.IsPowerOfTwo()
						|| (grid.GetCandidates(pos1) & ~(short)(ck | playground[i == 1 ? 2 : 1])) != 0)
					{
						continue;
					}

					short candidateMask = ck != 0 ? ck : playground[i];
					candidateMask &= grid.GetCandidates(pos2);
					if (candidateMask == 0)
					{
						continue;
					}

					foreach (int digit in candidateMask)
					{
						bibiElims.Add(new(ConclusionType.Elimination, pos2, digit));
						dic[pos2] &= (short)~(1 << digit);
					}
				}
			}

			// Now check all base digits last.
			short last = (short)(dic[b1] | dic[b2]);
			foreach (int digit in SudokuGrid.MaxCandidatesMask & ~last & ~lockedQ)
			{
				if (grid.Exists(tq1, digit) is true)
				{
					bibiElims.Add(new(ConclusionType.Elimination, tq1, digit));
				}
				if (grid.Exists(tq2, digit) is true)
				{
					bibiElims.Add(new(ConclusionType.Elimination, tq2, digit));
				}
			}
			foreach (int digit in SudokuGrid.MaxCandidatesMask & ~last & ~lockedR)
			{
				if (grid.Exists(tr1, digit) is true)
				{
					bibiElims.Add(new(ConclusionType.Elimination, tr1, digit));
				}
				if (grid.Exists(tr2, digit) is true)
				{
					bibiElims.Add(new(ConclusionType.Elimination, tr2, digit));
				}
			}

			// Then check target pairs if worth.
			if (PopCount((uint)last) == 2)
			{
				var elimMap = (targetMap & EmptyMap).PeerIntersection;
				if (elimMap.IsEmpty)
				{
					// Exit the method.
					return true;
				}

				var digits = last.GetAllSets();
				foreach (int digit in digits)
				{
					foreach (int cell in elimMap & CandMaps[digit])
					{
						targetPairElims.Add(new(ConclusionType.Elimination, cell, digit));
					}
				}
				elimMap = new Cells { b1, b2 }.PeerIntersection;
				if (elimMap.IsEmpty)
				{
					return true;
				}

				foreach (int digit in digits)
				{
					foreach (int cell in elimMap & CandMaps[digit])
					{
						targetPairElims.Add(new(ConclusionType.Elimination, cell, digit));
					}
				}

				// Then check swordfish pattern.
				foreach (int digit in digits)
				{
					short mask = isRow ? crossline.RowMask : crossline.ColumnMask;
					foreach (int offset in mask)
					{
						int region = offset + (isRow ? 9 : 18);
						if (!(crossline & RegionMaps[region] & CandMaps[digit]).IsEmpty)
						{
							foreach (int cell in (RegionMaps[region] & CandMaps[digit]) - crossline)
							{
								swordfishElims.Add(new(ConclusionType.Elimination, cell, digit));
							}
						}
					}
				}
			}

			return true;
		}
	}
}

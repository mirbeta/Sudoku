﻿namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
internal sealed unsafe partial class SueDeCoq3DimensionStepSearcher : ISueDeCoq3DimensionStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		using scoped ValueList<CellMap> rbList = new(3), cbList = new(3);
		foreach (var pivot in EmptyCells)
		{
			var r = pivot.ToHouseIndex(HouseType.Row);
			var c = pivot.ToHouseIndex(HouseType.Column);
			var b = pivot.ToHouseIndex(HouseType.Block);
			var rbMap = HousesMap[r] & HousesMap[b];
			var cbMap = HousesMap[c] & HousesMap[b];
			var rbEmptyMap = rbMap & EmptyCells;
			var cbEmptyMap = cbMap & EmptyCells;
			if (rbEmptyMap.Count < 2 || cbEmptyMap.Count < 2)
			{
				// The intersection needs at least two cells.
				continue;
			}

			reinitializeList(&rbList, &rbEmptyMap);
			reinitializeList(&cbList, &cbEmptyMap);

			foreach (var rbCurrentMap in rbList)
			{
				var rbSelectedInterMask = grid.GetDigitsUnion(rbCurrentMap);
				if (PopCount((uint)rbSelectedInterMask) <= rbCurrentMap.Count + 1)
				{
					continue;
				}

				foreach (var cbCurrentMap in cbList)
				{
					var cbSelectedInterMask = grid.GetDigitsUnion(cbCurrentMap);
					if (PopCount((uint)cbSelectedInterMask) <= cbCurrentMap.Count + 1)
					{
						continue;
					}

					if ((cbCurrentMap & rbCurrentMap).Count != 1)
					{
						continue;
					}

					// Get all maps to use later.
					var blockMap = HousesMap[b] - rbCurrentMap - cbCurrentMap & EmptyCells;
					var rowMap = HousesMap[r] - HousesMap[b] & EmptyCells;
					var columnMap = HousesMap[c] - HousesMap[b] & EmptyCells;

					// Iterate on the number of the cells that should be selected in block.
					for (int i = 1, count = blockMap.Count; i < count; i++)
					{
						foreach (var selectedBlockCells in blockMap & i)
						{
							var blockMask = grid.GetDigitsUnion(selectedBlockCells);
							var elimMapBlock = CellMap.Empty;

							// Get the elimination map in the block.
							foreach (var digit in blockMask)
							{
								elimMapBlock |= CandidatesMap[digit];
							}
							elimMapBlock &= blockMap - selectedBlockCells;

							for (int j = 1, limit = MathExtensions.Min(9 - i - selectedBlockCells.Count, rowMap.Count, columnMap.Count); j < limit; j++)
							{
								foreach (var selectedRowCells in rowMap & j)
								{
									var rowMask = grid.GetDigitsUnion(selectedRowCells);
									var elimMapRow = CellMap.Empty;

									foreach (var digit in rowMask)
									{
										elimMapRow |= CandidatesMap[digit];
									}
									elimMapRow &= HousesMap[r] - rbCurrentMap - selectedRowCells;

									for (var k = 1; k <= MathExtensions.Min(9 - i - j - selectedBlockCells.Count - selectedRowCells.Count, rowMap.Count, columnMap.Count); k++)
									{
										foreach (var selectedColumnCells in columnMap & k)
										{
											var columnMask = grid.GetDigitsUnion(selectedColumnCells);
											var elimMapColumn = CellMap.Empty;

											foreach (var digit in columnMask)
											{
												elimMapColumn |= CandidatesMap[digit];
											}
											elimMapColumn &= HousesMap[c] - cbCurrentMap - selectedColumnCells;

											if ((blockMask & rowMask) != 0
												&& (rowMask & columnMask) != 0
												&& (columnMask & blockMask) != 0)
											{
												continue;
											}

											var fullMap = rbCurrentMap | cbCurrentMap | selectedRowCells | selectedColumnCells | selectedBlockCells;
											var otherMap_row = fullMap - (selectedRowCells | rbCurrentMap);
											var otherMap_column = fullMap - (selectedColumnCells | cbCurrentMap);
											var mask = grid.GetDigitsUnion(otherMap_row);
											if ((mask & rowMask) != 0)
											{
												// At least one digit spanned two houses.
												continue;
											}

											mask = grid.GetDigitsUnion(otherMap_column);
											if ((mask & columnMask) != 0)
											{
												continue;
											}

											mask = (short)((short)(blockMask | rowMask) | columnMask);
											var rbMaskOnlyInInter = (short)(rbSelectedInterMask & ~mask);
											var cbMaskOnlyInInter = (short)(cbSelectedInterMask & ~mask);

											var bCount = PopCount((uint)blockMask);
											var rCount = PopCount((uint)rowMask);
											var cCount = PopCount((uint)columnMask);
											var rbCount = PopCount((uint)rbMaskOnlyInInter);
											var cbCount = PopCount((uint)cbMaskOnlyInInter);
											if (cbCurrentMap.Count + rbCurrentMap.Count + i + j + k - 1 == bCount + rCount + cCount + rbCount + cbCount
												&& (elimMapRow | elimMapColumn | elimMapBlock) is not [])
											{
												// Check eliminations.
												var conclusions = new List<Conclusion>();
												foreach (var digit in blockMask)
												{
													foreach (var cell in elimMapBlock & CandidatesMap[digit])
													{
														conclusions.Add(new(Elimination, cell, digit));
													}
												}
												foreach (var digit in rowMask)
												{
													foreach (var cell in elimMapRow & CandidatesMap[digit])
													{
														conclusions.Add(new(Elimination, cell, digit));
													}
												}
												foreach (var digit in columnMask)
												{
													foreach (var cell in elimMapColumn & CandidatesMap[digit])
													{
														conclusions.Add(new(Elimination, cell, digit));
													}
												}
												if (conclusions.Count == 0)
												{
													continue;
												}

												var candidateOffsets = new List<CandidateViewNode>();
												foreach (var digit in rowMask)
												{
													foreach (var cell in (selectedRowCells | rbCurrentMap) & CandidatesMap[digit])
													{
														candidateOffsets.Add(
															new(DisplayColorKind.Normal, cell * 9 + digit)
														);
													}
												}
												foreach (var digit in columnMask)
												{
													foreach (var cell in (selectedColumnCells | cbCurrentMap) & CandidatesMap[digit])
													{
														candidateOffsets.Add(
															new(DisplayColorKind.Auxiliary1, cell * 9 + digit)
														);
													}
												}
												foreach (var digit in blockMask)
												{
													foreach (var cell in (selectedBlockCells | rbCurrentMap | cbCurrentMap) & CandidatesMap[digit])
													{
														candidateOffsets.Add(
															new(DisplayColorKind.Auxiliary2, cell * 9 + digit)
														);
													}
												}

												var step = new SueDeCoq3DimensionStep(
													ImmutableArray.CreateRange(conclusions),
													ImmutableArray.Create(
														View.Empty
															| candidateOffsets
															| new HouseViewNode[]
															{
																new(DisplayColorKind.Normal, r),
																new(DisplayColorKind.Auxiliary2, c),
																new(DisplayColorKind.Auxiliary3, b)
															}
													),
													rowMask,
													columnMask,
													blockMask,
													selectedRowCells | rbCurrentMap,
													selectedColumnCells | cbCurrentMap,
													selectedBlockCells | rbCurrentMap | cbCurrentMap
												);
												if (context.OnlyFindOne)
												{
													return step;
												}

												context.Accumulator.Add(step);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void reinitializeList(ValueList<CellMap>* list, CellMap* emptyMap)
		{
			list->Clear();
			switch (*emptyMap)
			{
				case [_, _]:
				{
					list->Add(*emptyMap);

					break;
				}
				case [var i, var j, var k]:
				{
					list->Add(CellsMap[i] + j);
					list->Add(CellsMap[i] + k);
					list->Add(CellsMap[j] + k);

					break;
				}
			}
		}
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.DocComments;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Exocets.Eliminations
{
	/// <summary>
	/// Indicates the swordfish pattern eliminations.
	/// </summary>
	public struct SwordfishEliminations : IEnumerable<Conclusion>
	{
		/// <inheritdoc cref="ExocetElimination(IList{ExocetElimination.Conclusion})"/>
		public SwordfishEliminations(IList<Conclusion> conclusions) => Conclusions = conclusions;


		/// <inheritdoc cref="ExocetElimination.Count"/>
		public readonly int Count => Conclusions?.Count ?? 0;

		/// <inheritdoc cref="ExocetElimination.Conclusions"/>
		public IList<Conclusion>? Conclusions { readonly get; private set; }


		/// <inheritdoc cref="ExocetElimination.Add(in ExocetElimination.Conclusion)"/>
		public void Add(in Conclusion conclusion) =>
			(Conclusions ??= new List<Conclusion>()).AddIfDoesNotContain(conclusion);

		/// <inheritdoc cref="ExocetElimination.AddRange(IEnumerable{ExocetElimination.Conclusion})"/>
		public void AddRange(IEnumerable<Conclusion> conclusions) =>
			(Conclusions ??= new List<Conclusion>()).AddRange(conclusions, true);

		/// <inheritdoc cref="ExocetElimination.Merge(ExocetElimination?[])"/>
		public readonly SwordfishEliminations Merge(params SwordfishEliminations?[] eliminations)
		{
			var result = new SwordfishEliminations();
			foreach (var instance in eliminations)
			{
				if (instance is null)
				{
					continue;
				}

				result.AddRange(instance);
			}

			return result;
		}

		/// <inheritdoc/>
		public readonly IEnumerator<Conclusion> GetEnumerator() => Conclusions.NullableCollection().GetEnumerator();


		/// <inheritdoc cref="object.ToString"/>
		public override readonly string? ToString() =>
			Conclusions is null
			? null
			: $"  * Swordfish eliminations: {new ConclusionCollection(Conclusions).ToString()}";

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <inheritdoc cref="ExocetElimination.MergeAll(IEnumerable{ExocetElimination})"/>
		public static BibiPatternEliminations MergeAll(IEnumerable<BibiPatternEliminations> list)
		{
			var result = new BibiPatternEliminations();
			foreach (var z in list)
			{
				if (z.Conclusions is null)
				{
					continue;
				}

				result.AddRange(z);
			}

			return result;
		}

		/// <inheritdoc cref="ExocetElimination.MergeAll(ExocetElimination[])"/>
		public static BibiPatternEliminations MergeAll(params BibiPatternEliminations[] list) =>
			MergeAll(list.AsEnumerable());
	}
}

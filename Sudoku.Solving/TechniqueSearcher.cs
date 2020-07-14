﻿#nullable disable warnings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates a step finder that used in solving in
	/// <see cref="ManualSolver"/>.
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public abstract class TechniqueSearcher : IComparable<TechniqueSearcher>, IEquatable<TechniqueSearcher>
	{
		/// <summary>
		/// Get the searcher properties of type <see cref="SearcherPropertyAttribute"/>.
		/// </summary>
		/// <seealso cref="SearcherPropertyAttribute"/>
		public SearcherPropertyAttribute? SearcherProperties
		{
			get
			{
				var type = GetType();
				return type.IsAbstract ? null : type.GetCustomAttribute<SearcherPropertyAttribute>();
			}
		}


		/// <summary>
		/// The empty cells map.
		/// </summary>
		/// <include file='SolvingDocComments.xml' path='comments/property[@name="IMaps"]'/>
		/// <seealso cref="InitializeMaps(IReadOnlyGrid)"/>
		internal static GridMap EmptyMap { get; set; }

		/// <summary>
		/// The bi-value cells map.
		/// </summary>
		/// <include file='SolvingDocComments.xml' path='comments/property[@name="IMaps"]'/>
		/// <seealso cref="InitializeMaps(IReadOnlyGrid)"/>
		internal static GridMap BivalueMap { get; set; }

		/// <summary>
		/// The candidate maps.
		/// </summary>
		/// <include file='SolvingDocComments.xml' path='comments/property[@name="IMaps"]'/>
		/// <seealso cref="InitializeMaps(IReadOnlyGrid)"/>
		internal static GridMap[] CandMaps { get; set; }

		/// <summary>
		/// The digit maps.
		/// </summary>
		/// <include file='SolvingDocComments.xml' path='comments/property[@name="IMaps"]'/>
		/// <seealso cref="InitializeMaps(IReadOnlyGrid)"/>
		internal static GridMap[] DigitMaps { get; set; }

		/// <summary>
		/// The value maps.
		/// </summary>
		/// <include file='SolvingDocComments.xml' path='comments/property[@name="IMaps"]'/>
		/// <seealso cref="InitializeMaps(IReadOnlyGrid)"/>
		internal static GridMap[] ValueMaps { get; set; }


		/// <summary>
		/// Take a technique step after searched all solving steps.
		/// </summary>
		/// <param name="grid">The grid to search steps.</param>
		/// <returns>A technique information.</returns>
		public TechniqueInfo? GetOne(IReadOnlyGrid grid)
		{
			var bag = new List<TechniqueInfo>();
			GetAll(bag, grid);
			return bag.FirstOrDefault();
		}

		/// <summary>
		/// Accumulate all technique information instances into the specified accumulator.
		/// </summary>
		/// <param name="accumulator">The accumulator to store technique information.</param>
		/// <param name="grid">The grid to search for techniques.</param>
		public abstract void GetAll(IList<TechniqueInfo> accumulator, IReadOnlyGrid grid);

		/// <inheritdoc/>
		public virtual int CompareTo(TechniqueSearcher other) => GetPriority(this).CompareTo(GetPriority(other));

		/// <inheritdoc/>
		public sealed override int GetHashCode() => GetPriority(this) * 17 + 0xDEAD & 0xC0DE;

		/// <inheritdoc/>
		public virtual bool Equals(TechniqueSearcher other) => GetPriority(this) == GetPriority(other);

		/// <inheritdoc/>
		public sealed override bool Equals(object? obj) => obj is TechniqueSearcher comparer && Equals(comparer);

		/// <inheritdoc/>
		public override string ToString() => GetType().Name;


		/// <summary>
		/// Initialize the maps that used later.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public static void InitializeMaps(IReadOnlyGrid grid) =>
			(EmptyMap, BivalueMap, CandMaps, DigitMaps, ValueMaps) = grid;

		/// <summary>
		/// To get the priority of the technique searcher.
		/// </summary>
		/// <param name="instance">The technique searcher.</param>
		/// <returns>The priority.</returns>
		/// <remarks>
		/// This method uses reflection to get the specified value.
		/// </remarks>
		private static int GetPriority(TechniqueSearcher instance) =>
			(int)instance.GetType().GetProperty("Priority", BindingFlags.Static)!.GetValue(null)!;


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(TechniqueSearcher left, TechniqueSearcher right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(TechniqueSearcher left, TechniqueSearcher right) => !(left == right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThan"]'/>
		public static bool operator >(TechniqueSearcher left, TechniqueSearcher right) => left.CompareTo(right) > 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_GreaterThanOrEqual"]'/>
		public static bool operator >=(TechniqueSearcher left, TechniqueSearcher right) => left.CompareTo(right) >= 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_LessThan"]'/>
		public static bool operator <(TechniqueSearcher left, TechniqueSearcher right) => left.CompareTo(right) < 0;

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_LessThanOrEqual"]'/>
		public static bool operator <=(TechniqueSearcher left, TechniqueSearcher right) => left.CompareTo(right) <= 0;
	}
}

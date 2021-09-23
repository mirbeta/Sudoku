﻿namespace Sudoku.Data;

/// <summary>
/// Defines a chain node, with basic information about the node. At the same time you can get the root node
/// of the chain, using the current node as the tail node.
/// </summary>
/// <remarks>
/// The data structure uses an <see cref="int"/> value to represent an instance. The bit usage details is as below:
/// <code>
/// |  (4)  |  (3)  |  (2)  |  (1)  |
/// |-------|-------|-------|-------|
/// 32     24      16       8       0
/// </code>
/// Where:
/// <list type="table">
/// <item>
/// <term>Part <c>(1)</c>, bits 0..8</term>
/// <description>The mask of the digit used. The value only uses 4 bits.</description>
/// </item>
/// <item>
/// <term>Part <c>(2)</c>, bits 8..16</term>
/// <description>The mask of the cell used. The value only uses 7 bits.</description>
/// </item>
/// <item>
/// <term>Part <c>(3)</c>, bits 16..24</term>
/// <description>
/// The mask of the on/off status indicating whether the node is currently on. The value only uses 1 bit.
/// </description>
/// </item>
/// <item>
/// <term>Part <c>(4)</c>, bits 24..32</term>
/// <description>The mask of the number of parent nodes stored. The value only uses 3 bits.</description>
/// </item>
/// </list>
/// </remarks>
/// <param name="Mask">Indicates the mask that handles and stores the basic information of the current node.</param>
[AutoEquality(nameof(Mask))]
[AutoDeconstruct(nameof(Candidate), nameof(IsOn))]
[AutoDeconstruct(nameof(Cell), nameof(Digit), nameof(IsOn))]
public unsafe partial record struct ChainNode(int Mask) : IValueEquatable<ChainNode>
{
	/// <summary>
	/// Indicates the undefined instance that is used for providing with a value that only used in an invalid case.
	/// </summary>
	public static readonly ChainNode Undefined;


	/// <summary>
	/// Indicates the parents.
	/// </summary>
	private ChainNode[]? _rawParents = null;


	/// <summary>
	/// Initializes a <see cref="ChainNode"/> instance using the specified cell, digit and the status
	/// value as a <see cref="bool"/> value.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="isOn">A <see cref="bool"/> result indicating whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(byte cell, byte digit, bool isOn) : this(ConstructMask(cell, digit, isOn))
	{
	}

	/// <summary>
	/// Initializes a <see cref="ChainNode"/> instance using the specified cell, digit,
	/// the status value as a <see cref="bool"/> value, and a parent node.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="isOn">A <see cref="bool"/> result indicating whether the node is on.</param>
	/// <param name="parent">The parent node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(byte cell, byte digit, bool isOn, in ChainNode parent) : this(ConstructMask(cell, digit, isOn, 1))
	{
		_rawParents = new ChainNode[7];
		_rawParents[0] = parent;
	}

	/// <summary>
	/// Initializes a <see cref="ChainNode"/> instance using the specified cell, digit,
	/// the status value as a <see cref="bool"/> value, and the parent nodes.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="isOn">A <see cref="bool"/> result indicating whether the node is on.</param>
	/// <param name="parents">The parent nodes.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ChainNode(byte cell, byte digit, bool isOn, ChainNode[] parents)
	: this(ConstructMask(cell, digit, isOn, (byte)parents.Length)) =>
		_rawParents = parents;


	/// <summary>
	/// Gets the possible useful parents.
	/// </summary>
	public readonly ChainNode[]? Parents
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _rawParents?[..ParentsCount];
	}

	/// <summary>
	/// Indicates whether the node is at the <b>on</b> status.
	/// </summary>
	/// <remarks>
	/// The possible <b>status</b>es of the node are <b>on</b> and <b>off</b>, where:
	/// <list type="table">
	/// <listheader>
	/// <term>Status</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term>On</term>
	/// <description>
	/// The digit is true in the cell, which means the digit should be filled in this cell.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Off</term>
	/// <description>
	/// The digit is false in the cell, which means the digit should be eliminated from this cell.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	public readonly bool IsOn
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Mask >> 16 & 1) != 0;
	}

	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public readonly byte Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (byte)(Mask >> 8 & 255);
	}

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public readonly byte Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (byte)(Mask & 255);
	}

	/// <summary>
	/// Indicates the total number of parent nodes.
	/// </summary>
	/// <!--<value>The number of cells to assign.</value>-->
	/// <!--<returns>The number of parents.</returns>-->
	public byte ParentsCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => (byte)(Mask >> 24 & 7);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private set => Mask = Mask & ((1 << 24) - 1) | value << 24;
	}

	/// <summary>
	/// Indicates the total number of the ancestors.
	/// </summary>
	public readonly int AncestorsCount
	{
		get
		{
			var ancestors = new List<ChainNode>();
			for (List<ChainNode> todo = new() { this }, next; todo.Count != 0; todo = next)
			{
				next = new();
				foreach (var p in todo)
				{
					bool alreadyContains = false;
					foreach (var ancestor in ancestors)
					{
						if (ancestor == p)
						{
							alreadyContains = true;
							break;
						}
					}

					if (!alreadyContains)
					{
						ancestors.Add(p);
						for (int i = 0, count = p.ParentsCount; i < count; i++)
						{
							next.Add(p._rawParents![i]);
						}
					}
				}
			}

			return ancestors.Count;
		}
	}

	/// <summary>
	/// Indicates the candidate used.
	/// </summary>
	public readonly short Candidate
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (short)(Mask & 1023);
	}

	/// <summary>
	/// <para>Indicates the root of the current chain.</para>
	/// <para>
	/// If the property <see cref="Parents"/> isn't <see langword="null"/>,
	/// then the property will search for the parent node and check its parent node recursively
	/// (i.e. checks the grandparents, great-grandparents, etc.).
	/// </para>
	/// </summary>
	/// <returns>Returns the root of the chain.</returns>
	/// <seealso cref="Parents"/>
	/// <seealso cref="ParentsCount"/>
	public readonly ChainNode Root
	{
		get
		{
			if (ParentsCount == 0)
			{
				return Undefined;
			}

			var result = _rawParents![0];
			while (result._rawParents is { } p)
			{
				result = p[0];
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the nodes that the current node lies in.
	/// </summary>
	public readonly IReadOnlyList<ChainNode> WholeChain
	{
		get
		{
			List<ChainNode> todo = new() { this }, tempList = new(), done = new(), next = new();
			while (todo.Count != 0)
			{
				next.Clear();

				foreach (var p in todo)
				{
					bool contains = false;
					foreach (var node in done)
					{
						if (node == p)
						{
							contains = true;
							break;
						}
					}

					if (!contains)
					{
						done.Add(p);
						tempList.Add(p);
						for (int i = 0, count = p.ParentsCount; i < count; i++)
						{
							next.Add(p._rawParents![i]);
						}
					}
				}

				todo = next;
			}

			return tempList;
		}
	}


	/// <summary>
	/// Determine whether the node is the parent of the specified node.
	/// </summary>
	/// <param name="node">The chain node.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool IsParentOf(ChainNode node)
	{
		var temp = node;
		while (temp.ParentsCount != 0)
		{
			temp = temp._rawParents![0];
			if (temp == this)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => Mask;

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		if (ParentsCount == 0)
		{
			return $"Candidate: {new Cells { Cell }}({Digit + 1})";
		}
		else
		{
			var nodes = Candidates.Empty;
			for (int i = 0; i < ParentsCount; i++)
			{
				var parent = _rawParents![i];
				nodes.AddAnyway(parent.Candidate);
			}

			return $"Candidate: {new Cells { Cell }}({Digit + 1}), Parent(s): {nodes}";
		}
	}

	/// <summary>
	/// Append a chain node into the collection, as one of the parent nodes.
	/// </summary>
	/// <param name="chain">The chain node to be added.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws when the inner parent nodes collection is full.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddParent(ChainNode chain)
	{
		switch (ParentsCount)
		{
			case 0:
			{
				_rawParents = new ChainNode[7];
				ParentsCount = 1;

				break;
			}
			case >= 7:
			{
				throw new InvalidOperationException(
					"Can't append any elements into the collection due to the list being full."
				);
			}
			default:
			{
				_rawParents![ParentsCount++] = chain;

				break;
			}
		}
	}

	/// <summary>
	/// Constructs the mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="isOn">The on/off status.</param>
	/// <param name="parentsCount">The number of parents.</param>
	/// <returns>The mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ConstructMask(byte cell, byte digit, bool isOn, byte parentsCount = 0) =>
		parentsCount << 24 | (isOn ? 1 : 0) << 16 | cell << 8 | digit;
}
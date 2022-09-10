﻿#pragma warning disable IDE0011, IDE0032

namespace Sudoku.Concepts;

/// <summary>
/// Encapsulates a binary series of cell status table.
/// </summary>
/// <remarks>
/// This type holds a <see langword="static readonly"/> field called <see cref="Empty"/>,
/// it is the only field provided to be used as the entry to create or update collection.
/// If you want to add elements into it, you can use <see cref="Add(int)"/>, <see cref="AddRange(IEnumerable{int})"/>
/// or just <see cref="operator +(in CellMap, int)"/> or <see cref="operator +(in CellMap, IEnumerable{int})"/>:
/// <code><![CDATA[
/// var cellMap = CellMap.Empty;
/// cellMap += 0; // Adds 'r1c1' into the collection.
/// cellMap.Add(0); // Adds 'r1c2' into the collection.
/// cellMap.AddRange(stackalloc[] { 2, 3, 4 }); // Adds 'r1c345' into the collection.
/// cellMap |= anotherCellMap; // Adds a list of another instance of type 'CellMap' into the current collection.
/// ]]></code>
/// If you want to learn more information about this type, please visit
/// <see href="https://sunnieshine.github.io/Sudoku/data-structures/cells">this wiki page</see>.
/// </remarks>
[JsonConverter(typeof(CellMapJsonConverter))]
public struct CellMap :
	IAdditionOperators<CellMap, int, CellMap>,
	IAdditiveIdentity<CellMap, CellMap>,
	IBitwiseOperators<CellMap, CellMap, CellMap>,
	IBooleanOperators<CellMap>,
	ICollection<int>,
	IDivisionOperators<CellMap, int, short>,
	IEnumerable,
	IEnumerable<int>,
	IEquatable<CellMap>,
	IEqualityOperators<CellMap, CellMap, bool>,
	ILogicalNotOperators<CellMap, bool>,
	ILogicalOperators<CellMap, CellMap, CellMap>,
	IMinMaxValue<CellMap>,
	IModulusOperators<CellMap, CellMap, CellMap>,
	IMultiplyOperators<CellMap, int, Candidates>,
	IReadOnlyCollection<int>,
	IReadOnlyList<int>,
	IReadOnlySet<int>,
	ISet<int>,
	ISimpleFormattable,
	ISimpleParseable<CellMap>,
	ISubtractionOperators<CellMap, int, CellMap>,
	ISubtractionOperators<CellMap, CellMap, CellMap>,
	IUnaryPlusOperators<CellMap, CellMap>
{
	/// <summary>
	/// The value used for shifting.
	/// </summary>
	private const int Shifting = 41;


	/// <summary>
	/// Indicates the empty instance.
	/// </summary>
	public static readonly CellMap Empty;

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MaxValue"/>
	public static readonly CellMap MaxValue = ~default(CellMap);

	/// <inheritdoc cref="IMinMaxValue{TSelf}.MinValue"/>
	/// <remarks>
	/// This value is equivalent to <see cref="Empty"/>.
	/// </remarks>
	public static readonly CellMap MinValue;


	/// <summary>
	/// The background field of the property <see cref="Count"/>.
	/// </summary>
	/// <remarks><b><i>This field is explicitly declared on purpose. Please don't use auto property.</i></b></remarks>
	/// <seealso cref="Count"/>
	private int _count;

	/// <summary>
	/// Indicates the internal two <see cref="long"/> values,
	/// which represents 81 bits. <see cref="_high"/> represent the higher
	/// 40 bits and <see cref="_low"/> represents the lower 41 bits, where each bit is:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/> bit (1)</term>
	/// <description>The corresponding cell is contained in this collection</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/> bit (0)</term>
	/// <description>The corresponding cell is not contained in this collection</description>
	/// </item>
	/// </list>
	/// </summary>
	private long _high, _low;


	/// <summary>
	/// Throws a <see cref="NotSupportedException"/>-typed instance.
	/// </summary>
	/// <exception cref="NotSupportedException">The exception will always be thrown.</exception>
	/// <remarks>
	/// The main idea of the parameterless constructor is to create a new instance
	/// without any extra information, but the current type is special:
	/// I want you to use <see cref="Empty"/> instead of the syntax <c>new CellMap()</c>,
	/// in order to get a better experience on performance.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete($"Please use the member '{nameof(Empty)}' instead.", true)]
	public CellMap() => throw new NotSupportedException();


	/// <summary>
	/// Same as the method <see cref="AllSetsAreInOneHouse(out int)"/>, but this property doesn't contain
	/// the <see langword="out"/> argument, as the optimization.
	/// </summary>
	/// <seealso cref="AllSetsAreInOneHouse(out int)"/>
	public readonly bool InOneHouse
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
#pragma warning disable IDE0055
			if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) return true;
			if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) return true;
			if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) return true;
			if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) return true;
			if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) return true;
			if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) return true;
			if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) return true;
			if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) return true;
			if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) return true;
			if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) return true;
			if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) return true;
			if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) return true;
			if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) return true;
			if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) return true;
			if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) return true;
			if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) return true;
			if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) return true;
#pragma warning restore IDE0055

			return false;
		}
	}

	/// <summary>
	/// Determines whether the current list of cells are all lie in an intersection area,
	/// i.e. a locked candidates.
	/// </summary>
	public bool IsInIntersection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _count <= 3 && PopCount((uint)CoveredHouses) == 2;
	}

	/// <summary>
	/// Indicates the mask of block that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned blocks are 0 and 3, so the return
	/// mask is <c>0b000001001</c> (i.e. 9).
	/// </remarks>
	public readonly short BlockMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (this & HousesMap[0]) result |= 1;
			if (this & HousesMap[1]) result |= 2;
			if (this & HousesMap[2]) result |= 4;
			if (this & HousesMap[3]) result |= 8;
			if (this & HousesMap[4]) result |= 16;
			if (this & HousesMap[5]) result |= 32;
			if (this & HousesMap[6]) result |= 64;
			if (this & HousesMap[7]) result |= 128;
			if (this & HousesMap[8]) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of row that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned rows are 0 and 3, so the return
	/// mask is <c>0b000001001</c> (i.e. 9).
	/// </remarks>
	public readonly short RowMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (this & HousesMap[9]) result |= 1;
			if (this & HousesMap[10]) result |= 2;
			if (this & HousesMap[11]) result |= 4;
			if (this & HousesMap[12]) result |= 8;
			if (this & HousesMap[13]) result |= 16;
			if (this & HousesMap[14]) result |= 32;
			if (this & HousesMap[15]) result |= 64;
			if (this & HousesMap[16]) result |= 128;
			if (this & HousesMap[17]) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the mask of column that all cells in this collection spanned.
	/// </summary>
	/// <remarks>
	/// For example, if the cells are <c>{ 0, 1, 27, 28 }</c>, all spanned columns are 0 and 1, so the return
	/// mask is <c>0b000000011</c> (i.e. 3).
	/// </remarks>
	public readonly short ColumnMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			short result = 0;
			if (this & HousesMap[18]) result |= 1;
			if (this & HousesMap[19]) result |= 2;
			if (this & HousesMap[20]) result |= 4;
			if (this & HousesMap[21]) result |= 8;
			if (this & HousesMap[22]) result |= 16;
			if (this & HousesMap[23]) result |= 32;
			if (this & HousesMap[24]) result |= 64;
			if (this & HousesMap[25]) result |= 128;
			if (this & HousesMap[26]) result |= 256;

			return result;
		}
	}

	/// <summary>
	/// Indicates the covered line.
	/// </summary>
	/// <remarks>
	/// If the covered house can't be found, it'll return <see cref="InvalidValidOfTrailingZeroCountMethodFallback"/>.
	/// </remarks>
	/// <seealso cref="InvalidValidOfTrailingZeroCountMethodFallback"/>
	public readonly int CoveredLine
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TrailingZeroCount(CoveredHouses & ~511);
	}

	/// <summary>
	/// Indicates the number of the values stored in this collection.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _count;
	}

	/// <summary>
	/// Indicates all houses covered. This property is used to check all houses that all cells
	/// of this instance covered. For example, if the cells are <c>{ 0, 1 }</c>, the property
	/// <see cref="CoveredHouses"/> will return the house index 0 (block 1) and 9 (row 1);
	/// however, if cells spanned two houses or more (e.g. cells <c>{ 0, 1, 27 }</c>),
	/// this property won't contain any houses.
	/// </summary>
	/// <remarks>
	/// The return value will be an <see cref="int"/> value indicating each houses.
	/// Bits set 1 are covered houses.
	/// </remarks>
	public readonly int CoveredHouses
	{
		get
		{
			int z = 0;

#pragma warning disable IDE0055
			if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) z |=       0x1;
			if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) z |=       0x2;
			if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) z |=       0x4;
			if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) z |=       0x8;
			if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) z |=      0x10;
			if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) z |=      0x20;
			if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) z |=      0x40;
			if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) z |=      0x80;
			if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) z |=     0x100;
			if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) z |=     0x200;
			if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) z |=     0x400;
			if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) z |=     0x800;
			if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) z |=    0x1000;
			if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) z |=    0x2000;
			if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) z |=    0x4000;
			if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) z |=    0x8000;
			if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) z |=   0x10000;
			if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) z |=   0x20000;
			if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) z |=   0x40000;
			if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) z |=   0x80000;
			if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) z |=  0x100000;
			if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) z |=  0x200000;
			if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) z |=  0x400000;
			if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) z |=  0x800000;
			if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) z |= 0x1000000;
			if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) z |= 0x2000000;
			if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) z |= 0x4000000;
#pragma warning restore IDE0055

			return z;
		}
	}

	/// <summary>
	/// All houses that the map spanned. This property is used to check all houses that all cells of
	/// this instance spanned. For example, if the cells are <c>{ 0, 1 }</c>, the property
	/// <see cref="Houses"/> will return the house index 0 (block 1), 9 (row 1), 18 (column 1)
	/// and 19 (column 2).
	/// </summary>
	public readonly int Houses
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)BlockMask | RowMask << 9 | ColumnMask << 18;
	}

	/// <summary>
	/// Indicates the cell offsets in this collection.
	/// </summary>
	private readonly int[] Offsets
	{
		get
		{
			if (!this)
			{
				return Array.Empty<int>();
			}

			long value;
			int i, pos = 0;
			int[] arr = new int[_count];
			if (_low != 0)
			{
				for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						arr[pos++] = i;
					}
				}
			}
			if (_high != 0)
			{
				for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
				{
					if ((value & 1) != 0)
					{
						arr[pos++] = i;
					}
				}
			}

			return arr;
		}
	}

	/// <inheritdoc/>
	readonly bool ICollection<int>.IsReadOnly => false;

	/// <inheritdoc/>
	static CellMap IAdditiveIdentity<CellMap, CellMap>.AdditiveIdentity => Empty;

	/// <inheritdoc/>
	static CellMap IMinMaxValue<CellMap>.MinValue => MinValue;

	/// <inheritdoc/>
	static CellMap IMinMaxValue<CellMap>.MaxValue => MaxValue;


	/// <summary>
	/// Get the offset at the specified position index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>
	/// The offset at the specified position index. If the value is invalid, the return value will be <c>-1</c>.
	/// </returns>
	public readonly int this[int index]
	{
		get
		{
			if (!this)
			{
				return -1;
			}

			long value;
			int i, pos = -1;
			if (_low != 0)
			{
				for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
				{
					if ((value & 1) != 0 && ++pos == index)
					{
						return i;
					}
				}
			}
			if (_high != 0)
			{
				for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
				{
					if ((value & 1) != 0 && ++pos == index)
					{
						return i;
					}
				}
			}

			return -1;
		}
	}


	/// <summary>
	/// Copies the current instance to the target array specified as an <see cref="int"/>*.
	/// </summary>
	/// <param name="arr">The pointer that points to an array of type <see cref="int"/>.</param>
	/// <param name="length">The length of that array.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="arr"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the capacity isn't enough to store all values.
	/// </exception>
	public readonly unsafe void CopyTo(int* arr, int length)
	{
		ArgumentNullException.ThrowIfNull(arr);

		if (!this)
		{
			return;
		}

		Argument.ThrowIfInvalid(_count <= length, "The capacity is not enough.");

		long value;
		int i, pos = 0;
		if (_low != 0)
		{
			for (value = _low, i = 0; i < Shifting; i++, value >>= 1)
			{
				if ((value & 1) != 0)
				{
					arr[pos++] = i;
				}
			}
		}
		if (_high != 0)
		{
			for (value = _high, i = Shifting; i < 81; i++, value >>= 1)
			{
				if ((value & 1) != 0)
				{
					arr[pos++] = i;
				}
			}
		}
	}

	/// <summary>
	/// Copies the current instance to the target <see cref="Span{T}"/> instance.
	/// </summary>
	/// <param name="span">
	/// The target <see cref="Span{T}"/> instance.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly unsafe void CopyTo(scoped ref Span<int> span)
	{
		fixed (int* arr = span)
		{
			CopyTo(arr, span.Length);
		}
	}

	/// <summary>
	/// Indicates whether all cells in this instance are in one house.
	/// </summary>
	/// <param name="houseIndex">
	/// The house index whose corresponding house covered.
	/// If the return value is <see langword="false"/>, this value will be the constant -1.
	/// </param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// If you don't want to use the <see langword="out"/> parameter value, please
	/// use the property <see cref="InOneHouse"/> to improve the performance.
	/// </remarks>
	/// <seealso cref="InOneHouse"/>
	public readonly bool AllSetsAreInOneHouse(out int houseIndex)
	{
#pragma warning disable IDE0055
		if ((_high &            -1L) == 0 && (_low & ~     0x1C0E07L) == 0) { houseIndex =  0; return true; }
		if ((_high &            -1L) == 0 && (_low & ~     0xE07038L) == 0) { houseIndex =  1; return true; }
		if ((_high &            -1L) == 0 && (_low & ~    0x70381C0L) == 0) { houseIndex =  2; return true; }
		if ((_high & ~        0x70L) == 0 && (_low & ~ 0x7038000000L) == 0) { houseIndex =  3; return true; }
		if ((_high & ~       0x381L) == 0 && (_low & ~0x181C0000000L) == 0) { houseIndex =  4; return true; }
		if ((_high & ~      0x1C0EL) == 0 && (_low & ~  0xE00000000L) == 0) { houseIndex =  5; return true; }
		if ((_high & ~ 0x381C0E000L) == 0 && (_low &             -1L) == 0) { houseIndex =  6; return true; }
		if ((_high & ~0x1C0E070000L) == 0 && (_low &             -1L) == 0) { houseIndex =  7; return true; }
		if ((_high & ~0xE070380000L) == 0 && (_low &             -1L) == 0) { houseIndex =  8; return true; }
		if ((_high &            -1L) == 0 && (_low & ~        0x1FFL) == 0) { houseIndex =  9; return true; }
		if ((_high &            -1L) == 0 && (_low & ~      0x3FE00L) == 0) { houseIndex = 10; return true; }
		if ((_high &            -1L) == 0 && (_low & ~    0x7FC0000L) == 0) { houseIndex = 11; return true; }
		if ((_high &            -1L) == 0 && (_low & ~  0xFF8000000L) == 0) { houseIndex = 12; return true; }
		if ((_high & ~         0xFL) == 0 && (_low & ~0x1F000000000L) == 0) { houseIndex = 13; return true; }
		if ((_high & ~      0x1FF0L) == 0 && (_low &             -1L) == 0) { houseIndex = 14; return true; }
		if ((_high & ~    0x3FE000L) == 0 && (_low &             -1L) == 0) { houseIndex = 15; return true; }
		if ((_high & ~  0x7FC00000L) == 0 && (_low &             -1L) == 0) { houseIndex = 16; return true; }
		if ((_high & ~0xFF80000000L) == 0 && (_low &             -1L) == 0) { houseIndex = 17; return true; }
		if ((_high & ~  0x80402010L) == 0 && (_low & ~ 0x1008040201L) == 0) { houseIndex = 18; return true; }
		if ((_high & ~ 0x100804020L) == 0 && (_low & ~ 0x2010080402L) == 0) { houseIndex = 19; return true; }
		if ((_high & ~ 0x201008040L) == 0 && (_low & ~ 0x4020100804L) == 0) { houseIndex = 20; return true; }
		if ((_high & ~ 0x402010080L) == 0 && (_low & ~ 0x8040201008L) == 0) { houseIndex = 21; return true; }
		if ((_high & ~ 0x804020100L) == 0 && (_low & ~0x10080402010L) == 0) { houseIndex = 22; return true; }
		if ((_high & ~0x1008040201L) == 0 && (_low & ~  0x100804020L) == 0) { houseIndex = 23; return true; }
		if ((_high & ~0x2010080402L) == 0 && (_low & ~  0x201008040L) == 0) { houseIndex = 24; return true; }
		if ((_high & ~0x4020100804L) == 0 && (_low & ~  0x402010080L) == 0) { houseIndex = 25; return true; }
		if ((_high & ~0x8040201008L) == 0 && (_low & ~  0x804020100L) == 0) { houseIndex = 26; return true; }
#pragma warning restore IDE0055

		houseIndex = -1;
		return false;
	}

	/// <summary>
	/// Determine whether the map contains the specified offset.
	/// </summary>
	/// <param name="item">The offset.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(int item) => ((item < Shifting ? _low : _high) >> item % Shifting & 1) != 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is CellMap comparer && Equals(comparer);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped in CellMap other) => _low == other._low && _high == other._high;

	/// <summary>
	/// Get the sub-view mask of this map.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetSubviewMask(int houseIndex) => this / houseIndex;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => HashCode.Combine(_low, _high);

	/// <summary>
	/// Get all offsets whose bits are set <see langword="true"/>.
	/// </summary>
	/// <returns>An array of offsets.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int[] ToArray() => Offsets;

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString() => ToString(null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
	{
		return format switch
		{
			null or "N" or "n" => RxCyNotation.ToCellsString(this),
			"B" or "b" => binaryToString(this, false),
			"T" or "t" => tableToString(this),
			_ => throw new FormatException("The specified format is invalid.")
		};


		static string tableToString(scoped in CellMap @this)
		{
			scoped var sb = new StringHandler((3 * 7 + 2) * 13);
			for (int i = 0; i < 3; i++)
			{
				for (int bandLn = 0; bandLn < 3; bandLn++)
				{
					for (int j = 0; j < 3; j++)
					{
						for (int columnLn = 0; columnLn < 3; columnLn++)
						{
							sb.Append(@this.Contains((i * 3 + bandLn) * 9 + j * 3 + columnLn) ? '*' : '.');
							sb.Append(' ');
						}

						if (j != 2)
						{
							sb.Append("| ");
						}
						else
						{
							sb.AppendLine();
						}
					}
				}

				if (i != 2)
				{
					sb.Append("------+-------+------");
					sb.AppendLine();
				}
			}

			return sb.ToStringAndClear();
		}

		static string binaryToString(scoped in CellMap @this, bool withSeparator)
		{
			scoped var sb = new StringHandler(81);
			int i;
			long value = @this._low;
			for (i = 0; i < 27; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			if (withSeparator)
			{
				sb.Append(' ');
			}
			for (; i < 41; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			for (value = @this._high; i < 54; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}
			if (withSeparator)
			{
				sb.Append(' ');
			}
			for (; i < 81; i++, value >>= 1)
			{
				sb.Append(value & 1);
			}

			sb.Reverse();
			return sb.ToStringAndClear();
		}
	}

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<int> GetEnumerator() => Offsets.EnumerateImmutable();

	/// <summary>
	/// Gets the <see cref="CellMap"/> instance that starts with the specified index.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="count">The desired number of offsets.</param>
	/// <returns>The <see cref="CellMap"/> result.</returns>
	public readonly CellMap Slice(int start, int count)
	{
		var result = Empty;
		int[] offsets = Offsets;
		for (int i = start, end = start + count; i < end; i++)
		{
			result += offsets[i];
		}

		return result;
	}

	/// <summary>
	/// Set the specified offset as <see langword="true"/> value.
	/// </summary>
	/// <param name="item">The offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(int item)
	{
		scoped ref long v = ref item / Shifting == 0 ? ref _low : ref _high;
		bool older = Contains(item);
		v |= 1L << item % Shifting;
		if (!older)
		{
			_count++;
		}
	}

	/// <summary>
	/// Set the specified offset as <see langword="true"/> value, with range check.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified cell offset is invalid.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddChecked(int offset)
	{
		if (offset is not (>= 0 and < 81))
		{
			throw new ArgumentOutOfRangeException(nameof(offset), "The cell offset is invalid.");
		}

		scoped ref long v = ref offset / Shifting == 0 ? ref _low : ref _high;
		bool older = Contains(offset);
		v |= checked(1L << offset % Shifting);
		if (!older)
		{
			_count++;
		}
	}

	/// <summary>
	/// Set the specified offsets as <see langword="true"/> value.
	/// </summary>
	/// <param name="offsets">The offsets to add.</param>
	public void AddRange(scoped in ReadOnlySpan<int> offsets)
	{
		foreach (int cell in offsets)
		{
			Add(cell);
		}
	}

	/// <inheritdoc cref="AddRange(in ReadOnlySpan{int})"/>
	public void AddRange(IEnumerable<int> offsets)
	{
		foreach (int cell in offsets)
		{
			Add(cell);
		}
	}

	/// <inheritdoc cref="AddRange(in ReadOnlySpan{int})"/>
	/// <remarks>
	/// Different with the method <see cref="AddRange(IEnumerable{int})"/>, this method
	/// also checks for the validity of each cell offsets. If the value is below 0 or greater than 80,
	/// this method will throw an exception to report about this.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when found at least one cell offset invalid.
	/// </exception>
	public void AddRangeChecked(IEnumerable<int> offsets)
	{
		const string error = "The value cannot be added because the cell offset is an invalid value.";
		foreach (int cell in offsets)
		{
			Add(cell is < 0 or >= 81 ? throw new InvalidOperationException(error) : cell);
		}
	}

	/// <summary>
	/// Set the specified offset as <see langword="false"/> value.
	/// </summary>
	/// <param name="offset">The offset.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(int offset)
	{
		scoped ref long v = ref offset / Shifting == 0 ? ref _low : ref _high;
		bool older = Contains(offset);
		v &= ~(1L << offset % Shifting);
		if (older)
		{
			_count--;
		}
	}

	/// <summary>
	/// Clear all bits.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => _low = _high = _count = 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IEquatable<CellMap>.Equals(CellMap other) => Equals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => Offsets.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => ((IEnumerable<int>)Offsets).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<int>.IsProperSubsetOf(IEnumerable<int> other)
	{
		var otherCells = Empty + other;
		return this != otherCells && (otherCells & this) == this;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<int>.IsProperSupersetOf(IEnumerable<int> other)
	{
		var otherCells = Empty + other;
		return this != otherCells && (this & otherCells) == otherCells;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<int>.IsSubsetOf(IEnumerable<int> other) => ((Empty + other) & this) == this;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<int>.IsSupersetOf(IEnumerable<int> other)
	{
		var otherCells = Empty + other;
		return (this & otherCells) == otherCells;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<int>.Overlaps(IEnumerable<int> other) => (this & (Empty + other)) is not [];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool IReadOnlySet<int>.SetEquals(IEnumerable<int> other) => this == Empty + other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly unsafe void ICollection<int>.CopyTo(int[] array, int arrayIndex)
	{
		fixed (int* pArray = array)
		{
			CopyTo(pArray + arrayIndex, _count - arrayIndex);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<int>.IsProperSubsetOf(IEnumerable<int> other)
		=> ((IReadOnlySet<int>)this).IsProperSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<int>.IsProperSupersetOf(IEnumerable<int> other)
		=> ((IReadOnlySet<int>)this).IsProperSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<int>.IsSubsetOf(IEnumerable<int> other) => ((IReadOnlySet<int>)this).IsSubsetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<int>.IsSupersetOf(IEnumerable<int> other) => ((IReadOnlySet<int>)this).IsSupersetOf(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<int>.Overlaps(IEnumerable<int> other) => ((IReadOnlySet<int>)this).Overlaps(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly bool ISet<int>.SetEquals(IEnumerable<int> other) => ((IReadOnlySet<int>)this).SetEquals(other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISet<int>.Add(int item)
	{
		scoped ref long v = ref item / Shifting == 0 ? ref _low : ref _high;
		bool older = Contains(item);
		v |= 1L << item % Shifting;
		if (!older)
		{
			_count++;
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ICollection<int>.Remove(int item)
	{
		scoped ref long v = ref item / Shifting == 0 ? ref _low : ref _high;
		bool older = Contains(item);
		v &= ~(1L << item % Shifting);
		if (older)
		{
			_count--;
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.ExceptWith(IEnumerable<int> other) => this -= Empty + other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.IntersectWith(IEnumerable<int> other) => this &= Empty + other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.SymmetricExceptWith(IEnumerable<int> other)
	{
		Clear();
		this |= (this - (Empty + other)) | (Empty + other - this);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISet<int>.UnionWith(IEnumerable<int> other) => this |= Empty + other;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse(string str, out CellMap result) => RxCyNotation.TryParseCells(str, out result);

	/// <summary>
	/// Initializes an instance with two binary values.
	/// </summary>
	/// <param name="high">Higher 40 bits.</param>
	/// <param name="low">Lower 41 bits.</param>
	/// <returns>The result instance created.</returns>
	public static CellMap CreateByBits(long high, long low)
	{
		CellMap result;
		(result._high, result._low, result._count) = (high, low, PopCount((ulong)high) + PopCount((ulong)low));

		return result;
	}

	/// <summary>
	/// Initializes an instance with three binary values.
	/// </summary>
	/// <param name="high">Higher 27 bits.</param>
	/// <param name="mid">Medium 27 bits.</param>
	/// <param name="low">Lower 27 bits.</param>
	/// <returns>The result instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateByBits(int high, int mid, int low)
		=> CreateByBits((high & 0x7FFFFFFL) << 13 | mid >> 14 & 0x1FFFL, (mid & 0x3FFFL) << 27 | low & 0x7FFFFFFL);

	/// <summary>
	/// Initializes an instance with an <see cref="Int128"/> integer.
	/// </summary>
	/// <param name="int128">The <see cref="Int128"/> integer.</param>
	/// <returns>The result instance created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap CreateByInt128(scoped in Int128 int128)
		=> CreateByBits((long)(ulong)(int128 >> 64), (long)(ulong)(int128 & ulong.MaxValue));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap Parse(string str) => RxCyNotation.ParseCells(str);


	/// <summary>
	/// Gets the peer intersection of the current instance.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	/// <returns>The result list that is the peer intersection of the current instance.</returns>
	/// <remarks>
	/// A <b>Peer Intersection</b> is a set of cells that all cells from the base collection can be seen.
	/// For more information please visit <see href="https://sunnieshine.github.io/Sudoku/terms/peer">this link</see>.
	/// </remarks>
	public static CellMap operator +(scoped in CellMap offsets)
	{
		long lowerBits = 0, higherBits = 0;
		int i = 0;
		foreach (int offset in offsets.Offsets)
		{
			long low = 0, high = 0;
			foreach (int peer in Peers[offset])
			{
				(peer / Shifting == 0 ? ref low : ref high) |= 1L << peer % Shifting;
			}

			if (i++ == 0)
			{
				lowerBits = low;
				higherBits = high;
			}
			else
			{
				lowerBits &= low;
				higherBits &= high;
			}
		}

		return CreateByBits(higherBits, lowerBits);
	}

	/// <summary>
	/// Determines whether the current collection is empty.
	/// </summary>
	/// <param name="offsets">The cells to be checked.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !(scoped in CellMap offsets) => offsets ? false : true;

	/// <summary>
	/// Reverse status for all offsets, which means all <see langword="true"/> bits
	/// will be set <see langword="false"/>, and all <see langword="false"/> bits
	/// will be set <see langword="true"/>.
	/// </summary>
	/// <param name="offsets">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator ~(scoped in CellMap offsets)
		=> CreateByBits(~offsets._high & 0xFF_FFFF_FFFFL, ~offsets._low & 0x1FF_FFFF_FFFFL);

	/// <summary>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be added.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator +(scoped in CellMap collection, int offset)
	{
		var result = collection;
		if (result.Contains(offset))
		{
			return result;
		}

		(offset / Shifting == 0 ? ref result._low : ref result._high) |= 1L << offset % Shifting;
		result._count++;
		return result;
	}

	/// <summary>
	/// Adds the specified <paramref name="offset"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// This operator will check the validity of the argument <paramref name="offset"/>.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be added.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator checked +(scoped in CellMap collection, int offset)
	{
		Argument.ThrowIfInvalid(offset is >= 0 and < 81, "The offset is invalid.");

		return collection + offset;
	}

	/// <summary>
	/// Adds the specified list of <paramref name="cells"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="cells">A list of cells to be added.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator +(scoped in CellMap collection, IEnumerable<int> cells)
	{
		var result = collection;
		result.AddRange(cells);
		return result;
	}

	/// <summary>
	/// Adds the specified list of <paramref name="cells"/> to the <paramref name="collection"/>,
	/// and returns the added result.
	/// This operator will check the validity of the argument <paramref name="cells"/>.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="cells">A list of cells to be added.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator checked +(scoped in CellMap collection, IEnumerable<int> cells)
	{
		var result = collection;
		result.AddRangeChecked(cells);
		return result;
	}

	/// <summary>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator -(scoped in CellMap collection, int offset)
	{
		var result = collection;
		if (!result.Contains(offset))
		{
			return collection;
		}

		(offset / Shifting == 0 ? ref result._low : ref result._high) &= ~(1L << offset % Shifting);
		result._count--;
		return result;
	}

	/// <summary>
	/// Removes the specified <paramref name="offset"/> from the <paramref name="collection"/>,
	/// and returns the removed result. This operator will check the validity of the argument <paramref name="offset"/>.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <param name="offset">The offset to be removed.</param>
	/// <returns>The result collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator checked -(scoped in CellMap collection, int offset)
	{
		Argument.ThrowIfInvalid(offset is >= 0 and < 81, "The offset is invalid.");

		return collection - offset;
	}

	/// <summary>
	/// Get a <see cref="CellMap"/> that contains all <paramref name="left"/> instance
	/// but not in <paramref name="right"/> instance.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator -(scoped in CellMap left, scoped in CellMap right) => left & ~right;

	/// <summary>
	/// Gets the subsets of the current collection via the specified size
	/// indicating the number of elements of the each subset.
	/// </summary>
	/// <param name="cells">Indicates the base template cells.</param>
	/// <param name="subsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> &gt; <paramref name="cells"/>.Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> == <paramref name="cells"/>.Count</c></term>
	/// <description>
	/// Will return an array that contains only one element, same as the argument <paramref name="cells"/>.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// For example, if the argument <paramref name="cells"/> is <c>r1c1</c>, <c>r1c2</c> and <c>r1c3</c>
	/// and the argument <paramref name="subsetSize"/> is 2, the expression <c><![CDATA[cells & 2]]></c>
	/// will be an array of 3 elements given below: <c>r1c12</c>, <c>r1c13</c> and <c>r1c23</c>.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe CellMap[] operator &(scoped in CellMap cells, int subsetSize)
	{
		if (subsetSize == 0 || subsetSize > cells._count)
		{
			return Array.Empty<CellMap>();
		}

		if (subsetSize == cells._count)
		{
			return new[] { cells };
		}

		int totalIndex = 0, n = cells._count;
		int* buffer = stackalloc int[subsetSize];
		var result = new CellMap[Combinatorials[n - 1, subsetSize - 1]];
		f(subsetSize, n, subsetSize, cells.Offsets);
		return result;


		void f(int size, int last, int index, int[] offsets)
		{
			for (int i = last; i >= index; i--)
			{
				buffer[index - 1] = i - 1;
				if (index > 1)
				{
					f(size, i - 1, index - 1, offsets);
				}
				else
				{
					int[] temp = new int[size];
					for (int j = 0; j < size; j++)
					{
						temp[j] = offsets[buffer[j]];
					}

					result[totalIndex++] = (CellMap)temp;
				}
			}
		}
	}

	/// <summary>
	/// Get the elements that both <paramref name="left"/> and <paramref name="right"/> contain.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator &(scoped in CellMap left, scoped in CellMap right)
		=> CreateByBits(left._high & right._high, left._low & right._low);

	/// <summary>
	/// Gets all subsets of the current collection via the specified size
	/// indicating the <b>maximum</b> number of elements of the each subset.
	/// </summary>
	/// <param name="cells">Indicates the base template cells.</param>
	/// <param name="subsetSize">The size to get.</param>
	/// <returns>
	/// All possible subsets. If:
	/// <list type="table">
	/// <listheader>
	/// <term>Condition</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><paramref name="subsetSize"/> &gt; <paramref name="cells"/>.Count</c></term>
	/// <description>Will return an empty array</description>
	/// </item>
	/// <item>
	/// <term>Other cases</term>
	/// <description>The valid combinations.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// For example, the expression <c>cells | 3</c> is equivalent to all possible cases
	/// coming from <c><![CDATA[cells & 1]]></c>,
	/// <c><![CDATA[cells & 2]]></c> and <c><![CDATA[cells & 3]]></c>.
	/// </remarks>
	public static CellMap[] operator |(scoped in CellMap cells, int subsetSize)
	{
		if (subsetSize == 0 || cells is [])
		{
			return Array.Empty<CellMap>();
		}

		int n = cells._count;

		int desiredSize = 0;
		int length = Min(n, subsetSize);
		for (int i = 1; i <= length; i++)
		{
			int target = Combinatorials[n - 1, i - 1];
			desiredSize += target;
		}

		var result = new List<CellMap>(desiredSize);
		for (int i = 1; i <= length; i++)
		{
			result.AddRange(cells & i);
		}

		return result.ToArray();
	}

	/// <summary>
	/// Combine the elements from <paramref name="left"/> and <paramref name="right"/>,
	/// and return the merged result.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator |(scoped in CellMap left, scoped in CellMap right)
		=> CreateByBits(left._high | right._high, left._low | right._low);

	/// <summary>
	/// Get the elements that either <paramref name="left"/> or <paramref name="right"/> contains.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator ^(scoped in CellMap left, scoped in CellMap right)
		=> CreateByBits(left._high ^ right._high, left._low ^ right._low);

	/// <summary>
	/// Expands the operator to <c><![CDATA[+(a & b) & b]]></c>.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="template">The template map that the base map to check and cover.</param>
	/// <returns>The result map.</returns>
	/// <remarks>
	/// <para>
	/// The operator is commonly used for checking eliminations, especially in type 2 of deadly patterns. 
	/// </para>
	/// <para>
	/// For example, if we should check the eliminations
	/// of digit <c>d</c>, we may use the expression
	/// <code><![CDATA[
	/// +(urCells & grid.CandidatesMap[d]) & grid.CandidatesMap[d]
	/// ]]></code>
	/// to express the eliminations are the peer intersection of cells of digit <c>d</c>
	/// appeared in <c>urCells</c>. This expression can be simplified to
	/// <code><![CDATA[
	/// urCells % grid.CandidatesMap[d]
	/// ]]></code>
	/// </para>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CellMap operator %(scoped in CellMap @base, scoped in CellMap template)
		=> +(@base & template) & template;

	/// <summary>
	/// Expands via the specified digit.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The result instance.</returns>
	public static Candidates operator *(scoped in CellMap @base, int digit)
	{
		var result = Candidates.Empty;
		foreach (int cell in @base.Offsets)
		{
			result.AddAnyway(cell * 9 + digit);
		}

		return result;
	}

	/// <summary>
	/// Expands via the specified digit. This operator will check the validity of the argument <paramref name="digit"/>.
	/// </summary>
	/// <param name="base">The base map.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>The result instance.</returns>
	public static Candidates operator checked *(scoped in CellMap @base, int digit)
	{
		Argument.ThrowIfFalse(digit is >= 0 and < 9, "The argument is invalid.");

		return @base * digit;
	}

	/// <summary>
	/// Get the sub-view mask of this map.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The mask.</returns>
	public static short operator /(scoped in CellMap map, int houseIndex)
	{
		short p = 0, i = 0;
		foreach (int cell in HouseCells[houseIndex])
		{
			if (map.Contains(cell))
			{
				p |= (short)(1 << i);
			}

			i++;
		}

		return p;
	}

	/// <summary>
	/// Get the sub-view mask of this map. This operator will check the validity
	/// of the argument <paramref name="houseIndex"/>.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>The mask.</returns>
	public static short operator checked /(scoped in CellMap map, int houseIndex)
	{
		Argument.ThrowIfFalse(houseIndex is >= 0 and < 27);

		return map / houseIndex;
	}

	/// <summary>
	/// Determines whether two <see cref="CellMap"/> instances are considered equal.
	/// </summary>
	/// <param name="left">The first instance to be compared.</param>
	/// <param name="right">The second instance to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether they are equal.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in CellMap left, scoped in CellMap right) => left.Equals(right);

	/// <summary>
	/// Determines whether two <see cref="CellMap"/> instances are not totally equal.
	/// </summary>
	/// <param name="left">The first instance to be compared.</param>
	/// <param name="right">The second instance to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether they are not equal.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in CellMap left, scoped in CellMap right) => !(left == right);

	/// <summary>
	/// Determines whether the specified <see cref="CellMap"/> collection is not empty.
	/// </summary>
	/// <param name="cells">The collection.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator true(scoped in CellMap cells) => cells.Count != 0;

	/// <summary>
	/// Determines whether the specified <see cref="CellMap"/> collection is empty.
	/// </summary>
	/// <param name="cells">The collection.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator false(scoped in CellMap cells) => cells.Count == 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool ILogicalNotOperators<CellMap, bool>.operator !(CellMap value) => !value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static short IDivisionOperators<CellMap, int, short>.operator /(CellMap left, int right) => left / right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IUnaryPlusOperators<CellMap, CellMap>.operator +(CellMap value) => +value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IAdditionOperators<CellMap, int, CellMap>.operator +(CellMap left, int right) => left + right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ISubtractionOperators<CellMap, int, CellMap>.operator -(CellMap left, int right) => left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ISubtractionOperators<CellMap, CellMap, CellMap>.operator -(CellMap left, CellMap right) => left - right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Candidates IMultiplyOperators<CellMap, int, Candidates>.operator *(CellMap left, int right) => left * right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IModulusOperators<CellMap, CellMap, CellMap>.operator %(CellMap left, CellMap right) => left % right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IBitwiseOperators<CellMap, CellMap, CellMap>.operator ~(CellMap value) => ~value;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IBitwiseOperators<CellMap, CellMap, CellMap>.operator &(CellMap left, CellMap right) => left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IBitwiseOperators<CellMap, CellMap, CellMap>.operator |(CellMap left, CellMap right) => left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap IBitwiseOperators<CellMap, CellMap, CellMap>.operator ^(CellMap left, CellMap right) => left ^ right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ILogicalOperators<CellMap, CellMap, CellMap>.operator &(CellMap value, CellMap other) => value & other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static CellMap ILogicalOperators<CellMap, CellMap, CellMap>.operator |(CellMap value, CellMap other) => value | other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<CellMap, CellMap, bool>.operator ==(CellMap left, CellMap right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<CellMap, CellMap, bool>.operator !=(CellMap left, CellMap right) => left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IBooleanOperators<CellMap>.operator true(CellMap value) => value ? true : false;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IBooleanOperators<CellMap>.operator false(CellMap value) => value ? false : true;


	/// <summary>
	/// Implicit cast from <see cref="CellMap"/> to <see cref="int"/>[].
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator int[](scoped in CellMap offsets) => offsets.ToArray();

	/// <summary>
	/// Implicit cast from <see cref="CellMap"/> to <see cref="Int128"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Int128(scoped in CellMap offsets) => new((ulong)offsets._high, (ulong)offsets._low);

	/// <summary>
	/// Implicit cast from <see cref="Span{T}"/> to <see cref="CellMap"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	public static implicit operator CellMap(scoped in Span<int> offsets)
	{
		var result = Empty;
		foreach (int offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <summary>
	/// Implicit cast from <see cref="ReadOnlySpan{T}"/> to <see cref="CellMap"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	public static implicit operator CellMap(scoped in ReadOnlySpan<int> offsets)
	{
		var result = Empty;
		foreach (int offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <summary>
	/// Explicit cast from <see cref="int"/>[] to <see cref="CellMap"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	public static explicit operator CellMap(int[] offsets)
	{
		var result = Empty;
		foreach (int offset in offsets)
		{
			result.Add(offset);
		}

		return result;
	}

	/// <summary>
	/// Explicit cast from <see cref="int"/>[] to <see cref="CellMap"/>, with cell range check.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when a certain element in the argument <paramref name="offsets"/>
	/// is not a valid value to represent a cell offset.
	/// </exception>
	public static explicit operator checked CellMap(int[] offsets)
	{
		var result = Empty;
		foreach (int offset in offsets)
		{
			result.AddChecked(offset);
		}

		return result;
	}

	/// <summary>
	/// Explicit cast from <see cref="Int128"/> to <see cref="CellMap"/>.
	/// </summary>
	/// <param name="int128">The <see cref="Int128"/> integer.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator CellMap(scoped in Int128 int128) => CreateByInt128(int128);

	/// <summary>
	/// Explicit cast from <see cref="Int128"/> to <see cref="CellMap"/>.
	/// </summary>
	/// <param name="int128">The <see cref="Int128"/> integer.</param>
	/// <exception cref="OverflowException">
	/// Throws when the base argument <paramref name="int128"/> is greater than the maximum value
	/// corresponding to <see cref="MaxValue"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator checked CellMap(scoped in Int128 int128)
		=> int128 >> 81 == 0
			? CreateByInt128(int128)
			: throw new OverflowException($"The base {nameof(Int128)} integer is greater than '{nameof(MaxValue)}'.");

	/// <summary>
	/// Explicit cast from <see cref="CellMap"/> to <see cref="Span{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Span<int>(scoped in CellMap offsets) => offsets.Offsets;

	/// <summary>
	/// Explicit cast from <see cref="CellMap"/> to <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="offsets">The offsets.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ReadOnlySpan<int>(scoped in CellMap offsets) => offsets.Offsets;
}
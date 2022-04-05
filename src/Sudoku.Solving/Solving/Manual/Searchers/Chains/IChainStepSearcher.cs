﻿namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for chain steps.
/// </summary>
public interface IChainStepSearcher : IStepSearcher
{
	/// <summary>
	/// Creates an array of presentation data of candidates
	/// via the specified instance of type <see cref="AlternatingInferenceChain"/>.
	/// </summary>
	/// <param name="chain">The chain.</param>
	/// <returns>An array of presentation data of candidates.</returns>
	protected static CandidateViewNode[] GetViewOnCandidates(AlternatingInferenceChain chain)
	{
		var realChainNodes = chain.RealChainNodes;
		var result = new List<CandidateViewNode>(realChainNodes.Length);
		for (int i = 0; i < realChainNodes.Length; i++)
		{
			if (realChainNodes[i] is { Cells: var cells, Digit: var digit })
			{
				// TODO: Get grouped node.
				foreach (int cell in cells)
				{
					result.Add(new(i & 1, cell * 9 + digit));
				}
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// Creates an array of the presentation data of links
	/// via the specified instance of type <see cref="AlternatingInferenceChain"/>.
	/// </summary>
	/// <param name="chain">The chain.</param>
	/// <returns>An array of presentation data of links.</returns>
	protected static LinkViewNode[] GetViewOnLinks(in AlternatingInferenceChain chain)
	{
		var realChainNodes = chain.RealChainNodes;
		var result = new LinkViewNode[realChainNodes.Length + 1];
		for (int i = 0; i < realChainNodes.Length - 1; i++)
		{
			if (realChainNodes[i] is { Cells: var aCells, Digit: var aDigit }
				&& realChainNodes[i + 1] is { Cells: var bCells, Digit: var bDigit })
			{
				// TODO: Disctinct strong and weak link.
				result[i] = new(0, new(aDigit, aCells), new(bDigit, bCells), LinkKind.Strong);
			}
		}

		result[realChainNodes.Length] = new(
			0,
			new(realChainNodes[^1].Digit, realChainNodes[^1].Cells),
			new(realChainNodes[0].Digit, realChainNodes[0].Cells),
			LinkKind.Strong
		);

		return result;
	}
}

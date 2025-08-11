using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorEvaluator
{
	public TopNode baseNode;
	public Selector selector;
	public Node pointer;

	public BehaviorEvaluator(TopNode baseNode, Selector selector)
	{
		this.baseNode = baseNode;
		this.selector = selector;
	}

	public void Init()
	{
		pointer = baseNode;
	}

#nullable enable
	public void Tick()
	{
		pointer.Perform();

		Node? nextNode = selector.SelectNode(pointer.connections);
		if (nextNode == null)
		{
			// Failed to find any valid connections with given selector,
			// jump to top node to restart evaluation.
			pointer = baseNode;
			return;
		}

		pointer = nextNode;
	}
}
using System.Collections.Generic;

public class HighestPrioritySelector : Selector
{
#nullable enable
	public override Node? SelectNode(List<Connection> connections)
	{
		Node? highestNode = null;
		uint highestPriority = 0;

		foreach (Connection con in connections)
		{
			if (con.CanTraverse() && con.priority > highestPriority)
			{
				highestPriority = con.priority;
				highestNode = con.child;
			}
		}

		return highestNode;
	}
}

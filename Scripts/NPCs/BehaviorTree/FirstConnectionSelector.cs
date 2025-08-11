using System.Collections.Generic;

public class FirstConnectionSelector : Selector
{
#nullable enable
	public override Node? SelectNode(List<Connection> connections)
	{
		foreach (Connection con in connections)
		{
			if (con.CanTraverse())
				return con.child;
		}

		return null;
	}
}

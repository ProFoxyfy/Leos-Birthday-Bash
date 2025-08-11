using System.Collections.Generic;

public abstract class Selector
{
#nullable enable
	public abstract Node? SelectNode(List<Connection> connections);
}
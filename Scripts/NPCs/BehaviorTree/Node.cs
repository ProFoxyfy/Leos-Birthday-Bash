using System.Collections.Generic;

public abstract class Node
{
	public List<Connection> connections = new List<Connection>();
	public abstract void Perform();
}

public abstract class Connection
{
	public Node parent;
	public Node child;
	public Decorator decorator;
	public uint priority;

	public Connection(Node parent, Node child, Decorator decorator, uint priority)
	{
		this.parent = parent;
		this.child = child;
		this.decorator = decorator;
		this.priority = priority;
	}

	public abstract bool CanTraverse();
}

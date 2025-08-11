using System;

public class ConditionalConnection : Connection
{
	private Func<bool> condition;

	public ConditionalConnection(Node parent, Node child, Decorator decorator, uint priority, Func<bool> condition) : base(parent, child, decorator, priority)
	{
		this.parent = parent;
		this.child = child;
		this.decorator = decorator;
		this.priority = priority;
		this.condition = condition;
	}

	public override bool CanTraverse()
	{
		return decorator != null ? decorator.Evaluate(condition.Invoke()) : condition.Invoke();
	}
}
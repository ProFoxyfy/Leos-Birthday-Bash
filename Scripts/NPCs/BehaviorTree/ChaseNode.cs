public class ChaseNode : Node
{
	private BaseNPC npc;

	public ChaseNode(BaseNPC npc)
	{
		this.npc = npc;
	}

	public override void Perform()
	{
		npc.target = npc.nextTarget;
	}
}
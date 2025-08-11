public class WanderNode : Node
{
	private BaseNPC npc;

	public WanderNode(BaseNPC npc)
	{
		this.npc = npc;
	}

	public override void Perform()
	{
		if (npc.destinationReached)
			npc.target = EnvironmentController.Instance.GetRandomNavigableTile().position;
	}
}

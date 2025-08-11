public struct PosterData
{
	public PosterData(Pos2 position, string id, Direction dir)
	{
		this.position = position;
		this.id = id;
		this.dir = dir;
	}

	public Pos2 position;
	public string id;
	public Direction dir;
}
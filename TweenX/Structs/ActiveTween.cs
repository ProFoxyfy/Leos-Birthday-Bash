namespace TweenX
{
    public struct ActiveTween
    {
        public Tween data;
        public XFloat target;
        public int priority;

        public ActiveTween(Tween data, XFloat target, int priority)
        {
            this.data = data;
            this.target = target;
            this.priority = priority;
        }
    }
}
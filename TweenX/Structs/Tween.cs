using TweenX.EasingStyles.Advanced;

namespace TweenX
{
    public struct Tween
    {
        public float duration;
        public float start;
        public float end;
        public IEasingFunction ease;

        public Tween(float duration, IEasingFunction ease, float start, float end)
        {
            this.duration = duration;
            this.ease = ease;
            this.start = start;
            this.end = end;
        }
    }
}
using TweenX.EasingStyles.Advanced;

namespace TweenX.EasingStyles
{
    public class Linear : IEasingFunction
    {
        public float Evaluate(float T)
        {
            return T;
        }
    }
}
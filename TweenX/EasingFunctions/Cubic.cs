using TweenX.EasingStyles.Advanced;
using UnityEngine;

namespace TweenX.EasingStyles
{
    public class CubicIn : IEasingFunction
    {
        public float Evaluate(float T)
        {
            return T * T * T;
        }
    }

	public class CubicOut : IEasingFunction
	{
		public float Evaluate(float T)
		{
			return 1 - Mathf.Pow(1 - T, 3);
		}
	}

	public class CubicInOut : IEasingFunction
	{
		public float Evaluate(float T)
		{
			return T < 0.5 ? 4 * T * T * T : T - Mathf.Pow(-2 * T + 2, 3) / 2;
		}
	}
}
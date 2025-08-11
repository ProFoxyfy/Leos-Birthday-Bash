using System;
using UnityEngine;

namespace TweenX
{
	public class XFloat
	{
		private float value;

		public XFloat(float val)
		{
			this.value = val;
		}

		public void Set(float newVal)
		{
			this.value = newVal;
		}

		public static implicit operator float(XFloat val)
		{
			return val.value;
		}

		public static implicit operator XFloat(float v)
		{
			return new XFloat(v);
		}
	}
}
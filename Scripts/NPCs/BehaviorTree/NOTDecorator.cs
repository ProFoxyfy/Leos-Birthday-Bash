using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTDecorator : Decorator
{
	public override bool Evaluate(bool input)
	{
		return !input;
	}
}

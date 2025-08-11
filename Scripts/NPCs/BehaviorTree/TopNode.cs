using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopNode : Node
{
	public override void Perform()
	{
		// The top node does not need to execute logic,
		// it only serves as a parent to all nodes,
		// so connections can run properly.
		return;
	}
}

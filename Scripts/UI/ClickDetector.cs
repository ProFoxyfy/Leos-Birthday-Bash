using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickDetector : MonoBehaviour, IClickable
{
	public bool isHoverable { get; set; }
	public bool isEnabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}
	public ClickableState state { get; set; }
	public UnityEvent onClick = new UnityEvent();

	private void Awake()
	{
		isHoverable = false;
	}

	public void Down()
	{
		state = ClickableState.Down;
	}

	public void Hover()
	{
		state = ClickableState.Hover;
	}

	public void Unhover()
	{
		state = ClickableState.None;
	}

	public void Up()
	{
		if (state == ClickableState.Down)
			onClick.Invoke();
		state = ClickableState.None;
	}
}

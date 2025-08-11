using UnityEngine;
using TweenX;
using TweenX.EasingStyles;

public class FogEvent : ChaosEvent
{
	private XFloat fogEnd = -1;
	private TweenManager twMan;

	void Awake()
	{
		twMan = gameObject.AddComponent<TweenManager>();
		twMan.useUnscaled = true;
	}

	void Fade()
	{
		twMan.PlayTweenSingle(ref fogEnd, new Tween(3f, new Linear(), 3f, -1f));
	}

	void Update()
	{
		Shader.SetGlobalFloat("_FogEnd", fogEnd);
		Shader.SetGlobalColor("_FogColor", Color.gray);
	}

    public override void Activate(ChaosEventManager manager)
	{
		manager.ShowMessage("Oh noes!! The fog is coming...");
		twMan.PlayTweenSingle(ref fogEnd, new Tween(3f, new Linear(), 10000f, 3f));
		Invoke("Fade", 60);
	}
}

using UnityEngine;

public abstract class ChaosEvent : MonoBehaviour
{
    public abstract void Activate(ChaosEventManager manager);
}

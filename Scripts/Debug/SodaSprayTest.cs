using UnityEngine;
using UnityEngine.InputSystem;

public class SodaSprayTest : MonoBehaviour
{
    private GameObject spray;
    private PlayerManager player;
    private Camera mainCam;

    private void Start()
    {
        player = FindObjectOfType<PlayerManager>();
        mainCam = Camera.main;
        spray = Resources.Load<GameObject>("SodaSpray");

        GameInputManager.Instance.GetAction("ItemUse").performed += Action;
    }

    private void Action(InputAction.CallbackContext context)
    {
        if (player == null)
            return;

        Instantiate(spray, player.transform.position, mainCam.transform.rotation, null);
    }
}

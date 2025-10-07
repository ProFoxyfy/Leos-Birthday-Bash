using System;
using System.Collections;
using System.Globalization;
using TMPro;
using TweenX;
using TweenX.EasingStyles;
using TweenX.EasingStyles.Advanced;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
	internal CharacterController controller;
	public Camera cam;

	public float speed = 0f;
	public float walkSpeed = 3.0f;
	public float runSpeed = 5.0f;

	public bool sprinting = false;
	public float stamina = 1f;

	// slippy slidy
	public bool slippery = false;

	public ProjectileController projectile;

	public float staminaDecrement = 0.1f;
	public float staminaRegen = 0.05f;

	public float staminaRegenTimer = 0f;
	public int staminaRegenTime = 2;

	public float sensitivity = 1f;
	public Color selectedColor;

	private bool sprintRequest;
	public bool disableSprint = false;
	private bool mirrorRequest;
	private Vector2 moveDelta;
	private Vector2 lookDelta;
	private Vector2 lookDirection;
	private float mWheel;
	private int scrollState;

	private BaseItem[] inventory = new BaseItem[3];
	private int selectedSlot = 0;

	public Image[] slots;
	public Image[] itemImages;
	public TMP_Text itemTxt;
	internal ItemData itmData;
	public bool infItems = false;

	public float baseFOV = 60f;
	public float sprintFOV = 20f; // FOV increase when sprinting
	public float fovOffset = 0f;
	private float fovTweenDuration = 0.2f;
	private XFloat currentFOV;
	private IEasingFunction fovEase = new CubicInOut();
	public bool frozen = false;

	private TweenManager twMan;
	public AudioObject itemPickupSfx;
	AudioManager audMan;

	Leo leo;
	Doubt doubt;

	bool jumpscare = false;
	Transform jumpscareEnemy;

	private void Start()
	{
		slippery = false;
		currentFOV = baseFOV;
		twMan = gameObject.AddComponent<TweenManager>();
		twMan.useUnscaled = false;

		audMan = gameObject.AddComponent<AudioManager>();

		cam = Camera.main;

		controller = GetComponent<CharacterController>();

		Cursor.lockState = CursorLockMode.Locked;

		leo = UnityEngine.Object.FindAnyObjectByType<Leo>();

		UpdateSlots();

		lookDirection = new Vector3(0, 180, 0);
	}

	private void UpdateFOV()
	{
		if (GameInputManager.Instance.GetAction("Sprint").WasPressedThisFrame() && moveDelta.magnitude > 0)
			twMan.PlayTweenSingle(ref currentFOV, new Tween(fovTweenDuration, fovEase, currentFOV, baseFOV + sprintFOV));
		else if (GameInputManager.Instance.GetAction("Sprint").WasReleasedThisFrame())
			twMan.PlayTweenSingle(ref currentFOV, new Tween(fovTweenDuration, fovEase, currentFOV, baseFOV));
	}

	private void UpdateFOVOverride()
	{
		if (GameInputManager.Instance.GetAction("Sprint").IsPressed() && moveDelta.magnitude > 0)
			twMan.PlayTweenSingle(ref currentFOV, new Tween(fovTweenDuration, fovEase, currentFOV, baseFOV + sprintFOV));
		else if (!GameInputManager.Instance.GetAction("Sprint").IsPressed())
			twMan.PlayTweenSingle(ref currentFOV, new Tween(fovTweenDuration, fovEase, currentFOV, baseFOV));
	}

	public void ChangeFOV(float newFOV)
	{
		baseFOV = newFOV;
		UpdateFOVOverride();
	}

	Vector3 movDir = Vector3.zero;

	private void Update()
	{
		speed = sprinting ? runSpeed : walkSpeed;

		HUDManager.Instance.staminaMeterValue = stamina;

		lookDelta = GameInputManager.Instance.GetAction("Look").ReadValue<Vector2>();

		Vector2 adjustedVector = lookDelta * sensitivity * Time.timeScale;

		Vector3 scaledDelta = (bool)FlagManager.Instance.GetSetting("invertY") ? Vector2Helper.ToVec3YX(adjustedVector) : Vector2Helper.ToVec3nYX(adjustedVector);

		lookDirection.Set(
			Mathf.Clamp(
				lookDirection.x + scaledDelta.x,
				-80,
				80
			),
			(lookDirection.y + scaledDelta.y)
		);

		cam.transform.parent.eulerAngles = new Vector3(0, lookDirection.y);
		cam.fieldOfView = currentFOV + fovOffset;

		cam.transform.localEulerAngles = new Vector3(lookDirection.x, 0) + (mirrorRequest ? new Vector3(0, 180) : Vector3.zero);

		Vector3 direction = new Vector3(moveDelta.x, 0, moveDelta.y).normalized;
		direction = slippery ? (direction.magnitude > 0 ? direction : movDir) : direction;
		movDir = direction;

		if (!frozen)
			controller.Move((transform.forward * direction.z + transform.right * direction.x) * Time.deltaTime * speed);
		else
			controller.Move(Vector3.zero);

		transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);

		if (HUDManager.Instance.blockGameInput)
			return;

		if (jumpscare && jumpscareEnemy != null)
		{
			cam.clearFlags = CameraClearFlags.SolidColor;
			cam.backgroundColor = Color.black;
			cam.transform.position = jumpscareEnemy.transform.position + (-jumpscareEnemy.forward * 0.4f) + jumpscareEnemy.up * 0.5f;
			cam.farClipPlane = Mathf.Max(cam.farClipPlane - 80 * Time.unscaledDeltaTime, 0f);
			return;
		}

		sprintRequest = GameInputManager.Instance.GetAction("Sprint").IsPressed();
		mirrorRequest = GameInputManager.Instance.GetAction("Mirror").IsPressed();
		moveDelta = GameInputManager.Instance.GetAction("Move").ReadValue<Vector2>();

		mWheel = GameInputManager.Instance.GetAction("MouseWheel").ReadValue<float>();
		int upState = GameInputManager.Instance.GetAction("ItemUp").WasPressedThisFrame() ? 1 : 0;
		int downState = GameInputManager.Instance.GetAction("ItemDown").WasPressedThisFrame() ? -1 : 0;
		bool itemUse = GameInputManager.Instance.GetAction("ItemUse").WasPressedThisFrame();

		UpdateFOV();

		scrollState = upState + downState;

		if (mWheel != 0)
		{
			int inc = (int)Mathf.Clamp(mWheel, -1, 1);
			ChangeSlot(selectedSlot - inc);
		}
		else if (scrollState != 0)
			ChangeSlot(selectedSlot - scrollState);

		if (itemUse)
			UseCurrentItem();
	}

	private void LateUpdate()
	{
		if (HUDManager.Instance.blockGameInput)
			return;

		sensitivity = (float)FlagManager.Instance.GetSetting("sensitivity");

		if ((bool)FlagManager.Instance.GetSetting("lowerSens"))
			sensitivity *= Convert.ToSingle(FlagManager.Instance.gameVars.variables["lowerSensMult"], CultureInfo.InvariantCulture);

		RaycastHit hit;
		bool click = GameInputManager.Instance.GetAction("Click").WasPressedThisFrame();

		if (click && projectile != null)
		{
			projectile.Throw();
			projectile = null;
		}

		Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 0.7f, LayerMask.GetMask("Walls", "Floor", "Ceiling", "NPC", "Interactable"));
		IInteractable itmCtrl;
		if (hit.collider != null)
		{
			hit.collider.TryGetComponent<IInteractable>(out itmCtrl);
			HUDManager.Instance.interactable = itmCtrl != null;
			if (itmCtrl != null && click)
				itmCtrl.Use();
		}
		else
			HUDManager.Instance.interactable = false;
	}

	private void UseCurrentItem()
	{
		BaseItem slot = inventory[selectedSlot];
		if (slot == null) return;
		slot.Use(this);
		if (itmData.useSound.ContainsKey(slot.type))
			audMan.PlayOneShot(itmData.useSound[slot.type]);
		if (!infItems)
		{
			Destroy(slot.gameObject);
			inventory[selectedSlot] = null;
		}
		UpdateSlots();
	}

	public void ClearInventory()
	{
		foreach (BaseItem slot in inventory)
		{
			Destroy(slot);
		}

		for (int i = 0; i < inventory.Length; i++)
		{
			inventory[i] = null;
		}

		UpdateSlots();
	}

	private void FixedUpdate()
	{
		stamina = Mathf.Clamp01(stamina);
		sprinting = sprintRequest && !disableSprint && stamina > 0 && controller.velocity.magnitude > 0;
		if (sprinting)
		{
			staminaRegenTimer = 0f;
			stamina -= staminaDecrement * Time.fixedDeltaTime;
		}
		else
		{
			if (staminaRegenTimer >= staminaRegenTime && stamina < 1)
				stamina += staminaRegen * Time.fixedDeltaTime;
			if (staminaRegenTimer < 2)
				staminaRegenTimer += Time.fixedDeltaTime;
		}
	}

#nullable enable
	public void GiveItem(Type item, ItemController? ctrl)
	{
		int freeSlot = -1;
		for (int i = 0; i < inventory.Length; i++)
		{
			if (inventory[i] == null)
			{
				freeSlot = i;
				break;
			}
		}

		GameObject itmContainer = new GameObject("Item");
		itmContainer.transform.parent = transform;
		BaseItem inst = (BaseItem)itmContainer.AddComponent(item);
		ItemType originalType = inventory[selectedSlot] != null ? inventory[selectedSlot].type : ItemType.SodyPop;

		if (freeSlot == -1)
			inventory[selectedSlot] = inst;
		else
			inventory[freeSlot] = inst;

		if (freeSlot == -1 && ctrl != null)
		{
			ctrl.item = originalType;
			ctrl.Initialize();
		}
		else if (freeSlot != -1 && ctrl != null)
		{
			Destroy(ctrl.gameObject);
		}

		audMan.PlayOneShot(itemPickupSfx);

		UpdateSlots();
	}

	public Quaternion GetLookDirection()
	{
		Vector3 euler = new Vector3(0f, cam.transform.rotation.y, 0f);
		return Quaternion.LookRotation(euler);
	}

	public Vector3 GetLookDirectionVector()
	{
		return new Vector3(0f, 0f, cam.transform.rotation.z).normalized;
	}

	public void ChangeSlot(int slot)
	{
		if (slot > (inventory.Length - 1))
			selectedSlot = slot % inventory.Length;
		else if (slot < 0)
			selectedSlot = inventory.Length + slot;
		else
			selectedSlot = slot;
		UpdateSlots();
	}

	private void UpdateSlots()
	{
		for (int i = 0; i < inventory.Length; i++)
		{
			Image img = slots[i];
			Image slotImg = itemImages[i];
			BaseItem slotItem = inventory[i];
			slotImg.sprite = slotItem == null ? null : itmData.smallSprite[slotItem.type];
			slotImg.color = new Color(1, 1, 1, slotItem == null ? 0 : 1);
			img.color = Color.white;
		}

		Image slot = slots[selectedSlot];
		BaseItem currentItem = inventory[selectedSlot];
		slot.color = selectedColor;
		itemTxt.text = 
			currentItem != null ? 
			LocalizationManager.Instance.GetLocalizedString(itmData.nameKeys[currentItem.type], LangStringType.Menu)[0]
			:
			LocalizationManager.Instance.GetLocalizedString("ITM_Nothing", LangStringType.Menu)[0]
			;
	}

	IEnumerator ExitToMenuDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		EnvironmentController.Instance.ExitToMenu();
	}

	IEnumerator BroadcastTagged(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		EnvironmentController.Instance.gameManager.Broadcast("tagged");
		cam.farClipPlane = 0f;
		cam.nearClipPlane = 10f;
		cam.Render();
	}

	IEnumerator CrashDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
#if UNITY_EDITOR
		EnvironmentController.Instance.ExitToMenu();
#else
		Utils.ForceCrash(ForcedCrashCategory.Abort);
#endif
	}

	private void HandleCollision(Collider other)
	{
		if (jumpscare) return;
		if (leo == null)
			leo = FindObjectOfType<Leo>();
		if (doubt == null)
			doubt = FindObjectOfType<Doubt>();
		if (other.gameObject.CompareTag("Leo") && ((leo != null && leo.canEndGame && leo.canEndGameFun) || (doubt != null && doubt.canEndGame)))
		{
			jumpscare = true;
			if (doubt != null)
				StartCoroutine(BroadcastTagged(3f));
			else
				StartCoroutine(ExitToMenuDelay(3f));

			HUDManager.Instance.Disable();
			cam.farClipPlane = 100;
			jumpscareEnemy = other.transform.Find("Sprite").transform;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		HandleCollision(other);
	}
}

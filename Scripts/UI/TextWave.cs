using System.Collections;
using TMPro;
using UnityEngine;

public class TextWave : MonoBehaviour
{
	private TMP_Text text;
	private Vector2 origPos = Vector2.zero;
	public float speed = 1f;
	public float characterSpacing = 0.5f;
	public float intensity = 10f;

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
		origPos = text.rectTransform.anchoredPosition;

		text.ForceMeshUpdate();
	}

	private void OnEnable()
	{
		StartCoroutine(AnimateVertices());
	}

	IEnumerator AnimateVertices()
	{
		Vector3[] vertices;

		while (true)
		{
			text.ForceMeshUpdate();
			vertices = text.mesh.vertices;

			int characterCount = text.textInfo.characterCount;

			for (int i = 0; i < characterCount; i++)
			{
				TMP_CharacterInfo charInfo = text.textInfo.characterInfo[i];

				if (!charInfo.isVisible)
					continue;

				int vertexIndex = charInfo.vertexIndex;

				float charHeightOffset = Mathf.Sin(Time.unscaledTime * speed + (i * characterSpacing)) * intensity;

				Vector3 offset = new Vector3(0f, charHeightOffset);
				vertices[vertexIndex] += offset;
				vertices[vertexIndex + 1] += offset;
				vertices[vertexIndex + 2] += offset;
				vertices[vertexIndex + 3] += offset;
			}
			text.mesh.vertices = vertices;
			text.UpdateGeometry(text.mesh, 0);
			yield return new WaitForEndOfFrame();
		}
	}
}
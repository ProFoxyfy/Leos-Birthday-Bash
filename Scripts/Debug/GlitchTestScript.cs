using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchTestScript : MonoBehaviour
{
    float strength = 0f;
    IEnumerator Start()
    {
        while (true)
        {
            strength = 10f;
            Shader.SetGlobalInt("_VertexGlitchSeed", Random.Range(0, 300));
            yield return new WaitForSecondsRealtime(10f);
        }
    }

    void FixedUpdate()
    {
        strength *= 0.8f;
        Shader.SetGlobalFloat("_VertexGlitchStrength", strength);
    }
}

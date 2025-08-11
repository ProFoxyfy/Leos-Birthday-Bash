using UnityEngine;
using UnityEngine.UI;

public class DialogueEncrypter : MonoBehaviour
{
    public InputField txt;
    public InputField output;
    public InputField decrypted;
    public UnityEngine.UI.Button btn;

    private void Awake()
    {
        output = GetComponent<InputField>();

        btn.onClick.AddListener(Encrypt);
    }

    private void Encrypt()
    {
        output.text = TextCipher.Encrypt(txt.text);
        decrypted.text = TextCipher.Decrypt(output.text);
    }
}

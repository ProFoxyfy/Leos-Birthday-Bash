using UnityCipher;

public static class TextCipher
{
	public const string key = "PleaseDontDatamineThisThankYou_244";

	public static string Encrypt(string input)
	{
		return Base64.Encode(RijndaelEncryption.Encrypt(input, key));
	}

	public static string Decrypt(string input)
	{
		return RijndaelEncryption.Decrypt(Base64.Decode(input), key);
	}
}
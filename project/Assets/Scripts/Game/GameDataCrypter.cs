using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class GameDataCrypter
{
	private const string AesIV = "jCddaOybW3zEh0Kl";

	private const string AesKey = "giVJrbHRlWBDIggF";

	private static RijndaelManaged CreateAes()
	{
		RijndaelManaged aes = new RijndaelManaged();
		aes.BlockSize = 128;
		aes.KeySize = 128;
		aes.Padding = PaddingMode.Zeros;
		aes.Mode = CipherMode.CBC;
		aes.Key = Encoding.UTF8.GetBytes(AesKey);
		aes.IV = Encoding.UTF8.GetBytes(AesIV);
		return aes;
	}

	public static string Encrypt(string text)
	{
		using (RijndaelManaged aes = CreateAes())
		using (MemoryStream output = new MemoryStream())
		using (CryptoStream crypto = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
		{
			byte[] plain = Encoding.UTF8.GetBytes(text);
			crypto.Write(plain, 0, plain.Length);
			crypto.FlushFinalBlock();
			return Convert.ToBase64String(output.ToArray());
		}
	}

	public static string Decrypt(string cryptedText)
	{
		byte[] encrypted = Convert.FromBase64String(cryptedText);
		byte[] plain = new byte[encrypted.Length];
		using (RijndaelManaged aes = CreateAes())
		using (CryptoStream crypto = new CryptoStream(new MemoryStream(encrypted), aes.CreateDecryptor(), CryptoStreamMode.Read))
		{
			crypto.Read(plain, 0, plain.Length);
		}
		return Encoding.UTF8.GetString(plain);
	}
}

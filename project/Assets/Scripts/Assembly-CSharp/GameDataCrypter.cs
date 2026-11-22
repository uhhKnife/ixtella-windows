using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class GameDataCrypter
{
	private const string AesIV = "jCddaOybW3zEh0Kl";

	private const string AesKey = "giVJrbHRlWBDIggF";

	public static string Encrypt(string text)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.BlockSize = 128;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.Padding = PaddingMode.Zeros;
		rijndaelManaged.Mode = CipherMode.CBC;
		rijndaelManaged.Key = Encoding.UTF8.GetBytes("giVJrbHRlWBDIggF");
		rijndaelManaged.IV = Encoding.UTF8.GetBytes("jCddaOybW3zEh0Kl");
		ICryptoTransform transform = rijndaelManaged.CreateEncryptor();
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.FlushFinalBlock();
		byte[] inArray = memoryStream.ToArray();
		return Convert.ToBase64String(inArray);
	}

	public static string Decrypt(string cryptedText)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.BlockSize = 128;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.Padding = PaddingMode.Zeros;
		rijndaelManaged.Mode = CipherMode.CBC;
		rijndaelManaged.Key = Encoding.UTF8.GetBytes("giVJrbHRlWBDIggF");
		rijndaelManaged.IV = Encoding.UTF8.GetBytes("jCddaOybW3zEh0Kl");
		ICryptoTransform transform = rijndaelManaged.CreateDecryptor();
		byte[] array = Convert.FromBase64String(cryptedText);
		byte[] array2 = new byte[array.Length];
		MemoryStream stream = new MemoryStream(array);
		CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
		cryptoStream.Read(array2, 0, array2.Length);
		return Encoding.UTF8.GetString(array2);
	}
}

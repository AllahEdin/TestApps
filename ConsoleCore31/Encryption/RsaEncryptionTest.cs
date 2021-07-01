using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleCore31.Encryption
{
	public class RsaEncryptionTest
	{
        public RsaEncryptionTest()
        {
	        string serializedPrivate = "";
            string serializedPublic = "";
            byte[] encodedData;

            {
	            using (var fs = new StreamReader(File.Open("Encryption/MASTERKEY.txt", FileMode.Open, FileAccess.Read)))
	            {
		            serializedPrivate = fs.ReadLine();
		            serializedPublic = fs.ReadLine();
	            }

	            using (var fs = File.OpenRead("Encryption/ENCODED_DATA.txt"))
	            {
		            encodedData = new byte[fs.Length];

		            var task = fs.ReadAsync(encodedData, 0, (int) fs.Length);
		            task.Wait();
	            }
	        }

            RSAParameters masterKeyPrivate =
	            JsonConvert.DeserializeObject<RSAParameters>(serializedPrivate);

            RSAParameters masterKeyPublic =
	            JsonConvert.DeserializeObject<RSAParameters>(serializedPublic);

            //EncodeDecode("kek", masterKeyPublic, masterKeyPrivate);

            RSADecrypt(encodedData, masterKeyPrivate, true);
        }

        public void EncodeDecode(string data, RSAParameters masterKeyPublic, RSAParameters masterKeyPrivate)
		{
			UnicodeEncoding ByteConverter = new UnicodeEncoding();

			byte[] dataToEncrypt = ByteConverter.GetBytes(data);

			byte[] encryptedData;
	        byte[] decryptedData;

			using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
	        {
		        Console.WriteLine("Plaintext: {0}", ByteConverter.GetString(dataToEncrypt));

		        encryptedData = RSAEncrypt(dataToEncrypt, masterKeyPublic, false);

		        Console.WriteLine("Encrypted plaintext: {0}", ByteConverter.GetString(encryptedData));

		        decryptedData = RSADecrypt(encryptedData, masterKeyPrivate, false);

		        Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
	        }
		}

        public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
	        byte[] encryptedData;

	        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
	        {
		        RSA.ImportParameters(RSAKeyInfo);

		        encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
	        }
	        return encryptedData;
        }

        public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
	        byte[] decryptedData;

	        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
	        {
		        RSA.ImportParameters(RSAKeyInfo);

		        decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
	        }
	        return decryptedData;
        }

        public (RSAParameters publicRsa, RSAParameters privateRsa) GetParams()
        {
	        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
	        {
		        RSAParameters RSAParamsPublic = RSA.ExportParameters(false);

		        RSAParameters RSAParamsPrivate = RSA.ExportParameters(true);

		        return (RSAParamsPublic, RSAParamsPrivate);
	        }
        }
    }
}

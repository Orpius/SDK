//using System;
//using System.Security.Cryptography;

//namespace Orpius.Platform.Tooling.ToolRegistration
//{
//	/// <summary>
//	/// A one-shot access token. After use, it will be zeroed out.
//	/// </summary>
//	public sealed class AccessKeyContainer : IDisposable
//	{
//		readonly byte[] tokenBytes;
//		bool destroyed;

//		public AccessKeyContainer(byte[] data)
//		{
//			tokenBytes = data ?? throw new ArgumentNullException(nameof(data));
//		}

//		/// <summary>
//		/// Call this to hand the raw bytes to the SDK.  After this point, the SDK
//		/// will zero out the bytes (and Dispose clears them too).
//		/// </summary>
//		public byte[] GetRawBytes()
//		{
//			if (destroyed)
//			{
//				throw new ObjectDisposedException(nameof(AccessKeyContainer));
//			}

//			return tokenBytes;
//		}

//		public void Dispose()
//		{
//			if (!destroyed)
//			{
//				CryptographicOperations.ZeroMemory(tokenBytes);
//				destroyed = true;
//			}
//		}
//	}
//}
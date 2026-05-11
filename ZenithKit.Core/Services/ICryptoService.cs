using System.Security.Cryptography;

namespace ZenithKit.Core.Services;

public interface ICryptoService
{
    /// <summary>
    /// Derive key material via PBKDF2.
    /// </summary>
    byte[] DeriveKey(string password, byte[] salt, int iterations = 200_000, int keyBytes = 32, HashAlgorithmName? hash = null);

    /// <summary>
    /// Encrypt plaintext with AES-256-GCM.
    /// associatedData optional; pass null for none.
    /// </summary>
    byte[] Encrypt(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, byte[]? associatedData = null);

    /// <summary>
    /// Decrypt ciphertext with AES-256-GCM.
    /// associatedData optional; pass null for none.
    /// </summary>
    byte[] Decrypt(ReadOnlySpan<byte> ciphertextWithTag, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, byte[]? associatedData = null);
}

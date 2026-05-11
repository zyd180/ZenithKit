using System.Security.Cryptography;

namespace ZenithKit.Core.Services;

public sealed class CryptoService : ICryptoService
{
    public byte[] DeriveKey(string password, byte[] salt, int iterations = 200_000, int keyBytes = 32, HashAlgorithmName? hash = null)
    {
        if (salt == null || salt.Length == 0) throw new ArgumentException("Salt required", nameof(salt));
        hash ??= HashAlgorithmName.SHA256;
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, hash.Value);
        return pbkdf2.GetBytes(keyBytes);
    }

    public byte[] Encrypt(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, byte[]? associatedData = null)
    {
        if (key.Length != 32) throw new ArgumentException("AES-256 key must be 32 bytes", nameof(key));
        if (nonce.Length != 12) throw new ArgumentException("GCM nonce must be 12 bytes", nameof(nonce));

        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[16];

        using var aesGcm = new AesGcm(key, tag.Length);
        aesGcm.Encrypt(nonce, plaintext, ciphertext, tag, associatedData ?? ReadOnlySpan<byte>.Empty);

        // return ciphertext||tag
        return Concat(ciphertext, tag);
    }

    public byte[] Decrypt(ReadOnlySpan<byte> ciphertextWithTag, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce, byte[]? associatedData = null)
    {
        if (key.Length != 32) throw new ArgumentException("AES-256 key must be 32 bytes", nameof(key));
        if (nonce.Length != 12) throw new ArgumentException("GCM nonce must be 12 bytes", nameof(nonce));
        if (ciphertextWithTag.Length < 16) throw new ArgumentException("Ciphertext+tag too short", nameof(ciphertextWithTag));

        int cipherLen = ciphertextWithTag.Length - 16;
        var ciphertext = ciphertextWithTag[..cipherLen];
        var tag = ciphertextWithTag[cipherLen..];

        byte[] plaintext = new byte[cipherLen];
        using var aesGcm = new AesGcm(key, tag.Length);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext, associatedData ?? ReadOnlySpan<byte>.Empty);
        return plaintext;
    }

    private static byte[] Concat(ReadOnlySpan<byte> first, ReadOnlySpan<byte> second)
    {
        byte[] result = new byte[first.Length + second.Length];
        first.CopyTo(result);
        second.CopyTo(result.AsSpan(first.Length));
        return result;
    }
}

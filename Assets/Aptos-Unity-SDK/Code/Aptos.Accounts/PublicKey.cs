using Aptos.HdWallet.Utils;
using Chaos.NaCl;
using NBitcoin.DataEncoders;
using System;

namespace Aptos.Accounts
{
    /// <summary>
    /// Represents a 32-byte public key.
    /// </summary>
    public class PublicKey
    {
        /// <summary>
        /// Public key length.
        /// </summary>
        public const int KeyLength = 32;

        /// <summary>
        /// Hex string representation of public key.
        /// </summary>
        private string _key;

        /// <summary>
        /// Byte representation of public key.
        /// </summary>
        private byte[] _keyBytes;

        /// <summary>
        /// Base 58 string representation of public key.
        /// </summary>
        private string _keyBase58;

        /// <summary>
        /// Byte representation of public key.
        /// </summary>
        private byte[] _keyBytesBase58;

        /// <summary>
        /// The key as a hexadecimal string
        /// </summary>
        public string Key
        {
            get
            {
                if(_key == null && _keyBytes != null)
                {
                    string addressHex = CryptoBytes.ToHexStringLower(_keyBytes);
                    _key = "0x" + addressHex;
                }
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        /// <summary>
        /// The key in bytes.
        /// </summary>
        public byte[] KeyBytes
        {
            get
            {
                if (_keyBytes == null && _key != null)
                {
                    string key = _key;
                    if (_key[0..2].Equals("0x")) { key = _key[2..]; }
                    _keyBytes = key.HexStringToByteArray();
                }
                return _keyBytes;
            }

            set
            {
                _keyBytes = value;
            }
        }

        /// <summary>
        /// The key as base-58 encoded string
        /// Base58 encoding scheme is used to facilitate switching 
        /// from byte to alphanumeric text format (ASCII).
        /// </summary>
        public string KeyBase58
        {
            get
            {
                if (_keyBase58 == null && _keyBytesBase58 != null)
                {
                    _keyBase58 = Encoders.Base58.EncodeData(_keyBytesBase58);
                }
                return _keyBase58;
            }
            
            set
            {
                _keyBase58 = value;
            }
        }

        /// <summary>
        /// The public key bytes.
        /// </summary>
        public byte[] KeyBytesFromBase58
        {
            get
            {
                if (_keyBytesBase58 == null && _keyBase58 != null)
                {
                    _keyBytesBase58 = Encoders.Base58.DecodeData(_keyBase58);
                }
                return _keyBytesBase58;
            }
            set
            {
                _keyBytesBase58 = value;
            }
        }

        /// <summary>
        /// Initializes the PublicKey object with a given byte array.
        /// </summary>
        /// <param name="publicKey">The public key as byte array.</param>
        public PublicKey(byte[] publicKey)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));
            if (publicKey.Length != KeyLength)
                throw new ArgumentException("Invalid key length: ", nameof(publicKey));
            KeyBytes = new byte[KeyLength];
            Array.Copy(publicKey, KeyBytes, KeyLength);
        }

        /// <summary>
        /// Initializes the PublicKey object with a given hexadecimal representation of public .
        /// </summary>
        /// <param name="key">The public key as a hexadecimal string.</param> 
        public PublicKey(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        /// <summary>
        /// Initialize the PublicKey object from the given string.
        /// </summary>
        /// <param name="publicKey">The public key as a byte array.</param>
        public PublicKey(ReadOnlySpan<byte> publicKey)
        {
            if (publicKey.Length != KeyLength)
                throw new ArgumentException("Invalid key length: ", nameof(publicKey));
            KeyBytes = new byte[KeyLength];
            publicKey.CopyTo(KeyBytes.AsSpan());
        }

        /// <summary>
        /// Verify a signed message using the current public key.
        /// </summary>
        /// <param name="message">Message that was signed.</param>
        /// <param name="signature">The signature from the message.</param>
        /// <returns></returns>
        public bool Verify(byte[] message, byte[] signature)
        {
            return Ed25519.Verify(signature, message, KeyBytes);
        }

        /// <summary>
        /// Check if PubliKey is a valid on the Ed25519 curve.
        /// </summary>
        /// <returns>Returns true if public key is on the curve.</returns>
        public bool IsOnCurve()
        {
            return KeyBytes.IsOnCurve();
        }

        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object obj)
        {
            if (obj is PublicKey publicKey)
            {
                return publicKey.Key.Equals(Key);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return Key;
        }

        /// <summary>
        /// Compares two public key objects.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>True if public keys are equal. False is public keys are not equal.</returns>
        public static bool operator ==(PublicKey lhs, PublicKey rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PublicKey lhs, PublicKey rhs) {
            return lhs == rhs;
        } 

        /// <summary>
        /// Convert a PublicKey object to Base58 encoded string representatio public key.
        /// </summary>
        /// <param name="publicKey">The PublicKey object.</param>
        /// <returns>Base58 encoded string representing the public key.</returns>
        public static implicit operator string(PublicKey publicKey)
        {
            return publicKey.Key;
        }

        /// <summary>
        /// Convert Base58 encoded string of a public key to PublicKey object.
        /// </summary>
        /// <param name="publicKey">Base58 encoded string representing a public key.</param>
        /// <returns>PublicKey object.</returns>
        public static explicit operator PublicKey(string publicKey)
        {
            return new PublicKey(publicKey);
        }

        /// <summary>
        /// Convert a PublicKey object to a byte array representation of a public key.
        /// </summary>
        /// <param name="publicKey">The PublicKey object.</param>
        /// <returns>Public key as a byte array.</returns>
        public static implicit operator byte[](PublicKey publicKey) 
        {
            return publicKey.KeyBytes;
        }

        /// <summary>
        /// Convert byte array representation of a public key to a PublicKey object.
        /// </summary>
        /// <param name="keyBytes">The public key as a byte array.</param>
        /// <returns>PublicKey object.</returns>
        public static explicit operator PublicKey(byte[] keyBytes)
        {
            return new PublicKey(keyBytes);
        }
    }
}
﻿using System;
using System.IO;
using UnityEngine;

namespace Framework.Runtime
{

    /// <summary>
    /// 压缩解压缩相关的实用函数。
    /// </summary>
    public static partial class Zip
    {
        private static IZipHelper s_ZipHelper = null;

        /// <summary>
        /// 设置压缩解压缩辅助器。
        /// </summary>
        /// <param name="zipHelper">要设置的压缩解压缩辅助器。</param>
        public static void SetZipHelper(IZipHelper zipHelper)
        {
            s_ZipHelper = zipHelper;
        }

        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流。</param>
        /// <returns>压缩后的数据的二进制流。</returns>
        public static byte[] Compress(byte[] bytes)
        {
            if (bytes == null)
            {
                Debug.LogError("Bytes is invalid.");
            }

            return Compress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流。</param>
        /// <param name="compressedStream">压缩后的数据的二进制流。</param>
        /// <returns>是否压缩数据成功。</returns>
        public static bool Compress(byte[] bytes, Stream compressedStream)
        {
            if (bytes == null)
            {
                Debug.LogError("Bytes is invalid.");
            }

            return Compress(bytes, 0, bytes.Length, compressedStream);
        }

        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流。</param>
        /// <param name="offset">要压缩的数据的二进制流的偏移。</param>
        /// <param name="length">要压缩的数据的二进制流的长度。</param>
        /// <returns>压缩后的数据的二进制流。</returns>
        public static byte[] Compress(byte[] bytes, int offset, int length)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                if (Compress(bytes, offset, length, compressedStream))
                {
                    return compressedStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流。</param>
        /// <param name="offset">要压缩的数据的二进制流的偏移。</param>
        /// <param name="length">要压缩的数据的二进制流的长度。</param>
        /// <param name="compressedStream">压缩后的数据的二进制流。</param>
        /// <returns>是否压缩数据成功。</returns>
        public static bool Compress(byte[] bytes, int offset, int length, Stream compressedStream)
        {
            if (s_ZipHelper == null)
            {
                Debug.LogError("Zip helper is invalid.");
                return false;
            }

            if (bytes == null)
            {
                Debug.LogError("Bytes is invalid.");
                return false;
            }

            if (offset < 0 || length <= 0 || offset + length > bytes.Length)
            {
                Debug.LogError("Offset or length is invalid.");
                return false;
            }

            if (compressedStream == null)
            {
                Debug.LogError("Compressed stream is invalid.");
                return false;
            }

            try
            {
                return s_ZipHelper.Compress(bytes, offset, length, compressedStream);
            }
            catch (Exception exception)
            {
                //if (exception is GameFrameworkException)
                //{
                //    throw;
                //}

                Debug.LogError(string.Format("Can not compress with exception '{0}'.", exception.ToString()));
            }
            return false;
        }

        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="stream">要压缩的数据的二进制流。</param>
        /// <returns>压缩后的数据的二进制流。</returns>
        public static byte[] Compress(Stream stream)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                if (Compress(stream, compressedStream))
                {
                    return compressedStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="stream">要压缩的数据的二进制流。</param>
        /// <param name="compressedStream">压缩后的数据的二进制流。</param>
        /// <returns>是否压缩数据成功。</returns>
        public static bool Compress(Stream stream, Stream compressedStream)
        {
            if (s_ZipHelper == null)
            {
                Debug.LogError("Zip helper is invalid.");
                return false;
            }

            if (stream == null)
            {
                Debug.LogError("Stream is invalid.");
                return false;
            }

            if (compressedStream == null)
            {
                //throw new GameFrameworkException("Compressed stream is invalid.");
                return false;
            }

            try
            {
                return s_ZipHelper.Compress(stream, compressedStream);
            }
            catch (Exception exception)
            {
                //if (exception is GameFrameworkException)
                //{
                //    throw;
                //}

                Debug.LogError(string.Format("Can not compress with exception '{0}'.", exception.ToString()));
            }
            return false;
        }

        /// <summary>
        /// 解压缩数据。
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流。</param>
        /// <returns>解压缩后的数据的二进制流。</returns>
        public static byte[] Decompress(byte[] bytes)
        {
            if (bytes == null)
            {
                Debug.LogError("Bytes is invalid.");
                return null;
            }

            return Decompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 解压缩数据。
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流。</param>
        /// <param name="decompressedStream">解压缩后的数据的二进制流。</param>
        /// <returns>是否解压缩数据成功。</returns>
        public static bool Decompress(byte[] bytes, Stream decompressedStream)
        {
            if (bytes == null)
            {
                Debug.LogError("Bytes is invalid.");
                return false;
            }

            return Decompress(bytes, 0, bytes.Length, decompressedStream);
        }

        /// <summary>
        /// 解压缩数据。
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流。</param>
        /// <param name="offset">要解压缩的数据的二进制流的偏移。</param>
        /// <param name="length">要解压缩的数据的二进制流的长度。</param>
        /// <returns>解压缩后的数据的二进制流。</returns>
        public static byte[] Decompress(byte[] bytes, int offset, int length)
        {
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                if (Decompress(bytes, offset, length, decompressedStream))
                {
                    return decompressedStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 解压缩数据。
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流。</param>
        /// <param name="offset">要解压缩的数据的二进制流的偏移。</param>
        /// <param name="length">要解压缩的数据的二进制流的长度。</param>
        /// <param name="decompressedStream">解压缩后的数据的二进制流。</param>
        /// <returns>是否解压缩数据成功。</returns>
        public static bool Decompress(byte[] bytes, int offset, int length, Stream decompressedStream)
        {
            if (s_ZipHelper == null)
            {
                Debug.LogError("Zip helper is invalid.");
            }

            if (bytes == null)
            {
                Debug.LogError("Bytes is invalid.");
            }

            if (offset < 0 || length <= 0 || offset + length > bytes.Length)
            {
                Debug.LogError("Offset or length is invalid.");
            }

            if (decompressedStream == null)
            {
                Debug.LogError("Decompressed stream is invalid.");
            }

            try
            {
                return s_ZipHelper.Decompress(bytes, offset, length, decompressedStream);
            }
            catch (Exception exception)
            {
                //if (exception is GameFrameworkException)
                //{
                //    throw;
                //}

                Debug.LogError(string.Format("Can not decompress with exception '{0}'.", exception.ToString()));
            }
            return false;
        }

        /// <summary>
        /// 解压缩数据。
        /// </summary>
        /// <param name="stream">要解压缩的数据的二进制流。</param>
        /// <returns>是否解压缩数据成功。</returns>
        public static byte[] Decompress(Stream stream)
        {
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                if (Decompress(stream, decompressedStream))
                {
                    return decompressedStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 解压缩数据。
        /// </summary>
        /// <param name="stream">要解压缩的数据的二进制流。</param>
        /// <param name="decompressedStream">解压缩后的数据的二进制流。</param>
        /// <returns>是否解压缩数据成功。</returns>
        public static bool Decompress(Stream stream, Stream decompressedStream)
        {
            if (s_ZipHelper == null)
            {
                Debug.LogError("Zip helper is invalid.");
            }

            if (stream == null)
            {
                Debug.LogError("Stream is invalid.");
            }

            if (decompressedStream == null)
            {
                Debug.LogError("Decompressed stream is invalid.");
            }

            try
            {
                return s_ZipHelper.Decompress(stream, decompressedStream);
            }
            catch (Exception exception)
            {
                //if (exception is GameFrameworkException)
                //{
                //    throw;
                //}

                Debug.LogError(string.Format("Can not decompress with exception '{0}'.", exception.ToString()));
            }
            return false;
        }
    }
}

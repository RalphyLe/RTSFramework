﻿namespace Framework.Editor
{
    /// <summary>
    /// 资源加载方式类型。
    /// </summary>
    public enum LoadType : byte
    {
        /// <summary>
        /// 使用文件方式加载。
        /// </summary>
        LoadFromFile = 0,

        /// <summary>
        /// 使用内存方式加载。
        /// </summary>
        LoadFromMemory,

        /// <summary>
        /// 使用内存快速解密方式加载。
        /// </summary>
        LoadFromMemoryAndQuickDecrypt,

        /// <summary>
        /// 使用内存解密方式加载。
        /// </summary>
        LoadFromMemoryAndDecrypt,

        /// <summary>
        /// 使用二进制方式加载。
        /// </summary>
        LoadFromBinary,

        /// <summary>
        /// 使用二进制快速解密方式加载。
        /// </summary>
        LoadFromBinaryAndQuickDecrypt,

        /// <summary>
        /// 使用二进制解密方式加载。
        /// </summary>
        LoadFromBinaryAndDecrypt
    }
}
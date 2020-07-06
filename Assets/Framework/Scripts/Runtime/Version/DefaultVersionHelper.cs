using UnityEngine;
using System.Collections;

namespace Framework.Runtime
{
    public class DefaultVersionHelper : Version.IVersionHelper
    {

        /// <summary>
        /// 获取游戏版本号。
        /// </summary>
        public string GameVersion
        {
            get
            {
                return Application.version;
            }
        }

        /// <summary>
        /// 获取内部游戏版本号。
        /// </summary>
        public int InternalGameVersion
        {
            get
            {
                return 0;
            }
        }
    } 
}

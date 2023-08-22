using System;

namespace PKFramework.Runtime.Base
{
    [Serializable]
    public class GameVersion
    {
        //打包的正式版本为1，其他为测试版本
        //巨大版本号
        private int major;
        //整包更新版本号
        private int minor;
        //服务器协议版本号
        private int build;
        //编译版本号\热更版本号
        private int revision;
        
        public int Major => major;

        public int Minor => minor;

        public int Build => build;

        public int Revision => revision;
        
        public GameVersion(int major, int minor, int build, int revision)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.revision = revision;
        }
    }
}
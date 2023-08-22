using PKFramework.Core.Net.Data;

namespace PKFramework.Core.Data
{
    public class DataManager
    {
        private DataCenter _userData;
        private DataCenter _featureData;

        public void SetUserData(PackBase pack)
        {
            if (_userData == null)
            {
                _userData = new DataCenter("userdata", pack);
            }
        }

        public void SetFeatureData(PackBase pack)
        {
            if (_featureData == null)
            {
                _featureData = new DataCenter("feadata", pack);
            }
        }
    }
}
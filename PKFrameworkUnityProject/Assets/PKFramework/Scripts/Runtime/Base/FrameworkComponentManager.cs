

using PKFramework.Runtime.Fsm;
using PKFramework.Runtime.Singleton;

namespace PKFramework.Runtime.Base
{
    public class FrameworkComponentManager : MonoSingleton<FrameworkComponentManager>
    {
        public static IFrameworkComponent GetFrameworkComponent()
        {
            return null;
        }
    }
}
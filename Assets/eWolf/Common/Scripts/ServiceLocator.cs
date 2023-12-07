namespace eWolf.Common
{
    public class ServiceLocator : ServiceLocatorBase
    {
        public static ServiceLocatorBase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }
                return _instance;
            }
        }
    }
}
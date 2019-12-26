using System.Windows.Forms;

namespace Elmanager.Forms
{
    internal class MaybeOpened<T> where T : Form, new()
    {
        private T _instance;

        public T ExistingInstance
        {
            get
            {
                if (_instance != null && !_instance.IsDisposed)
                {
                    return _instance;
                }

                return null;
            }
        }

        public T Instance => ExistingInstance ?? (_instance = new T());
    }
}
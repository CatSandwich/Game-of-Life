using UnityEngine;

namespace InfoObjects
{
    public class InfoObject<T> : MonoBehaviour
    {
        private T _data;

        public void Set(T data)
        {
            _data = data;
            DontDestroyOnLoad(gameObject);
        }
        
        public T Consume()
        {
            Destroy(gameObject);
            return _data;
        }
    }
}

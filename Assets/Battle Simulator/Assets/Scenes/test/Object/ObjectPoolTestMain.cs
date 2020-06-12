using Framework.Runtime;
using UnityEngine;

public class ObjectPoolTestMain : MonoBehaviour
{

    private ObjectPool<TestObject> m_testPool;

    private void Start()
    {
        //创建对象池
        ObjectPoolManager m_objectPoolManager = FrameworkEntry.Instance.GetManager<ObjectPoolManager>();
        m_testPool = m_objectPoolManager.CreateObjectPool<TestObject>();

        //注册对象
        TestObject testObject = new TestObject("hello ObjectPool", "test1");
        m_testPool.Register(testObject, false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TestObject testObject = m_testPool.Spawn("test1");
            Debug.Log(testObject.Target);
            m_testPool.Unspawn(testObject.Target);
        }
    }
}

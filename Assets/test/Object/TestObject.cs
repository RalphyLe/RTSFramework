using Framework.Runtime;

public class TestObject : ObjectBase
{
    public TestObject(object target, string name = "") : base(target, name)
    {
    }

    public override void Release()
    {

    }
}

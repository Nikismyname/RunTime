using System.Reflection;

public class MyMethodInfo
{
    public int ID { get; set; }

    public MethodInfo Info { get; set; }

    public MyMethodInfo(int iD, MethodInfo info)
    {
        ID = iD;
        Info = info;
    }
}


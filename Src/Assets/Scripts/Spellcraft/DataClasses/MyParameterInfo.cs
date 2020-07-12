using System.Reflection;

public class MyParameterInfo
{
    public int ID;
    public ParameterInfo Info { get; set; }

    public MyParameterInfo(int iD, ParameterInfo info)
    {
        ID = iD;
        Info = info;
    }
}


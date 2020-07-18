using System;

public class VariableInput
{
    public Type Type { get; set; }

    public string Name { get; set; }

    public VariableInput(Type type, string name)
    {
        Type = type;
        Name = name;
    }
}
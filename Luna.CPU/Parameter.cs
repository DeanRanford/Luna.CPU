namespace Luna.CPU;

public enum ParameterType
{
    Constant,
    Location,
    Indirect
}

public enum ParameterConstraint
{
    Constant,
    Memory,
    ConstantOrMemory
}

public class Parameter(ParameterType type, int value)
{

    public ParameterType Type { get; set; } = type;
    public int Value { get; set; } = value;
}

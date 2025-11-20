namespace Luna.CPU;

public interface IDefinition
{
    public string Token { get; }
    public List<ParameterConstraint> ParameterConstraints { get; }
    public Func<ICPU, Instruction, bool> Func { get; }
}

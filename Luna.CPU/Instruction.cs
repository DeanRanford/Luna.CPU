namespace Luna.CPU;

public class Instruction(IDefinition definition, List<Parameter> parameters)
{
    public IDefinition Definition { get; set; } = definition;
    public List<Parameter> Parameters = parameters;
}

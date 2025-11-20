namespace Luna.CPU;

public interface ICPU
{
    public IMemory Memory { get; set; }
    public List<Instruction> Instructions { get; set; }
    public StepResult Step();
    public ParseResult Parse(string[] code);
    public bool SetParameterValue(Parameter parameter, int value);
    public bool GetParameterValue(Parameter parameter, out int value);
    public void SetPointer(int value);
}

namespace Luna.CPU;

using System.Text.RegularExpressions;

public partial class CPU(IMemory memory) : ICPU
{
    public int Pointer { get; set; }
    public IMemory Memory { get; set; } = memory;
    public List<Instruction> Instructions { get; set; } = [];
    public List<IDefinition> Definitions { get; set; } = [];
    public string Commenter { get; set; } = "--";

    private sealed class DummyDef : IDefinition
    {
        public string Token { get; set; } = "NULL";
        public List<ParameterConstraint> ParameterConstraints { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<ICPU, Instruction, bool> Func { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    private DummyDef DummyDefRef { get; } = new DummyDef();
    private Parameter DummyParameter { get; } = new Parameter(ParameterType.Constant, 0);

    private string ProcessComments(string line)
    {
        var commentIndex = line.IndexOf(this.Commenter);
        if (commentIndex != -1)
        {
            line = line[..commentIndex].TrimEnd();
        }

        return line.Trim();
    }

    private static string[] GetInstructionParts(string line) => [.. line.Split(' ').Where(v => v != "")];

    private bool GetDefinition(string token, out IDefinition definition)
    {
        definition = this.Definitions.Find(v => v.Token == token) ?? this.DummyDefRef;
        return definition != this.DummyDefRef;
    }

    public bool SetParameterValue(Parameter parameter, int value)
    {
        switch (parameter.Type)
        {
            case ParameterType.Constant:
                parameter.Value = value;
                return true;
            case ParameterType.Location:
                return this.Memory.SetAt(parameter.Value, value);
            case ParameterType.Indirect:
                return this.Memory.SetAtIndirect(parameter.Value, value);
            default:
                break;
        }
        return false;
    }

    public static bool ValidateParameter(Parameter parameter, ParameterConstraint constraint) => constraint switch
    {
        ParameterConstraint.Constant => parameter.Type == ParameterType.Constant,
        ParameterConstraint.Memory => parameter.Type != ParameterType.Constant,
        ParameterConstraint.ConstantOrMemory => true,
        _ => false,
    };

    public bool GetParameterValue(Parameter parameter, out int value)
    {
        value = 0;
        switch (parameter.Type)
        {
            case ParameterType.Constant:
                value = parameter.Value;
                return true;
            case ParameterType.Location:
                return this.Memory.GetAt(parameter.Value, out value);
            case ParameterType.Indirect:
                return this.Memory.GetAtIndirect(parameter.Value, out value);
            default:
                break;
        }
        return false;
    }

    public bool GetParameter(string element, out Parameter parameter)
    {
        parameter = this.DummyParameter;
        var location = element.StartsWith("#");
        var indirect = element.StartsWith(">");
        element = element.Replace("#", "").Replace(">", "");
        if (!MyRegex().IsMatch(element))
        {
            return false;
        }

        if (!int.TryParse(element, out var value))
        {
            return false;
        }

        parameter = new Parameter(location
            ? ParameterType.Location
            : indirect
                ? ParameterType.Indirect
                : ParameterType.Constant, value);
        return true;
    }

    public void SetPointer(int value) => this.Pointer = value;

    public ParseResult Parse(string[] code)
    {
        this.Instructions.Clear();

        for (var lineIndex = 0; lineIndex < code.Length; lineIndex++)
        {
            var line = this.ProcessComments(code[lineIndex]);
            if (line.Length == 0)
            {
                continue;
            }

            var intructionParts = GetInstructionParts(line);
            var token = intructionParts[0];
            var definitionFound = this.GetDefinition(token, out var definition);
            if (!definitionFound)
            {
                return new ParseResult(false, ParseResultComment.INVALIDINSTRUCTION, lineIndex);
            }

            var parameterCount = definition.ParameterConstraints.Count;
            if (parameterCount != intructionParts.Length - 1)
            {
                return new ParseResult(false, ParseResultComment.INVALIDPARAMETERS, lineIndex);
            }

            var parameters = new List<Parameter>();
            for (var paramIndex = 1; paramIndex <= parameterCount; paramIndex++)
            {
                var foundParameter = this.GetParameter(intructionParts[paramIndex], out var parameter);
                if (!foundParameter)
                {
                    return new ParseResult(false, ParseResultComment.INVALIDPARAMETER, lineIndex);
                }
                if (!ValidateParameter(parameter, definition.ParameterConstraints[paramIndex - 1]))
                {
                    return new ParseResult(false, ParseResultComment.INVALIDPARAMETER, lineIndex);
                }
                parameters.Add(parameter);
            }
            this.Instructions.Add(new Instruction(definition, parameters));
        }
        var success = this.Instructions.Count != 0;
        return new ParseResult(success, success ? ParseResultComment.OK : ParseResultComment.NOCODE, this.Instructions.Count);

    }


    public StepResult Step()
    {
        if (this.Pointer < 0 || this.Pointer >= this.Instructions.Count)
        {
            return new StepResult(false, StepResultComment.ENDOFCODE, this.Pointer);
        }

        var instruction = this.Instructions[this.Pointer];
        var result = instruction.Definition.Func(this, instruction);
        if (!result)
        {
            return new StepResult(false, StepResultComment.NOINSTRUCTION, this.Pointer);
        }
        this.Pointer++;
        return new StepResult(true, StepResultComment.OK, this.Pointer);
    }

    [GeneratedRegex(@"^-?\d+$")]
    private static partial Regex MyRegex();
}

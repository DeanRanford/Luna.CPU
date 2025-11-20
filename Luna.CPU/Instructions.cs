namespace Luna.CPU;

public class LBL : IDefinition
{
    public string Token => "LBL";
    public List<ParameterConstraint> ParameterConstraints => [ParameterConstraint.Constant];
    public Func<ICPU, Instruction, bool> Func => (cpu, instruction) => true;
}

public class SET : IDefinition
{
    public string Token => "SET";

    public List<ParameterConstraint> ParameterConstraints => [ParameterConstraint.Memory, ParameterConstraint.ConstantOrMemory];

    public Func<ICPU, Instruction, bool> Func => (cpu, instruction) =>
    {
        var p2 = cpu.GetParameterValue(instruction.Parameters[1], out var p2Value);
        if (!p2)
        {
            return false;
        }

        return cpu.SetParameterValue(instruction.Parameters[0], p2Value);
    };
}

public class ADD : IDefinition
{
    public string Token => "ADD";

    public List<ParameterConstraint> ParameterConstraints => [ParameterConstraint.Memory, ParameterConstraint.ConstantOrMemory];

    public Func<ICPU, Instruction, bool> Func => (cpu, instruction) =>
    {
        var p1 = cpu.GetParameterValue(instruction.Parameters[0], out var p1Value);
        var p2 = cpu.GetParameterValue(instruction.Parameters[1], out var p2Value);
        if (!p1 || !p2)
        {
            return false;
        }

        return cpu.SetParameterValue(instruction.Parameters[0], p1Value + p2Value);
    };
}

public class SUB : IDefinition
{
    public string Token => "SUB";

    public List<ParameterConstraint> ParameterConstraints => [ParameterConstraint.Memory, ParameterConstraint.ConstantOrMemory];

    public Func<ICPU, Instruction, bool> Func => (cpu, instruction) =>
    {
        var p1 = cpu.GetParameterValue(instruction.Parameters[0], out var p1Value);
        var p2 = cpu.GetParameterValue(instruction.Parameters[1], out var p2Value);
        if (!p1 || !p2)
        {
            return false;
        }

        return cpu.SetParameterValue(instruction.Parameters[0], p1Value - p2Value);
    };
}


public class JMP : IDefinition
{
    public string Token => "JMP";

    public List<ParameterConstraint> ParameterConstraints => [ParameterConstraint.Constant];

    public Func<ICPU, Instruction, bool> Func => (cpu, instruction) =>
    {
        var p1 = cpu.GetParameterValue(instruction.Parameters[0], out var p1Value);
        if (!p1)
        {
            return false;
        }

        var idx = cpu.Instructions.FindIndex(
            (v) => v.Definition.Token == "LBL" && v.Parameters[0].Value == p1Value
        );
        if (idx == -1)
        {
            return false;
        }

        cpu.SetPointer(idx);
        return true;
    };
}

public class JEZ : IDefinition
{
    public string Token => "JEZ";

    public List<ParameterConstraint> ParameterConstraints => [ParameterConstraint.ConstantOrMemory, ParameterConstraint.Constant];

    public Func<ICPU, Instruction, bool> Func => (cpu, instruction) =>
    {
        var p1 = cpu.GetParameterValue(instruction.Parameters[0], out var p1Value);
        var p2 = cpu.GetParameterValue(instruction.Parameters[1], out var p2Value);
        if (!p1 || !p2)
        {
            return false;
        }

        if (p1Value == 0)
        {
            var idx = cpu.Instructions.FindIndex(v => v.Definition.Token == "LBL" && v.Parameters[0].Value == p2Value);
            if (idx == -1)
            {
                return false;
            }

            cpu.SetPointer(idx);
        }

        return true;
    };
}

public class JLZ : IDefinition
{
    public string Token => "JLZ";

    public List<ParameterConstraint> ParameterConstraints => [ParameterConstraint.ConstantOrMemory, ParameterConstraint.Constant];

    public Func<ICPU, Instruction, bool> Func => (cpu, instruction) =>
    {
        var p1 = cpu.GetParameterValue(instruction.Parameters[0], out var p1Value);
        var p2 = cpu.GetParameterValue(instruction.Parameters[1], out var p2Value);
        if (!p1 || !p2)
        {
            return false;
        }

        if (p1Value < 0)
        {
            var idx = cpu.Instructions.FindIndex(v => v.Definition.Token == "LBL" && v.Parameters[0].Value == p2Value);
            if (idx == -1)
            {
                return false;
            }

            cpu.SetPointer(idx);
        }

        return true;
    };
}

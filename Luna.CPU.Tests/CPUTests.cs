namespace Luna.CPU.Tests;

using Luna.CPU;

[TestClass]
public class CPUTests
{
    [TestMethod]
    public void MemBounds()
    {
        var memory = new Memory(16);
        Assert.IsFalse(memory.GetAt(-1, out var _));
        Assert.IsFalse(memory.GetAt(16, out var _));
        Assert.IsFalse(memory.SetAt(-1, 101));
        Assert.IsFalse(memory.SetAt(16, 101));
    }

    [TestMethod]
    public void MemSetGet()
    {
        var memory = new Memory(16);
        memory.SetAt(1, 101);
        memory.GetAt(1, out var value);
        Assert.AreEqual(101, value);
    }

    [TestMethod]
    public void MemSetGetIndirect()
    {
        var memory = new Memory(16);
        memory.SetAt(1, 8);
        memory.SetAtIndirect(1, 101);
        memory.GetAtIndirect(1, out var value);
        Assert.AreEqual(101, value);
    }

    [TestMethod]
    public void MemSetGetIndirectBounds()
    {
        var memory = new Memory(16);
        memory.SetAt(1, -1);
        memory.SetAt(2, 16);
        Assert.IsFalse(memory.SetAtIndirect(1, 101));
        Assert.IsFalse(memory.SetAtIndirect(2, 101));
        Assert.IsFalse(memory.GetAtIndirect(1, out var _));
        Assert.IsFalse(memory.GetAtIndirect(2, out var _));
    }

    [TestMethod]
    public void CPUAdd()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["LBL 0", "ADD #1 1", "JMP 0"]);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(ParseResultComment.OK, result.Comment);
        Assert.AreEqual(3, result.Line);
        for (var i = 0; i <= 4; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(1, out var value));
        Assert.AreEqual(2, value);
    }

    [TestMethod]
    public void CPUNoCode()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Step();
        Assert.AreEqual(StepResultComment.ENDOFCODE, result.Comment);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, result.Line);
    }

    [TestMethod]
    public void CPUNoCodeParsed()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse([]);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, result.Line);
    }

    [TestMethod]
    public void CPUPointerOutOfCode()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL()]);
        Assert.AreEqual(true, cpu.Parse(["LBL 0"]).Success);
        cpu.Step();
        var result = cpu.Step();
        Assert.IsFalse(result.Success);
        Assert.AreEqual(StepResultComment.ENDOFCODE, result.Comment);
        Assert.AreEqual(1, result.Line);
    }

    [TestMethod]
    public void CPUBasicAdd1()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["ADD #0 1", "ADD #0 #0", "ADD >0 101"]);
        for (var i = 0; i <= 2; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(0, out var value1));
        Assert.AreEqual(2, value1);
        Assert.IsTrue(cpu.Memory.GetAt(2, out var value2));
        Assert.AreEqual(101, value2);
    }

    [TestMethod]
    public void CPUBasicAdd2()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["ADD #0 1", "ADD #0 #0", "ADD >0 101"]);
        for (var i = 0; i <= 2; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(0, out var value1));
        Assert.AreEqual(2, value1);
        Assert.IsTrue(cpu.Memory.GetAt(2, out var value2));
        Assert.AreEqual(101, value2);
    }

    [TestMethod]
    public void CPUBasicAdd3()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["ADD #0 1", "ADD #1 1", "ADD #3 #0", "ADD #3 #1"]);
        for (var i = 0; i <= 3; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(3, out var value));
        Assert.AreEqual(2, value);
    }

    [TestMethod]
    public void CPUBasicAdd4()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["ADD #0 1", "ADD >0 2", "ADD #3 >0"]);
        for (var i = 0; i <= 2; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(3, out var value));
        Assert.AreEqual(2, value);
    }

    [TestMethod]
    public void CPUBasicAdd5()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["ADD #2 0", "ADD #0 101", "ADD #1 >2"]);
        for (var i = 0; i <= 2; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(1, out var value));
        Assert.AreEqual(101, value);
    }

    [TestMethod]
    public void CPUBasicAddSplit()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse("ADD #0 1\nADD #0 #0\nADD >0 101".Split("\n"));
        for (var i = 0; i <= 2; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(0, out var value1));
        Assert.AreEqual(2, value1);
        Assert.IsTrue(cpu.Memory.GetAt(2, out var value2));
        Assert.AreEqual(101, value2);
    }

    [TestMethod]
    public void CPUBasicAddComment()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["--test", " --test", "--test ADD #0 1", "ADD #2 0  --test  ", "ADD #0 101", "ADD #1 >2"]);
        for (var i = 0; i <= 2; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(1, out var value));
        Assert.AreEqual(101, value);
        Assert.AreEqual(3, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddFormatting()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["  ADD #2 0", "ADD   #0 101", "ADD #1   >2"]);
        for (var i = 0; i <= 2; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(1, out var value));
        Assert.AreEqual(101, value);
        Assert.AreEqual(3, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddInvalid1()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["ADD #2 0 ADD #0 101", "ADD #1 >2"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.INVALIDPARAMETERS, result.Comment);
    }

    [TestMethod]
    public void CPUBasicAddInvalid2()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["ADD #2 0 1", "ADD #0 101", "ADD #1 >2"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.INVALIDPARAMETERS, result.Comment);
    }

    [TestMethod]
    public void CPUBasicAddInvalidDecimal1()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["ADD #2 0.0", "ADD #0 101", "ADD #1 >2"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.INVALIDPARAMETER, result.Comment);
    }

    [TestMethod]
    public void CPUBasicAddInvalidDecimal2()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["ADD #2.0 0", "ADD #0 101", "ADD #1 >2"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.INVALIDPARAMETER, result.Comment);
    }

    [TestMethod]
    public void CPUBasicAddInvalidDecimal3()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["ADD #2 1z", "ADD #0 101", "ADD #1 >2"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.INVALIDPARAMETER, result.Comment);
    }

    [TestMethod]
    public void CPUBasicAddEmpty1()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse([""]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddEmpty2()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse([]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddEmptyMulti()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["", ""]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddSpace()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse([" "]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }


    [TestMethod]
    public void CPUBasicAddSpaceMulti()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse([" ", " "]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddMix1()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse(["", " ", " "]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddMix2()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        var result = cpu.Parse([" ", "", " "]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.NOCODE, result.Comment);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicJmpInvalid()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["JMP 0"]);
        var result = cpu.Step();
        Assert.IsFalse(result.Success);
        Assert.AreEqual(StepResultComment.NOINSTRUCTION, result.Comment);
    }

    [TestMethod]
    public void CPUBasicAddJmp()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JMP()]);
        cpu.Parse(["LBL 0", "ADD #1 1", "JMP 0"]);
        for (var i = 0; i <= 3; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(1, out var value));
        Assert.AreEqual(2, value);
    }

    [TestMethod]
    public void CPUBasicAddJez()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JEZ()]);
        cpu.Parse(["LBL 0", "ADD #1 1", "JEZ #0 0"]);
        for (var i = 0; i <= 4; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(1, out var value));
        Assert.AreEqual(2, value);
    }

    [TestMethod]
    public void CPUBasicAddJlz()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new JLZ()]);
        cpu.Parse(["ADD #0 -1", "LBL 0", "ADD #1 1", "JLZ #0 0",]);
        for (var i = 0; i <= 5; i++)
        {
            Assert.IsTrue(cpu.Step().Success);
        }

        Assert.IsTrue(cpu.Memory.GetAt(1, out var value));
        Assert.AreEqual(2, value);
    }

    [TestMethod]
    public void CPUBasicAddInvalid3()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new ADD()]);
        var result = cpu.Parse(["ADD 0 1"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUBasicAddInvalid4()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new ADD()]);
        var result = cpu.Parse(["ADD #0 1 1"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, cpu.Instructions.Count);
    }

    [TestMethod]
    public void CPUIncorrectMemoryParameter()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL()]);
        var result = cpu.Parse(["LBL #0"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.INVALIDPARAMETER, result.Comment);
    }

    [TestMethod]
    public void CPUIncorrectIndirectParameter()
    {
        var cpu = new CPU(new Memory(16));
        cpu.Definitions.AddRange([new LBL()]);
        var result = cpu.Parse(["LBL >0"]);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(ParseResultComment.INVALIDPARAMETER, result.Comment);
    }

    [TestMethod]
    public void CPUFib()
    {
        var cpu = new CPU(new Memory(32));
        cpu.Definitions.AddRange([new LBL(), new ADD(), new SUB(), new JMP(), new SET(), new JLZ()]);
        cpu.Parse([
            "--comment",
            "SET #0 24",
            "SET #2 5",
            "SET #4 1 --comment",
            "--comment",
            "LBL 0",
            "SET >2 #3",
            "ADD >2 #4",
            "SET #3 #4",
            "SET #4 >2",
            "ADD #2 1",
            "SUB #0 1",
            "JLZ #0 1",
            "JMP 0",
            "LBL 1" ]);
        for (var i = 0; i < 201; i++)
        {
            var result = cpu.Step();
            Assert.IsTrue(result.Success);
        }
        Assert.IsTrue(cpu.Memory.GetAt(29, out var value));
        Assert.AreEqual(121393, value);
    }
}

namespace Luna.CPU;

public enum ParseResultComment
{
    OK,
    ERROR,
    NOCODE,
    INVALIDINSTRUCTION,
    INVALIDPARAMETERS,
    INVALIDPARAMETER
}

public enum StepResultComment
{
    OK,
    ERROR,
    ENDOFCODE,
    NOINSTRUCTION
}

public abstract class Result(bool success, int line)
{
    public bool Success { get; set; } = success;

    public int Line { get; set; } = line;
}

public class ParseResult(bool success, ParseResultComment comment, int line) : Result(success, line)
{

    public ParseResultComment Comment { get; set; } = comment;
}

public class StepResult(bool success, StepResultComment comment, int line) : Result(success, line)
{

    public StepResultComment Comment { get; set; } = comment;
}


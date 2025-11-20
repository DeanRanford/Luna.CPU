namespace Luna.CPU;

public interface IMemory
{
    public bool IsLocationValid(int index);
    public bool GetAt(int index, out int value);
    public bool SetAt(int index, int value);
    public bool GetAtIndirect(int index, out int value);
    public bool SetAtIndirect(int index, int value);
    public int[] GetAll();
    public int GetSize();
    public void SetSize(int size);
    public void Reset();
}

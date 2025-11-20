namespace Luna.CPU;

public class Memory : IMemory
{
    private int[] _data = [];

    public Memory(int size) => this.SetSize(size);

    public int[] GetAll() => this._data;

    public bool GetAt(int index, out int value)
    {
        value = 0;
        if (!this.IsLocationValid(index))
        {
            return false;
        }

        value = this._data[index];
        return true;
    }

    public bool GetAtIndirect(int index, out int value)
    {
        value = 0;
        if (!this.GetAt(index, out index))
        {
            return false;
        }

        if (!this.GetAt(index, out value))
        {
            return false;
        }

        return true;
    }

    public int GetSize() => this._data.Length;

    public bool IsLocationValid(int index) => index >= 0 && index < this._data.Length;

    public void Reset() => Array.Clear(this._data, 0, this._data.Length);

    public bool SetAt(int index, int value)
    {
        if (!this.IsLocationValid(index))
        {
            return false;
        }

        this._data[index] = value;
        return true;

    }

    public bool SetAtIndirect(int index, int value)
    {
        if (!this.GetAt(index, out index))
        {
            return false;
        }

        if (!this.SetAt(index, value))
        {
            return false;
        }

        return true;
    }

    public void SetSize(int size) => Array.Resize(ref this._data, size);
}

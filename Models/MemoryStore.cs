using KalkulatorMAUI_MVVM.ValueObjects;

public class MemoryStore : ObservableObject
{
    private readonly List<MemoryValue> _memoryValues;

    public MemoryStore()
    {
        _memoryValues = new List<MemoryValue>();
    }

    public IReadOnlyList<MemoryValue> MemoryValues => _memoryValues.AsReadOnly();

    public void AddMemoryValue(MemoryValue memoryValue)
    {
        _memoryValues.Add(memoryValue);
        OnPropertyChanged(nameof(MemoryValues));
    }

    public void ClearMemoryValues()
    {
        _memoryValues.Clear();
        OnPropertyChanged(nameof(MemoryValues));
    }

    public long RecallLastMemoryValue()
    {
        return _memoryValues.Any() ? _memoryValues.Last().Value : 0;
    }

    public void AddToMemoryValue(MemoryValue memoryValue, long valueToAdd)
    {
        var index = _memoryValues.IndexOf(memoryValue);
        if (index >= 0)
        {
            var updatedValue = new MemoryValue(memoryValue.Value + valueToAdd);
            _memoryValues[index] = updatedValue;
            OnPropertyChanged(nameof(MemoryValues));
        }
    }

    public void SubtractFromMemoryValue(MemoryValue memoryValue, long valueToSubtract)
    {
        var index = _memoryValues.IndexOf(memoryValue);
        if (index >= 0)
        {
            var updatedValue = new MemoryValue(memoryValue.Value - valueToSubtract);
            _memoryValues[index] = updatedValue;
            OnPropertyChanged(nameof(MemoryValues));
        }
    }

    public void RemoveMemoryValue(MemoryValue memoryValue)
    {
        _memoryValues.Remove(memoryValue);
        OnPropertyChanged(nameof(MemoryValues));
    }
}

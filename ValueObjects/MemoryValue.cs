namespace KalkulatorMAUI_MVVM.ValueObjects
{
    public record MemoryValue
    {
        public long Value { get; }

        public MemoryValue(long value)
        {
            Value = value;
        }
    }
}

namespace KalkulatorMAUI_MVVM.Services
{
    public static class MemoryService
    {
        private static Stack<long> _memoryStack = new Stack<long>();

        public static void MemoryStore(long value)
        {
            _memoryStack.Push(value);
        }

        public static void MemoryAdd(long value)
        {
            long newValue = _memoryStack.Pop();
            newValue = newValue + value;
            _memoryStack.Push(newValue);
        }

        public static void MemorySubtract(long value)
        {
            long newValue = _memoryStack.Pop();
            newValue = newValue - value;
            _memoryStack.Push(newValue);
        }

        public static void MemoryClear()
        {
            _memoryStack.Clear();
        }

        public static long MemoryRecall()
        {
            return _memoryStack.Count > 0 ? _memoryStack.Peek() : 0;
        }

        public static List<long> GetMemoryStack()
        {
            return _memoryStack.ToList();
        }
    }
}

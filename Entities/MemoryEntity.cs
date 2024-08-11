using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorMAUI_MVVM.Entities
{
    public class MemoryEntity
    {
        public Guid Id { get; private set; }
        public long Value { get; private set; }

        public MemoryEntity(long value)
        {
            Id = Guid.NewGuid();
            Value = value;
        }

        public void AddValue(long valueToAdd)
        {
            Value += valueToAdd;
        }

        public void SubtractValue(long valueToSubtract)
        {
            Value -= valueToSubtract;
        }
    }
}

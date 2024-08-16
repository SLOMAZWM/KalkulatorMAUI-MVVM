using CommunityToolkit.Mvvm.ComponentModel;
using KalkulatorMAUI_MVVM.Entities;
using System.Collections.Generic;
using System.Linq;

namespace KalkulatorMAUI_MVVM.Models
{
    public class MemoryStore : ObservableObject
    {
        private readonly List<MemoryEntity> _memoryEntities;

        public MemoryStore()
        {
            _memoryEntities = new List<MemoryEntity>();
        }

        public IReadOnlyList<MemoryEntity> MemoryEntities => _memoryEntities.AsReadOnly();

        public void AddMemoryEntity(MemoryEntity memoryEntity)
        {
            _memoryEntities.Add(memoryEntity);
            OnPropertyChanged(nameof(MemoryEntities));
        }

        public void ClearMemoryEntities()
        {
            _memoryEntities.Clear();
            OnPropertyChanged(nameof(MemoryEntities));
        }

        public void AddValueToLastMemoryEntity(MemoryEntity newValue)
        {
            if(_memoryEntities.Any())
            {
                var LastValue = _memoryEntities.Last();
                LastValue.AddValue(newValue.Value);
                OnPropertyChanged(nameof(MemoryEntities));
            }
        }

        public long RecallMemoryEntityValue(MemoryEntity memoryEntity)
        {
            return _memoryEntities.FirstOrDefault(me => me.Id == memoryEntity.Id)?.Value ?? 0;
        }

        public long RecallLastMemoryEntityValue()
        {
            return _memoryEntities.Any() ? _memoryEntities.Last().Value : 0;
        }

        public void AddToMemoryEntity(MemoryEntity memoryEntity, long valueToAdd)
        {
            var entity = _memoryEntities.FirstOrDefault(me => me.Id == memoryEntity.Id);
            if (entity != null)
            {
                entity.AddValue(valueToAdd);
                OnPropertyChanged(nameof(MemoryEntities));
            }
        }

        public void SubtractFromMemoryEntity(MemoryEntity memoryEntity, long valueToSubtract)
        {
            var entity = _memoryEntities.FirstOrDefault(me => me.Id == memoryEntity.Id);
            if (entity != null)
            {
                entity.SubtractValue(valueToSubtract);
                OnPropertyChanged(nameof(MemoryEntities));
            }
        }

        public void RemoveMemoryEntity(MemoryEntity memoryEntity)
        {
            _memoryEntities.Remove(memoryEntity);
            OnPropertyChanged(nameof(MemoryEntities));
        }
    }
}

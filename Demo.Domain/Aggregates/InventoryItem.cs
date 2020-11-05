using Demo.Domain.Events.InventoryItem;
using EventSourcing.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Domain.Aggregates
{
    public class InventoryItem : AggregateRoot
    {
        private Guid _id;

        public override Guid Id { get { return _id; } }
        public bool Activated { get; private set; }
        public string Name { get; private set; }
        public int Count { get; private set; }

        private void Apply(InventoryItemCreated e)
        {
            _id = e.AggregateId;
            Activated = true;
            Name = e.Name;
        }

        private void Apply(InventoryItemDeactivated e)
        {
            Activated = false;
        }

        private void Apply(InventoryItemRenamed e)
        {
            Name = e.NewName;
        }

        private void Apply(ItemsRemovedFromInventory e)
        {
            Count -= e.Count;
        }

        private void Apply(ItemsCheckedIntoInventory e)
        {
            Count += e.Count;
        }

        public void ChangeName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) throw new ArgumentException("newName");
            ApplyChange(new InventoryItemRenamed(_id, newName));
        }

        public void Remove(int count)
        {
            if (count <= 0) throw new InvalidOperationException("cant remove negative count from inventory");
            ApplyChange(new ItemsRemovedFromInventory(_id, count));
        }

        public void CheckIn(int count)
        {
            if (count <= 0) throw new InvalidOperationException("must have a count greater than 0 to add to inventory");
            ApplyChange(new ItemsCheckedIntoInventory(_id, count));
        }

        public void Deactivate()
        {
            if (!Activated) throw new InvalidOperationException("already deactivated");
            ApplyChange(new InventoryItemDeactivated(_id));
        }

        public InventoryItem()
        {
        }

        public InventoryItem(Guid id, string name)
        {
            ApplyChange(new InventoryItemCreated(id, name));
        }
    }
}

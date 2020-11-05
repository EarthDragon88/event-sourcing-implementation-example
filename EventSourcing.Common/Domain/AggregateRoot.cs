using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EventSourcing.Common.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();

        public abstract Guid Id { get; }
        public int Version { get; set; }



        // protected abstract void HandleApplyChange(Event @event);

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        // push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
        private void ApplyChange(Event @event, bool isNew)
        {
            var bla1 = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            var applyMethodForEvent = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x =>
                {
                    var parameters = x.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType.Equals(@event.GetType());
                })
                .SingleOrDefault();

            if(applyMethodForEvent != null)
            {
                applyMethodForEvent.Invoke(this, new object[] { @event });
            }

            //HandleApplyChange(@event);
            if (isNew) _changes.Add(@event);
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history) ApplyChange(e, false);
        }

        // protected void ApplyChange(Event @event)
        // {
        //     ApplyChange(@event, true);
        // }
        // 
        // // push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
        // private void ApplyChange(Event @event, bool isNew)
        // {
        //     this.AsDynamic().Apply(@event);
        //     if (isNew) _changes.Add(@event);
        // }
    }
}

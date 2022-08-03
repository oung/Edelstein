﻿using System.Collections.Immutable;
using Edelstein.Common.Gameplay.Stages.Game.Objects;
using Edelstein.Protocol.Gameplay.Stages.Game;
using Edelstein.Protocol.Gameplay.Stages.Game.Objects;
using Edelstein.Protocol.Network;
using Edelstein.Protocol.Util.Buffers.Packets;

namespace Edelstein.Common.Gameplay.Stages.Game;

public class FieldSplit : AbstractFieldObjectPool, IFieldSplit
{
    private readonly ICollection<IFieldObject> _objects;
    private readonly ICollection<IFieldSplitObserver> _observers;

    public FieldSplit(int row, int col)
    {
        Row = row;
        Col = col;
        _objects = new List<IFieldObject>();
        _observers = new List<IFieldSplitObserver>();
    }

    public override IReadOnlyCollection<IFieldObject> Objects => _objects.ToImmutableList();
    public IReadOnlyCollection<IFieldSplitObserver> Observers => _observers.ToImmutableList();

    public int Row { get; }
    public int Col { get; }

    public override Task Enter(IFieldObject obj) => Enter(obj, null);
    public override Task Leave(IFieldObject obj) => Leave(obj, null);

    public async Task Enter(IFieldObject obj, Func<IPacket>? getEnterPacket, Func<IPacket>? getLeavePacket = null)
    {
        var from = obj.FieldSplit;

        if (from != null)
            await from.MigrateOut(obj);
        await MigrateIn(obj);

        obj.FieldSplit = this;

        var toObservers = Observers;
        var fromObservers = from?.Observers ?? new List<IFieldSplitObserver>();
        var newWatchers = toObservers
            .Where(w => w != obj)
            .Except(fromObservers)
            .ToImmutableList();
        var oldWatchers = fromObservers
            .Where(w => w != obj)
            .Except(toObservers)
            .ToImmutableList();

        var enterPacket = getEnterPacket?.Invoke() ?? obj.GetEnterFieldPacket();
        var leavePacket = getLeavePacket?.Invoke() ?? obj.GetLeaveFieldPacket();

        await Task.WhenAll(newWatchers.Select(w => w.Dispatch(enterPacket)));
        await Task.WhenAll(oldWatchers.Select(w => w.Dispatch(leavePacket)));

        if (obj is IFieldSplitObserver observer)
        {
            var enclosingSplits = observer.Field?.GetEnclosingSplits(this) ?? Array.Empty<IFieldSplit>();
            var oldSplits = observer.Observing
                .Except(enclosingSplits)
                .Where(s => s != null)
                .ToImmutableList();
            var newSplits = enclosingSplits
                .Except(observer.Observing)
                .Where(s => s != null)
                .ToImmutableList();

            await Task.WhenAll(oldSplits.Select(s => s!.Unobserve(observer)));
            await Task.WhenAll(newSplits.Select(s => s!.Observe(observer)));
        }
    }

    public async Task Leave(IFieldObject obj, Func<IPacket>? getLeavePacket)
    {
        obj.FieldSplit = null;

        await MigrateOut(obj);
        await Dispatch(getLeavePacket?.Invoke() ?? obj.GetLeaveFieldPacket(), obj);
    }

    public Task MigrateIn(IFieldObject obj)
    {
        _objects.Add(obj);
        return Task.CompletedTask;
    }

    public Task MigrateOut(IFieldObject obj)
    {
        _objects.Remove(obj);
        return Task.CompletedTask;
    }

    public async Task Observe(IFieldSplitObserver observer)
    {
        _observers.Add(observer);
        observer.Observing.Add(this);

        await Task.WhenAll(GetObjects()
            .Where(o => o != observer)
            .Select(o => observer.Dispatch(o.GetEnterFieldPacket())));
    }

    public async Task Unobserve(IFieldSplitObserver observer)
    {
        _observers.Remove(observer);
        observer.Observing.Remove(this);

        await Task.WhenAll(GetObjects()
            .Where(o => o != observer)
            .Select(o => observer.Dispatch(o.GetLeaveFieldPacket())));
    }

    public override Task Dispatch(IPacket packet) =>
        Task.WhenAll(Observers
            .OfType<IAdapter>()
            .Select(a => a.Dispatch(packet)));

    public override Task Dispatch(IPacket packet, IFieldObject obj) =>
        Task.WhenAll(Observers
            .OfType<IAdapter>()
            .Where(a => a != obj)
            .Select(a => a.Dispatch(packet)));

    public override IFieldObject? GetObject(int id) => Objects.FirstOrDefault(o => o.ObjectID == id);
    public override IEnumerable<IFieldObject> GetObjects() => Objects;
}
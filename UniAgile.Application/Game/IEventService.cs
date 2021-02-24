using System;
using UniAgile.Observable;

namespace UniAgile.Game
{
    public interface IEventService
    {
        ISignal<TimeSpan> UpdateUiEvent     { get; }
        ISignal<TimeSpan> BeforeUpdateEvent { get; }
        ISignal<TimeSpan> UpdateEvent       { get; }
        ISignal<TimeSpan> PostUpdateEvent   { get; }
    }
}
using UniAgile.Observable;

namespace UniAgile.Game
{
    public interface IEventService
    {
        ISignal<float> BeforeUpdateEvent { get; }

        ISignal<float> UpdateEvent     { get; }
        ISignal<float> PostUpdateEvent { get; }
    }
}
namespace eWolf.Common.Interfaces
{
    public interface IRandomizer
    {
        bool IsLocked { get; }

        void Randomize();

        void RandomizeVisual();
    }
}
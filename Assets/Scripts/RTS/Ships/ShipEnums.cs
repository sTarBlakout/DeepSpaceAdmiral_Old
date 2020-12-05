namespace RTS.Ships
{
    public enum State
    {
        Idle,
        MoveToPosition,
        AttackTarget,
        Destroyed,
        Empty
    }

    public enum DestructionLevel
    {
        New,
        Destroyed
    }

    public enum EngineSection
    {
        Main,
        Right,
        Left
    }

    public enum FireMode
    {
        NoGuns,
        OnlyMain,
        OnlyOnboard,
        AllGuns
    }
    
    public enum DimensionPointPos
    {
        Front,
        Back,
        Left,
        Right
    }
}
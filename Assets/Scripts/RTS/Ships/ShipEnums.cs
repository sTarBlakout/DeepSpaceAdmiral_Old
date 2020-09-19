namespace RTS.Ships
{
    public enum Stance
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
}
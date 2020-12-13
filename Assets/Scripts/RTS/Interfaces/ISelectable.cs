namespace RTS.Interfaces
{
    public interface ISelectable
    {
        byte TeamId { get; }
        bool CanSelect();
        void Select();
        void Unselect();
    }
}
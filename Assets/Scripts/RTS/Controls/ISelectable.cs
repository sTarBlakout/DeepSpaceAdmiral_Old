namespace RTS.Controls
{
    public interface ISelectable
    {
        byte TeamId { get; }
        bool CanSelect();
        void Select();
        void Unselect();
    }
}
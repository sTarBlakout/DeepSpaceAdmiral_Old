namespace RTS.Controls
{
    public interface ISelectable
    {
        bool CanSelect();
        void Select();
        void Unselect();
    }
}
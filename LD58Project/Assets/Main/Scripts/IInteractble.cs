namespace Main.Scripts
{
    public interface IInteractable
    {
        
        string Id { get; }
        void EnableOutline();
        void DisableOutline();
        void Interact();
        bool TryGetComponent<T>(out T value);
    }
}
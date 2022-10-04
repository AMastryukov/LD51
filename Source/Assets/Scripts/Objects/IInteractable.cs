using UnityEngine.Events;
public interface IInteractable
{
    public void Interact();

    public bool CanInteract();
    public string GetName();
}
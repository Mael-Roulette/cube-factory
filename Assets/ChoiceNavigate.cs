using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceNavigate : MonoBehaviour, IPointerDownHandler
{
    public enum Direction { Next, Prev }
    public Direction direction;

    public void OnPointerDown(PointerEventData eventData)
    {
        choiceInteract choice = FindFirstObjectByType<choiceInteract>();
        if (choice == null) return;

        if (direction == Direction.Next)
            choice.Next();
        else
            choice.Prev();
    }
}
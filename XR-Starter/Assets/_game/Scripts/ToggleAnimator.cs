using HoloToolkit.Unity.InputModule;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ToggleAnimator : MonoBehaviour, IInputClickHandler
{
    [SerializeField] Animator animator;
    
    public void OnInputClicked(InputClickedEventData eventData)
    {
        animator.enabled = !animator.enabled;
    }
}
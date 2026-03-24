using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trevaughn.Scripts.UI
{
    // Simple script used to activate dialogue. Mostly used on trigger enter
    public class DialogueActivator : MonoBehaviour
    {
        [SerializeField] private DialogueData dialogueInfo;

        public void PlayDialogue()
        {
            DialogueManager.Instance.PlayDialogue(dialogueInfo);
        }
    }
}
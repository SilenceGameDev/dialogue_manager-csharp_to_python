using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trevaughn.Scripts.UI
{
    [CreateAssetMenu(fileName = "DialogueData", menuName = "Data/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        [Tooltip("Used for dialouge that follow the usual A,B,C structure")]
        public string dialogueLetter;

        [Tooltip("Used to display the speaker name. If left empty, will show up blank.")]
        public string speakerNameKey;

        [Tooltip("-1 will play the default delayContinueButton time on DialogueManager. 0 will make sure continue button doesnt appear.")]
        public float delayButtonTimer = -1;

        [Tooltip("Used to delay the next button for each sentance. -1 will play the default delayContinueButton time on DialogueManager and index with 0 will skip causing the continue button to not appear." +
            " \nCount should match sentances count.")]
        public List<float> delayButtonTimerIndexes = new ();

        [Tooltip("Mostly used during minigames to display dialogue that doesn't follow the usual A,B,C structure. Don't need to set dialogueLetter if setting this key.")]
        public string dialogueKey;

        [Tooltip("Should be left at 0 in most cases. Used for testing from a specific point in a dialogue.")]
        public int startingIndex = 0;

        [Tooltip("Used to visualize the dialogue. Check the language file and populate these")]
        [TextArea(2,4)]
        [SerializeField] private string[] sentances;
    }
}

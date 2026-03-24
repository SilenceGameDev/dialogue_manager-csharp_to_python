using System.Collections;
using System.Collections.Generic;
using TMPro;
using Trevaughn.Scripts.EventSystem;
using Trevaughn.Scripts.LevelData;
using UnityEngine;
using UnityEngine.UI;

namespace Trevaughn.Scripts.UI
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [Tooltip("The UI that will turn on/off when dialogue is being played")]
        [SerializeField] private SimpleMenuAnimation simpleMenu;

        [Tooltip("Text that will update each time PlayDialogue is called")]
        [SerializeField] private TMP_Text dialogueText;

        [Tooltip("Name of the person speaking during dialogue")]
        [SerializeField] private TMP_Text speakerNameText;

        [Tooltip("Delay before the continue button should appear when dialogue is active. Used to stop players from spamming through dialogue.\n " +
            "This default value can be overriden (DialogueTrigger) to have more control of longer dialouges. ")]
        [SerializeField] private float defaultContinueButtonAppearDelay = 2f;

        [Tooltip("Used to progress the dialogue on click")]
        [SerializeField] private Button continueButton;

        [SerializeField] private LevelManagerData levelManagerData;

        // Event channels that will invoke events when needed.
        [SerializeField] private EventChannel dialogueStartedChannel;
        [SerializeField] private EventChannel dialogueNextSentanceChannel;
        [SerializeField] private EventChannel dialogueEndedChannel;

        private WaitForSeconds _continueButtonTimeWFS;
        private int _currentDialogueIndex;
        public int GetCurrentDialogueIndex => _currentDialogueIndex;

        private string _currentDialogueLetter;

        // Set by combining the dialogue letter and index. Used to get the dialogue inside language file
        private string _dialogueLanguageKey;
        private string _speakerNameLanguageKey;

        // Used to delay the continue button appearing for each index. If left null will use continueButtonDelay instead, keeping continue button delay uniform for all indexes.
        private List<float> _delayButtonTimerIndexes;

        // Used to pass an empty value into event channel. Other event channels may want data but using and empty value for channels that dont need a value.
        private Empty _emptyValue;

        private void Awake()
        {
            _continueButtonTimeWFS = new WaitForSeconds(defaultContinueButtonAppearDelay);
            continueButton.onClick.AddListener(DisplayNextBasicSecentance);
        }

        public void PlayDialogue(DialogueData dialogueInfo)
        {
            levelManagerData.FreezePlayer();

            UpdateContinueButtonDelay(dialogueInfo.delayButtonTimer);

            // Set delay timers for each index. If left null will use default continue button delay
            _delayButtonTimerIndexes = new List<float>(dialogueInfo.delayButtonTimerIndexes);

            ToggleDialogueScreen(true);

            SetupDialogueText(dialogueInfo);

            // invoke on dialogue started event
            dialogueStartedChannel.Invoke(_emptyValue);

            DisplayNextBasicSecentance();

        }

        private void SetupDialogueText(DialogueData dialogueInfo)
        {
            // Sets default name text to name of NPC talking.
            _speakerNameLanguageKey = "character_" + dialogueInfo.speakerNameKey.ToString();
            speakerNameText.text = SharedState.LanguageDefs[_speakerNameLanguageKey];

            // Will be 0 most of the time. When testing, startingIndex can be set to a different value for ease of use.
            _currentDialogueIndex = dialogueInfo.startingIndex;
            _currentDialogueLetter = dialogueInfo.dialogueLetter;
            _dialogueLanguageKey = "Dialogue_" + _currentDialogueLetter + "_Sen_" + _currentDialogueIndex.ToString();
        }

        // Also called from Continue/Next button
        public void DisplayNextBasicSecentance()
        {
            if (SharedState.LanguageDefs[_dialogueLanguageKey] == null)
            {
                EndDialogue();

                return;
            }

            SetContinueButtonAppearDelay();

            dialogueText.text = SharedState.LanguageDefs[_dialogueLanguageKey];

            // If TTS is disabled it will not play
            TTSManager.Instance.PlayTTS(SharedState.LanguageDefs[_dialogueLanguageKey]);

            _currentDialogueIndex++;
            _dialogueLanguageKey = "Dialogue_" + _currentDialogueLetter + "_Sen_" + _currentDialogueIndex.ToString();
            dialogueNextSentanceChannel.Invoke(_emptyValue);
        }

        public void EndDialogue()
        {
            ToggleDialogueScreen(false);

            _delayButtonTimerIndexes.Clear();

            levelManagerData.UnFreezePlayer();

            // Triggering event after unfreezing because there would be some cases where the player would need to be frozen again after dialogue ends.
            dialogueEndedChannel.Invoke(_emptyValue);
        }

        // Mostly used during minigames to display dialogue that doesn't follow the usual A,B,C structure.
        public void StartSpecificDialogue(DialogueData dialogueInfo)
        {
            continueButton.gameObject.SetActive(false);

            ToggleDialogueScreen(true);
            dialogueStartedChannel.Invoke(_emptyValue);

            // if no delayButtonTime was passed in, use default delay. Else use the one that was passed in.
            UpdateContinueButtonDelay(dialogueInfo.delayButtonTimer);

            _speakerNameLanguageKey = dialogueInfo.speakerNameKey;
            speakerNameText.text = SharedState.LanguageDefs[dialogueInfo.speakerNameKey];

            _dialogueLanguageKey = dialogueInfo.dialogueKey;
            DisplayNextBasicSecentance();
        }

        private void UpdateContinueButtonDelay(float newDelayTimer = -1)
        {
            if (newDelayTimer != -1)
            {
                _continueButtonTimeWFS = new WaitForSeconds(newDelayTimer);
            }
        }

        private IEnumerator DelayContinueButtonAppear()
        {
            continueButton.gameObject.SetActive(false);
            yield return _continueButtonTimeWFS;
            continueButton.gameObject.SetActive(true);
        }

        private void ToggleDialogueScreen(bool active)
        {
            if (active)
            {
                simpleMenu.EnableMenu();
            }
            else
            {
                simpleMenu.DisableMenu();
            }
        }

        private void SetContinueButtonAppearDelay()
        {
            // no array was added so return
            if (_delayButtonTimerIndexes.Count == 0)
            {
                StartCoroutine(DelayContinueButtonAppear());
                return;
            }

            // this index == -1 so use default
            if (_delayButtonTimerIndexes[_currentDialogueIndex] == -1)
            {
                _continueButtonTimeWFS = new WaitForSeconds(defaultContinueButtonAppearDelay);
            }
            // this index == 0 so skip it
            else if(_delayButtonTimerIndexes[_currentDialogueIndex] == 0)
            {
                return;
            }
            else
            {
                _continueButtonTimeWFS = new WaitForSeconds(_delayButtonTimerIndexes[_currentDialogueIndex]);
            }

            StartCoroutine(DelayContinueButtonAppear());
        }

    }
}


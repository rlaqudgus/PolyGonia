using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bases;
using Manager.DialogueScripts;
using TMPro;
using UnityEngine;

// How to make a Dialogue System in Unity
// https://www.youtube.com/watch?v=_nRzoTzeyxU

namespace Manager
{
    public class DialogueManager : Singleton<DialogueManager>
    {   
        private Sentence _sentence;
        private string _speaker;

        private Queue<Sentence> _sentences = new Queue<Sentence>();
        private Queue<string> _speakers = new Queue<string>();

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;

        // Make a Dialogue System (that types letter-by-letter with NO line overflow) | Unity Tutorial
        // https://www.youtube.com/watch?v=jTPOCglHejE
    
        private bool _isTyping = false;
        //private const string _HTML_ALPHA = "<color=#00000000>";
        [SerializeField] [Range(1f, 1000f)] private float _typeSpeed;
        private float _initTypeSpeed;
        [SerializeField] private bool _syncTextToVoice;

        //public Animator animator;

        public event Action OnDialogueStarted;
        public event Action OnDialogueDisplayed;
        public event Action OnDialogueEnded;

        private void Awake()
        {
            CreateSingleton(this);
        
            _initTypeSpeed = _typeSpeed;
        }

        public void StartDialogue(Dialogue dialogue)
        {
            Debug.Log(dialogue.utterances);
            
            if (dialogue == null)
            {
                Debug.LogWarning("There is no dialogue data assigned");
                return;
            }
            else if (dialogue.utterances.Length == 0)
                Debug.LogWarning("Dialogue exists but there is no utterances assigned in " + dialogue.name);

            _sentences.Clear();
            _speakers.Clear();

            foreach (Utterance utterance in dialogue.utterances)
            {
                foreach (Sentence sentence in utterance.sentences)
                {
                    _sentences.Enqueue(sentence);
                    _speakers.Enqueue(utterance.speaker);
                }
            }
        
            UIManager.Instance.OpenPopupUI(UIManager.DIALOGUE_CANVAS);
            GameManager.Instance.playerInput.SwitchCurrentActionMap("UI");

            //animator.SetBool("IsOpen", true);

            OnDialogueStarted?.Invoke();

            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {   
            // Initialize Voice
            SoundManager.Instance.StopVoice();

            // Initialize Text and Typing
            StopAllCoroutines();
            _typeSpeed = _initTypeSpeed;

            if (_isTyping) FinishTypingEarly(_sentence.text);
            else
            {
                if (_sentences.Count == 0)
                {
                    EndDialogue();
                    return;
                }

                _sentence = _sentences.Dequeue();
                _speaker = _speakers.Dequeue();

                // Voice
                if (!SoundManager.Instance.isVoiceMuted && _sentence.voice != null)
                {
                    SoundManager.Instance.PlayVoice(_sentence.voice);
                    if (_syncTextToVoice) _typeSpeed = (float)_sentence.text.Length / _sentence.voice.length;
                }

                // Text
                nameText.text = _speaker;
                StartCoroutine(TypeText(_sentence.text));
            } 

            OnDialogueDisplayed?.Invoke();
        }

        IEnumerator TypeText(string text)
        {
            _isTyping = true;

            StringBuilder sb = new StringBuilder();
        
            dialogueText.text = "";

            foreach (char letter in text)
            {
                sb.Append(letter);
                dialogueText.text = sb.ToString();
            
                yield return new WaitForSeconds(1f / _typeSpeed);
            }

            _isTyping = false;
        }

        private void FinishTypingEarly(string text)
        {
            dialogueText.text = text;
            _isTyping = false;
        }

        public void EndDialogue()
        {
            UIManager.Instance.ClosePopupUI();
            GameManager.Instance.playerInput.SwitchCurrentActionMap("Player");

            //animator.SetBool("IsOpen", false);
        
            _sentence = null;
            _speaker = null;
            Debug.Assert(!_isTyping, "The dialogue already ended but _isTyping is true");

            OnDialogueEnded?.Invoke();
        }
    }
}

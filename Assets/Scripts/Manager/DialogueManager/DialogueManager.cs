using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using TMPro;
using Manager;

// How to make a Dialogue System in Unity
// https://www.youtube.com/watch?v=_nRzoTzeyxU

public class DialogueManager : MonoBehaviour
{   
    private static DialogueManager _instance;
    public  static DialogueManager  Instance { get { return _instance; } }

    private Sentence _sentence;
    private string _speaker;

    private Queue<Sentence> _sentences;
    private Queue<string> _speakers;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    // Make a Dialogue System (that types letter-by-letter with NO line overflow) | Unity Tutorial
    // https://www.youtube.com/watch?v=jTPOCglHejE
    private bool _isTyping;
    private const string _HTML_ALPHA = "<color=#00000000>";
    [SerializeField] [Range(1f, 1000f)] float _typeSpeed;
    float _initTypeSpeed;
    [SerializeField] private bool _syncTextToVoice;

    //public Animator animator;

    public event Action OnDialogueStarted;
    public event Action OnDialogueDisplayed;
    public event Action OnDialogueEnded;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _sentences = new Queue<Sentence>();
        _speakers = new Queue<string>();

        _typeSpeed = Mathf.Max(_typeSpeed, 1f);
        _initTypeSpeed = _typeSpeed;

        _isTyping = false;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogWarning("There is no dialogue data assigned");
            return;
        }
        else if (dialogue.utterances.Length == 0)
        {
            Debug.LogWarning("Dialogue exists but there is no utterances assigned in " + dialogue.name);
        }

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
        KeyboardInputManager.Instance.UpdateInputState(KeyboardInputManager.UI);

        //animator.SetBool("IsOpen", true);

        if (OnDialogueStarted != null) OnDialogueStarted();

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {   
        // Initialize Voice
        SoundManager.Instance.StopVoice();

        // Initialize Text and Typing
        StopAllCoroutines();
        _typeSpeed = _initTypeSpeed;

        if (!_isTyping)
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

                if (_syncTextToVoice)
                {
                    // TypeText yield return WaitForSeconds(1f / _typeSpeed);
                    // _sentence.text.Length * 1f / _typeSpeed == _sentence.voice.Length
                    _typeSpeed = (float) _sentence.text.Length / _sentence.voice.length;
                }
            }

            // Text
            nameText.text = _speaker;
            StartCoroutine(TypeText(_sentence.text));
        }
        else
        {
            FinishTypingEarly(_sentence.text);
        }

        if (OnDialogueDisplayed != null) OnDialogueDisplayed();
    }

    IEnumerator TypeText(string text)
    {
        _isTyping = true;

        dialogueText.text = "";

        string originalText = text;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char letter in text.ToCharArray())
        {
            alphaIndex++;
            dialogueText.text = originalText;
            displayedText = dialogueText.text.Insert(alphaIndex, _HTML_ALPHA);

            dialogueText.text = displayedText;
            
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
        KeyboardInputManager.Instance.UpdateInputState(KeyboardInputManager.PLAYER);

        //animator.SetBool("IsOpen", false);
        
        _sentence = null;
        _speaker = null;
        Debug.Assert(!_isTyping, "The dialogue already ended but _isTyping is true");
        
        if (OnDialogueEnded != null) OnDialogueEnded();
    }
}

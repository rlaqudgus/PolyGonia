using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using TMPro;

// How to make a Dialogue System in Unity
// https://www.youtube.com/watch?v=_nRzoTzeyxU

public class DialogueManager : Singleton<DialogueManager>
{
    private string _sentence;
    private string _speaker;

    private Queue<string> _sentences;
    private Queue<string> _speakers;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    // Make a Dialogue System (that types letter-by-letter with NO line overflow) | Unity Tutorial
    // https://www.youtube.com/watch?v=jTPOCglHejE
    private bool _isTyping;
    private const float _MAX_TYPE_TIME = 1f;
    private const string _HTML_ALPHA = "<color=#00000000>";
    [SerializeField] [Range(1, 100)] int _typeSpeed;

    public Animator animator;

    private void Awake()
    {
        CreateSingleton(this);
    }

    private void Start()
    {
        _sentences = new Queue<string>();
        _speakers = new Queue<string>();

        _typeSpeed = Mathf.Max(_typeSpeed, 1);
        _isTyping = false;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        _sentences.Clear();
        _speakers.Clear();

        foreach (Utterance utterance in dialogue.utterances)
        {
            foreach (string sentence in utterance.sentences)
            {
                _sentences.Enqueue(sentence);
                _speakers.Enqueue(utterance.speaker);
            }
        }
        
        UIManager.Instance.OpenDialogueWindow();
        GameManager.Instance.playerInput.SwitchCurrentActionMap("UI");

        animator.SetBool("IsOpen", true);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {   
        StopAllCoroutines();

        if (!_isTyping)
        {
            if (_sentences.Count == 0)
            {
                EndDialogue();
                return;
            }
            
            _sentence = _sentences.Dequeue();
            _speaker = _speakers.Dequeue();

            nameText.text = _speaker;
            StartCoroutine(TypeSentence(_sentence));
            
        }
        else
        {
            FinishTypingEarly(_sentence);
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        _isTyping = true;

        dialogueText.text = "";

        string originalText = sentence;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char letter in sentence.ToCharArray())
        {
            alphaIndex++;
            dialogueText.text = originalText;
            displayedText = dialogueText.text.Insert(alphaIndex, _HTML_ALPHA);

            dialogueText.text = displayedText;
            
            yield return new WaitForSeconds(_MAX_TYPE_TIME / _typeSpeed);
        }

        _isTyping = false;
    }

    private void FinishTypingEarly(string sentence)
    {
        dialogueText.text = sentence;
        _isTyping = false;
    }

    public void EndDialogue()
    {
        UIManager.Instance.CloseDialogueWindow();
        GameManager.Instance.playerInput.SwitchCurrentActionMap("Player");

        animator.SetBool("IsOpen", false);
        
        _sentence = null;
        Debug.Assert(!_isTyping, "The dialogue already ended but _isTyping is true");
    }
}

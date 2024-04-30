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
    private Queue<string> _sentences;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Animator animator;

    private void Awake()
    {
        CreateSingleton(this);
    }

    private void Start()
    {
        _sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {

        nameText.text = dialogue.name;

        _sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }
        
        UIManager.Instance.OpenDialogueWindow();
        GameManager.Instance.playerInput.SwitchCurrentActionMap("UI");

        animator.SetBool("IsOpen", true);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // Enter 키를 누르면 빠르게 완성되도록 하기
    // Enter 키를 다시 누르면 넘어가도록 하기
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        UIManager.Instance.CloseDialogueWindow();
        GameManager.Instance.playerInput.SwitchCurrentActionMap("Player");

        animator.SetBool("IsOpen", false);
    }
}

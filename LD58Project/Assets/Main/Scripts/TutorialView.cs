using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialView : MonoBehaviour {
    [SerializeField] Button _continueBtn;
    [SerializeField] TextMeshProUGUI _tmp;
    [SerializeField, TextArea(2, 10)] string _text;

    DialogueVertexAnimator _dialogueVertexAnimator;
    Coroutine typeRoutine;
    
    public event Action OnNext = delegate { };
    
    void Awake() {
        _dialogueVertexAnimator = new(_tmp, null);
        _continueBtn.onClick.AddListener(() => OnNext.Invoke());
        PlayDialogue(_text);
    }
    
    void PlayDialogue(string message) {
        this.EnsureCoroutineStopped(ref typeRoutine);
        _dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(message, out string totalTextMessage);
        typeRoutine = StartCoroutine(_dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, null, null));
    }
}

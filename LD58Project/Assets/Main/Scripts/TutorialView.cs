using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialView : MonoBehaviour {
    [SerializeField] Button _continueBtn;
    [SerializeField] TextMeshProUGUI _continueBtnText;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _tmp;
    [SerializeField, TextArea(2, 10)] string _text;

    [Header("Params")]
    [SerializeField] float _waitBeforeTextBegin = 0;
    [SerializeField] float _imageBeginAlpha;
    [SerializeField] float _imageAlphaDur = 1f;
    [SerializeField] float _contBtnBeginAlpha = 0, _contBtnAlphaDur = 0.5f, _contBtnDelay = 1f;

    DialogueVertexAnimator _dialogueVertexAnimator;
    Coroutine typeRoutine;
    
    public event Action OnNext = delegate { };

    void Update() {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public IEnumerator Begin() {
        gameObject.SetActive(true);
        _continueBtn.image.color = _continueBtn.image.color.WithAlpha(_imageBeginAlpha);
        _continueBtnText.color = _continueBtnText.color.WithAlpha(_imageBeginAlpha);
        Tween.Alpha(_image, _imageBeginAlpha, 1f, _imageAlphaDur);
        Tween.Alpha(_continueBtn.image, _contBtnBeginAlpha, 1f, _contBtnAlphaDur,
            startDelay: _contBtnDelay); 
        Tween.Alpha(_continueBtnText, _contBtnBeginAlpha, 1f, _contBtnAlphaDur,
            startDelay: _contBtnDelay);

        yield return new WaitForSeconds(_waitBeforeTextBegin);
        
        _dialogueVertexAnimator = new(_tmp, null);
        PlayDialogue(_text);
        OnNext += Closure;
        _continueBtn.onClick.AddListener(() => OnNext.Invoke());
    }
    void Closure() {
        gameObject.SetActive(false);
    }

    void PlayDialogue(string message) {
        this.EnsureCoroutineStopped(ref typeRoutine);
        _dialogueVertexAnimator.textAnimating = false;
        List<DialogueCommand> commands = DialogueUtility.ProcessInputString(message, out string totalTextMessage);
        typeRoutine = StartCoroutine(_dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, null, null));
    }
}

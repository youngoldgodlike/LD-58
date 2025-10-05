using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
    [SerializeField] TutorialView _mobsTutor;
    
    void Awake() {
        if (PlayerPrefs.GetInt("TutorialCompleted") == 1) Destroy(gameObject);
        
        
    }
}
public class TutorialView : MonoBehaviour {
    [SerializeField] Button _continueBtn;

    public event Action OnNext = delegate { };
    
    void Awake() {
        _continueBtn.onClick.AddListener(() => OnNext.Invoke());
    }
    
}

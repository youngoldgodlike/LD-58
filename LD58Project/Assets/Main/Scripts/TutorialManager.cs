using UnityEngine;
public class TutorialManager : MonoBehaviour {
    [SerializeField] TutorialView _mobsTutor;
    
    void Awake() {
        if (PlayerPrefs.GetInt("TutorialCompleted") == 1) Destroy(gameObject);
        
        
    }
}

using Main.Scripts;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
    [SerializeField] TutorialView _mobsTutor, _beginTutor;
    [SerializeField] Image _blackScreen;

    Spawner _spawner;
    TowerZek _tower;
    Player _player;
    
    public void Initialize(Spawner spawner, TowerZek tower, Player player) {
        _spawner = spawner;
        _tower = tower;
        _player = player;
        
        _player.Disable();
        
        if (PlayerPrefs.GetInt("TutorialCompleted") == 1) return;
        
        _blackScreen.color = _blackScreen.color.WithAlpha(1);
        StartCoroutine(_beginTutor.Begin());

        _beginTutor.OnNext += ContinueGame;
    }
    void ContinueGame() {
        _spawner.SetActive(true);
        _tower.StartAttack();
        _player.Enable();
        Tween.Alpha(_blackScreen, 0f, 0.65f);
    }
}

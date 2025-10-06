using Main.Scripts;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
public static class ColorExt {
    public static Color WithAlpha(this Color color, float a) => new Color(color.r, color.g, color.b, a);
}
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

        if (PlayerPrefs.GetInt("TutorialCompleted") == 1) {
            _blackScreen.color = _blackScreen.color.WithAlpha(0);
            return;
        }
        
        
        _blackScreen.color = _blackScreen.color.WithAlpha(1);
        StartCoroutine(_beginTutor.Begin());

        _beginTutor.OnNext += ContinueGame;
        spawner.OnSpawn += ShowEnemyTutor;
    }
    void ContinueGame() {
        _spawner.SetActive(true);
        _tower.StartAttack();
        _player.Enable();
        Tween.Alpha(_blackScreen, 0f, 0.65f);
    }
    
    void ShowEnemyTutor() {
        _spawner.OnSpawn -= ShowEnemyTutor;
        _spawner.SetActive(false);
        _tower.StopAttack();
        _player.Disable();
        StartCoroutine(_mobsTutor.Begin());
        _mobsTutor.OnNext += () => {
            _spawner.SetActive(true);
            _tower.StartAttack();
            _player.Enable();
            PlayerPrefs.SetInt("TutorialCompleted", 1);
        };
    }
}

using Main.Scripts;
using UnityEngine;

public class EntryPoint : MonoBehaviour {
    [SerializeField] Player _player;
    [SerializeField] Spawner _spawner;
    [SerializeField] Terminal _terminal;
    [SerializeField] TutorialManager _tutorialManager;
    [SerializeField] TowerZek _tower;

    void Awake() {
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        _terminal.Initialize();
        _spawner.Initialize(_terminal,false);
        _tower.Initialize(false);
        _player.Initialize();
        _tutorialManager?.Initialize(_spawner, _tower, _player);
    }
}

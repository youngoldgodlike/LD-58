using Main.Scripts;
using UnityEngine;

public class EntryPoint : MonoBehaviour {
    [SerializeField] Player _player;
    [SerializeField] Spawner _spawner;
    [SerializeField] Terminal _terminal;
    [SerializeField] TutorialManager _tutorialManager;
    [SerializeField] TowerZek _tower;

    void Awake() {
        _terminal.Initialize();
        _spawner.Initialize();
        _tower.Initialize(true);
        _player.Initialize();
        _tutorialManager?.Initialize(_spawner, _tower, _player);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class BattleSystem : MonoBehaviour
{
    public enum StateBattle
    {
        TEST,
        PREBATTLE,
        BATTLE,
        RESULTS

    }
    public StateBattle battlestate;

    public static BattleSystem Instance;

    private IAEnemySys[] _enemys;

    private PlayerSystem _player;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private PlayableDirector present;
    [SerializeField] private GameObject panel;
    [SerializeField] private float _presentTime;
    private ScoreSystem scoreSys;
    private SoundSystem soundfx;
    private bool pause;
    private void Start()
    {

        panel.SetActive(false);
        _enemys = FindObjectsOfType<IAEnemySys>();
        _player = FindObjectOfType<PlayerSystem>();
        scoreSys = GetComponent<ScoreSystem>();
        soundfx = GetComponent<SoundSystem>();
        if (scoreSys.lvl > 1)
        {
            StartGame();
        }
        
        
    }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void StartGame()
    {
        StartCoroutine(Presentation());
        GameManager.Instance.ChangeCursorBattle();
       
    }
  
    IEnumerator Presentation()
    {
        present.Play();
        yield return new WaitForSeconds(_presentTime);
        
        battlestate = StateBattle.PREBATTLE;
        count.gameObject.SetActive(true);
        count.text = "3";
        soundfx.Soundfx03();
        yield return new WaitForSeconds(1);
        count.text = "2";
        soundfx.Soundfx03();
        yield return new WaitForSeconds(1);
        count.text = "1";
        soundfx.Soundfx03();
        yield return new WaitForSeconds(1);
        Battle();
        count.text = "Desenfunda";
        soundfx.Soundfx03();
        yield return new WaitForSeconds(0.5f);
        count.gameObject.SetActive(false);
        
        yield return null;

    }
    private void Battle()
    {

        battlestate = StateBattle.BATTLE;
        for (int i = 0; i < _enemys.Length; i++)
        {
            _enemys[i].Desenfundar();
        }

    }

    public void CheckDead()
    {
        for (int i = 0; i < _enemys.Length; i++)
        {
            if (_enemys[i].GetDead() == false)
            {
                Debug.Log("Faltan enemigos por matar");

                return;
            }
        } 
        battlestate = StateBattle.RESULTS;
        _player.EnfundarPistola();
        scoreSys.SetWinCondition(true);
        StartCoroutine(Results());

    }
    public void Lose()
    {
        
        scoreSys.SetWinCondition(false);
        battlestate = StateBattle.RESULTS;
        StartCoroutine(Results());


    }
    IEnumerator Results()
    {
        if (_enemys.Length == 1)
        {
            if ((_player.GetDead()) && (_enemys[0].GetDead()))
            {
                scoreSys.Draw(true);
            }
        }
        
        yield return new WaitForSeconds(2);
        scoreSys.CalculateResults();
        panel.SetActive(true);
    }
}

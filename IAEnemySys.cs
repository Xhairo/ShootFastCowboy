using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IAEnemySys : MonoBehaviour
{
    #region Variables Settings
    private bool _dead;//para comprobar si estas muerto

    //referencias
    private PlayerSystem _player;
    [SerializeField]
    private Transform _target;
    
    enum _dificultad
    {
        NONE,
        EASY,
        NORMAL,
        HARD
    }
    [SerializeField]
    private _dificultad miDificultad;
    [SerializeField]
    private float[] _enemyShootPorcent = { 0.30f, 1f };//success,failure

    [SerializeField]
    private GameObject _revolver;
    [SerializeField]
    private GameObject _handGrip;
    [SerializeField]
    private GameObject _fundArm;
    //REFERENCIAS A OTROS SCRIPTS
    private AnimationSystem animSys;
    [SerializeField] private Recoil _recoil;
    [SerializeField] private RigBuilder rigAim;
    private SoundSystem _soundfx;
    #endregion

    private void Awake()
    {
        animSys = GetComponent<AnimationSystem>();
        rigAim = GetComponent<RigBuilder>();
        _soundfx = GetComponent<SoundSystem>();
        
    }
    private void Start()
    {
        _player = FindObjectOfType<PlayerSystem>();
        SetEnemy();//set enemy defficult
        _target = GameObject.FindGameObjectWithTag("EnemyTarget").transform;
    }

    #region funciones
    public void SetEnemy()
    {
        _balas = 6;
        switch (miDificultad)
        {
            case _dificultad.NONE:
                velAtaque = 10;
                _enemyShootPorcent[0] = 0.05f;// porcent of sucess
                _enemyShootPorcent[1] = 0.9f;// porcent of fail
                break;

            case _dificultad.EASY:
                
                _enemyShootPorcent[0] = 0.2f;// porcent of sucess
                _enemyShootPorcent[1] = 0.9f;// porcent of fail
                break;
            case _dificultad.NORMAL:
                
                _enemyShootPorcent[0] = 0.4f;// porcent of sucess
                _enemyShootPorcent[1] = 0.9f;// porcent of fail
                break;
            case _dificultad.HARD:
                
                _enemyShootPorcent[0] = 0.9f;// porcent of sucess
                _enemyShootPorcent[1] = 1f;// porcent of fail
                break;



        }
        Debug.Log("Enemy Mode:"+miDificultad+"y velAtaq:"+velAtaque);
    }

    private IEnumerator GripArm()
    {

        yield return new WaitForSeconds(0.5f);
        Debug.Log("Agarrararma");
        _revolver.transform.SetParent(_handGrip.transform);
        _revolver.transform.position = _handGrip.transform.position;
        _revolver.transform.rotation = _handGrip.transform.rotation;
        yield return new WaitForSeconds(0.2f);
        isAiming = true;
        yield return null;

    }

    //funcion de muerte y reason sirve para identificar que animacion hacer segun impacto
    void Dead()
    {
       
        animSys.DeathAnim();

    }
    public void  SetDead(bool deadset)
    {
        _soundfx.Soundfx03();
        _dead = deadset;
        Dead();
    }
    public bool GetDead()
    {
        return _dead;
    }
    #endregion


    #region Patrones

    private void Update()
    {
        if (!_dead)
        {
           
            if (isAiming)
            {
                rigAim.layers[0].active = true;
                rigAim.layers[1].active = true;
            }
            else
            {
                rigAim.layers[0].active = false;
                rigAim.layers[1].active = false;
            }

        }
        else
        {
            isAiming = false;
            rigAim.layers[0].active = false;
            rigAim.layers[1].active = false;
        }
        
    }

    public void Desenfundar()//Funcion de Desenfundar
    {
        _desenfundado = true;
        animSys.DesenfundaAnim();
        StartCoroutine(GripArm());
        StartCoroutine(IaShooting());
    }

    
    

    #endregion

    #region ShootSys
    private bool _desenfundado;
    private bool isAiming;
    private int _balas;
    [SerializeField] private float velAtaque;

    [SerializeField] private Transform pointofShoot;
    [SerializeField] private ParticleSystem shoot;
    [SerializeField] private TrailRenderer bullettrail;
    [SerializeField] private ParticleSystem impactbody;
    [SerializeField] private ParticleSystem impactground;

    public void Shoot(Vector3 target, string fx)//Funcion de Disparo
    {
        if (_desenfundado == true)
        {
            _soundfx.Soundfx01();
            animSys.ShootAnim();
            _recoil.RecoilFire();
            _balas--;
            Instantiate(shoot, pointofShoot.position, Quaternion.identity);
            TrailRenderer trail = Instantiate(bullettrail, pointofShoot.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, target,fx));

            
        }
        else
        {
            Debug.Log("desenfunda");
        }
    }
    private void Recharge()
    {
        if (_balas == 6)
        {

            //Debug.Log("Cargador Full");
            return;
        }
        if (_balas < 6)
        {
            for (int i = 0; _balas < 6; i++)
            {
                _balas++;

            }
            // Debug.Log("Recargando");
            //**ANIMACION DE RECARGAR**
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer _Trail, Vector3 hit,string fx)
    {
        float time = 0;
        Vector3 startposition = _Trail.transform.position;

        while (time < 1)
        {
            _Trail.transform.position = Vector3.Lerp(startposition, hit, time);
            time += Time.deltaTime / _Trail.time;

            yield return null;
        }
        _Trail.transform.position = hit;
        if (fx == "Body")
        {
           Instantiate(impactbody, hit, Quaternion.LookRotation(pointofShoot.position));
            _soundfx.Soundfx02();
        }
        if (fx== "Ground")
        {
            Instantiate(impactground, hit, Quaternion.LookRotation(pointofShoot.position));
            
        }
        _soundfx.Soundfx04();

        Destroy(_Trail.gameObject, _Trail.time);



    }

    IEnumerator IaShooting()
    {
        if (BattleSystem.Instance.battlestate == BattleSystem.StateBattle.BATTLE)
        {     
            float digitchoice = Random.Range(0f, 1f);

            yield return new WaitForSeconds(velAtaque);
            if (_dead)
            {
                yield break;
            }
            if(BattleSystem.Instance.battlestate == BattleSystem.StateBattle.RESULTS){

                yield break;
            }
            //Success
            if (digitchoice <= _enemyShootPorcent[0])//0.4
            {
                _target.position = GameObject.FindGameObjectWithTag("ChestPlayer").transform.position;
                Shoot(_target.position, "Body");
                _player.SetDead(true);
                BattleSystem.Instance.Lose();
                Debug.Log("ShootSucces with "+digitchoice);
            }
            //Fail
            else if ((digitchoice <= _enemyShootPorcent[1]) && (digitchoice > _enemyShootPorcent[0]))
            {

                GameObject[] _PlayerMisstarget = GameObject.FindGameObjectsWithTag("TargetPlayer");

                int missTarget = Random.Range(0, _PlayerMisstarget.Length);
                _target.position = _PlayerMisstarget[missTarget].transform.position;
                if (missTarget >= 4)
                {
                    Shoot(_target.position, "Ground");
                }
                else
                {
                    Shoot(_target.position, "none");
                }
                Debug.Log("Shootfai with " + digitchoice);

            }
            
            if (!_dead)
            {
                StartCoroutine(IaShooting());
            }

        }
        else
        {
            yield break;
        }
        
        
    }



    #endregion
}

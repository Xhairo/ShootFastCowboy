using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class PlayerSystem : MonoBehaviour
{
    #region Variables
    [SerializeField]

    //seteo de personaje
    private Vector3 _inticalPos;//LevelSystem Indicara la posicion Inicial del Player
    [SerializeField] private bool _desenfundado;//Para chequear si el juegador esta en posicion de ataque
    [SerializeField] private float _velDeDisparo;//que tan rapido fuiste en disparar


    [SerializeField]
    private bool _dead;//para comprobar si estas muerto
    
    
    //REFERENCIAS
    AnimationSystem animSys;//Script de Animacion
    [SerializeField] private Recoil _recoil;
    private RigBuilder rigAim;
    private ControlSys _controlSys;
    #endregion

    private void Awake()
    {
        _controlSys = GetComponent<ControlSys>();
        animSys = GetComponent<AnimationSystem>();
        rigAim = GetComponent<RigBuilder>();
        ResetPlayer();
        scoreSys = FindObjectOfType<ScoreSystem>();
        _soundfx = GetComponent<SoundSystem>();
    }
    private void Update()
    {

        if (_desenfundado)
        {
            _velDeDisparo += Time.deltaTime;
        }
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
    void Recargar()//Funcion de Recarga
    {
        if (_balas == 6)
        {
            
            Debug.Log("Cargador Full");
            return;
        }
        if (_balas < 6)
        {
            for (int i = 0; _balas < 6; i++)
            {               
                _balas++;
           
            }
            Debug.Log("Recargando");
            //**ANIMACION DE RECARGAR**
        }
        

    }
    public bool GetDesenfundarInfo()
    {
        return _desenfundado;
    }
    public void Desenfundar()//Funcion de Desenfundar
    {
        _desenfundado = true;
        animSys.DesenfundaAnim();
        
    }

    void Dead()
    {
        isAiming = false;
        rigAim.layers[0].active = false;
        rigAim.layers[1].active = false;
        animSys.DeathAnim();
    }
    public void SetDead(bool b)
    {
        _soundfx.Soundfx06();
        _dead = b;
        Dead();
       
    }
    public bool GetDead()
    {
        return _dead;
    }
    
    void ResetPlayer()//recargar player base
    {
        _balas = 6;
        _desenfundado = false;
        _dead = false;
        _velDeDisparo = 0; 
       
    }
    
    public void EnfundarPistola()
    {
      
        rigAim.layers[0].active = false;
        animSys.Enfundar();
        _controlSys.StartCoroutine(_controlSys.Enfundar());
        _desenfundado = false;
    }



    // variables y funciones sobre el disparo
    #region ShootSystem

    //Shoot Variables
    //boquilla de la pistola
    [SerializeField] private Transform pointofShoot;
    public bool isAiming;
    [SerializeField]
    private int _balas;//contador de balas en el cargador
    //Prefabs
    [SerializeField] private ParticleSystem shoot;
    [SerializeField] private TrailRenderer bullettrail;
    [SerializeField] private ParticleSystem impactbody;
    [SerializeField] private ParticleSystem impactground;
    [SerializeField] private ParticleSystem impactwood;

    private ScoreSystem scoreSys;
    private SoundSystem _soundfx;

    //Funcion de Disparo
    public void Shoot(RaycastHit hit)
    {
        if (_balas > 0)
        {
            if (_desenfundado == true)
            {
                _soundfx.Soundfx01();
                animSys.ShootAnim();
                _recoil.RecoilFire();
                //StartCoroutine(Recoil());
                _balas--;
                Instantiate(shoot, pointofShoot.position, Quaternion.identity);
                TrailRenderer trail = Instantiate(bullettrail, pointofShoot.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));

            }
            else
            {
                Debug.Log("desenfunda");
            }
        }
        else if (_balas == 0)
        {
            _soundfx.Soundfx04();
            Debug.Log("Recarga el arma porfavor");
        }

    }
    //efectos de disparo
    private IEnumerator SpawnTrail(TrailRenderer _Trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startposition = _Trail.transform.position;

        while (time < 1)
        {
            _Trail.transform.position = Vector3.Lerp(startposition, hit.point, time);
            time += Time.deltaTime / _Trail.time;

            yield return null;
        }
        _Trail.transform.position = hit.point;
        if (hit.transform.tag=="Enemy")
        {
            IAEnemySys _enemy = hit.transform.GetComponentInParent <IAEnemySys>();
            Instantiate(impactbody, hit.point, Quaternion.LookRotation(hit.normal));
            _enemy.SetDead(true);
            BattleSystem.Instance.CheckDead();
            if (hit.transform.name == "Head")
            {
                scoreSys.Headshoot();
            }
            scoreSys.VeldeDisparo(_velDeDisparo);
            scoreSys.Precision(true);
            _soundfx.Soundfx05();
        }
        else
        {
            scoreSys.Precision(false);
        }
        if (hit.transform.tag == "Ground")
        {
            Instantiate(impactground, hit.point, Quaternion.LookRotation(hit.normal));
            
        }
        if (hit.transform.tag == "Wood")
        {
            Instantiate(impactwood, hit.point, Quaternion.LookRotation(hit.normal));
            scoreSys.Precision(false);
        }

        Destroy(_Trail.gameObject, _Trail.time);



    }



    #endregion


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlSys : MonoBehaviour
{
    [SerializeField]
    private GameObject _revolver;
    [SerializeField]
    private GameObject _handGrip;
    [SerializeField]
    private GameObject _fundArm;


   

    PlayerSystem myPlayer;
    [SerializeField]
    private LayerMask mouseAimMask;
    [SerializeField]
    private Transform _target;
    private SoundSystem _soundfx;



    private void Start()
    {
        myPlayer = GetComponent<PlayerSystem>();
        _target = GameObject.FindGameObjectWithTag("Target").transform;
        _soundfx = GetComponent<SoundSystem>();
    }
    void Update()
    {
        if (BattleSystem.Instance.battlestate == BattleSystem.StateBattle.BATTLE)
        {
            if (myPlayer.isAiming)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
                {
                    _target.position = hit.point;

                }
                if (Input.GetMouseButtonDown(1))
                {
                    myPlayer.Shoot(hit);

                }
            }//Shoot

            if ((Input.GetMouseButtonDown(0)))
            {
                Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit2;
                if (Physics.Raycast(ray2, out hit2, Mathf.Infinity))
                {
                    if (hit2.transform.tag == "MiPistola")
                    {
                        if (myPlayer.GetDesenfundarInfo() == false)
                        {
                            
                            myPlayer.Desenfundar();
                            Debug.Log("desenfundado");
                            StartCoroutine(GripArm());

                        }
                    }
                }
            }//Desenfundar y Recargar
           
        }
         
    }

    private IEnumerator GripArm()
    {
        _soundfx.Soundfx02();
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Agarrararma");
        _revolver.transform.SetParent(_handGrip.transform);
        _revolver.transform.position = _handGrip.transform.position;
        _revolver.transform.rotation = _handGrip.transform.rotation;
        yield return new WaitForSeconds(0.1f);
        myPlayer.isAiming = true;
        yield return null;

    }
    public IEnumerator Enfundar()
    {
        myPlayer.isAiming = false;
        Animator revolveranim = _revolver.GetComponent<Animator>();
        revolveranim.SetTrigger("Giro");
        _soundfx.Soundfx07();
        yield return new WaitForSeconds(1f);
        _soundfx.Soundfx03();
        yield return new WaitForSeconds(0.7f);
        
        _revolver.transform.SetParent(_fundArm.transform);
        _revolver.transform.position = _fundArm.transform.position;
        _revolver.transform.rotation = _fundArm.transform.rotation;
    }




}

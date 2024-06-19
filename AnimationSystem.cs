using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
   
    private Animator anim;


    [SerializeField]
    private GameObject revolver;
    
    public float recoil;
    [SerializeField] bool cantDeadForward;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void DesenfundaAnim()
    {
        anim.SetTrigger("Desenfundado");
    }
    public void ShootAnim()
    {
       
        
        anim.SetTrigger("Shoot");
    }
    public void DeathAnim()
    {
        if (cantDeadForward)
        {
            anim.SetInteger("AnimID", 0);
            anim.SetTrigger("Dead");
        }
        else
        {
            int randomDead = Random.Range(0, 2);
            anim.SetInteger("AnimID", randomDead);
            anim.SetTrigger("Dead");
        }
        
            
        
        
    }
    public void Enfundar()
    {


        anim.SetTrigger("Enfundar");
    }





}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region vairables
    public static GameManager Instance;
    public GameObject[] menuitems;


    public Texture2D cursorMENU;
    public Vector2 menucursorhotspot;
    public Texture2D cursorAim;
    public Vector2 aimcursorhotspot;

    public enum _layout
    {
        LOGIN,//login
        MENU,//Menu
        LEVEL,//Level
        VS//Mecanica futura
    }
    public _layout _actualLayaut;//variable que sirve para decir en que layaout nos encontramos





    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);

        }
        else if(Instance!=this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {      
       
        Login();
       

    }
    #region InGameSelection

    #endregion

    public void ChangeCursorBattle()
    {
        Cursor.SetCursor(cursorAim, aimcursorhotspot, CursorMode.Auto);
    }
    public void ChangeCursorMenu()
    {
        Cursor.SetCursor(cursorMENU, menucursorhotspot, CursorMode.Auto);
    }

    public void Login()
    {
        _actualLayaut = _layout.LOGIN;
       
        foreach (var item in menuitems)
        {
            item.SetActive(false);
        }
    }
    public void Menu()
    {
       
        foreach (var item in menuitems)
        {
            item.SetActive(true);
        }
        _actualLayaut = _layout.MENU;
    }
    public void SetMenu()
    {
        menuitems = GameObject.FindGameObjectsWithTag("UIMenu");
       
    }

}


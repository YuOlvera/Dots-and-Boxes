using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Funciones : MonoBehaviour
{
    public void Play(){
        SceneManager.LoadScene("Juego");
    }

    public void Quit(){
        Application.Quit();
    }
}

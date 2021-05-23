using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Linea : MonoBehaviour
{
    [SerializeField] private Button boton;
    public List<GameObject> cuadros;
    [SerializeField] private GameManager gameManager;
    public string turno;

    void Awake() {
        boton = GetComponent<Button>();
    }
    public void AccionBoton(){ //Se llama cada vez que das clic en el botón
        gameManager.ReproducirSonido(); //Hace POP!
        turno = gameManager.GetTurno();
        boton.interactable = false;
        CambiarColorBoton();
        bool nuevoTurno = false;
        //Cambiar la cantidad de líneas existentes en los cuadros asociados a la línea creada
        foreach(GameObject cuadro in cuadros){ //Buscamos en todos los cuadros del tablero
            //Buscamos en la lista de Lineas de ese cuadro si se encuentra la que seleccionamos
            foreach(var l in cuadro.GetComponent<Cuadro>().Lineas){ 
                if(l.name == this.name){
                    cuadro.GetComponent<Cuadro>().lineasPintadas++;
                    if(cuadro.GetComponent<Cuadro>().lineasPintadas == 4){ //Si ya se completó ese cuadro
                        if(turno == "IA"){
                            cuadro.GetComponent<Cuadro>().firma = "IA";
                            gameManager.puntuacionIA++;
                        }else{ 
                            cuadro.GetComponent<Cuadro>().firma = "PLAYER";
                            gameManager.puntuacionPlayer++;
                        }
                        nuevoTurno = true;
                        //Rellenar el cuadro con el color del turno actual
                        if(turno=="IA")
                            cuadro.GetComponent<Image>().color = new Color32(9,130,173,255);
                        else
                            cuadro.GetComponent<Image>().color = new Color32(43,152,25,255);
                    }
                }
            }
        }
        //Permitir que el gameManager revise si el juego ha terminado
        gameManager.GameOver();
        if(!nuevoTurno)
            gameManager.TerminarTurno(); //Tras escoger termina su turno, en caso de haber ganado llama a la función gameover
    }
    public void SetReferenciaGameManager(GameManager manager){
        gameManager = manager;
    }
    
    public void CambiarColorBoton(){
        if(turno == "IA"){
            boton.image.color = new Color32(9,130,173,255);
        }else if(turno == "PLAYER"){
            boton.image.color = new Color32(43,152,25,255);
        }
    }
}

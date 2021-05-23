using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<Button> listaLineas;
    public List<Cuadro> cuadros;
    public List<Punto> puntos;
    private int cantidadCuadros = 25;
    private string turno = ""; // IA / PLAYER
    public GameObject YouWin, YouLose;

    private bool turnoIA = true;
    private bool decisionIA = false;
    private int cantidadTurnosIA = 0; 

    public List<Punto> puntosTemporales = new List<Punto>();

    //Para sonidos
    public AudioSource audioSource;
    public AudioClip pop;

    //Para puntajes
    public int puntuacionPlayer = 0;
    public int puntuacionIA = 0;
    public Text puntosPlayer, puntosIA;
    public Image indicadorTurnoIA, indicadorTurnoPlayer;
    void Awake()
    {
        audioSource.GetComponent<AudioSource>();
        turno = "PLAYER";
        SetReferenciaLineasDeGameManager();
        YouLose.SetActive(false);
        YouWin.SetActive(false);
        indicadorTurnoIA.gameObject.SetActive(false);
        indicadorTurnoPlayer.gameObject.SetActive(false);
    }

    void Update() {
        if(turno == "IA"){
            if(turnoIA){
                Debug.Log("Turno IA");
                turnoIA = false; //Para que no se repita el update
                //StartCoroutine(FuncionCosto1());
                StartCoroutine(FuncionCosto2());
            }
        }
        //Actualizar los puntajes
        puntosIA.text = puntuacionIA.ToString();
        puntosPlayer.text = puntuacionPlayer.ToString();

        if(turno == "IA"){
            indicadorTurnoIA.gameObject.SetActive(true);
            indicadorTurnoPlayer.gameObject.SetActive(false);
        }else{
            indicadorTurnoPlayer.gameObject.SetActive(true);
            indicadorTurnoIA.gameObject.SetActive(false);
        }
    }

    public void ReproducirSonido(){
        audioSource.PlayOneShot(pop);
    }

    public IEnumerator FuncionCosto1(){
        /* Esta función de costo realiza lo siguiente:
            - Evitar que pinte la tercera línea en cualquier cuadro (para que no empiece a llenar el jugador), sólo lo hará como último recurso
            - Si cerca de esta líneahay cuadros con 3 líneas pintadas llenarlos, sino escoger alguna línea al azar disponible que cumpla lo de arriba
            - Llamar a AccionBoton()
        */
        //Escoge una línea del tablero
        int lineaEscogida = Random.Range(0,59); //Sirve a manera de selección
        string nombreLinea = listaLineas[lineaEscogida].name;

        //Regla general
        foreach(Cuadro c in cuadros){
            if(c.lineasPintadas == 3){ //Si el agente encontró un cuadro con 3 líneas pintadas
                //Pinta esa última línea para impedir que el player gane
                foreach(var linea in c.Lineas){
                    if(linea.GetComponent<Button>().interactable){
                        linea.GetComponent<Linea>().AccionBoton();
                        goto salto;
                    }
                }
            }
        }

        //Busco los cuadros que tengan esa línea
        foreach(Cuadro c in cuadros){
            foreach(var l in c.Lineas){
                if(l.name == nombreLinea){ //Si existe la línea escogida en la listadelineas del cuadro
                    if(c.lineasPintadas == 3){
                        foreach(var linea in c.Lineas){ //Rellena el cuadro buscando la línea que está disponible
                            if(linea.GetComponent<Button>().interactable){
                                linea.GetComponent<Linea>().AccionBoton();
                                goto salto;
                            }
                        }
                    }else{
                        //Sólo pinta la línea
                        if(l.GetComponent<Button>().interactable){
                            l.GetComponent<Linea>().AccionBoton();
                        }
                        goto salto;
                    }
                }
            }
        }
        salto:
        yield return new WaitForSeconds(0.2f);
        turnoIA = true;
    }

    public IEnumerator FuncionCosto2(){
        /* Esta función de costo realiza lo siguiente:
            - Crea una línea inicial en cualquier posición en el primer turno de la IA
            - Los puntos que tienen al menos una línea no interactuable se agregan a una lista de puntos temporales
            - Después en siguientes turnos la IA recorre la lista en busca de puntos con al menos una línea no interactuable y 
                escoge al primero que encuentra para crear una línea adyacente
            - La IA sigue el mismo comportamiento de bloquear jugadas cuando detecta que el jugador está a punto de llenar un cuadro
        */
        if(cantidadTurnosIA == 0){ //Primer turno
            Debug.Log("turno "+ cantidadTurnosIA);
            //Escoge una línea del tablero
            int lineaEscogida = Random.Range(0,59); //Sirve a manera de selección
            string nombreLinea = listaLineas[lineaEscogida].name;
            listaLineas[lineaEscogida].GetComponent<Linea>().AccionBoton();
            foreach(Punto p in puntos){
                foreach(var l in p.Lineas){
                    if(l.name == nombreLinea){
                        puntosTemporales.Add(p); //Agregamos a la lista temporal de puntos
                    }
                }
            }
            goto salto;
        }else{
            Debug.Log("turno "+ cantidadTurnosIA);
            //Regla general
            foreach(Cuadro c in cuadros){
                if(c.lineasPintadas == 3){ //Si el agente encontró un cuadro con 3 líneas pintadas
                    //Pinta esa última línea para impedir que el player gane
                    foreach(var linea in c.Lineas){
                        if(linea.GetComponent<Button>().interactable){
                            linea.GetComponent<Linea>().AccionBoton();
                            goto salto;
                        }
                    }
                }
            }

            foreach(Punto p in puntos){
                foreach(var l in p.Lineas){
                    if(!l.GetComponent<Button>().interactable){ //Si hay al menos una línea no interactuable
                        if(!puntosTemporales.Contains(p))
                            puntosTemporales.Add(p);
                    }
                }
            }
            
            foreach(Punto p in puntosTemporales){
                foreach(var l in p.Lineas){
                    if(l.GetComponent<Button>().interactable){ //Toma cualquier línea interactuable y dibujala, se cumple adyacencia 
                        l.AccionBoton();
                        goto salto;
                    }
                }
            }
        }
        
        salto:
            cantidadTurnosIA++;
            yield return new WaitForSeconds(0.2f);
            turnoIA = true;
    }


    public string GetTurno(){
        return turno; //Regresa una X u O al botón
    }

    public void CambiaTurno(){
        turno = (turno == "IA") ? "PLAYER" : "IA";
    }

    void SetReferenciaLineasDeGameManager(){
        foreach(var linea in listaLineas){
            linea.GetComponent<Linea>().SetReferenciaGameManager(this); //Referencia del código Boton; Asigna el GameManager
        }
    }

    public void TerminarTurno(){
        CambiaTurno();
    }

    public void GameOver(){
        int cont = 0;
        int playerCont = 0;
        int IAcont = 0;
        foreach(Cuadro c in cuadros){
            if(c.lineasPintadas == 4){
                cont++;
                if(c.firma == "IA")
                    IAcont++;
                if(c.firma == "PLAYER")
                    playerCont++;
            }
        }
        if(cont == cantidadCuadros){ //Si todos los cuadros están completos
            foreach(Button b in listaLineas){ //Desactiva todos los botones
                b.interactable = false;
            }
            if(IAcont > playerCont)
                YouLose.SetActive(true);
            else
                YouWin.SetActive(true);
            StartCoroutine(ReiniciarJuego());
        }
    }

    public IEnumerator ReiniciarJuego(){
        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene("Juego");
    }
}

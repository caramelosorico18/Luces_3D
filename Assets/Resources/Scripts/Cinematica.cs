using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///Cinematica2
///1. Crea un GameObjet vacio llamado "Cinematica2"
///2. Arrastra este script al objeto
/// 3. en el inspector asigna:
///-CamaraA: Tu cámara principal
///-CamaraB: Una segunda cámara en una posición y angulos diferentes
///-Objeto1: Personaje Principal
///Objeto2: El objeto hacia el que se mueve  
/// 4. Dale play - La cinemática arranca sola
/// 5. Al terminar, camara A se reactiva para volver al gameplay
///</Summary>

public class Cinematica : MonoBehaviour
{
    [Header("Cámaras")]
    public Camera camaraA; //camara principal
    public Camera camaraB; //Otra cámara con otro angulo

    [Header("Objetos de la escena")] //Objeto que se mueve durante la cinemática
    public Transform objeto1; //Objeto que se mueve durante la cinemática
    public Transform puntoIntermedio;
    public Transform destinoObjeto2; //Objeto meta de objeto1

    [Header("Configuración de movimiento")]
    public float velocidadMovimiento1 = 0.1f; //Controla la velocidad - equivale a MetroSeg en trasladarObjeto
    public float escalaSlowMo = 0.3f; //valor del SlowMotion en % de la velocidad real
    public float velocidadFade = 0.5f; //Duración de la transición del fade

    private float tiempoTranscurrido = 0f; //acumula tiempo por frame, equivale a Time.deltaTime
    public UnityEngine.UI.Image pantallaFade;
    //Imagen para el fade, crea una imegen en negro en un canvas, asignar la imagen en el inspector
    void Start()
    {
        camaraA.enabled = true;
        camaraB.enabled = false;
        if (pantallaFade != null)
        {
            pantallaFade.color = new Color(0, 0, 0, 0); //100% trasparente
        }
        StartCoroutine(SecuenciaCinematica());
    }

    //FADE: Oscurece o aclara la pantalla interpolando el alpha
    //Uso interno: yield return StartCoroutine(Fade(0f, 1f)) para fundir a negro
    //yield return Startcoroutine(Fade(1f, 0f)) para quitar negro

    IEnumerator Fade(float alphaInicio, float alphaFin)
    {
        if (pantallaFade == null) yield break; //Si no se asigna imagen, se omite
        float t = 0f;
        while (t > velocidadFade)
        {
            t += Time.unscaledDeltaTime; //Usamos tiempo real para que funcione el eslowMo
            float alpha = Mathf.Lerp(alphaInicio, alphaFin, t / velocidadFade);
            pantallaFade.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        pantallaFade.color = new Color(0, 0, 0, alphaFin);
    }

    IEnumerator SecuenciaCinematica()
    {
        //Fase1
        //Basado en Vector3Movimiento (Vector3.down) y TiempoDelta
        //objeto1 rota para mirar a destinoObjeto2 mientras la cámara baja

        Debug.Log("Inicio Fase1");
        float duracionFase1 = 5f;
        tiempoTranscurrido = 0f;

        Vector3 posInicioCam = camaraA.transform.position;
        Vector3 posFinCam = posInicioCam + Vector3.down * 4f;

        Quaternion rotInicio = objeto1.rotation;
        objeto1.LookAt(destinoObjeto2);
        Quaternion rotFin = objeto1.rotation;
        objeto1.rotation = rotInicio;

        while (tiempoTranscurrido < duracionFase1)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracionFase1;


            camaraA.transform.position = Vector3.Lerp(posInicioCam, posFinCam, t); //Mueve la cámara hacia abajo
            objeto1.rotation = Quaternion.Slerp(rotInicio, rotFin, t); //objeto1 rota hacia objeto2

            yield return null;
        }

        //Fase2: CámaraA se fija mirando a objetivo1 (LookAt)
        //LookAt es exactamente lo que hacia MirarHacia en su Update()

        Debug.Log("Inicio Fase2");
        camaraA.transform.LookAt(objeto1);
        yield return new WaitForSeconds(1f);

        //Fase3: Objeto1 se traslada hacia objeto2 (TrasladaObjeto)
        //La cámara lo sigue con LookAt en tiempo real

        Debug.Log("Inicio Fase3");
        float duracionFase3 = 4f;
        tiempoTranscurrido = 0f;

        Vector3 origenObjeto1 = objeto1.position;
        Vector3 destinoObjeto1 = destinoObjeto2.position;

        while (tiempoTranscurrido < duracionFase3)
        {
            //Time.deltaTime equivale al metrosSeg * Time.deltatime de trasladarObjeto
            tiempoTranscurrido += Time.deltaTime;
            objeto1.position = Vector3.Lerp(origenObjeto1, destinoObjeto1, tiempoTranscurrido / duracionFase3);
            camaraA.transform.LookAt(objeto1); //Mira a objeto1 en tiempo real
            yield return null;
        }

        //Fase4: Fade a negro, cambio a CámaraB, Fade de vuelta
        //+ slow motion al abrir (EscalarTiempo)

        //Oscurece la pantalla(fade out)
        Debug.Log("Inicio Fase4");
        yield return StartCoroutine(Fade(0f, 1f));

        //Cambiamos a cámaraB
        camaraA.enabled = false;
        camaraB.enabled = true;
        camaraB.transform.LookAt(objeto1); //Apuntamos a objeto1
        Time.timeScale = escalaSlowMo; //Activamos slowMo

        yield return StartCoroutine(Fade(1f, 0f));

        yield return new WaitForSecondsRealtime(2f);//Mantenemos el slowMo 2 segundos

        Time.timeScale = 1f; //Restauramos la velocidad de reproducción

        //Fase5: objeto2 se mueve a posición final (Vector3Constructor)
        //CámaraB lo sigue con LookAt

        //Vector3Constructor: Posición destino devinida con nuevo Vector3
        //Cambia estos valores en el inspector o haciendolos aqui

        Debug.Log("Inicio Fase5");
        Vector3 posicionFinal = new Vector3(0f, 1f, 200f);
        float duracionFase5 = 5.5f;
        tiempoTranscurrido = 0f;
        Vector3 origenObjeto2 = destinoObjeto2.position;

        while (tiempoTranscurrido < duracionFase5)
        {
            tiempoTranscurrido += Time.deltaTime;
            destinoObjeto2.position = Vector3.Lerp(origenObjeto2, posicionFinal, tiempoTranscurrido / duracionFase5);

            camaraB.transform.LookAt(destinoObjeto2);
            yield return null;

        }

        //Fin: Fade a negro, restaura cámara A para el gameplay
        //Time.realTimeSinceStartup = concepto de TiempoPasado

        yield return StartCoroutine(Fade(0f, 1f));
        camaraB.enabled = false;
        camaraA.enabled = true;
        yield return StartCoroutine(Fade(1f, 0f));

        Debug.Log($"Cinemática terminada, Tiempo real desde inicio: {Time.realtimeSinceStartup} segundos");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlcamara : MonoBehaviour
{
    // Objeto padre que contiene las cámaras (asignar en el Inspector).
    [SerializeField] private Transform colCams;

    // Array de cámaras obtenido de los hijos de colCams.
    private Camera[] cams;

    // Índice de la cámara que está activa actualmente.
    private int indAct = 0;

    void Start()
    {
        // Actualiza la lista de cámaras al iniciar.
        ActualizarCamaras();
        MostrarCamara(indAct);
    }

    // Actualiza la lista de cámaras según los hijos de colCams.
    void ActualizarCamaras()
    {
        if (colCams != null)
        {
            cams = colCams.GetComponentsInChildren<Camera>();
        }
        else
        {
            Debug.LogError("Colección de cámaras (colCams) no asignada.");
        }
    }

    // Activa solo la cámara en el índice especificado y desactiva el resto.
    void MostrarCamara(int idx)
    {
        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].gameObject.SetActive(i == idx);
        }
    }

    // Función para el botón "adelantar" (siguiente cámara).
    public void Adelantar()
    {
        if (cams == null || cams.Length == 0) return;
        cams[indAct].gameObject.SetActive(false);
        indAct = (indAct + 1) % cams.Length;
        cams[indAct].gameObject.SetActive(true);
    }

    // Función para el botón "retroceder" (anterior cámara).
    public void Retroceder()
    {
        if (cams == null || cams.Length == 0) return;
        cams[indAct].gameObject.SetActive(false);
        indAct--;
        if (indAct < 0)
            indAct = cams.Length - 1;
        cams[indAct].gameObject.SetActive(true);
    }
}

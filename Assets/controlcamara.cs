using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlcamara : MonoBehaviour
{
    // Objeto padre que contiene las c�maras (asignar en el Inspector).
    [SerializeField] private Transform colCams;

    // Array de c�maras obtenido de los hijos de colCams.
    private Camera[] cams;

    // �ndice de la c�mara que est� activa actualmente.
    private int indAct = 0;

    void Start()
    {
        // Actualiza la lista de c�maras al iniciar.
        ActualizarCamaras();
        MostrarCamara(indAct);
    }

    // Actualiza la lista de c�maras seg�n los hijos de colCams.
    void ActualizarCamaras()
    {
        if (colCams != null)
        {
            cams = colCams.GetComponentsInChildren<Camera>();
        }
        else
        {
            Debug.LogError("Colecci�n de c�maras (colCams) no asignada.");
        }
    }

    // Activa solo la c�mara en el �ndice especificado y desactiva el resto.
    void MostrarCamara(int idx)
    {
        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].gameObject.SetActive(i == idx);
        }
    }

    // Funci�n para el bot�n "adelantar" (siguiente c�mara).
    public void Adelantar()
    {
        if (cams == null || cams.Length == 0) return;
        cams[indAct].gameObject.SetActive(false);
        indAct = (indAct + 1) % cams.Length;
        cams[indAct].gameObject.SetActive(true);
    }

    // Funci�n para el bot�n "retroceder" (anterior c�mara).
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

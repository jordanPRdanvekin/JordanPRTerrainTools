using UnityEngine;

public class MoverCam : MonoBehaviour
{
    // ------------------------------
    // Par�metros de Interacci�n (modificables)
    // ------------------------------

    [Header("Control de Rotaci�n (Click Derecho)")]
    [Tooltip("Velocidad de rotaci�n horizontal y vertical al usar el bot�n derecho del rat�n.")]
    [SerializeField] private float velocidadRotacion = 5f;

    [Header("Control de Arrastre (Click Izquierdo)")]
    [Tooltip("Velocidad del arrastre (panning) al usar el bot�n izquierdo del rat�n.")]
    [SerializeField] private float velocidadArrastre = 0.5f;

    [Header("Control de Zoom (Rueda del Rat�n)")]
    [Tooltip("Velocidad de zoom al mover la rueda del rat�n. Si el valor es positivo, la c�mara avanza; si es negativo, retrocede.")]
    [SerializeField] private float velocidadZoomRueda = 10f;

    [Header("Reset Autom�tico")]
    [Tooltip("Tiempo (en segundos) sin interacci�n para iniciar el reset de la c�mara.")]
    [SerializeField] private float tiempoInactividadParaReset = 10f;
    [Tooltip("Velocidad con la que la c�mara vuelve suavemente a su posici�n y rotaci�n originales.")]
    [SerializeField] private float velocidadReset = 5f;

    [Header("L�mites del Zoom")]
    [Tooltip("Valor m�nimo del zoom (distancia m�nima desde la posici�n original).")]
    [SerializeField] private float zoomMinimo = 0f;
    [Tooltip("Valor m�ximo del zoom (distancia m�xima desde la posici�n original).")]
    [SerializeField] private float zoomMaximo = 20f;

    // ------------------------------
    // Variables Internas (no modificables directamente)
    // ------------------------------

    // Almacenan la posici�n y rotaci�n originales de la c�mara
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    // Offsets acumulados por las interacciones:
    private Vector3 offsetArrastre = Vector3.zero;
    private float offsetZoom = 0f;
    private float offsetGiroHorizontal = 0f;
    private float offsetGiroVertical = 0f;

    // Temporizador para detectar inactividad
    private float temporizadorInactividad = 0f;

    // Referencia al componente Camera de este GameObject
    private Camera camara;

    // ------------------------------
    // M�todos del Ciclo de Vida
    // ------------------------------

    // Usamos Awake() para almacenar la posici�n original, incluso si la c�mara est� desactivada inicialmente.
    void Awake()
    {
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
        camara = GetComponent<Camera>();
    }

    // Al activarse la c�mara se reinicia el temporizador y los offsets
    void OnEnable()
    {
        temporizadorInactividad = 0f;
        offsetArrastre = Vector3.zero;
        offsetZoom = 0f;
        offsetGiroHorizontal = 0f;
        offsetGiroVertical = 0f;
        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;
    }

    void Update()
    {
        // Procesa el input solo si este componente Camera est� habilitado
        if (!camara.enabled) return;

        bool hayInteraccion = false;

        // 1. Control de Zoom (Rueda del Rat�n)
        float inputRueda = Input.GetAxis("Mouse ScrollWheel");
        if (inputRueda != 0f)
        {
            hayInteraccion = true;
            offsetZoom += inputRueda * velocidadZoomRueda;
            offsetZoom = Mathf.Clamp(offsetZoom, zoomMinimo, zoomMaximo);
        }

        // 2. Control de Arrastre (Click Izquierdo)
        if (Input.GetMouseButton(0))
        {
            hayInteraccion = true;
            float movimientoX = Input.GetAxis("Mouse X");
            float movimientoY = Input.GetAxis("Mouse Y");
            offsetArrastre += (-transform.right * movimientoX * velocidadArrastre) + (-transform.up * movimientoY * velocidadArrastre);
        }

        // 3. Control de Rotaci�n (Click Derecho)
        if (Input.GetMouseButton(1))
        {
            hayInteraccion = true;
            float movimientoRotX = Input.GetAxis("Mouse X");
            float movimientoRotY = Input.GetAxis("Mouse Y");
            offsetGiroHorizontal += movimientoRotX * velocidadRotacion;
            offsetGiroVertical += -movimientoRotY * velocidadRotacion;
            offsetGiroVertical = Mathf.Clamp(offsetGiroVertical, -80f, 80f);
        }

        // 4. Temporizador y Reset
        if (hayInteraccion)
        {
            temporizadorInactividad = 0f;
        }
        else
        {
            temporizadorInactividad += Time.deltaTime;
            if (temporizadorInactividad >= tiempoInactividadParaReset)
            {
                offsetArrastre = Vector3.Lerp(offsetArrastre, Vector3.zero, Time.deltaTime * velocidadReset);
                offsetZoom = Mathf.Lerp(offsetZoom, 0f, Time.deltaTime * velocidadReset);
                offsetGiroHorizontal = Mathf.Lerp(offsetGiroHorizontal, 0f, Time.deltaTime * velocidadReset);
                offsetGiroVertical = Mathf.Lerp(offsetGiroVertical, 0f, Time.deltaTime * velocidadReset);
            }
        }

        // 5. Actualizaci�n de Transformaci�n
        Quaternion nuevaRotacion = Quaternion.Euler(rotacionOriginal.eulerAngles.x + offsetGiroVertical,
                                                      rotacionOriginal.eulerAngles.y + offsetGiroHorizontal,
                                                      0f);
        transform.rotation = nuevaRotacion;
        transform.position = posicionOriginal + offsetArrastre + nuevaRotacion * Vector3.forward * offsetZoom;
    }
}




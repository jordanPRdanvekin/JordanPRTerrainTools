using UnityEngine;

public class MoverCam : MonoBehaviour
{
    // ------------------------------
    // Parámetros de Interacción (modificables)
    // ------------------------------

    [Header("Control de Rotación (Click Derecho)")]
    [Tooltip("Velocidad de rotación horizontal y vertical al usar el botón derecho del ratón.")]
    [SerializeField] private float velocidadRotacion = 5f;

    [Header("Control de Arrastre (Click Izquierdo)")]
    [Tooltip("Velocidad del arrastre (panning) al usar el botón izquierdo del ratón.")]
    [SerializeField] private float velocidadArrastre = 0.5f;

    [Header("Control de Zoom (Rueda del Ratón)")]
    [Tooltip("Velocidad de zoom al mover la rueda del ratón. Si el valor es positivo, la cámara avanza; si es negativo, retrocede.")]
    [SerializeField] private float velocidadZoomRueda = 10f;

    [Header("Reset Automático")]
    [Tooltip("Tiempo (en segundos) sin interacción para iniciar el reset de la cámara.")]
    [SerializeField] private float tiempoInactividadParaReset = 10f;
    [Tooltip("Velocidad con la que la cámara vuelve suavemente a su posición y rotación originales.")]
    [SerializeField] private float velocidadReset = 5f;

    [Header("Límites del Zoom")]
    [Tooltip("Valor mínimo del zoom (distancia mínima desde la posición original).")]
    [SerializeField] private float zoomMinimo = 0f;
    [Tooltip("Valor máximo del zoom (distancia máxima desde la posición original).")]
    [SerializeField] private float zoomMaximo = 20f;

    // ------------------------------
    // Variables Internas (no modificables directamente)
    // ------------------------------

    // Almacenan la posición y rotación originales de la cámara
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
    // Métodos del Ciclo de Vida
    // ------------------------------

    // Usamos Awake() para almacenar la posición original, incluso si la cámara está desactivada inicialmente.
    void Awake()
    {
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
        camara = GetComponent<Camera>();
    }

    // Al activarse la cámara se reinicia el temporizador y los offsets
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
        // Procesa el input solo si este componente Camera está habilitado
        if (!camara.enabled) return;

        bool hayInteraccion = false;

        // 1. Control de Zoom (Rueda del Ratón)
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

        // 3. Control de Rotación (Click Derecho)
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

        // 5. Actualización de Transformación
        Quaternion nuevaRotacion = Quaternion.Euler(rotacionOriginal.eulerAngles.x + offsetGiroVertical,
                                                      rotacionOriginal.eulerAngles.y + offsetGiroHorizontal,
                                                      0f);
        transform.rotation = nuevaRotacion;
        transform.position = posicionOriginal + offsetArrastre + nuevaRotacion * Vector3.forward * offsetZoom;
    }
}




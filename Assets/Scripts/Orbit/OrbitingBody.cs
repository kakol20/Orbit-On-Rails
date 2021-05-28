using UnityEngine;

[System.Serializable]
public struct ParentBody
{
    public float mass;
    public Transform transform;
}

// https://www.youtube.com/watch?v=Ie5L8Nz1Ns0

public class OrbitingBody : MonoBehaviour
{
    [Header("Body Attributes")]
    [SerializeField] private float semiMajorAxis;
    [SerializeField] [Range(0f, 0.99f)] private float eccentricity;
    [SerializeField] [Range(0, 180f)] private float inclination;
    [SerializeField] [Range(0, 360f)] private float argumentOfPeriapsis;
    [SerializeField] private float longitudeOfAscendingNode;
    [SerializeField] private float meanLongitude;

    public float GetSMA => semiMajorAxis;
    public float GetEcc => eccentricity;
    public float GetIncl => inclination;
    public float GetArgPe => argumentOfPeriapsis;
    public float GetLOAN => longitudeOfAscendingNode;

    private const float gravConst = 6.667f;

    /// <summary>
    /// Standard Gravitational Constant
    /// </summary>
    public float SGP { get; private set; }

    /// <summary>
    /// Mean Angular Motion
    /// </summary>
    public float MAM { get; private set; }

    /// <summary>
    /// True Anomaly Constant
    /// </summary>
    public float TAC { get; private set; }

    public float CosI { get; private set; }
    public float CosLOAN { get; private set; }
    public float SinI { get; private set; }
    public float SinLOAN { get; private set; }

    private float MeanAnomaly { get { return (float)(MAM * (timeElapsed - meanLongitude)); } }

    private float F(float E, float e, float M)  //Function f(x) = 0
    {
        return (M - E + e * Mathf.Sin(E));
    }
    private float DF(float E, float e)      //Derivative of the function
    {
        return (-1f) + e * Mathf.Cos(E);
    }

    /// <summary>
    /// Eccentric Anomaly
    /// </summary>
    private float EccAnomaly
    {
        get
        {
            //float oneTimeMA = MeanAnomaly * Mathf.Deg2Rad;
            float oneTimeMA = MeanAnomaly;

            float e1 = oneTimeMA; // first guess
            float diff = 1;
            float tolerance = 1e-6f;
            int maxIteration = 5;

            for (int i = 0; diff > tolerance && i < maxIteration; i++)
            {
                float e0 = e1;

                // calculate kepler's equation
                float kepler = F(e0, eccentricity, oneTimeMA) / DF(e0, eccentricity);

                e1 = e0 - kepler;
                //if (float.IsNaN(e1)) e1 = 0;

                diff = Mathf.Abs(e1 - e0);
            }

            return e1;
        }
    }

    private float Distance;

    private float TrueAnomaly
    {
        get
        {
            float oneTimeEA = EccAnomaly;

            Distance = semiMajorAxis * (1 - (eccentricity * Mathf.Cos(oneTimeEA)));

            return 2 * Mathf.Atan(TAC * Mathf.Tan(oneTimeEA / 2.0f));
        }
    }

    [Space(10)]
    [SerializeField] private ParentBody parentBody;

    public Vector3 GetCenter => parentBody.transform.position;

    private float timeElapsed;

    private void Awake()
    {
        argumentOfPeriapsis *= Mathf.Deg2Rad;
        inclination *= Mathf.Deg2Rad;
        longitudeOfAscendingNode *= Mathf.Deg2Rad;
        meanLongitude *= Mathf.Deg2Rad;
    }

    private void CalcPos()
    {
        float onetimeTA = TrueAnomaly;

        float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + onetimeTA);
        float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + onetimeTA);

        float x = Distance * ((CosLOAN * cosAOPPlusTA) - (SinLOAN * sinAOPPlusTA * CosI));
        float z = Distance * ((SinLOAN * cosAOPPlusTA) + (CosLOAN * sinAOPPlusTA * CosI));
        float y = Distance * (SinI * sinAOPPlusTA);

        Vector3 newPos = new Vector3(x, y, z);

        transform.position = newPos + parentBody.transform.position;
    }

    public void CalcSemiConstants()
    {
        SGP = parentBody.mass * gravConst;
        MAM = Mathf.Sqrt(SGP / (float)Mathf.Pow(semiMajorAxis, 3));
        TAC = Mathf.Sqrt((1 + eccentricity) / (1 - eccentricity));

        CosLOAN = Mathf.Cos(longitudeOfAscendingNode);
        SinLOAN = Mathf.Sin(longitudeOfAscendingNode);

        CosI = Mathf.Cos(inclination);
        SinI = Mathf.Sin(inclination);
    }
    // Start is called before the first frame update
    private void Start()
    {
        CalcSemiConstants();
        timeElapsed = 0;

        CalcPos();
    }

    // Update is called once per frame
    private void Update()
    {
        timeElapsed += Time.deltaTime * GlobalVariables.Instance.timeScale;
        //timeElapsed = Time.time;

        // calculate position
        CalcPos();
    }
}
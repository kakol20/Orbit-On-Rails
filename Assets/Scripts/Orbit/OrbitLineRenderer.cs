using System.Collections.Generic;
using UnityEngine;

[RequireComponent(requiredComponent: typeof(LineRenderer), requiredComponent2: typeof(OrbitingBody))]
public class OrbitLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private OrbitingBody orbitingBody;
    [SerializeField] private int resolution = 32;
    //[SerializeField] private Material mat;

    private List<Vector3> points = new List<Vector3>();

    public void GetPoints()
    {
        points.Clear();

        Vector3 center = orbitingBody.GetCenter;

        float orbitFraction = 1f / resolution;
        for (int i = 0; i < resolution + 1; i++)
        {
            float eccentricAnomaly = i * orbitFraction * Mathf.PI * 2f;

            float trueAnomaly = 2 * Mathf.Atan(orbitingBody.TAC * Mathf.Tan(eccentricAnomaly / 2f));
            float distance = orbitingBody.GetSMA * (1 - (orbitingBody.GetEcc * Mathf.Cos(eccentricAnomaly)));

            float cosAOPPlusTA = Mathf.Cos(orbitingBody.GetArgPe + trueAnomaly);
            float sinAOPPlusTA = Mathf.Sin(orbitingBody.GetArgPe + trueAnomaly);

            float x = distance * ((orbitingBody.CosLOAN * cosAOPPlusTA) - (orbitingBody.SinLOAN * sinAOPPlusTA * orbitingBody.CosI));
            float z = distance * ((orbitingBody.SinLOAN * cosAOPPlusTA) + (orbitingBody.CosLOAN * sinAOPPlusTA * orbitingBody.CosI));
            float y = distance * (orbitingBody.SinI * sinAOPPlusTA);

            points.Add(center + new Vector3(x, y, z));
        }

        lineRenderer.positionCount = points.Count;
        UpdateLines();
    }

    private void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        orbitingBody = GetComponent<OrbitingBody>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        orbitingBody.CalcSemiConstants();
        GetPoints();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void UpdateLines()
    {
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}
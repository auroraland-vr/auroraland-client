using UnityEngine;
using System.Collections;

public class FoggyLightsOrbits : MonoBehaviour
{

    public float SpinSpeed = 50.0f;
    float X, Y, Z;
    [Range(0, 1)]
    public float _ColorSpeed = 1;
    float ColorSpeed;
    Vector3 RandomRangeXYZ;
    float Opacity;
    void Start()
    {//Get opacity and restore it later
        Opacity = transform.GetChild(0).GetComponent<FoggyLight>().PointLightColor.a;
        RandomRangeXYZ.x = Random.Range(0f, 1f);
        RandomRangeXYZ.y = Random.Range(0f, 1f);
        RandomRangeXYZ.z = Random.Range(0f, 1f);
        //  transform.GetChild(0).GetComponent<FoggyLight>().PointLightColor = new Color(RandomRangeXYZ.x, RandomRangeXYZ.y, RandomRangeXYZ.z);
    }
    void Update()
    {
        ColorSpeed += Time.deltaTime * _ColorSpeed;
        transform.Rotate(0, Time.deltaTime * SpinSpeed, 0);

        X = Mathf.Sin(ColorSpeed * RandomRangeXYZ.x) * 0.5f + 0.5f;
        Y = Mathf.Sin(ColorSpeed * RandomRangeXYZ.y) * 0.5f + 0.5f;
        Z = Mathf.Sin(ColorSpeed * RandomRangeXYZ.z) * 0.5f + 0.5f;
        Color RandomColor = new Color(X, Y, Z, Opacity);
        // print(new Vector4(X, Y, Z, Opacity));
        transform.GetChild(0).GetComponent<FoggyLight>().PointLightColor = RandomColor;
    }

}

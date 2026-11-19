using UnityEngine;
using System.Collections;


public class GalaxyScript : MonoBehaviour
{

    [Header("Shader FX")]
    public Material galaxyShaderMat;
    public float galaxySpeed = 3f;
    public AudioAnalyzer audioAnalyzer;
    public float gradientPulse = 1f;
    public float gradientMultiplier = 2f;
    public float beatSizeMultiplier = 5f;
    public float gradientReturnSpeed = 1f;
    public float offSetpulseX = 1f;
    public float offSetpulseY = 1f;
    public float offsetMultiplier = 4f;
    public float offsetReturnSpeed = 1f;
    public float alpha = 1f;
    public float alphaMultiplier = 10f;
    public float speedMultiplier = 1f;
    private bool offsetPulsing = false;
    private bool gradientPulsing = false;
    private Vector2 ogOffset;
    private float ogGradient;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        galaxyShaderMat.SetFloat("_galaxySpeed", galaxySpeed);
        ogOffset = galaxyShaderMat.GetVector("_OffsetXY");
        ogGradient = galaxyShaderMat.GetFloat("_gradientNoise");
        galaxyShaderMat.SetFloat("_alpha", alpha);
    }

    // Update is called once per frame
    void Update()
    {
        if (AudioAnalyzer.beatDetected)
        {
            if (!gradientPulsing)
            {
                StartCoroutine(PulseGradientOnBeat());
            }
            if (!offsetPulsing)
            {
                StartCoroutine(PulseXYOffsetOnBeat());
            }
        }
    }

    IEnumerator PulseGradientOnBeat()
    {
        gradientPulsing = true;
        float startTime = Time.time;
        float curPulse = gradientPulse * gradientMultiplier + audioAnalyzer.volume;
        float speed = galaxySpeed * speedMultiplier + (audioAnalyzer.volume/100);
        float galaxyAlpha = alpha * alphaMultiplier + (audioAnalyzer.volume / 100);
        float curGradient = galaxyShaderMat.GetFloat("_gradientNoise");
        float curSpeed = galaxyShaderMat.GetFloat("_galaxySpeed");
        float curAlpha = galaxyShaderMat.GetFloat("_alpha");
        while (Time.time - startTime < gradientReturnSpeed)
        {
            float t = (Time.time - startTime) / gradientReturnSpeed;
            float pulse = Mathf.Lerp(curGradient, curPulse, t);
            float s = Mathf.Lerp(curSpeed, speed, t);
            float a = Mathf.Lerp(curAlpha, galaxyAlpha, t);
            galaxyShaderMat.SetFloat("_gradientNoise", pulse);
            galaxyShaderMat.SetFloat("_galaxySpeed", s);
            galaxyShaderMat.SetFloat("_alpha", a);
            yield return null;
        }
        startTime = Time.time;
        curGradient = galaxyShaderMat.GetFloat("_gradientNoise");
        curSpeed = galaxyShaderMat.GetFloat("_galaxySpeed");
        while (Time.time - startTime < gradientReturnSpeed)
        {
            float t = (Time.time - startTime) / gradientReturnSpeed;
            float pulse = Mathf.Lerp(curGradient, ogGradient, t);
            float s = Mathf.Lerp(curSpeed, galaxySpeed, t);
            float a = Mathf.Lerp(galaxyAlpha, 1f, t);
            galaxyShaderMat.SetFloat("_gradientNoise", pulse);
            galaxyShaderMat.SetFloat("_galaxySpeed", s);
            galaxyShaderMat.SetFloat("_alpha", a);
            yield return null;
        }
        gradientPulsing = false;
    }


    IEnumerator PulseXYOffsetOnBeat()
    {
        offsetPulsing = true;
        float startTime = Time.time;
        float curPulseX = offSetpulseX * offsetMultiplier + audioAnalyzer.volume;
        float curPulseY = offSetpulseY * offsetMultiplier + audioAnalyzer.volume;
        while (Time.time - startTime < offsetReturnSpeed)
        {
            float t = (Time.time - startTime) / offsetReturnSpeed;
            float pulseX = Mathf.Lerp(offSetpulseX, curPulseX, t);
            float pulseY = Mathf.Lerp(offSetpulseY, curPulseY, t);
            Vector2 vec = new Vector2(pulseX, pulseY);
            galaxyShaderMat.SetVector("_OffsetXY", vec);
            yield return null;
        }
        // startTime = Time.time;
        // Vector2 curOffset = galaxyShaderMat.GetVector("_OffsetXY");
        // while (Time.time - startTime < returnToNormalSpeed)
        // {
        //     float t = (Time.time - startTime) / returnToNormalSpeed;
        //     float offsetX = Mathf.Lerp(curOffset.x, ogOffset.x, t);
        //     float offsetY = Mathf.Lerp(curOffset.y, ogOffset.y, t);
        //     galaxyShaderMat.SetVector("_OffsetXY", new Vector2(offsetX, offsetY));
        //     yield return null;
        // }
        offsetPulsing = false;
    }

}

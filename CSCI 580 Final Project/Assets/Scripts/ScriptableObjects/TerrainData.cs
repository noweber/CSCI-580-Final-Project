using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    [Header("Terrain")]
    public float uniformScale = 5f;
    public float waterHeight = 1f;
    public float waterScale = 24f;

    public bool useFlatShading;
    public bool useFalloff;
    public bool useCircleFalloff;
    public float circleFalloffRadius = 10;
    public float circleFalloffGradient = 1;
    public float meshHeightMultiplier;
    public AnimationCurve meshAnimationCurve;

    //[Header("Objects")]
    //public List<TerrainObject> objectList;

    public float minHeight
    {
        get
        {
            return uniformScale * meshHeightMultiplier * meshAnimationCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return uniformScale * meshHeightMultiplier * meshAnimationCurve.Evaluate(1);
        }
    }
}

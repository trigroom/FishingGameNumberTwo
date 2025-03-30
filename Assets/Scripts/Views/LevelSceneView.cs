using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelSceneView : MonoBehaviour
{
  //  public Light2D[] lightsInHouses;
    public Transform[] playerSpawns;
    public Transform[] enemySpawns;
    public Transform[] exitSpawns;
    public Transform[] interesPointsSpawn;
    public Transform lightsContainer;
    public Transform exitTransform;
    public InterestObjectOnLocationView[] interestsObjectsViews;
}

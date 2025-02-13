using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GrenadeView : MonoBehaviour
{
    public Light2D flashLight;
    public ParticleSystem blackSmokeParticle;
    public Rigidbody2D rigidbodyGrenade;
    public SpriteRenderer grenadeSprite;
    public Collider2D grenadeCollider;


    public Light2D SetVisualParametrs(float radius)
    {
        if (flashLight.pointLightOuterRadius != radius)
        {
            flashLight.pointLightOuterRadius = radius;
            var mainParticlesModule = blackSmokeParticle.main;
            var mainParticlesSpeed = mainParticlesModule.startSpeed;
            var mainParticlesSize = mainParticlesModule.startSize;
            mainParticlesSize.constantMin = radius * 0.1f;
            mainParticlesSize.constantMax = radius * 0.2f;
            mainParticlesSpeed.constantMin = radius * 0.2f;
            mainParticlesSpeed.constantMax = radius * 0.6f;
            mainParticlesModule.startSpeed = mainParticlesSpeed;
            mainParticlesModule.startSize = mainParticlesSize;
        }
        flashLight.intensity = radius * 2f;
        blackSmokeParticle.Play();
        flashLight.gameObject.SetActive(true);
        return flashLight;
    }
}

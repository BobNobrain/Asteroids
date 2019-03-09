using UnityEngine;
using Aster.Utils.Bars;

namespace Aster.Player {

public class PlayerStats: MonoBehaviour
{
    public SimpleBar Health;
    public SimpleBar Oxygen;
    public CooldownBar Stamina;

    [SerializeField]
    private float StaminaRegenerationSpeed = .5f;

    [SerializeField]
    private float BreathSpeed = .001f;
    [SerializeField]
    private float AsphyxiaDamage = .01f;

    [SerializeField]
    private float MinimumImpulseToHurt = 450f;

    void Update()
    {
        float dt = Time.deltaTime;

        // Breathe
        if (!Oxygen.Acquire(dt * BreathSpeed))
        {
            var dead = !Health.Acquire(dt * AsphyxiaDamage);
            if (dead)
            {
                Debug.Log("YOU DIED");
                Application.Quit();
            }
        }

        // Regenerate stamina
        Stamina.Fill(dt * StaminaRegenerationSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        var imp = collision.impulse / Time.fixedDeltaTime;
        var m = imp.magnitude;
        if (m > MinimumImpulseToHurt)
        {
            Health.Acquire((m - MinimumImpulseToHurt) / 3000f);
        }
    }
}

}

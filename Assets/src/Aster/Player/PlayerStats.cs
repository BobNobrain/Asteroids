using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Player {

public class PlayerStats: MonoBehaviour
{
    public Bar Health;
    public Bar Oxygen;
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

    [System.Serializable]
    public class Bar
    {
        [SerializeField]
        [Range(0f, 1f)]
        protected float v;

        public float Value
        {
            get { return v; }
        }

        public virtual bool Acquire(float amount)
        {
            if (amount > v)
            {
                return false;
            }
            v -= amount;
            return true;
        }
        public virtual void Fill(float amount)
        {
            v += amount;
            if (v > 1f)
            {
                v = 1f;
            }
        }

        public bool IsFull
        {
            get { return Mathf.Approximately(v, 1f); }
        }
        public bool IsEmpty
        {
            get { return Mathf.Approximately(v, 0f); }
        }
    }

    [System.Serializable]
    public class CooldownBar: Bar
    {
        private bool cooldown;

        public override bool Acquire(float amount)
        {
            if (cooldown) return false;
            if (amount >= v)
            {
                v = 0f;
                cooldown = true;
                return true;
            };
            v -= amount;
            return true;
        }
        public override void Fill(float amount)
        {
            v += amount;
            if (v > 1f)
            {
                v = 1f;
                cooldown = false;
            }
        }

        public bool IsCooldown
        {
            get { return cooldown; }
        }
    }
}

}

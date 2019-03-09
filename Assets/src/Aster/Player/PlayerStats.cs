using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Player {

public class PlayerStats: MonoBehaviour
{
    public Bar Stamina;

    [SerializeField]
    private float StaminaRegenerationSpeed = .5f;
    void Update()
    {
        Stamina.Regenerate(Time.deltaTime * StaminaRegenerationSpeed);
    }

    [System.Serializable]
    public class Bar
    {
        [SerializeField]
        [Range(0f, 1f)]
        private float v;

        private bool cooldown;

        public float Value
        {
            get { return v; }
        }

        public bool Acquire(float amount)
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
        public void Regenerate(float amount)
        {
            v += amount;
            if (v > 1f)
            {
                v = 1f;
                cooldown = false;
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
        public bool IsCooldown
        {
            get { return cooldown; }
        }
    }
}

}

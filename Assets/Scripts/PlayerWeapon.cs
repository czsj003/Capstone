using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour
{
    [SerializeField] protected Sprite weapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public abstract void PrimaryAttack();
    public abstract void SecondaryAttack();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class criticalDamage : MonoBehaviour
{
    public void kill()
    {
        gameObject.transform.parent.gameObject.GetComponent<enemySCR>().headShot();
    }
}

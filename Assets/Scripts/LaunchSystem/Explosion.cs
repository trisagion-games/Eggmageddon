﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Explosion : MonoBehaviour
{

    public float sizeLimit;
    public float rateOfSizeOfIncrease;
    public float detonationDelay;
    public GameObject explosionObject;
    public Animation explosionAnimation;
    private bool increasing;
    private bool applyingAffect;
    

    public float getX()
    {
        return transform.position.x;
    }

    public float getY()
    {
        return transform.position.y;
    }
    
    public Vector2 getLocation()
    {
        return new Vector2( transform.position.x, transform.position.y); 
    }

    void Awake ()
    {
        increasing = true;
        applyingAffect = false;
    }

    public void FixedUpdate()
    {
        Debug.Log(increasing);
        if (increasing)
        {
            increaseSize();
        }
        if(this.transform.localScale.x >= sizeLimit)
        {
            increasing = false;
            StartCoroutine(startMassacre());            
        }
    }

    IEnumerator startMassacre()
    {
        yield return new WaitForSeconds(detonationDelay);
        if (!applyingAffect)
            AudioHelper.PlaySound("explosion", 0.5f);
        applyingAffect = true;
        explosionObject.SetActive(true);
        foreach(GameObject eggu in GameObject.FindGameObjectsWithTag("Egg")) {
            ApplyEffect(eggu);
        }
        yield return new WaitForSeconds(explosionObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(default).length);
        LaunchSystem.currentExplosions.Remove(gameObject);
        Destroy(gameObject);
    }

    void increaseSize() {this.transform.localScale += new Vector3(rateOfSizeOfIncrease, rateOfSizeOfIncrease, 0);}

    public abstract void ApplyEffect(GameObject unit);

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    //Fields
    //Public
    public ParticleSystem explosionSystem;
    public Rigidbody2D rigid;
    public FireworkManager manager;
    public Vector2 launchPoint;

    //Private
    private bool isFlying = false;
    private float resetTimer;
    private bool isTarget = false;


    //Properties
    public bool IsFlying
    {
        get { return isFlying; }
    }


    // Use this for initialization
    void Start()
    {
        ResetFirework();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFlying)
        {
            if (rigid.velocity.magnitude > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan(rigid.velocity.y / rigid.velocity.x) - 90);
            }
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0)
            {
                manager.CheckForNoExplosion();
                ResetFirework();
                
            }
        }
    }

    public void Launch(Vector2 launchVelocity, bool target)
    {
        transform.position = launchPoint;
        rigid.simulated = true;
        rigid.AddForce(launchVelocity);
        isFlying = true;
        isTarget = target;
    }

    private void OnMouseDown()
    {
        if (manager.HandleExplosion(this))
        {
            ResetFirework();
        }
    }

    private void ResetFirework()
    {
        isFlying = false;
        rigid.simulated = false;
        resetTimer = manager.timeBetweenLaunches - 4.0f;
        manager.CheckIfFireworksFlying();
        rigid.velocity = Vector2.zero;
        transform.position = manager.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

}

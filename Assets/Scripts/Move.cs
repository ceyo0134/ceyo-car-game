using System;
using System.IO;
using UnityEngine;
using System.Collections;


public class Move : MonoBehaviour
{
    public float speed = 0f; 
    public float maxSpeed = 50f; 
    public float sideSpeed = 0f; 
    public float rotateSpeed = 50f; 
    public int coins = 0;
    public float acceleration = 0.03f; 
    public float deceleration = 0.06f; 

    public float liftedObjectHeight = 2f; 
    public float liftedObjectDuration = 2f; 

    // --- Ceyo güç kartı desteği ---
    [HideInInspector] public bool isShielded = false;
    [HideInInspector] public bool isSlowed = false;
    private float baseMaxSpeed;
    private Coroutine nitroRoutine;
    private Coroutine shieldRoutine;

    void Awake()
    {
        baseMaxSpeed = maxSpeed;
    }

    // Antiboost / engel gibi olumsuz etkiler buradan uygulanmalı ki Kalkan onları engelleyebilsin.
    public void ApplySlowEffect(float amount)
    {
        if (isShielded) return;
        speed -= amount;
        isSlowed = true;
    }

    // "Onarım" kartı: mevcut yavaşlama durumunu anında temizler.
    public void ClearSlowEffect()
    {
        isSlowed = false;
        if (speed < 0) speed = 0;
    }

    // "Nitro" kartı: geçici hız patlaması.
    public void ActivateNitro(float boostAmount, float extraMaxSpeed, float duration)
    {
        if (nitroRoutine != null) StopCoroutine(nitroRoutine);
        nitroRoutine = StartCoroutine(NitroRoutine(boostAmount, extraMaxSpeed, duration));
    }

    private IEnumerator NitroRoutine(float boostAmount, float extraMaxSpeed, float duration)
    {
        maxSpeed = baseMaxSpeed + extraMaxSpeed;
        speed += boostAmount;
        yield return new WaitForSeconds(duration);
        maxSpeed = baseMaxSpeed;
        nitroRoutine = null;
    }

    // "Kalkan" kartı: belirli süre boyunca olumsuz etkilere karşı bağışıklık.
    public void ActivateShield(float duration)
    {
        if (shieldRoutine != null) StopCoroutine(shieldRoutine);
        shieldRoutine = StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        isShielded = true;
        yield return new WaitForSeconds(duration);
        isShielded = false;
        shieldRoutine = null;
    }

    void Update()
    {
        float moveSide = Input.GetAxis("Horizontal");
        float moveForward = Input.GetAxis("Vertical");
        float rotation = moveSide * rotateSpeed;

        if (moveForward > 0)
        {
            if (speed >= 0)
                speed += acceleration * moveForward;
            else
                speed += deceleration * moveForward;
        }
        else if (moveForward < 0)
        {
            if (speed <= 0)
                speed += acceleration * moveForward;
            else
                speed += deceleration * moveForward;
        }
        else
        {
            if (speed > 0)
            {
                speed -= deceleration;
                if (speed < 0)
                    speed = 0;
            }
            else if (speed < 0)
            {
                speed += deceleration;
                if (speed > 0)
                    speed = 0;
            }
        }

        if (Mathf.Abs(speed) > maxSpeed)
        {
            speed = Mathf.Sign(speed) * maxSpeed;
        }

        MoveCar(rotation);
    }


    void MoveCar(float rotation)
    {
        transform.Rotate(0, rotation * Time.deltaTime, 0);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    

    
}

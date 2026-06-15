using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopRunner : MonoBehaviour
{
    //public EnemyStats enemystats; /*public [Script] [Var]*/
    //public float maxhealth;
    //public float damage;
    public float speedwalk;
    public GameObject policia;
    public Rigidbody rigidbody;
    public GameObject AreaA; /*Limite de movimiento*/
    public GameObject AreaB;
    public Transform currentPoint;
    //public AudioSource audioSource;
    /*void Awake()
    {
        maxhealth = enemystats.MaxHealth;
        damage = enemystats.Damage;
        speedwalk = enemystats.SpeedWalk;
        audioSource = GetComponent<AudioSource>();
    }*/
    void Start()
    {
        policia = GameObject.FindGameObjectsWithTag("Policia")[3];
        rigidbody = policia.GetComponent<Rigidbody>();
        speedwalk = 25f;
        currentPoint.position = new Vector3(policia.transform.position.x, policia.transform.position.y, policia.transform.position.z);
    }

    void Update()
    {
        currentPoint.transform.position = new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, rigidbody.transform.position.z);
        /*Vector que resta las posiciones del enemigo y el punto en el que se encuentra para que pueda ir a buscarlo*/
        if (currentPoint.transform.position.z <= AreaB.transform.position.z + 3f)
        {
            rigidbody.velocity = new Vector3(0, 0, speedwalk);
        }
        else
        {
            rigidbody.transform.position = new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, AreaA.transform.position.z + 3f);
            Debug.Log("El policia ha llegado al final del recorrido");
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AreaA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(AreaB.transform.position, 0.5f);
        Gizmos.DrawLine(AreaA.transform.position, AreaB.transform.position);
    }
}


using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void ProjectileCollision()
    {
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermoneterValue : MonoBehaviour {
    public float maxScale = 1;
    public float minScale = 1;
    PenguinStateMachine psm;
    // Use this for initialization
    private void Awake()
    {
        psm = GameObject.Find("Penguin").GetComponent<PenguinStateMachine>();
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3( Mathf.Lerp(minScale, maxScale, psm.bodyTemp),transform.localScale.y,transform.localScale.z);
	}
}

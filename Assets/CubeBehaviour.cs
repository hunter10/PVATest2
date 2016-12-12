using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CubeBehaviour : MonoBehaviour {
	[DllImport("PVA")]
	private extern static void PVA_init();

	[DllImport("PVA")]
	private extern static void PVA_term();

	[DllImport("PVA")]
	private extern static int PVA_getCamraStatus();

	[DllImport("PVA")]
	private extern static int PVA_config(IntPtr hWnd);

	[DllImport("PVA")]
	private extern static int PVA_startDetect(IntPtr hWnd, uint msgid);

	[DllImport("PVA")]
	private extern static int PVA_endDetect(IntPtr hWnd);

	[DllImport("PVA")]
	private extern static int PVA_startBall();

	[DllImport("PVA")]
	private extern static int PVA_endBall();

    private Text m_LogMsg;


    private void Awake()
    {
        m_LogMsg = GameObject.Find("LogMsg").GetComponent<Text>();
        m_LogMsg.text = "";
    }

    public void AddLogMsg(string msg)
    {
        m_LogMsg.text += msg + "\n";
    }

    // Use this for initialization
    void Start () {
	}

	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.C))
        {
            AddLogMsg(" 'c' OR 'C' pressed");

            //if (m_bPVA == 0)
            //{
            //    PVA_init();
            //    m_bPVA = 1;
            //}
            //int status = PVA_getCamraStatus();
            //AddLogMsg("getCamraStatus = " + status);

            ////Debug.Log("Detected character: " + e.character);
            //PVA_config(IntPtr.Zero);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            AddLogMsg("space pressed");
        }
    }

    /*
	void OnGUI() {
		Event e = Event.current;
		if( e.isKey ){
			if( e.character == 'c' || e.character == 'C' ){
				if( m_bPVA == 0 )
				{
					PVA_init();
					m_bPVA = 1;
				}
				int status = PVA_getCamraStatus();
				Debug.Log("getCamraStatus = " + status);

				//Debug.Log("Detected character: " + e.character);
				PVA_config( IntPtr.Zero );
			}
			else if( e.character == ' ' ){
				Debug.Log("space pressed");
			}
		}
	}
    */

	void OnDestroy(){
		//Debug.Log(this.gameObject.name+"OnDestroy");
		if( m_bPVA != 0 )
		{
			PVA_term();
			m_bPVA = 0;
		}
	}
	private int m_bPVA = 0;
	//private uint uDetectNotify = 0;
}

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
    public PVA m_pva;
    public SerialController m_SerialContoller;


    private void Awake()
    {
        m_LogMsg = GameObject.Find("LogMsg").GetComponent<Text>();
        m_LogMsg.text = "";
    }

    public void AddLogMsg(string msg)
    {
        m_LogMsg.text += msg + "\n";
    }

	void OnDestroy(){
		//Debug.Log(this.gameObject.name+"OnDestroy");
		if( m_bPVA != 0 )
		{
			PVA_term();
			m_bPVA = 0;
		}
	}
	private int m_bPVA = 0;

    public void Click_BallInit()
    {
        AddLogMsg("Ball Init...");
        m_pva.BallInit();
    }

    public void Click_StartBall()
    {
        AddLogMsg("Start Ball!!");
        m_pva.StartBall();
    }

    // 이하 다 야구공 던짐

    // LED Send
    public void Click_SendSerialData_N()
    {
        AddLogMsg("Send N");
        m_SerialContoller.SendSerialMessage("N"); 
    }

    public void Click_SendSerialData_M()
    {
        AddLogMsg("Send M");
        m_SerialContoller.SendSerialMessage("M"); 
    }

    public void Click_SendSerialData_Y()
    {
        AddLogMsg("Send Y");
        m_SerialContoller.SendSerialMessage("Y");
    }
}

#define Enable_DetectNotify

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class PVA : MonoBehaviour
{
struct PVAVector3D {
	public float x, y, z;
};

struct PVAVector2D {
	public float x, y;
};

struct PVADetectData {
	public uint time;
	public PVAVector3D pos;
	public PVAVector2D pos2D_0;
	public PVAVector2D pos2D_1;
};

struct PVADetectPath {
	public int num;
	public IntPtr data;
	//public ref PVADetectData data;
};

struct DetectNotify {
	public int flags;
	public int lost;
	public PVADetectPath pitch_path;
	public PVADetectPath hit_path;
	/* 16/12/03 added */
	public float speed;
	public float azimuth;
	public float altitude;
}

	[DllImport("PVA")]
	private static extern void PVA_init();

	[DllImport("PVA")]
	private static extern void PVA_term();

	[DllImport("PVA")]
	private static extern int PVA_getCamraStatus();

	[DllImport("PVA")]
	private static extern int PVA_config(IntPtr hWnd);

	[DllImport("PVA")]
	private static extern int PVA_startDetect(IntPtr hWnd, uint msgid);

	[DllImport("PVA")]
	private static extern int PVA_endDetect(IntPtr hWnd);

	[DllImport("PVA")]
	private static extern int PVA_getDetect(ref DetectNotify detect);

	[DllImport("PVA")]
	private static extern int PVA_startBall();

	[DllImport("PVA")]
	private static extern int PVA_endBall();

    public CubeBehaviour m_Parent;

	// Use this for initialization
	void Start ()
    {
        if ( m_bPVA == 0 )
		{
            PVA_init();
            m_bPVA = 1;
        }

        m_Parent.AddLogMsg("PVA Start() m_bPVA " + m_bPVA);
    }
	void OnDestroy(){
		if( m_bPVA != 0 )
		{
			PVA_term();
			m_bPVA = 0;
		}
	}

	// Update is called once per frame
	void Update ()
	{
        
        if ( m_bPVA != 0 )
		{
			int status = PVA_getCamraStatus();
            if ( m_status != status ){
				m_status = status;

				string text;
				switch( m_status ){
				case PVAResult_OK:
					text = "OK"; break;
				case PVAResult_Uninitialize:
					text = "Uninitialize"; break;
				case PVAResult_NotConnect:
					text = "NotConnect"; break;
				case PVAResult_AlreadyRegist:
					text = "AlreadyRegist"; break;
				case PVAResult_RegistNotFound:
					text = "RegistNotFound"; break;
				case PVAResult_DongleNotFound:
					text = "DongleNotFound"; break;
				case PVAResult_NoFPS:
					text = "NoFPS"; break;
				case PVAResult_NoData:
					text = "NoData"; break;
				default:
					text = "Unknown"; break;
				}
				Debug.Log("getCamraStatus = " + text);
			}

			if( m_ball_start != 0 ){
				int ret = PVA_getDetect( ref m_detect );
				if( ret == PVAResult_OK ){
					Debug.Log("Detected!");

#if Enable_DetectNotify
					GameObject bullet = LoadBullet();
					if( bullet != null ){
						IntPtr pData;
						if( (m_detect.flags & PVAFlg_Hit) != 0 ){
							pData = m_detect.hit_path.data;
						}
						else{
							pData = m_detect.pitch_path.data;
						}
						PVADetectData first = (PVADetectData)Marshal.PtrToStructure(pData, typeof(PVADetectData));
						pData = new IntPtr(pData.ToInt32() + Marshal.SizeOf(typeof(PVADetectData)));
						PVADetectData last = (PVADetectData)Marshal.PtrToStructure(pData, typeof(PVADetectData));

						PVAVector3D pos0 = first.pos;
						PVAVector3D pos1 = last.pos;

						uint send_time = last.time - first.time;
						float t = (float)1000 / (float)send_time;

						Vector3 dir = new Vector3();
						dir.x = pos1.x - pos0.x;
						dir.y = pos1.y - pos0.y;
						dir.z = -(pos1.z - pos0.z);
						float hit_speed = dir.magnitude * t;

						dir.Normalize();

						//bullet.transform.parent = transform;
						bullet.transform.localPosition = new Vector3(pos0.x, pos0.y, -pos0.z);

						Rigidbody rb = bullet.GetComponent<Rigidbody>();
						rb.velocity = dir * hit_speed;
						//rb.velocity = dir * power;
					}
#endif
					PVA_endBall();
					m_ball_start = 0;
				}
			}
		}

		if( Input.GetMouseButtonDown(0) ){
#if Enable_DetectNotify
			if( m_status == PVAResult_OK ){
				Debug.Log("startBall");

				PVA_startBall();
				m_ball_start = 1;
			}
#else
			GameObject bullet = LoadBullet();
			if( bullet != null ){
				Debug.Log("startBall");

				bullet.transform.parent = transform;
				bullet.transform.localPosition = Vector3.zero;

				Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
				Vector3 dir = ray.direction.normalized;
				Rigidbody rb = bullet.GetComponent<Rigidbody>();
				rb.velocity = dir * power;

				PVA_startBall();
				m_ball_start = 1;
			}
#endif
		}
	}

    public void BallInit()
    {
        PVA_config(IntPtr.Zero);
    }

    public void StartBall()
    {
        if (m_status == PVAResult_OK)
        {
            PVA_startBall();
            m_ball_start = 1;
        }
    }

	private GameObject LoadBullet()
	{
		GameObject bullet = Instantiate(prefab);
		return bullet;
	}

	public GameObject prefab;
	public float power;

	private const int PVAResult_OK             = 0;
	private const int PVAResult_Uninitialize   = -1;
	private const int PVAResult_NotConnect     = -2;
	private const int PVAResult_AlreadyRegist  = -3;
	private const int PVAResult_RegistNotFound = -4;
	private const int PVAResult_DongleNotFound = -5;
	private const int PVAResult_NoFPS          = -6;
	private const int PVAResult_NoData         = -7;
	private const int PVAResult_Unknown        = -100;
	private const int PVAFlg_Pitch  = 0x01;
	private const int PVAFlg_Hit    = 0x02;

	private DetectNotify m_detect = new DetectNotify();

	private int m_bPVA = 0;
	private int m_status = PVAResult_Uninitialize;
	private int m_ball_start = 0;
}

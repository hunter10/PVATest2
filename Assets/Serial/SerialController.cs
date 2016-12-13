using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class SerialController : MonoBehaviour {

    [Tooltip("Port name with which the SerialPort object will be created.")]
    public string portName = "COM1";

    [Tooltip("Baud rate that the serial device is using to transmit data.")]
    public int baudRate = 9600;

    [Tooltip("Reference to an scene object that will receive the events of connection, " +
             "disconnection and the messages from the serial device.")]
    public GameObject messageListener;

    [Tooltip("After an error in the serial communication, or an unsuccessful " +
             "connect, how many milliseconds we should wait.")]
    public int reconnectionDelay = 1000;

    [Tooltip("Maximum number of unread data messages in the queue. " +
             "New messages will be discarded.")]
    public int maxUnreadMessages = 1;

    public const string SERIAL_DEVICE_CONNECTED = "__Connected__";
    public const string SERIAL_DEVICE_DISCONNECTED = "__Disconnected__";

    // Internal reference to the Thread and the object that runs in it.
    private Thread thread;
    private SerialThread serialThread;

    public CubeBehaviour mParent;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {
        mParent.AddLogMsg("Serial Start.");
    }

    

    void OnEnable()
    {
        serialThread = new SerialThread(portName, baudRate, reconnectionDelay,
                                        maxUnreadMessages);
        thread = new Thread(new ThreadStart(serialThread.RunForever));
        thread.Start();
    }


    void OnDisable()
    {
        // If there is a user-defined tear-down function, execute it before
        // closing the underlying COM port.
        if (userDefinedTearDownFunction != null)
            userDefinedTearDownFunction();

        // The serialThread reference should never be null at this point,
        // unless an Exception happened in the OnEnable(), in which case I've
        // no idea what face Unity will make.
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }

        // This reference shouldn't be null at this point anyway.
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }

    // Update is called once per frame
    void Update () {
        // If the user prefers to poll the messages instead of receiving them
        // via SendMessage, then the message listener should be null.
        if (messageListener == null)
            return;

        // Read the next message from the queue
        string message = serialThread.ReadSerialMessage();
        if (message == null)
            return;

        // Check if the message is plain data or a connect/disconnect event.
        if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
        {
            mParent.AddLogMsg("OnConnectionEvent... true");
            messageListener.SendMessage("OnConnectionEvent", true);
        }
        else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
        {
            mParent.AddLogMsg("OnConnectionEvent... false");
            messageListener.SendMessage("OnConnectionEvent", false);
        }
        else
        {
            // 시리얼 통신 시 이쪽으로 메세지 날아오는지 체크

            mParent.AddLogMsg("Message arrived: " + message);
            messageListener.SendMessage("OnMessageArrived", message);
        }
    }

    public string ReadSerialMessage()
    {
        // Read the next message from the queue
        return serialThread.ReadSerialMessage();
    }

    public void SendSerialMessage(string message)
    {
        serialThread.SendSerialMessage(message);
    }

    public delegate void TearDownFunction();
    private TearDownFunction userDefinedTearDownFunction;
    public void SetTearDownFunction(TearDownFunction userFunction)
    {
        this.userDefinedTearDownFunction = userFunction;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class GameManager : MonoBehaviour
{
    public static GameManager ins = null;

    private void Awake()
    {
        ins = this;

        DebugSetting();
        AwakeSetting();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] bool isDebug = true;

    [SerializeField] bool isCursorVisible = false;
    private void AwakeSetting()
    {
        Screen.SetResolution(1080, 1920, true);
    }

    private void StartSetting()
    {
        Cursor.visible = isCursorVisible;
    }

    private void DebugSetting()
    {
        Debug.unityLogger.logEnabled = isDebug;
    }

    public void Restart()
    {
        Debug.Log("NOTICE : Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

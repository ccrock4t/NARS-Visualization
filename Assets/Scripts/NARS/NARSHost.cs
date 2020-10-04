using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using TMPro;

public class NARSHost : MonoBehaviour
{
    public enum NARSType : int
    {
        NARS, ONA
    }

    public NARSType type;
    Visualizer _visualizer;
    Process process = null;
    StreamWriter messageStream;

    TMP_InputField inputTextField;

    private void Start()
    {
        Application.targetFrameRate = 60;
        switch (type)
        {
            case NARSType.NARS:
                LaunchNARS();
                break;
            case NARSType.ONA:
                LaunchONA();
                break;
            default:
                break;
        }

        _visualizer = GetComponent<Visualizer>();
        inputTextField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
    }



    private Visualizer GetVisualizer()
    {
        return _visualizer;
    }

    private void Update()
    {
    }

    public void LaunchONA()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo(@"cmd.exe");
        startInfo.WorkingDirectory = Application.dataPath + @"\NARS";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += new DataReceivedEventHandler(ONAOutputReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.StandardInput.WriteLine("NAR shell");
        process.StandardInput.Flush();

        messageStream = process.StandardInput;
    }

    public void LaunchNARS()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
        startInfo.WorkingDirectory = Application.dataPath + @"\NARS";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += new DataReceivedEventHandler(NARSOutputReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.StandardInput.WriteLine("java -Xmx1024m -jar opennars.jar");
        process.StandardInput.Flush();

        messageStream = process.StandardInput;
        AddInput("*volume=100");
    }

    public void AddInferenceCycles(int cycles)
    {
        AddInput("" + cycles);
    }

    public void AddInputText()
    {
        AddInput(inputTextField.text);
        inputTextField.text = "";
    }

    public void AddInput(string message)
    {
        //Debug.Log("SENDING INPUT: " + message);

        messageStream.WriteLine(message);
    }
    void NARSOutputReceived(object sender, DataReceivedEventArgs eventArgs)
    {

        string outputStr = eventArgs.Data;
        //Debug.Log(outputStr);
        if (outputStr.Contains("EXE:")) //operation executed
        {
            int length = outputStr.IndexOf("(") - eventArgs.Data.IndexOf("^");
            string operation = outputStr.Substring(eventArgs.Data.IndexOf("^"), length);

            if (operation == "^left")
            {
            }
            else if (operation == "^right")
            {

            }
            else if (operation == "^deactivate")
            {

            }
        }

        if (outputStr.Contains("OUT:"))
        {
            GetVisualizer().QueueStatement(outputStr);
        }

    }

    void ONAOutputReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.Log(eventArgs.Data);
        if (eventArgs.Data.Contains("executed with args")) //operation executed
        {
            string operation = eventArgs.Data.Split(' ')[0];
          
            if (operation == "^left")
            {
                // UnityEngine.Debug.Log("RECEIVED OUTPUT: " + operation);



            }
            else if(operation == "^right")
            {
                // UnityEngine.Debug.Log("RECEIVED OUTPUT: " + operation);

            }
            else if (operation == "^deactivate")
            {
                // UnityEngine.Debug.Log("RECEIVED OUTPUT: " + operation);
            }
        }
        
    }

    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }

    void OnApplicationQuit()
    {
        if (process != null || !process.HasExited )
        {
            process.CloseMainWindow();
        }
    }

}




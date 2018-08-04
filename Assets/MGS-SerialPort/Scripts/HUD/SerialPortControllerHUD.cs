﻿/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SerialPortControllerHUD.cs
 *  Description  :  Draw UI in scene to control serialport.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/5/2017
 *  Description  :  Initial development version.
 *  
 *  Author       :  Mogoson
 *  Version      :  0.1.1
 *  Date         :  3/2/2018
 *  Description  :  Optimize.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Mogoson.IO.Ports
{
    [AddComponentMenu("Mogoson/IO/Ports/SerialPortControllerHUD")]
    public class SerialPortControllerHUD : MonoBehaviour
    {
        #region Field and Property
        public float xOffset = 10;
        public float yOffset = 10;

        private string readText = string.Empty;
        private string writeText = string.Empty;

        private const string Space = "\x0020";
        private readonly string[] separater = { Space };

        private SerialPortController Controller { get { return SerialPortController.Instance; } }
        #endregion

        #region Private Method
        private void Update()
        {
            if (Controller.IsReading)
            {
                if (Controller.IsReadTimeout)
                {
                    readText = "No serialport device input.";
                }
                else
                {
                    var readString = string.Empty;
                    foreach (var @byte in Controller.ReadBytes)
                    {
                        readString += @byte.ToString("X2") + Space;
                    }
                    readText = readString;
                }
            }
            if (Controller.IsWriting)
            {
                var writeBuffer = new List<byte>();
                var bytesString = writeText.Split(separater, StringSplitOptions.RemoveEmptyEntries);
                foreach (var @byte in bytesString)
                {
                    try
                    {
                        writeBuffer.Add(byte.Parse(@byte.Trim(), NumberStyles.HexNumber));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
                Controller.WriteBytes = writeBuffer.ToArray();
            }
        }

        private void OnGUI()
        {
            var rect = new Rect(xOffset, yOffset, 180, 220);
            GUILayout.BeginArea(rect, "Controller", "Window");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ReInitialize"))
            {
                var error = string.Empty;
                Controller.ReInitializeSerialPort(out error);
            }
            if (Controller.IsOpen)
            {
                if (GUILayout.Button("Close"))
                {
                    var error = string.Empty;
                    Controller.CloseSerialPort(out error);
                }
            }
            else
            {
                if (GUILayout.Button("Open"))
                {
                    var error = string.Empty;
                    Controller.OpenSerialPort(out error);
                }
            }
            GUILayout.EndHorizontal();

            writeText = GUILayout.TextArea(writeText, GUILayout.Height(50));

            GUILayout.BeginHorizontal();
            if (Controller.IsWriting)
            {
                if (GUILayout.Button("StopWrite"))
                {
                    var error = string.Empty;
                    Controller.StopWrite(out error);
                }
            }
            else
            {
                if (GUILayout.Button("StartWrite"))
                {
                    var error = string.Empty;
                    Controller.StartWrite(out error);
                }
            }
            if (GUILayout.Button("Clear"))
                writeText = string.Empty;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (Controller.IsReading)
            {
                if (GUILayout.Button("StopRead"))
                {
                    var error = string.Empty;
                    Controller.StopRead(out error);
                }
            }
            else
            {
                if (GUILayout.Button("StartRead"))
                {
                    var error = string.Empty;
                    Controller.StartRead(out error);
                }
            }
            if (GUILayout.Button("Clear"))
                readText = string.Empty;
            GUILayout.EndHorizontal();

            GUILayout.TextArea(readText, GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
        }

        private void OnApplicationQuit()
        {
            if (Controller.IsOpen)
            {
                var error = string.Empty;
                Controller.CloseSerialPort(out error);
            }
        }
        #endregion
    }
}
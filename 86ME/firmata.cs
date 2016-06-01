/*
 * Arduino.cs - Arduino/firmata library for Visual C# .NET
 * Copyright (C) 2009 Tim Farley
 * 
 * Special thanks to David A. Mellis, on whose Processing library
 * this code is based.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General
 * Public License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place, Suite 330,
 * Boston, MA  02111-1307  USA
 *
 *
 * ***************** *
 * TODO/KNOWN ISSUES *
 * ***************** *
 * Exception Handling: At this time there is no exception handling.
 * It should be trivial to add exception handling as-needed.
 * 
 * $Id$
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace _86ME_ver1
{
    /*
     * Together with the Firmata 2 firmware (an Arduino sketch uploaded to the
     * Arduino board), this class allows you to control the Arduino board from
     * Processing: reading from and writing to the digital pins and reading the
     * analog inputs.
     */
    public class Arduino
    {
        public bool dataRecieved = false;

        public static int INPUT = 0;
        public static int OUTPUT = 1;
        public static int LOW = 0;
        public static int HIGH = 1;

        private const int MAX_DATA_BYTES = 128;

        private const int DIGITAL_MESSAGE = 0x90; // send data for a digital port
        private const int ANALOG_MESSAGE = 0xE0; // send data for an analog pin (or PWM)
        private const int REPORT_ANALOG = 0xC0; // enable analog input by pin #
        private const int REPORT_DIGITAL = 0xD0; // enable digital input by port
        private const int SET_PIN_MODE = 0xF4; // set a pin to INPUT/OUTPUT/PWM/etc
        private const int REPORT_VERSION = 0xF9; // report firmware version
        private const int SYSTEM_RESET = 0xFF; // reset from MIDI
        private const int START_SYSEX = 0xF0; // start a MIDI SysEx message
        private const int END_SYSEX = 0xF7; // end a MIDI SysEx message

        private SerialPort _serialPort;
        private int delay;

        private int waitForData = 0;
        private int executeMultiByteCommand = 0;
        private int multiByteChannel = 0;
        private int[] storedInputData = new int[MAX_DATA_BYTES];
        public long captured_data;
        public float captured_float;
        public float[] quaternion = new float[4];
        private bool parsingSysex;
        private int sysexBytesRead;

        private volatile int[] digitalOutputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private volatile int[] digitalInputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private volatile int[] analogInputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private int majorVersion = 0;
        private int minorVersion = 0;
        private Thread readThread = null;
        private object locker = new object();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        /// <param name="baudRate">The baud rate of the communication. Default 115200</param>
        /// <param name="autoStart">Determines whether the serial port should be opened automatically.
        ///                     use the Open() method to open the connection manually.</param>
        /// <param name="delay">Time delay that may be required to allow some arduino models
        ///                     to reboot after opening a serial connection. The delay will only activate
        ///                     when autoStart is true.</param>
        public Arduino(string serialPortName, Int32 baudRate, bool autoStart, int delay)
        {
            quaternion[0] = 1;
            quaternion[1] = 0;
            quaternion[2] = 0;
            quaternion[3] = 0;

            _serialPort = new SerialPort(serialPortName, baudRate);
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.RequestToSend;
            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = true;

            if (autoStart)
            {
                this.delay = delay;
                this.Open();
            }
        }

        /// <summary>
        /// Creates an instance of the Arduino object, based on a user-specified serial port.
        /// Assumes default values for baud rate (115200) and reboot delay (0.1 seconds)
        /// and automatically opens the specified serial connection.
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        public Arduino(string serialPortName) : this(serialPortName, 115200, true, 100) { }

        /// <summary>
        /// Creates an instance of the Arduino object, based on user-specified serial port and baud rate.
        /// Assumes default value for reboot delay (0.1 seconds).
        /// and automatically opens the specified serial connection.
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4</param>
        /// <param name="baudRate">Baud rate.</param>
        public Arduino(string serialPortName, Int32 baudRate) : this(serialPortName, baudRate, true, 100) { }

        /// <summary>
        /// Creates an instance of the Arduino object using default arguments.
        /// Assumes the arduino is connected as the HIGHEST serial port on the machine,
        /// default baud rate (115200), and a reboot delay (0.1 seconds).
        /// and automatically opens the specified serial connection.
        /// </summary>
        public Arduino() : this(Arduino.list().ElementAt(list().Length - 1), 115200, true, 100) { }

        /// <summary>
        /// Opens the serial port connection, should it be required. By default the port is
        /// opened when the object is first created.
        /// </summary>
        public void Open()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }

            Thread.Sleep(delay);

            byte[] command = new byte[2];

            for (int i = 0; i < 6; i++)
            {
                command[0] = (byte)(REPORT_ANALOG | i);
                command[1] = (byte)1;
                _serialPort.Write(command, 0, 2);
            }

            for (int i = 0; i < 2; i++)
            {
                command[0] = (byte)(REPORT_DIGITAL | i);
                command[1] = (byte)1;
                _serialPort.Write(command, 0, 2);
            }
            command = null;

            if (readThread == null)
            {
                readThread = new Thread(processInput);
                readThread.Start();
            }
        }

        /// <summary>
        /// Closes the serial port.
        /// </summary>
        public void Close()
        {
            if (readThread != null)
            {
                readThread.Join(500);
                readThread = null;
            }
            try
            {
                _serialPort.Close();
            }
            catch
            {
            }
        }

        public bool is_Open()
        {
            return _serialPort.IsOpen;
        }

        /// <summary>
        /// Lists all available serial ports on current system.
        /// </summary>
        /// <returns>An array of strings containing all available serial ports.</returns>
        public static string[] list()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// Returns the last known state of the digital pin.
        /// </summary>
        /// <param name="pin">The arduino digital input pin.</param>
        /// <returns>Arduino.HIGH or Arduino.LOW</returns>
        public int digitalRead(int pin)
        {
            return (digitalInputData[pin >> 3] >> (pin & 0x07)) & 0x01;
        }

        /// <summary>
        /// Returns the last known state of the analog pin.
        /// </summary>
        /// <param name="pin">The arduino analog input pin.</param>
        /// <returns>A value representing the analog value between 0 (0V) and 1023 (5V).</returns>
        public int analogRead(int pin)
        {
            return analogInputData[pin];
        }

        /// <summary>
        /// Sets the mode of the specified pin (INPUT or OUTPUT).
        /// </summary>
        /// <param name="pin">The arduino pin.</param>
        /// <param name="mode">Mode Arduino.INPUT or Arduino.OUTPUT.</param>
        public void pinMode(int pin, int mode)
        {
            byte[] message = new byte[3];
            message[0] = (byte)(SET_PIN_MODE);
            message[1] = (byte)(pin);
            message[2] = (byte)(mode);
            _serialPort.Write(message, 0, 3);
            message = null;
        }

        /// <summary>
        /// Write to a digital pin that has been toggled to output mode with pinMode() method.
        /// </summary>
        /// <param name="pin">The digital pin to write to.</param>
        /// <param name="value">Value either Arduino.LOW or Arduino.HIGH.</param>
        public void digitalWrite(int pin, int value)
        {
            int portNumber = (pin >> 3) & 0x0F;
            byte[] message = new byte[3];

            if (value == 0)
                digitalOutputData[portNumber] &= ~(1 << (pin & 0x07));
            else
                digitalOutputData[portNumber] |= (1 << (pin & 0x07));

            message[0] = (byte)(DIGITAL_MESSAGE | portNumber);
            message[1] = (byte)(digitalOutputData[portNumber] & 0x7F);
            message[2] = (byte)(digitalOutputData[portNumber] >> 7);
            _serialPort.Write(message, 0, 3);
        }

        /// <summary>
        /// Write to an analog pin using Pulse-width modulation (PWM).
        /// </summary>
        /// <param name="pin">Analog output pin.</param>
        /// <param name="value">PWM frequency from 0 (always off) to 255 (always on).</param>
        public void analogWrite(int pin, int value)
        {
            byte[] message = new byte[3];
            message[0] = (byte)(ANALOG_MESSAGE | (pin & 0x0F));
            message[1] = (byte)(value & 0x7F);
            message[2] = (byte)(value >> 7);
            _serialPort.Write(message, 0, 3);
        }

        public void sysexgWrite(int command, int pin, int value, int delay)
        {
            byte[] message = new byte[8];
            message[0] = 0xF0;
            message[1] = (byte)(command);
            message[2] = (byte)(pin & 0x7F);
            message[3] = (byte)(value & 0x7F);
            message[4] = (byte)(value >> 7);
            message[5] = (byte)(delay & 0x7F);
            message[6] = (byte)(delay >> 7);
            message[7] = 0xF7;
            _serialPort.Write(message, 0, 8);
        }

        public void motor_release()
        {
            byte[] message = new byte[3];
            message[0] = 0xF0;
            message[1] = 0x70;
            message[2] = 0xF7;
            _serialPort.Write(message, 0, 3);
        }

        public void frameWrite(int command, int[] frame, int delay)
        {
            int msg_index = 2;
            byte[] message = new byte[95];
            message[0] = 0xF0;
            message[1] = (byte)(command);
            for (int i = 0; i < 45; i++, msg_index += 2)
            {
                message[msg_index] = (byte)(frame[i] & 0x7F);
                message[msg_index + 1] = (byte)((frame[i] >> 7) & 0x7F);
            }
            message[msg_index] = (byte)(delay & 0x7F);
            message[msg_index + 1] = (byte)((delay >> 7) & 0x7F);
            message[msg_index + 2] = 0xF7;
            _serialPort.Write(message, 0, 95);
        }

        public void setSyncSpeed(int speed)
        {
            byte[] message = new byte[7];
            message[0] = 0xF0;
            message[1] = 0x6F;
            message[2] = 0x01;
            message[3] = 0x00;
            message[4] = (byte)(speed & 0x7F);
            message[5] = (byte)(speed >> 7);
            message[6] = 0xF7;
            _serialPort.Write(message, 0, 7);
        }

        public void frame_capture(int pin)
        {
            byte[] message = new byte[4];
            message[0] = 0xF0;
            message[1] = 0x69;
            message[2] = (byte)(pin & 0x7F);
            message[3] = 0xF7;
            _serialPort.Write(message, 0, 4);
        }

        public void pin_capture(int pin)
        {
            byte[] message = new byte[4];
            message[0] = 0xF0;
            message[1] = 0x6D;
            message[2] = (byte)(pin & 0x7F);
            message[3] = 0xF7;
            _serialPort.Write(message, 0, 4);
        }

        public void init_IMU(int module)
        {
            byte[] message = new byte[4];
            message[0] = 0xF0;
            message[1] = 0x78;
            message[2] = (byte)(module & 0x7F);
            message[3] = 0xF7;
            _serialPort.Write(message, 0, 4);
        }

        public void getQ()
        {
            byte[] message = new byte[4];
            message[0] = 0xF0;
            message[1] = 0x76;
            message[2] = 0x00; // getQ()
            message[3] = 0xF7;
            _serialPort.Write(message, 0, 4);
        }

        private void setDigitalInputs(int portNumber, int portData)
        {
            digitalInputData[portNumber] = portData;
        }

        private void setAnalogInput(int pin, int value)
        {
            analogInputData[pin] = value;
        }

        private void setVersion(int majorVersion, int minorVersion)
        {
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
        }

        private int available()
        {
            return _serialPort.BytesToRead;
        }

        public void processInput()
        {
            while (_serialPort.IsOpen)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    lock (this)
                    {
                        int inputData = _serialPort.ReadByte();
                        int command;

                        if (parsingSysex)
                        {
                            if (inputData == END_SYSEX)
                            {
                                parsingSysex = false;
                                if (storedInputData[0] == 0x6A)
                                {
                                    captured_data = (int)((storedInputData[2] << 7) + storedInputData[1]);
                                    dataRecieved = true;
                                }
                                else if (storedInputData[0] == 0x6E) // PIN_STATE_RESPONSE
                                {
                                    if (storedInputData[1] == 0)
                                    {
                                        byte[] b2u = new byte[4];
                                        for (int i = 0; i < 4; i++)
                                            b2u[i] = (byte)((storedInputData[2 + i]) | ((storedInputData[6] << (4 + i)) & 0x80));
                                        captured_data = System.BitConverter.ToUInt32(b2u, 0);
                                        dataRecieved = true;
                                    }
                                    else if (storedInputData[1] == 1)
                                    {
                                        byte[] b2i = new byte[4];
                                        for (int i = 0; i < 4; i++)
                                            b2i[i] = (byte)((storedInputData[2 + i]) | ((storedInputData[6] << (4 + i)) & 0x80));
                                        captured_data = System.BitConverter.ToInt32(b2i, 0);
                                        dataRecieved = true;
                                    }
                                    else if (storedInputData[1] < 8 && storedInputData[1] >= 2)
                                    {
                                        captured_data = (int)((storedInputData[3] << 7) + storedInputData[2]);
                                        dataRecieved = true;
                                    }
                                    else
                                    {
                                        byte[] b2f = new byte[4];
                                        for (int i = 0; i < 4; i++)
                                            b2f[i] = (byte)((storedInputData[2 + i]) | ((storedInputData[6] << (4 + i)) & 0x80));
                                        captured_float = System.BitConverter.ToSingle(b2f, 0);
                                        dataRecieved = true;
                                    }
                                }
                                else if (storedInputData[0] == 0x77) // I2C_REPLY
                                {
                                    if (storedInputData[1] == 0x00) // Quaternion
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            byte[] b2f = new byte[4];
                                            for (int j = 0; j < 4; j++)
                                                b2f[j] = (byte)((storedInputData[2 + j + i*4]) | ((storedInputData[18 + i] << (4 + j)) & 0x80));
                                            quaternion[i] = System.BitConverter.ToSingle(b2f, 0);
                                        }
                                        dataRecieved = true;
                                    }
                                }
                            }
                            else
                            {
                                storedInputData[sysexBytesRead] = inputData;
                                sysexBytesRead++;
                            }
                        }
                        else if (waitForData > 0 && inputData < 128)
                        {
                            waitForData--;
                            storedInputData[waitForData] = inputData;

                            if (executeMultiByteCommand != 0 && waitForData == 0)
                            {
                                //we got everything
                                switch (executeMultiByteCommand)
                                {
                                    case DIGITAL_MESSAGE:
                                        setDigitalInputs(multiByteChannel, (storedInputData[0] << 7) + storedInputData[1]);
                                        break;
                                    case ANALOG_MESSAGE:
                                        setAnalogInput(multiByteChannel, (storedInputData[0] << 7) + storedInputData[1]);
                                        break;
                                    case REPORT_VERSION:
                                        setVersion(storedInputData[1], storedInputData[0]);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (inputData < 0xF0)
                            {
                                command = inputData & 0xF0;
                                multiByteChannel = inputData & 0x0F;
                            }
                            else
                            {
                                command = inputData;
                                // commands in the 0xF* range don't use channel data
                            }
                            switch (command)
                            {
                                case DIGITAL_MESSAGE:

                                case ANALOG_MESSAGE:
                                case REPORT_VERSION:
                                    waitForData = 2;
                                    executeMultiByteCommand = command;
                                    break;
                                case START_SYSEX:
                                    parsingSysex = true;
                                    sysexBytesRead = 0;
                                    break;
                            }
                        }
                    }
                }
            }
        }

    } // End Arduino class

} // End namespace
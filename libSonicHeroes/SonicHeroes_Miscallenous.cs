﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SonicHeroes.Misc
{
    /// <summary>
    /// This class holds all sorts of miscallenous methods, mostly external which may simply at one point prove useful. The structs and any miscallenous 
    /// </summary>
    class SonicHeroes_Miscallenous
    {

        /// <summary>
        /// Returns the Sonic Heroes executable, TSonic_Win, as an array.
        /// </summary>
        /// <returns></returns>
        public static byte[] Get_SonicHeroes_Executable_As_Array()
        {
            byte[] SonicHeroesExecutable = new byte[new FileInfo("Tsonic_win.exe").Length];
            SonicHeroesExecutable = File.ReadAllBytes("Tsonic_win.exe");
            return SonicHeroesExecutable;
        }

        /// <summary>
        /// Returns the Sonic Heroes executable, TSonic_Win, as an array.
        /// </summary>
        /// <returns></returns>
        public static byte[] Get_SonicHeroes_Executable_As_Array(string Executable_Path)
        {
            byte[] SonicHeroesExecutable = new byte[new FileInfo(Executable_Path).Length];
            SonicHeroesExecutable = File.ReadAllBytes(Executable_Path);
            return SonicHeroesExecutable;
        }

        /// <summary>
        /// Represents the structure which can be used to store the configuration file in.
        /// </summary>
        public struct Sonic_Heroes_Configuration_File
        {
            public byte FrameRate;
            public byte FreeCamera;
            public byte FogEmulation;
            public byte ClipRange;
            public byte AnisotropicFiltering;
            public byte[] ControllerOne;
            public byte[] ControllerTwo;
            public byte[] MouseControls;
            public byte Screensize;
            public byte FullScreen;
            public byte Language;
            public byte SurroundSound;
            public byte SFXVolume;
            public byte SFXToggle;
            public byte BGMVolume;
            public byte BGMToggle;
            public byte SoftShadows;
            public byte MouseControlType;
        }

        /// <summary>
        /// Loads the Sonic Heroes Configuration File, returns under the custom struct defined in the same file.
        /// </summary>
        public static Sonic_Heroes_Configuration_File Load_Configuration_File()
        {
            Sonic_Heroes_Configuration_File ConfigFile = new Sonic_Heroes_Configuration_File();
            string ConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SEGA\SONICHEROES\sonic_h.ini";
            string CurrentLine;
            System.IO.StreamReader INIConfigFile = new System.IO.StreamReader(ConfigFilePath);

            // Initialize size of struct arrays;
            ConfigFile.ControllerOne = new byte[8];
            ConfigFile.ControllerTwo = new byte[8];
            ConfigFile.MouseControls = new byte[8];

            while ((CurrentLine = INIConfigFile.ReadLine()) != null)
            {
                if (CurrentLine.StartsWith("Frame_Rate")) { ConfigFile.FrameRate = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Free_Camera")) { ConfigFile.FreeCamera = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Fog")) { ConfigFile.FogEmulation = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Clip_Range")) { ConfigFile.ClipRange = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Anisotoropic")) { ConfigFile.AnisotropicFiltering = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Pad_Assign1")) { ReadStringByteArray(CurrentLine.Substring(CurrentLine.IndexOf(" ") + 1), ConfigFile.ControllerOne); }
                else if (CurrentLine.StartsWith("Pad_Assign2")) { ReadStringByteArray(CurrentLine.Substring(CurrentLine.IndexOf(" ") + 1), ConfigFile.ControllerTwo); }
                else if (CurrentLine.StartsWith("Mouse_Assign")) { ReadStringByteArray(CurrentLine.Substring(CurrentLine.IndexOf(" ") + 1), ConfigFile.MouseControls); }
                else if (CurrentLine.StartsWith("Screen_Size_Selection")) { ConfigFile.Screensize = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Screen_Full")) { ConfigFile.FullScreen = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Language")) { ConfigFile.Language = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("3D_Sound")) { ConfigFile.SurroundSound = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("SE_Volume")) { ConfigFile.SFXVolume = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("SE_On")) { ConfigFile.SFXToggle = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("BGM_Volume")) { ConfigFile.BGMVolume = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("BGM_On")) { ConfigFile.BGMToggle = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Cheap_Shadow")) { ConfigFile.SoftShadows = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
                else if (CurrentLine.StartsWith("Mouse_Control_Type")) { ConfigFile.MouseControlType = byte.Parse(CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1)); }
            }

            INIConfigFile.Dispose();
            return ConfigFile;
        }

        /// <summary>
        /// Ignore this, I'm too lazy to edit another method.
        /// </summary>
        /// <param name="OriginalString"></param>
        /// <param name="ByteArray"></param>
        public static void ReadStringByteArray(string OriginalString, byte[] ByteArray) { string[] StringArray = OriginalString.Split(' '); for (int x = 0; x < StringArray.Length; x++) { ByteArray[x] = byte.Parse(StringArray[x]); } }

        /// <summary>
        /// Saves a Sonic Heroes configuration file based off of the Sonic_Heroes_Configuration_File struct.
        /// </summary>
        public static void Save_Configuration_File(Sonic_Heroes_Configuration_File ConfigFile)
        {
            string ConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SEGA\SONICHEROES\sonic_h.ini";
            System.IO.StreamWriter INIConfigFile = new System.IO.StreamWriter(ConfigFilePath);

            string PadControls1 = ""; foreach (byte PadButton in ConfigFile.ControllerOne) { PadControls1 = PadControls1 + " " + PadButton; }
            string PadControls2 = ""; foreach (byte PadButton in ConfigFile.ControllerTwo) { PadControls2 = PadControls2 + " " + PadButton; }
            string MouseControls = ""; foreach (byte MouseButton in ConfigFile.MouseControls) { MouseControls = MouseControls + " " + MouseButton; }
            INIConfigFile.WriteLine("Frame_Rate " + ConfigFile.FrameRate);
            INIConfigFile.WriteLine("Free_Camera " + ConfigFile.FreeCamera);
            INIConfigFile.WriteLine("Fog " + ConfigFile.FogEmulation);
            INIConfigFile.WriteLine("Clip_Range " + ConfigFile.ClipRange);
            INIConfigFile.WriteLine("Anisotoropic " + ConfigFile.AnisotropicFiltering);
            INIConfigFile.WriteLine("Pad_Assign1" + PadControls1);
            INIConfigFile.WriteLine("Pad_Assign2" + PadControls2);
            INIConfigFile.WriteLine("Mouse_Assign" + MouseControls);
            INIConfigFile.WriteLine("Screen_Size_Selection 3");
            INIConfigFile.WriteLine("Screen_Full " + ConfigFile.FullScreen);
            INIConfigFile.WriteLine("Language " + ConfigFile.Language);
            INIConfigFile.WriteLine("3D_Sound " + ConfigFile.SurroundSound);
            INIConfigFile.WriteLine("SE_Volume " + ConfigFile.SFXVolume);
            INIConfigFile.WriteLine("SE_On " + ConfigFile.SFXToggle);
            INIConfigFile.WriteLine("BGM_Volume " + ConfigFile.BGMVolume);
            INIConfigFile.WriteLine("BGM_On " + ConfigFile.BGMToggle);
            INIConfigFile.WriteLine("Cheap_Shadow " + ConfigFile.SoftShadows);
            INIConfigFile.WriteLine("Mouse_Control_Type " + ConfigFile.MouseControlType);
            INIConfigFile.Dispose();
        }

        /// <summary>
        /// Mod Loader DLL Skeleton Code | DO NOT TOUCH
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static System.Reflection.Assembly CurrentDomain_SetAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string Folder_Path = Directory.GetCurrentDirectory() + @"\Mod-Loader-Libraries\"; // Path of current folder where the assembly is (Inside RAM of TSonic_win.exe therefore this gets current game directory). Append Mod-Loader-Libraries :)
            string Assembly_Path = Path.Combine(Folder_Path, new AssemblyName(args.Name).Name + ".dll"); // The new path for the assembly! Including the library path.s
            if (!File.Exists(Assembly_Path)) return null; // If the assembly does not exist, return null.
            Assembly Assembly_Physical = Assembly.LoadFrom(Assembly_Path); // Else just load the assembly :)
            return Assembly_Physical;
        }

        /// <summary>
        /// Retrieves a range of bytes from an array.s
        /// </summary>
        /// <param name="Source_Array"></param>
        /// <param name="Length"></param>
        /// <param name="Starting_Byte"></param>
        /// <returns></returns>
        public static byte[] Get_Byte_Range_From_Array(byte[] Source_Array, int Length, int Starting_Byte)
        {
            byte[] Bytes = new byte[Length]; // Set length to bytes
            int Max = Starting_Byte + Length; // Maximum Iteration
            int Y = 0;
            for (int x = Starting_Byte; x < Max; x++)
            {
                Bytes[Y] = Source_Array[x];
                Y++;
            }
            return Bytes;
        }
    }
}
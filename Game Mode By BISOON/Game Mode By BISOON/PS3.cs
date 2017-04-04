using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


    class PS3
    {
        public static PS3API API = new PS3API();
        public enum Lang
        {
            Null,
            French,
            English,
            Arabic
        }

        public enum SelectAPI
        {
            ControlConsole,
            TargetManager
        }

        public class PS3API
        {
            private static string targetName = String.Empty;
            private static string targetIp = String.Empty;
            public PS3API(SelectAPI API = SelectAPI.TargetManager)
            {
                SetAPI.API = API;
                MakeInstanceAPI(API);
            }

            public void setTargetName(string value)
            {
                targetName = value;
            }

            private void MakeInstanceAPI(SelectAPI API)
            {
                if (API == SelectAPI.TargetManager)
                    if (Common.TmApi == null)
                        Common.TmApi = new TMAPI();
                if (API == SelectAPI.ControlConsole)
                    if (Common.CcApi == null)
                        Common.CcApi = new CCAPI();
            }

            private class SetLang
            {
                public static Lang defaultLang = Lang.Null;
            }

            private class SetAPI
            {
                public static SelectAPI API;
            }

            private class Common
            {
                public static CCAPI CcApi;
                public static TMAPI TmApi;
            }

            /// <summary>Force a language for the console list popup.</summary>
            public void SetFormLang(Lang Language)
            {
                SetLang.defaultLang = Language;
            }

            /// <summary>init again the connection if you use a Thread or a Timer.</summary>
            public void InitTarget()
            {
                if (SetAPI.API == SelectAPI.TargetManager)
                    Common.TmApi.InitComms();
            }

            /// <summary>Connect your console with selected API.</summary>
            public bool ConnectTarget(int target = 0)
            {
                // We'll check again if the instance has been done, else you'll got an exception error.
                MakeInstanceAPI(GetCurrentAPI());

                bool result = false;
                if (SetAPI.API == SelectAPI.TargetManager)
                    result = Common.TmApi.ConnectTarget(target);
                else
                    result = new ConsoleList(API).Show();
                return result;
            }

            public void Reconnect(int target = 0)
            {
                if (SetAPI.API == SelectAPI.TargetManager)
                     Common.TmApi.ConnectTarget(target);

            }

            /// <summary>Connect your console with CCAPI.</summary>
            public bool ConnectTarget(string ip)
            {
                // We'll check again if the instance has been done.
                MakeInstanceAPI(GetCurrentAPI());
                if (Common.CcApi.SUCCESS(Common.CcApi.ConnectTarget(ip)))
                {
                    targetIp = ip;
                    return true;
                }
                else return false;
            }

            /// <summary>Disconnect Target with selected API.</summary>
            public void DisconnectTarget()
            {
                if (SetAPI.API == SelectAPI.TargetManager)
                    Common.TmApi.DisconnectTarget();
                else Common.CcApi.DisconnectTarget();
            }

            /// <summary>Attach the current process (current Game) with selected API.</summary>
            public bool AttachProcess()
            {
                // We'll check again if the instance has been done.
                MakeInstanceAPI(GetCurrentAPI());

                bool AttachResult = false;
                if (SetAPI.API == SelectAPI.TargetManager)
                    AttachResult = Common.TmApi.AttachProcess();
                else if (SetAPI.API == SelectAPI.ControlConsole)
                    AttachResult = Common.CcApi.SUCCESS(Common.CcApi.AttachProcess());
                return AttachResult;
            }

            public string GetConsoleName()
            {
                if (SetAPI.API == SelectAPI.TargetManager)
                    return Common.TmApi.SCE.GetTargetName();
                else
                {
                    if (targetName != String.Empty)
                        return targetName;

                    if (targetIp != String.Empty)
                    {
                        List<CCAPI.ConsoleInfo> Data = new List<CCAPI.ConsoleInfo>();
                        Data = Common.CcApi.GetConsoleList();
                        if (Data.Count > 0)
                        {
                            for (int i = 0; i < Data.Count; i++)
                                if (Data[i].Ip == targetIp)
                                    return Data[i].Name;
                        }
                    }
                    return targetIp;
                }
            }

            /// <summary>Set memory to offset with selected API.</summary>
            public void SetMemory(uint offset, byte[] buffer)
            {
                if (SetAPI.API == SelectAPI.TargetManager)
                    Common.TmApi.SetMemory(offset, buffer);
                else if (SetAPI.API == SelectAPI.ControlConsole)
                    Common.CcApi.SetMemory(offset, buffer);
            }

            /// <summary>Get memory from offset using the Selected API.</summary>
            public void GetMemory(uint offset, byte[] buffer)
            {
                if (SetAPI.API == SelectAPI.TargetManager)
                    Common.TmApi.GetMemory(offset, buffer);
                else if (SetAPI.API == SelectAPI.ControlConsole)
                    Common.CcApi.GetMemory(offset, buffer);
            }
            public  void GetMemory(uint offset, ref byte[] buffer)
            {
                if (SetAPI.API == SelectAPI.TargetManager)
                    Common.TmApi.GetMemory(offset, buffer);
                else if (SetAPI.API == SelectAPI.ControlConsole)
                    Common.CcApi.GetMemory(offset, buffer);
            }
            public  byte[] ReverseArray(float value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return bytes;
            }
            public  byte[] ReverseBytes(byte[] inArray)
            {
                Array.Reverse(inArray);
                return inArray;
            }
            public  byte[] uintBytes(uint input)
            {
                byte[] bytes = BitConverter.GetBytes(input);
                Array.Reverse(bytes);
                return bytes;
            }
            public  byte[] GetMemory(uint offset, int length)
            {
                byte[] buffer = new byte[length];
                if (SetAPI.API == SelectAPI.TargetManager)
                    Common.TmApi.GetMemory(offset, buffer);
                else if (SetAPI.API == SelectAPI.ControlConsole)
                    Common.CcApi.GetMemory(offset, buffer);
                return buffer;
            }

            /// <summary>Get memory from offset with a length using the Selected API.</summary>
            public byte[] GetBytes(uint offset, int length)
            {
                byte[] buffer = new byte[length];
                if (SetAPI.API == SelectAPI.TargetManager)
                    Common.TmApi.GetMemory(offset, buffer);
                else if (SetAPI.API == SelectAPI.ControlConsole)
                    Common.CcApi.GetMemory(offset, buffer);
                return buffer;
            }

            /// <summary>Change current API.</summary>
            public void ChangeAPI(SelectAPI API)
            {
                SetAPI.API = API;
                MakeInstanceAPI(GetCurrentAPI());
            }

            /// <summary>Return selected API.</summary>
            public SelectAPI GetCurrentAPI()
            {
                return SetAPI.API;
            }

            /// <summary>Return selected API into string format.</summary>
            public string GetCurrentAPIName()
            {
                string output = String.Empty;
                if (SetAPI.API == SelectAPI.TargetManager)
                    output = Enum.GetName(typeof(SelectAPI), SelectAPI.TargetManager).Replace("Manager", " Manager");
                else output = Enum.GetName(typeof(SelectAPI), SelectAPI.ControlConsole).Replace("Console", " Console");
                return output;
            }

            /// <summary>This will find the dll ps3tmapi_net.dll for TMAPI.</summary>
            public Assembly PS3TMAPI_NET()
            {
                return Common.TmApi.PS3TMAPI_NET();
            }

            /// <summary>Use the extension class with your selected API.</summary>
            public Extension Extension
            {
                get { return new Extension(SetAPI.API); }
            }

            /// <summary>Access to all TMAPI functions.</summary>
            public TMAPI TMAPI
            {
                get { return new TMAPI(); }
            }

            /// <summary>Access to all CCAPI functions.</summary>
            public CCAPI CCAPI
            {
                get { return new CCAPI(); }
            }

            public class ConsoleList
            {
                private PS3API Api;
                private List<CCAPI.ConsoleInfo> data;

                public ConsoleList(PS3API f)
                {
                    Api = f;
                    data = Api.CCAPI.GetConsoleList();
                }

                /// <summary>Return the systeme language, if it's others all text will be in english.</summary>
                private Lang getSysLanguage()
                {
                    if (SetLang.defaultLang == Lang.Null)
                    {
                        if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("FRA"))
                            return Lang.French;
                        else if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("ARA"))
                            return Lang.Arabic;
                        else return Lang.English;
                    }
                    else return SetLang.defaultLang;
                }

                private string strTraduction(string keyword)
                {
                    if (getSysLanguage() == Lang.French)
                    {
                        switch (keyword)
                        {
                            case "btnConnect": return "Connexion";
                            case "btnRefresh": return "Rafraîchir";
                            case "errorSelect": return "Vous devez d'abord sélectionner une console.";
                            case "errorSelectTitle": return "Sélectionnez une console.";
                            case "selectGrid": return "Sélectionnez une console dans la grille.";
                            case "selectedLbl": return "Sélection :";
                            case "formTitle": return "Choisissez une console...";
                            case "noConsole": return "Aucune console disponible, démarrez CCAPI Manager (v2.5) et ajoutez une nouvelle console.";
                            case "noConsoleTitle": return "Aucune console disponible.";
                        }
                    }
                    else if (getSysLanguage() == Lang.English)
                    {
                        switch (keyword)
                        {
                            case "btnConnect": return "Connection";
                            case "btnRefresh": return "Refresh";
                            case "errorSelect": return "You need to select a console first.";
                            case "errorSelectTitle": return "Select a console.";
                            case "selectGrid": return "Select a console ..";
                            case "selectedLbl": return "Selected :";
                            case "formTitle": return "Consoles List..";
                            case "noConsole": return "None consoles available, run CCAPI Manager (v2.6) and add a new console.";
                            case "noConsoleTitle": return "None console available.";
                        }
                    }
                    else if (getSysLanguage() == Lang.Arabic)
                    {
                        switch (keyword)
                        {
                            case "btnConnect": return "إتصال";
                            case "btnRefresh": return "تحديث";
                            case "errorSelect": return "تحتاج إلى تحديد جهاز ..";
                            case "errorSelectTitle": return "إختار جهاز";
                            case "selectGrid": return "حدد جهاز ..";
                            case "selectedLbl": return "محدد :";
                            case "formTitle": return "قائمة الأجهزة";
                            case "noConsole": return "لا يوجد جهاز ..! تحتاج إلى إضافة جهاز عن طريق Control Console";
                            case "noConsoleTitle": return "لا يوجد جهاز ..";
                        }
                    }
                    return "?";
                }

                public bool Show()
                {
                    bool Result = false;
                    int tNum = -1;

                    // Instance of widgets
                    Label lblInfo = new Label();
                    Button btnConnect = new Button();
                    Button btnRefresh = new Button();
                    ListViewGroup listViewGroup = new ListViewGroup("Consoles", HorizontalAlignment.Left);
                    ListView listView = new ListView();
                    Form formList = new Form();

                    // Create our button connect
                    btnConnect.Location = new Point(12, 195);
                    btnConnect.Name = "btnConnect";
                    btnConnect.Size = new Size(198, 23);
                    btnConnect.TabIndex = 1;
                    btnConnect.Text = strTraduction("btnConnect");
                    btnConnect.UseVisualStyleBackColor = true;
                    btnConnect.Enabled = false;
                    btnConnect.Click += (sender, e) =>
                    {
                        if (tNum > -1)
                        {
                            if (Api.ConnectTarget(data[tNum].Ip))
                            {
                                Api.setTargetName(data[tNum].Name);
                                Result = true;
                            }
                            else Result = false;
                            formList.Close();
                        }
                        else
                            MessageBox.Show(strTraduction("errorSelect"), strTraduction("errorSelectTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    };

                    // Create our button refresh
                    btnRefresh.Location = new Point(216, 195);
                    btnRefresh.Name = "btnRefresh";
                    btnRefresh.Size = new Size(86, 23);
                    btnRefresh.TabIndex = 1;
                    btnRefresh.Text = strTraduction("btnRefresh");
                    btnRefresh.UseVisualStyleBackColor = true;
                    btnRefresh.Click += (sender, e) =>
                    {
                        tNum = -1;
                        listView.Clear();
                        lblInfo.Text = strTraduction("selectGrid");
                        btnConnect.Enabled = false;
                        data = Api.CCAPI.GetConsoleList();
                        int sizeD = data.Count();
                        for (int i = 0; i < sizeD; i++)
                        {
                            ListViewItem item = new ListViewItem(" " + data[i].Name + " - " + data[i].Ip);
                            item.ImageIndex = 0;
                            listView.Items.Add(item);
                        }
                    };

                    // Create our list view
                    listView.Font = new Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    listViewGroup.Header = "Consoles";
                    listViewGroup.Name = "consoleGroup";
                    listView.Groups.AddRange(new ListViewGroup[] { listViewGroup });
                    listView.HideSelection = false;
                    listView.Location = new Point(12, 12);
                    listView.MultiSelect = false;
                    listView.Name = "ConsoleList";
                    listView.ShowGroups = false;
                    listView.Size = new Size(290, 160);
                    listView.TabIndex = 0;
                    listView.UseCompatibleStateImageBehavior = false;
                    listView.View = View.List;
                    Color clr = Color.FromArgb(0, 128, 128);
                    Color Clr = Color.FromArgb(clr.R, clr.G, clr.B);

                    //listView.BackColor = clr;
                    listView.ItemSelectionChanged += (sender, e) =>
                    {
                        tNum = e.ItemIndex;
                        btnConnect.Enabled = true;
                        string Name, Ip = "?";
                        if (data[tNum].Name.Length > 18)
                            Name = data[tNum].Name.Substring(0, 17) + "...";
                        else Name = data[tNum].Name;
                        if (data[tNum].Ip.Length > 16)
                            Ip = data[tNum].Name.Substring(0, 16) + "...";
                        else Ip = data[tNum].Ip;
                        lblInfo.Text = strTraduction("selectedLbl") + " " + Name + " / " + Ip;
                    };

                    // Create our label
                    lblInfo.AutoSize = true;
                    lblInfo.Location = new Point(12, 180);
                    lblInfo.Name = "lblInfo";
                    lblInfo.Size = new Size(158, 13);
                    lblInfo.TabIndex = 3;
                    lblInfo.Text = strTraduction("selectGrid");

                    // Create our form
                    formList.MinimizeBox = false;
                    formList.MaximizeBox = false;
                    formList.ClientSize = new Size(314, (285 - 55));
                    formList.AutoScaleDimensions = new SizeF(6F, 13F);
                    formList.AutoScaleMode = AutoScaleMode.Font;
                    formList.FormBorderStyle = FormBorderStyle.Fixed3D;
                    formList.StartPosition = FormStartPosition.CenterScreen;
                    formList.Text = strTraduction("formTitle");
                    formList.Controls.Add(listView);
                    formList.Controls.Add(lblInfo);
                    formList.Controls.Add(btnConnect);
                    formList.Controls.Add(btnRefresh);

                    //Color temp = Color.FromArgb(45, 47, 49);
                    //Color result = Color.FromArgb(temp.R, temp.G, temp.B);
                    //formList.BackColor = result;

                    Icon appIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                    formList.Icon = appIcon;
                    // Start to update our list
                    ImageList imgL = new ImageList();
                    imgL.Images.Add(appIcon);
                    listView.SmallImageList = imgL;
                    int sizeData = data.Count();

                    for (int i = 0; i < sizeData; i++)
                    {
                        ListViewItem item = new ListViewItem(" " + data[i].Name + " - " + data[i].Ip);
                        item.ImageIndex = 0;
                        listView.Items.Add(item);
                    }

                    // If there are more than 0 targets we show the form
                    // Else we inform the user to create a console.
                    if (sizeData > 0)
                        formList.ShowDialog();
                    else
                    {
                        Result = false;
                        formList.Close();
                        MessageBox.Show(strTraduction("noConsole"), strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return Result;
                }
            }
        }

        public class TMAPI
        {
            public static int Target = 0xFF;
            public static bool AssemblyLoaded = true;
            public static PS3TMAPI.ResetParameter resetParameter;

            public TMAPI()
            {

            }

            public Extension Extension
            {
                get { return new Extension(SelectAPI.TargetManager); }
            }

            public class SCECMD
            {
                /// <summary>Get the target status and return the string value.</summary>
                public string SNRESULT()
                {
                    return Parameters.snresult;
                }

                /// <summary>Get the target name.</summary>
                public string GetTargetName()
                {
                    if (Parameters.ConsoleName == null || Parameters.ConsoleName == String.Empty)
                    {
                        PS3TMAPI.InitTargetComms();
                        PS3TMAPI.TargetInfo TargetInfo = new PS3TMAPI.TargetInfo();
                        TargetInfo.Flags = PS3TMAPI.TargetInfoFlag.TargetID;
                        TargetInfo.Target = TMAPI.Target;
                        PS3TMAPI.GetTargetInfo(ref TargetInfo);
                        Parameters.ConsoleName = TargetInfo.Name;
                    }
                    return Parameters.ConsoleName;
                }

                /// <summary>Get the target status and return the string value.</summary>
                public string GetStatus()
                {
                    if (TMAPI.AssemblyLoaded)
                        return "NotConnected";
                    Parameters.connectStatus = new PS3TMAPI.ConnectStatus();
                    PS3TMAPI.GetConnectStatus(Target, out Parameters.connectStatus, out Parameters.usage);
                    Parameters.Status = Parameters.connectStatus.ToString();
                    return Parameters.Status;
                }

                /// <summary>Get the ProcessID by the current process.</summary>
                public uint ProcessID()
                {
                    return Parameters.ProcessID;
                }

                /// <summary>Get an array of processID's.</summary>
                public uint[] ProcessIDs()
                {
                    return Parameters.processIDs;
                }

                /// <summary>Get some details from your target.</summary>
                public PS3TMAPI.ConnectStatus DetailStatus()
                {
                    return Parameters.connectStatus;
                }
            }

            public SCECMD SCE
            {
                get { return new SCECMD(); }
            }

            public class Parameters
            {
                public static string
                    usage,
                    info,
                    snresult,
                    Status,
                    MemStatus,
                    ConsoleName;
                public static uint
                    ProcessID;
                public static uint[]
                    processIDs;
                public static byte[]
                    Retour;
                public static PS3TMAPI.ConnectStatus
                    connectStatus;
            }

            /// <summary>Enum of flag reset.</summary>
            public enum ResetTarget
            {
                Hard,
                Quick,
                ResetEx,
                Soft
            }

            public void InitComms()
            {
                PS3TMAPI.InitTargetComms();
            }

            /// <summary>Connect the default target and initialize the dll. Possible to put an int as arugment for determine which target to connect.</summary>
            public bool ConnectTarget(int TargetIndex = 0)
            {
                bool result = false;
                if (AssemblyLoaded)
                    PS3TMAPI_NET();
                AssemblyLoaded = false;
                Target = TargetIndex;
                result = PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
                result = PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(TargetIndex, null));
                return result;
            }

            /// <summary>Connect the target by is name.</summary>
            public bool ConnectTarget(string TargetName)
            {
                bool result = false;
                if (AssemblyLoaded)
                    PS3TMAPI_NET();
                AssemblyLoaded = false;
                result = PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
                if (result)
                {
                    result = PS3TMAPI.SUCCEEDED(PS3TMAPI.GetTargetFromName(TargetName, out Target));
                    result = PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(Target, null));
                }
                return result;
            }

            /// <summary>Disconnect the target.</summary>
            public void DisconnectTarget()
            {
                PS3TMAPI.Disconnect(Target);
            }

            /// <summary>Power on selected target.</summary>
            public void PowerOn(int numTarget = 0)
            {
                if (Target != 0xFF)
                    numTarget = Target;
                PS3TMAPI.PowerOn(numTarget);
            }

            /// <summary>Power off selected target.</summary>
            public void PowerOff(bool Force)
            {
                PS3TMAPI.PowerOff(Target, Force);
            }

            /// <summary>Attach and continue the current process from the target.</summary>
            public bool AttachProcess()
            {
                bool isOK = false;
                PS3TMAPI.GetProcessList(Target, out Parameters.processIDs);
                if (Parameters.processIDs.Length > 0)
                    isOK = true;
                else isOK = false;
                if (isOK)
                {
                    ulong uProcess = Parameters.processIDs[0];
                    Parameters.ProcessID = Convert.ToUInt32(uProcess);
                    PS3TMAPI.ProcessAttach(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID);
                    PS3TMAPI.ProcessContinue(Target, Parameters.ProcessID);
                    Parameters.info = "The Process 0x" + Parameters.ProcessID.ToString("X8") + " Has Been Attached !";
                }
                return isOK;
            }

            /// <summary>Set memory to the target (byte[]).</summary>
            public void SetMemory(uint Address, byte[] Bytes)
            {
                PS3TMAPI.ProcessSetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, Bytes);
            }

            /// <summary>Set memory to the address (byte[]).</summary>
            public void SetMemory(uint Address, ulong value)
            {
                byte[] b = BitConverter.GetBytes(value);
                Array.Reverse(b);
                PS3TMAPI.ProcessSetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, b);
            }

            /// <summary>Set memory with value as string hexadecimal to the address (string).</summary>
            public void SetMemory(uint Address, string hexadecimal, EndianType Type = EndianType.BigEndian)
            {
                byte[] Entry = StringToByteArray(hexadecimal);
                if (Type == EndianType.LittleEndian)
                    Array.Reverse(Entry);
                PS3TMAPI.ProcessSetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, Entry);
            }

            /// <summary>Get memory from the address.</summary>
            public void GetMemory(uint Address, byte[] Bytes)
            {
                PS3TMAPI.ProcessGetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Bytes);
            }

            /// <summary>Get a bytes array with the length input.</summary>
            public byte[] GetBytes(uint Address, uint lengthByte)
            {
                byte[] Longueur = new byte[lengthByte];
                PS3TMAPI.ProcessGetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Longueur);
                return Longueur;
            }

            /// <summary>Get a string with the length input.</summary>
            public string GetString(uint Address, uint lengthString)
            {
                byte[] Longueur = new byte[lengthString];
                PS3TMAPI.ProcessGetMemory(Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Longueur);
                string StringResult = Hex2Ascii(ReplaceString(Longueur));
                return StringResult;
            }

            internal static string Hex2Ascii(string iMCSxString)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i <= (iMCSxString.Length - 2); i += 2)
                {
                    builder.Append(Convert.ToString(Convert.ToChar(int.Parse(iMCSxString.Substring(i, 2), NumberStyles.HexNumber))));
                }
                return builder.ToString();
            }

            internal static byte[] StringToByteArray(string hex)
            {
                string replace = hex.Replace("0x", "");
                string Stringz = replace.Insert(replace.Length - 1, "0");

                int Odd = replace.Length;
                bool Nombre;
                if (Odd % 2 == 0)
                    Nombre = true;
                else
                    Nombre = false;
                try
                {
                    if (Nombre == true)
                    {
                        return Enumerable.Range(0, replace.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(replace.Substring(x, 2), 16))
                     .ToArray();
                    }
                    else
                    {
                        return Enumerable.Range(0, replace.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(Stringz.Substring(x, 2), 16))
                     .ToArray();
                    }
                }
                catch { throw new System.ArgumentException("Value not possible.", "Byte Array"); }
            }

            internal static string ReplaceString(byte[] bytes)
            {
                string PSNString = BitConverter.ToString(bytes);
                PSNString = PSNString.Replace("00", string.Empty);
                PSNString = PSNString.Replace("-", string.Empty);
                for (int i = 0; i < 10; i++)
                    PSNString = PSNString.Replace("^" + i.ToString(), string.Empty);
                return PSNString;
            }

            /// <summary>Reset target to XMB , Sometimes the target restart quickly.</summary>
            public void ResetToXMB(ResetTarget flag)
            {
                if (flag == ResetTarget.Hard)
                    resetParameter = PS3TMAPI.ResetParameter.Hard;
                else if (flag == ResetTarget.Quick)
                    resetParameter = PS3TMAPI.ResetParameter.Quick;
                else if (flag == ResetTarget.ResetEx)
                    resetParameter = PS3TMAPI.ResetParameter.ResetEx;
                else if (flag == ResetTarget.Soft)
                    resetParameter = PS3TMAPI.ResetParameter.Soft;
                PS3TMAPI.Reset(Target, resetParameter);
            }

            internal static Assembly LoadApi;
            ///<summary>Load the PS3 API for use with your Application .NET.</summary>
            public Assembly PS3TMAPI_NET()
            {
                AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                {
                    var filename = new AssemblyName(e.Name).Name;
                    var x = string.Format(@"C:\Program Files\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                    var x64 = string.Format(@"C:\Program Files (x64)\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                    var x86 = string.Format(@"C:\Program Files (x86)\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                    if (System.IO.File.Exists(x))
                        LoadApi = Assembly.LoadFile(x);
                    else
                    {
                        if (System.IO.File.Exists(x64))
                            LoadApi = Assembly.LoadFile(x64);

                        else
                        {
                            if (System.IO.File.Exists(x86))
                                LoadApi = Assembly.LoadFile(x86);
                            else
                            {
                                MessageBox.Show("Target Manager API cannot be founded to:\r\n\r\n" + x86, "Error with PS3 API!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    return LoadApi;
                };
                return LoadApi;
            }
        }
        public class PS3TMAPI
        {
            public enum SNRESULT
            {
                SN_E_BAD_ALIGN = -28,
                SN_E_BAD_MEMSPACE = -18,
                SN_E_BAD_PARAM = -21,
                SN_E_BAD_TARGET = -3,
                SN_E_BAD_UNIT = -11,
                SN_E_BUSY = -22,
                SN_E_CHECK_TARGET_CONFIGURATION = -33,
                SN_E_COMMAND_CANCELLED = -36,
                SN_E_COMMS_ERR = -5,
                SN_E_COMMS_EVENT_MISMATCHED_ERR = -39,
                SN_E_CONNECT_TO_GAMEPORT_FAILED = -35,
                SN_E_CONNECTED = -38,
                SN_E_DATA_TOO_LONG = -26,
                SN_E_DECI_ERROR = -23,
                SN_E_DEPRECATED = -27,
                SN_E_DLL_NOT_INITIALISED = -15,
                SN_E_ERROR = -2147483648,
                SN_E_EXISTING_CALLBACK = -24,
                SN_E_FILE_ERROR = -29,
                SN_E_HOST_NOT_FOUND = -8,
                SN_E_INSUFFICIENT_DATA = -25,
                SN_E_LICENSE_ERROR = -32,
                SN_E_LOAD_ELF_FAILED = -10,
                SN_E_LOAD_MODULE_FAILED = -31,
                SN_E_MODULE_NOT_FOUND = -34,
                SN_E_NO_SEL = -20,
                SN_E_NO_TARGETS = -19,
                SN_E_NOT_CONNECTED = -4,
                SN_E_NOT_IMPL = -1,
                SN_E_NOT_LISTED = -13,
                SN_E_NOT_SUPPORTED_IN_SDK_VERSION = -30,
                SN_E_OUT_OF_MEM = -12,
                SN_E_PROTOCOL_ALREADY_REGISTERED = -37,
                SN_E_TARGET_IN_USE = -9,
                SN_E_TARGET_RUNNING = -17,
                SN_E_TIMEOUT = -7,
                SN_E_TM_COMMS_ERR = -6,
                SN_E_TM_NOT_RUNNING = -2,
                SN_E_TM_VERSION = -14,
                SN_S_NO_ACTION = 6,
                SN_S_NO_MSG = 3,
                SN_S_OK = 0,
                SN_S_PENDING = 1,
                SN_S_REPLACED = 5,
                SN_S_TARGET_STILL_REGISTERED = 7,
                SN_S_TM_VERSION = 4
            }

            public enum UnitType
            {
                PPU,
                SPU,
                SPURAW
            }

            [Flags]
            public enum ResetParameter : ulong
            {
                Hard = 1L,
                Quick = 2L,
                ResetEx = 9223372036854775808L,
                Soft = 0L
            }

            private class ScopedGlobalHeapPtr
            {
                private IntPtr m_intPtr = IntPtr.Zero;

                public ScopedGlobalHeapPtr(IntPtr intPtr)
                {
                    this.m_intPtr = intPtr;
                }

                ~ScopedGlobalHeapPtr()
                {
                    if (this.m_intPtr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(this.m_intPtr);
                    }
                }

                public IntPtr Get()
                {
                    return this.m_intPtr;
                }
            }

            public enum ConnectStatus
            {
                Connected,
                Connecting,
                NotConnected,
                InUse,
                Unavailable
            }

            [StructLayout(LayoutKind.Sequential)]
            public class TCPIPConnectProperties
            {
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xff)]
                public string IPAddress;
                public uint Port;
            }

            [Flags]
            public enum TargetInfoFlag : uint
            {
                Boot = 0x20,
                FileServingDir = 0x10,
                HomeDir = 8,
                Info = 4,
                Name = 2,
                TargetID = 1
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct TargetInfoPriv
            {
                public PS3TMAPI.TargetInfoFlag Flags;
                public int Target;
                public IntPtr Name;
                public IntPtr Type;
                public IntPtr Info;
                public IntPtr HomeDir;
                public IntPtr FSDir;
                public PS3TMAPI.BootParameter Boot;
            }

            [Flags]
            public enum BootParameter : ulong
            {
                BluRayEmuOff = 4L,
                BluRayEmuUSB = 0x20L,
                DebugMode = 0x10L,
                Default = 0L,
                DualNIC = 0x80L,
                HDDSpeedBluRayEmu = 8L,
                HostFSTarget = 0x40L,
                MemSizeConsole = 2L,
                ReleaseMode = 1L,
                SystemMode = 0x11L
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct TargetInfo
            {
                public PS3TMAPI.TargetInfoFlag Flags;
                public int Target;
                public string Name;
                public string Type;
                public string Info;
                public string HomeDir;
                public string FSDir;
                public PS3TMAPI.BootParameter Boot;
            }

            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT InitTargetCommsX64();
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT InitTargetCommsX86();
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT PowerOnX64(int target);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT PowerOnX86(int target);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT PowerOffX64(int target, uint force);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT PowerOffX86(int target, uint force);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ConnectX64(int target, string application);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ConnectX86(int target, string application);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetConnectionInfoX64(int target, IntPtr connectProperties);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetConnectionInfoX86(int target, IntPtr connectProperties);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetConnectStatusX64(int target, out uint status, out IntPtr usage);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetConnectStatusX86(int target, out uint status, out IntPtr usage);
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern int MultiByteToWideChar(int codepage, int flags, IntPtr utf8, int utf8len, StringBuilder buffer, int buflen);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetProcessListX64(int target, ref uint count, IntPtr processIdArray);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetProcessListX86(int target, ref uint count, IntPtr processIdArray);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessContinueX64(int target, uint processId);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessContinueX86(int target, uint processId);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessAttachX64(int target, uint unitId, uint processId);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessAttachX86(int target, uint unitId, uint processId);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessGetMemoryX64(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessGetMemoryX86(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetTargetFromNameX64(IntPtr name, out int target);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetTargetFromNameX86(IntPtr name, out int target);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ResetX64(int target, ulong resetParameter);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ResetX86(int target, ulong resetParameter);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessSetMemoryX64(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT ProcessSetMemoryX86(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetTargetInfoX64(ref TargetInfoPriv targetInfoPriv);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT GetTargetInfoX86(ref TargetInfoPriv targetInfoPriv);
            [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT DisconnectX64(int target);
            [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
            private static extern SNRESULT DisconnectX86(int target);
            private static bool Is32Bit()
            {
                return (IntPtr.Size == 4);
            }

            public static bool FAILED(SNRESULT res)
            {
                return !SUCCEEDED(res);
            }

            public static bool SUCCEEDED(SNRESULT res)
            {
                return (res >= SNRESULT.SN_S_OK);
            }

            private static IntPtr AllocUtf8FromString(string wcharString)
            {
                if (wcharString == null)
                {
                    return IntPtr.Zero;
                }
                byte[] bytes = Encoding.UTF8.GetBytes(wcharString);
                IntPtr destination = Marshal.AllocHGlobal((int)(bytes.Length + 1));
                Marshal.Copy(bytes, 0, destination, bytes.Length);
                Marshal.WriteByte((IntPtr)(destination.ToInt64() + bytes.Length), 0);
                return destination;
            }

            public static string Utf8ToString(IntPtr utf8, uint maxLength)
            {
                int len = MultiByteToWideChar(65001, 0, utf8, -1, null, 0);
                if (len == 0) throw new System.ComponentModel.Win32Exception();
                var buf = new StringBuilder(len);
                len = MultiByteToWideChar(65001, 0, utf8, -1, buf, len);
                return buf.ToString();
            }

            private static IntPtr ReadDataFromUnmanagedIncPtr<T>(IntPtr unmanagedBuf, ref T storage)
            {
                storage = (T)Marshal.PtrToStructure(unmanagedBuf, typeof(T));
                return new IntPtr(unmanagedBuf.ToInt64() + Marshal.SizeOf((T)storage));
            }

            public static SNRESULT InitTargetComms()
            {
                if (!Is32Bit())
                {
                    return InitTargetCommsX64();
                }
                return InitTargetCommsX86();
            }

            public static SNRESULT Connect(int target, string application)
            {
                if (!Is32Bit())
                {
                    return ConnectX64(target, application);
                }
                return ConnectX86(target, application);
            }

            public static SNRESULT PowerOn(int target)
            {
                if (!Is32Bit())
                {
                    return PowerOnX64(target);
                }
                return PowerOnX86(target);
            }

            public static SNRESULT PowerOff(int target, bool bForce)
            {
                uint force = bForce ? (uint)1 : 0;
                if (!Is32Bit())
                {
                    return PowerOffX64(target, force);
                }
                return PowerOffX86(target, force);
            }

            public static SNRESULT GetProcessList(int target, out uint[] processIDs)
            {
                processIDs = null;
                uint count = 0;
                SNRESULT res = Is32Bit() ? GetProcessListX86(target, ref count, IntPtr.Zero) : GetProcessListX64(target, ref count, IntPtr.Zero);
                if (!FAILED(res))
                {
                    ScopedGlobalHeapPtr ptr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal((int)(4 * count)));
                    res = Is32Bit() ? GetProcessListX86(target, ref count, ptr.Get()) : GetProcessListX64(target, ref count, ptr.Get());
                    if (FAILED(res))
                    {
                        return res;
                    }
                    IntPtr unmanagedBuf = ptr.Get();
                    processIDs = new uint[count];
                    for (uint i = 0; i < count; i++)
                    {
                        unmanagedBuf = ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processIDs[i]);
                    }
                }
                return res;
            }

            public static SNRESULT ProcessAttach(int target, UnitType unit, uint processID)
            {
                if (!Is32Bit())
                {
                    return ProcessAttachX64(target, (uint)unit, processID);
                }
                return ProcessAttachX86(target, (uint)unit, processID);
            }

            public static SNRESULT ProcessContinue(int target, uint processID)
            {
                if (!Is32Bit())
                {
                    return ProcessContinueX64(target, processID);
                }
                return ProcessContinueX86(target, processID);
            }

            public static SNRESULT GetTargetInfo(ref TargetInfo targetInfo)
            {
                TargetInfoPriv targetInfoPriv = new TargetInfoPriv
                {
                    Flags = targetInfo.Flags,
                    Target = targetInfo.Target
                };
                SNRESULT res = Is32Bit() ? GetTargetInfoX86(ref targetInfoPriv) : GetTargetInfoX64(ref targetInfoPriv);
                if (!FAILED(res))
                {
                    targetInfo.Flags = targetInfoPriv.Flags;
                    targetInfo.Target = targetInfoPriv.Target;
                    targetInfo.Name = Utf8ToString(targetInfoPriv.Name, uint.MaxValue);
                    targetInfo.Type = Utf8ToString(targetInfoPriv.Type, uint.MaxValue);
                    targetInfo.Info = Utf8ToString(targetInfoPriv.Info, uint.MaxValue);
                    targetInfo.HomeDir = Utf8ToString(targetInfoPriv.HomeDir, uint.MaxValue);
                    targetInfo.FSDir = Utf8ToString(targetInfoPriv.FSDir, uint.MaxValue);
                    targetInfo.Boot = targetInfoPriv.Boot;
                }
                return res;
            }

            public static SNRESULT GetTargetFromName(string name, out int target)
            {
                ScopedGlobalHeapPtr ptr = new ScopedGlobalHeapPtr(AllocUtf8FromString(name));
                if (!Is32Bit())
                {
                    return GetTargetFromNameX64(ptr.Get(), out target);
                }
                return GetTargetFromNameX86(ptr.Get(), out target);
            }

            public static SNRESULT GetConnectionInfo(int target, out TCPIPConnectProperties connectProperties)
            {
                connectProperties = null;
                ScopedGlobalHeapPtr ptr = new ScopedGlobalHeapPtr(Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TCPIPConnectProperties))));
                SNRESULT res = Is32Bit() ? GetConnectionInfoX86(target, ptr.Get()) : GetConnectionInfoX64(target, ptr.Get());
                if (SUCCEEDED(res))
                {
                    connectProperties = new TCPIPConnectProperties();
                    Marshal.PtrToStructure(ptr.Get(), connectProperties);
                }
                return res;
            }

            public static SNRESULT GetConnectStatus(int target, out ConnectStatus status, out string usage)
            {
                IntPtr ptr;
                uint num;
                SNRESULT snresult = Is32Bit() ? GetConnectStatusX86(target, out num, out ptr) : GetConnectStatusX64(target, out num, out ptr);
                status = (ConnectStatus)num;
                usage = Utf8ToString(ptr, uint.MaxValue);
                return snresult;
            }

            public static SNRESULT Reset(int target, ResetParameter resetParameter)
            {
                if (!Is32Bit())
                {
                    return ResetX64(target, (ulong)resetParameter);
                }
                return ResetX86(target, (ulong)resetParameter);
            }

            public static SNRESULT ProcessGetMemory(int target, UnitType unit, uint processID, ulong threadID, ulong address, ref byte[] buffer)
            {
                if (!Is32Bit())
                {
                    return ProcessGetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
                }
                return ProcessGetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
            }

            public static SNRESULT ProcessSetMemory(int target, UnitType unit, uint processID, ulong threadID, ulong address, byte[] buffer)
            {
                if (!Is32Bit())
                {
                    return ProcessSetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
                }
                return ProcessSetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
            }

            public static SNRESULT Disconnect(int target)
            {
                if (!Is32Bit())
                {
                    return DisconnectX64(target);
                }
                return DisconnectX86(target);
            }
        }

        public class Extension
        {
            byte[] ReverseBytes(byte[] buffer)
            {
                Array.Reverse(buffer);
                return buffer;
            }
            private SelectAPI CurrentAPI;
            public Extension(SelectAPI API)
            {
                CurrentAPI = API;
                if (API == SelectAPI.TargetManager)
                    if (Common.TmApi == null)
                        Common.TmApi = new TMAPI();
                if (API == SelectAPI.ControlConsole)
                    if (Common.CcApi == null)
                        Common.CcApi = new CCAPI();
            }

            /// <summary>Read a signed byte.</summary>
            public sbyte ReadSByte(uint offset)
            {
                byte[] buffer = new byte[1];
                GetMem(offset, buffer, CurrentAPI);
                return (sbyte)buffer[0];
            }

            /// <summary>Read a byte a check if his value. This return a bool according the byte detected.</summary>
            public bool ReadBool(uint offset)
            {
                byte[] buffer = new byte[1];
                GetMem(offset, buffer, CurrentAPI);
                return buffer[0] != 0;
            }

            /// <summary>Read and return an integer 16 bits.</summary>
            public short ReadInt16(uint offset)
            {
                byte[] buffer = GetBytes(offset, 2, CurrentAPI);
                Array.Reverse(buffer, 0, 2);
                return BitConverter.ToInt16(buffer, 0);
            }

            /// <summary>Read and return an integer 32 bits.</summary>
            public int ReadInt32(uint offset)
            {
                byte[] buffer = GetBytes(offset, 4, CurrentAPI);
                Array.Reverse(buffer, 0, 4);
                return BitConverter.ToInt32(buffer, 0);
            }

            /// <summary>Read and return an integer 64 bits.</summary>
            public long ReadInt64(uint offset)
            {
                byte[] buffer = GetBytes(offset, 8, CurrentAPI);
                Array.Reverse(buffer, 0, 8);
                return BitConverter.ToInt64(buffer, 0);
            }

            /// <summary>Read and return a byte.</summary>
            public byte ReadByte(uint offset)
            {
                byte[] buffer = GetBytes(offset, 1, CurrentAPI);
                return buffer[0];
            }

            /// <summary>Read a string with a length to the first byte equal to an value null (0x00).</summary>
            public byte[] ReadBytes(uint offset, int length)
            {
                byte[] buffer = GetBytes(offset, (uint)length, CurrentAPI);
                return buffer;
            }

            /// <summary>Read and return an unsigned integer 16 bits.</summary>
            public ushort ReadUInt16(uint offset)
            {
                byte[] buffer = GetBytes(offset, 2, CurrentAPI);
                Array.Reverse(buffer, 0, 2);
                return BitConverter.ToUInt16(buffer, 0);
            }

            /// <summary>Read and return an unsigned integer 32 bits.</summary>
            public uint ReadUInt32(uint offset)
            {
                byte[] buffer = GetBytes(offset, 4, CurrentAPI);
                Array.Reverse(buffer, 0, 4);
                return BitConverter.ToUInt32(buffer, 0);
            }
            public  int Read2Bytes(uint Offset, bool Reverse)
            {
                byte[] Bytes = GetBytes(Offset, 2, CurrentAPI);
                if (Reverse)
                {
                    Array.Reverse(Bytes);
                }
                return BitConverter.ToInt16(Bytes, 0);
            }
            public  int Read4Bytes(uint Offset, bool Reverse)
            {
                byte[] bytes = GetBytes(Offset, 4, CurrentAPI);
                if (Reverse)
                {
                    Array.Reverse(bytes);
                }
                return BitConverter.ToInt32(bytes, 0);
            }
            public  ushort ReadUInt16(uint offset, bool Reverse)
            {
                byte[] array = GetBytes(offset, 2, CurrentAPI);
                if (Reverse)
                {
                    Array.Reverse(array, 0, 2);
                }

                return BitConverter.ToUInt16(array, 0);
            }
            public  uint ReadUInt32(uint offset, bool Reverse)
            {
                byte[] array = GetBytes(offset, 4, CurrentAPI);
                if (Reverse)
                {
                    Array.Reverse(array, 0, 4);
                }
                return BitConverter.ToUInt32(array, 0);
            }
            public  ulong ReadUInt64(uint offset, bool Reverse)
            {
                byte[] array = GetBytes(offset, 8, CurrentAPI);
                if (Reverse)
                {
                    Array.Reverse(array, 0, 8);
                }
                return BitConverter.ToUInt64(array, 0);
            }
            public  Int64 Read8Bytes(uint Offset, bool Reverse)
            {
                byte[] bytes = GetBytes(Offset, 8 , CurrentAPI);
                if (Reverse)
                {
                    Array.Reverse(bytes);
                }
                return BitConverter.ToInt64(bytes, 0);
            }
            /// <summary>Read and return an unsigned integer 64 bits.</summary>
            public ulong ReadUInt64(uint offset)
            {
                byte[] buffer = GetBytes(offset, 8, CurrentAPI);
                Array.Reverse(buffer, 0, 8);
                return BitConverter.ToUInt64(buffer, 0);
            }

            /// <summary>Read and return a Float.</summary>
            public float ReadFloat(uint offset)
            {
                byte[] buffer = GetBytes(offset, 4, CurrentAPI);
                Array.Reverse(buffer, 0, 4);
                return BitConverter.ToSingle(buffer, 0);
            }
            public  float[] ReadFloat(uint offset, int length)
            {
                byte[] memory = GetBytes(offset,(uint)length * 4, CurrentAPI);
                ReverseBytes(memory);
                float[] numArray = new float[length - 1 + 1];
                for (int i = 0; i <= length - 1; i++)
                {
                    numArray[i] = System.Convert.ToSingle(BitConverter.ToSingle(memory, ((length - 1) - i) * 4));
                }
                return numArray;
            }
            public float ReadSingle(uint offset)
            {
                byte[] memory = GetBytes(offset, 4, CurrentAPI);
                Array.Reverse(memory, 0, 4);
                return BitConverter.ToSingle(memory, 0);
            }
            public float[] ReadSingle(uint offset, int length)
            {
                byte[] memory = GetBytes(offset, (uint)length * 4, CurrentAPI);
                ReverseBytes(memory);
                float[] numArray = new float[length - 1 + 1];
                for (int i = 0; i <= length - 1; i++)
                {
                    numArray[i] = System.Convert.ToSingle(BitConverter.ToSingle(memory, ((length - 1) - i) * 4));
                }
                return numArray;
            }
            /// <summary>Read a string very fast and stop only when a byte null is detected (0x00).</summary>
            public string ReadString(uint offset)
            {
                int block = 40;
                int addOffset = 0;
                string str = "";
            repeat:
                byte[] buffer = ReadBytes(offset + (uint)addOffset, block);
                str += Encoding.UTF8.GetString(buffer);
                addOffset += block;
                if (str.Contains('\0'))
                {
                    int index = str.IndexOf('\0');
                    string final = str.Substring(0, index);
                    str = String.Empty;
                    return final;
                }
                else
                    goto repeat;
            }

            /// <summary>Write a signed byte.</summary>
            public void WriteSByte(uint offset, sbyte input)
            {
                byte[] buff = new byte[1];
                buff[0] = (byte)input;
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write a boolean.</summary>
            public void WriteBool(uint offset, bool input)
            {
                byte[] buff = new byte[1];
                buff[0] = input ? (byte)1 : (byte)0;
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write an interger 16 bits.</summary>
            public void WriteInt16(uint offset, short input)
            {
                byte[] buff = new byte[2];
                BitConverter.GetBytes(input).CopyTo(buff, 0);
                Array.Reverse(buff, 0, 2);
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write an integer 32 bits.</summary>
            public void WriteInt32(uint offset, int input)
            {
                byte[] buff = new byte[4];
                BitConverter.GetBytes(input).CopyTo(buff, 0);
                Array.Reverse(buff, 0, 4);
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write an integer 64 bits.</summary>
            public void WriteInt64(uint offset, long input)
            {
                byte[] buff = new byte[8];
                BitConverter.GetBytes(input).CopyTo(buff, 0);
                Array.Reverse(buff, 0, 8);
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write a byte.</summary>
            public void WriteByte(uint offset, byte input)
            {
                byte[] buff = new byte[1];
                buff[0] = input;
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write a byte array.</summary>
            public void WriteBytes(uint offset, byte[] input)
            {
                byte[] buff = input;
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write a string.</summary>
            public void WriteString(uint offset, string input)
            {
                byte[] buff = Encoding.UTF8.GetBytes(input);
                Array.Resize(ref buff, buff.Length + 1);
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write an unsigned integer 16 bits.</summary>
            public void WriteUInt16(uint offset, ushort input)
            {
                byte[] buff = new byte[2];
                BitConverter.GetBytes(input).CopyTo(buff, 0);
                Array.Reverse(buff, 0, 2);
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write an unsigned integer 32 bits.</summary>
            public void WriteUInt32(uint offset, uint input)
            {
                byte[] buff = new byte[4];
                BitConverter.GetBytes(input).CopyTo(buff, 0);
                Array.Reverse(buff, 0, 4);
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write an unsigned integer 64 bits.</summary>
            public void WriteUInt64(uint offset, ulong input)
            {
                byte[] buff = new byte[8];
                BitConverter.GetBytes(input).CopyTo(buff, 0);
                Array.Reverse(buff, 0, 8);
                SetMem(offset, buff, CurrentAPI);
            }

            /// <summary>Write a Float.</summary>
            public void WriteFloat(uint offset, float input)
            {
                byte[] buff = new byte[4];
                BitConverter.GetBytes(input).CopyTo(buff, 0);
                Array.Reverse(buff, 0, 4);
                SetMem(offset, buff, CurrentAPI);
            }
            public  void WriteFloat(uint address, float[] input)
            {
                int length = input.Length;
                byte[] array = new byte[length * 4];
                for (int i = 0; i < length; i++)
                {
                    ReverseBytes(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
                }
                SetMem(address, array, CurrentAPI);
            }
            public  void WriteSingle(uint address, float input)
            {
                byte[] array = new byte[4];
                BitConverter.GetBytes(input).CopyTo(array, 0);
                Array.Reverse(array, 0, 4);
                SetMem(address, array, CurrentAPI);
            }

            public  void WriteSingle(uint address, float[] input)
            {
                int length = input.Length;
                byte[] array = new byte[length * 4];
                for (int i = 0; i < length; i++)
                {
                    ReverseBytes(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
                }
                SetMem(address, array, CurrentAPI);
            }
            private void SetMem(uint Address, byte[] buffer, SelectAPI API)
            {
                if (API == SelectAPI.ControlConsole)
                    Common.CcApi.SetMemory(Address, buffer);
                else if (API == SelectAPI.TargetManager)
                    Common.TmApi.SetMemory(Address, buffer);
            }

            private void GetMem(uint offset, byte[] buffer, SelectAPI API)
            {
                if (API == SelectAPI.ControlConsole)
                    Common.CcApi.GetMemory(offset, buffer);
                else if (API == SelectAPI.TargetManager)
                    Common.TmApi.GetMemory(offset, buffer);
            }

            private byte[] GetBytes(uint offset, uint length, SelectAPI API)
            {
                byte[] buffer = new byte[length];
                if (API == SelectAPI.ControlConsole)
                    buffer = Common.CcApi.GetBytes(offset, length);
                else if (API == SelectAPI.TargetManager)
                    buffer = Common.TmApi.GetBytes(offset, length);
                return buffer;
            }

            private class Common
            {
                public static CCAPI CcApi;
                public static TMAPI TmApi;
            }

        }
        public class CCAPI
        {
            [DllImport("kernel32.dll")]
            static extern IntPtr LoadLibrary(string dllName);

            [DllImport("kernel32.dll")]
            static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int connectConsoleDelegate(string targetIP);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int disconnectConsoleDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getConnectionStatusDelegate(ref int status);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getConsoleInfoDelegate(int index, IntPtr ptrN, IntPtr ptrI);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getDllVersionDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getFirmwareInfoDelegate(ref int firmware, ref int ccapi, ref int consoleType);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getNumberOfConsolesDelegate();
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getProcessListDelegate(ref uint numberProcesses, IntPtr processIdPtr);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getProcessMemoryDelegate(uint processID, ulong offset, uint size, byte[] buffOut);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getProcessNameDelegate(uint processID, IntPtr strPtr);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int getTemperatureDelegate(ref int cell, ref int rsx);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int notifyDelegate(int mode, string msgWChar);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int ringBuzzerDelegate(int type);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int setBootConsoleIdsDelegate(int idType, int on, byte[] ID);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int setConsoleIdsDelegate(int idType, byte[] consoleID);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int setConsoleLedDelegate(int color, int status);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int setProcessMemoryDelegate(uint processID, ulong offset, uint size, byte[] buffIn);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int shutdownDelegate(int mode);

            private connectConsoleDelegate connectConsole;
            private disconnectConsoleDelegate disconnectConsole;
            private getConnectionStatusDelegate getConnectionStatus;
            private getConsoleInfoDelegate getConsoleInfo;
            private getDllVersionDelegate getDllVersion;
            private getFirmwareInfoDelegate getFirmwareInfo;
            private getNumberOfConsolesDelegate getNumberOfConsoles;
            private getProcessListDelegate getProcessList;
            private getProcessMemoryDelegate getProcessMemory;
            private getProcessNameDelegate getProcessName;
            private getTemperatureDelegate getTemperature;
            private notifyDelegate notify;
            private ringBuzzerDelegate ringBuzzer;
            private setBootConsoleIdsDelegate setBootConsoleIds;
            private setConsoleIdsDelegate setConsoleIds;
            private setConsoleLedDelegate setConsoleLed;
            private setProcessMemoryDelegate setProcessMemory;
            private shutdownDelegate shutdown;

            private IntPtr libModule = IntPtr.Zero;
            private readonly string CCAPIHASH = "C2FE9E1C387CF29AAC781482C28ECF86";

            public CCAPI()
            {
                RegistryKey Key = Registry
                    .CurrentUser
                    .OpenSubKey(@"Software\FrenchModdingTeam\CCAPI\InstallFolder");

                if (Key != null)
                {
                    string Path = Key.GetValue("path") as String;
                    if (!string.IsNullOrEmpty(Path))
                    {
                        string DllUrl = Path + @"\CCAPI.dll";
                        if (File.Exists(DllUrl))
                        {
                            if (BitConverter.ToString(MD5.Create()
                                .ComputeHash(File.ReadAllBytes(DllUrl)))
                                .Replace("-", "").Equals(CCAPIHASH))
                            {
                                if (libModule == IntPtr.Zero)
                                    libModule = LoadLibrary(DllUrl);

                                if (libModule != IntPtr.Zero)
                                {
                                    connectConsole = (connectConsoleDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIConnectConsole"), typeof(connectConsoleDelegate));
                                    disconnectConsole = (disconnectConsoleDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIDisconnectConsole"), typeof(disconnectConsoleDelegate));
                                    getConnectionStatus = (getConnectionStatusDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetConnectionStatus"), typeof(getConnectionStatusDelegate));
                                    getConsoleInfo = (getConsoleInfoDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetConsoleInfo"), typeof(getConsoleInfoDelegate));
                                    getDllVersion = (getDllVersionDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetDllVersion"), typeof(getDllVersionDelegate));
                                    getFirmwareInfo = (getFirmwareInfoDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetFirmwareInfo"), typeof(getFirmwareInfoDelegate));
                                    getNumberOfConsoles = (getNumberOfConsolesDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetNumberOfConsoles"), typeof(getNumberOfConsolesDelegate));
                                    getProcessList = (getProcessListDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetProcessList"), typeof(getProcessListDelegate));

                                    getProcessMemory = (getProcessMemoryDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetMemory"), typeof(getProcessMemoryDelegate));
                                    getProcessName = (getProcessNameDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetProcessName"), typeof(getProcessNameDelegate));
                                    getTemperature = (getTemperatureDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIGetTemperature"), typeof(getTemperatureDelegate));
                                    notify = (notifyDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIVshNotify"), typeof(notifyDelegate));
                                    ringBuzzer = (ringBuzzerDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIRingBuzzer"), typeof(ringBuzzerDelegate));
                                    setBootConsoleIds = (setBootConsoleIdsDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPISetBootConsoleIds"), typeof(setBootConsoleIdsDelegate));
                                    setConsoleIds = (setConsoleIdsDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPISetConsoleIds"), typeof(setConsoleIdsDelegate));
                                    setConsoleLed = (setConsoleLedDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPISetConsoleLed"), typeof(setConsoleLedDelegate));

                                    setProcessMemory = (setProcessMemoryDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPISetMemory"), typeof(setProcessMemoryDelegate));
                                    shutdown = (shutdownDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(libModule, "CCAPIShutdown"), typeof(shutdownDelegate));
                                }
                                else
                                {
                                    MessageBox.Show("Impossible to load CCAPI.dll version 2.60.", "CCAPI.dll cannot be load", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("You're not using the right CCAPI.dll please install the version 2.60.", "CCAPI.dll version incorrect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("You need to install CCAPI to use this library.", "CCAPI.dll not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid CCAPI folder, please re-install it.", "CCAPI not installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("You need to install CCAPI to use this library.", "CCAPI not installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            public enum IdType
            {
                IDPS,
                PSID
            }

            public enum NotifyIcon
            {
                INFO,
                CAUTION,
                FRIEND,
                SLIDER,
                WRONGWAY,
                DIALOG,
                DIALOGSHADOW,
                TEXT,
                POINTER,
                GRAB,
                HAND,
                PEN,
                FINGER,
                ARROW,
                ARROWRIGHT,
                PROGRESS,
                TROPHY1,
                TROPHY2,
                TROPHY3,
                TROPHY4
            }

            public enum ConsoleType
            {
                CEX = 1,
                DEX = 2,
                TOOL = 3
            }

            public enum ProcessType
            {
                VSH,
                SYS_AGENT,
                CURRENTGAME
            }

            public enum RebootFlags
            {
                ShutDown = 1,
                SoftReboot = 2,
                HardReboot = 3
            }

            public enum BuzzerMode
            {
                Continuous,
                Single,
                Double
            }

            public enum LedColor
            {
                Green = 1,
                Red = 2
            }

            public enum LedMode
            {
                Off,
                On,
                Blink
            }

            private TargetInfo pInfo = new TargetInfo();

            private IntPtr ReadDataFromUnBufPtr<T>(IntPtr unBuf, ref T storage)
            {
                storage = (T)Marshal.PtrToStructure(unBuf, typeof(T));
                return new IntPtr(unBuf.ToInt64() + Marshal.SizeOf((T)storage));
            }

            private class System
            {
                public static int
                    connectionID = -1;
                public static uint
                    processID = 0;
                public static uint[]
                    processIDs;
            }

            /// <summary>Get informations from your target.</summary>
            public class TargetInfo
            {
                public int
                    Firmware = 0,
                    CCAPI = 0,
                    ConsoleType = 0,
                    TempCell = 0,
                    TempRSX = 0;
                public ulong
                    SysTable = 0;
            }

            /// <summary>Get Info for targets.</summary>
            public class ConsoleInfo
            {
                public string
                    Name,
                    Ip;
            }

            public Extension Extension
            {
                get { return new Extension(SelectAPI.ControlConsole); }
            }

            private void CompleteInfo(ref TargetInfo Info, int fw, int ccapi, ulong sysTable, int consoleType, int tempCELL, int tempRSX)
            {
                Info.Firmware = fw;
                Info.CCAPI = ccapi;
                Info.SysTable = sysTable;
                Info.ConsoleType = consoleType;
                Info.TempCell = tempCELL;
                Info.TempRSX = tempRSX;
            }

            /// <summary>Return true if a ccapi function return a good integer.</summary>
            public bool SUCCESS(int Void)
            {
                if (Void == 0)
                    return true;
                else return false;
            }

            /// <summary>Connect your console by console list.</summary>
            public bool ConnectTarget()
            {
                return new PS3API.ConsoleList(new PS3API(SelectAPI.ControlConsole)).Show();
            }

            /// <summary>Connect your console by ip address.</summary>
            public int ConnectTarget(string targetIP)
            {
                int code = connectConsole(targetIP);
                return code;
            }
           
            /// <summary>Get the status of the console.</summary>
          
            public int GetConnectionStatus()
            {
                int status = 0;
                getConnectionStatus(ref status);
                return status;
            }

            /// <summary>Disconnect your console.</summary>
            public int DisconnectTarget()
            {
                return disconnectConsole();
            }

            /// <summary>Attach the default process (Current Game).</summary>
            public int AttachProcess()
            {
                int result = -1; System.processID = 0;
                result = GetProcessList(out System.processIDs);
                if (SUCCESS(result) && System.processIDs.Length > 0)
                {
                    for (int i = 0; i < System.processIDs.Length; i++)
                    {
                        string name = String.Empty;
                        result = GetProcessName(System.processIDs[i], out name);
                        if (!SUCCESS(result))
                            break;
                        if (!name.Contains("flash"))
                        {
                            System.processID = System.processIDs[i];
                            break;
                        }
                        else result = -1;
                    }
                    if (System.processID == 0)
                        System.processID = System.processIDs[System.processIDs.Length - 1];
                }
                else result = -1;
                return result;
            }

            /// <summary>Attach your desired process.</summary>
            public int AttachProcess(ProcessType procType)
            {
                int result = -1; System.processID = 0;
                result = GetProcessList(out System.processIDs);
                if (result >= 0 && System.processIDs.Length > 0)
                {
                    for (int i = 0; i < System.processIDs.Length; i++)
                    {
                        string name = String.Empty;
                        result = GetProcessName(System.processIDs[i], out name);
                        if (result < 0)
                            break;
                        if (procType == ProcessType.VSH && name.Contains("vsh"))
                        {
                            System.processID = System.processIDs[i]; break;
                        }
                        else if (procType == ProcessType.SYS_AGENT && name.Contains("agent"))
                        {
                            System.processID = System.processIDs[i]; break;
                        }
                        else if (procType == ProcessType.CURRENTGAME && !name.Contains("flash"))
                        {
                            System.processID = System.processIDs[i]; break;
                        }
                    }
                    if (System.processID == 0)
                        System.processID = System.processIDs[System.processIDs.Length - 1];
                }
                else result = -1;
                return result;
            }

            /// <summary>Attach your desired process.</summary>
            public int AttachProcess(uint process)
            {
                int result = -1;
                uint[] procs = new uint[64];
                result = GetProcessList(out procs);
                if (SUCCESS(result))
                {
                    for (int i = 0; i < procs.Length; i++)
                    {
                        if (procs[i] == process)
                        {
                            result = 0;
                            System.processID = process;
                            break;
                        }
                        else result = -1;
                    }
                }
                procs = null;
                return result;
            }

            /// <summary>Get a list of all processes available.</summary>
            public int GetProcessList(out uint[] processIds)
            {
                uint numOfProcs = 64; int result = -1;
                IntPtr ptr = Marshal.AllocHGlobal((int)(4 * 0x40));
                result = getProcessList(ref numOfProcs, ptr);
                processIds = new uint[numOfProcs];
                if (SUCCESS(result))
                {
                    IntPtr unBuf = ptr;
                    for (uint i = 0; i < numOfProcs; i++)
                        unBuf = ReadDataFromUnBufPtr<uint>(unBuf, ref processIds[i]);
                }
                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>Get the process name of your choice.</summary>
            public int GetProcessName(uint processId, out string name)
            {
                IntPtr ptr = Marshal.AllocHGlobal((int)(0x211)); int result = -1;
                result = getProcessName(processId, ptr);
                name = String.Empty;
                if (SUCCESS(result))
                    name = Marshal.PtrToStringAnsi(ptr);
                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>Return the current process attached. Use this function only if you called AttachProcess before.</summary>
            public uint GetAttachedProcess()
            {
                return System.processID;
            }

            /// <summary>Set memory to offset (uint).</summary>
            public int SetMemory(uint offset, byte[] buffer)
            {
                return setProcessMemory(System.processID, (ulong)offset, (uint)buffer.Length, buffer);
            }

            /// <summary>Set memory to offset (ulong).</summary>
            public int SetMemory(ulong offset, byte[] buffer)
            {
                return setProcessMemory(System.processID, offset, (uint)buffer.Length, buffer);
            }

            /// <summary>Set memory to offset (string hex).</summary>
            public int SetMemory(ulong offset, string hexadecimal, EndianType Type = EndianType.BigEndian)
            {
                byte[] Entry = StringToByteArray(hexadecimal);
                if (Type == EndianType.LittleEndian)
                    Array.Reverse(Entry);
                return setProcessMemory(System.processID, offset, (uint)Entry.Length, Entry);
            }

            /// <summary>Get memory from offset (uint).</summary>
            public int GetMemory(uint offset, byte[] buffer)
            {
                return getProcessMemory(System.processID, (ulong)offset, (uint)buffer.Length, buffer);
            }

            /// <summary>Get memory from offset (ulong).</summary>
            public int GetMemory(ulong offset, byte[] buffer)
            {
                return getProcessMemory(System.processID, offset, (uint)buffer.Length, buffer);
            }

            /// <summary>Like Get memory but this function return directly the buffer from the offset (uint).</summary>
            public byte[] GetBytes(uint offset, uint length)
            {
                byte[] buffer = new byte[length];
                GetMemory(offset, buffer);
                return buffer;
            }

            /// <summary>Like Get memory but this function return directly the buffer from the offset (ulong).</summary>
            public byte[] GetBytes(ulong offset, uint length)
            {
                byte[] buffer = new byte[length];
                GetMemory(offset, buffer);
                return buffer;
            }

            /// <summary>Display the notify message on your PS3.</summary>
            public int Notify(NotifyIcon icon, string message)
            {
                return notify((int)icon, message);
            }

            /// <summary>Display the notify message on your PS3.</summary>
            public int Notify(int icon, string message)
            {
                return notify(icon, message);
            }

            /// <summary>You can shutdown the console or just reboot her according the flag selected.</summary>
            public int ShutDown(RebootFlags flag)
            {
                return shutdown((int)flag);
            }

            /// <summary>Your console will emit a song.</summary>
            public int RingBuzzer(BuzzerMode flag)
            {
                return ringBuzzer((int)flag);
            }

            /// <summary>Change leds for your console.</summary>
            public int SetConsoleLed(LedColor color, LedMode mode)
            {
                return setConsoleLed((int)color, (int)mode);
            }

            private int GetTargetInfo()
            {
                int result = -1; int[] sysTemp = new int[2];
                int fw = 0, ccapi = 0, consoleType = 0; ulong sysTable = 0;
                result = getFirmwareInfo(ref fw, ref ccapi, ref consoleType);
                if (result >= 0)
                {
                    result = getTemperature(ref sysTemp[0], ref sysTemp[1]);
                    if (result >= 0)
                        CompleteInfo(ref pInfo, fw, ccapi, sysTable, consoleType, sysTemp[0], sysTemp[1]);
                }

                return result;
            }

            /// <summary>Get informations of your console and store them into TargetInfo class.</summary>
            public int GetTargetInfo(out TargetInfo Info)
            {
                Info = new TargetInfo();
                int result = -1; int[] sysTemp = new int[2];
                int fw = 0, ccapi = 0, consoleType = 0; ulong sysTable = 0;
                result = getFirmwareInfo(ref fw, ref ccapi, ref consoleType);
                if (result >= 0)
                {
                    result = getTemperature(ref sysTemp[0], ref sysTemp[1]);
                    if (result >= 0)
                    {
                        CompleteInfo(ref Info, fw, ccapi, sysTable, consoleType, sysTemp[0], sysTemp[1]);
                        CompleteInfo(ref pInfo, fw, ccapi, sysTable, consoleType, sysTemp[0], sysTemp[1]);
                    }
                }
                return result;
            }

            /// <summary>Return the current firmware of your console in string format.</summary>
            public string GetFirmwareVersion()
            {
                if (pInfo.Firmware == 0)
                    GetTargetInfo();

                string ver = pInfo.Firmware.ToString("X8");
                string char1 = ver.Substring(1, 1) + ".";
                string char2 = ver.Substring(3, 1);
                string char3 = ver.Substring(4, 1);
                return char1 + char2 + char3;
            }

            /// <summary>Return the current temperature of your system in string.</summary>
            public string GetTemperatureCELL()
            {
                if (pInfo.TempCell == 0)
                    GetTargetInfo(out pInfo);

                return pInfo.TempCell.ToString() + " C";
            }

            /// <summary>Return the current temperature of your system in string.</summary>
            public string GetTemperatureRSX()
            {
                if (pInfo.TempRSX == 0)
                    GetTargetInfo(out pInfo);
                return pInfo.TempRSX.ToString() + " C";
            }

            /// <summary>Return the type of your firmware in string format.</summary>
            public string GetFirmwareType()
            {
                if (pInfo.ConsoleType.ToString() == "")
                    GetTargetInfo(out pInfo);
                string type = String.Empty;
                if (pInfo.ConsoleType == (int)ConsoleType.CEX)
                    type = "CEX";
                else if (pInfo.ConsoleType == (int)ConsoleType.DEX)
                    type = "DEX";
                else if (pInfo.ConsoleType == (int)ConsoleType.TOOL)
                    type = "TOOL";
                return type;
            }

            /// <summary>Clear informations into the DLL (PS3Lib).</summary>
            public void ClearTargetInfo()
            {
                pInfo = new TargetInfo();
            }

            /// <summary>Set a new ConsoleID in real time. (string)</summary>
            public int SetConsoleID(string consoleID)
            {
                if (string.IsNullOrEmpty(consoleID))
                {
                    MessageBox.Show("Cannot send an empty value", "Empty or null console id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                string newCID = String.Empty;
                if (consoleID.Length >= 32)
                    newCID = consoleID.Substring(0, 32);
                return SetConsoleID(StringToByteArray(newCID));
            }

            /// <summary>Set a new ConsoleID in real time. (bytes)</summary>
            public int SetConsoleID(byte[] consoleID)
            {
                if (consoleID.Length <= 0)
                {
                    MessageBox.Show("Cannot send an empty value", "Empty or null console id", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                return setConsoleIds((int)IdType.IDPS, consoleID);
            }

            /// <summary>Set a new PSID in real time. (string)</summary>
            public int SetPSID(string PSID)
            {
                if (string.IsNullOrEmpty(PSID))
                {
                    MessageBox.Show("Cannot send an empty value", "Empty or null psid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                string PS_ID = String.Empty;
                if (PSID.Length >= 32)
                    PS_ID = PSID.Substring(0, 32);
                return SetPSID(StringToByteArray(PS_ID));
            }

            /// <summary>Set a new PSID in real time. (bytes)</summary>
            public int SetPSID(byte[] consoleID)
            {
                if (consoleID.Length <= 0)
                {
                    MessageBox.Show("Cannot send an empty value", "Empty or null psid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                return setConsoleIds((int)IdType.PSID, consoleID);
            }

            /// <summary>Set a console ID when the console is running. (string)</summary>
            public int SetBootConsoleID(string consoleID, IdType Type = IdType.IDPS)
            {
                string newCID = String.Empty;
                if (consoleID.Length >= 32)
                    newCID = consoleID.Substring(0, 32);
                return SetBootConsoleID(StringToByteArray(consoleID), Type);
            }

            /// <summary>Set a console ID when the console is running. (bytes)</summary>
            public int SetBootConsoleID(byte[] consoleID, IdType Type = IdType.IDPS)
            {
                return setBootConsoleIds((int)Type, 1, consoleID);
            }

            /// <summary>Reset a console ID when the console is running.</summary>
            public int ResetBootConsoleID(IdType Type = IdType.IDPS)
            {
                return setBootConsoleIds((int)Type, 0, null);
            }

            /// <summary>Return CCAPI Version.</summary>
            public int GetDllVersion()
            {
                return getDllVersion();
            }

            /// <summary>Return a list of informations for each console available.</summary>
            public List<ConsoleInfo> GetConsoleList()
            {
                List<ConsoleInfo> data = new List<ConsoleInfo>();
                int targetCount = getNumberOfConsoles();
                IntPtr name = Marshal.AllocHGlobal((int)(256)),
                           ip = Marshal.AllocHGlobal((int)(256));
                for (int i = 0; i < targetCount; i++)
                {
                    ConsoleInfo Info = new ConsoleInfo();
                    getConsoleInfo(i, name, ip);
                    Info.Name = Marshal.PtrToStringAnsi(name);
                    Info.Ip = Marshal.PtrToStringAnsi(ip);
                    data.Add(Info);
                }
                Marshal.FreeHGlobal(name);
                Marshal.FreeHGlobal(ip);
                return data;
            }

            internal static byte[] StringToByteArray(string hex)
            {
                try
                {
                    string replace = hex.Replace("0x", "");
                    string Stringz = replace.Insert(replace.Length - 1, "0");

                    int Odd = replace.Length;
                    bool Nombre;
                    if (Odd % 2 == 0)
                        Nombre = true;
                    else
                        Nombre = false;
                    if (Nombre == true)
                    {
                        return Enumerable.Range(0, replace.Length)
                        .Where(x => x % 2 == 0)
                        .Select(x => Convert.ToByte(replace.Substring(x, 2), 16))
                        .ToArray();
                    }
                    else
                    {
                        return Enumerable.Range(0, replace.Length)
                        .Where(x => x % 2 == 0)
                        .Select(x => Convert.ToByte(Stringz.Substring(x, 2), 16))
                        .ToArray();
                    }
                }
                catch
                {
                    MessageBox.Show("Incorrect value (empty)", "StringToByteArray Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new byte[1];
                }
            }
        }

        public enum EndianType
        {
            LittleEndian,
            BigEndian
        }

        public class ArrayBuilder
        {
            private byte[] buffer;
            private int size;

            public ArrayBuilder(byte[] BytesArray)
            {
                buffer = BytesArray;
                size = buffer.Length;
            }

            /// <summary>Enter into all functions "Reader".</summary>
            public ArrayReader Read
            {
                get { return new ArrayReader(buffer); }
            }

            /// <summary>Enter into all functions "Writer".</summary>
            public ArrayWriter Write
            {
                get { return new ArrayWriter(buffer); }
            }

            public class ArrayReader
            {
                private byte[] buffer;
                private int size;

                public ArrayReader(byte[] BytesArray)
                {
                    buffer = BytesArray;
                    size = buffer.Length;
                }

                sbyte GetSByte(int pos)
                {
                    return (sbyte)buffer[pos];
                }

                public byte GetByte(int pos)
                {
                    return buffer[pos];
                }

                public char GetChar(int pos)
                {
                    string s = buffer[pos].ToString();
                    char b = s[0];
                    return b;
                }

                public bool GetBool(int pos)
                {
                    return buffer[pos] != 0;
                }

                public short GetInt16(int pos, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = new byte[2];
                    for (int i = 0; i < 2; i++)
                        b[i] = buffer[pos + i];
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 2);
                    return BitConverter.ToInt16(b, 0);
                }

                public int GetInt32(int pos, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = new byte[4];
                    for (int i = 0; i < 4; i++)
                        b[i] = buffer[pos + i];
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 4);
                    return BitConverter.ToInt32(b, 0);
                }

                public long GetInt64(int pos, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = new byte[8];
                    for (int i = 0; i < 8; i++)
                        b[i] = buffer[pos + i];
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 8);
                    return BitConverter.ToInt64(b, 0);
                }

                public ushort GetUInt16(int pos, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = new byte[2];
                    for (int i = 0; i < 2; i++)
                        b[i] = buffer[pos + i];
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 2);
                    return BitConverter.ToUInt16(b, 0);
                }

                public uint GetUInt32(int pos, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = new byte[4];
                    for (int i = 0; i < 4; i++)
                        b[i] = buffer[pos + i];
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 4);
                    return BitConverter.ToUInt32(b, 0);
                }

                public ulong GetUInt64(int pos, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = new byte[8];
                    for (int i = 0; i < 8; i++)
                        b[i] = buffer[pos + i];
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 8);
                    return BitConverter.ToUInt64(b, 0);
                }

                public byte[] GetBytes(int pos, int length)
                {
                    byte[] b = new byte[length];
                    for (int i = 0; i < length; i++)
                        b[i] = buffer[pos + i];
                    return b;
                }

                public string GetString(int pos)
                {
                    int strlen = 0;
                    while (true)
                        if (buffer[pos + strlen] != (byte)0)
                            strlen++;
                        else break;
                    byte[] b = new byte[strlen];
                    for (int i = 0; i < strlen; i++)
                        b[i] = buffer[pos + i];
                    return Encoding.UTF8.GetString(b);
                }

                public float GetFloat(int pos)
                {
                    byte[] b = new byte[4];
                    for (int i = 0; i < 4; i++)
                        b[i] = buffer[pos + i];
                    Array.Reverse(b, 0, 4);
                    return BitConverter.ToSingle(b, 0);
                }
            }

            public class ArrayWriter
            {
                private byte[] buffer;
                private int size;

                public ArrayWriter(byte[] BytesArray)
                {
                    buffer = BytesArray;
                    size = buffer.Length;
                }

                public void SetSByte(int pos, sbyte value)
                {
                    buffer[0 + pos] = (byte)value;
                }

                public void SetByte(int pos, byte value)
                {
                    buffer[0 + pos] = value;
                }

                public void SetChar(int pos, char value)
                {
                    byte[] b = Encoding.UTF8.GetBytes(value.ToString());
                    buffer[0 + pos] = b[0];
                }

                public void SetBool(int pos, bool value)
                {
                    byte[] b = new byte[1];
                    b[0] = value ? (byte)1 : (byte)0;
                    buffer[pos] = b[0];
                }

                public void SetInt16(int pos, short value, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = BitConverter.GetBytes(value);
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 2);
                    for (int i = 0; i < 2; i++)
                        buffer[i + pos] = b[i];
                }

                public void SetInt32(int pos, int value, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = BitConverter.GetBytes(value);
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 4);
                    for (int i = 0; i < 4; i++)
                        buffer[i + pos] = b[i];
                }

                public void SetInt64(int pos, long value, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = BitConverter.GetBytes(value);
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 8);
                    for (int i = 0; i < 8; i++)
                        buffer[i + pos] = b[i];
                }

                public void SetUInt16(int pos, ushort value, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = BitConverter.GetBytes(value);
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 2);
                    for (int i = 0; i < 2; i++)
                        buffer[i + pos] = b[i];
                }

                public void SetUInt32(int pos, uint value, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = BitConverter.GetBytes(value);
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 4);
                    for (int i = 0; i < 4; i++)
                        buffer[i + pos] = b[i];
                }

                public void SetUInt64(int pos, ulong value, EndianType Type = EndianType.BigEndian)
                {
                    byte[] b = BitConverter.GetBytes(value);
                    if (Type == EndianType.BigEndian)
                        Array.Reverse(b, 0, 8);
                    for (int i = 0; i < 8; i++)
                        buffer[i + pos] = b[i];
                }

                public void SetBytes(int pos, byte[] value)
                {
                    int valueSize = value.Length;
                    for (int i = 0; i < valueSize; i++)
                        buffer[i + pos] = value[i];
                }

                public void SetString(int pos, string value)
                {
                    byte[] b = Encoding.UTF8.GetBytes(value + "\0");
                    for (int i = 0; i < b.Length; i++)
                        buffer[i + pos] = b[i];
                }

                public void SetFloat(int pos, float value)
                {
                    byte[] b = BitConverter.GetBytes(value);
                    Array.Reverse(b, 0, 4);
                    for (int i = 0; i < 4; i++)
                        buffer[i + pos] = b[i];
                }
            }
        }
    }


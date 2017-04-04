using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Game_Mode_By_BISOON
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        
        struct Locations
        {
           public static float[] NukeTown = new float[] { -1929.671f, 395.697f, -63.875f };
           public static float NukeWall = -1214.844f;
           public static float[] NukeTown_ = new float[] { 1956.776f, 424.83f, -60.625f };
           public static float NukeWall_ = 1041.589f;
            //
           public static float[] Carrier = new float[] { -2520.521f, -1140.367f, 25.75175f };
           public static float Carrier_Wall = -3409.228f;
           public static float[] Carrier_ = new float[] { -6042.989f, 1055.179f, 44.125f };
           public static float Carrier_Wall_ = -5593.807f;

        }
        
        float[] sp1_ = null,
            sp2_ = null;
        float? sp1Wall = null,
        sp2Wall = null;
        public void NewThread(Action task)
        {
            new Thread((ThreadStart)(() => task()))
            {
                Name = task.Method.Name
            }.Start();
        }

        byte ClientTeam(int client)
        {
            return PS3.API.Extension.ReadByte(0x178642F + 0x5808 * (uint)client);
        }

        List<byte> teams = new List<byte>();
        byte[] team = { 1, 2 };
        int PlayerCount()
        {
            teams.AddRange(team);
            int num = 0;
            for (int i = 0; i < 12; i++)
            {
                if (teams.Contains(ClientTeam(i)))
                    num++;
            }
            return num;
        }

        float[] GetLocation(int Client)
        {
            return PS3.API.Extension.ReadFloat(0x1780F50 + 0x5808 * (uint)Client, 3);
        }
        void Teleport(int Client, float[] pos)
        {
            PS3.API.Extension.WriteFloat(0x1780F50 + 0x5808 * (uint)Client, pos);
        }
        void LoadLocation()
        {
            bool check = false;
            PS3.API.InitTarget();
            {
                while (check == true)
                {
                    if (!InGame())
                    {
                        InLocation2(false);
                        InLocation(false);
                        check = false;
                    }
                }
               
            }
        }
        bool start = false;
        void InLocation(bool on)
        {
            start = on;
            PS3.API.InitTarget();
            if (InGame())
            {
                while (start == true)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (ClientTeam(i) == 1)
                        {
                            if (GetLocation(i)[0] >= sp1Wall)
                            {
                                Teleport(i, sp1_);
                            }
                        }
                    }
                }
            }
            else
            {
                InLocation2(false);
                InLocation(false);
            }
        }
        void InLocation2(bool on)
        {
            PS3.API.InitTarget();
            start = on;
            if (InGame())
            {
                while (start == true)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (ClientTeam(i) == 2)
                        {
                            if (GetLocation(i)[0] <= sp2Wall)
                            {
                                Teleport(i, sp2_);
                            }
                        }
                    }
                }
            }
            else
            {
                InLocation2(false);
                InLocation(false);
            }
        }
        string MapInfo()
        {
            return PS3.API.Extension.ReadString(0x00ED7A08);
        }
        string MapName()
        {
            switch (MapInfo())
            {
                case "":
                    return "Map is loading...";

                case "mp_carrier":
                    sp1_ = Locations.Carrier_;
                    sp2_ = Locations.Carrier;
                    sp1Wall = Locations.Carrier_Wall_;
                    sp2Wall = Locations.Carrier_Wall;
                    return "Carrier";

                case "mp_dockside":
                    return "Cargo";

                case "mp_nuketown_2020":
                    sp1_ = Locations.NukeTown;
                    sp2_ = Locations.NukeTown_;
                    sp1Wall = Locations.NukeWall;
                    sp2Wall = Locations.NukeWall_;
                    return "Nuketown 2025";

            }
            return "Map Is Not Supported";
        }

        void Delay(double sec)
        {
            DateTime dt = DateTime.Now.AddSeconds(1.15740740740741E-05).AddSeconds(sec);
            while (DateTime.Now < dt)
                Application.DoEvents();
        }
        /// <summary> Check if client in game </summary>
        bool InGame()
        {
            return PS3.API.Extension.ReadByte(0x1CB68E8) == 1;
        }
        /// <summary> Take All Weapons & Equipment + Give One Weapon </summary>
        void GiveWeapon(int Client)
        {//All Weapons May You need To apply more options * 
            PS3.API.SetMemory((uint)(0x1781170 + (0x5808 * Client)), new byte[0x255]);
            byte[] buffer = new byte[] { 0x00, 0x00, 0x00, 0x5A };
            PS3.API.SetMemory((uint)(0x17811e0 + (0x5808 * Client)), buffer);
            buffer = new byte[4];
            buffer[3] = 0x30;
            PS3.API.SetMemory((uint)(0x1781320 + (0x5808 * Client)), buffer);
            buffer = new byte[4];
            buffer[3] = 0x20;
            PS3.API.SetMemory((uint)(0x1781360 + (0x5808 * Client)), buffer);
        }
         /// <summary> Getting Ready </summary>
        void OnSpawn()
        {
            PS3.API.InitTarget();
            int count = 15;
            while (count >= 0)
            {
                RPC.iPrintlnBold(-1, "Setting Up The Game & | ^2 Sending Stuff  ^1  |  All Players Should Spawn Before  ^5 : " + count);
                Delay(1);
                count--;
            }
        }
        // Give Or Change The ScoreStreak .. Thanks to MangoKnife For His Function And Saving my time to build one :)
        public static class GiveKillstreak
        {//Thank To MangoKnife 
            public static void Give(Int32 Killstreak, Int32 Client, Byte[] Streak)
            {
                Byte[] On = new Byte[] { 1 };
                //1 - Enable Streak
                //0 - Take it off
                UInt32 Index = (UInt32)Client *  0x5808;
                //Give the streak to whatever Killstreak you want
                //1,2 or 3
                //Then Enable it, so it wont be blank, in case the client didnt choosed scorestreaks
                //So:
                if (Killstreak == 1)
                {
                    PS3.API.SetMemory(Enable_1 + Index, On);//Enable Streak
                    //Give Streak:
                    PS3.API.SetMemory(Killstreak_1 + Index, Streak);
                    PS3.API.SetMemory(Change_1 + Index, Streak);
                    //We will change 2 Offsets, otherwise it will not work.
                    //And then, just give the scorestreak:
                    PS3.API.SetMemory(Scorestreak_1 + Index, On);
                }
                if (Killstreak == 2)
                {
                    PS3.API.SetMemory(Enable_2 + Index, On);//Enable Streak
                    //Give Streak:
                    PS3.API.SetMemory(Killstreak_2 + Index, Streak);
                    PS3.API.SetMemory(Change_2 + Index, Streak);
                    //We will change 2 Offsets, otherwise it will not work.
                    //And then, just give the scorestreak:
                    PS3.API.SetMemory(Scorestreak_2 + Index, On);
                }
                if (Killstreak == 3)
                {
                    PS3.API.SetMemory(Enable_3 + Index, On);//Enable Streak
                    //Give Streak:
                    PS3.API.SetMemory(Killstreak_3 + Index, Streak);
                    PS3.API.SetMemory(Change_3 + Index, Streak);
                    //We will change 2 Offsets, otherwise it will not work.
                    //And then, just give the scorestreak:
                    PS3.API.SetMemory(Scorestreak_3 + Index, On);
                }
            }
            private static UInt32 G_Client = 0x1780F28;
            private static UInt32 Change_1 = G_Client + 0x283;
            private static UInt32 Change_2 = G_Client + 0x29F;
            private static UInt32 Change_3 = G_Client + 0x267;

            private static UInt32 Scorestreak_1 = G_Client + 0x433;
            private static UInt32 Scorestreak_2 = G_Client + 0x437;
            private static UInt32 Scorestreak_3 = G_Client + 0x42F;

            private static UInt32 Enable_1 = G_Client + 0x55F;
            private static UInt32 Enable_2 = G_Client + 0x557;
            private static UInt32 Enable_3 = G_Client + 0x553;

            private static UInt32 Killstreak_1 = G_Client + 0x56F;
            private static UInt32 Killstreak_2 = G_Client + 0x567;
            private static UInt32 Killstreak_3 = G_Client + 0x563;
            //Streaks:
            public class Streaks
            {
                public static class Nuketown
                {
                    public static Byte[]
                     UAV = new Byte[] { 0x80 },
                     RCXD = new Byte[] { 0x94 },
                     HunterKiller = new Byte[] { 0x8F },
                     CarePackage = new Byte[] { 0x7A },
                     CounterUAV = new Byte[] { 0x81 },
                     Guardian = new Byte[] { 0xA4 },
                     HellstormMissile = new Byte[] { 0x97 },
                     LightningStrike = new Byte[] { 0x92 },
                     SentryGun = new Byte[] { 0xA2 },
                     DeathMachine = new Byte[] { 0x58 },
                     WarMachine = new Byte[] { 0x53 },
                     DragonFire = new Byte[] { 0x9B },
                     AGR = new Byte[] { 0x7F },
                     StealthChopper = new Byte[] { 0x87 },
                     OrbitalVSAT = new Byte[] { 0x82 },
                     EscortDrone = new Byte[] { 0x88 },
                     EMPSystems = new Byte[] { 0x83 },
                     Warthog = new Byte[] { 0x9E },
                     Loadstar = new Byte[] { 0x99 },
                     VTOLWarship = new Byte[] { 0x89 },
                     K9Unit = new Byte[] { 0x6F },
                     Swarm = new Byte[] { 0x91 };
                }
                public static class OtherMaps
                {
                    //On other maps its -0x1 from the bytes on nuketown2025
                    public static Byte[]
                     UAV = new Byte[] { 0x80 - 0x1 },
                     RCXD = new Byte[] { 0x94 - 0x1 },
                     HunterKiller = new Byte[] { 0x8F - 0x1 },
                     CarePackage = new Byte[] { 0x7A - 0x1 },
                     CounterUAV = new Byte[] { 0x81 - 0x1 },
                     Guardian = new Byte[] { 0xA4 - 0x1 },
                     HellstormMissile = new Byte[] { 0x97 - 0x1 },
                     LightningStrike = new Byte[] { 0x92 - 0x1 },
                     SentryGun = new Byte[] { 0xA2 - 0x1 },
                     DeathMachine = new Byte[] { 0x58 - 0x1 },
                     WarMachine = new Byte[] { 0x53 - 0x1 },
                     DragonFire = new Byte[] { 0x9B - 0x1 },
                     AGR = new Byte[] { 0x7F - 0x1 },
                     StealthChopper = new Byte[] { 0x87 - 0x1 },
                     OrbitalVSAT = new Byte[] { 0x82 - 0x1 },
                     EscortDrone = new Byte[] { 0x88 - 0x1 },
                     EMPSystems = new Byte[] { 0x83 - 0x1 },
                     Warthog = new Byte[] { 0x9E - 0x1 },
                     Loadstar = new Byte[] { 0x99 - 0x1 },
                     VTOLWarship = new Byte[] { 0x89 - 0x1 },
                     K9Unit = new Byte[] { 0x6F - 0x1 },
                     Swarm = new Byte[] { 0x91 - 0x1 };
                }
            }
        }
        bool init = false;
        private void conBtn_Click(object sender, EventArgs e)
        {
            PS3.API.SetFormLang(PS3.Lang.Arabic);
            if (cexCh.Checked && init)
            {
                Process.Start(Application.ExecutablePath);
                Process.GetCurrentProcess().Kill();
            }
            if (dexCh.Checked)
                PS3.API.ChangeAPI(PS3.SelectAPI.TargetManager);
            else if (cexCh.Checked)
            {
                //PS3.API.SetFormLang(PS3.Lang.Null);
                PS3.API.ChangeAPI(PS3.SelectAPI.ControlConsole);
            }
            else
            {
                MessageBox.Show("Select your API before ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (PS3.API.ConnectTarget() && PS3.API.AttachProcess())
            {
                RPC.Init();
                MessageBox.Show("Connected & Attached ", "Connection", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                init = true;
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (InGame())
            {
                label1.Text = "Map : " + MapName();
                if (MapName() != "Map Is Not Supported")
                {
                    NewThread(new Action(LoadLocation));
                    RPC.PlayerLive(1);
                    RPC.Fast_Restart();
                    NewThread(new Action(() => InLocation(true)));
                    NewThread(new Action(() => InLocation2(true)));
                    OnSpawn();
                    for (int i = 0; i < 12; i++)
                    {
                        GiveWeapon(i);
                    }
                    Delay(5);
                    for (int i = 0; i < 12; i++)
                    {
                        GiveKillstreak.Give(2, i, GiveKillstreak.Streaks.Nuketown.WarMachine);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry\rMap Not Supported\rWorks Only For : Carrier And NukeTown !!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("You are not in a game !!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void flatButton1_Click(object sender, EventArgs e)
        {
            theme.FlatButton bt = (theme.FlatButton)sender;
            if (bt.Text == "By BISOON")
            {
                Process.Start("http://www.youtube.com/c/bisoon");
            }
            else
            {
                MessageBox.Show("How to start the game mode :\rStart a game mode that has two teams and make sure the map is supported then press start\n\rSupported Map :\r- NukeTown\r- Carrier","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}

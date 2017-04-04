using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public class RPC
{
    // Enable RPC By Init() <<
    private static UInt32 function_address = 0x7AA050;
    public static int Call(UInt32 func_address, params object[] parameters)
    {
        int length = parameters.Length;
        int inPS3 = 0;
        UInt32 num3 = 0;
        UInt32 num4 = 0;
        UInt32 num5 = 0;
        UInt32 num6 = 0;
        while (inPS3 < length)
        {
            if (parameters[inPS3] is int)
            {
                PS3.API.Extension.WriteInt32(0x10020000 + (num3 * 4), ((int)(parameters[inPS3])));
                num3++;
            }
            else if (parameters[inPS3] is UInt32)
            {
                PS3.API.Extension.WriteUInt32(0x10020000 + (num3 * 4), ((UInt32)(parameters[inPS3])));
                num3++;
            }
            else
            {
                UInt32 num7 = 0;
                if (parameters[inPS3] is string)
                {
                    num7 = 0x10022000 + (num4 * 0x400);
                    PS3.API.Extension.WriteString(num7, Convert.ToString(parameters[inPS3]));
                    PS3.API.Extension.WriteUInt32(0x10020000 + (num3 * 4), num7);
                    num3++;
                    num4++;
                }
                else if (parameters[inPS3] is Single)
                {
                    PS3.API.Extension.WriteFloat(0x10020024 + (num5 * 4), ((Single)(parameters[inPS3])));
                    num5++;
                }
                else if (parameters[inPS3] is Single[])
                {
                    float[] input = (Single[])(parameters[inPS3]);
                    num7 = 0x10021000 + (num6 * 4);
                    PS3.API.Extension.WriteSingle(num7, input);
                    PS3.API.Extension.WriteUInt32(0x10020000 + (num3 * 4), num7);
                    num3++;
                    num6 += (uint)input.Length;
                }
            }
            inPS3++;
        }
        PS3.API.Extension.WriteUInt32(0x1002004C, func_address);
        Thread.Sleep(20);
        return PS3.API.Extension.ReadInt32(0x10020050);

    }
    public static void Enable()
    {
        if (PS3.API.Extension.ReadByte(0x7AA050 + 3) != 0x91)
        {
            PS3.API.Extension.WriteBytes(function_address, new byte[] { 0x4E, 0x80, 0x0, 0x20 });
            Thread.Sleep(20);
            byte[] memory = new byte[] { (byte)(0x7C), (byte)(0x8), (byte)(0x2), (byte)(0xA6), (byte)(0xF8), (byte)(0x1), (byte)(0x0), (byte)(0x80), (byte)(0x3C), (byte)(0x60), (byte)(0x10), (byte)(0x2), (byte)(0x81), (byte)(0x83), (byte)(0x0), (byte)(0x4C), (byte)(0x2C), (byte)(0xC), (byte)(0x0), (byte)(0x0), (byte)(0x41), (byte)(0x82), (byte)(0x0), (byte)(0x64), (byte)(0x80), (byte)(0x83), (byte)(0x0), (byte)(0x4), (byte)(0x80), (byte)(0xA3), (byte)(0x0), (byte)(0x8), (byte)(0x80), (byte)(0xC3), (byte)(0x0), (byte)(0xC), (byte)(0x80), (byte)(0xE3), (byte)(0x0), (byte)(0x10), (byte)(0x81), (byte)(0x3), (byte)(0x0), (byte)(0x14), (byte)(0x81), (byte)(0x23), (byte)(0x0), (byte)(0x18), (byte)(0x81), (byte)(0x43), (byte)(0x0), (byte)(0x1C), (byte)(0x81), (byte)(0x63), (byte)(0x0), (byte)(0x20), (byte)(0xC0), (byte)(0x23), (byte)(0x0), (byte)(0x24), (byte)(0xC0), (byte)(0x43), (byte)(0x0), (byte)(0x28), (byte)(0xC0), (byte)(0x63), (byte)(0x0), (byte)(0x2C), (byte)(0xC0), (byte)(0x83), (byte)(0x0), (byte)(0x30), (byte)(0xC0), (byte)(0xA3), (byte)(0x0), (byte)(0x34), (byte)(0xC0), (byte)(0xC3), (byte)(0x0), (byte)(0x38), (byte)(0xC0), (byte)(0xE3), (byte)(0x0), (byte)(0x3C), (byte)(0xC1), (byte)(0x3), (byte)(0x0), (byte)(0x40), (byte)(0xC1), (byte)(0x23), (byte)(0x0), (byte)(0x48), (byte)(0x80), (byte)(0x63), (byte)(0x0), (byte)(0x0), (byte)(0x7D), (byte)(0x89), (byte)(0x3), (byte)(0xA6), (byte)(0x4E), (byte)(0x80), (byte)(0x4), (byte)(0x21), (byte)(0x3C), (byte)(0x80), (byte)(0x10), (byte)(0x2), (byte)(0x38), (byte)(0xA0), (byte)(0x0), (byte)(0x0), (byte)(0x90), (byte)(0xA4), (byte)(0x0), (byte)(0x4C), (byte)(0x90), (byte)(0x64), (byte)(0x0), (byte)(0x50), (byte)(0xE8), (byte)(0x1), (byte)(0x0), (byte)(0x80), (byte)(0x7C), (byte)(0x8), (byte)(0x3), (byte)(0xA6), (byte)(0x38), (byte)(0x21), (byte)(0x0), (byte)(0x70), (byte)(0x4E), (byte)(0x80), (byte)(0x0), (byte)(0x20) };
            PS3.API.Extension.WriteBytes(function_address + 4, memory);
            PS3.API.Extension.WriteBytes(0x10020000, new byte[10324]);
            PS3.API.Extension.WriteBytes(function_address, new byte[] { 0xF8, 0x21, 0xFF, 0x91 });
        }
    }

    public static int Init()
    {
        function_address = 0x7AA050;
        Enable();
        return 0;
    }
    public static void SV_GameSendServerCommand(int client, string command)
    {
        Call(0x349f6c, new object[] { client, 0, command });
    }
    public static void iPrintlnBold(int client, string txt)
    {
        SV_GameSendServerCommand(client, "< \"" + txt + "\"");
    }
    public static void CBuf_AddText(string command)
    {
        Call(0x313C18, 0, command);
    }
    public static void PlayerLive(int value)
    {
        CBuf_AddText("gametype_setting playerNumlives " + value);
    }
    public static void Fast_Restart()
    {
        CBuf_AddText("fast_restart");
    }
}


using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

class HidProbe {
    [DllImport("hid.dll")] static extern void HidD_GetHidGuid(out Guid guid);
    [DllImport("setupapi.dll", CharSet = CharSet.Auto)] static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, int flags);
    [DllImport("setupapi.dll")] static extern bool SetupDiEnumDeviceInterfaces(IntPtr devInfo, IntPtr devInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
    [DllImport("setupapi.dll", CharSet = CharSet.Auto)] static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr devInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr detailData, int detailDataSize, ref int requiredSize, IntPtr parent);
    [DllImport("setupapi.dll")] static extern bool SetupDiDestroyDeviceInfoList(IntPtr devInfo);
    [DllImport("hid.dll")] static extern bool HidD_GetAttributes(SafeFileHandle device, ref HIDD_ATTRIBUTES attrs);
    [DllImport("hid.dll")] static extern bool HidD_GetPreparsedData(SafeFileHandle device, out IntPtr preparsedData);
    [DllImport("hid.dll")] static extern bool HidD_FreePreparsedData(IntPtr preparsedData);
    [DllImport("hid.dll")] static extern int HidP_GetCaps(IntPtr preparsedData, out HIDP_CAPS caps);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] static extern SafeFileHandle CreateFile(string fileName, uint access, uint share, IntPtr sec, uint create, uint flags, IntPtr template);

    const uint GENERIC_READ = 0x80000000;
    const uint GENERIC_WRITE = 0x40000000;
    const uint FILE_SHARE_READ = 1;
    const uint FILE_SHARE_WRITE = 2;
    const uint OPEN_EXISTING = 3;
    const int DIGCF_PRESENT = 2;
    const int DIGCF_DEVICEINTERFACE = 16;

    struct SP_DEVICE_INTERFACE_DATA { public int cbSize; public Guid InterfaceClassGuid; public int Flags; public IntPtr Reserved; }
    struct HIDD_ATTRIBUTES { public int Size; public ushort VendorID; public ushort ProductID; public ushort VersionNumber; }
    struct HIDP_CAPS {
        public ushort Usage; public ushort UsagePage;
        public ushort InputReportByteLength; public ushort OutputReportByteLength; public ushort FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=17)] public ushort[] Reserved;
        public ushort NumberLinkCollectionNodes; public ushort NumberInputButtonCaps; public ushort NumberInputValueCaps;
        public ushort NumberInputDataIndices; public ushort NumberOutputButtonCaps; public ushort NumberOutputValueCaps;
        public ushort NumberOutputDataIndices; public ushort NumberFeatureButtonCaps; public ushort NumberFeatureValueCaps;
        public ushort NumberFeatureDataIndices;
    }

    static void Main() {
        Guid hidGuid;
        HidD_GetHidGuid(out hidGuid);
        IntPtr devInfo = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
        if (devInfo == IntPtr.Zero) { Console.WriteLine("SetupDiGetClassDevs failed"); return; }
        try {
            SP_DEVICE_INTERFACE_DATA ifData = new SP_DEVICE_INTERFACE_DATA { cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA)) };
            for (int i = 0; ; i++) {
                if (!SetupDiEnumDeviceInterfaces(devInfo, IntPtr.Zero, ref hidGuid, i, ref ifData)) break;
                int reqSize = 0;
                SetupDiGetDeviceInterfaceDetail(devInfo, ref ifData, IntPtr.Zero, 0, ref reqSize, IntPtr.Zero);
                if (reqSize <= 0) continue;
                IntPtr buf = Marshal.AllocHGlobal(reqSize);
                Marshal.WriteInt32(buf, IntPtr.Size == 8 ? 8 : (4 + Marshal.SystemDefaultCharSize));
                if (!SetupDiGetDeviceInterfaceDetail(devInfo, ref ifData, buf, reqSize, ref reqSize, IntPtr.Zero)) { Marshal.FreeHGlobal(buf); continue; }
                string path = Marshal.PtrToStringAuto(new IntPtr(buf.ToInt64() + 4));
                Marshal.FreeHGlobal(buf);
                if (string.IsNullOrEmpty(path)) { Console.WriteLine("path is null/empty for index " + i); continue; }
                Console.WriteLine("  Trying: " + path);
                SafeFileHandle h = CreateFile(path, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                if (h.IsInvalid) {
                    int err = Marshal.GetLastWin32Error();
                    Console.WriteLine("  OPEN FAILED (Win32=" + err + ") for: " + path);
                    continue;
                }
                HIDD_ATTRIBUTES attrs = new HIDD_ATTRIBUTES { Size = Marshal.SizeOf(typeof(HIDD_ATTRIBUTES)) };
                if (!HidD_GetAttributes(h, ref attrs)) {
                    Console.WriteLine(path + " -> GetAttr FAILED");
                    h.Close(); continue;
                }
                bool matchSony = (attrs.VendorID == 1356);
                bool matchDs = (attrs.ProductID == 3302 || attrs.ProductID == 3570);
                Console.WriteLine(path + " -> VID=" + attrs.VendorID + " PID=" + attrs.ProductID + " Sony=" + matchSony + " DS=" + matchDs);
                IntPtr preparsed;
                if (HidD_GetPreparsedData(h, out preparsed)) {
                    HIDP_CAPS caps;
                    if (HidP_GetCaps(preparsed, out caps) == 1114112) {
                        Console.WriteLine("   OutputReportByteLength=" + caps.OutputReportByteLength);
                    } else {
                        Console.WriteLine("   HidP_GetCaps FAILED");
                    }
                    HidD_FreePreparsedData(preparsed);
                }
                h.Close();
            }
        } finally { SetupDiDestroyDeviceInfoList(devInfo); }
    }
}

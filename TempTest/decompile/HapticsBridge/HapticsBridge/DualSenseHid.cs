using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace HapticsBridge;

internal static class DualSenseHid
{
	private struct SP_DEVICE_INTERFACE_DATA
	{
		public int cbSize;

		public Guid InterfaceClassGuid;

		public int Flags;

		public IntPtr Reserved;
	}

	private struct HIDD_ATTRIBUTES
	{
		public int Size;

		public ushort VendorID;

		public ushort ProductID;

		public ushort VersionNumber;
	}

	private struct HIDP_CAPS
	{
		public ushort Usage;

		public ushort UsagePage;

		public ushort InputReportByteLength;

		public ushort OutputReportByteLength;

		public ushort FeatureReportByteLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
		public ushort[] Reserved;

		public ushort NumberLinkCollectionNodes;

		public ushort NumberInputButtonCaps;

		public ushort NumberInputValueCaps;

		public ushort NumberInputDataIndices;

		public ushort NumberOutputButtonCaps;

		public ushort NumberOutputValueCaps;

		public ushort NumberOutputDataIndices;

		public ushort NumberFeatureButtonCaps;

		public ushort NumberFeatureValueCaps;

		public ushort NumberFeatureDataIndices;
	}

	private const ushort SonyVid = 1356;

	private const ushort DualSensePid = 3302;

	private const ushort DualSenseEdgePid = 3570;

	private static readonly object sync = new object();

	private static List<string> cachedPaths;

	private static readonly HashSet<string> badPaths = new HashSet<string>();

	private static bool loggedAsserted;

	private static bool loggedNone;

	private static bool loggedTimeout;

	private const uint GENERIC_READ = 2147483648u;

	private const uint GENERIC_WRITE = 1073741824u;

	private const uint FILE_SHARE_READ = 1u;

	private const uint FILE_SHARE_WRITE = 2u;

	private const uint OPEN_EXISTING = 3u;

	private const int DIGCF_PRESENT = 2;

	private const int DIGCF_DEVICEINTERFACE = 16;

	private const int HIDP_STATUS_SUCCESS = 1114112;

	private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

	public static void AssertAudioHaptics()
	{
		lock (sync)
		{
			int num = TryWriteAll();
			if (num == 0)
			{
				cachedPaths = null;
				num = TryWriteAll();
			}
			if (num > 0 && !loggedAsserted)
			{
				loggedAsserted = true;
				loggedNone = false;
				Engine.Log(string.Format("HID: audio haptics enabled ({0} USB controller{1})", num, (num > 1) ? "s" : ""));
			}
			else if (num == 0 && !loggedNone)
			{
				loggedNone = true;
				loggedAsserted = false;
				Engine.Log("HID: no USB DualSense found to enable (fine on Bluetooth or when DSX manages the pad)");
			}
		}
	}

	private static int TryWriteAll()
	{
		try
		{
			if (cachedPaths == null)
			{
				cachedPaths = FindDualSensePaths();
				badPaths.Clear();
			}
			int num = 0;
			foreach (string cachedPath in cachedPaths)
			{
				if (TryWriteModeSelect(cachedPath))
				{
					num++;
				}
			}
			return num;
		}
		catch
		{
			return 0;
		}
	}

	private static bool TryWriteModeSelect(string path)
	{
		if (badPaths.Contains(path))
		{
			return false;
		}
		bool ok = false;
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				ok = WriteModeSelect(path);
			}
			catch
			{
			}
		});
		thread.IsBackground = true;
		thread.Name = "HidWrite";
		thread.Start();
		if (!thread.Join(300))
		{
			badPaths.Add(path);
			if (!loggedTimeout)
			{
				loggedTimeout = true;
				Engine.Log("HID: a controller's write stalled (Bluetooth pad?); skipping it");
			}
			return false;
		}
		return ok;
	}

	private static bool WriteModeSelect(string path)
	{
		using SafeFileHandle safeFileHandle = CreateFile(path, 3221225472u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
		if (safeFileHandle.IsInvalid)
		{
			return false;
		}
		if (!HidD_GetPreparsedData(safeFileHandle, out var preparsedData))
		{
			return false;
		}
		HIDP_CAPS capabilities;
		try
		{
			if (HidP_GetCaps(preparsedData, out capabilities) != 1114112)
			{
				return false;
			}
		}
		finally
		{
			HidD_FreePreparsedData(preparsedData);
		}
		int outputReportByteLength = capabilities.OutputReportByteLength;
		if (outputReportByteLength < 48 || outputReportByteLength > 64)
		{
			return false;
		}
		byte[] array = new byte[outputReportByteLength];
		array[0] = 2;
		array[1] = 176;
		array[2] = 128;
		array[3] = 0;
		array[4] = 0;
		array[5] = 127;
		array[6] = byte.MaxValue;
		array[7] = 0;
		array[8] = 0;
		try
		{
			using (FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write, outputReportByteLength, isAsync: false))
			{
				fileStream.Write(array, 0, outputReportByteLength);
				fileStream.Flush();
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static List<string> FindDualSensePaths()
	{
		List<string> list = new List<string>();
		HidD_GetHidGuid(out var hidGuid);
		IntPtr intPtr = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, 18);
		if (intPtr == INVALID_HANDLE_VALUE)
		{
			return list;
		}
		try
		{
			SP_DEVICE_INTERFACE_DATA ifData = new SP_DEVICE_INTERFACE_DATA
			{
				cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA))
			};
			for (int i = 0; SetupDiEnumDeviceInterfaces(intPtr, IntPtr.Zero, ref hidGuid, i, ref ifData); i++)
			{
				string interfacePath = GetInterfacePath(intPtr, ref ifData);
				if (interfacePath == null)
				{
					continue;
				}
				SafeFileHandle safeFileHandle = CreateFile(interfacePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0u, IntPtr.Zero);
				if (!safeFileHandle.IsInvalid)
				{
					HIDD_ATTRIBUTES attributes = new HIDD_ATTRIBUTES
					{
						Size = Marshal.SizeOf(typeof(HIDD_ATTRIBUTES))
					};
					if (HidD_GetAttributes(safeFileHandle, ref attributes) && attributes.VendorID == 1356 && (attributes.ProductID == 3302 || attributes.ProductID == 3570))
					{
						list.Add(interfacePath);
					}
				}
			}
			return list;
		}
		finally
		{
			SetupDiDestroyDeviceInfoList(intPtr);
		}
	}

	private static string GetInterfacePath(IntPtr devInfo, ref SP_DEVICE_INTERFACE_DATA ifData)
	{
		int requiredSize = 0;
		SetupDiGetDeviceInterfaceDetail(devInfo, ref ifData, IntPtr.Zero, 0, ref requiredSize, IntPtr.Zero);
		if (requiredSize <= 0)
		{
			return null;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(requiredSize);
		try
		{
			Marshal.WriteInt32(intPtr, (IntPtr.Size == 8) ? 8 : (4 + Marshal.SystemDefaultCharSize));
			if (!SetupDiGetDeviceInterfaceDetail(devInfo, ref ifData, intPtr, requiredSize, ref requiredSize, IntPtr.Zero))
			{
				return null;
			}
			return Marshal.PtrToStringAuto(new IntPtr(intPtr.ToInt64() + 4));
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[DllImport("hid.dll")]
	private static extern void HidD_GetHidGuid(out Guid hidGuid);

	[DllImport("hid.dll")]
	private static extern bool HidD_GetAttributes(SafeFileHandle hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

	[DllImport("hid.dll")]
	private static extern bool HidD_GetPreparsedData(SafeFileHandle hidDeviceObject, out IntPtr preparsedData);

	[DllImport("hid.dll")]
	private static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

	[DllImport("hid.dll")]
	private static extern int HidP_GetCaps(IntPtr preparsedData, out HIDP_CAPS capabilities);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, int flags);

	[DllImport("setupapi.dll")]
	private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

	[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
	private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

	[DllImport("setupapi.dll")]
	private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);
}

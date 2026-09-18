using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using InControl;
using JetBrains.Annotations;
using Language;
using Microsoft.CodeAnalysis;
using Modding;
using Modding.Delegates;
using Modding.Menu;
using Modding.Menu.Components;
using Modding.Menu.Config;
using Modding.Patches;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using On;
using On.HutongGames.PlayMaker.Actions;
using On.UnityEngine.UI;
using Satchel.BetterMenus.Config;
using Satchel.Futils;
using Satchel.Futils.Extractors;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("Satchel")]
[assembly: AssemblyDescription("Satchel - Hollow Knight modding utilities")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Satchel")]
[assembly: AssemblyCopyright("Copyright ©  2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyFileVersion("0.9.4.0")]
[assembly: AssemblyInformationalVersion("0.9.4.0汉化")]
[assembly: ComVisible(false)]
[assembly: Guid("ec832acd-2374-45b5-ad18-376cef2ac643")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyVersion("0.9.4.0")]
[module: RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		public readonly byte[] NullableFlags;

		public NullableAttribute(byte P_0)
		{
			NullableFlags = new byte[1] { P_0 };
		}

		public NullableAttribute(byte[] P_0)
		{
			NullableFlags = P_0;
		}
	}
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute(int P_0)
		{
			Version = P_0;
		}
	}
}
namespace Satchel
{
	public class Animation
	{
		public string[] frames;

		public float fps;

		public bool loop;
	}
	public static class CustomAnimation
	{
		public static Dictionary<Animation, Sprite[]> loadedSprites = new Dictionary<Animation, Sprite[]>();

		public static Animation LoadAnimation(string animationJsonPath)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			string directoryName = Path.GetDirectoryName(animationJsonPath);
			Animation animation = JsonConvert.DeserializeObject<Animation>(File.ReadAllText(animationJsonPath), new JsonSerializerSettings
			{
				ObjectCreationHandling = (ObjectCreationHandling)2
			});
			loadedSprites[animation] = (Sprite[])(object)new Sprite[animation.frames.Length];
			for (int i = 0; i < animation.frames.Length; i++)
			{
				Texture2D val = TextureUtils.LoadTextureFromFile(Path.Combine(directoryName, animation.frames[i]));
				loadedSprites[animation][i] = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 64f, 0u, (SpriteMeshType)0);
			}
			return animation;
		}

		public static Animation LoadAnimation(Animation anim, Sprite[] sprites)
		{
			loadedSprites[anim] = sprites;
			return anim;
		}
	}
	[Obsolete("Use the nuget package WavLib by SFGrenade", false)]
	public class WavUtils
	{
		private const int BlockSize_16Bit = 2;

		public static AudioClip ToAudioClip(string filePath)
		{
			if (!filePath.StartsWith(Application.persistentDataPath) && !filePath.StartsWith(Application.dataPath))
			{
				Debug.LogWarning((object)"This only supports files that are stored using Unity's Application data path. \nTo load bundled resources use 'Resources.Load(\"filename\") typeof(AudioClip)' method. \nhttps://docs.unity3d.com/ScriptReference/Resources.Load.html");
				return null;
			}
			return ToAudioClip(File.ReadAllBytes(filePath));
		}

		public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "wav")
		{
			int num = BitConverter.ToInt32(fileBytes, 16);
			FormatCode(BitConverter.ToUInt16(fileBytes, 20));
			ushort num2 = BitConverter.ToUInt16(fileBytes, 22);
			int num3 = BitConverter.ToInt32(fileBytes, 24);
			ushort num4 = BitConverter.ToUInt16(fileBytes, 34);
			int num5 = 20 + num + 4;
			int dataSize = BitConverter.ToInt32(fileBytes, num5);
			float[] array = num4 switch
			{
				8 => Convert8BitByteArrayToAudioClipData(fileBytes, num5, dataSize), 
				16 => Convert16BitByteArrayToAudioClipData(fileBytes, num5, dataSize), 
				24 => Convert24BitByteArrayToAudioClipData(fileBytes, num5, dataSize), 
				32 => Convert32BitByteArrayToAudioClipData(fileBytes, num5, dataSize), 
				_ => throw new Exception(num4 + " bit depth is not supported."), 
			};
			AudioClip obj = AudioClip.Create(name, array.Length / num2, (int)num2, num3, false);
			obj.SetData(array, 0);
			return obj;
		}

		private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			int num = BitConverter.ToInt32(source, headerOffset);
			headerOffset += 4;
			float[] array = new float[num];
			float num2 = 127.5f;
			for (int i = 0; i < num; i++)
			{
				array[i] = Mathf.Clamp((float)(int)source[i] / num2 - 1f, -1f, 1f);
			}
			return array;
		}

		private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			int num = BitConverter.ToInt32(source, headerOffset);
			headerOffset += 4;
			int num2 = 2;
			int num3 = num / num2;
			float[] array = new float[num3];
			short num4 = short.MaxValue;
			int num5 = 0;
			for (int i = 0; i < num3; i++)
			{
				num5 = i * num2 + headerOffset;
				array[i] = (float)BitConverter.ToInt16(source, num5) / (float)num4;
			}
			return array;
		}

		private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			int num = BitConverter.ToInt32(source, headerOffset);
			headerOffset += 4;
			int num2 = 3;
			int num3 = num / num2;
			int num4 = int.MaxValue;
			float[] array = new float[num3];
			byte[] array2 = new byte[4];
			int num5 = 0;
			for (int i = 0; i < num3; i++)
			{
				num5 = i * num2 + headerOffset;
				Buffer.BlockCopy(source, num5, array2, 1, num2);
				array[i] = (float)BitConverter.ToInt32(array2, 0) / (float)num4;
			}
			return array;
		}

		private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
		{
			int num = BitConverter.ToInt32(source, headerOffset);
			headerOffset += 4;
			int num2 = 4;
			int num3 = num / num2;
			int num4 = int.MaxValue;
			float[] array = new float[num3];
			int num5 = 0;
			for (int i = 0; i < num3; i++)
			{
				num5 = i * num2 + headerOffset;
				array[i] = (float)BitConverter.ToInt32(source, num5) / (float)num4;
			}
			return array;
		}

		public static byte[] FromAudioClip(AudioClip audioClip)
		{
			string filepath;
			return FromAudioClip(audioClip, out filepath, saveAsFile: false);
		}

		public static byte[] FromAudioClip(AudioClip audioClip, out string filepath, bool saveAsFile = true, string dirname = "recordings")
		{
			MemoryStream stream = new MemoryStream();
			ushort bitDepth = 16;
			int fileSize = audioClip.samples * 2 * audioClip.channels + 44;
			WriteFileHeader(ref stream, fileSize);
			WriteFileFormat(ref stream, audioClip.channels, audioClip.frequency, bitDepth);
			WriteFileData(ref stream, audioClip, bitDepth);
			byte[] array = stream.ToArray();
			if (saveAsFile)
			{
				filepath = string.Format("{0}/{1}/{2}.{3}", Application.persistentDataPath, dirname, DateTime.UtcNow.ToString("yyMMdd-HHmmss-fff"), "wav");
				Directory.CreateDirectory(Path.GetDirectoryName(filepath));
				File.WriteAllBytes(filepath, array);
			}
			else
			{
				filepath = null;
			}
			stream.Dispose();
			return array;
		}

		private static int WriteFileHeader(ref MemoryStream stream, int fileSize)
		{
			byte[] bytes = Encoding.ASCII.GetBytes("RIFF");
			int num = 0 + WriteBytesToMemoryStream(ref stream, bytes, "ID");
			int value = fileSize - 8;
			int num2 = num + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value), "CHUNK_SIZE");
			byte[] bytes2 = Encoding.ASCII.GetBytes("WAVE");
			return num2 + WriteBytesToMemoryStream(ref stream, bytes2, "FORMAT");
		}

		private static int WriteFileFormat(ref MemoryStream stream, int channels, int sampleRate, ushort bitDepth)
		{
			byte[] bytes = Encoding.ASCII.GetBytes("fmt ");
			int num = 0 + WriteBytesToMemoryStream(ref stream, bytes, "FMT_ID");
			int value = 16;
			int num2 = num + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value), "SUBCHUNK_SIZE");
			ushort value2 = 1;
			int num3 = num2 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value2), "AUDIO_FORMAT");
			ushort value3 = Convert.ToUInt16(channels);
			int num4 = num3 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value3), "CHANNELS") + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(sampleRate), "SAMPLE_RATE");
			int value4 = sampleRate * channels * BytesPerSample(bitDepth);
			int num5 = num4 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value4), "BYTE_RATE");
			ushort value5 = Convert.ToUInt16(channels * BytesPerSample(bitDepth));
			return num5 + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value5), "BLOCK_ALIGN") + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(bitDepth), "BITS_PER_SAMPLE");
		}

		private static int WriteFileData(ref MemoryStream stream, AudioClip audioClip, ushort bitDepth)
		{
			float[] array = new float[audioClip.samples * audioClip.channels];
			audioClip.GetData(array, 0);
			byte[] bytes = ConvertAudioClipDataToInt16ByteArray(array);
			byte[] bytes2 = Encoding.ASCII.GetBytes("data");
			int num = 0 + WriteBytesToMemoryStream(ref stream, bytes2, "DATA_ID");
			int value = Convert.ToInt32(audioClip.samples * 2 * audioClip.channels);
			return num + WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(value), "SAMPLES") + WriteBytesToMemoryStream(ref stream, bytes, "DATA");
		}

		private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			int count = 2;
			short num = short.MaxValue;
			for (int i = 0; i < data.Length; i++)
			{
				memoryStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * (float)num)), 0, count);
			}
			byte[] result = memoryStream.ToArray();
			memoryStream.Dispose();
			return result;
		}

		private static int WriteBytesToMemoryStream(ref MemoryStream stream, byte[] bytes, string tag = "")
		{
			int num = bytes.Length;
			stream.Write(bytes, 0, num);
			return num;
		}

		public static ushort BitDepth(AudioClip audioClip)
		{
			return Convert.ToUInt16((float)(audioClip.samples * audioClip.channels) * audioClip.length / (float)audioClip.frequency);
		}

		private static int BytesPerSample(ushort bitDepth)
		{
			return bitDepth / 8;
		}

		private static int BlockSize(ushort bitDepth)
		{
			return bitDepth switch
			{
				32 => 4, 
				16 => 2, 
				8 => 1, 
				_ => throw new Exception(bitDepth + " bit depth is not supported."), 
			};
		}

		private static string FormatCode(ushort code)
		{
			switch (code)
			{
			case 1:
				return "PCM";
			case 2:
				return "ADPCM";
			case 3:
				return "IEEE";
			case 7:
				return "μ-law";
			case 65534:
				return "WaveFormatExtensable";
			default:
				Debug.LogWarning((object)("Unknown wav code format:" + code));
				return "";
			}
		}
	}
	public class Satchel : Mod
	{
		public static Satchel Instance;

		public Satchel()
			: base("Satchel（依赖库）")
		{
		}

		public override string GetVersion()
		{
			return AssemblyUtils.GetAssemblyVersionHash(AssemblyUtils.ver) ?? "";
		}

		public override void Initialize()
		{
			if (Instance == null)
			{
				Instance = this;
			}
		}
	}
	public class Core
	{
		public static AssetBundle shaderBundle;

		public static Shader spriteFlash;

		public static AssetBundle spriteExtractBundle;

		public static Shader spriteExtract;

		public static Shader spriteMask;

		public CustomSaveSlotsManager customSaveSlotsManager;

		public CustomDialogueManager customDialogueManager;

		public CustomMapManager customMapManager;

		public CustomEnemyManager customEnemyManager;

		public CustomDreamNailManager customDreamNailManager;

		public CustomShinyManager customShinyManager;

		public CustomBigItemGetManager customBigItemGetManager;

		public Dictionary<string, CustomScene> customScenes = new Dictionary<string, CustomScene>();

		public static void LoadShaders()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected I4, but got Unknown
			OperatingSystemFamily operatingSystemFamily = SystemInfo.operatingSystemFamily;
			switch (operatingSystemFamily - 1)
			{
			case 0:
				shaderBundle = AssemblyUtils.GetAssetBundleFromResources("mac.shaders");
				spriteExtractBundle = AssemblyUtils.GetAssetBundleFromResources("mac.spriteextract");
				break;
			case 2:
				shaderBundle = AssemblyUtils.GetAssetBundleFromResources("lin.shaders");
				spriteExtractBundle = AssemblyUtils.GetAssetBundleFromResources("lin.spriteextract");
				break;
			default:
				shaderBundle = AssemblyUtils.GetAssetBundleFromResources("win.shaders");
				spriteExtractBundle = AssemblyUtils.GetAssetBundleFromResources("win.spriteextract");
				break;
			}
			if ((Object)(object)spriteFlash == (Object)null)
			{
				spriteFlash = shaderBundle.GetShader("spriteflash.shader");
			}
			if ((Object)(object)spriteExtractBundle != (Object)null)
			{
				spriteExtract = spriteExtractBundle.GetShader("SpriteExtract");
				spriteMask = spriteExtractBundle.GetShader("SpriteMaskMaker");
			}
		}

		static Core()
		{
			LoadShaders();
		}

		public CustomSaveSlotsManager GetCustomSaveSlotsManager()
		{
			if (customSaveSlotsManager == null)
			{
				customSaveSlotsManager = new CustomSaveSlotsManager();
			}
			return customSaveSlotsManager;
		}

		public CustomDialogueManager GetCustomDialogueManager(GameObject CardPrefab)
		{
			if (customDialogueManager == null)
			{
				customDialogueManager = new CustomDialogueManager(CardPrefab);
			}
			return customDialogueManager;
		}

		public CustomMapManager GetCustomMapManager()
		{
			if (customMapManager == null)
			{
				customMapManager = new CustomMapManager();
			}
			return customMapManager;
		}

		public CustomEnemyManager GetCustomEnemyManager()
		{
			if (customEnemyManager == null)
			{
				customEnemyManager = new CustomEnemyManager();
			}
			return customEnemyManager;
		}

		public CustomDreamNailManager GetCustomDreamNailManager()
		{
			if (customDreamNailManager == null)
			{
				customDreamNailManager = new CustomDreamNailManager();
			}
			return customDreamNailManager;
		}

		public CustomShinyManager GetCustomShinyManager()
		{
			if (customShinyManager == null)
			{
				customShinyManager = new CustomShinyManager();
			}
			return customShinyManager;
		}

		public CustomBigItemGetManager GetCustomBigItemGetManager()
		{
			if (customBigItemGetManager == null)
			{
				customBigItemGetManager = new CustomBigItemGetManager();
			}
			return customBigItemGetManager;
		}

		public CustomScene GetCustomScene(string sceneName)
		{
			if (customScenes.TryGetValue(sceneName, out var value))
			{
				return value;
			}
			return null;
		}

		public CustomScene GetCustomScene(string sceneName, GameObject TileMap, GameObject SceneManager)
		{
			if (!customScenes.TryGetValue(sceneName, out var value))
			{
				value = new CustomScene();
			}
			if (value != null)
			{
				value.sceneName = sceneName;
				value.TileMap = TileMap;
				value.SceneManager = SceneManager;
			}
			else
			{
				((Loggable)Satchel.Instance).Log("Error Creating scene : " + sceneName + " probably a scene with the same name already exists");
			}
			return value;
		}
	}
	public class UniqueTk2d : MonoBehaviour
	{
		private tk2dSpriteAnimator animator;

		private tk2dSpriteAnimation animation;

		private tk2dSpriteAnimation animationClone;

		private tk2dSpriteCollectionData collection;

		private tk2dSpriteCollectionData collectionClone;

		public bool IsUnique { get; private set; }

		private void EnsureDefaults()
		{
			animator = ((Component)this).GetComponent<tk2dSpriteAnimator>();
			if ((Object)(object)animator != (Object)null)
			{
				if ((Object)(object)animation == (Object)null)
				{
					animation = animator.Library;
				}
				if ((Object)(object)collection == (Object)null)
				{
					collection = animator.Library.clips[0].frames[0].spriteCollection;
				}
			}
		}

		private void EnsureClones()
		{
			EnsureDefaults();
			if ((Object)(object)animationClone == (Object)null)
			{
				animationClone = animation.Clone();
				((Component)animationClone).gameObject.transform.SetParent(((Component)this).transform);
			}
			if (!((Object)(object)collectionClone == (Object)null))
			{
				return;
			}
			collectionClone = collection.Clone();
			((Component)collectionClone).gameObject.transform.SetParent(((Component)this).transform);
			tk2dSpriteAnimationClip[] clips = animationClone.clips;
			for (int i = 0; i < clips.Length; i++)
			{
				tk2dSpriteAnimationFrame[] frames = clips[i].frames;
				for (int j = 0; j < frames.Length; j++)
				{
					frames[j].spriteCollection = collectionClone;
				}
			}
		}

		public UniqueTk2d MakeUnique()
		{
			EnsureClones();
			animator.Library = animationClone;
			IsUnique = true;
			return this;
		}

		public UniqueTk2d MakeGeneric()
		{
			EnsureDefaults();
			animator.Library = animation;
			IsUnique = false;
			return this;
		}

		public void SetTexture(Texture2D tex)
		{
			if (IsUnique)
			{
				collectionClone.SetTexture((Texture)(object)tex);
			}
			else
			{
				collection.SetTexture((Texture)(object)tex);
			}
		}
	}
	public static class AnimationUtils
	{
		public static bool IsPlaying(this Animator anim)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			return ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime <= 1f;
		}

		public static bool IsPlaying(this Animator anim, string s)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			if (((AnimatorStateInfo)(ref currentAnimatorStateInfo)).IsName(s))
			{
				return anim.IsPlaying();
			}
			return false;
		}

		public static string GetClipName(this Animator anim)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			AnimatorClipInfo val = anim.GetCurrentAnimatorClipInfo(0)[0];
			return ((Object)((AnimatorClipInfo)(ref val)).clip).name;
		}

		public static int GetCurrentFrame(this Animator anim)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			AnimatorClipInfo val = anim.GetCurrentAnimatorClipInfo(0)[0];
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			return (int)(((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime % 1f * (((AnimatorClipInfo)(ref val)).clip.length * ((AnimatorClipInfo)(ref val)).clip.frameRate));
		}

		public static void PlayAt(this Animator anim, string name, int frame)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			AnimatorClipInfo val = anim.GetCurrentAnimatorClipInfo(0)[0];
			float num = (float)frame / (((AnimatorClipInfo)(ref val)).clip.length * ((AnimatorClipInfo)(ref val)).clip.frameRate);
			anim.Play(name, 0, num);
		}

		public static void logTk2dAnimationClips(this GameObject go)
		{
			tk2dSpriteAnimator component = go.GetComponent<tk2dSpriteAnimator>();
			((Loggable)Satchel.Instance).Log("clip.names for " + ((Object)go).name);
			tk2dSpriteAnimationClip[] clips = component.Library.clips;
			foreach (tk2dSpriteAnimationClip val in clips)
			{
				((Loggable)Satchel.Instance).Log(val.name);
			}
		}

		public static IEnumerator playAllAnim(this GameObject go, int i = 0)
		{
			tk2dSpriteAnimator component = go.GetComponent<tk2dSpriteAnimator>();
			tk2dSpriteAnimationClip[] clips = component.Library.clips;
			i++;
			if (clips.Length <= i)
			{
				i = 0;
			}
			component.PlayFromFrame(clips[i], 0);
			((Loggable)Satchel.Instance).Log(clips[i].name);
			yield return (object)new WaitForSeconds(2f);
			((MonoBehaviour)CoroutineHelper.GetRunner()).StartCoroutine(go.playAllAnim(i));
		}
	}
	public static class AssemblyUtils
	{
		public static string name = "Satchel（依赖库）";

		public static string ver = "0.9.4";

		public static string Version()
		{
			return name + " v" + ver;
		}

		public static string GetAssemblyHash(this Assembly asm)
		{
			asm.GetName().Version.ToString();
			SHA1 sHA = SHA1.Create();
			FileStream fileStream = File.OpenRead(asm.Location);
			string text = BitConverter.ToString(sHA.ComputeHash(fileStream)).Replace("-", "").ToLowerInvariant();
			fileStream.Close();
			sHA.Clear();
			return text.Substring(0, 6) ?? "";
		}

		public static string GetAssemblyVersionHash()
		{
			return Assembly.GetCallingAssembly().GetAssemblyVersionHash();
		}

		public static string GetAssemblyVersionHash(string version = "")
		{
			return Assembly.GetCallingAssembly().GetAssemblyVersionHash(version);
		}

		public static string GetAssemblyVersionHash(this Assembly asm, string version = "")
		{
			string obj = ((version == "") ? asm.GetName().Version.ToString() : version);
			string assemblyHash = asm.GetAssemblyHash();
			return obj + "-" + assemblyHash;
		}

		public static string GetCurrentDirectory()
		{
			return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
		}

		[Obsolete("use GetCurrentDirectory instead")]
		public static string getCurrentDirectory()
		{
			return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
		}

		public static byte[] GetBytesFromResources(this Assembly asm, string fileName)
		{
			string[] manifestResourceNames = asm.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				if (!text.EndsWith(fileName))
				{
					continue;
				}
				using Stream stream = asm.GetManifestResourceStream(text);
				if (stream == null)
				{
					continue;
				}
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, array.Length);
				stream.Dispose();
				return array;
			}
			return null;
		}

		public static byte[] GetBytesFromResources(string fileName)
		{
			return Assembly.GetCallingAssembly().GetBytesFromResources(fileName);
		}

		public static Sprite GetSpriteFromResources(string fileName)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(2, 2);
			byte[] bytesFromResources = Assembly.GetCallingAssembly().GetBytesFromResources(fileName);
			if (bytesFromResources == null)
			{
				return null;
			}
			ImageConversion.LoadImage(val, bytesFromResources);
			val.Apply();
			return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 64f);
		}

		public static Sprite GetSpriteFromResources(string fileName, float pixelsPerUnit)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(2, 2);
			byte[] bytesFromResources = Assembly.GetCallingAssembly().GetBytesFromResources(fileName);
			if (bytesFromResources == null)
			{
				return null;
			}
			ImageConversion.LoadImage(val, bytesFromResources);
			val.Apply();
			return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
		}

		public static Texture2D GetTextureFromResources(string fileName)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			Texture2D val = new Texture2D(2, 2);
			byte[] bytesFromResources = Assembly.GetCallingAssembly().GetBytesFromResources(fileName);
			if (bytesFromResources == null)
			{
				return null;
			}
			ImageConversion.LoadImage(val, bytesFromResources);
			val.Apply();
			return val;
		}

		public static AssetBundle GetAssetBundleFromResources(string fileName)
		{
			byte[] bytesFromResources = Assembly.GetCallingAssembly().GetBytesFromResources(fileName);
			if (bytesFromResources == null)
			{
				return null;
			}
			return AssetBundle.LoadFromMemory(bytesFromResources);
		}

		public static Shader GetShader(this AssetBundle bundle, string shader)
		{
			return bundle.LoadAsset<Shader>(shader);
		}

		public static void ExtractFiles(this Assembly asm, string outpath, Func<string, string> shouldExtractAs)
		{
			IoUtils.EnsureDirectory(outpath);
			string[] manifestResourceNames = asm.GetManifestResourceNames();
			foreach (string arg in manifestResourceNames)
			{
				string text = shouldExtractAs(arg);
				if (!(text != ""))
				{
					continue;
				}
				using Stream stream = asm.GetManifestResourceStream(arg);
				if (stream != null)
				{
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, array.Length);
					File.WriteAllBytes(Path.Combine(outpath, text), array);
					stream.Dispose();
				}
			}
		}

		public static void ExtractFiles(string outpath, Func<string, string> shouldExtractAs)
		{
			Assembly.GetCallingAssembly().ExtractFiles(outpath, shouldExtractAs);
		}
	}
	public class Convo
	{
		public static string prefix = "dc_custom_dn_";

		public string Id;

		public string Key;

		public int Amount;

		public Convo(string convId)
		{
			Id = convId;
			Key = prefix + Id;
		}
	}
	public static class EnemyUtils
	{
		public static GameObject createCompanionFromPrefab(this GameObject CompanionPrefab)
		{
			return CompanionPrefab.createCompanionFromPrefab(false);
		}

		public static GameObject createCompanionFromPrefab(this GameObject CompanionPrefab, bool DestroyOnLoad = false)
		{
			GameObject val = Object.Instantiate<GameObject>(CompanionPrefab);
			((Object)val).name = "Companion";
			val.layer = 18;
			val.DisableChildren();
			val.SetActive(false);
			while (val.RemoveComponent<PlayMakerFSM>())
			{
			}
			val.RemoveComponent<PlayMakerUnity2DProxy>();
			val.RemoveComponent<PlayMakerFixedUpdate>();
			val.RemoveComponent<Recoil>();
			val.RemoveComponent<EnemyDreamnailReaction>();
			val.RemoveComponent<EnemyHitEffectsUninfected>();
			val.RemoveComponent<EnemyDeathEffectsUninfected>();
			val.RemoveComponent<HealthManager>();
			val.RemoveComponent<ExtraDamageable>();
			val.RemoveComponent<DamageHero>();
			val.RemoveComponent<ConstrainPosition>();
			if (!DestroyOnLoad)
			{
				Object.DontDestroyOnLoad((Object)(object)val);
			}
			return val;
		}

		public static GameObject createCustomDialogFromPrefab(this GameObject prefab)
		{
			GameObject val = Object.Instantiate<GameObject>(prefab);
			((Object)val).name = "CustomDialog";
			val.SetActive(false);
			PlayMakerFSM[] components = val.GetComponents<PlayMakerFSM>();
			foreach (PlayMakerFSM val2 in components)
			{
				if (val2.FsmName != "Conversation Control")
				{
					Object.DestroyImmediate((Object)(object)val2);
				}
			}
			val.RemoveComponent<PlayMakerUnity2DProxy>();
			val.RemoveComponent<PlayMakerFixedUpdate>();
			Object.DontDestroyOnLoad((Object)(object)val);
			return val;
		}

		public static void activateAlertRanges(this GameObject enemy)
		{
			AlertRangeMarker[] componentsInChildren = enemy.GetComponentsInChildren<AlertRangeMarker>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((Component)componentsInChildren[i]).gameObject.AddComponent<AlertRange>();
			}
		}

		public static WaitForSeconds moveTowards(this Rigidbody2D rb, Vector2 direction, float distance, float time)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			float num = distance / time;
			Vector2 velocity = ((Vector2)(ref direction)).normalized * num;
			rb.velocity = velocity;
			return new WaitForSeconds(time);
		}

		public static void copyHealthManagerFields(HealthManager from, HealthManager to)
		{
			if ((Object)(object)from == (Object)null)
			{
				return;
			}
			foreach (FieldInfo item in from x in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
				where x.Name.Contains("Prefab")
				select x)
			{
				item.SetValue(to, item.GetValue(from));
			}
			((Behaviour)to).enabled = true;
		}

		public static void copyInfectedEnemyEffectsFields(InfectedEnemyEffects from, InfectedEnemyEffects to)
		{
			if (!((Object)(object)from == (Object)null))
			{
				FieldInfo[] fields = typeof(InfectedEnemyEffects).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					fieldInfo.SetValue(to, fieldInfo.GetValue(from));
				}
				((Behaviour)to).enabled = true;
			}
		}

		public static void copyEnemyHitEffectsUninfectedFields(EnemyHitEffectsUninfected from, EnemyHitEffectsUninfected to)
		{
			if (!((Object)(object)from == (Object)null))
			{
				FieldInfo[] fields = typeof(EnemyHitEffectsUninfected).GetFields(BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn);
				foreach (FieldInfo fieldInfo in fields)
				{
					fieldInfo.SetValue(to, fieldInfo.GetValue(from));
				}
				((Behaviour)to).enabled = true;
			}
		}

		public static void copyEnemyDeathEffectsUninfectedFields(EnemyDeathEffectsUninfected from, EnemyDeathEffectsUninfected to)
		{
			if (!((Object)(object)from == (Object)null))
			{
				FieldInfo[] fields = typeof(EnemyDeathEffectsUninfected).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					fieldInfo.SetValue(to, fieldInfo.GetValue(from));
				}
				((Behaviour)to).enabled = true;
			}
		}

		public static void copyEnemyPrefabFields(GameObject from, GameObject to, bool isHealthManaged, bool isInfected)
		{
			if (isInfected)
			{
				copyInfectedEnemyEffectsFields(from.GetComponent<InfectedEnemyEffects>(), to.GetAddComponent<InfectedEnemyEffects>());
				return;
			}
			copyEnemyHitEffectsUninfectedFields(from.GetComponent<EnemyHitEffectsUninfected>(), to.GetAddComponent<EnemyHitEffectsUninfected>());
			copyEnemyDeathEffectsUninfectedFields(from.GetComponent<EnemyDeathEffectsUninfected>(), to.GetAddComponent<EnemyDeathEffectsUninfected>());
		}

		public static void addRecoil(this GameObject enemy, bool freezeInPlace, bool stopVelocityXWhenRecoilingUp, bool preventRecoilUp, float recoilSpeedBase, float recoilDuration)
		{
			Recoil addComponent = enemy.gameObject.GetAddComponent<Recoil>();
			ReflectionHelper.SetField<Recoil, bool>(addComponent, "freezeInPlace", freezeInPlace);
			ReflectionHelper.SetField<Recoil, bool>(addComponent, "stopVelocityXWhenRecoilingUp", stopVelocityXWhenRecoilingUp);
			ReflectionHelper.SetField<Recoil, bool>(addComponent, "preventRecoilUp", preventRecoilUp);
			ReflectionHelper.SetField<Recoil, float>(addComponent, "recoilSpeedBase", recoilSpeedBase);
			ReflectionHelper.SetField<Recoil, float>(addComponent, "recoilDuration", recoilDuration);
			((Behaviour)addComponent).enabled = true;
		}

		public static HealthManager manageHealth(this GameObject enemy, int health)
		{
			HealthManager addComponent = enemy.gameObject.GetAddComponent<HealthManager>();
			addComponent.hp = health;
			return addComponent;
		}

		public static EnemyDreamnailReaction addDreamNailDialogue(this GameObject enemy, string key, int Amount = 1)
		{
			EnemyDreamnailReaction addComponent = enemy.gameObject.GetAddComponent<EnemyDreamnailReaction>();
			((Behaviour)addComponent).enabled = true;
			addComponent.SetConvoTitle(key);
			ReflectionHelper.SetField<EnemyDreamnailReaction, int>(addComponent, "convoAmount", Amount);
			return addComponent;
		}

		public static EnemyDreamnailReaction disableDreamNailDialogue(this GameObject enemy)
		{
			EnemyDreamnailReaction addComponent = enemy.gameObject.GetAddComponent<EnemyDreamnailReaction>();
			((Behaviour)addComponent).enabled = false;
			return addComponent;
		}
	}
	public static class GameObjectUtils
	{
		public static T GetAddComponent<T>(this GameObject go) where T : Component
		{
			T val = go.GetComponent<T>();
			if ((Object)(object)val == (Object)null)
			{
				val = go.AddComponent<T>();
			}
			return val;
		}

		public static bool RemoveComponent<T>(this GameObject go) where T : Component
		{
			T component = go.GetComponent<T>();
			if ((Object)(object)component != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)component);
				return true;
			}
			return false;
		}

		public static T copyComponent<T>(this GameObject to, GameObject from) where T : Component
		{
			if ((Object)(object)from == (Object)null)
			{
				return default(T);
			}
			T component = from.GetComponent<T>();
			T addComponent = to.GetAddComponent<T>();
			if ((Object)(object)component == (Object)null)
			{
				return default(T);
			}
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn);
			foreach (FieldInfo fieldInfo in fields)
			{
				fieldInfo.SetValue(addComponent, fieldInfo.GetValue(component));
			}
			return addComponent;
		}

		public static void SetScale(this GameObject gameObject, float scaleX, float scaleY)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = gameObject.transform.localScale;
			localScale.x = scaleX;
			localScale.y = scaleY;
			gameObject.transform.localScale = localScale;
		}

		public static GameObject FindGameObjectInChildren(this GameObject gameObject, string name, bool useBaseName = false)
		{
			if ((Object)(object)gameObject == (Object)null)
			{
				return null;
			}
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform val in componentsInChildren)
			{
				if (val.GetName(useBaseName) == name)
				{
					return ((Component)val).gameObject;
				}
			}
			return null;
		}

		public static List<GameObject> FindGameObjectsInChildren(this GameObject gameObject, string name, bool useBaseName = false)
		{
			if ((Object)(object)gameObject == (Object)null)
			{
				return null;
			}
			List<GameObject> list = new List<GameObject>();
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform val in componentsInChildren)
			{
				if (val.GetName(useBaseName) == name)
				{
					list.Add(((Component)val).gameObject);
				}
			}
			return list;
		}

		public static void Log(this GameObject gameObject)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)gameObject == (Object)null)
			{
				return;
			}
			((Loggable)Satchel.Instance).Log(gameObject.GetName());
			((Loggable)Satchel.Instance).Log(gameObject.GetPath());
			((Loggable)Satchel.Instance).Log("Layer : " + gameObject.layer);
			((Loggable)Satchel.Instance).Log("Position : " + ((object)gameObject.transform.position/*cast due to constrained. prefix*/).ToString());
			((Loggable)Satchel.Instance).Log("Rotation : " + ((object)gameObject.transform.rotation/*cast due to constrained. prefix*/).ToString());
			((Loggable)Satchel.Instance).Log("Scale : " + ((object)gameObject.transform.localScale/*cast due to constrained. prefix*/).ToString());
			Component[] components = gameObject.GetComponents<Component>();
			foreach (Component val in components)
			{
				((Loggable)Satchel.Instance).Log("Component : " + ((object)val).GetType());
				if (val is PlayMakerFSM)
				{
					((Loggable)Satchel.Instance).Log("---- Fsm name :" + ((PlayMakerFSM)((val is PlayMakerFSM) ? val : null)).FsmName);
				}
			}
		}

		public static void LogWithChildren(this GameObject gameObject)
		{
			if (!((Object)(object)gameObject == (Object)null))
			{
				gameObject.Log();
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					((Component)componentsInChildren[i]).gameObject.Log();
				}
			}
		}

		public static void PrintAllActiveGameObjectsInScene()
		{
			GameObject[] array = Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Log();
			}
		}

		public static GameObject Find(this GameObject go, string name)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				GameObject gameObject = ((Component)go.transform.GetChild(i)).gameObject;
				if (((Object)gameObject).name == name)
				{
					return gameObject;
				}
			}
			for (int j = 0; j < go.transform.childCount; j++)
			{
				GameObject val = ((Component)go.transform.GetChild(j)).gameObject.Find(name);
				if ((Object)(object)val != (Object)null)
				{
					return val;
				}
			}
			return null;
		}

		public static void FindAllChildren(this GameObject go, List<GameObject> allGoList)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				allGoList.Add(((Component)go.transform.GetChild(i)).gameObject);
			}
			for (int j = 0; j < go.transform.childCount; j++)
			{
				((Component)go.transform.GetChild(j)).gameObject.FindAllChildren(allGoList);
			}
		}

		public static void DisableChildren(this GameObject go)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				((Component)go.transform.GetChild(i)).gameObject.SetActive(false);
			}
		}

		public static List<GameObject> GetAllGameObjects(this Scene scene)
		{
			List<GameObject> list = new List<GameObject>();
			GameObject[] rootGameObjects = ((Scene)(ref scene)).GetRootGameObjects();
			foreach (GameObject val in rootGameObjects)
			{
				list.Add(val);
				val.FindAllChildren(list);
			}
			return list;
		}

		public static GameObject GetGameObjectByName(this Scene scene, string name, bool useBaseName = false)
		{
			GameObject[] rootGameObjects = ((Scene)(ref scene)).GetRootGameObjects();
			foreach (GameObject val in rootGameObjects)
			{
				if (val.GetName(useBaseName) == name)
				{
					return val;
				}
				GameObject val2 = val.FindGameObjectInChildren(name, useBaseName);
				if ((Object)(object)val2 != (Object)null)
				{
					return val2;
				}
			}
			return null;
		}

		public static GameObject[] GetRootGameObjects()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			return ((Scene)(ref activeScene)).GetRootGameObjects();
		}

		public static GameObject GetGameObjectInScene(string name, bool useBaseName = false)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return SceneManager.GetActiveScene().GetGameObjectByName(name, useBaseName);
		}

		public static List<GameObject> GetAllGameObjectsInScene()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return SceneManager.GetActiveScene().GetAllGameObjects();
		}

		public static string GetName(this Transform transform, bool useBaseName = false)
		{
			return ((Component)transform).gameObject.GetName(useBaseName);
		}

		public static string GetName(this GameObject go, bool useBaseName = false)
		{
			string text = ((Object)go).name;
			if (useBaseName)
			{
				text = text.ToLower();
				text.Replace("(clone)", "");
				text = text.Trim();
				text.Replace("cln", "");
				text = text.Trim();
				text = Regex.Replace(text, "\\([0-9+]+\\)", "");
				text = text.Trim();
				text = Regex.Replace(text, "[0-9+]+$", "");
				text = text.Trim();
				text.Replace("(clone)", "");
				text = text.Trim();
			}
			return text;
		}

		public static string GetPath(this GameObject go, bool useBaseName = false)
		{
			string text = go.GetName(useBaseName);
			GameObject val = go;
			while ((Object)(object)val.transform.parent != (Object)null && (Object)(object)((Component)val.transform.parent).gameObject != (Object)null)
			{
				val = ((Component)val.transform.parent).gameObject;
				text = val.GetName(useBaseName) + "/" + text;
			}
			return text;
		}

		public static CustomData GetAddCustomData(this GameObject go)
		{
			return go.GetAddComponent<CustomData>();
		}
	}
	public static class IoUtils
	{
		public static void OpenDefault(string path)
		{
			if (path != null && (Directory.Exists(path) || File.Exists(path)))
			{
				Process.Start(path);
			}
		}

		public static void EnsureDirectory(string directory)
		{
			DirectoryExists(directory, create: true);
		}

		public static bool DirectoryExists(string directory, bool create = false)
		{
			if (!Directory.Exists(directory))
			{
				if (!create)
				{
					return false;
				}
				Directory.CreateDirectory(directory);
			}
			return true;
		}

		public static bool DirectoryHas(string sourceDirectory, string match = "*", SearchOption op = SearchOption.TopDirectoryOnly)
		{
			return Directory.EnumerateFiles(sourceDirectory, match, op).Any();
		}

		public static void DirectoryCopyAllFiles(string source, string destination, bool create = false)
		{
			DirectoryCopyAllFiles(source, destination, create, recursive: false);
		}

		public static void DirectoryCopyAllFiles(string source, string destination, bool create, bool recursive)
		{
			if (!Directory.Exists(source) || (!create && !Directory.Exists(destination)))
			{
				return;
			}
			DirectoryExists(destination, create);
			string[] files = Directory.GetFiles(source);
			foreach (string text in files)
			{
				try
				{
					File.Copy(text, Path.Combine(destination, Path.GetFileName(text)));
				}
				catch (Exception ex)
				{
					((Loggable)Satchel.Instance).LogError("A File could not be Copied : " + ex.ToString());
				}
			}
			if (recursive)
			{
				files = Directory.GetDirectories(source);
				foreach (string text2 in files)
				{
					string destination2 = Path.Combine(destination, Path.GetFileName(text2));
					DirectoryCopyAllFiles(text2, destination2, create, recursive);
				}
			}
		}
	}
	public static class PlayerDataUtils
	{
		public static int ConvertSlotToNumber(SaveSlot saveSlot)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			if ((int)saveSlot == 0)
			{
				return 1;
			}
			if ((int)saveSlot == 1)
			{
				return 2;
			}
			if ((int)saveSlot == 2)
			{
				return 3;
			}
			if ((int)saveSlot == 3)
			{
				return 4;
			}
			return 0;
		}

		public static void GetPlayerDataForSlot(int saveSlot, Action<PlayerData> callback)
		{
			Platform.Current.ReadSaveSlot(saveSlot, (Action<byte[]>)delegate(byte[] fileBytes)
			{
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Expected O, but got Unknown
				if (fileBytes == null)
				{
					if (callback != null)
					{
						CoreLoop.InvokeNext((Action)delegate
						{
							callback(null);
						});
					}
					return;
				}
				try
				{
					string text;
					if (GameManager.instance.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						MemoryStream serializationStream = new MemoryStream(fileBytes);
						text = Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
					}
					else
					{
						text = Encoding.UTF8.GetString(fileBytes);
					}
					SaveGameData val;
					try
					{
						val = JsonConvert.DeserializeObject<SaveGameData>(text, new JsonSerializerSettings
						{
							ContractResolver = (IContractResolver)(object)ShouldSerializeContractResolver.Instance,
							TypeNameHandling = (TypeNameHandling)4,
							ObjectCreationHandling = (ObjectCreationHandling)2,
							Converters = JsonConverterTypes.ConverterTypes
						});
					}
					catch (Exception)
					{
						((Loggable)Satchel.Instance).LogWarn($"Failed to get save stats for slot {saveSlot} using Json.NET, falling back");
						val = JsonUtility.FromJson<SaveGameData>(text);
					}
					PlayerData playerData = val.playerData;
					if (callback != null)
					{
						CoreLoop.InvokeNext((Action)delegate
						{
							callback(playerData);
						});
					}
				}
				catch (Exception ex2)
				{
					((Loggable)Satchel.Instance).LogError("Error while loading save file for slot " + saveSlot + " Exception: " + ex2);
					if (callback != null)
					{
						CoreLoop.InvokeNext((Action)delegate
						{
							callback(null);
						});
					}
				}
			});
		}
	}
	public static class SceneUtils
	{
		public struct CustomSaveSlotParams
		{
			public string sceneName;

			public string sceneTitle;

			public Sprite background;
		}

		public class CustomSceneManagerSettings
		{
			public SceneType sceneType;

			public MapZone mapZone;

			public int darknessLevel;

			public float saturation;

			public bool ignorePlatformSaturationModifiers;

			public bool isWindy;

			public bool isTremorZone;

			public int environmentType;

			public bool noParticles;

			public MapZone overrideParticlesWith;

			public Color defaultColor;

			public float defaultIntensity;

			public Color heroLightColor;

			public AudioClip backgroundMusic;

			public Func<AudioClip> backgroundMusicGet;

			public CustomSceneManagerSettings(SceneManager prefab)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				base..ctor();
				sceneType = prefab.sceneType;
				mapZone = prefab.mapZone;
				darknessLevel = prefab.darknessLevel;
				saturation = prefab.saturation;
				ignorePlatformSaturationModifiers = prefab.ignorePlatformSaturationModifiers;
				isWindy = prefab.isWindy;
				isTremorZone = prefab.isTremorZone;
				environmentType = prefab.environmentType;
				noParticles = prefab.noParticles;
				overrideParticlesWith = prefab.overrideParticlesWith;
				defaultColor = prefab.defaultColor;
				defaultIntensity = prefab.defaultIntensity;
				heroLightColor = prefab.heroLightColor;
			}
		}

		public struct BenchParams
		{
			public GameObject prefab;

			public string benchName;

			public Vector3 pos;

			public string sceneName;
		}

		public struct GatewayParams
		{
			public string gateName;

			public Vector2 pos;

			public Vector2 size;

			public string fromScene;

			public string toScene;

			public string entryGate;

			public Vector2 respawnPoint;

			public bool onlyOut;

			public SceneLoadVisualizations vis;
		}

		internal static GameObject DontDestroyOnLoadObj;

		public static Scene GetDontDestroyOnLoadScene()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			if ((Object)(object)DontDestroyOnLoadObj == (Object)null)
			{
				DontDestroyOnLoadObj = new GameObject();
				Object.DontDestroyOnLoad((Object)(object)DontDestroyOnLoadObj);
			}
			return DontDestroyOnLoadObj.scene;
		}

		public static Scene getCurrentScene()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return SceneManager.GetActiveScene();
		}

		public static List<Scene> GetAllLoadedScenes(bool includeDontDestroyScene = false)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			List<Scene> list = new List<Scene>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				list.Add(sceneAt);
			}
			if (includeDontDestroyScene)
			{
				list.Add(GetDontDestroyOnLoadScene());
			}
			return list;
		}

		public static void CreateGateway(GatewayParams gateway)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			CreateGateway(gateway.gateName, gateway.pos, gateway.size, gateway.toScene, gateway.entryGate, gateway.respawnPoint, gateway.onlyOut, gateway.vis);
		}

		public static void CreateGateway(string gateName, Vector2 pos, Vector2 size, string toScene, string entryGate, Vector2 respawnPoint, bool onlyOut, SceneLoadVisualizations vis)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected O, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)GameObject.Find(gateName) != (Object)null))
			{
				GameObject val = new GameObject(gateName);
				Extensions.SetPosition2D(val.transform, pos);
				TransitionPoint val2 = val.AddComponent<TransitionPoint>();
				if (!onlyOut)
				{
					BoxCollider2D obj = val.AddComponent<BoxCollider2D>();
					obj.size = size;
					((Collider2D)obj).isTrigger = true;
					val2.SetTargetScene(toScene);
					val2.entryPoint = entryGate;
				}
				val2.alwaysEnterLeft = gateName.Contains("left");
				val2.alwaysEnterRight = gateName.Contains("right");
				GameObject val3 = new GameObject("Hazard Respawn Marker");
				val3.transform.parent = val.transform;
				Extensions.SetPosition2D(val3.transform, pos.x + respawnPoint.x, pos.y + respawnPoint.y);
				val3.AddComponent<HazardRespawnMarker>().respawnFacingRight = val2.alwaysEnterRight;
				val2.respawnMarker = val3.GetComponent<HazardRespawnMarker>();
				val2.sceneLoadVisualization = vis;
			}
		}

		public static GameObject CreateBenchFromPrefab(BenchParams bench)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return CreateBenchFromPrefab(bench.prefab, bench.benchName, bench.pos, bench.sceneName);
		}

		public static GameObject CreateBenchFromPrefab(GameObject benchPrefab, string benchName, Vector3 pos, string sceneName)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)GameObject.Find(benchName) != (Object)null)
			{
				return null;
			}
			GameObject val = Object.Instantiate<GameObject>(benchPrefab);
			val.transform.position = pos;
			((Object)val).name = benchName;
			PlayMakerFSM obj = FSMUtility.LocateMyFSM(val, "Bench Control");
			obj.FsmVariables.FindFsmString("Scene Name").Value = sceneName;
			obj.FsmVariables.FindFsmString("Spawn Name").Value = ((Object)val).name;
			val.SetActive(true);
			return val;
		}

		public static tk2dTileMap FakeTileMapFromPrefab(GameObject TileMapPrefab)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			GameObject obj = Object.Instantiate<GameObject>(TileMapPrefab);
			obj.SetActive(false);
			tk2dTileMap component = obj.GetComponent<tk2dTileMap>();
			component.renderData = new GameObject("dummydata");
			obj.SetActive(true);
			return component;
		}

		public static SceneManager getSceneManagerFromPrefab(GameObject SceneManagerPrefab)
		{
			GameObject obj = Object.Instantiate<GameObject>(SceneManagerPrefab);
			obj.SetActive(false);
			return obj.GetComponent<SceneManager>();
		}

		public static void PlayBackgroundMusicForScene(AudioClip clip)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			MusicCue val = ScriptableObject.CreateInstance<MusicCue>();
			List<MusicChannelInfo> list = new List<MusicChannelInfo>();
			MusicChannelInfo val2 = new MusicChannelInfo();
			ReflectionHelper.SetField<MusicChannelInfo, AudioClip>(val2, "clip", clip);
			list.Add(val2);
			list.Add(null);
			list.Add(null);
			list.Add(null);
			list.Add(null);
			list.Add(null);
			ReflectionHelper.SetField<MusicCue, MusicChannelInfo[]>(val, "channelInfos", list.ToArray());
			AudioMixer[] array = Resources.FindObjectsOfTypeAll<AudioMixer>();
			foreach (AudioMixer val3 in array)
			{
				if (((Object)val3).name == "Music")
				{
					val3.FindSnapshot("Main Only").TransitionTo(1f);
					GameManager.instance.AudioManager.ApplyMusicCue(val, 0f, 0f, false);
					break;
				}
			}
		}
	}
	public static class SpriteUtils
	{
		public static Sprite CreateSpriteFromTexture(Texture2D texture)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			return Sprite.Create(texture, new Rect(0f, 0f, (float)((Texture)texture).width, (float)((Texture)texture).height), new Vector2(0.5f, 0.5f));
		}

		public static Sprite CreateSpriteFromTexture(Texture2D texture, float ppu)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			return Sprite.Create(texture, new Rect(0f, 0f, (float)((Texture)texture).width, (float)((Texture)texture).height), new Vector2(0.5f, 0.5f), ppu);
		}

		private static void saveTriangle(bool[][] triangle, string spriteName, int num)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(triangle[0].Length, triangle.Length);
			for (int i = 0; i < triangle[0].Length; i++)
			{
				for (int j = 0; j < triangle.Length; j++)
				{
					val.SetPixel(i, j, triangle[j][i] ? Color.white : Color.black);
				}
			}
			val.Apply();
			TextureUtils.WriteTextureToFile((Texture)(object)val, $"{AssemblyUtils.getCurrentDirectory()}/{spriteName}-{num}.png");
			Object.DestroyImmediate((Object)(object)val);
		}

		private static float CalcTriangleArea(Vector2Int a, Vector2Int b, Vector2Int c)
		{
			return Mathf.Abs((float)(((Vector2Int)(ref a)).x * (((Vector2Int)(ref b)).y - ((Vector2Int)(ref c)).y) + ((Vector2Int)(ref b)).x * (((Vector2Int)(ref c)).y - ((Vector2Int)(ref a)).y) + ((Vector2Int)(ref c)).x * (((Vector2Int)(ref a)).y - ((Vector2Int)(ref b)).y)) / 2f);
		}

		private static Vector2 GetSpriteOrigin(Sprite originalSprite)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			(int, int) tuple = (((Texture)originalSprite.texture).width, ((Texture)originalSprite.texture).height);
			List<Vector2Int> list = new List<Vector2Int>();
			Vector2[] uv = originalSprite.uv;
			foreach (Vector2 val in uv)
			{
				list.Add(new Vector2Int(Mathf.RoundToInt(val.x * (float)(tuple.Item1 - 1)), Mathf.RoundToInt(val.y * (float)(tuple.Item2 - 1))));
			}
			int num = list.Select((Vector2Int val2) => ((Vector2Int)(ref val2)).x).ToList().Min();
			list.Select((Vector2Int val2) => ((Vector2Int)(ref val2)).x).ToList().Max();
			int num2 = list.Select((Vector2Int val2) => ((Vector2Int)(ref val2)).y).ToList().Min();
			list.Select((Vector2Int val2) => ((Vector2Int)(ref val2)).y).ToList().Max();
			return new Vector2((float)num, (float)num2);
		}

		private static Texture2D ExtractTextureWithShader(Texture2D tex, Vector2Int offset, Vector2Int realSize, Vector2Int texSize, Vector2Int border, bool flipHorizontal, bool flipVertical, List<(Vector2Int, Vector2Int, Vector2Int)> triangles, bool drawMask = false)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Expected O, but got Unknown
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Expected O, but got Unknown
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Expected O, but got Unknown
			if ((Object)(object)Core.spriteExtract == (Object)null || (Object)(object)Core.spriteMask == (Object)null)
			{
				throw new InvalidOperationException();
			}
			Material val = new Material(Core.spriteMask);
			val.SetColor("_Color", Color.red);
			RenderTexture val2 = new RenderTexture(((Vector2Int)(ref realSize)).x, ((Vector2Int)(ref realSize)).y, 0, (RenderTextureFormat)7, (RenderTextureReadWrite)1);
			val2.Create();
			RenderTexture val3 = new RenderTexture(((Vector2Int)(ref realSize)).x, ((Vector2Int)(ref realSize)).y, 0, (RenderTextureFormat)7, (RenderTextureReadWrite)1);
			val3.Create();
			Vector2 val5 = default(Vector2);
			Vector2 val6 = default(Vector2);
			Vector2 val7 = default(Vector2);
			foreach (var triangle in triangles)
			{
				Vector2Int val4 = triangle.Item1;
				float num = (float)(((Vector2Int)(ref val4)).x - ((Vector2Int)(ref offset)).x) / (float)((Vector2Int)(ref realSize)).x;
				(val4, _, _) = triangle;
				((Vector2)(ref val5))..ctor(num, (float)(((Vector2Int)(ref val4)).y - ((Vector2Int)(ref offset)).y) / (float)((Vector2Int)(ref realSize)).y);
				val4 = triangle.Item2;
				float num2 = (float)(((Vector2Int)(ref val4)).x - ((Vector2Int)(ref offset)).x) / (float)((Vector2Int)(ref realSize)).x;
				val4 = triangle.Item2;
				((Vector2)(ref val6))..ctor(num2, (float)(((Vector2Int)(ref val4)).y - ((Vector2Int)(ref offset)).y) / (float)((Vector2Int)(ref realSize)).y);
				val4 = triangle.Item3;
				float num3 = (float)(((Vector2Int)(ref val4)).x - ((Vector2Int)(ref offset)).x) / (float)((Vector2Int)(ref realSize)).x;
				val4 = triangle.Item3;
				((Vector2)(ref val7))..ctor(num3, (float)(((Vector2Int)(ref val4)).y - ((Vector2Int)(ref offset)).y) / (float)((Vector2Int)(ref realSize)).y);
				val.SetVector("_P0", Vector4.op_Implicit(val5));
				val.SetVector("_P1", Vector4.op_Implicit(val6));
				val.SetVector("_P2", Vector4.op_Implicit(val7));
				Graphics.Blit((Texture)(object)val3, val2, val);
				Graphics.Blit((Texture)(object)val2, val3);
			}
			val3.Release();
			Object.Destroy((Object)(object)val);
			Material val8 = new Material(Core.spriteExtract);
			val8.SetTexture("_MainTex", (Texture)(object)tex);
			val8.SetTexture("_MaskTex", (Texture)(object)val2);
			val8.SetFloat("_DrawMask", (float)(drawMask ? 1 : 0));
			val8.SetFloat("_OffsetX", (float)((Vector2Int)(ref offset)).x);
			val8.SetFloat("_OffsetY", (float)((Vector2Int)(ref offset)).y);
			val8.SetFloat("_SrcWidth", (float)((Texture)tex).width);
			val8.SetFloat("_SrcHeight", (float)((Texture)tex).height);
			val8.SetFloat("_Width", (float)((Vector2Int)(ref realSize)).x);
			val8.SetFloat("_Height", (float)((Vector2Int)(ref realSize)).y);
			val8.SetFloat("_DstWidth", (float)((Vector2Int)(ref texSize)).x);
			val8.SetFloat("_DstHeight", (float)((Vector2Int)(ref texSize)).y);
			val8.SetFloat("_FlipH", (float)(flipHorizontal ? 1 : (-1)));
			val8.SetFloat("_FlipV", (float)(flipVertical ? 1 : (-1)));
			val8.SetFloat("_BorderLeft", (float)((Vector2Int)(ref border)).x);
			val8.SetFloat("_BorderBottom", (float)((Vector2Int)(ref border)).y);
			RenderTexture val9 = new RenderTexture(((Vector2Int)(ref texSize)).x, ((Vector2Int)(ref texSize)).y, 0, (RenderTextureFormat)7, (RenderTextureReadWrite)1);
			val9.Create();
			Graphics.Blit((Texture)(object)tex, val9, val8);
			val2.Release();
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = val9;
			Texture2D val10 = new Texture2D(((Vector2Int)(ref texSize)).x, ((Vector2Int)(ref texSize)).y);
			val10.ReadPixels(new Rect(0f, 0f, (float)((Vector2Int)(ref texSize)).x, (float)((Vector2Int)(ref texSize)).y), 0, 0);
			val10.Apply();
			RenderTexture.active = active;
			Object.Destroy((Object)(object)val8);
			val9.Release();
			return val10;
		}

		public static Texture2D ExtractTextureFromSpriteExperimental(Sprite originalSprite, bool saveTriangles = false)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Invalid comparison between Unknown and I4
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Invalid comparison between Unknown and I4
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Invalid comparison between Unknown and I4
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Invalid comparison between Unknown and I4
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			(int, int) tuple = (((Texture)originalSprite.texture).width, ((Texture)originalSprite.texture).height);
			List<Vector2Int> list = new List<Vector2Int>();
			Vector2[] uv = originalSprite.uv;
			foreach (Vector2 val in uv)
			{
				list.Add(new Vector2Int(Mathf.RoundToInt(val.x * (float)(tuple.Item1 - 1)), Mathf.RoundToInt(val.y * (float)(tuple.Item2 - 1))));
			}
			int num = list.Select((Vector2Int val5) => ((Vector2Int)(ref val5)).x).ToList().Min();
			int num2 = list.Select((Vector2Int val5) => ((Vector2Int)(ref val5)).x).ToList().Max();
			int num3 = list.Select((Vector2Int val5) => ((Vector2Int)(ref val5)).y).ToList().Min();
			int num4 = list.Select((Vector2Int val5) => ((Vector2Int)(ref val5)).y).ToList().Max();
			int num5 = num2 - num + 1;
			int num6 = num4 - num3 + 1;
			List<(Vector2Int, Vector2Int, Vector2Int)> list2 = new List<(Vector2Int, Vector2Int, Vector2Int)>();
			for (int num7 = 0; num7 < originalSprite.triangles.Length; num7 += 3)
			{
				list2.Add((list[originalSprite.triangles[num7]], list[originalSprite.triangles[num7 + 1]], list[originalSprite.triangles[num7 + 2]]));
			}
			if (saveTriangles)
			{
				bool[][] array = new bool[num6][];
				for (int num7 = 0; num7 < array.Length; num7++)
				{
					array[num7] = new bool[num5];
				}
				foreach (var item in list2)
				{
					float num8 = CalcTriangleArea(item.Item1, item.Item2, item.Item3);
					for (int num9 = 0; num9 < num5; num9++)
					{
						for (int num10 = 0; num10 < num6; num10++)
						{
							Vector2Int val2 = new Vector2Int(num + num9, num3 + num10);
							float num11 = CalcTriangleArea(item.Item1, item.Item2, val2);
							float num12 = CalcTriangleArea(val2, item.Item2, item.Item3);
							float num13 = CalcTriangleArea(item.Item1, val2, item.Item3);
							if (num11 + num12 + num13 == num8)
							{
								array[num10][num9] = true;
							}
						}
					}
				}
				saveTriangle(array, ((Object)originalSprite).name, 1000000);
			}
			bool flipHorizontal = false;
			bool flipVertical = false;
			if (originalSprite.packed)
			{
				flipHorizontal = (int)originalSprite.packingRotation == 1 || (int)originalSprite.packingRotation == 3;
				flipVertical = (int)originalSprite.packingRotation == 2 || (int)originalSprite.packingRotation == 3;
			}
			Vector2 pivot = originalSprite.pivot;
			Vector2Int val3 = default(Vector2Int);
			((Vector2Int)(ref val3))..ctor(num5, num6);
			Vector2 val4 = default(Vector2);
			((Vector2)(ref val4))..ctor(0f, 0f);
			val4.x = pivot.x / (float)((Vector2Int)(ref val3)).x;
			val4.y = pivot.y / (float)((Vector2Int)(ref val3)).y;
			Vector2Int border = default(Vector2Int);
			((Vector2Int)(ref border))..ctor(0, 0);
			float num14 = 0.5f - val4.x;
			float num15 = 0.5f - val4.y;
			if (num14 > 0f)
			{
				((Vector2Int)(ref border)).x = (int)(2f * Mathf.Abs(num14) * (float)num5);
				((Vector2Int)(ref val3)).x = num5 + ((Vector2Int)(ref border)).x;
			}
			else
			{
				((Vector2Int)(ref border)).x = 0;
				((Vector2Int)(ref val3)).x = num5 + (int)(2f * Mathf.Abs(num14) * (float)num5);
			}
			if (num15 < 0f)
			{
				((Vector2Int)(ref border)).y = (int)(2f * Mathf.Abs(num15) * (float)num6);
				((Vector2Int)(ref val3)).y = num6 + ((Vector2Int)(ref border)).y;
			}
			else
			{
				((Vector2Int)(ref border)).y = 0;
				((Vector2Int)(ref val3)).y = num6 + (int)(2f * Mathf.Abs(num15) * (float)num6);
			}
			return ExtractTextureWithShader(originalSprite.texture, new Vector2Int(num, num3), new Vector2Int(num5, num6), new Vector2Int(((Vector2Int)(ref val3)).x, ((Vector2Int)(ref val3)).y), border, flipHorizontal, flipVertical, list2);
		}

		public static Texture2D ExtractTextureFromSpriteLegacy(Sprite originalSprite, bool saveTriangles = false)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Expected O, but got Unknown
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Invalid comparison between Unknown and I4
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Invalid comparison between Unknown and I4
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Invalid comparison between Unknown and I4
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Invalid comparison between Unknown and I4
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			(int, int) tuple = (((Texture)originalSprite.texture).width, ((Texture)originalSprite.texture).height);
			List<Vector2Int> list = new List<Vector2Int>();
			List<(Vector2Int, Vector2Int, Vector2Int)> list2 = new List<(Vector2Int, Vector2Int, Vector2Int)>();
			Vector2[] uv = originalSprite.uv;
			foreach (Vector2 val in uv)
			{
				list.Add(new Vector2Int(Mathf.RoundToInt(val.x * (float)(tuple.Item1 - 1)), Mathf.RoundToInt(val.y * (float)(tuple.Item2 - 1))));
			}
			for (int j = 0; j < originalSprite.triangles.Length; j += 3)
			{
				list2.Add((list[originalSprite.triangles[j]], list[originalSprite.triangles[j + 1]], list[originalSprite.triangles[j + 2]]));
			}
			int num = list.Select((Vector2Int val9) => ((Vector2Int)(ref val9)).x).ToList().Min();
			int num2 = list.Select((Vector2Int val9) => ((Vector2Int)(ref val9)).x).ToList().Max();
			int num3 = list.Select((Vector2Int val9) => ((Vector2Int)(ref val9)).y).ToList().Min();
			int num4 = list.Select((Vector2Int val9) => ((Vector2Int)(ref val9)).y).ToList().Max();
			int num5 = num2 - num + 1;
			int num6 = num4 - num3 + 1;
			bool[][] array = new bool[num6][];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new bool[num5];
			}
			foreach (var item in list2)
			{
				float num7 = CalcTriangleArea(item.Item1, item.Item2, item.Item3);
				for (int num8 = 0; num8 < num5; num8++)
				{
					for (int num9 = 0; num9 < num6; num9++)
					{
						Vector2Int val2 = new Vector2Int(num + num8, num3 + num9);
						float num10 = CalcTriangleArea(item.Item1, item.Item2, val2);
						float num11 = CalcTriangleArea(val2, item.Item2, item.Item3);
						float num12 = CalcTriangleArea(item.Item1, val2, item.Item3);
						if (num10 + num11 + num12 == num7)
						{
							array[num9][num8] = true;
						}
					}
				}
			}
			if (saveTriangles)
			{
				saveTriangle(array, ((Object)originalSprite).name, 1000000);
			}
			Texture2D val3 = TextureUtils.duplicateTexture(originalSprite.texture);
			Texture2D val4 = new Texture2D(num5, num6);
			for (int num8 = 0; num8 < num5; num8++)
			{
				for (int num9 = 0; num9 < num6; num9++)
				{
					if (array[num9][num8])
					{
						val4.SetPixel(num8, num9, val3.GetPixel(num + num8, num3 + num9));
					}
					else
					{
						val4.SetPixel(num8, num9, new Color(0f, 0f, 0f, 0f));
					}
				}
			}
			Object.DestroyImmediate((Object)(object)val3);
			val4.Apply();
			bool flag = false;
			bool flag2 = false;
			if (originalSprite.packed)
			{
				flag = (int)originalSprite.packingRotation == 1 || (int)originalSprite.packingRotation == 3;
				flag2 = (int)originalSprite.packingRotation == 2 || (int)originalSprite.packingRotation == 3;
			}
			val3 = ((!(flag | flag2)) ? val4 : val4.Flip(flag, flag2));
			Vector2 pivot = originalSprite.pivot;
			Vector2Int val5 = default(Vector2Int);
			((Vector2Int)(ref val5))..ctor(num5, num6);
			Vector2 val6 = default(Vector2);
			((Vector2)(ref val6))..ctor(0f, 0f);
			val6.x = pivot.x / (float)((Vector2Int)(ref val5)).x;
			val6.y = pivot.y / (float)((Vector2Int)(ref val5)).y;
			Vector2Int val7 = default(Vector2Int);
			((Vector2Int)(ref val7))..ctor(0, 0);
			float num13 = 0.5f - val6.x;
			float num14 = 0.5f - val6.y;
			if (num13 > 0f)
			{
				((Vector2Int)(ref val7)).x = (int)(2f * Mathf.Abs(num13) * (float)num5);
				((Vector2Int)(ref val5)).x = num5 + ((Vector2Int)(ref val7)).x;
			}
			else
			{
				((Vector2Int)(ref val7)).x = 0;
				((Vector2Int)(ref val5)).x = num5 + (int)(2f * Mathf.Abs(num13) * (float)num5);
			}
			if (num14 < 0f)
			{
				((Vector2Int)(ref val7)).y = (int)(2f * Mathf.Abs(num14) * (float)num6);
				((Vector2Int)(ref val5)).y = num6 + ((Vector2Int)(ref val7)).y;
			}
			else
			{
				((Vector2Int)(ref val7)).y = 0;
				((Vector2Int)(ref val5)).y = num6 + (int)(2f * Mathf.Abs(num14) * (float)num6);
			}
			Texture2D val8 = TextureUtils.createTextureOfColor(((Vector2Int)(ref val5)).x, ((Vector2Int)(ref val5)).y, new Color(0f, 0f, 0f, 0f));
			for (int num8 = 0; num8 < num5; num8++)
			{
				for (int num9 = 0; num9 < num6; num9++)
				{
					val8.SetPixel(((Vector2Int)(ref val7)).x + num8, ((Vector2Int)(ref val7)).y + num9, val3.GetPixel(num8, num9));
				}
			}
			val8.Apply();
			Object.DestroyImmediate((Object)(object)val3);
			return val8;
		}

		public static Texture2D ExtractTextureFromSprite(Sprite originalSprite, bool saveTriangles = false)
		{
			return ExtractTextureFromSpriteLegacy(originalSprite, saveTriangles);
		}
	}
	public static class TextureUtils
	{
		public static Texture2D createTextureOfColor(int width, int height, Color color)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(width, height);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					val.SetPixel(i, j, color);
				}
			}
			val.Apply();
			return val;
		}

		public static Texture2D LoadTextureFromFile(string path)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			Texture2D val = new Texture2D(2, 2);
			byte[] array = File.ReadAllBytes(path);
			ImageConversion.LoadImage(val, array);
			val.Apply();
			return val;
		}

		public static void WriteTextureToFile(Texture texture, string path)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			File.WriteAllBytes(path, ImageConversion.EncodeToPNG(duplicateTexture((Texture2D)texture)));
		}

		public static Texture2D duplicateTexture(Texture2D source)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			RenderTexture temporary = RenderTexture.GetTemporary(((Texture)source).width, ((Texture)source).height, 0, (RenderTextureFormat)7, (RenderTextureReadWrite)1);
			Graphics.Blit((Texture)(object)source, temporary);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			Texture2D val = new Texture2D(((Texture)source).width, ((Texture)source).height);
			val.ReadPixels(new Rect(0f, 0f, (float)((Texture)temporary).width, (float)((Texture)temporary).height), 0, 0);
			val.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return val;
		}

		public static Texture2D Flip(this Texture2D original, bool horizontal = true, bool vertical = false)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(((Texture)original).width, ((Texture)original).height);
			if (horizontal)
			{
				for (int i = 0; i < ((Texture)val).width; i++)
				{
					for (int j = 0; j < ((Texture)val).height; j++)
					{
						val.SetPixel(((Texture)val).width - i - 1, j, original.GetPixel(i, j));
					}
				}
			}
			if (vertical)
			{
				for (int k = 0; k < ((Texture)val).width; k++)
				{
					for (int l = 0; l < ((Texture)val).height; l++)
					{
						val.SetPixel(k, ((Texture)val).height - l - 1, original.GetPixel(k, l));
					}
				}
			}
			val.Apply();
			return val;
		}

		public static Texture2D GetCropped(this Texture2D original, Rect region)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			Color[] pixels = original.GetPixels((int)((Rect)(ref region)).x, (int)((Rect)(ref region)).y, (int)((Rect)(ref region)).width, (int)((Rect)(ref region)).height);
			Texture2D val = new Texture2D((int)((Rect)(ref region)).width, (int)((Rect)(ref region)).height);
			val.SetPixels(pixels);
			val.Apply();
			return val;
		}

		[Obsolete("Use GetCropped instead to get a texture of the new size")]
		public static Texture2D GetRegion(this Texture2D original, Rect region)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(((Texture)original).width, ((Texture)original).height);
			for (int i = (int)((Rect)(ref region)).xMin; i < (int)((Rect)(ref region)).xMax; i++)
			{
				for (int j = (int)((Rect)(ref region)).yMin; j < (int)((Rect)(ref region)).yMax; j++)
				{
					val.SetPixel(i, j, original.GetPixel(i, j));
				}
			}
			val.Apply();
			return val;
		}

		public static string getHash(this Texture2D tex)
		{
			byte[] buffer = ImageConversion.EncodeToPNG(tex);
			return BitConverter.ToString(SHA1.Create().ComputeHash(buffer)).Replace("-", string.Empty);
		}
	}
	public class CoroutineRunner : MonoBehaviour
	{
	}
	public static class CoroutineHelper
	{
		private static GameObject gameObject;

		private static CoroutineRunner runner;

		public static CoroutineRunner GetRunner()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			if ((Object)(object)runner == (Object)null)
			{
				if ((Object)(object)gameObject == (Object)null)
				{
					gameObject = new GameObject();
					Object.DontDestroyOnLoad((Object)(object)gameObject);
				}
				runner = gameObject.GetAddComponent<CoroutineRunner>();
			}
			return runner;
		}

		public static Coroutine WaitForFramesBeforeInvoke(int numFrames, Action codeToRun)
		{
			return ((MonoBehaviour)GetRunner()).StartCoroutine(WaitBeforeInvokeRoutine(numFrames, codeToRun));
		}

		public static Coroutine WaitForSecondsBeforeInvoke(float seconds, Action codeToRun)
		{
			return ((MonoBehaviour)GetRunner()).StartCoroutine(WaitBeforeInvokeRoutine(seconds, codeToRun));
		}

		private static IEnumerator WaitBeforeInvokeRoutine(int numFrames, Action codeToRun)
		{
			for (int i = 0; i < numFrames; i++)
			{
				yield return null;
			}
			codeToRun();
		}

		private static IEnumerator WaitBeforeInvokeRoutine(float seconds, Action codeToRun)
		{
			yield return (object)new WaitForSeconds(seconds);
			codeToRun();
		}
	}
	public class ObjectPoolManager<T>
	{
		public int poolSize;

		public List<T> pool = new List<T>();

		public List<bool> inUse = new List<bool>();

		protected Func<T> Generator;

		public ObjectPoolManager(int poolSize, Func<T> Generator)
		{
			this.poolSize = poolSize;
			this.Generator = Generator;
			Populate();
		}

		public void Populate()
		{
			if (pool.Count < poolSize)
			{
				for (int i = pool.Count; i < poolSize; i++)
				{
					pool.Add(Generator());
					inUse.Add(item: false);
				}
			}
		}

		public T PickFromPool()
		{
			int num = inUse.FindIndex((bool i) => !i);
			if (num == -1)
			{
				poolSize++;
				Populate();
				return PickFromPool();
			}
			inUse[num] = true;
			return pool[num];
		}

		public void RecycleToPool(T obj)
		{
			int num = pool.FindIndex((T i) => i.GetHashCode() == obj.GetHashCode());
			if (num == -1)
			{
				throw new ArgumentOutOfRangeException("Passed object was not a pooled object");
			}
			inUse[num] = false;
		}
	}
	public class AlertRangeMarker : MonoBehaviour
	{
		private bool processed;

		public bool isProcessed()
		{
			return processed;
		}

		public void setProcessed()
		{
			processed = true;
		}
	}
	internal class ChangeMeshColor : MonoBehaviour
	{
		private Material mat;

		public Color color;

		public void Start()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			MeshRenderer component = ((Component)this).gameObject.GetComponent<MeshRenderer>();
			if ((Object)(object)component != (Object)null)
			{
				mat = new Material(Shader.Find("tk2d/BlendVertexColor"));
				mat.SetTexture(Shader.PropertyToID("_MainTex"), (Texture)(object)TextureUtils.createTextureOfColor(2, 2, color));
				((Renderer)component).material = mat;
			}
		}

		public ChangeMeshColor()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			color = Color.black;
			((MonoBehaviour)this)..ctor();
		}
	}
	public class CustomEnemyMarker : MonoBehaviour
	{
		public int damageDealt = 1;

		public int hazardType = 1;

		public bool shadowDashHazard;

		public bool resetOnEnable;

		public bool manageHealth = true;

		public int initialHp = 100;

		public bool showHiteffects;

		public bool isInfected;

		public bool hasRecoil;

		public bool freezeInPlace;

		public bool stopVelocityXWhenRecoilingUp;

		public bool preventRecoilUp;

		public float recoilSpeedBase = 15f;

		public float recoilDuration = 0.1f;

		private bool processed;

		public bool isProcessed()
		{
			return processed;
		}

		public void setProcessed()
		{
			processed = true;
		}
	}
	public class DamageHeroMarker : MonoBehaviour
	{
		public int damageDealt = 1;

		public int hazardType = 1;

		public bool shadowDashHazard;

		public bool resetOnEnable;

		private bool processed;

		public bool isProcessed()
		{
			return processed;
		}

		public void setProcessed()
		{
			processed = true;
		}
	}
	public class SpriteRendererMaterial : MonoBehaviour
	{
		public bool enableFlash;

		private void Awake()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			SpriteRenderer component = ((Component)this).GetComponent<SpriteRenderer>();
			if ((Object)(object)component != (Object)null)
			{
				Shader val = null;
				val = ((!enableFlash) ? Shader.Find("Sprites/Default") : Core.spriteFlash);
				((Renderer)component).material = new Material(val);
			}
		}
	}
	public class CustomAnimationController : MonoBehaviour
	{
		public Animation anim;

		public Sprite[] sprites;

		public int currentFrame;

		public bool animating;

		private SpriteRenderer sr;

		private DateTime lastFrameChange;

		public void Start()
		{
			sr = ((Component)this).gameObject.GetAddComponent<SpriteRenderer>();
		}

		public void Init(Animation animation)
		{
			anim = animation;
			lastFrameChange = DateTime.MinValue;
			currentFrame = -1;
			animating = true;
			sprites = CustomAnimation.loadedSprites[anim];
		}

		public void Update()
		{
			if (!animating)
			{
				return;
			}
			_ = lastFrameChange;
			if ((DateTime.Now - lastFrameChange).TotalMilliseconds > (double)(1000f / anim.fps))
			{
				currentFrame++;
				if (currentFrame >= sprites.Length)
				{
					if (anim.loop)
					{
						currentFrame = 0;
					}
					else
					{
						currentFrame = sprites.Length - 1;
					}
				}
				lastFrameChange = DateTime.Now;
			}
			if ((Object)(object)sr == (Object)null)
			{
				sr = ((Component)this).gameObject.GetAddComponent<SpriteRenderer>();
			}
			sr.sprite = sprites[currentFrame];
		}
	}
	[Obsolete("CustomAnimationFrames is deprecated, it never really worked super well.")]
	public class CustomAnimationFrames : MonoBehaviour
	{
	}
	public class CustomArrowPromptBehaviour : MonoBehaviour
	{
		public GameObject prompt;

		public GameObject customLabel;

		public PromptMarker promptMarker;

		public Action OnPromptTrigger;

		public SetTextMeshProGameText setTextMeshProGameText;

		public string oldConvName;

		public string oldSheetName;

		public string PromptText = "Custom Prompt";

		public string languageKey = "customLabel_";

		public bool showing;

		private void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			ModHooks.LanguageGetHook += new LanguageGetProxy(LanguageGet);
		}

		public void OnTriggerEnter2D(Collider2D col)
		{
			if ((Object)(object)((Component)col).gameObject == (Object)(object)((Component)HeroController.instance).gameObject)
			{
				Show();
			}
		}

		public void OnTriggerExit2D(Collider2D col)
		{
			if ((Object)(object)((Component)col).gameObject == (Object)(object)((Component)HeroController.instance).gameObject)
			{
				Hide();
			}
		}

		public void Show()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (!showing)
			{
				if ((Object)(object)prompt == (Object)null)
				{
					prompt = CustomArrowPrompt.GetNewPrompt();
					prompt.transform.position = ((Component)this).gameObject.transform.position + new Vector3(0f, 2f, 0f);
					prompt.transform.SetParent(((Component)this).gameObject.transform, true);
					promptMarker = prompt.GetComponent<PromptMarker>();
					customLabel = prompt.Find("Sit");
					setTextMeshProGameText = customLabel.GetComponent<SetTextMeshProGameText>();
					languageKey = "customLabel_" + PromptText;
					oldConvName = setTextMeshProGameText.convName;
					oldSheetName = setTextMeshProGameText.sheetName;
					setTextMeshProGameText.convName = languageKey;
					setTextMeshProGameText.sheetName = languageKey;
					setTextMeshProGameText.UpdateText();
					((TMP_Text)customLabel.GetComponent<TextMeshPro>()).text = PromptText;
				}
				promptMarker.SetLabel("Sit");
				promptMarker.SetOwner(((Component)this).gameObject);
				promptMarker.Show();
			}
			showing = true;
		}

		public IEnumerator resetBeforeRecycle(SetTextMeshProGameText setTextMeshProGameTextOriginal)
		{
			yield return (object)new WaitForSeconds(2f);
			if ((Object)(object)setTextMeshProGameTextOriginal != (Object)null)
			{
				setTextMeshProGameTextOriginal.convName = oldConvName;
				setTextMeshProGameTextOriginal.sheetName = oldSheetName;
				setTextMeshProGameTextOriginal.UpdateText();
			}
		}

		public void Hide()
		{
			if (showing)
			{
				((MonoBehaviour)this).StartCoroutine(resetBeforeRecycle(setTextMeshProGameText));
				promptMarker.Hide();
			}
			showing = false;
		}

		public void Update()
		{
			if (showing && !GameManager.instance.isPaused && (((OneAxisInputControl)GameManager.instance.inputHandler.inputActions.up).WasPressed || ((OneAxisInputControl)GameManager.instance.inputHandler.inputActions.down).WasPressed) && OnPromptTrigger != null)
			{
				OnPromptTrigger();
				Hide();
			}
		}

		public string LanguageGet(string key, string sheet, string orig)
		{
			languageKey = "customLabel_" + PromptText;
			if (key == languageKey && sheet == languageKey)
			{
				return PromptText;
			}
			return orig;
		}
	}
	public class CustomEnemy : MonoBehaviour
	{
		public DamageHero dh;

		public Rigidbody2D rb;

		public IHitEffectReciever hitEffects;

		public SpriteRenderer sr;

		private Color UnInfected;

		private Color Infected;

		public Color FlashColor;

		private Coroutine _flashRoutine;

		private void Start()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Expected O, but got Unknown
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Expected O, but got Unknown
			if ((Object)(object)sr == (Object)null)
			{
				sr = ((Component)this).GetComponent<SpriteRenderer>();
			}
			if ((Object)(object)dh == (Object)null)
			{
				CustomEnemyMarker component = ((Component)this).gameObject.GetComponent<CustomEnemyMarker>();
				dh = ((Component)this).gameObject.AddComponent<DamageHero>();
				dh.damageDealt = component.damageDealt;
				dh.hazardType = component.hazardType;
				dh.shadowDashHazard = component.shadowDashHazard;
				dh.resetOnEnable = component.resetOnEnable;
				if (component.isInfected)
				{
					FlashColor = Infected;
				}
				else
				{
					FlashColor = UnInfected;
				}
			}
			if ((Object)(object)rb == (Object)null)
			{
				rb = ((Component)this).gameObject.GetComponent<Rigidbody2D>();
			}
			if (hitEffects == null)
			{
				hitEffects = (IHitEffectReciever)(object)((Component)this).gameObject.GetComponent<InfectedEnemyEffects>();
			}
			if (hitEffects == null)
			{
				hitEffects = (IHitEffectReciever)(object)((Component)this).gameObject.GetComponent<EnemyHitEffectsUninfected>();
			}
			InfectedEnemyEffects.RecieveHitEffect += new hook_RecieveHitEffect(ReceiveHit);
			EnemyHitEffectsUninfected.RecieveHitEffect += new hook_RecieveHitEffect(ReceiveHit);
			HealthManager.TakeDamage += new hook_TakeDamage(OnHealthManagerTakeDamage);
		}

		public void StartFlashColor(float time)
		{
			if ((Object)(object)((Renderer)sr).material.shader != (Object)(object)Core.spriteFlash)
			{
				((Loggable)Satchel.Instance).LogWarn("Cannot FlashSprite because the spriteFlash shader is not being used");
				return;
			}
			if (_flashRoutine != null)
			{
				((MonoBehaviour)this).StopCoroutine(_flashRoutine);
			}
			_flashRoutine = ((MonoBehaviour)this).StartCoroutine(Flash());
			IEnumerator Flash()
			{
				float flashAmount = 1f;
				Material material = ((Renderer)sr).material;
				while (flashAmount > 0f)
				{
					material.SetFloat("_FlashAmount", flashAmount);
					material.SetColor("_Color", FlashColor);
					material.SetFloat("_SelfIllum", 0.4f);
					flashAmount -= 0.01f;
					if (flashAmount <= 0f)
					{
						material.SetFloat("_FlashAmount", 0f);
						material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
						material.SetFloat("_SelfIllum", 0f);
					}
					yield return (object)new WaitForSeconds(0.01f * time);
				}
				yield return null;
			}
		}

		private void OnHealthManagerTakeDamage(orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (((Object)self).name == ((Object)((Component)this).gameObject).name && hitEffects != null)
			{
				hitEffects.RecieveHitEffect(hitInstance.Direction);
			}
			orig.Invoke(self, hitInstance);
		}

		private void ReceiveHit(orig_RecieveHitEffect orig, InfectedEnemyEffects self, float attack_direction)
		{
			orig.Invoke(self, attack_direction);
			if (!(((Object)self).name != ((Object)((Component)this).gameObject).name))
			{
				StartFlashColor(0.1f);
			}
		}

		private void ReceiveHit(orig_RecieveHitEffect orig, EnemyHitEffectsUninfected self, float attack_direction)
		{
			orig.Invoke(self, attack_direction);
			if (!(((Object)self).name != ((Object)((Component)this).gameObject).name))
			{
				StartFlashColor(0.1f);
			}
		}

		private void OnDestroy()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			InfectedEnemyEffects.RecieveHitEffect -= new hook_RecieveHitEffect(ReceiveHit);
			EnemyHitEffectsUninfected.RecieveHitEffect -= new hook_RecieveHitEffect(ReceiveHit);
			HealthManager.TakeDamage -= new hook_TakeDamage(OnHealthManagerTakeDamage);
		}

		public CustomEnemy()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			UnInfected = new Color(1f, 1f, 1f, 1f);
			Infected = new Color(1f, 0.5f, 0f, 1f);
			FlashColor = new Color(1f, 1f, 1f, 1f);
			((MonoBehaviour)this)..ctor();
		}
	}
	public class CustomBigItemGetBehaviour : MonoBehaviour
	{
		public string ItemName = "ITEM";

		public string Intro1;

		public string ButtonPress;

		public string Prompt1;

		public string Prompt2;

		public string languageKey = "SATCHEL_";

		public Func<PlayerAction> actionGet;

		public PlayMakerFSM dialogFsm;

		private float blnkWidth = 1.685f;

		private float blnkHeight = 0.6f;

		private float blnkFontMax = 9.5f;

		private float blnkFontMin = 4f;

		private float sqrWidth = 0.7f;

		private float sqrHeight = 0.8f;

		private float sqrFontMax = 5.5f;

		private float sqrFontMin = 3.35f;

		private float wideWidth = 2f;

		private float wideHeight = 0.7f;

		private float wideFontMax = 3f;

		private float wideFontMin = 2f;

		private void Start()
		{
		}

		public void SetItem(string ItemName, string Intro1, string ButtonPress, string Prompt1, string Prompt2, Sprite sprite, Func<PlayerAction> actionGet, Action Callback)
		{
			this.ItemName = ItemName;
			this.Intro1 = Intro1;
			this.ButtonPress = ButtonPress;
			this.Prompt1 = Prompt1;
			this.Prompt2 = Prompt2;
			this.actionGet = actionGet;
			dialogFsm = FSMUtility.LocateMyFSM(((Component)this).gameObject, "Msg Control");
			dialogFsm.GetVariable<FsmString>("Item").Value = "Dash";
			languageKey = "SATCHEL_" + ItemName + "_";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 2).sheetName.Value = languageKey + "UI";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 2).convName.Value = languageKey + "INV_NAME";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 4).sheetName.Value = languageKey + "Prompts";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 4).convName.Value = languageKey + "GET_ITEM_INTRO1";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 6).sheetName.Value = languageKey + "Prompts";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 6).convName.Value = languageKey + "BUTTON_DESC_PRESS";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 8).sheetName.Value = languageKey + "Prompts";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 8).convName.Value = languageKey + "GET_1";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 10).sheetName.Value = languageKey + "Prompts";
			dialogFsm.GetAction<GetLanguageString>("Set Dash", 10).convName.Value = languageKey + "GET_2";
			dialogFsm.RemoveAction("Set Dash", 1);
			dialogFsm.RemoveAction("Set Dash", 0);
			if ((Object)(object)sprite != (Object)null)
			{
				Transform val = ((Component)this).gameObject.transform.Find("Icon");
				if (Object.op_Implicit((Object)(object)val))
				{
					SpriteRenderer component = ((Component)val).GetComponent<SpriteRenderer>();
					if (Object.op_Implicit((Object)(object)component))
					{
						component.sprite = sprite;
					}
				}
			}
			if (Callback != null)
			{
				dialogFsm.InsertCustomAction("Done", (Action)delegate
				{
					Callback();
					Object.Destroy((Object)(object)((Component)this).gameObject);
				}, 1);
			}
		}

		public IEnumerator updateButtonIcon(PlayerAction action)
		{
			yield return (object)new WaitForSeconds(1f);
			ButtonSkin buttonSkinFor = UIManager.instance.uiButtonSkins.GetButtonSkinFor(action);
			ActionButtonIcon componentInChildren = ((Component)this).gameObject.GetComponentInChildren<ActionButtonIcon>();
			if ((Object)(object)componentInChildren == (Object)null)
			{
				((Loggable)Satchel.Instance).Log("[Satchel] Couldn't get ActionButtonIcon skin for " + (object)action);
			}
			if (buttonSkinFor == null)
			{
				((Loggable)Satchel.Instance).Log("[Satchel] Couldn't get button skin for " + (object)action);
			}
			TextMeshPro label = ((ActionButtonIconBase)componentInChildren).label;
			TextContainer textContainer = ((ActionButtonIconBase)componentInChildren).textContainer;
			GameObject gameObject = ((Component)componentInChildren).gameObject;
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			Object.Destroy((Object)(object)componentInChildren);
			component.sprite = buttonSkinFor.sprite;
			if ((Object)(object)textContainer != (Object)null)
			{
				if ((int)buttonSkinFor.skinType == 0)
				{
					textContainer.width = blnkWidth;
					textContainer.height = blnkHeight;
				}
				else if ((int)buttonSkinFor.skinType == 1)
				{
					textContainer.width = sqrWidth;
					textContainer.height = sqrHeight;
				}
				else if ((int)buttonSkinFor.skinType == 2)
				{
					textContainer.width = wideWidth;
					textContainer.height = wideHeight;
				}
			}
			if ((Object)(object)label != (Object)null)
			{
				if ((int)buttonSkinFor.skinType == 0)
				{
					((TMP_Text)label).fontSizeMin = blnkFontMin;
					((TMP_Text)label).fontSizeMax = blnkFontMax;
				}
				else if ((int)buttonSkinFor.skinType == 1)
				{
					((TMP_Text)label).fontSizeMin = sqrFontMin;
					((TMP_Text)label).fontSizeMax = sqrFontMax;
				}
				else if ((int)buttonSkinFor.skinType == 2)
				{
					((TMP_Text)label).fontSizeMin = wideFontMin;
					((TMP_Text)label).fontSizeMax = wideFontMax;
					Transform transform = gameObject.transform;
					transform.position += new Vector3(0.5f, 0f, 0f);
				}
				((TMP_Text)label).text = buttonSkinFor.symbol;
			}
		}

		public void Show()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			ModHooks.LanguageGetHook += new LanguageGetProxy(LanguageGet);
			((Component)this).gameObject.SetActive(true);
			if (actionGet() != null)
			{
				((MonoBehaviour)this).StartCoroutine(updateButtonIcon(actionGet()));
			}
			else
			{
				((Component)((Component)this).gameObject.GetComponentInChildren<ActionButtonIcon>()).gameObject.SetActive(false);
			}
		}

		public void OnDestroy()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			ModHooks.LanguageGetHook -= new LanguageGetProxy(LanguageGet);
		}

		public string LanguageGet(string key, string sheet, string orig)
		{
			languageKey = "SATCHEL_" + ItemName + "_";
			if (sheet == languageKey + "UI" && key == languageKey + "INV_NAME")
			{
				return ItemName;
			}
			if (sheet == languageKey + "Prompts")
			{
				if (key == languageKey + "GET_ITEM_INTRO1")
				{
					return Intro1;
				}
				if (key == languageKey + "BUTTON_DESC_PRESS")
				{
					return ButtonPress;
				}
				if (key == languageKey + "GET_1")
				{
					return Prompt1;
				}
				if (key == languageKey + "GET_2")
				{
					return Prompt2;
				}
			}
			return orig;
		}
	}
	public class CustomData : MonoBehaviour
	{
		private Dictionary<string, object> dataStore = new Dictionary<string, object>();

		public void setData(string key, object data)
		{
			dataStore[key] = data;
		}

		public object getData(string key)
		{
			dataStore.TryGetValue(key, out var value);
			return value;
		}
	}
	public static class CustomArrowPrompt
	{
		public static GameObject ArrowPromptPrefab;

		public static void Prepare(GameObject CardPrefab)
		{
			PlayMakerFSM val = FSMUtility.LocateMyFSM(CardPrefab, "npc_control");
			if ((Object)(object)val != (Object)null)
			{
				ArrowPromptPrefab = val.GetAction<ShowPromptMarker>("In Range", 0).prefab.Value;
			}
		}

		public static GameObject GetNewPrompt()
		{
			return Object.Instantiate<GameObject>(ArrowPromptPrefab);
		}

		public static void GetAddCustomArrowPrompt(this GameObject go, Action OnPromptTrigger)
		{
			go.GetAddCustomArrowPrompt("Listen", OnPromptTrigger);
		}

		public static void GetAddCustomArrowPrompt(this GameObject go, string PromptText, Action OnPromptTrigger)
		{
			CustomArrowPromptBehaviour addComponent = go.GetAddComponent<CustomArrowPromptBehaviour>();
			addComponent.PromptText = PromptText;
			addComponent.OnPromptTrigger = OnPromptTrigger;
		}
	}
	public class CustomDialogueManager
	{
		public string prefix;

		public Dictionary<string, string> Conversations;

		public bool isDialogueBoxOpen;

		public bool takenControl;

		public string currentDialog;

		public GameObject DialogManager;

		public GameObject DialogTextBox;

		public GameObject DialogBox;

		public DialogueBox dboxComponent;

		public PlayMakerFSM conversationControl;

		public FsmEventTarget boxEventTarget;

		private List<Action<string, int>> PageCompletionCallback;

		private List<Action<string>> ConversationCompletionCallback;

		private DialogueBox GetDialogueBox()
		{
			if ((Object)(object)dboxComponent == (Object)null)
			{
				conversationControl = FSMUtility.LocateMyFSM(DialogManager, "Conversation Control");
				boxEventTarget = conversationControl.GetAction<SendEventByName>("Box Up", 0).eventTarget;
				DialogTextBox = conversationControl.GetAction<CallMethodProper>("Greet", 1).gameObject.GameObject.Value;
				dboxComponent = DialogTextBox.GetComponent<DialogueBox>();
				addEventsInFSM();
			}
			return dboxComponent;
		}

		public CustomDialogueManager(GameObject cardPrefab)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			prefix = "SatchelCustomDialogue";
			Conversations = new Dictionary<string, string>();
			PageCompletionCallback = new List<Action<string, int>>();
			ConversationCompletionCallback = new List<Action<string>>();
			base..ctor();
			DialogManager = cardPrefab.createCustomDialogFromPrefab();
			DialogManager.SetActive(true);
			ModHooks.LanguageGetHook += new LanguageGetProxy(LanguageGet);
		}

		private void addEventsInFSM()
		{
			PlayMakerFSM val = FSMUtility.LocateMyFSM(DialogTextBox, "Dialogue Page Control");
			if ((Object)(object)val != (Object)null)
			{
				val.InsertCustomAction("Page End", (Action)delegate
				{
					EndPage();
				}, 0);
				val.InsertCustomAction("End Conversation", (Action)delegate
				{
					EndConversation();
				}, 5);
			}
		}

		public void OnEndPage(Action<string, int> callback)
		{
			PageCompletionCallback.Add(callback);
		}

		private void EndPage()
		{
			foreach (Action<string, int> item in PageCompletionCallback)
			{
				item(currentDialog, dboxComponent.currentPage);
			}
		}

		public void OnEndConversation(Action<string> callback)
		{
			ConversationCompletionCallback.Add(callback);
		}

		private void EndConversation()
		{
			foreach (Action<string> item in ConversationCompletionCallback)
			{
				item(currentDialog);
			}
			conversationControl.Fsm.Event(boxEventTarget, "BOX DOWN");
			isDialogueBoxOpen = false;
			if (takenControl)
			{
				HeroController.instance.RegainControl();
				takenControl = false;
			}
		}

		public void AddConversation(string name, string conversation)
		{
			Conversations[prefix + name] = conversation;
		}

		public void ShowDialogue(string name, bool takeControl = true)
		{
			if (isDialogueBoxOpen)
			{
				return;
			}
			currentDialog = name;
			if ((Object)(object)dboxComponent == (Object)null)
			{
				dboxComponent = GetDialogueBox();
			}
			if ((Object)(object)dboxComponent != (Object)null)
			{
				dboxComponent.StartConversation(prefix + name, prefix);
				conversationControl.Fsm.Event(boxEventTarget, "BOX UP");
				isDialogueBoxOpen = true;
				if (takeControl)
				{
					takenControl = true;
					HeroController.instance.RelinquishControl();
				}
			}
			else
			{
				((Loggable)Satchel.Instance).Log("Component is null");
			}
		}

		private string LanguageGet(string key, string sheet, string orig)
		{
			if (key.StartsWith(prefix) && sheet.StartsWith(prefix) && Conversations.TryGetValue(key, out var value))
			{
				return value;
			}
			return orig;
		}
	}
	public class CustomDreamNailManager
	{
		public Dictionary<string, string> DNdialog;

		public CustomDreamNailManager()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			DNdialog = new Dictionary<string, string>();
			base..ctor();
			ModHooks.LanguageGetHook += new LanguageGetProxy(LanguageGet);
		}

		public Convo SetText(Convo conv, string text)
		{
			conv.Amount++;
			DNdialog[$"{conv.Id}_{conv.Amount}"] = text;
			return conv;
		}

		private string LanguageGet(string key, string sheet, string orig)
		{
			if (key.StartsWith(Convo.prefix))
			{
				string key2 = key.Replace(Convo.prefix, "");
				if (DNdialog.TryGetValue(key2, out var value) && value != "" && value != null)
				{
					return value;
				}
			}
			return orig;
		}

		private void OnDestroy()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			ModHooks.LanguageGetHook -= new LanguageGetProxy(LanguageGet);
		}
	}
	public class CustomEnemyManager
	{
		private GameObject defaultInfectedEnemyPrefab;

		private GameObject defaultUnInfectedEnemyPrefab;

		private List<GameObject> customEnemies = new List<GameObject>();

		private List<Action<List<GameObject>>> enemyCreatedCallbacks = new List<Action<List<GameObject>>>();

		public CustomEnemyManager()
		{
			SceneManager.activeSceneChanged += OnSceneChanged;
		}

		public void Setup(GameObject InfectedEnemyPrefab, GameObject UnInfectedEnemyPrefab)
		{
			if ((Object)(object)InfectedEnemyPrefab != (Object)null)
			{
				defaultInfectedEnemyPrefab = InfectedEnemyPrefab;
			}
			if ((Object)(object)UnInfectedEnemyPrefab != (Object)null)
			{
				defaultUnInfectedEnemyPrefab = UnInfectedEnemyPrefab;
			}
		}

		public void AddCallbackForNewEnemies(Action<List<GameObject>> callback)
		{
			enemyCreatedCallbacks.Add(callback);
		}

		public GameObject Add()
		{
			return null;
		}

		private void OnSceneChanged(Scene from, Scene to)
		{
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Expected O, but got Unknown
			customEnemies = new List<GameObject>();
			CustomEnemyMarker[] array = Object.FindObjectsOfType<CustomEnemyMarker>();
			foreach (CustomEnemyMarker enemy in array)
			{
				if (((Object)(object)enemy == (Object)null && (Object)(object)((Component)enemy).gameObject == (Object)null) || enemy.isProcessed())
				{
					continue;
				}
				enemy.setProcessed();
				customEnemies.Add(((Component)enemy).gameObject);
				((Component)enemy).gameObject.AddComponent<CustomEnemy>();
				((Component)enemy).gameObject.activateAlertRanges();
				if (!enemy.manageHealth)
				{
					continue;
				}
				if (enemy.hasRecoil)
				{
					((Component)enemy).gameObject.addRecoil(enemy.freezeInPlace, enemy.stopVelocityXWhenRecoilingUp, enemy.preventRecoilUp, enemy.recoilSpeedBase, enemy.recoilDuration);
				}
				if (enemy.isInfected)
				{
					EnemyUtils.copyHealthManagerFields(defaultInfectedEnemyPrefab.GetComponent<HealthManager>(), ((Component)enemy).gameObject.GetAddComponent<HealthManager>());
					if (enemy.showHiteffects)
					{
						EnemyUtils.copyEnemyPrefabFields(defaultInfectedEnemyPrefab, ((Component)enemy).gameObject, enemy.manageHealth, enemy.isInfected);
					}
				}
				else
				{
					EnemyUtils.copyHealthManagerFields(defaultUnInfectedEnemyPrefab.GetComponent<HealthManager>(), ((Component)enemy).gameObject.GetAddComponent<HealthManager>());
					if (enemy.showHiteffects)
					{
						EnemyUtils.copyEnemyPrefabFields(defaultUnInfectedEnemyPrefab, ((Component)enemy).gameObject, enemy.manageHealth, enemy.isInfected);
					}
				}
				((Component)enemy).gameObject.manageHealth(enemy.initialHp).OnDeath += (DeathEvent)delegate
				{
					((Component)enemy).gameObject.layer = 26;
				};
			}
			if (customEnemies.Count <= 0)
			{
				return;
			}
			foreach (Action<List<GameObject>> enemyCreatedCallback in enemyCreatedCallbacks)
			{
				enemyCreatedCallback(customEnemies);
			}
		}
	}
	public class customMapZone
	{
		public string ZoneName;

		public string mapZoneTitle;

		public List<string> sceneNames = new List<string>();

		public Dictionary<string, customMap> scenes = new Dictionary<string, customMap>();

		public Func<bool> hasMap;

		public Func<bool> hasFullMap;

		public GameObject areaCustomMap;

		public Vector2 position;

		public Vector2 titleOffset;
	}
	public class customMap
	{
		public string ZoneName;

		public string sceneName;

		public string sceneTitle;

		public Func<bool> hasMap;

		public Vector2 position;

		public Vector2 titleOffset;

		public Func<Sprite> GetSprite;
	}
	public class CustomMapManager
	{
		private Dictionary<string, customMap> Maps;

		private Dictionary<string, customMapZone> Zones;

		private List<string> MapAreas;

		private GameMap gameMapComponent;

		private GameObject AreaNameTxt;

		public CustomMapManager()
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected O, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Expected O, but got Unknown
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Expected O, but got Unknown
			Maps = new Dictionary<string, customMap>();
			Zones = new Dictionary<string, customMapZone>();
			MapAreas = new List<string>
			{
				"mapCrossroads", "mapGreenpath", "mapFogCanyon", "mapRoyalGardens", "mapFungalWastes", "mapCity", "mapWaterways", "mapMines", "mapDeepnest", "mapCliffs",
				"mapOutskirts", "mapRestingGrounds", "mapAbyss"
			};
			base..ctor();
			GameManager.SetGameMap += new hook_SetGameMap(OnGameManagerSetGameMap);
			GameMap.WorldMap += new hook_WorldMap(NewWorldMap);
			GameMap.CloseQuickMap += new hook_CloseQuickMap(NewCloseQuickMap);
			GameMap.PositionCompass += new hook_PositionCompass(NewPositionCompass);
			PlayerDataBoolTest.OnEnter += new hook_OnEnter(HandlePlayerDataBoolTest);
			ModHooks.LanguageGetHook += new LanguageGetProxy(LanguageGet);
		}

		public void AddZone(customMapZone newZone)
		{
			Zones.Add(newZone.ZoneName, newZone);
		}

		public void AddScene(customMap map)
		{
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			Maps.Add(map.sceneName, map);
			if (Zones.TryGetValue(map.ZoneName, out var value))
			{
				if (!value.sceneNames.Contains(map.sceneName))
				{
					value.sceneNames.Add(map.sceneName);
				}
				value.scenes[map.sceneName] = map;
			}
			else
			{
				value = new customMapZone
				{
					ZoneName = map.ZoneName,
					mapZoneTitle = map.ZoneName,
					sceneNames = new List<string> { map.sceneName },
					scenes = new Dictionary<string, customMap>(),
					hasMap = map.hasMap,
					position = new Vector2(map.position.x, map.position.y),
					titleOffset = map.titleOffset
				};
				map.position = new Vector2(0f, 0f);
				value.scenes.Add(map.sceneName, map);
				Zones.Add(value.ZoneName, value);
			}
			((MonoBehaviour)CoroutineHelper.GetRunner()).StartCoroutine(generateCustomMap(map));
		}

		public void Remove(string sceneName)
		{
		}

		private GameObject GetAreaNameTxt()
		{
			if ((Object)(object)AreaNameTxt == (Object)null)
			{
				PlayMakerFSM val = FSMUtility.LocateMyFSM(GameObject.Find("Quick Map"), "Quick Map");
				_ = val.FsmVariables;
				AreaNameTxt = val.GetAction<SetTextMeshProText>("Cliffs", 2).gameObject.GameObject.Value;
			}
			return AreaNameTxt;
		}

		private void AddSceneToZone(customMapZone Zone, customMap map)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			((Loggable)Satchel.Instance).Log("processing scene " + map.sceneName + " for map");
			customMapZone customMapZone2 = Zones[map.ZoneName];
			float z = gameMapComponent.areaCliffs.transform.GetChild(6).localPosition.z;
			Material material = Object.Instantiate<Material>(((Renderer)((Component)gameMapComponent.areaCliffs.transform.GetChild(1)).GetComponent<SpriteRenderer>()).material);
			GameObject val = new GameObject(map.sceneName);
			val.transform.SetParent(customMapZone2.areaCustomMap.transform);
			((Object)val).name = map.sceneName;
			val.transform.localPosition = new Vector3(map.position.x, map.position.y, z);
			val.layer = 5;
			val.transform.localScale = Vector3.one;
			SpriteRenderer obj = val.AddComponent<SpriteRenderer>();
			((Renderer)obj).material = material;
			obj.sprite = map.GetSprite();
			((Renderer)obj).sortingLayerID = 629535577;
			((Renderer)obj).sortingOrder = 0;
			val.SetActive(true);
		}

		private IEnumerator generateCustomMap(customMap map)
		{
			yield return (object)new WaitWhile((Func<bool>)(() => (Object)(object)gameMapComponent == (Object)null));
			customMapZone customMapZone2 = Zones[map.ZoneName];
			if ((Object)(object)customMapZone2.areaCustomMap == (Object)null)
			{
				GameObject val = Object.Instantiate<GameObject>(((Component)gameMapComponent.areaCliffs.transform.GetChild(0)).gameObject);
				val.SetActive(false);
				customMapZone2.areaCustomMap = Object.Instantiate<GameObject>(gameMapComponent.areaCliffs, ((Component)gameMapComponent).transform);
				customMapZone2.areaCustomMap.SetActive(true);
				for (int num = 0; num < customMapZone2.areaCustomMap.transform.childCount; num++)
				{
					Object.Destroy((Object)(object)((Component)customMapZone2.areaCustomMap.transform.GetChild(num)).gameObject);
				}
				((Object)customMapZone2.areaCustomMap).name = "dc_custom_mapzone_" + customMapZone2.ZoneName;
				customMapZone2.areaCustomMap.layer = 5;
				customMapZone2.areaCustomMap.transform.localScale = Vector3.one;
				customMapZone2.areaCustomMap.transform.localPosition = new Vector3(customMapZone2.position.x, customMapZone2.position.y, gameMapComponent.areaCliffs.transform.localPosition.z);
				GameObject val2 = Object.Instantiate<GameObject>(val, customMapZone2.areaCustomMap.transform);
				val2.transform.position = new Vector3(0f, 0f, 0f);
				val2.transform.localPosition = new Vector3(5f + customMapZone2.titleOffset.x, -0.5f + customMapZone2.titleOffset.y, val2.transform.localPosition.z - 0.002f);
				val2.GetComponent<SetTextMeshProGameText>().convName = "dc_custom_mapzone_" + customMapZone2.ZoneName;
				val2.SetActive(true);
				customMapZone2.areaCustomMap.SetActive(false);
				Object.DontDestroyOnLoad((Object)(object)customMapZone2.areaCustomMap);
			}
			AddSceneToZone(customMapZone2, map);
		}

		protected void EditQuickMapFsm(GameObject gameMapGameObject)
		{
			PlayMakerFSM obj = FSMUtility.LocateMyFSM(GameObject.Find("Quick Map"), "Quick Map");
			_ = obj.FsmVariables;
			CustomFsmAction action = new CustomFsmAction
			{
				method = delegate
				{
					CheckOpenCustomQuickMap(gameMapGameObject);
				}
			};
			obj.InsertAction("Crossroads", (FsmStateAction)(object)action, 7);
			obj.InsertAction("Dirtmouth", (FsmStateAction)(object)action, 5);
			obj.InsertAction("Greenpath", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Cliffs", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Fungal Wastes", (FsmStateAction)(object)action, 6);
			obj.InsertAction("City", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Mines", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Resting Grounds", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Fog Canyon", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Royal Gardens", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Deepnest", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Waterways", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Abyss", (FsmStateAction)(object)action, 6);
			obj.InsertAction("Outskirts", (FsmStateAction)(object)action, 7);
		}

		private void OpenCustomMap(GameMap gm)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			Scene currentScene = SceneUtils.getCurrentScene();
			string name = ((Scene)(ref currentScene)).name;
			if (!Maps.TryGetValue(name, out var value))
			{
				return;
			}
			gm.areaGreenpath.SetActive(false);
			gm.areaAncientBasin.SetActive(false);
			gm.areaCity.SetActive(false);
			gm.areaCliffs.SetActive(false);
			gm.areaCrossroads.SetActive(false);
			gm.areaCrystalPeak.SetActive(false);
			gm.areaDeepnest.SetActive(false);
			gm.areaFogCanyon.SetActive(false);
			gm.areaFungalWastes.SetActive(false);
			gm.areaGreenpath.SetActive(false);
			gm.areaKingdomsEdge.SetActive(false);
			gm.areaQueensGardens.SetActive(false);
			gm.areaRestingGrounds.SetActive(false);
			gm.areaDirtmouth.SetActive(false);
			gm.areaWaterways.SetActive(false);
			resetMapVisibility();
			foreach (KeyValuePair<string, customMapZone> zone in Zones)
			{
				if (zone.Value.sceneNames.Contains(name))
				{
					zone.Value.areaCustomMap.SetActive(true);
				}
			}
			((TMP_Text)GetAreaNameTxt().GetComponent<TextMeshPro>()).text = ((Zones[value.ZoneName].mapZoneTitle != "") ? Zones[value.ZoneName].mapZoneTitle : Zones[value.ZoneName].ZoneName);
			MoveMapTo(gm, new Vector2(Zones[value.ZoneName].position.x * -1.5365f + value.position.x, Zones[value.ZoneName].position.y * -1.5365f + value.position.y));
		}

		private void MoveMapTo(GameMap gm, Vector2 point)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			((Component)gm).gameObject.transform.localPosition = new Vector3(point.x, point.y, 18f);
		}

		private void CheckOpenCustomQuickMap(GameObject go)
		{
			OpenCustomMap(go.GetComponent<GameMap>());
		}

		private string LanguageGet(string key, string sheet, string orig)
		{
			if (key.StartsWith("dc_custom_map_"))
			{
				string key2 = key.Replace("dc_custom_map_", "");
				if (Maps.TryGetValue(key2, out var value))
				{
					if (!(value.sceneTitle != ""))
					{
						return value.sceneName;
					}
					return value.sceneTitle;
				}
			}
			if (key.StartsWith("dc_custom_mapzone_"))
			{
				string key3 = key.Replace("dc_custom_mapzone_", "");
				if (Zones.TryGetValue(key3, out var value2))
				{
					if (!(value2.mapZoneTitle != ""))
					{
						return value2.ZoneName;
					}
					return value2.mapZoneTitle;
				}
			}
			return orig;
		}

		private void HandlePlayerDataBoolTest(orig_OnEnter orig, PlayerDataBoolTest self)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (MapAreas.Contains(self.boolName.Value))
			{
				Dictionary<string, customMap> maps = Maps;
				Scene currentScene = SceneUtils.getCurrentScene();
				if (maps.TryGetValue(((Scene)(ref currentScene)).name, out var value))
				{
					if (value.hasMap() || Zones[value.ZoneName].hasFullMap())
					{
						((FsmStateAction)self).Fsm.Event(self.isTrue);
					}
					else
					{
						((FsmStateAction)self).Fsm.Event(self.isFalse);
					}
					return;
				}
			}
			orig.Invoke(self);
		}

		private void OnGameManagerSetGameMap(orig_SetGameMap orig, GameManager self, GameObject gameMapGo)
		{
			Object.DontDestroyOnLoad((Object)(object)gameMapGo);
			gameMapComponent = gameMapGo.GetComponent<GameMap>();
			EditQuickMapFsm(gameMapGo);
			orig.Invoke(self, gameMapGo);
		}

		protected void NewWorldMap(orig_WorldMap orig, GameMap self)
		{
			orig.Invoke(self);
			self.panMinX = float.MinValue;
			self.panMaxX = float.MaxValue;
			self.panMinY = float.MinValue;
			self.panMaxY = float.MaxValue;
			if (!PlayerData.instance.GetBool("equippedCharm_2"))
			{
				return;
			}
			foreach (KeyValuePair<string, customMapZone> zone in Zones)
			{
				zone.Value.areaCustomMap.SetActive(zone.Value.hasMap());
			}
		}

		private void resetMapVisibility()
		{
			foreach (KeyValuePair<string, customMapZone> zone in Zones)
			{
				foreach (string sceneName in zone.Value.sceneNames)
				{
					zone.Value.areaCustomMap.FindGameObjectInChildren(sceneName).SetActive(zone.Value.scenes[sceneName].hasMap() || zone.Value.hasFullMap());
				}
				zone.Value.areaCustomMap.SetActive(false);
			}
		}

		protected void NewCloseQuickMap(orig_CloseQuickMap orig, GameMap self)
		{
			orig.Invoke(self);
			resetMapVisibility();
		}

		protected void NewPositionCompass(orig_PositionCompass orig, GameMap self, bool posShade)
		{
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_069f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = null;
			string text = ReflectionHelper.GetField<GameMap, GameManager>(self, "gm").GetCurrentMapZone();
			string text2;
			if (!self.inRoom)
			{
				text2 = ReflectionHelper.GetField<GameMap, GameManager>(self, "gm").sceneName;
			}
			else
			{
				text = self.doorMapZone;
				text2 = self.doorScene;
			}
			switch (text)
			{
			case "ABYSS":
				val = self.areaAncientBasin;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "CITY":
			case "KINGS_STATION":
			case "SOUL_SOCIETY":
			case "LURIENS_TOWER":
				val = self.areaCity;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "CLIFFS":
				val = self.areaCliffs;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "CROSSROADS":
			case "SHAMAN_TEMPLE":
				val = self.areaCrossroads;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "MINES":
				val = self.areaCrystalPeak;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "DEEPNEST":
			case "BEASTS_DEN":
				val = self.areaDeepnest;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "FOG_CANYON":
			case "MONOMON_ARCHIVE":
				val = self.areaFogCanyon;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "WASTES":
			case "QUEENS_STATION":
				val = self.areaFungalWastes;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "GREEN_PATH":
				val = self.areaGreenpath;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "OUTSKIRTS":
			case "HIVE":
			case "COLOSSEUM":
				val = self.areaKingdomsEdge;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "ROYAL_GARDENS":
				val = self.areaQueensGardens;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "RESTING_GROUNDS":
				val = self.areaRestingGrounds;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "TOWN":
			case "KINGS_PASS":
				val = self.areaDirtmouth;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			case "WATERWAYS":
			case "GODSEEKER_WASTE":
				val = self.areaWaterways;
				self.currentScene = val.FindGameObjectInChildren(text2);
				break;
			}
			((Component)self).gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			if (Maps.TryGetValue(text2, out var value))
			{
				val = Zones[value.ZoneName].areaCustomMap;
				self.currentScene = val.FindGameObjectInChildren(text2);
			}
			if ((Object)(object)self.currentScene != (Object)null)
			{
				self.currentScenePos = new Vector3(self.currentScene.transform.localPosition.x + val.transform.localPosition.x, self.currentScene.transform.localPosition.y + val.transform.localPosition.y, 0f);
				if (!posShade && !ReflectionHelper.GetField<GameMap, bool>(self, "posGate"))
				{
					if (ReflectionHelper.GetField<GameMap, PlayerData>(self, "pd").GetBool("equippedCharm_2"))
					{
						ReflectionHelper.SetField<GameMap, bool>(self, "displayingCompass", true);
						self.compassIcon.SetActive(true);
					}
					else
					{
						ReflectionHelper.SetField<GameMap, bool>(self, "displayingCompass", false);
						self.compassIcon.SetActive(false);
					}
				}
				if (posShade)
				{
					if (!self.inRoom)
					{
						self.shadeMarker.transform.localPosition = new Vector3(self.currentScenePos.x, self.currentScenePos.y, 0f);
					}
					else
					{
						float x = self.currentScenePos.x;
						Rect rect = self.currentScene.GetComponent<SpriteRenderer>().sprite.rect;
						float num = x - ((Rect)(ref rect)).size.x / 100f / 2f;
						float num2 = (self.doorX + self.doorOriginOffsetX) / self.doorSceneWidth;
						rect = self.currentScene.GetComponent<SpriteRenderer>().sprite.rect;
						float num3 = num + num2 * (((Rect)(ref rect)).size.x / 100f * ((Component)self).transform.localScale.x) / ((Component)self).transform.localScale.x;
						float y = self.currentScenePos.y;
						rect = self.currentScene.GetComponent<SpriteRenderer>().sprite.rect;
						float num4 = y - ((Rect)(ref rect)).size.y / 100f / 2f;
						float num5 = (self.doorY + self.doorOriginOffsetY) / self.doorSceneHeight;
						rect = self.currentScene.GetComponent<SpriteRenderer>().sprite.rect;
						float num6 = num4 + num5 * (((Rect)(ref rect)).size.y / 100f * ((Component)self).transform.localScale.y) / ((Component)self).transform.localScale.y;
						self.shadeMarker.transform.localPosition = new Vector3(num3, num6, 0f);
					}
					ReflectionHelper.GetField<GameMap, PlayerData>(self, "pd").SetVector3("shadeMapPos", new Vector3(self.currentScenePos.x, self.currentScenePos.y, 0f));
				}
				if (ReflectionHelper.GetField<GameMap, bool>(self, "posGate"))
				{
					self.dreamGateMarker.transform.localPosition = new Vector3(self.currentScenePos.x, self.currentScenePos.y, 0f);
					ReflectionHelper.GetField<GameMap, PlayerData>(self, "pd").SetVector3("dreamgateMapPos", new Vector3(self.currentScenePos.x, self.currentScenePos.y, 0f));
				}
			}
			else
			{
				((Loggable)Satchel.Instance).Log("Couldn't find current scene object!");
				if (posShade)
				{
					ReflectionHelper.GetField<GameMap, PlayerData>(self, "pd").SetVector3("shadeMapPos", new Vector3(-10000f, -10000f, 0f));
					self.shadeMarker.transform.localPosition = ReflectionHelper.GetField<GameMap, PlayerData>(self, "pd").GetVector3("shadeMapPos");
				}
			}
		}
	}
	public class CustomSaveSlotsManager
	{
		public Dictionary<int, string> RespawnScene;

		public Dictionary<string, SceneUtils.CustomSaveSlotParams> CustomSaveSlots;

		public CustomSaveSlotsManager()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			RespawnScene = new Dictionary<int, string>();
			CustomSaveSlots = new Dictionary<string, SceneUtils.CustomSaveSlotParams>();
			base..ctor();
			GameManager.GetSaveStatsForSlot += new hook_GetSaveStatsForSlot(OnGetSaveStatsForSlot);
			SaveSlotButton.PresentSaveSlot += new hook_PresentSaveSlot(OnPresetSaveSlot);
		}

		public void Add(SceneUtils.CustomSaveSlotParams saveSlot)
		{
			CustomSaveSlots[saveSlot.sceneName] = saveSlot;
		}

		private void OnPresetSaveSlot(orig_PresentSaveSlot orig, SaveSlotButton self, SaveStats saveStats)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			orig.Invoke(self, saveStats);
			if (RespawnScene.TryGetValue(PlayerDataUtils.ConvertSlotToNumber(self.saveSlot), out var value) && CustomSaveSlots.TryGetValue(value, out var value2))
			{
				if (value2.sceneTitle != null)
				{
					self.locationText.text = value2.sceneTitle;
				}
				if ((Object)(object)value2.background != (Object)null)
				{
					self.background.sprite = value2.background;
				}
			}
		}

		private void OnGetSaveStatsForSlot(orig_GetSaveStatsForSlot orig, GameManager self, int SaveSlot, Action<SaveStats> Callback)
		{
			PlayerDataUtils.GetPlayerDataForSlot(SaveSlot, delegate(PlayerData pd)
			{
				RespawnScene[SaveSlot] = pd.respawnScene;
			});
			orig.Invoke(self, SaveSlot, Callback);
		}
	}
	public class SceneLoadedEventArgs : EventArgs
	{
	}
	public class CustomScene
	{
		public string sceneName;

		public float width;

		public float height;

		public GameObject TileMap;

		public GameObject SceneManager;

		public SceneManager sceneManagerComponent;

		public SceneUtils.CustomSceneManagerSettings settings;

		public List<SceneUtils.BenchParams> Benches;

		public List<SceneUtils.GatewayParams> gatesToScene;

		public List<SceneUtils.GatewayParams> gatesFromScene;

		public event EventHandler<SceneLoadedEventArgs> OnLoaded;

		public CustomScene()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			sceneName = "unknown";
			width = 32f;
			height = 32f;
			Benches = new List<SceneUtils.BenchParams>();
			gatesToScene = new List<SceneUtils.GatewayParams>();
			gatesFromScene = new List<SceneUtils.GatewayParams>();
			base..ctor();
			GameManager.RefreshTilemapInfo += new hook_RefreshTilemapInfo(OnGameManagerRefreshTilemapInfo);
			GameManager.EnterHero += new hook_EnterHero(OnGameManagerEnterHero);
			SceneManager.activeSceneChanged += OnSceneChanged;
		}

		public void Config(float width, float height, SceneUtils.CustomSceneManagerSettings settings)
		{
			this.width = width;
			this.height = height;
			this.settings = settings;
		}

		public void AddGateway(SceneUtils.GatewayParams gateway)
		{
			if (gateway.fromScene == sceneName)
			{
				gatesFromScene.Add(gateway);
			}
			else if (gateway.toScene == sceneName)
			{
				gatesToScene.Add(gateway);
			}
			else
			{
				((Loggable)Satchel.Instance).Log("Tried to add a Gateway that does not connect to this scene");
			}
		}

		public void AddBenchFromPrefab(GameObject prefab, string benchName, Vector3 pos)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Benches.Add(new SceneUtils.BenchParams
			{
				prefab = prefab,
				benchName = benchName,
				pos = pos,
				sceneName = sceneName
			});
		}

		private void OnGameManagerRefreshTilemapInfo(orig_RefreshTilemapInfo orig, GameManager self, string targetScene)
		{
			orig.Invoke(self, targetScene);
			if (targetScene == sceneName)
			{
				self.tilemap.width = (int)width;
				self.tilemap.height = (int)height;
				self.sceneWidth = width;
				self.sceneHeight = height;
				GameMap val = Object.FindObjectOfType<GameMap>();
				if ((Object)(object)val != (Object)null)
				{
					val.SetManualTilemap(0f, 0f, width, height);
				}
			}
		}

		private void OnGameManagerEnterHero(orig_EnterHero orig, GameManager self, bool additivegatesearch)
		{
			string text = self.sceneName;
			foreach (SceneUtils.GatewayParams item in gatesToScene)
			{
				if (text == item.fromScene)
				{
					SceneUtils.CreateGateway(item);
				}
			}
			if (text == sceneName)
			{
				if ((Object)(object)settings.backgroundMusic == (Object)null && settings.backgroundMusicGet != null)
				{
					settings.backgroundMusic = settings.backgroundMusicGet();
				}
				if ((Object)(object)settings.backgroundMusic != (Object)null)
				{
					SceneUtils.PlayBackgroundMusicForScene(settings.backgroundMusic);
				}
			}
			orig.Invoke(self, additivegatesearch);
		}

		private void OnSceneChanged(Scene from, Scene to)
		{
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			string name = ((Scene)(ref to)).name;
			if (name == sceneName)
			{
				foreach (SceneUtils.GatewayParams item in gatesFromScene)
				{
					SceneUtils.CreateGateway(item);
				}
				foreach (SceneUtils.BenchParams bench in Benches)
				{
					SceneUtils.CreateBenchFromPrefab(bench);
				}
			}
			else
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("RespawnPoint");
				foreach (GameObject val in array)
				{
					PlayMakerFSM val2 = FSMUtility.LocateMyFSM(val, "Bench Control");
					if ((Object)(object)val2 != (Object)null && val2.FsmVariables.FindFsmString("Scene Name").Value == sceneName)
					{
						Object.Destroy((Object)(object)val);
					}
				}
			}
			if (name == sceneName)
			{
				SceneUtils.FakeTileMapFromPrefab(TileMap);
				SceneManager sceneManagerFromPrefab = SceneUtils.getSceneManagerFromPrefab(SceneManager);
				((Object)((Component)sceneManagerFromPrefab).gameObject).name = "_SceneManager";
				sceneManagerFromPrefab.sceneType = settings.sceneType;
				sceneManagerFromPrefab.mapZone = settings.mapZone;
				sceneManagerFromPrefab.darknessLevel = settings.darknessLevel;
				sceneManagerFromPrefab.saturation = settings.saturation;
				sceneManagerFromPrefab.ignorePlatformSaturationModifiers = settings.ignorePlatformSaturationModifiers;
				sceneManagerFromPrefab.isWindy = settings.isWindy;
				sceneManagerFromPrefab.isTremorZone = settings.isTremorZone;
				sceneManagerFromPrefab.environmentType = settings.environmentType;
				sceneManagerFromPrefab.noParticles = settings.noParticles;
				sceneManagerFromPrefab.overrideParticlesWith = settings.overrideParticlesWith;
				sceneManagerFromPrefab.defaultColor = settings.defaultColor;
				sceneManagerFromPrefab.defaultIntensity = settings.defaultIntensity;
				sceneManagerFromPrefab.heroLightColor = settings.heroLightColor;
				((Component)sceneManagerFromPrefab).gameObject.SetActive(true);
				OnLoaded?.Invoke(this, new SceneLoadedEventArgs());
			}
		}
	}
	public class CustomBigItemGetManager
	{
		public GameObject MsgUiPrefab;

		public void Prepare(GameObject ShinyPrefab)
		{
			PlayMakerFSM fsm = FSMUtility.LocateMyFSM(ShinyPrefab, "Shiny Control");
			MsgUiPrefab = fsm.GetAction<CreateUIMsgGetItem>("Walljump", 3).gameObject.Value;
		}

		public void ShowDialog(string ItemName, string Intro1, string ButtonPress, string Prompt1, string Prompt2, Sprite sprite, Func<PlayerAction> actionGet, Action Callback)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			GameObject obj = Object.Instantiate<GameObject>(MsgUiPrefab, Vector3.zero, Quaternion.identity);
			Object.DontDestroyOnLoad((Object)(object)obj);
			CustomBigItemGetBehaviour customBigItemGetBehaviour = obj.AddComponent<CustomBigItemGetBehaviour>();
			customBigItemGetBehaviour.SetItem(ItemName, Intro1, ButtonPress, Prompt1, Prompt2, sprite, actionGet, Callback);
			customBigItemGetBehaviour.Show();
		}
	}
	public class CustomShinyManager
	{
		public GameObject standPrefab;

		public GameObject prefab;

		public static readonly string languageKey = "SATCHEL_SHINY_";

		private List<CustomShiny> shinies;

		private Dictionary<string, string> lang;

		public CustomShiny AddShiny(string ItemName, string sceneName, Vector3 position, bool isStand, bool isTrinket, Action Callback, Func<bool> ShouldSpawn, Sprite sprite = null)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			CustomShiny customShiny = new CustomShiny(ItemName, sceneName, position, isStand, isTrinket, Callback, ShouldSpawn, sprite);
			shinies.Add(customShiny);
			lang.Add(languageKey + "_" + customShiny.ItemName, customShiny.ItemName);
			return customShiny;
		}

		public CustomShinyManager()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			shinies = new List<CustomShiny>();
			lang = new Dictionary<string, string>();
			base..ctor();
			SceneManager.activeSceneChanged += SpawnOnSceneLoad;
			ModHooks.LanguageGetHook += new LanguageGetProxy(LanguageGet);
		}

		private IEnumerator SpawnCoro(Scene scene)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			yield return null;
			foreach (CustomShiny shiny in shinies)
			{
				if (((Scene)(ref scene)).name == shiny.sceneName && shiny.ShouldSpawn())
				{
					shiny.Spawn(shiny.isStand ? standPrefab : prefab);
				}
			}
		}

		private void SpawnOnSceneLoad(Scene oldScene, Scene newScene)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((MonoBehaviour)CoroutineHelper.GetRunner()).StartCoroutine(SpawnCoro(newScene));
		}

		private string LanguageGet(string key, string sheet, string orig)
		{
			if (sheet == languageKey + "UI" && key.StartsWith(languageKey))
			{
				return lang[key];
			}
			return orig;
		}
	}
	public class CustomShiny
	{
		public string sceneName;

		public Vector3 position;

		public Action Callback;

		public Func<bool> ShouldSpawn;

		public bool isTrinket;

		public bool isStand;

		public Sprite sprite;

		public string ItemName;

		public GameObject preparePrefab(GameObject prefab)
		{
			GameObject obj = Object.Instantiate<GameObject>(prefab);
			PlayMakerFSM fsm = FSMUtility.LocateMyFSM(obj, "Shiny Control");
			fsm.GetVariable<FsmBool>("Activated").Value = false;
			fsm.GetVariable<FsmBool>("Charm").Value = false;
			fsm.GetVariable<FsmBool>("Dash Cloak").Value = false;
			fsm.GetVariable<FsmBool>("Exit Dream").Value = false;
			fsm.GetVariable<FsmBool>("Journal").Value = false;
			fsm.GetVariable<FsmBool>("King's Brand").Value = false;
			fsm.GetVariable<FsmBool>("Pure Seed").Value = false;
			fsm.GetVariable<FsmBool>("Quake").Value = false;
			fsm.GetVariable<FsmBool>("Show Charm Tute").Value = false;
			fsm.GetVariable<FsmBool>("Slug Fling").Value = false;
			fsm.GetVariable<FsmBool>("Super Dash").Value = false;
			fsm.GetVariable<FsmBool>("Mantis Claw").Value = false;
			fsm.ChangeTransition("Init", "FINISHED", "Idle");
			fsm.RemoveAction("Init", 2);
			fsm.RemoveAction("Finish", 1);
			if (isTrinket)
			{
				fsm.ChangeTransition("Hero Down", "FINISHED", "Trink Flash");
				fsm.ChangeTransition("Trink Flash", "FINISHED", "Trink 1");
				fsm.GetAction<SetSpriteRendererSprite>("Trink 1", 2).sprite.Value = (Object)(object)sprite;
				fsm.GetAction<GetLanguageString>("Trink 1", 3).sheetName.Value = CustomShinyManager.languageKey + "UI";
				fsm.GetAction<GetLanguageString>("Trink 1", 3).convName.Value = CustomShinyManager.languageKey + "_" + ItemName;
				fsm.RemoveAction("Trink 1", 5);
				fsm.RemoveAction("Trink 1", 1);
				fsm.RemoveAction("Trink 1", 0);
				fsm.AddCustomAction("Trink 1", (Action)delegate
				{
					if (Callback != null)
					{
						Callback();
					}
				});
				return obj;
			}
			fsm.ChangeTransition("Hero Down", "FINISHED", "Big Get Flash");
			fsm.ChangeTransition("Big Get Flash", "FINISHED", "Dash");
			fsm.AddTransition("Dash", "FINISHED", "Hero Up");
			fsm.RemoveAction("Big Get Flash", 6);
			fsm.RemoveAction("Dash", 4);
			fsm.RemoveAction("Dash", 3);
			fsm.RemoveAction("Dash", 2);
			fsm.RemoveAction("Dash", 1);
			fsm.RemoveAction("Dash", 0);
			fsm.AddCustomAction("Dash", (Action)delegate
			{
				if (Callback != null)
				{
					Callback();
				}
			});
			return obj;
		}

		public CustomShiny(string ItemName, string sceneName, Vector3 position, bool isStand, bool isTrinket, Action Callback, Func<bool> ShouldSpawn, Sprite sprite)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			this.sceneName = "Town";
			this.position = new Vector3(0f, 0f, 0f);
			this.ShouldSpawn = () => true;
			base..ctor();
			this.ItemName = ItemName;
			this.sceneName = sceneName;
			this.position = position;
			this.isTrinket = isTrinket;
			this.Callback = Callback;
			this.ShouldSpawn = ShouldSpawn;
			this.sprite = sprite;
			this.isStand = isStand;
		}

		public void Spawn(GameObject prefab)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			GameObject obj = preparePrefab(prefab);
			obj.transform.position = position;
			obj.SetActive(true);
		}
	}
	public static class FsmUtil
	{
		public static FsmState AddState(this PlayMakerFSM fsm, FsmState state)
		{
			FsmState[] states = fsm.Fsm.States;
			FsmState[] array = (FsmState[])(object)new FsmState[states.Length + 1];
			int i;
			for (i = 0; i < states.Length; i++)
			{
				array[i] = states[i];
			}
			array[i] = state;
			fsm.Fsm.States = array;
			return array[i];
		}

		public static void GiveStatesPosition(this PlayMakerFSM fsm, float startX = 500f, float Y = 1100f)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			FsmState[] states = fsm.Fsm.States;
			float num = startX;
			for (int i = 0; i < states.Length; i++)
			{
				Rect position = states[i].Position;
				if (((Rect)(ref position)).height == 0f)
				{
					FsmState val = states[i];
					val.GiveStatePosition(num, Y, 10 * val.Name.Length + 10, 15 * (val.Transitions.Length + 1));
					float num2 = num;
					position = val.Position;
					num = num2 + (((Rect)(ref position)).width + 50f);
				}
			}
		}

		public static void GiveStatePosition(this FsmState state, float x, float y, float w, float h)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			state.Position = new Rect(x, y, w, h);
		}

		public static FsmState AddState(this PlayMakerFSM fsm, string stateName)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			return fsm.AddState(new FsmState(fsm.Fsm)
			{
				Name = stateName
			});
		}

		[Obsolete("Satchel.FsmUtil.GetState is deprecated, please use Satchel.FsmUtil.TryGetState or Satchel.FsmUtil.GetValidState instead.")]
		public static FsmState GetState(this PlayMakerFSM fsm, string stateName)
		{
			return fsm.Fsm.GetState(stateName);
		}

		public static bool TryGetState(this PlayMakerFSM fsm, string stateName, out FsmState state)
		{
			state = fsm.Fsm.GetState(stateName);
			return state != null;
		}

		public static FsmState GetValidState(this PlayMakerFSM fsm, string stateName)
		{
			if (fsm.TryGetState(stateName, out var state))
			{
				return state;
			}
			throw new ArgumentException("FSM \"" + fsm.FsmName + "\" does not have state \"" + stateName + "\"");
		}

		public static FsmState CopyState(this PlayMakerFSM fsm, string origStateName, string newStateName)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			FsmState val = new FsmState(fsm.Fsm.GetState(origStateName))
			{
				Name = newStateName
			};
			val.FixTransitionsFromState();
			fsm.AddState(val);
			return val;
		}

		public static void FixTransitionsFromState(this FsmState state)
		{
			FsmTransition[] transitions = state.Transitions;
			foreach (FsmTransition val in transitions)
			{
				val.ToFsmState = state.Fsm.GetState(val.ToState);
			}
		}

		public static void FixTransitionToState(this FsmState state)
		{
			FsmState[] states = state.Fsm.States;
			for (int i = 0; i < states.Length; i++)
			{
				FsmTransition[] transitions = states[i].Transitions;
				foreach (FsmTransition val in transitions)
				{
					if (val.ToState == state.Name)
					{
						val.ToFsmState = state;
					}
				}
			}
		}

		public static void AddTransition(this FsmState state, string onEventName, string toStateName)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			FsmTransition[] transitions = state.Transitions;
			FsmTransition[] array = (FsmTransition[])(object)new FsmTransition[transitions.Length + 1];
			FsmTransition val = new FsmTransition
			{
				ToState = toStateName,
				ToFsmState = state.Fsm.GetState(toStateName),
				FsmEvent = FsmEvent.GetFsmEvent(onEventName)
			};
			int i;
			for (i = 0; i < transitions.Length; i++)
			{
				array[i] = transitions[i];
			}
			array[i] = val;
			state.Transitions = array;
		}

		public static void AddTransition(this PlayMakerFSM fsm, string fromStateName, string onEventName, string toStateName)
		{
			fsm.Fsm.GetState(fromStateName).AddTransition(onEventName, toStateName);
		}

		public static void RemoveTransition(this FsmState state, string onEventName)
		{
			FsmTransition[] transitions = state.Transitions;
			FsmTransition[] array = (FsmTransition[])(object)new FsmTransition[transitions.Length - 1];
			int i = 0;
			int num = 0;
			for (; i < transitions.Length; i++)
			{
				if (transitions[i].EventName != onEventName)
				{
					array[num] = transitions[i];
					num++;
				}
			}
			state.Transitions = array;
		}

		public static void RemoveTransition(this PlayMakerFSM fsm, string fromStateName, string onEventName)
		{
			fsm.GetValidState(fromStateName).RemoveTransition(onEventName);
		}

		public static FsmTransition GetTransition(this FsmState state, string onEventName)
		{
			FsmTransition[] transitions = state.Transitions;
			for (int i = 0; i < transitions.Length; i++)
			{
				if (transitions[i].EventName == onEventName)
				{
					return transitions[i];
				}
			}
			return null;
		}

		public static FsmTransition GetTransition(this PlayMakerFSM fsm, string fromStateName, string onEventName)
		{
			return fsm.Fsm.GetState(fromStateName).GetTransition(onEventName);
		}

		public static void ChangeTransition(this FsmState state, string onEventName, string toStateName)
		{
			FsmTransition transition = state.GetTransition(onEventName);
			transition.ToState = toStateName;
			transition.ToFsmState = state.Fsm.GetState(toStateName);
		}

		public static void ChangeTransition(this PlayMakerFSM fsm, string fromStateName, string onEventName, string toStateName)
		{
			fsm.GetValidState(fromStateName).ChangeTransition(onEventName, toStateName);
		}

		public static void AddGlobalTransition(this PlayMakerFSM fsm, string onGlobalEventName, string toStateName)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			FsmTransition[] globalTransitions = fsm.Fsm.GlobalTransitions;
			FsmTransition[] array = (FsmTransition[])(object)new FsmTransition[globalTransitions.Length + 1];
			FsmTransition val = new FsmTransition
			{
				ToState = toStateName,
				ToFsmState = fsm.Fsm.GetState(toStateName),
				FsmEvent = FsmEvent.GetFsmEvent(onGlobalEventName)
			};
			int i;
			for (i = 0; i < globalTransitions.Length; i++)
			{
				array[i] = globalTransitions[i];
			}
			array[i] = val;
			fsm.Fsm.GlobalTransitions = array;
		}

		public static FsmStateAction GetAction(this FsmState state, int index)
		{
			return state.Actions[index];
		}

		public static FsmStateAction GetAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			return fsm.GetValidState(stateName).GetAction(index);
		}

		public static T GetAction<T>(this FsmState state, int index) where T : FsmStateAction
		{
			FsmStateAction action = state.GetAction(index);
			return (T)(object)((action is T) ? action : null);
		}

		public static T GetAction<T>(this PlayMakerFSM fsm, string stateName, int index) where T : FsmStateAction
		{
			return fsm.GetValidState(stateName).GetAction<T>(index);
		}

		public static T[] GetActions<T>(this FsmState state) where T : FsmStateAction
		{
			FsmStateAction[] actions = state.Actions;
			List<T> list = new List<T>();
			FsmStateAction[] array = actions;
			foreach (FsmStateAction val in array)
			{
				if (((object)val).GetType() == typeof(T))
				{
					list.Add((T)(object)val);
				}
			}
			return list.ToArray();
		}

		public static T[] GetActions<T>(this PlayMakerFSM fsm, string stateName) where T : FsmStateAction
		{
			return fsm.GetValidState(stateName).GetActions<T>();
		}

		public static T GetFirstActionOfType<T>(this FsmState state) where T : FsmStateAction
		{
			return state.GetActions<T>().FirstOrDefault();
		}

		public static T GetFirstActionOfType<T>(this PlayMakerFSM fsm, string state) where T : FsmStateAction
		{
			return fsm.GetActions<T>(state).FirstOrDefault();
		}

		public static void InsertAction(this FsmState state, FsmStateAction action, int index)
		{
			FsmStateAction[] actions = state.Actions;
			FsmStateAction[] array = (FsmStateAction[])(object)new FsmStateAction[actions.Length + 1];
			int num = 0;
			int num2 = 0;
			while (num < array.Length)
			{
				if (num == index)
				{
					array[num] = action;
					num++;
				}
				if (num2 < actions.Length)
				{
					array[num] = actions[num2];
				}
				num++;
				num2++;
			}
			state.Actions = array;
			action.Init(state);
		}

		public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index)
		{
			fsm.GetValidState(stateName).InsertAction(action, index);
		}

		public static void AddAction(this FsmState state, FsmStateAction action)
		{
			state.InsertAction(action, state.Actions.Length);
		}

		public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action)
		{
			fsm.GetValidState(stateName).AddAction(action);
		}

		public static void AddFirstAction(this FsmState state, FsmStateAction action)
		{
			state.InsertAction(action, 0);
		}

		public static void AddFirstAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action)
		{
			fsm.GetValidState(stateName).AddFirstAction(action);
		}

		public static void RemoveAction(this FsmState state, int index)
		{
			FsmStateAction[] actions = state.Actions;
			FsmStateAction[] array = (FsmStateAction[])(object)new FsmStateAction[actions.Length - 1];
			int i = 0;
			int num = 0;
			for (; i < actions.Length; i++)
			{
				if (i != index)
				{
					array[num] = actions[i];
					num++;
				}
			}
			state.Actions = array;
		}

		public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int index)
		{
			fsm.GetValidState(stateName).RemoveAction(index);
		}

		public static void DisableAction(this FsmState state, int index)
		{
			state.GetAction(index).Enabled = false;
		}

		public static void DisableAction(this PlayMakerFSM fsm, string state, int index)
		{
			fsm.GetValidState(state).DisableAction(index);
		}

		public static void AddCustomAction(this FsmState state, Action method)
		{
			state.AddAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = method
			});
		}

		public static void AddCustomAction(this PlayMakerFSM fsm, string stateName, Action method)
		{
			fsm.GetValidState(stateName).AddAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = method
			});
		}

		public static void AddCustomAction(this FsmState state, Action<FsmState> method)
		{
			state.AddAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = delegate
				{
					method(state);
				}
			});
		}

		public static void AddCustomAction(this PlayMakerFSM fsm, string stateName, Action<PlayMakerFSM> method)
		{
			fsm.GetValidState(stateName).AddAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = delegate
				{
					method(fsm);
				}
			});
		}

		public static void InsertCustomAction(this FsmState state, Action method, int index)
		{
			state.InsertAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = method
			}, index);
		}

		public static void InsertCustomAction(this PlayMakerFSM fsm, string stateName, Action method, int index)
		{
			fsm.GetValidState(stateName).InsertAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = method
			}, index);
		}

		public static void InsertCustomAction(this FsmState state, Action<FsmState> method, int index)
		{
			state.InsertAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = delegate
				{
					method(state);
				}
			}, index);
		}

		public static void InsertCustomAction(this PlayMakerFSM fsm, string stateName, Action<PlayMakerFSM> method, int index)
		{
			fsm.GetValidState(stateName).InsertAction((FsmStateAction)(object)new CustomFsmAction
			{
				method = delegate
				{
					method(fsm);
				}
			}, index);
		}

		public static void RemoveState(this PlayMakerFSM fsm, string stateName)
		{
			FsmState[] states = fsm.Fsm.States;
			fsm.Fsm.States = states.Where((FsmState state) => state.Name != stateName).ToArray();
		}

		[Obsolete("Satchel.FsmUtil.AddMethod is deprecated, please use Satchel.FsmUtil.AddCustomAction instead.")]
		public static void AddMethod(this PlayMakerFSM self, string state, Action method)
		{
			self.AddCustomAction(state, method);
		}

		[Obsolete("Satchel.FsmUtil.InsertMethod is deprecated, please use Satchel.FsmUtil.InsertCustomAction instead.")]
		public static void InsertMethod(this PlayMakerFSM self, string state, Action method, int index)
		{
			self.InsertCustomAction(state, method, index);
		}

		[Obsolete("Satchel.FsmUtil.getAudioClips is deprecated, please use Satchel.Futils.Extractors.GetAudioClips instead.")]
		public static Dictionary<string, AudioClip> getAudioClips(this PlayMakerFSM self)
		{
			return self.GetAudioClips();
		}

		[Obsolete("Satchel.FsmUtil.AddCustomAction with stateName is deprecated, please use the overload without stateName.")]
		public static void AddCustomAction(this FsmState state, string stateName, Action method)
		{
			state.AddCustomAction(method);
		}

		[Obsolete("Satchel.FsmUtil.AddCustomAction with stateName is deprecated, please use the overload without stateName.")]
		public static void AddCustomAction(this FsmState state, string stateName, Action<FsmState> method)
		{
			state.AddCustomAction(method);
		}

		[Obsolete("Satchel.FsmUtil.InsertCustomAction with stateName is deprecated, please use the overload without stateName.")]
		public static void InsertCustomAction(this FsmState state, string stateName, Action method, int index)
		{
			state.InsertCustomAction(method, index);
		}

		[Obsolete("Satchel.FsmUtil.InsertCustomAction with stateName is deprecated, please use the overload without stateName.")]
		public static void InsertCustomAction(this FsmState state, string stateName, Action<FsmState> method, int index)
		{
			state.InsertCustomAction(method, index);
		}
	}
	[Obsolete("Satchel.MethodAction is deprecated, please use Satchel.Futils.CustomFsmAction instead.")]
	public class MethodAction : CustomFsmAction
	{
	}
	public static class Tk2dUtils
	{
		private static FieldInfo[] spriteDefinitionFields;

		private static Type spriteDefinitionType;

		public static void SetTexture(this tk2dSpriteCollectionData collection, Texture tex)
		{
			if (collection.textures.Length != 0)
			{
				collection.textures[0] = tex;
			}
			if (collection.materials.Length != 0)
			{
				collection.materials[0].mainTexture = tex;
			}
			else
			{
				collection.material.mainTexture = tex;
			}
		}

		public static tk2dSpriteAnimation Clone(this tk2dSpriteAnimation animation)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			tk2dSpriteAnimation val = Object.Instantiate<tk2dSpriteAnimation>(animation);
			for (int i = 0; i < val.clips.Length; i++)
			{
				val.clips[i] = new tk2dSpriteAnimationClip(val.clips[i]);
			}
			((Object)val).name = ((Object)animation).name;
			return val;
		}

		public static tk2dSpriteCollectionData Clone(this tk2dSpriteCollectionData collection)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			tk2dSpriteCollectionData val = Object.Instantiate<tk2dSpriteCollectionData>(collection);
			for (int i = 0; i < val.materials.Length; i++)
			{
				val.materials[i] = Object.Instantiate<Material>(val.materials[i]);
			}
			for (int j = 0; j < val.spriteDefinitions.Length; j++)
			{
				tk2dSpriteDefinition val2 = new tk2dSpriteDefinition();
				if (spriteDefinitionType == null)
				{
					spriteDefinitionType = typeof(tk2dSpriteDefinition);
				}
				if (spriteDefinitionFields == null)
				{
					spriteDefinitionFields = spriteDefinitionType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}
				FieldInfo[] array = spriteDefinitionFields;
				foreach (FieldInfo fieldInfo in array)
				{
					fieldInfo.SetValue(val2, fieldInfo.GetValue(val.spriteDefinitions[j]));
				}
				val2.material = val.materials[val2.materialId];
				val.spriteDefinitions[j] = val2;
			}
			val.spriteCollectionName = collection.spriteCollectionName;
			((Object)val).name = ((Object)collection).name;
			return val;
		}
	}
}
namespace Satchel.BetterPreloads
{
	public abstract class BetterPreloadsMod<T> : Mod where T : new()
	{
		public static T Preloads = new T();

		private List<FieldInfo> fi;

		private List<PropertyInfo> pi;

		public override List<(string, string)> GetPreloadNames()
		{
			List<(string, string)> list = new List<(string, string)>();
			fi = new List<FieldInfo>(typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			pi = new List<PropertyInfo>(typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			foreach (FieldInfo item in fi)
			{
				PreloadAttribute customAttribute = item.GetCustomAttribute<PreloadAttribute>();
				if (customAttribute != null)
				{
					list.Add((customAttribute.scene, customAttribute.objPath));
				}
			}
			foreach (PropertyInfo item2 in pi)
			{
				if (item2.CanWrite)
				{
					PreloadAttribute customAttribute2 = item2.GetCustomAttribute<PreloadAttribute>();
					if (customAttribute2 != null)
					{
						list.Add((customAttribute2.scene, customAttribute2.objPath));
					}
				}
			}
			return list;
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			if (preloadedObjects == null)
			{
				return;
			}
			foreach (FieldInfo item in fi)
			{
				PreloadAttribute customAttribute = item.GetCustomAttribute<PreloadAttribute>();
				if (customAttribute != null)
				{
					ReflectionHelper.SetField<T, GameObject>(Preloads, item.Name, preloadedObjects[customAttribute.scene][customAttribute.objPath]);
				}
			}
			foreach (PropertyInfo item2 in pi)
			{
				if (item2.CanWrite)
				{
					PreloadAttribute customAttribute2 = item2.GetCustomAttribute<PreloadAttribute>();
					if (customAttribute2 != null)
					{
						ReflectionHelper.SetProperty<T, GameObject>(Preloads, item2.Name, preloadedObjects[customAttribute2.scene][customAttribute2.objPath]);
					}
				}
			}
			fi.Clear();
			pi.Clear();
			((Mod)this).Initialize();
		}

		protected BetterPreloadsMod()
			: base((string)null)
		{
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class PreloadAttribute : Attribute
	{
		public string scene { get; protected set; }

		public string objPath { get; protected set; }

		public PreloadAttribute(string scene, string objPath)
		{
			this.scene = scene;
			this.objPath = objPath;
		}
	}
}
namespace Satchel.Reflected
{
	[Obsolete("Please use HKMirror", true)]
	public static class GameManagerR
	{
		public static bool verboseMode
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "verboseMode");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "verboseMode", value);
			}
		}

		public static GameState gameState
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return GameManager.instance.gameState;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				GameManager.instance.gameState = value;
			}
		}

		public static bool isPaused
		{
			get
			{
				return GameManager.instance.isPaused;
			}
			set
			{
				GameManager.instance.isPaused = value;
			}
		}

		public static int timeSlowedCount
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, int>(GameManager.instance, "timeSlowedCount");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, int>(GameManager.instance, "timeSlowedCount", value);
			}
		}

		public static string sceneName
		{
			get
			{
				return GameManager.instance.sceneName;
			}
			set
			{
				GameManager.instance.sceneName = value;
			}
		}

		public static string nextSceneName
		{
			get
			{
				return GameManager.instance.nextSceneName;
			}
			set
			{
				GameManager.instance.nextSceneName = value;
			}
		}

		public static string entryGateName
		{
			get
			{
				return GameManager.instance.entryGateName;
			}
			set
			{
				GameManager.instance.entryGateName = value;
			}
		}

		public static TransitionPoint callingGate
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, TransitionPoint>(GameManager.instance, "callingGate");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, TransitionPoint>(GameManager.instance, "callingGate", value);
			}
		}

		public static Vector3 entrySpawnPoint
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<GameManager, Vector3>(GameManager.instance, "entrySpawnPoint");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<GameManager, Vector3>(GameManager.instance, "entrySpawnPoint", value);
			}
		}

		public static float entryDelay
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "entryDelay");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, float>(GameManager.instance, "entryDelay", value);
			}
		}

		public static float sceneWidth
		{
			get
			{
				return GameManager.instance.sceneWidth;
			}
			set
			{
				GameManager.instance.sceneWidth = value;
			}
		}

		public static float sceneHeight
		{
			get
			{
				return GameManager.instance.sceneHeight;
			}
			set
			{
				GameManager.instance.sceneHeight = value;
			}
		}

		public static GameConfig gameConfig
		{
			get
			{
				return GameManager.instance.gameConfig;
			}
			set
			{
				GameManager.instance.gameConfig = value;
			}
		}

		public static InputHandler inputHandler
		{
			get
			{
				return GameManager.instance.inputHandler;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, InputHandler>(GameManager.instance, "inputHandler", value);
			}
		}

		public static GameCameras gameCams
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, GameCameras>(GameManager.instance, "gameCams");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, GameCameras>(GameManager.instance, "gameCams", value);
			}
		}

		public static AchievementHandler achievementHandler
		{
			get
			{
				return GameManager.instance.achievementHandler;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, AchievementHandler>(GameManager.instance, "achievementHandler", value);
			}
		}

		public static AudioManager audioManager
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, AudioManager>(GameManager.instance, "audioManager");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, AudioManager>(GameManager.instance, "audioManager", value);
			}
		}

		public static InControlManager inControlManagerPrefab
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, InControlManager>(GameManager.instance, "inControlManagerPrefab");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, InControlManager>(GameManager.instance, "inControlManagerPrefab", value);
			}
		}

		public static GameSettings gameSettings
		{
			get
			{
				return GameManager.instance.gameSettings;
			}
			set
			{
				GameManager.instance.gameSettings = value;
			}
		}

		public static TimeScaleIndependentUpdate timeTool
		{
			get
			{
				return GameManager.instance.timeTool;
			}
			set
			{
				GameManager.instance.timeTool = value;
			}
		}

		public static GameObject gameMap
		{
			get
			{
				return GameManager.instance.gameMap;
			}
			set
			{
				GameManager.instance.gameMap = value;
			}
		}

		public static CameraController cameraCtrl
		{
			get
			{
				return GameManager.instance.cameraCtrl;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, CameraController>(GameManager.instance, "cameraCtrl", value);
			}
		}

		public static HeroController hero_ctrl
		{
			get
			{
				return GameManager.instance.hero_ctrl;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, HeroController>(GameManager.instance, "hero_ctrl", value);
			}
		}

		public static SpriteRenderer heroLight
		{
			get
			{
				return GameManager.instance.heroLight;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, SpriteRenderer>(GameManager.instance, "heroLight", value);
			}
		}

		public static SceneManager sm
		{
			get
			{
				return GameManager.instance.sm;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, SceneManager>(GameManager.instance, "sm", value);
			}
		}

		public static UIManager ui
		{
			get
			{
				return GameManager.instance.ui;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, UIManager>(GameManager.instance, "ui", value);
			}
		}

		public static tk2dTileMap tilemap
		{
			get
			{
				return GameManager.instance.tilemap;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, tk2dTileMap>(GameManager.instance, "tilemap", value);
			}
		}

		public static PlayMakerFSM soulOrb_fsm
		{
			get
			{
				return GameManager.instance.soulOrb_fsm;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, PlayMakerFSM>(GameManager.instance, "soulOrb_fsm", value);
			}
		}

		public static PlayMakerFSM soulVessel_fsm
		{
			get
			{
				return GameManager.instance.soulVessel_fsm;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, PlayMakerFSM>(GameManager.instance, "soulVessel_fsm", value);
			}
		}

		public static PlayMakerFSM inventoryFSM
		{
			get
			{
				return GameManager.instance.inventoryFSM;
			}
			set
			{
				GameManager.instance.inventoryFSM = value;
			}
		}

		public static PlayerData playerData
		{
			get
			{
				return GameManager.instance.playerData;
			}
			set
			{
				GameManager.instance.playerData = value;
			}
		}

		public static SceneData sceneData
		{
			get
			{
				return GameManager.instance.sceneData;
			}
			set
			{
				GameManager.instance.sceneData = value;
			}
		}

		public static int profileID
		{
			get
			{
				return GameManager.instance.profileID;
			}
			set
			{
				GameManager.instance.profileID = value;
			}
		}

		public static bool needsFlush
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "needsFlush");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "needsFlush", value);
			}
		}

		public static bool isEmergencyReturningToMenu
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "isEmergencyReturningToMenu");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "isEmergencyReturningToMenu", value);
			}
		}

		public static float sessionPlayTimer
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "sessionPlayTimer");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, float>(GameManager.instance, "sessionPlayTimer", value);
			}
		}

		public static float sessionStartTime
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "sessionStartTime");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, float>(GameManager.instance, "sessionStartTime", value);
			}
		}

		public static bool startedOnThisScene
		{
			get
			{
				return GameManager.instance.startedOnThisScene;
			}
			set
			{
				GameManager.instance.startedOnThisScene = value;
			}
		}

		public static bool RespawningHero
		{
			get
			{
				return GameManager.instance.RespawningHero;
			}
			set
			{
				GameManager.instance.RespawningHero = value;
			}
		}

		public static bool hazardRespawningHero
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "hazardRespawningHero");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "hazardRespawningHero", value);
			}
		}

		public static string targetScene
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, string>(GameManager.instance, "targetScene");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, string>(GameManager.instance, "targetScene", value);
			}
		}

		public static bool tilemapDirty
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "tilemapDirty");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "tilemapDirty", value);
			}
		}

		public static bool needFirstFadeIn
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "needFirstFadeIn");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "needFirstFadeIn", value);
			}
		}

		public static bool waitForManualLevelStart
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "waitForManualLevelStart");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "waitForManualLevelStart", value);
			}
		}

		public static bool startedSteamEnabled
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "startedSteamEnabled");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "startedSteamEnabled", value);
			}
		}

		public static bool startedGOGEnabled
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "startedGOGEnabled");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "startedGOGEnabled", value);
			}
		}

		public static bool startedLanguageDisabled
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "startedLanguageDisabled");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "startedLanguageDisabled", value);
			}
		}

		public static AudioMixerSnapshot actorSnapshotUnpaused
		{
			get
			{
				return GameManager.instance.actorSnapshotUnpaused;
			}
			set
			{
				GameManager.instance.actorSnapshotUnpaused = value;
			}
		}

		public static AudioMixerSnapshot actorSnapshotPaused
		{
			get
			{
				return GameManager.instance.actorSnapshotPaused;
			}
			set
			{
				GameManager.instance.actorSnapshotPaused = value;
			}
		}

		public static AudioMixerSnapshot silentSnapshot
		{
			get
			{
				return GameManager.instance.silentSnapshot;
			}
			set
			{
				GameManager.instance.silentSnapshot = value;
			}
		}

		public static AudioMixerSnapshot noMusicSnapshot
		{
			get
			{
				return GameManager.instance.noMusicSnapshot;
			}
			set
			{
				GameManager.instance.noMusicSnapshot = value;
			}
		}

		public static MusicCue noMusicCue
		{
			get
			{
				return GameManager.instance.noMusicCue;
			}
			set
			{
				GameManager.instance.noMusicCue = value;
			}
		}

		public static AudioMixerSnapshot noAtmosSnapshot
		{
			get
			{
				return GameManager.instance.noAtmosSnapshot;
			}
			set
			{
				GameManager.instance.noAtmosSnapshot = value;
			}
		}

		public static bool IsInSceneTransition
		{
			get
			{
				return GameManager.instance.IsInSceneTransition;
			}
			set
			{
				ReflectionHelper.SetProperty<GameManager, bool>(GameManager.instance, "IsInSceneTransition", value);
			}
		}

		public static bool hasFinishedEnteringScene
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "hasFinishedEnteringScene");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "hasFinishedEnteringScene", value);
			}
		}

		public static WorldInfo worldInfo
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, WorldInfo>(GameManager.instance, "worldInfo");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, WorldInfo>(GameManager.instance, "worldInfo", value);
			}
		}

		public static bool isLoading
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "isLoading");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "isLoading", value);
			}
		}

		public static float currentLoadDuration
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, float>(GameManager.instance, "currentLoadDuration");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, float>(GameManager.instance, "currentLoadDuration", value);
			}
		}

		public static int sceneLoadsWithoutGarbageCollect
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, int>(GameManager.instance, "sceneLoadsWithoutGarbageCollect");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, int>(GameManager.instance, "sceneLoadsWithoutGarbageCollect", value);
			}
		}

		public static bool isUsingCustomLoadAnimation
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, bool>(GameManager.instance, "isUsingCustomLoadAnimation");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, bool>(GameManager.instance, "isUsingCustomLoadAnimation", value);
			}
		}

		public static StandaloneLoadingSpinner standaloneLoadingSpinnerPrefab
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, StandaloneLoadingSpinner>(GameManager.instance, "standaloneLoadingSpinnerPrefab");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, StandaloneLoadingSpinner>(GameManager.instance, "standaloneLoadingSpinnerPrefab", value);
			}
		}

		public static SceneLoad sceneLoad
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, SceneLoad>(GameManager.instance, "sceneLoad");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, SceneLoad>(GameManager.instance, "sceneLoad", value);
			}
		}

		public static int saveIconShowCounter
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, int>(GameManager.instance, "saveIconShowCounter");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, int>(GameManager.instance, "saveIconShowCounter", value);
			}
		}

		public static UIManager _uiInstance
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, UIManager>(GameManager.instance, "_uiInstance");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, UIManager>(GameManager.instance, "_uiInstance", value);
			}
		}

		public static InControlManager _spawnedInControlManager
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, InControlManager>("_spawnedInControlManager");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, InControlManager>("_spawnedInControlManager", value);
			}
		}

		public static int NoSaveSlotID => -1;

		public static string[] SubSceneNameSuffixes
		{
			get
			{
				return ReflectionHelper.GetField<GameManager, string[]>("SubSceneNameSuffixes");
			}
			set
			{
				ReflectionHelper.SetField<GameManager, string[]>("SubSceneNameSuffixes", value);
			}
		}

		public static void Awake()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "Awake", Array.Empty<object>());
		}

		public static void Start()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "Start", Array.Empty<object>());
		}

		public static void Update()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "Update", Array.Empty<object>());
		}

		public static void UpdateEngagement()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "UpdateEngagement", Array.Empty<object>());
		}

		public static void LevelActivated(Scene sceneFrom, Scene sceneTo)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "LevelActivated", new object[2] { sceneFrom, sceneTo });
		}

		public static void OnDisable()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "OnDisable", Array.Empty<object>());
		}

		public static void OnApplicationQuit()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "OnApplicationQuit", Array.Empty<object>());
		}

		public static void BeginSceneTransition(SceneLoadInfo info)
		{
			GameManager.instance.BeginSceneTransition(info);
		}

		public static IEnumerator BeginSceneTransitionRoutine(SceneLoadInfo info)
		{
			return ReflectionHelper.CallMethod<GameManager, IEnumerator>(GameManager.instance, "BeginSceneTransitionRoutine", new object[1] { info });
		}

		public static IEnumerator TransitionScene(TransitionPoint gate)
		{
			return GameManager.instance.TransitionScene(gate);
		}

		public static void ChangeToScene(string targetScene, string entryGateName, float pauseBeforeEnter)
		{
			GameManager.instance.ChangeToScene(targetScene, entryGateName, pauseBeforeEnter);
		}

		public static void WarpToDreamGate()
		{
			GameManager.instance.WarpToDreamGate();
		}

		public static void LeftScene(bool doAdditiveLoad = false)
		{
			GameManager.instance.LeftScene(doAdditiveLoad);
		}

		public static IEnumerator PlayerDead(float waitTime)
		{
			return GameManager.instance.PlayerDead(waitTime);
		}

		public static IEnumerator PlayerDeadFromHazard(float waitTime)
		{
			return GameManager.instance.PlayerDeadFromHazard(waitTime);
		}

		public static void ReadyForRespawn(bool isFirstLevelForPlayer)
		{
			GameManager.instance.ReadyForRespawn(isFirstLevelForPlayer);
		}

		public static void HazardRespawn()
		{
			GameManager.instance.HazardRespawn();
		}

		public static void TimePasses()
		{
			GameManager.instance.TimePasses();
		}

		public static void FadeSceneIn()
		{
			GameManager.instance.FadeSceneIn();
		}

		public static IEnumerator FadeSceneInWithDelay(float delay)
		{
			return GameManager.instance.FadeSceneInWithDelay(delay);
		}

		public static bool IsGamePaused()
		{
			return GameManager.instance.IsGamePaused();
		}

		public static void SetGameMap(GameObject go_gameMap)
		{
			GameManager.instance.SetGameMap(go_gameMap);
		}

		public static void CalculateNotchesUsed()
		{
			GameManager.instance.CalculateNotchesUsed();
		}

		public static string GetLanguageAsString()
		{
			return GameManager.instance.GetLanguageAsString();
		}

		public static string GetEntryGateName()
		{
			return GameManager.instance.GetEntryGateName();
		}

		public static void SetPlayerDataBool(string boolName, bool value)
		{
			GameManager.instance.SetPlayerDataBool(boolName, value);
		}

		public static void SetPlayerDataInt(string intName, int value)
		{
			GameManager.instance.SetPlayerDataInt(intName, value);
		}

		public static void SetPlayerDataFloat(string floatName, float value)
		{
			GameManager.instance.SetPlayerDataFloat(floatName, value);
		}

		public static void SetPlayerDataString(string stringName, string value)
		{
			GameManager.instance.SetPlayerDataString(stringName, value);
		}

		public static void IncrementPlayerDataInt(string intName)
		{
			GameManager.instance.IncrementPlayerDataInt(intName);
		}

		public static void DecrementPlayerDataInt(string intName)
		{
			GameManager.instance.DecrementPlayerDataInt(intName);
		}

		public static void IntAdd(string intName, int amount)
		{
			GameManager.instance.IntAdd(intName, amount);
		}

		public static bool GetPlayerDataBool(string boolName)
		{
			return GameManager.instance.GetPlayerDataBool(boolName);
		}

		public static int GetPlayerDataInt(string intName)
		{
			return GameManager.instance.GetPlayerDataInt(intName);
		}

		public static float GetPlayerDataFloat(string floatName)
		{
			return GameManager.instance.GetPlayerDataFloat(floatName);
		}

		public static string GetPlayerDataString(string stringName)
		{
			return GameManager.instance.GetPlayerDataString(stringName);
		}

		public static void SetPlayerDataVector3(string vectorName, Vector3 value)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			GameManager.instance.SetPlayerDataVector3(vectorName, value);
		}

		public static Vector3 GetPlayerDataVector3(string vectorName)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return GameManager.instance.GetPlayerDataVector3(vectorName);
		}

		public static T GetPlayerDataVariable<T>(string fieldName)
		{
			return GameManager.instance.GetPlayerDataVariable<T>(fieldName);
		}

		public static void SetPlayerDataVariable<T>(string fieldName, T value)
		{
			GameManager.instance.SetPlayerDataVariable<T>(fieldName, value);
		}

		public static void EquipCharm(int charmNum)
		{
			GameManager.instance.EquipCharm(charmNum);
		}

		public static void UnequipCharm(int charmNum)
		{
			GameManager.instance.UnequipCharm(charmNum);
		}

		public static void RefreshOvercharm()
		{
			GameManager.instance.RefreshOvercharm();
		}

		public static void UpdateBlueHealth()
		{
			GameManager.instance.UpdateBlueHealth();
		}

		public static void SetCurrentMapZoneAsRespawn()
		{
			GameManager.instance.SetCurrentMapZoneAsRespawn();
		}

		public static void SetMapZoneToSpecific(string mapZone)
		{
			GameManager.instance.SetMapZoneToSpecific(mapZone);
		}

		public static void StartSoulLimiter()
		{
			GameManager.instance.StartSoulLimiter();
		}

		public static void EndSoulLimiter()
		{
			GameManager.instance.EndSoulLimiter();
		}

		public static bool UpdateGameMap()
		{
			return GameManager.instance.UpdateGameMap();
		}

		public static void CheckAllMaps()
		{
			GameManager.instance.CheckAllMaps();
		}

		public static void AddToScenesVisited(string scene)
		{
			GameManager.instance.AddToScenesVisited(scene);
		}

		public static bool GetIsSceneVisited(string scene)
		{
			return GameManager.instance.GetIsSceneVisited(scene);
		}

		public static void AddToBenchList()
		{
			GameManager.instance.AddToBenchList();
		}

		public static void AddToGrubList()
		{
			GameManager.instance.AddToGrubList();
		}

		public static void AddToFlameList()
		{
			GameManager.instance.AddToFlameList();
		}

		public static void AddToCocoonList()
		{
			GameManager.instance.AddToCocoonList();
		}

		public static void AddToDreamPlantList()
		{
			GameManager.instance.AddToDreamPlantList();
		}

		public static void AddToDreamPlantCList()
		{
			GameManager.instance.AddToDreamPlantCList();
		}

		public static void CountGameCompletion()
		{
			GameManager.instance.CountGameCompletion();
		}

		public static void CountCharms()
		{
			GameManager.instance.CountCharms();
		}

		public static void CountJournalEntries()
		{
			GameManager.instance.CountJournalEntries();
		}

		public static void ActivateTestingCheats()
		{
			GameManager.instance.ActivateTestingCheats();
		}

		public static void GetAllPowerups()
		{
			GameManager.instance.GetAllPowerups();
		}

		public static void StoryRecord_death()
		{
			GameManager.instance.StoryRecord_death();
		}

		public static void StoryRecord_rescueGrub()
		{
			GameManager.instance.StoryRecord_rescueGrub();
		}

		public static void StoryRecord_defeatedShade()
		{
			GameManager.instance.StoryRecord_defeatedShade();
		}

		public static void StoryRecord_discoveredArea(string areaName)
		{
			GameManager.instance.StoryRecord_discoveredArea(areaName);
		}

		public static void StoryRecord_travelledToArea(string areaName)
		{
			GameManager.instance.StoryRecord_travelledToArea(areaName);
		}

		public static void StoryRecord_bankDeposit(int amount)
		{
			GameManager.instance.StoryRecord_bankDeposit(amount);
		}

		public static void StoryRecord_bankWithdraw(int amount)
		{
			GameManager.instance.StoryRecord_bankWithdraw(amount);
		}

		public static void StoryRecord_boughtCorniferMap(string map)
		{
			GameManager.instance.StoryRecord_boughtCorniferMap(map);
		}

		public static void StoryRecord_visited(string visited)
		{
			GameManager.instance.StoryRecord_visited(visited);
		}

		public static void StoryRecord_defeated(string defeated)
		{
			GameManager.instance.StoryRecord_defeated(defeated);
		}

		public static void StoryRecord_jiji()
		{
			GameManager.instance.StoryRecord_jiji();
		}

		public static void StoryRecord_rodeStag(string dest)
		{
			GameManager.instance.StoryRecord_rodeStag(dest);
		}

		public static void StoryRecord_acquired(string item)
		{
			GameManager.instance.StoryRecord_acquired(item);
		}

		public static void StoryRecord_bought(string item)
		{
			GameManager.instance.StoryRecord_bought(item);
		}

		public static void StoryRecord_quit()
		{
			GameManager.instance.StoryRecord_quit();
		}

		public static void StoryRecord_rest()
		{
			GameManager.instance.StoryRecord_rest();
		}

		public static void StoryRecord_upgradeNail()
		{
			GameManager.instance.StoryRecord_upgradeNail();
		}

		public static void StoryRecord_heartPiece()
		{
			GameManager.instance.StoryRecord_heartPiece();
		}

		public static void StoryRecord_maxHealthUp()
		{
			GameManager.instance.StoryRecord_maxHealthUp();
		}

		public static void StoryRecord_soulPiece()
		{
			GameManager.instance.StoryRecord_soulPiece();
		}

		public static void StoryRecord_maxSoulUp()
		{
			GameManager.instance.StoryRecord_maxSoulUp();
		}

		public static void StoryRecord_charmsChanged()
		{
			GameManager.instance.StoryRecord_charmsChanged();
		}

		public static void StoryRecord_charmEquipped(string charmName)
		{
			GameManager.instance.StoryRecord_charmEquipped(charmName);
		}

		public static void StoryRecord_start()
		{
			GameManager.instance.StoryRecord_start();
		}

		public static void AwardAchievement(string key)
		{
			GameManager.instance.AwardAchievement(key);
		}

		public static void QueueAchievement(string key)
		{
			GameManager.instance.QueueAchievement(key);
		}

		public static void AwardQueuedAchievements()
		{
			GameManager.instance.AwardQueuedAchievements();
		}

		public static bool IsAchievementAwarded(string key)
		{
			return GameManager.instance.IsAchievementAwarded(key);
		}

		public static void ClearAllAchievements()
		{
			GameManager.instance.ClearAllAchievements();
		}

		public static void CheckCharmAchievements()
		{
			GameManager.instance.CheckCharmAchievements();
		}

		public static void CheckGrubAchievements()
		{
			GameManager.instance.CheckGrubAchievements();
		}

		public static void CheckStagStationAchievements()
		{
			GameManager.instance.CheckStagStationAchievements();
		}

		public static void CheckMapAchievement()
		{
			GameManager.instance.CheckMapAchievement();
		}

		public static void CheckJournalAchievements()
		{
			GameManager.instance.CheckJournalAchievements();
		}

		public static void CheckAllAchievements()
		{
			GameManager.instance.CheckAllAchievements();
		}

		public static void CheckBanishmentAchievement()
		{
			GameManager.instance.CheckBanishmentAchievement();
		}

		public static void SetStatusRecordInt(string key, int value)
		{
			GameManager.instance.SetStatusRecordInt(key, value);
		}

		public static int GetStatusRecordInt(string key)
		{
			return GameManager.instance.GetStatusRecordInt(key);
		}

		public static void ResetStatusRecords()
		{
			GameManager.instance.ResetStatusRecords();
		}

		public static void SaveStatusRecords()
		{
			GameManager.instance.SaveStatusRecords();
		}

		public static void SetState(GameState newState)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			GameManager.instance.SetState(newState);
		}

		public static void LoadScene(string destScene)
		{
			GameManager.instance.LoadScene(destScene);
		}

		public static IEnumerator LoadSceneAdditive(string destScene)
		{
			return GameManager.instance.LoadSceneAdditive(destScene);
		}

		public static void OnNextLevelReady()
		{
			GameManager.instance.OnNextLevelReady();
		}

		public static void OnWillActivateFirstLevel()
		{
			GameManager.instance.OnWillActivateFirstLevel();
		}

		public static IEnumerator LoadFirstScene()
		{
			return GameManager.instance.LoadFirstScene();
		}

		public static void LoadPermadeathUnlockScene()
		{
			GameManager.instance.LoadPermadeathUnlockScene();
		}

		public static void LoadMrMushromScene()
		{
			GameManager.instance.LoadMrMushromScene();
		}

		public static void LoadOpeningCinematic()
		{
			GameManager.instance.LoadOpeningCinematic();
		}

		public static void PositionHeroAtSceneEntrance()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "PositionHeroAtSceneEntrance", Array.Empty<object>());
		}

		public static Vector2? FindEntryPoint(string entryPointName, Scene filterScene)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<GameManager, Vector2?>(GameManager.instance, "FindEntryPoint", new object[2] { entryPointName, filterScene });
		}

		public static TransitionPoint FindTransitionPoint(string entryPointName, Scene filterScene, bool fallbackToAnyAvailable)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<GameManager, TransitionPoint>(GameManager.instance, "FindTransitionPoint", new object[3] { entryPointName, filterScene, fallbackToAnyAvailable });
		}

		public static void EnterHero(bool additiveGateSearch = false)
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "EnterHero", new object[1] { additiveGateSearch });
		}

		public static void FinishedEnteringScene()
		{
			GameManager.instance.FinishedEnteringScene();
		}

		public static void SetupGameRefs()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "SetupGameRefs", Array.Empty<object>());
		}

		public static void SetupSceneRefs(bool refreshTilemapInfo)
		{
			GameManager.instance.SetupSceneRefs(refreshTilemapInfo);
		}

		public static void SetupHeroRefs()
		{
			GameManager.instance.SetupHeroRefs();
		}

		public static void BeginScene()
		{
			GameManager.instance.BeginScene();
		}

		public static void UpdateUIStateFromGameState()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "UpdateUIStateFromGameState", Array.Empty<object>());
		}

		public static void SkipCutscene()
		{
			GameManager.instance.SkipCutscene();
		}

		public static IEnumerator SkipCutsceneNoMash()
		{
			return ReflectionHelper.CallMethod<GameManager, IEnumerator>(GameManager.instance, "SkipCutsceneNoMash", Array.Empty<object>());
		}

		public static void NoLongerFirstGame()
		{
			GameManager.instance.NoLongerFirstGame();
		}

		public static void SetupStatusModifiers()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "SetupStatusModifiers", Array.Empty<object>());
		}

		public static void MatchBackerCreditsSetting()
		{
			GameManager.instance.MatchBackerCreditsSetting();
		}

		public static void RefreshLocalization()
		{
			GameManager.instance.RefreshLocalization();
		}

		public static void RefreshParticleSystems()
		{
			GameManager.instance.RefreshParticleSystems();
		}

		public static void ApplyNativeInput()
		{
			GameManager.instance.ApplyNativeInput();
		}

		public static void EnablePermadeathMode()
		{
			GameManager.instance.EnablePermadeathMode();
		}

		public static string GetCurrentMapZone()
		{
			return GameManager.instance.GetCurrentMapZone();
		}

		public static float GetSceneWidth()
		{
			return GameManager.instance.GetSceneWidth();
		}

		public static float GetSceneHeight()
		{
			return GameManager.instance.GetSceneHeight();
		}

		public static GameObject GetSceneManager()
		{
			return GameManager.instance.GetSceneManager();
		}

		public static string GetFormattedMapZoneString(MapZone mapZone)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return GameManager.instance.GetFormattedMapZoneString(mapZone);
		}

		public static void UpdateSceneName()
		{
			GameManager.instance.UpdateSceneName();
		}

		public static string GetSceneNameString()
		{
			return GameManager.instance.GetSceneNameString();
		}

		public static void RefreshTilemapInfo(string targetScene)
		{
			GameManager.instance.RefreshTilemapInfo(targetScene);
		}

		public static void SaveLevelState()
		{
			GameManager.instance.SaveLevelState();
		}

		public static void ResetSemiPersistentItems()
		{
			GameManager.instance.ResetSemiPersistentItems();
		}

		public static bool IsMenuScene()
		{
			return GameManager.instance.IsMenuScene();
		}

		public static bool IsTitleScreenScene()
		{
			return GameManager.instance.IsTitleScreenScene();
		}

		public static bool IsGameplayScene()
		{
			return GameManager.instance.IsGameplayScene();
		}

		public static bool IsNonGameplayScene()
		{
			return GameManager.instance.IsNonGameplayScene();
		}

		public static bool IsCinematicScene()
		{
			return GameManager.instance.IsCinematicScene();
		}

		public static bool IsStagTravelScene()
		{
			return GameManager.instance.IsStagTravelScene();
		}

		public static bool IsBetaEndScene()
		{
			return GameManager.instance.IsBetaEndScene();
		}

		public static bool IsTutorialScene()
		{
			return GameManager.instance.IsTutorialScene();
		}

		public static bool IsTestScene()
		{
			return GameManager.instance.IsTestScene();
		}

		public static bool IsBossDoorScene()
		{
			return GameManager.instance.IsBossDoorScene();
		}

		public static bool ShouldKeepHUDCameraActive()
		{
			return GameManager.instance.ShouldKeepHUDCameraActive();
		}

		public static bool IsUnloadAssetsRequired(string sourceSceneName, string destinationSceneName)
		{
			return GameManager.instance.IsUnloadAssetsRequired(sourceSceneName, destinationSceneName);
		}

		public static void HasSaveFile(int saveSlot, Action<bool> callback)
		{
			GameManager.instance.HasSaveFile(saveSlot, callback);
		}

		public static void SaveGame()
		{
			GameManager.instance.SaveGame();
		}

		public static void ShowSaveIcon()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "ShowSaveIcon", Array.Empty<object>());
		}

		public static void HideSaveIcon()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "HideSaveIcon", Array.Empty<object>());
		}

		public static void SaveGame(Action<bool> callback)
		{
			GameManager.instance.SaveGame(callback);
		}

		public static void ResetGameTimer()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "ResetGameTimer", Array.Empty<object>());
		}

		public static void IncreaseGameTimer(ref float timer)
		{
			GameManager.instance.IncreaseGameTimer(ref timer);
		}

		public static void SaveGame(int saveSlot, Action<bool> callback)
		{
			GameManager.instance.SaveGame(saveSlot, callback);
		}

		public static void LoadGameFromUI(int saveSlot)
		{
			GameManager.instance.LoadGameFromUI(saveSlot);
		}

		public static IEnumerator LoadGameFromUIRoutine(int saveSlot)
		{
			return ReflectionHelper.CallMethod<GameManager, IEnumerator>(GameManager.instance, "LoadGameFromUIRoutine", new object[1] { saveSlot });
		}

		public static void LoadGame(int saveSlot, Action<bool> callback)
		{
			GameManager.instance.LoadGame(saveSlot, callback);
		}

		public static void ClearSaveFile(int saveSlot, Action<bool> callback)
		{
			GameManager.instance.ClearSaveFile(saveSlot, callback);
		}

		public static void GetSaveStatsForSlot(int saveSlot, Action<SaveStats> callback)
		{
			GameManager.instance.GetSaveStatsForSlot(saveSlot, callback);
		}

		public static IEnumerator PauseGameToggleByMenu()
		{
			return GameManager.instance.PauseGameToggleByMenu();
		}

		public static IEnumerator PauseGameToggle()
		{
			return GameManager.instance.PauseGameToggle();
		}

		public static IEnumerator SetTimeScale(float newTimeScale, float duration)
		{
			return ReflectionHelper.CallMethod<GameManager, IEnumerator>(GameManager.instance, "SetTimeScale", new object[2] { newTimeScale, duration });
		}

		public static void SetTimeScale(float newTimeScale)
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "SetTimeScale", new object[1] { newTimeScale });
		}

		public static void FreezeMoment(int type)
		{
			GameManager.instance.FreezeMoment(type);
		}

		public static IEnumerator FreezeMoment(float rampDownTime, float waitTime, float rampUpTime, float targetSpeed)
		{
			return GameManager.instance.FreezeMoment(rampDownTime, waitTime, rampUpTime, targetSpeed);
		}

		public static IEnumerator FreezeMomentGC(float rampDownTime, float waitTime, float rampUpTime, float targetSpeed)
		{
			return GameManager.instance.FreezeMomentGC(rampDownTime, waitTime, rampUpTime, targetSpeed);
		}

		public static IEnumerator FreezeMoment(float rampDownTime, float waitTime, float rampUpTime, bool runGc = false)
		{
			return GameManager.instance.FreezeMoment(rampDownTime, waitTime, rampUpTime, runGc);
		}

		public static void EnsureSaveSlotSpace(Action<bool> callback)
		{
			GameManager.instance.EnsureSaveSlotSpace(callback);
		}

		public static void StartNewGame(bool permadeathMode = false, bool bossRushMode = false)
		{
			GameManager.instance.StartNewGame(permadeathMode, bossRushMode);
		}

		public static IEnumerator RunStartNewGame()
		{
			return GameManager.instance.RunStartNewGame();
		}

		public static void ContinueGame()
		{
			GameManager.instance.ContinueGame();
		}

		public static IEnumerator RunContinueGame()
		{
			return GameManager.instance.RunContinueGame();
		}

		public static IEnumerator ReturnToMainMenu(ReturnToMainMenuSaveModes saveMode, Action<bool> callback = null)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return GameManager.instance.ReturnToMainMenu(saveMode, callback);
		}

		public static void WillTerminateInBackground()
		{
			GameManager.instance.WillTerminateInBackground();
		}

		public static void Platform_IDisengageHandler_OnDisengage(Action next)
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "Platform.IDisengageHandler.OnDisengage", new object[1] { next });
		}

		public static ControllerConnectionStates GetControllerConnectionState()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return GameManager.instance.GetControllerConnectionState();
		}

		public static void EmergencyReturnToMenu(Action callback)
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "EmergencyReturnToMenu", new object[1] { callback });
		}

		public static IEnumerator QuitGame()
		{
			return GameManager.instance.QuitGame();
		}

		public static void LoadedBoss()
		{
			GameManager.instance.LoadedBoss();
		}

		public static void DoDestroyPersonalPools()
		{
			GameManager.instance.DoDestroyPersonalPools();
		}

		public static float GetImplicitCinematicVolume()
		{
			return GameManager.instance.GetImplicitCinematicVolume();
		}

		public static UIManager orig_get_ui()
		{
			return GameManager.instance.orig_get_ui();
		}

		public static void orig_set_ui(UIManager value)
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "orig_set_ui", new object[1] { value });
		}

		public static void orig_OnApplicationQuit()
		{
			ReflectionHelper.CallMethod<GameManager>(GameManager.instance, "orig_OnApplicationQuit", Array.Empty<object>());
		}

		public static void orig_LoadScene(string destScene)
		{
			GameManager.instance.orig_LoadScene(destScene);
		}

		public static void orig_BeginSceneTransition(SceneLoadInfo info)
		{
			GameManager.instance.orig_BeginSceneTransition(info);
		}

		public static void orig_ClearSaveFile(int saveSlot, Action<bool> callback)
		{
			GameManager.instance.orig_ClearSaveFile(saveSlot, callback);
		}

		public static IEnumerator orig_PlayerDead(float waitTime)
		{
			return GameManager.instance.orig_PlayerDead(waitTime);
		}

		public static void orig_SetupSceneRefs(bool refreshTilemapInfo)
		{
			GameManager.instance.orig_SetupSceneRefs(refreshTilemapInfo);
		}

		public static void orig_OnWillActivateFirstLevel()
		{
			GameManager.instance.orig_OnWillActivateFirstLevel();
		}

		public static IEnumerator PauseToggleDynamicMenu(MenuScreen screen, bool allowUnpause = false)
		{
			return GameManager.instance.PauseToggleDynamicMenu(screen, allowUnpause);
		}

		public static string GetBaseSceneName(string fullSceneName)
		{
			return GameManager.GetBaseSceneName(fullSceneName);
		}

		public static tk2dTileMap GetTileMap(GameObject gameObject)
		{
			return ReflectionHelper.CallMethod<GameManager, tk2dTileMap>("GetTileMap", new object[1] { gameObject });
		}

		public static string GetSceneZoneName(string str)
		{
			return ReflectionHelper.CallMethod<GameManager, string>("GetSceneZoneName", new object[1] { str });
		}

		public static int CountBits(int val)
		{
			return ReflectionHelper.CallMethod<GameManager, int>("CountBits", new object[1] { val });
		}

		public static string ModdedSavePath(int slot)
		{
			return ReflectionHelper.CallMethod<GameManager, string>("ModdedSavePath", new object[1] { slot });
		}
	}
	[Obsolete("Please use HKMirror", true)]
	public class GameManagerILIEnumerators
	{
		public static event Manipulator BeginSceneTransitionRoutine
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("BeginSceneTransitionRoutine", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("BeginSceneTransitionRoutine", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator TransitionScene
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("TransitionScene", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("TransitionScene", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator PlayerDead
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PlayerDead", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PlayerDead", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator PlayerDeadFromHazard
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PlayerDeadFromHazard", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PlayerDeadFromHazard", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator FadeSceneInWithDelay
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("FadeSceneInWithDelay", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("FadeSceneInWithDelay", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator LoadSceneAdditive
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("LoadSceneAdditive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("LoadSceneAdditive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator LoadFirstScene
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("LoadFirstScene", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("LoadFirstScene", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator SkipCutsceneNoMash
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("SkipCutsceneNoMash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("SkipCutsceneNoMash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator LoadGameFromUIRoutine
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("LoadGameFromUIRoutine", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("LoadGameFromUIRoutine", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator PauseGameToggleByMenu
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PauseGameToggleByMenu", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PauseGameToggleByMenu", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator PauseGameToggle
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PauseGameToggle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PauseGameToggle", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator SetTimeScale
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("SetTimeScale", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("SetTimeScale", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator FreezeMoment
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("FreezeMoment", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("FreezeMoment", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator FreezeMomentGC
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("FreezeMomentGC", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("FreezeMomentGC", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator RunStartNewGame
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("RunStartNewGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("RunStartNewGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator RunContinueGame
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("RunContinueGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("RunContinueGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator ReturnToMainMenu
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("ReturnToMainMenu", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("ReturnToMainMenu", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator QuitGame
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("QuitGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("QuitGame", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator orig_PlayerDead
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("orig_PlayerDead", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("orig_PlayerDead", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}

		public static event Manipulator PauseToggleDynamicMenu
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PauseToggleDynamicMenu", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(typeof(GameManager).GetMethod("PauseToggleDynamicMenu", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)), (Delegate)(object)value);
			}
		}
	}
	[Obsolete("Please use HKMirror", true)]
	public static class HeroControllerR
	{
		public static bool verboseMode
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "verboseMode");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "verboseMode", value);
			}
		}

		public static HeroType heroType
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.heroType;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.heroType = value;
			}
		}

		public static float RUN_SPEED
		{
			get
			{
				return HeroController.instance.RUN_SPEED;
			}
			set
			{
				HeroController.instance.RUN_SPEED = value;
			}
		}

		public static float RUN_SPEED_CH
		{
			get
			{
				return HeroController.instance.RUN_SPEED_CH;
			}
			set
			{
				HeroController.instance.RUN_SPEED_CH = value;
			}
		}

		public static float RUN_SPEED_CH_COMBO
		{
			get
			{
				return HeroController.instance.RUN_SPEED_CH_COMBO;
			}
			set
			{
				HeroController.instance.RUN_SPEED_CH_COMBO = value;
			}
		}

		public static float WALK_SPEED
		{
			get
			{
				return HeroController.instance.WALK_SPEED;
			}
			set
			{
				HeroController.instance.WALK_SPEED = value;
			}
		}

		public static float UNDERWATER_SPEED
		{
			get
			{
				return HeroController.instance.UNDERWATER_SPEED;
			}
			set
			{
				HeroController.instance.UNDERWATER_SPEED = value;
			}
		}

		public static float JUMP_SPEED
		{
			get
			{
				return HeroController.instance.JUMP_SPEED;
			}
			set
			{
				HeroController.instance.JUMP_SPEED = value;
			}
		}

		public static float JUMP_SPEED_UNDERWATER
		{
			get
			{
				return HeroController.instance.JUMP_SPEED_UNDERWATER;
			}
			set
			{
				HeroController.instance.JUMP_SPEED_UNDERWATER = value;
			}
		}

		public static float MIN_JUMP_SPEED
		{
			get
			{
				return HeroController.instance.MIN_JUMP_SPEED;
			}
			set
			{
				HeroController.instance.MIN_JUMP_SPEED = value;
			}
		}

		public static int JUMP_STEPS
		{
			get
			{
				return HeroController.instance.JUMP_STEPS;
			}
			set
			{
				HeroController.instance.JUMP_STEPS = value;
			}
		}

		public static int JUMP_STEPS_MIN
		{
			get
			{
				return HeroController.instance.JUMP_STEPS_MIN;
			}
			set
			{
				HeroController.instance.JUMP_STEPS_MIN = value;
			}
		}

		public static int JUMP_TIME
		{
			get
			{
				return HeroController.instance.JUMP_TIME;
			}
			set
			{
				HeroController.instance.JUMP_TIME = value;
			}
		}

		public static int DOUBLE_JUMP_STEPS
		{
			get
			{
				return HeroController.instance.DOUBLE_JUMP_STEPS;
			}
			set
			{
				HeroController.instance.DOUBLE_JUMP_STEPS = value;
			}
		}

		public static int WJLOCK_STEPS_SHORT
		{
			get
			{
				return HeroController.instance.WJLOCK_STEPS_SHORT;
			}
			set
			{
				HeroController.instance.WJLOCK_STEPS_SHORT = value;
			}
		}

		public static int WJLOCK_STEPS_LONG
		{
			get
			{
				return HeroController.instance.WJLOCK_STEPS_LONG;
			}
			set
			{
				HeroController.instance.WJLOCK_STEPS_LONG = value;
			}
		}

		public static float WJ_KICKOFF_SPEED
		{
			get
			{
				return HeroController.instance.WJ_KICKOFF_SPEED;
			}
			set
			{
				HeroController.instance.WJ_KICKOFF_SPEED = value;
			}
		}

		public static int WALL_STICKY_STEPS
		{
			get
			{
				return HeroController.instance.WALL_STICKY_STEPS;
			}
			set
			{
				HeroController.instance.WALL_STICKY_STEPS = value;
			}
		}

		public static float DASH_SPEED
		{
			get
			{
				return HeroController.instance.DASH_SPEED;
			}
			set
			{
				HeroController.instance.DASH_SPEED = value;
			}
		}

		public static float DASH_SPEED_SHARP
		{
			get
			{
				return HeroController.instance.DASH_SPEED_SHARP;
			}
			set
			{
				HeroController.instance.DASH_SPEED_SHARP = value;
			}
		}

		public static float DASH_TIME
		{
			get
			{
				return HeroController.instance.DASH_TIME;
			}
			set
			{
				HeroController.instance.DASH_TIME = value;
			}
		}

		public static int DASH_QUEUE_STEPS
		{
			get
			{
				return HeroController.instance.DASH_QUEUE_STEPS;
			}
			set
			{
				HeroController.instance.DASH_QUEUE_STEPS = value;
			}
		}

		public static float BACK_DASH_SPEED
		{
			get
			{
				return HeroController.instance.BACK_DASH_SPEED;
			}
			set
			{
				HeroController.instance.BACK_DASH_SPEED = value;
			}
		}

		public static float BACK_DASH_TIME
		{
			get
			{
				return HeroController.instance.BACK_DASH_TIME;
			}
			set
			{
				HeroController.instance.BACK_DASH_TIME = value;
			}
		}

		public static float SHADOW_DASH_SPEED
		{
			get
			{
				return HeroController.instance.SHADOW_DASH_SPEED;
			}
			set
			{
				HeroController.instance.SHADOW_DASH_SPEED = value;
			}
		}

		public static float SHADOW_DASH_TIME
		{
			get
			{
				return HeroController.instance.SHADOW_DASH_TIME;
			}
			set
			{
				HeroController.instance.SHADOW_DASH_TIME = value;
			}
		}

		public static float SHADOW_DASH_COOLDOWN
		{
			get
			{
				return HeroController.instance.SHADOW_DASH_COOLDOWN;
			}
			set
			{
				HeroController.instance.SHADOW_DASH_COOLDOWN = value;
			}
		}

		public static float SUPER_DASH_SPEED
		{
			get
			{
				return HeroController.instance.SUPER_DASH_SPEED;
			}
			set
			{
				HeroController.instance.SUPER_DASH_SPEED = value;
			}
		}

		public static float DASH_COOLDOWN
		{
			get
			{
				return HeroController.instance.DASH_COOLDOWN;
			}
			set
			{
				HeroController.instance.DASH_COOLDOWN = value;
			}
		}

		public static float DASH_COOLDOWN_CH
		{
			get
			{
				return HeroController.instance.DASH_COOLDOWN_CH;
			}
			set
			{
				HeroController.instance.DASH_COOLDOWN_CH = value;
			}
		}

		public static float BACKDASH_COOLDOWN
		{
			get
			{
				return HeroController.instance.BACKDASH_COOLDOWN;
			}
			set
			{
				HeroController.instance.BACKDASH_COOLDOWN = value;
			}
		}

		public static float WALLSLIDE_SPEED
		{
			get
			{
				return HeroController.instance.WALLSLIDE_SPEED;
			}
			set
			{
				HeroController.instance.WALLSLIDE_SPEED = value;
			}
		}

		public static float WALLSLIDE_DECEL
		{
			get
			{
				return HeroController.instance.WALLSLIDE_DECEL;
			}
			set
			{
				HeroController.instance.WALLSLIDE_DECEL = value;
			}
		}

		public static float NAIL_CHARGE_TIME_DEFAULT
		{
			get
			{
				return HeroController.instance.NAIL_CHARGE_TIME_DEFAULT;
			}
			set
			{
				HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = value;
			}
		}

		public static float NAIL_CHARGE_TIME_CHARM
		{
			get
			{
				return HeroController.instance.NAIL_CHARGE_TIME_CHARM;
			}
			set
			{
				HeroController.instance.NAIL_CHARGE_TIME_CHARM = value;
			}
		}

		public static float CYCLONE_HORIZONTAL_SPEED
		{
			get
			{
				return HeroController.instance.CYCLONE_HORIZONTAL_SPEED;
			}
			set
			{
				HeroController.instance.CYCLONE_HORIZONTAL_SPEED = value;
			}
		}

		public static float SWIM_ACCEL
		{
			get
			{
				return HeroController.instance.SWIM_ACCEL;
			}
			set
			{
				HeroController.instance.SWIM_ACCEL = value;
			}
		}

		public static float SWIM_MAX_SPEED
		{
			get
			{
				return HeroController.instance.SWIM_MAX_SPEED;
			}
			set
			{
				HeroController.instance.SWIM_MAX_SPEED = value;
			}
		}

		public static float TIME_TO_ENTER_SCENE_BOT
		{
			get
			{
				return HeroController.instance.TIME_TO_ENTER_SCENE_BOT;
			}
			set
			{
				HeroController.instance.TIME_TO_ENTER_SCENE_BOT = value;
			}
		}

		public static float TIME_TO_ENTER_SCENE_HOR
		{
			get
			{
				return HeroController.instance.TIME_TO_ENTER_SCENE_HOR;
			}
			set
			{
				HeroController.instance.TIME_TO_ENTER_SCENE_HOR = value;
			}
		}

		public static float SPEED_TO_ENTER_SCENE_HOR
		{
			get
			{
				return HeroController.instance.SPEED_TO_ENTER_SCENE_HOR;
			}
			set
			{
				HeroController.instance.SPEED_TO_ENTER_SCENE_HOR = value;
			}
		}

		public static float SPEED_TO_ENTER_SCENE_UP
		{
			get
			{
				return HeroController.instance.SPEED_TO_ENTER_SCENE_UP;
			}
			set
			{
				HeroController.instance.SPEED_TO_ENTER_SCENE_UP = value;
			}
		}

		public static float SPEED_TO_ENTER_SCENE_DOWN
		{
			get
			{
				return HeroController.instance.SPEED_TO_ENTER_SCENE_DOWN;
			}
			set
			{
				HeroController.instance.SPEED_TO_ENTER_SCENE_DOWN = value;
			}
		}

		public static float DEFAULT_GRAVITY
		{
			get
			{
				return HeroController.instance.DEFAULT_GRAVITY;
			}
			set
			{
				HeroController.instance.DEFAULT_GRAVITY = value;
			}
		}

		public static float UNDERWATER_GRAVITY
		{
			get
			{
				return HeroController.instance.UNDERWATER_GRAVITY;
			}
			set
			{
				HeroController.instance.UNDERWATER_GRAVITY = value;
			}
		}

		public static float ATTACK_DURATION
		{
			get
			{
				return HeroController.instance.ATTACK_DURATION;
			}
			set
			{
				HeroController.instance.ATTACK_DURATION = value;
			}
		}

		public static float ATTACK_DURATION_CH
		{
			get
			{
				return HeroController.instance.ATTACK_DURATION_CH;
			}
			set
			{
				HeroController.instance.ATTACK_DURATION_CH = value;
			}
		}

		public static float ALT_ATTACK_RESET
		{
			get
			{
				return HeroController.instance.ALT_ATTACK_RESET;
			}
			set
			{
				HeroController.instance.ALT_ATTACK_RESET = value;
			}
		}

		public static float ATTACK_RECOVERY_TIME
		{
			get
			{
				return HeroController.instance.ATTACK_RECOVERY_TIME;
			}
			set
			{
				HeroController.instance.ATTACK_RECOVERY_TIME = value;
			}
		}

		public static float ATTACK_COOLDOWN_TIME
		{
			get
			{
				return HeroController.instance.ATTACK_COOLDOWN_TIME;
			}
			set
			{
				HeroController.instance.ATTACK_COOLDOWN_TIME = value;
			}
		}

		public static float ATTACK_COOLDOWN_TIME_CH
		{
			get
			{
				return HeroController.instance.ATTACK_COOLDOWN_TIME_CH;
			}
			set
			{
				HeroController.instance.ATTACK_COOLDOWN_TIME_CH = value;
			}
		}

		public static float BOUNCE_TIME
		{
			get
			{
				return HeroController.instance.BOUNCE_TIME;
			}
			set
			{
				HeroController.instance.BOUNCE_TIME = value;
			}
		}

		public static float BOUNCE_SHROOM_TIME
		{
			get
			{
				return HeroController.instance.BOUNCE_SHROOM_TIME;
			}
			set
			{
				HeroController.instance.BOUNCE_SHROOM_TIME = value;
			}
		}

		public static float BOUNCE_VELOCITY
		{
			get
			{
				return HeroController.instance.BOUNCE_VELOCITY;
			}
			set
			{
				HeroController.instance.BOUNCE_VELOCITY = value;
			}
		}

		public static float SHROOM_BOUNCE_VELOCITY
		{
			get
			{
				return HeroController.instance.SHROOM_BOUNCE_VELOCITY;
			}
			set
			{
				HeroController.instance.SHROOM_BOUNCE_VELOCITY = value;
			}
		}

		public static float RECOIL_HOR_TIME
		{
			get
			{
				return HeroController.instance.RECOIL_HOR_TIME;
			}
			set
			{
				HeroController.instance.RECOIL_HOR_TIME = value;
			}
		}

		public static float RECOIL_HOR_VELOCITY
		{
			get
			{
				return HeroController.instance.RECOIL_HOR_VELOCITY;
			}
			set
			{
				HeroController.instance.RECOIL_HOR_VELOCITY = value;
			}
		}

		public static float RECOIL_HOR_VELOCITY_LONG
		{
			get
			{
				return HeroController.instance.RECOIL_HOR_VELOCITY_LONG;
			}
			set
			{
				HeroController.instance.RECOIL_HOR_VELOCITY_LONG = value;
			}
		}

		public static float RECOIL_HOR_STEPS
		{
			get
			{
				return HeroController.instance.RECOIL_HOR_STEPS;
			}
			set
			{
				HeroController.instance.RECOIL_HOR_STEPS = value;
			}
		}

		public static float RECOIL_DOWN_VELOCITY
		{
			get
			{
				return HeroController.instance.RECOIL_DOWN_VELOCITY;
			}
			set
			{
				HeroController.instance.RECOIL_DOWN_VELOCITY = value;
			}
		}

		public static float RUN_PUFF_TIME
		{
			get
			{
				return HeroController.instance.RUN_PUFF_TIME;
			}
			set
			{
				HeroController.instance.RUN_PUFF_TIME = value;
			}
		}

		public static float BIG_FALL_TIME
		{
			get
			{
				return HeroController.instance.BIG_FALL_TIME;
			}
			set
			{
				HeroController.instance.BIG_FALL_TIME = value;
			}
		}

		public static float HARD_LANDING_TIME
		{
			get
			{
				return HeroController.instance.HARD_LANDING_TIME;
			}
			set
			{
				HeroController.instance.HARD_LANDING_TIME = value;
			}
		}

		public static float DOWN_DASH_TIME
		{
			get
			{
				return HeroController.instance.DOWN_DASH_TIME;
			}
			set
			{
				HeroController.instance.DOWN_DASH_TIME = value;
			}
		}

		public static float MAX_FALL_VELOCITY
		{
			get
			{
				return HeroController.instance.MAX_FALL_VELOCITY;
			}
			set
			{
				HeroController.instance.MAX_FALL_VELOCITY = value;
			}
		}

		public static float MAX_FALL_VELOCITY_UNDERWATER
		{
			get
			{
				return HeroController.instance.MAX_FALL_VELOCITY_UNDERWATER;
			}
			set
			{
				HeroController.instance.MAX_FALL_VELOCITY_UNDERWATER = value;
			}
		}

		public static float RECOIL_DURATION
		{
			get
			{
				return HeroController.instance.RECOIL_DURATION;
			}
			set
			{
				HeroController.instance.RECOIL_DURATION = value;
			}
		}

		public static float RECOIL_DURATION_STAL
		{
			get
			{
				return HeroController.instance.RECOIL_DURATION_STAL;
			}
			set
			{
				HeroController.instance.RECOIL_DURATION_STAL = value;
			}
		}

		public static float RECOIL_VELOCITY
		{
			get
			{
				return HeroController.instance.RECOIL_VELOCITY;
			}
			set
			{
				HeroController.instance.RECOIL_VELOCITY = value;
			}
		}

		public static float DAMAGE_FREEZE_DOWN
		{
			get
			{
				return HeroController.instance.DAMAGE_FREEZE_DOWN;
			}
			set
			{
				HeroController.instance.DAMAGE_FREEZE_DOWN = value;
			}
		}

		public static float DAMAGE_FREEZE_WAIT
		{
			get
			{
				return HeroController.instance.DAMAGE_FREEZE_WAIT;
			}
			set
			{
				HeroController.instance.DAMAGE_FREEZE_WAIT = value;
			}
		}

		public static float DAMAGE_FREEZE_UP
		{
			get
			{
				return HeroController.instance.DAMAGE_FREEZE_UP;
			}
			set
			{
				HeroController.instance.DAMAGE_FREEZE_UP = value;
			}
		}

		public static float INVUL_TIME
		{
			get
			{
				return HeroController.instance.INVUL_TIME;
			}
			set
			{
				HeroController.instance.INVUL_TIME = value;
			}
		}

		public static float INVUL_TIME_STAL
		{
			get
			{
				return HeroController.instance.INVUL_TIME_STAL;
			}
			set
			{
				HeroController.instance.INVUL_TIME_STAL = value;
			}
		}

		public static float INVUL_TIME_PARRY
		{
			get
			{
				return HeroController.instance.INVUL_TIME_PARRY;
			}
			set
			{
				HeroController.instance.INVUL_TIME_PARRY = value;
			}
		}

		public static float INVUL_TIME_QUAKE
		{
			get
			{
				return HeroController.instance.INVUL_TIME_QUAKE;
			}
			set
			{
				HeroController.instance.INVUL_TIME_QUAKE = value;
			}
		}

		public static float INVUL_TIME_CYCLONE
		{
			get
			{
				return HeroController.instance.INVUL_TIME_CYCLONE;
			}
			set
			{
				HeroController.instance.INVUL_TIME_CYCLONE = value;
			}
		}

		public static float CAST_TIME
		{
			get
			{
				return HeroController.instance.CAST_TIME;
			}
			set
			{
				HeroController.instance.CAST_TIME = value;
			}
		}

		public static float CAST_RECOIL_TIME
		{
			get
			{
				return HeroController.instance.CAST_RECOIL_TIME;
			}
			set
			{
				HeroController.instance.CAST_RECOIL_TIME = value;
			}
		}

		public static float CAST_RECOIL_VELOCITY
		{
			get
			{
				return HeroController.instance.CAST_RECOIL_VELOCITY;
			}
			set
			{
				HeroController.instance.CAST_RECOIL_VELOCITY = value;
			}
		}

		public static float WALLSLIDE_CLIP_DELAY
		{
			get
			{
				return HeroController.instance.WALLSLIDE_CLIP_DELAY;
			}
			set
			{
				HeroController.instance.WALLSLIDE_CLIP_DELAY = value;
			}
		}

		public static int GRUB_SOUL_MP
		{
			get
			{
				return HeroController.instance.GRUB_SOUL_MP;
			}
			set
			{
				HeroController.instance.GRUB_SOUL_MP = value;
			}
		}

		public static int GRUB_SOUL_MP_COMBO
		{
			get
			{
				return HeroController.instance.GRUB_SOUL_MP_COMBO;
			}
			set
			{
				HeroController.instance.GRUB_SOUL_MP_COMBO = value;
			}
		}

		public static int JUMP_QUEUE_STEPS
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "JUMP_QUEUE_STEPS");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "JUMP_QUEUE_STEPS", value);
			}
		}

		public static int JUMP_RELEASE_QUEUE_STEPS
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "JUMP_RELEASE_QUEUE_STEPS");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "JUMP_RELEASE_QUEUE_STEPS", value);
			}
		}

		public static int DOUBLE_JUMP_QUEUE_STEPS
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "DOUBLE_JUMP_QUEUE_STEPS");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "DOUBLE_JUMP_QUEUE_STEPS", value);
			}
		}

		public static int ATTACK_QUEUE_STEPS
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "ATTACK_QUEUE_STEPS");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "ATTACK_QUEUE_STEPS", value);
			}
		}

		public static float DELAY_BEFORE_ENTER
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "DELAY_BEFORE_ENTER");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "DELAY_BEFORE_ENTER", value);
			}
		}

		public static float LOOK_DELAY
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "LOOK_DELAY");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "LOOK_DELAY", value);
			}
		}

		public static float LOOK_ANIM_DELAY
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "LOOK_ANIM_DELAY");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "LOOK_ANIM_DELAY", value);
			}
		}

		public static float DEATH_WAIT
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "DEATH_WAIT");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "DEATH_WAIT", value);
			}
		}

		public static float HAZARD_DEATH_CHECK_TIME
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "HAZARD_DEATH_CHECK_TIME");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "HAZARD_DEATH_CHECK_TIME", value);
			}
		}

		public static float FLOATING_CHECK_TIME
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "FLOATING_CHECK_TIME");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "FLOATING_CHECK_TIME", value);
			}
		}

		public static float NAIL_TERRAIN_CHECK_TIME
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "NAIL_TERRAIN_CHECK_TIME");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "NAIL_TERRAIN_CHECK_TIME", value);
			}
		}

		public static float BUMP_VELOCITY
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "BUMP_VELOCITY");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "BUMP_VELOCITY", value);
			}
		}

		public static float BUMP_VELOCITY_DASH
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "BUMP_VELOCITY_DASH");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "BUMP_VELOCITY_DASH", value);
			}
		}

		public static int LANDING_BUFFER_STEPS
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "LANDING_BUFFER_STEPS");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "LANDING_BUFFER_STEPS", value);
			}
		}

		public static int LEDGE_BUFFER_STEPS
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "LEDGE_BUFFER_STEPS");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "LEDGE_BUFFER_STEPS", value);
			}
		}

		public static int HEAD_BUMP_STEPS
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "HEAD_BUMP_STEPS");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "HEAD_BUMP_STEPS", value);
			}
		}

		public static float MANTIS_CHARM_SCALE
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "MANTIS_CHARM_SCALE");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "MANTIS_CHARM_SCALE", value);
			}
		}

		public static float FIND_GROUND_POINT_DISTANCE
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "FIND_GROUND_POINT_DISTANCE");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "FIND_GROUND_POINT_DISTANCE", value);
			}
		}

		public static float FIND_GROUND_POINT_DISTANCE_EXT
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "FIND_GROUND_POINT_DISTANCE_EXT");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "FIND_GROUND_POINT_DISTANCE_EXT", value);
			}
		}

		public static ActorStates hero_state
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.hero_state;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.hero_state = value;
			}
		}

		public static ActorStates prev_hero_state
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.prev_hero_state;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.prev_hero_state = value;
			}
		}

		public static HeroTransitionState transitionState
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.transitionState;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.transitionState = value;
			}
		}

		public static DamageMode damageMode
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.damageMode;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.damageMode = value;
			}
		}

		public static float move_input
		{
			get
			{
				return HeroController.instance.move_input;
			}
			set
			{
				HeroController.instance.move_input = value;
			}
		}

		public static float vertical_input
		{
			get
			{
				return HeroController.instance.vertical_input;
			}
			set
			{
				HeroController.instance.vertical_input = value;
			}
		}

		public static float controller_deadzone
		{
			get
			{
				return HeroController.instance.controller_deadzone;
			}
			set
			{
				HeroController.instance.controller_deadzone = value;
			}
		}

		public static Vector2 current_velocity
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.current_velocity;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.current_velocity = value;
			}
		}

		public static bool isGameplayScene
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "isGameplayScene");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "isGameplayScene", value);
			}
		}

		public static bool isEnteringFirstLevel
		{
			get
			{
				return HeroController.instance.isEnteringFirstLevel;
			}
			set
			{
				HeroController.instance.isEnteringFirstLevel = value;
			}
		}

		public static Vector2 slashOffset
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.slashOffset;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.slashOffset = value;
			}
		}

		public static Vector2 upSlashOffset
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.upSlashOffset;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.upSlashOffset = value;
			}
		}

		public static Vector2 downwardSlashOffset
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.downwardSlashOffset;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.downwardSlashOffset = value;
			}
		}

		public static Vector2 spell1Offset
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.spell1Offset;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.spell1Offset = value;
			}
		}

		public static int jump_steps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "jump_steps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "jump_steps", value);
			}
		}

		public static int jumped_steps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "jumped_steps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "jumped_steps", value);
			}
		}

		public static int doubleJump_steps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "doubleJump_steps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "doubleJump_steps", value);
			}
		}

		public static float dash_timer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "dash_timer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "dash_timer", value);
			}
		}

		public static float back_dash_timer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "back_dash_timer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "back_dash_timer", value);
			}
		}

		public static float shadow_dash_timer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "shadow_dash_timer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "shadow_dash_timer", value);
			}
		}

		public static float attack_time
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "attack_time");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "attack_time", value);
			}
		}

		public static float attack_cooldown
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "attack_cooldown");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "attack_cooldown", value);
			}
		}

		public static Vector2 transition_vel
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<HeroController, Vector2>(HeroController.instance, "transition_vel");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<HeroController, Vector2>(HeroController.instance, "transition_vel", value);
			}
		}

		public static float altAttackTime
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "altAttackTime");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "altAttackTime", value);
			}
		}

		public static float lookDelayTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "lookDelayTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "lookDelayTimer", value);
			}
		}

		public static float bounceTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "bounceTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "bounceTimer", value);
			}
		}

		public static float recoilHorizontalTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "recoilHorizontalTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "recoilHorizontalTimer", value);
			}
		}

		public static float runPuffTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "runPuffTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "runPuffTimer", value);
			}
		}

		public static float fallTimer
		{
			get
			{
				return HeroController.instance.fallTimer;
			}
			set
			{
				ReflectionHelper.SetProperty<HeroController, float>(HeroController.instance, "fallTimer", value);
			}
		}

		public static float hardLandingTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "hardLandingTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "hardLandingTimer", value);
			}
		}

		public static float dashLandingTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "dashLandingTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "dashLandingTimer", value);
			}
		}

		public static float recoilTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "recoilTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "recoilTimer", value);
			}
		}

		public static int recoilSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "recoilSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "recoilSteps", value);
			}
		}

		public static int landingBufferSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "landingBufferSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "landingBufferSteps", value);
			}
		}

		public static int dashQueueSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "dashQueueSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "dashQueueSteps", value);
			}
		}

		public static bool dashQueuing
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "dashQueuing");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "dashQueuing", value);
			}
		}

		public static float shadowDashTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "shadowDashTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "shadowDashTimer", value);
			}
		}

		public static float dashCooldownTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "dashCooldownTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "dashCooldownTimer", value);
			}
		}

		public static float nailChargeTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "nailChargeTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "nailChargeTimer", value);
			}
		}

		public static int wallLockSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "wallLockSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "wallLockSteps", value);
			}
		}

		public static float wallslideClipTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "wallslideClipTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "wallslideClipTimer", value);
			}
		}

		public static float hardLandFailSafeTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "hardLandFailSafeTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "hardLandFailSafeTimer", value);
			}
		}

		public static float hazardDeathTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "hazardDeathTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "hazardDeathTimer", value);
			}
		}

		public static float floatingBufferTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "floatingBufferTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "floatingBufferTimer", value);
			}
		}

		public static float attackDuration
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "attackDuration");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "attackDuration", value);
			}
		}

		public static float parryInvulnTimer
		{
			get
			{
				return HeroController.instance.parryInvulnTimer;
			}
			set
			{
				HeroController.instance.parryInvulnTimer = value;
			}
		}

		public static GameObject slashPrefab
		{
			get
			{
				return HeroController.instance.slashPrefab;
			}
			set
			{
				HeroController.instance.slashPrefab = value;
			}
		}

		public static GameObject slashAltPrefab
		{
			get
			{
				return HeroController.instance.slashAltPrefab;
			}
			set
			{
				HeroController.instance.slashAltPrefab = value;
			}
		}

		public static GameObject upSlashPrefab
		{
			get
			{
				return HeroController.instance.upSlashPrefab;
			}
			set
			{
				HeroController.instance.upSlashPrefab = value;
			}
		}

		public static GameObject downSlashPrefab
		{
			get
			{
				return HeroController.instance.downSlashPrefab;
			}
			set
			{
				HeroController.instance.downSlashPrefab = value;
			}
		}

		public static GameObject wallSlashPrefab
		{
			get
			{
				return HeroController.instance.wallSlashPrefab;
			}
			set
			{
				HeroController.instance.wallSlashPrefab = value;
			}
		}

		public static NailSlash normalSlash
		{
			get
			{
				return HeroController.instance.normalSlash;
			}
			set
			{
				HeroController.instance.normalSlash = value;
			}
		}

		public static NailSlash alternateSlash
		{
			get
			{
				return HeroController.instance.alternateSlash;
			}
			set
			{
				HeroController.instance.alternateSlash = value;
			}
		}

		public static NailSlash upSlash
		{
			get
			{
				return HeroController.instance.upSlash;
			}
			set
			{
				HeroController.instance.upSlash = value;
			}
		}

		public static NailSlash downSlash
		{
			get
			{
				return HeroController.instance.downSlash;
			}
			set
			{
				HeroController.instance.downSlash = value;
			}
		}

		public static NailSlash wallSlash
		{
			get
			{
				return HeroController.instance.wallSlash;
			}
			set
			{
				HeroController.instance.wallSlash = value;
			}
		}

		public static PlayMakerFSM normalSlashFsm
		{
			get
			{
				return HeroController.instance.normalSlashFsm;
			}
			set
			{
				HeroController.instance.normalSlashFsm = value;
			}
		}

		public static PlayMakerFSM alternateSlashFsm
		{
			get
			{
				return HeroController.instance.alternateSlashFsm;
			}
			set
			{
				HeroController.instance.alternateSlashFsm = value;
			}
		}

		public static PlayMakerFSM upSlashFsm
		{
			get
			{
				return HeroController.instance.upSlashFsm;
			}
			set
			{
				HeroController.instance.upSlashFsm = value;
			}
		}

		public static PlayMakerFSM downSlashFsm
		{
			get
			{
				return HeroController.instance.downSlashFsm;
			}
			set
			{
				HeroController.instance.downSlashFsm = value;
			}
		}

		public static PlayMakerFSM wallSlashFsm
		{
			get
			{
				return HeroController.instance.wallSlashFsm;
			}
			set
			{
				HeroController.instance.wallSlashFsm = value;
			}
		}

		public static GameObject nailTerrainImpactEffectPrefab
		{
			get
			{
				return HeroController.instance.nailTerrainImpactEffectPrefab;
			}
			set
			{
				HeroController.instance.nailTerrainImpactEffectPrefab = value;
			}
		}

		public static GameObject spell1Prefab
		{
			get
			{
				return HeroController.instance.spell1Prefab;
			}
			set
			{
				HeroController.instance.spell1Prefab = value;
			}
		}

		public static GameObject takeHitPrefab
		{
			get
			{
				return HeroController.instance.takeHitPrefab;
			}
			set
			{
				HeroController.instance.takeHitPrefab = value;
			}
		}

		public static GameObject takeHitDoublePrefab
		{
			get
			{
				return HeroController.instance.takeHitDoublePrefab;
			}
			set
			{
				HeroController.instance.takeHitDoublePrefab = value;
			}
		}

		public static GameObject softLandingEffectPrefab
		{
			get
			{
				return HeroController.instance.softLandingEffectPrefab;
			}
			set
			{
				HeroController.instance.softLandingEffectPrefab = value;
			}
		}

		public static GameObject hardLandingEffectPrefab
		{
			get
			{
				return HeroController.instance.hardLandingEffectPrefab;
			}
			set
			{
				HeroController.instance.hardLandingEffectPrefab = value;
			}
		}

		public static GameObject runEffectPrefab
		{
			get
			{
				return HeroController.instance.runEffectPrefab;
			}
			set
			{
				HeroController.instance.runEffectPrefab = value;
			}
		}

		public static GameObject backDashPrefab
		{
			get
			{
				return HeroController.instance.backDashPrefab;
			}
			set
			{
				HeroController.instance.backDashPrefab = value;
			}
		}

		public static GameObject jumpEffectPrefab
		{
			get
			{
				return HeroController.instance.jumpEffectPrefab;
			}
			set
			{
				HeroController.instance.jumpEffectPrefab = value;
			}
		}

		public static GameObject jumpTrailPrefab
		{
			get
			{
				return HeroController.instance.jumpTrailPrefab;
			}
			set
			{
				HeroController.instance.jumpTrailPrefab = value;
			}
		}

		public static GameObject fallEffectPrefab
		{
			get
			{
				return HeroController.instance.fallEffectPrefab;
			}
			set
			{
				HeroController.instance.fallEffectPrefab = value;
			}
		}

		public static ParticleSystem wallslideDustPrefab
		{
			get
			{
				return HeroController.instance.wallslideDustPrefab;
			}
			set
			{
				HeroController.instance.wallslideDustPrefab = value;
			}
		}

		public static GameObject artChargeEffect
		{
			get
			{
				return HeroController.instance.artChargeEffect;
			}
			set
			{
				HeroController.instance.artChargeEffect = value;
			}
		}

		public static GameObject artChargedEffect
		{
			get
			{
				return HeroController.instance.artChargedEffect;
			}
			set
			{
				HeroController.instance.artChargedEffect = value;
			}
		}

		public static GameObject artChargedFlash
		{
			get
			{
				return HeroController.instance.artChargedFlash;
			}
			set
			{
				HeroController.instance.artChargedFlash = value;
			}
		}

		public static tk2dSpriteAnimator artChargedEffectAnim
		{
			get
			{
				return HeroController.instance.artChargedEffectAnim;
			}
			set
			{
				HeroController.instance.artChargedEffectAnim = value;
			}
		}

		public static GameObject shadowdashBurstPrefab
		{
			get
			{
				return HeroController.instance.shadowdashBurstPrefab;
			}
			set
			{
				HeroController.instance.shadowdashBurstPrefab = value;
			}
		}

		public static GameObject shadowdashDownBurstPrefab
		{
			get
			{
				return HeroController.instance.shadowdashDownBurstPrefab;
			}
			set
			{
				HeroController.instance.shadowdashDownBurstPrefab = value;
			}
		}

		public static GameObject dashParticlesPrefab
		{
			get
			{
				return HeroController.instance.dashParticlesPrefab;
			}
			set
			{
				HeroController.instance.dashParticlesPrefab = value;
			}
		}

		public static GameObject shadowdashParticlesPrefab
		{
			get
			{
				return HeroController.instance.shadowdashParticlesPrefab;
			}
			set
			{
				HeroController.instance.shadowdashParticlesPrefab = value;
			}
		}

		public static GameObject shadowRingPrefab
		{
			get
			{
				return HeroController.instance.shadowRingPrefab;
			}
			set
			{
				HeroController.instance.shadowRingPrefab = value;
			}
		}

		public static GameObject shadowRechargePrefab
		{
			get
			{
				return HeroController.instance.shadowRechargePrefab;
			}
			set
			{
				HeroController.instance.shadowRechargePrefab = value;
			}
		}

		public static GameObject dJumpWingsPrefab
		{
			get
			{
				return HeroController.instance.dJumpWingsPrefab;
			}
			set
			{
				HeroController.instance.dJumpWingsPrefab = value;
			}
		}

		public static GameObject dJumpFlashPrefab
		{
			get
			{
				return HeroController.instance.dJumpFlashPrefab;
			}
			set
			{
				HeroController.instance.dJumpFlashPrefab = value;
			}
		}

		public static ParticleSystem dJumpFeathers
		{
			get
			{
				return HeroController.instance.dJumpFeathers;
			}
			set
			{
				HeroController.instance.dJumpFeathers = value;
			}
		}

		public static GameObject wallPuffPrefab
		{
			get
			{
				return HeroController.instance.wallPuffPrefab;
			}
			set
			{
				HeroController.instance.wallPuffPrefab = value;
			}
		}

		public static GameObject sharpShadowPrefab
		{
			get
			{
				return HeroController.instance.sharpShadowPrefab;
			}
			set
			{
				HeroController.instance.sharpShadowPrefab = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabL
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabL;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabL = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabR
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabR;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabR = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabU
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabU;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabU = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabD
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabD;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabD = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabL_fury
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabL_fury;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabL_fury = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabR_fury
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabR_fury;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabR_fury = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabU_fury
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabU_fury;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabU_fury = value;
			}
		}

		public static GameObject grubberFlyBeamPrefabD_fury
		{
			get
			{
				return HeroController.instance.grubberFlyBeamPrefabD_fury;
			}
			set
			{
				HeroController.instance.grubberFlyBeamPrefabD_fury = value;
			}
		}

		public static GameObject carefreeShield
		{
			get
			{
				return HeroController.instance.carefreeShield;
			}
			set
			{
				HeroController.instance.carefreeShield = value;
			}
		}

		public static GameObject corpsePrefab
		{
			get
			{
				return HeroController.instance.corpsePrefab;
			}
			set
			{
				HeroController.instance.corpsePrefab = value;
			}
		}

		public static GameObject spikeDeathPrefab
		{
			get
			{
				return HeroController.instance.spikeDeathPrefab;
			}
			set
			{
				HeroController.instance.spikeDeathPrefab = value;
			}
		}

		public static GameObject acidDeathPrefab
		{
			get
			{
				return HeroController.instance.acidDeathPrefab;
			}
			set
			{
				HeroController.instance.acidDeathPrefab = value;
			}
		}

		public static GameObject lavaDeathPrefab
		{
			get
			{
				return HeroController.instance.lavaDeathPrefab;
			}
			set
			{
				HeroController.instance.lavaDeathPrefab = value;
			}
		}

		public static GameObject heroDeathPrefab
		{
			get
			{
				return HeroController.instance.heroDeathPrefab;
			}
			set
			{
				HeroController.instance.heroDeathPrefab = value;
			}
		}

		public static GameObject cutscenePrefab
		{
			get
			{
				return HeroController.instance.cutscenePrefab;
			}
			set
			{
				HeroController.instance.cutscenePrefab = value;
			}
		}

		public static GameManager gm
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameManager>(HeroController.instance, "gm");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameManager>(HeroController.instance, "gm", value);
			}
		}

		public static Rigidbody2D rb2d
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, Rigidbody2D>(HeroController.instance, "rb2d");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, Rigidbody2D>(HeroController.instance, "rb2d", value);
			}
		}

		public static Collider2D col2d
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, Collider2D>(HeroController.instance, "col2d");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, Collider2D>(HeroController.instance, "col2d", value);
			}
		}

		public static MeshRenderer renderer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, MeshRenderer>(HeroController.instance, "renderer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, MeshRenderer>(HeroController.instance, "renderer", value);
			}
		}

		public static Transform transform
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, Transform>(HeroController.instance, "transform");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, Transform>(HeroController.instance, "transform", value);
			}
		}

		public static HeroAnimationController animCtrl
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, HeroAnimationController>(HeroController.instance, "animCtrl");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, HeroAnimationController>(HeroController.instance, "animCtrl", value);
			}
		}

		public static HeroControllerStates cState
		{
			get
			{
				return HeroController.instance.cState;
			}
			set
			{
				HeroController.instance.cState = value;
			}
		}

		public static PlayerData playerData
		{
			get
			{
				return HeroController.instance.playerData;
			}
			set
			{
				HeroController.instance.playerData = value;
			}
		}

		public static HeroAudioController audioCtrl
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, HeroAudioController>(HeroController.instance, "audioCtrl");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, HeroAudioController>(HeroController.instance, "audioCtrl", value);
			}
		}

		public static AudioSource audioSource
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, AudioSource>(HeroController.instance, "audioSource");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, AudioSource>(HeroController.instance, "audioSource", value);
			}
		}

		public static UIManager ui
		{
			get
			{
				return HeroController.instance.ui;
			}
			set
			{
				HeroController.instance.ui = value;
			}
		}

		public static InputHandler inputHandler
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, InputHandler>(HeroController.instance, "inputHandler");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, InputHandler>(HeroController.instance, "inputHandler", value);
			}
		}

		public static GeoCounter geoCounter
		{
			get
			{
				return HeroController.instance.geoCounter;
			}
			set
			{
				ReflectionHelper.SetProperty<HeroController, GeoCounter>(HeroController.instance, "geoCounter", value);
			}
		}

		public static PlayMakerFSM proxyFSM
		{
			get
			{
				return HeroController.instance.proxyFSM;
			}
			set
			{
				ReflectionHelper.SetProperty<HeroController, PlayMakerFSM>(HeroController.instance, "proxyFSM", value);
			}
		}

		public static PlayMakerFSM damageEffectFSM
		{
			get
			{
				return HeroController.instance.damageEffectFSM;
			}
			set
			{
				HeroController.instance.damageEffectFSM = value;
			}
		}

		public static ParticleSystem dashParticleSystem
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, ParticleSystem>(HeroController.instance, "dashParticleSystem");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, ParticleSystem>(HeroController.instance, "dashParticleSystem", value);
			}
		}

		public static InvulnerablePulse invPulse
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, InvulnerablePulse>(HeroController.instance, "invPulse");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, InvulnerablePulse>(HeroController.instance, "invPulse", value);
			}
		}

		public static SpriteFlash spriteFlash
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, SpriteFlash>(HeroController.instance, "spriteFlash", value);
			}
		}

		public static AudioSource footStepsRunAudioSource
		{
			get
			{
				return HeroController.instance.footStepsRunAudioSource;
			}
			set
			{
				HeroController.instance.footStepsRunAudioSource = value;
			}
		}

		public static AudioSource footStepsWalkAudioSource
		{
			get
			{
				return HeroController.instance.footStepsWalkAudioSource;
			}
			set
			{
				HeroController.instance.footStepsWalkAudioSource = value;
			}
		}

		public static float prevGravityScale
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "prevGravityScale");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "prevGravityScale", value);
			}
		}

		public static Vector2 recoilVector
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<HeroController, Vector2>(HeroController.instance, "recoilVector");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<HeroController, Vector2>(HeroController.instance, "recoilVector", value);
			}
		}

		public static Vector2 lastInputState
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<HeroController, Vector2>(HeroController.instance, "lastInputState");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<HeroController, Vector2>(HeroController.instance, "lastInputState", value);
			}
		}

		public static GatePosition gatePosition
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.gatePosition;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.gatePosition = value;
			}
		}

		public static TransitionPoint sceneEntryGate
		{
			get
			{
				return HeroController.instance.sceneEntryGate;
			}
			set
			{
				ReflectionHelper.SetProperty<HeroController, TransitionPoint>(HeroController.instance, "sceneEntryGate", value);
			}
		}

		public static bool IsDreamReturning => HeroController.instance.IsDreamReturning;

		public static bool runMsgSent
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "runMsgSent");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "runMsgSent", value);
			}
		}

		public static bool hardLanded
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "hardLanded");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "hardLanded", value);
			}
		}

		public static bool fallRumble
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "fallRumble");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "fallRumble", value);
			}
		}

		public static bool acceptingInput
		{
			get
			{
				return HeroController.instance.acceptingInput;
			}
			set
			{
				HeroController.instance.acceptingInput = value;
			}
		}

		public static bool fallTrailGenerated
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "fallTrailGenerated");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "fallTrailGenerated", value);
			}
		}

		public static bool drainMP
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "drainMP");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "drainMP", value);
			}
		}

		public static float drainMP_timer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "drainMP_timer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "drainMP_timer", value);
			}
		}

		public static float drainMP_time
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "drainMP_time");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "drainMP_time", value);
			}
		}

		public static float MP_drained
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "MP_drained");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "MP_drained", value);
			}
		}

		public static float drainMP_seconds
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "drainMP_seconds");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "drainMP_seconds", value);
			}
		}

		public static float focusMP_amount
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "focusMP_amount");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "focusMP_amount", value);
			}
		}

		public static float dashBumpCorrection
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "dashBumpCorrection");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "dashBumpCorrection", value);
			}
		}

		public static bool controlReqlinquished
		{
			get
			{
				return HeroController.instance.controlReqlinquished;
			}
			set
			{
				HeroController.instance.controlReqlinquished = value;
			}
		}

		public static bool enterWithoutInput
		{
			get
			{
				return HeroController.instance.enterWithoutInput;
			}
			set
			{
				HeroController.instance.enterWithoutInput = value;
			}
		}

		public static bool lookingUpAnim
		{
			get
			{
				return HeroController.instance.lookingUpAnim;
			}
			set
			{
				HeroController.instance.lookingUpAnim = value;
			}
		}

		public static bool lookingDownAnim
		{
			get
			{
				return HeroController.instance.lookingDownAnim;
			}
			set
			{
				HeroController.instance.lookingDownAnim = value;
			}
		}

		public static bool carefreeShieldEquipped
		{
			get
			{
				return HeroController.instance.carefreeShieldEquipped;
			}
			set
			{
				HeroController.instance.carefreeShieldEquipped = value;
			}
		}

		public static int hitsSinceShielded
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "hitsSinceShielded");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "hitsSinceShielded", value);
			}
		}

		public static EndBeta endBeta
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, EndBeta>(HeroController.instance, "endBeta");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, EndBeta>(HeroController.instance, "endBeta", value);
			}
		}

		public static int jumpQueueSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "jumpQueueSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "jumpQueueSteps", value);
			}
		}

		public static bool jumpQueuing
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "jumpQueuing");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "jumpQueuing", value);
			}
		}

		public static int doubleJumpQueueSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "doubleJumpQueueSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "doubleJumpQueueSteps", value);
			}
		}

		public static bool doubleJumpQueuing
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "doubleJumpQueuing");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "doubleJumpQueuing", value);
			}
		}

		public static int jumpReleaseQueueSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "jumpReleaseQueueSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "jumpReleaseQueueSteps", value);
			}
		}

		public static bool jumpReleaseQueuing
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "jumpReleaseQueuing");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "jumpReleaseQueuing", value);
			}
		}

		public static int attackQueueSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "attackQueueSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "attackQueueSteps", value);
			}
		}

		public static bool attackQueuing
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "attackQueuing");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "attackQueuing", value);
			}
		}

		public static bool touchingWallL
		{
			get
			{
				return HeroController.instance.touchingWallL;
			}
			set
			{
				HeroController.instance.touchingWallL = value;
			}
		}

		public static bool touchingWallR
		{
			get
			{
				return HeroController.instance.touchingWallR;
			}
			set
			{
				HeroController.instance.touchingWallR = value;
			}
		}

		public static bool wallSlidingL
		{
			get
			{
				return HeroController.instance.wallSlidingL;
			}
			set
			{
				HeroController.instance.wallSlidingL = value;
			}
		}

		public static bool wallSlidingR
		{
			get
			{
				return HeroController.instance.wallSlidingR;
			}
			set
			{
				HeroController.instance.wallSlidingR = value;
			}
		}

		public static bool airDashed
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "airDashed");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "airDashed", value);
			}
		}

		public static bool dashingDown
		{
			get
			{
				return HeroController.instance.dashingDown;
			}
			set
			{
				HeroController.instance.dashingDown = value;
			}
		}

		public static bool wieldingLantern
		{
			get
			{
				return HeroController.instance.wieldingLantern;
			}
			set
			{
				HeroController.instance.wieldingLantern = value;
			}
		}

		public static bool startWithWallslide
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "startWithWallslide");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "startWithWallslide", value);
			}
		}

		public static bool startWithJump
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "startWithJump");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "startWithJump", value);
			}
		}

		public static bool startWithFullJump
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "startWithFullJump");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "startWithFullJump", value);
			}
		}

		public static bool startWithDash
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "startWithDash");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "startWithDash", value);
			}
		}

		public static bool startWithAttack
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "startWithAttack");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "startWithAttack", value);
			}
		}

		public static bool nailArt_cyclone
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "nailArt_cyclone");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "nailArt_cyclone", value);
			}
		}

		public static bool wallSlashing
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "wallSlashing");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "wallSlashing", value);
			}
		}

		public static bool doubleJumped
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "doubleJumped");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "doubleJumped", value);
			}
		}

		public static bool inAcid
		{
			get
			{
				return HeroController.instance.inAcid;
			}
			set
			{
				HeroController.instance.inAcid = value;
			}
		}

		public static bool wallJumpedR
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "wallJumpedR");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "wallJumpedR", value);
			}
		}

		public static bool wallJumpedL
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "wallJumpedL");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "wallJumpedL", value);
			}
		}

		public static bool wallLocked
		{
			get
			{
				return HeroController.instance.wallLocked;
			}
			set
			{
				HeroController.instance.wallLocked = value;
			}
		}

		public static float currentWalljumpSpeed
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "currentWalljumpSpeed");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "currentWalljumpSpeed", value);
			}
		}

		public static float walljumpSpeedDecel
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "walljumpSpeedDecel");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "walljumpSpeedDecel", value);
			}
		}

		public static int wallUnstickSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "wallUnstickSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "wallUnstickSteps", value);
			}
		}

		public static bool recoilLarge
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "recoilLarge");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "recoilLarge", value);
			}
		}

		public static float conveyorSpeed
		{
			get
			{
				return HeroController.instance.conveyorSpeed;
			}
			set
			{
				HeroController.instance.conveyorSpeed = value;
			}
		}

		public static float conveyorSpeedV
		{
			get
			{
				return HeroController.instance.conveyorSpeedV;
			}
			set
			{
				HeroController.instance.conveyorSpeedV = value;
			}
		}

		public static bool enteringVertically
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "enteringVertically");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "enteringVertically", value);
			}
		}

		public static bool playingWallslideClip
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "playingWallslideClip");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "playingWallslideClip", value);
			}
		}

		public static bool playedMantisClawClip
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "playedMantisClawClip");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "playedMantisClawClip", value);
			}
		}

		public static bool exitedSuperDashing
		{
			get
			{
				return HeroController.instance.exitedSuperDashing;
			}
			set
			{
				HeroController.instance.exitedSuperDashing = value;
			}
		}

		public static bool exitedQuake
		{
			get
			{
				return HeroController.instance.exitedQuake;
			}
			set
			{
				HeroController.instance.exitedQuake = value;
			}
		}

		public static bool fallCheckFlagged
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "fallCheckFlagged");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "fallCheckFlagged", value);
			}
		}

		public static int ledgeBufferSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "ledgeBufferSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "ledgeBufferSteps", value);
			}
		}

		public static int headBumpSteps
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, int>(HeroController.instance, "headBumpSteps");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, int>(HeroController.instance, "headBumpSteps", value);
			}
		}

		public static float nailChargeTime
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "nailChargeTime");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "nailChargeTime", value);
			}
		}

		public static bool takeNoDamage
		{
			get
			{
				return HeroController.instance.takeNoDamage;
			}
			set
			{
				HeroController.instance.takeNoDamage = value;
			}
		}

		public static bool joniBeam
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "joniBeam");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "joniBeam", value);
			}
		}

		public static bool fadedSceneIn
		{
			get
			{
				return HeroController.instance.fadedSceneIn;
			}
			set
			{
				HeroController.instance.fadedSceneIn = value;
			}
		}

		public static bool stopWalkingOut
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "stopWalkingOut");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "stopWalkingOut", value);
			}
		}

		public static bool boundsChecking
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "boundsChecking");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "boundsChecking", value);
			}
		}

		public static bool blockerFix
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "blockerFix");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "blockerFix", value);
			}
		}

		public static Vector2[] positionHistory
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, Vector2[]>(HeroController.instance, "positionHistory");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, Vector2[]>(HeroController.instance, "positionHistory", value);
			}
		}

		public static bool tilemapTestActive
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "tilemapTestActive");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "tilemapTestActive", value);
			}
		}

		public static Vector2 groundRayOriginC
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<HeroController, Vector2>(HeroController.instance, "groundRayOriginC");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<HeroController, Vector2>(HeroController.instance, "groundRayOriginC", value);
			}
		}

		public static Vector2 groundRayOriginL
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<HeroController, Vector2>(HeroController.instance, "groundRayOriginL");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<HeroController, Vector2>(HeroController.instance, "groundRayOriginL", value);
			}
		}

		public static Vector2 groundRayOriginR
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<HeroController, Vector2>(HeroController.instance, "groundRayOriginR");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<HeroController, Vector2>(HeroController.instance, "groundRayOriginR", value);
			}
		}

		public static Coroutine takeDamageCoroutine
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, Coroutine>(HeroController.instance, "takeDamageCoroutine");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, Coroutine>(HeroController.instance, "takeDamageCoroutine", value);
			}
		}

		public static Coroutine tilemapTestCoroutine
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, Coroutine>(HeroController.instance, "tilemapTestCoroutine");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, Coroutine>(HeroController.instance, "tilemapTestCoroutine", value);
			}
		}

		public static AudioClip footstepsRunDust
		{
			get
			{
				return HeroController.instance.footstepsRunDust;
			}
			set
			{
				HeroController.instance.footstepsRunDust = value;
			}
		}

		public static AudioClip footstepsRunGrass
		{
			get
			{
				return HeroController.instance.footstepsRunGrass;
			}
			set
			{
				HeroController.instance.footstepsRunGrass = value;
			}
		}

		public static AudioClip footstepsRunBone
		{
			get
			{
				return HeroController.instance.footstepsRunBone;
			}
			set
			{
				HeroController.instance.footstepsRunBone = value;
			}
		}

		public static AudioClip footstepsRunSpa
		{
			get
			{
				return HeroController.instance.footstepsRunSpa;
			}
			set
			{
				HeroController.instance.footstepsRunSpa = value;
			}
		}

		public static AudioClip footstepsRunMetal
		{
			get
			{
				return HeroController.instance.footstepsRunMetal;
			}
			set
			{
				HeroController.instance.footstepsRunMetal = value;
			}
		}

		public static AudioClip footstepsRunWater
		{
			get
			{
				return HeroController.instance.footstepsRunWater;
			}
			set
			{
				HeroController.instance.footstepsRunWater = value;
			}
		}

		public static AudioClip footstepsWalkDust
		{
			get
			{
				return HeroController.instance.footstepsWalkDust;
			}
			set
			{
				HeroController.instance.footstepsWalkDust = value;
			}
		}

		public static AudioClip footstepsWalkGrass
		{
			get
			{
				return HeroController.instance.footstepsWalkGrass;
			}
			set
			{
				HeroController.instance.footstepsWalkGrass = value;
			}
		}

		public static AudioClip footstepsWalkBone
		{
			get
			{
				return HeroController.instance.footstepsWalkBone;
			}
			set
			{
				HeroController.instance.footstepsWalkBone = value;
			}
		}

		public static AudioClip footstepsWalkSpa
		{
			get
			{
				return HeroController.instance.footstepsWalkSpa;
			}
			set
			{
				HeroController.instance.footstepsWalkSpa = value;
			}
		}

		public static AudioClip footstepsWalkMetal
		{
			get
			{
				return HeroController.instance.footstepsWalkMetal;
			}
			set
			{
				HeroController.instance.footstepsWalkMetal = value;
			}
		}

		public static AudioClip nailArtCharge
		{
			get
			{
				return HeroController.instance.nailArtCharge;
			}
			set
			{
				HeroController.instance.nailArtCharge = value;
			}
		}

		public static AudioClip nailArtChargeComplete
		{
			get
			{
				return HeroController.instance.nailArtChargeComplete;
			}
			set
			{
				HeroController.instance.nailArtChargeComplete = value;
			}
		}

		public static AudioClip blockerImpact
		{
			get
			{
				return HeroController.instance.blockerImpact;
			}
			set
			{
				HeroController.instance.blockerImpact = value;
			}
		}

		public static AudioClip shadowDashClip
		{
			get
			{
				return HeroController.instance.shadowDashClip;
			}
			set
			{
				HeroController.instance.shadowDashClip = value;
			}
		}

		public static AudioClip sharpShadowClip
		{
			get
			{
				return HeroController.instance.sharpShadowClip;
			}
			set
			{
				HeroController.instance.sharpShadowClip = value;
			}
		}

		public static AudioClip doubleJumpClip
		{
			get
			{
				return HeroController.instance.doubleJumpClip;
			}
			set
			{
				HeroController.instance.doubleJumpClip = value;
			}
		}

		public static AudioClip mantisClawClip
		{
			get
			{
				return HeroController.instance.mantisClawClip;
			}
			set
			{
				HeroController.instance.mantisClawClip = value;
			}
		}

		public static GameObject slash
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "slash");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "slash", value);
			}
		}

		public static NailSlash slashComponent
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, NailSlash>(HeroController.instance, "slashComponent");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, NailSlash>(HeroController.instance, "slashComponent", value);
			}
		}

		public static PlayMakerFSM slashFsm
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, PlayMakerFSM>(HeroController.instance, "slashFsm");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, PlayMakerFSM>(HeroController.instance, "slashFsm", value);
			}
		}

		public static GameObject runEffect
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "runEffect");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "runEffect", value);
			}
		}

		public static GameObject backDash
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "backDash");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "backDash", value);
			}
		}

		public static GameObject jumpEffect
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "jumpEffect");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "jumpEffect", value);
			}
		}

		public static GameObject fallEffect
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "fallEffect");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "fallEffect", value);
			}
		}

		public static GameObject dashEffect
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "dashEffect");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "dashEffect", value);
			}
		}

		public static GameObject grubberFlyBeam
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "grubberFlyBeam");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "grubberFlyBeam", value);
			}
		}

		public static GameObject hazardCorpe
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, GameObject>(HeroController.instance, "hazardCorpe");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, GameObject>(HeroController.instance, "hazardCorpe", value);
			}
		}

		public static PlayMakerFSM vignetteFSM
		{
			get
			{
				return HeroController.instance.vignetteFSM;
			}
			set
			{
				HeroController.instance.vignetteFSM = value;
			}
		}

		public static SpriteRenderer heroLight
		{
			get
			{
				return HeroController.instance.heroLight;
			}
			set
			{
				HeroController.instance.heroLight = value;
			}
		}

		public static SpriteRenderer vignette
		{
			get
			{
				return HeroController.instance.vignette;
			}
			set
			{
				HeroController.instance.vignette = value;
			}
		}

		public static PlayMakerFSM dashBurst
		{
			get
			{
				return HeroController.instance.dashBurst;
			}
			set
			{
				HeroController.instance.dashBurst = value;
			}
		}

		public static PlayMakerFSM superDash
		{
			get
			{
				return HeroController.instance.superDash;
			}
			set
			{
				HeroController.instance.superDash = value;
			}
		}

		public static PlayMakerFSM fsm_thornCounter
		{
			get
			{
				return HeroController.instance.fsm_thornCounter;
			}
			set
			{
				HeroController.instance.fsm_thornCounter = value;
			}
		}

		public static PlayMakerFSM spellControl
		{
			get
			{
				return HeroController.instance.spellControl;
			}
			set
			{
				HeroController.instance.spellControl = value;
			}
		}

		public static PlayMakerFSM fsm_fallTrail
		{
			get
			{
				return HeroController.instance.fsm_fallTrail;
			}
			set
			{
				HeroController.instance.fsm_fallTrail = value;
			}
		}

		public static PlayMakerFSM fsm_orbitShield
		{
			get
			{
				return HeroController.instance.fsm_orbitShield;
			}
			set
			{
				HeroController.instance.fsm_orbitShield = value;
			}
		}

		public static VibrationData softLandVibration
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.softLandVibration;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.softLandVibration = value;
			}
		}

		public static VibrationData wallJumpVibration
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.wallJumpVibration;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.wallJumpVibration = value;
			}
		}

		public static VibrationPlayer wallSlideVibrationPlayer
		{
			get
			{
				return HeroController.instance.wallSlideVibrationPlayer;
			}
			set
			{
				HeroController.instance.wallSlideVibrationPlayer = value;
			}
		}

		public static VibrationData dashVibration
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.dashVibration;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.dashVibration = value;
			}
		}

		public static VibrationData shadowDashVibration
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.shadowDashVibration;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.shadowDashVibration = value;
			}
		}

		public static VibrationData doubleJumpVibration
		{
			get
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				return HeroController.instance.doubleJumpVibration;
			}
			set
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				HeroController.instance.doubleJumpVibration = value;
			}
		}

		public static bool jumpReleaseQueueingEnabled
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, bool>(HeroController.instance, "jumpReleaseQueueingEnabled");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "jumpReleaseQueueingEnabled", value);
			}
		}

		public static float preventCastByDialogueEndTimer
		{
			get
			{
				return ReflectionHelper.GetField<HeroController, float>(HeroController.instance, "preventCastByDialogueEndTimer");
			}
			set
			{
				ReflectionHelper.SetField<HeroController, float>(HeroController.instance, "preventCastByDialogueEndTimer", value);
			}
		}

		public static Vector2 oldPos
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				return ReflectionHelper.GetField<HeroController, Vector2>(HeroController.instance, "oldPos");
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				ReflectionHelper.SetField<HeroController, Vector2>(HeroController.instance, "oldPos", value);
			}
		}

		public static void Awake()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "Awake", Array.Empty<object>());
		}

		public static void Start()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "Start", Array.Empty<object>());
		}

		public static void SceneInit()
		{
			HeroController.instance.SceneInit();
		}

		public static void Update()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "Update", Array.Empty<object>());
		}

		public static void FixedUpdate()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "FixedUpdate", Array.Empty<object>());
		}

		public static void Update10()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "Update10", Array.Empty<object>());
		}

		public static void OnLevelUnload()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "OnLevelUnload", Array.Empty<object>());
		}

		public static void OnDisable()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "OnDisable", Array.Empty<object>());
		}

		public static void Move(float move_direction)
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "Move", new object[1] { move_direction });
		}

		public static void Jump()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "Jump", Array.Empty<object>());
		}

		public static void DoubleJump()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "DoubleJump", Array.Empty<object>());
		}

		public static void Attack(AttackDirection attackDir)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			HeroController.instance.Attack(attackDir);
		}

		public static void Dash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "Dash", Array.Empty<object>());
		}

		public static void BackDash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "BackDash", Array.Empty<object>());
		}

		public static void ShadowDash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ShadowDash", Array.Empty<object>());
		}

		public static void SuperDash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "SuperDash", Array.Empty<object>());
		}

		public static void FaceRight()
		{
			HeroController.instance.FaceRight();
		}

		public static void FaceLeft()
		{
			HeroController.instance.FaceLeft();
		}

		public static void StartMPDrain(float time)
		{
			HeroController.instance.StartMPDrain(time);
		}

		public static void StopMPDrain()
		{
			HeroController.instance.StopMPDrain();
		}

		public static void SetBackOnGround()
		{
			HeroController.instance.SetBackOnGround();
		}

		public static void SetStartWithWallslide()
		{
			HeroController.instance.SetStartWithWallslide();
		}

		public static void SetStartWithJump()
		{
			HeroController.instance.SetStartWithJump();
		}

		public static void SetStartWithFullJump()
		{
			HeroController.instance.SetStartWithFullJump();
		}

		public static void SetStartWithDash()
		{
			HeroController.instance.SetStartWithDash();
		}

		public static void SetStartWithAttack()
		{
			HeroController.instance.SetStartWithAttack();
		}

		public static void SetSuperDashExit()
		{
			HeroController.instance.SetSuperDashExit();
		}

		public static void SetQuakeExit()
		{
			HeroController.instance.SetQuakeExit();
		}

		public static void SetTakeNoDamage()
		{
			HeroController.instance.SetTakeNoDamage();
		}

		public static void EndTakeNoDamage()
		{
			HeroController.instance.EndTakeNoDamage();
		}

		public static void SetHeroParent(Transform newParent)
		{
			HeroController.instance.SetHeroParent(newParent);
		}

		public static void IsSwimming()
		{
			HeroController.instance.IsSwimming();
		}

		public static void NotSwimming()
		{
			HeroController.instance.NotSwimming();
		}

		public static void EnableRenderer()
		{
			HeroController.instance.EnableRenderer();
		}

		public static void ResetAirMoves()
		{
			HeroController.instance.ResetAirMoves();
		}

		public static void SetConveyorSpeed(float speed)
		{
			HeroController.instance.SetConveyorSpeed(speed);
		}

		public static void SetConveyorSpeedV(float speed)
		{
			HeroController.instance.SetConveyorSpeedV(speed);
		}

		public static void EnterWithoutInput(bool flag)
		{
			HeroController.instance.EnterWithoutInput(flag);
		}

		public static void SetDarkness(int darkness)
		{
			HeroController.instance.SetDarkness(darkness);
		}

		public static void CancelHeroJump()
		{
			HeroController.instance.CancelHeroJump();
		}

		public static void CharmUpdate()
		{
			HeroController.instance.CharmUpdate();
		}

		public static void checkEnvironment()
		{
			HeroController.instance.checkEnvironment();
		}

		public static void SetBenchRespawn(string spawnMarker, string sceneName, int spawnType, bool facingRight)
		{
			HeroController.instance.SetBenchRespawn(spawnMarker, sceneName, spawnType, facingRight);
		}

		public static void SetHazardRespawn(Vector3 position, bool facingRight)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			HeroController.instance.SetHazardRespawn(position, facingRight);
		}

		public static void AddGeo(int amount)
		{
			HeroController.instance.AddGeo(amount);
		}

		public static void ToZero()
		{
			HeroController.instance.ToZero();
		}

		public static void AddGeoQuietly(int amount)
		{
			HeroController.instance.AddGeoQuietly(amount);
		}

		public static void AddGeoToCounter(int amount)
		{
			HeroController.instance.AddGeoToCounter(amount);
		}

		public static void TakeGeo(int amount)
		{
			HeroController.instance.TakeGeo(amount);
		}

		public static void UpdateGeo()
		{
			HeroController.instance.UpdateGeo();
		}

		public static bool CanInput()
		{
			return HeroController.instance.CanInput();
		}

		public static bool CanTalk()
		{
			return HeroController.instance.CanTalk();
		}

		public static void FlipSprite()
		{
			HeroController.instance.FlipSprite();
		}

		public static void NailParry()
		{
			HeroController.instance.NailParry();
		}

		public static void NailParryRecover()
		{
			HeroController.instance.NailParryRecover();
		}

		public static void QuakeInvuln()
		{
			HeroController.instance.QuakeInvuln();
		}

		public static void CancelParryInvuln()
		{
			HeroController.instance.CancelParryInvuln();
		}

		public static void CycloneInvuln()
		{
			HeroController.instance.CycloneInvuln();
		}

		public static void SetWieldingLantern(bool set)
		{
			HeroController.instance.SetWieldingLantern(set);
		}

		public static void TakeDamage(GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			HeroController.instance.TakeDamage(go, damageSide, damageAmount, hazardType);
		}

		public static string GetEntryGateName()
		{
			return HeroController.instance.GetEntryGateName();
		}

		public static void AddMPCharge(int amount)
		{
			HeroController.instance.AddMPCharge(amount);
		}

		public static void SoulGain()
		{
			HeroController.instance.SoulGain();
		}

		public static void AddMPChargeSpa(int amount)
		{
			HeroController.instance.AddMPChargeSpa(amount);
		}

		public static bool TryAddMPChargeSpa(int amount)
		{
			return HeroController.instance.TryAddMPChargeSpa(amount);
		}

		public static void SetMPCharge(int amount)
		{
			HeroController.instance.SetMPCharge(amount);
		}

		public static void TakeMP(int amount)
		{
			HeroController.instance.TakeMP(amount);
		}

		public static void TakeMPQuick(int amount)
		{
			HeroController.instance.TakeMPQuick(amount);
		}

		public static void TakeReserveMP(int amount)
		{
			HeroController.instance.TakeReserveMP(amount);
		}

		public static void AddHealth(int amount)
		{
			HeroController.instance.AddHealth(amount);
		}

		public static void TakeHealth(int amount)
		{
			HeroController.instance.TakeHealth(amount);
		}

		public static void MaxHealth()
		{
			HeroController.instance.MaxHealth();
		}

		public static void MaxHealthKeepBlue()
		{
			HeroController.instance.MaxHealthKeepBlue();
		}

		public static void AddToMaxHealth(int amount)
		{
			HeroController.instance.AddToMaxHealth(amount);
		}

		public static void ClearMP()
		{
			HeroController.instance.ClearMP();
		}

		public static void ClearMPSendEvents()
		{
			HeroController.instance.ClearMPSendEvents();
		}

		public static void AddToMaxMPReserve(int amount)
		{
			HeroController.instance.AddToMaxMPReserve(amount);
		}

		public static void Bounce()
		{
			HeroController.instance.Bounce();
		}

		public static void BounceHigh()
		{
			HeroController.instance.BounceHigh();
		}

		public static void ShroomBounce()
		{
			HeroController.instance.ShroomBounce();
		}

		public static void RecoilLeft()
		{
			HeroController.instance.RecoilLeft();
		}

		public static void RecoilRight()
		{
			HeroController.instance.RecoilRight();
		}

		public static void RecoilRightLong()
		{
			HeroController.instance.RecoilRightLong();
		}

		public static void RecoilLeftLong()
		{
			HeroController.instance.RecoilLeftLong();
		}

		public static void RecoilDown()
		{
			HeroController.instance.RecoilDown();
		}

		public static void ForceHardLanding()
		{
			HeroController.instance.ForceHardLanding();
		}

		public static void EnterSceneDreamGate()
		{
			HeroController.instance.EnterSceneDreamGate();
		}

		public static IEnumerator EnterScene(TransitionPoint enterGate, float delayBeforeEnter)
		{
			return HeroController.instance.EnterScene(enterGate, delayBeforeEnter);
		}

		public static void LeaveScene(GatePosition? gate = null)
		{
			HeroController.instance.LeaveScene(gate);
		}

		public static IEnumerator BetaLeave(EndBeta betaEndTrigger)
		{
			return HeroController.instance.BetaLeave(betaEndTrigger);
		}

		public static IEnumerator BetaReturn()
		{
			return HeroController.instance.BetaReturn();
		}

		public static IEnumerator Respawn()
		{
			return HeroController.instance.Respawn();
		}

		public static IEnumerator HazardRespawn()
		{
			return HeroController.instance.HazardRespawn();
		}

		public static void StartCyclone()
		{
			HeroController.instance.StartCyclone();
		}

		public static void EndCyclone()
		{
			HeroController.instance.EndCyclone();
		}

		public static bool GetState(string stateName)
		{
			return HeroController.instance.GetState(stateName);
		}

		public static bool GetCState(string stateName)
		{
			return HeroController.instance.GetCState(stateName);
		}

		public static void SetCState(string stateName, bool value)
		{
			HeroController.instance.SetCState(stateName, value);
		}

		public static void ResetHardLandingTimer()
		{
			HeroController.instance.ResetHardLandingTimer();
		}

		public static void CancelSuperDash()
		{
			HeroController.instance.CancelSuperDash();
		}

		public static void RelinquishControlNotVelocity()
		{
			HeroController.instance.RelinquishControlNotVelocity();
		}

		public static void RelinquishControl()
		{
			HeroController.instance.RelinquishControl();
		}

		public static void RegainControl()
		{
			HeroController.instance.RegainControl();
		}

		public static void PreventCastByDialogueEnd()
		{
			HeroController.instance.PreventCastByDialogueEnd();
		}

		public static bool CanCast()
		{
			return HeroController.instance.CanCast();
		}

		public static bool CanFocus()
		{
			return HeroController.instance.CanFocus();
		}

		public static bool CanNailArt()
		{
			return HeroController.instance.CanNailArt();
		}

		public static bool CanQuickMap()
		{
			return HeroController.instance.CanQuickMap();
		}

		public static bool CanInspect()
		{
			return HeroController.instance.CanInspect();
		}

		public static bool CanBackDash()
		{
			return HeroController.instance.CanBackDash();
		}

		public static bool CanSuperDash()
		{
			return HeroController.instance.CanSuperDash();
		}

		public static bool CanDreamNail()
		{
			return HeroController.instance.CanDreamNail();
		}

		public static bool CanDreamGate()
		{
			return HeroController.instance.CanDreamGate();
		}

		public static bool CanInteract()
		{
			return HeroController.instance.CanInteract();
		}

		public static bool CanOpenInventory()
		{
			return HeroController.instance.CanOpenInventory();
		}

		public static void SetDamageMode(int invincibilityType)
		{
			HeroController.instance.SetDamageMode(invincibilityType);
		}

		public static void SetDamageModeFSM(int invincibilityType)
		{
			HeroController.instance.SetDamageModeFSM(invincibilityType);
		}

		public static void ResetQuakeDamage()
		{
			HeroController.instance.ResetQuakeDamage();
		}

		public static void SetDamageMode(DamageMode newDamageMode)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			HeroController.instance.SetDamageMode(newDamageMode);
		}

		public static void StopAnimationControl()
		{
			HeroController.instance.StopAnimationControl();
		}

		public static void StartAnimationControl()
		{
			HeroController.instance.StartAnimationControl();
		}

		public static void IgnoreInput()
		{
			HeroController.instance.IgnoreInput();
		}

		public static void IgnoreInputWithoutReset()
		{
			HeroController.instance.IgnoreInputWithoutReset();
		}

		public static void AcceptInput()
		{
			HeroController.instance.AcceptInput();
		}

		public static void Pause()
		{
			HeroController.instance.Pause();
		}

		public static void UnPause()
		{
			HeroController.instance.UnPause();
		}

		public static void NearBench(bool isNearBench)
		{
			HeroController.instance.NearBench(isNearBench);
		}

		public static void SetWalkZone(bool inWalkZone)
		{
			HeroController.instance.SetWalkZone(inWalkZone);
		}

		public static void ResetState()
		{
			HeroController.instance.ResetState();
		}

		public static void StopPlayingAudio()
		{
			HeroController.instance.StopPlayingAudio();
		}

		public static void PauseAudio()
		{
			HeroController.instance.PauseAudio();
		}

		public static void UnPauseAudio()
		{
			HeroController.instance.UnPauseAudio();
		}

		public static void PauseInput()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "PauseInput", Array.Empty<object>());
		}

		public static void UnPauseInput()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "UnPauseInput", Array.Empty<object>());
		}

		public static void SpawnSoftLandingPrefab()
		{
			HeroController.instance.SpawnSoftLandingPrefab();
		}

		public static void AffectedByGravity(bool gravityApplies)
		{
			HeroController.instance.AffectedByGravity(gravityApplies);
		}

		public static void LookForInput()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "LookForInput", Array.Empty<object>());
		}

		public static void LookForQueueInput()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "LookForQueueInput", Array.Empty<object>());
		}

		public static void HeroJump()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "HeroJump", Array.Empty<object>());
		}

		public static void HeroJumpNoEffect()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "HeroJumpNoEffect", Array.Empty<object>());
		}

		public static void DoWallJump()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "DoWallJump", Array.Empty<object>());
		}

		public static void DoDoubleJump()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "DoDoubleJump", Array.Empty<object>());
		}

		public static void DoHardLanding()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "DoHardLanding", Array.Empty<object>());
		}

		public static void DoAttack()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "DoAttack", Array.Empty<object>());
		}

		public static void HeroDash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "HeroDash", Array.Empty<object>());
		}

		public static void StartFallRumble()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "StartFallRumble", Array.Empty<object>());
		}

		public static void SetState(ActorStates newState)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "SetState", new object[1] { newState });
		}

		public static void FinishedEnteringScene(bool setHazardMarker = true, bool preventRunBob = false)
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "FinishedEnteringScene", new object[2] { setHazardMarker, preventRunBob });
		}

		public static IEnumerator Die()
		{
			return ReflectionHelper.CallMethod<HeroController, IEnumerator>(HeroController.instance, "Die", Array.Empty<object>());
		}

		public static IEnumerator DieFromHazard(HazardType hazardType, float angle)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<HeroController, IEnumerator>(HeroController.instance, "DieFromHazard", new object[2] { hazardType, angle });
		}

		public static IEnumerator StartRecoil(CollisionSide impactSide, bool spawnDamageEffect, int damageAmount)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<HeroController, IEnumerator>(HeroController.instance, "StartRecoil", new object[3] { impactSide, spawnDamageEffect, damageAmount });
		}

		public static IEnumerator Invulnerable(float duration)
		{
			return ReflectionHelper.CallMethod<HeroController, IEnumerator>(HeroController.instance, "Invulnerable", new object[1] { duration });
		}

		public static IEnumerator FirstFadeIn()
		{
			return ReflectionHelper.CallMethod<HeroController, IEnumerator>(HeroController.instance, "FirstFadeIn", Array.Empty<object>());
		}

		public static void FallCheck()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "FallCheck", Array.Empty<object>());
		}

		public static void OutOfBoundsCheck()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "OutOfBoundsCheck", Array.Empty<object>());
		}

		public static void ConfirmOutOfBounds()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ConfirmOutOfBounds", Array.Empty<object>());
		}

		public static void FailSafeChecks()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "FailSafeChecks", Array.Empty<object>());
		}

		public static Transform LocateSpawnPoint()
		{
			return HeroController.instance.LocateSpawnPoint();
		}

		public static void CancelJump()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelJump", Array.Empty<object>());
		}

		public static void CancelDoubleJump()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelDoubleJump", Array.Empty<object>());
		}

		public static void CancelDash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelDash", Array.Empty<object>());
		}

		public static void CancelWallsliding()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelWallsliding", Array.Empty<object>());
		}

		public static void CancelBackDash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelBackDash", Array.Empty<object>());
		}

		public static void CancelDownAttack()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelDownAttack", Array.Empty<object>());
		}

		public static void CancelAttack()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelAttack", Array.Empty<object>());
		}

		public static void CancelBounce()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelBounce", Array.Empty<object>());
		}

		public static void CancelRecoilHorizontal()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelRecoilHorizontal", Array.Empty<object>());
		}

		public static void CancelDamageRecoil()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelDamageRecoil", Array.Empty<object>());
		}

		public static void CancelFallEffects()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "CancelFallEffects", Array.Empty<object>());
		}

		public static void ResetAttacks()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ResetAttacks", Array.Empty<object>());
		}

		public static void ResetAttacksDash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ResetAttacksDash", Array.Empty<object>());
		}

		public static void ResetMotion()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ResetMotion", Array.Empty<object>());
		}

		public static void ResetMotionNotVelocity()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ResetMotionNotVelocity", Array.Empty<object>());
		}

		public static void ResetLook()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ResetLook", Array.Empty<object>());
		}

		public static void ResetInput()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ResetInput", Array.Empty<object>());
		}

		public static void BackOnGround()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "BackOnGround", Array.Empty<object>());
		}

		public static void JumpReleased()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "JumpReleased", Array.Empty<object>());
		}

		public static void FinishedDashing()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "FinishedDashing", Array.Empty<object>());
		}

		public static void SetStartingMotionState()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "SetStartingMotionState", Array.Empty<object>());
		}

		public static void SetStartingMotionState(bool preventRunDip)
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "SetStartingMotionState", new object[1] { preventRunDip });
		}

		public static void EnterAcid()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "EnterAcid", Array.Empty<object>());
		}

		public static void ExitAcid()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "ExitAcid", Array.Empty<object>());
		}

		public static void TileMapTest()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "TileMapTest", Array.Empty<object>());
		}

		public static IEnumerator TilemapTestPause()
		{
			return ReflectionHelper.CallMethod<HeroController, IEnumerator>(HeroController.instance, "TilemapTestPause", Array.Empty<object>());
		}

		public static void StopTilemapTest()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "StopTilemapTest", Array.Empty<object>());
		}

		public static IEnumerator CheckForTerrainThunk(AttackDirection attackDir)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return HeroController.instance.CheckForTerrainThunk(attackDir);
		}

		public static bool CheckStillTouchingWall(CollisionSide side, bool checkTop = false)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CheckStillTouchingWall", new object[2] { side, checkTop });
		}

		public static bool CheckForBump(CollisionSide side)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return HeroController.instance.CheckForBump(side);
		}

		public static bool CheckNearRoof()
		{
			return HeroController.instance.CheckNearRoof();
		}

		public static bool CheckTouchingGround()
		{
			return HeroController.instance.CheckTouchingGround();
		}

		public static List<CollisionSide> CheckTouching(PhysLayers layer)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<HeroController, List<CollisionSide>>(HeroController.instance, "CheckTouching", new object[1] { layer });
		}

		public static List<CollisionSide> CheckTouchingAdvanced(PhysLayers layer)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<HeroController, List<CollisionSide>>(HeroController.instance, "CheckTouchingAdvanced", new object[1] { layer });
		}

		public static CollisionSide FindCollisionDirection(Collision2D collision)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<HeroController, CollisionSide>(HeroController.instance, "FindCollisionDirection", new object[1] { collision });
		}

		public static bool CanJump()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanJump", Array.Empty<object>());
		}

		public static bool CanDoubleJump()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanDoubleJump", Array.Empty<object>());
		}

		public static bool CanInfiniteAirJump()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanInfiniteAirJump", Array.Empty<object>());
		}

		public static bool CanSwim()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanSwim", Array.Empty<object>());
		}

		public static bool CanDash()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanDash", Array.Empty<object>());
		}

		public static bool CanAttack()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanAttack", Array.Empty<object>());
		}

		public static bool CanNailCharge()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanNailCharge", Array.Empty<object>());
		}

		public static bool CanWallSlide()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanWallSlide", Array.Empty<object>());
		}

		public static bool CanTakeDamage()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanTakeDamage", Array.Empty<object>());
		}

		public static bool CanWallJump()
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "CanWallJump", Array.Empty<object>());
		}

		public static bool ShouldHardLand(Collision2D collision)
		{
			return ReflectionHelper.CallMethod<HeroController, bool>(HeroController.instance, "ShouldHardLand", new object[1] { collision });
		}

		public static void OnCollisionEnter2D(Collision2D collision)
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "OnCollisionEnter2D", new object[1] { collision });
		}

		public static void OnCollisionStay2D(Collision2D collision)
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "OnCollisionStay2D", new object[1] { collision });
		}

		public static void OnCollisionExit2D(Collision2D collision)
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "OnCollisionExit2D", new object[1] { collision });
		}

		public static void SetupGameRefs()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "SetupGameRefs", Array.Empty<object>());
		}

		public static void SetupPools()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "SetupPools", Array.Empty<object>());
		}

		public static void FilterInput()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "FilterInput", Array.Empty<object>());
		}

		public static Vector3 FindGroundPoint(Vector2 startPoint, bool useExtended = false)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return HeroController.instance.FindGroundPoint(startPoint, useExtended);
		}

		public static float FindGroundPointY(float x, float y, bool useExtended = false)
		{
			return ReflectionHelper.CallMethod<HeroController, float>(HeroController.instance, "FindGroundPointY", new object[3] { x, y, useExtended });
		}

		public static void orig_StartMPDrain(float time)
		{
			HeroController.instance.orig_StartMPDrain(time);
		}

		public static void orig_Update()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "orig_Update", Array.Empty<object>());
		}

		public static Vector2 OrigDashVector()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return ReflectionHelper.CallMethod<HeroController, Vector2>(HeroController.instance, "OrigDashVector", Array.Empty<object>());
		}

		public static void orig_Dash()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "orig_Dash", Array.Empty<object>());
		}

		public static void orig_CharmUpdate()
		{
			HeroController.instance.orig_CharmUpdate();
		}

		public static void orig_DoAttack()
		{
			ReflectionHelper.CallMethod<HeroController>(HeroController.instance, "orig_DoAttack", Array.Empty<object>());
		}
	}
	[Obsolete("Please use HKMirror", true)]
	public class HeroControllerILIEnumerators
	{
		public static event Manipulator EnterScene
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "EnterScene", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "EnterScene", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator BetaLeave
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "BetaLeave", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "BetaLeave", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator BetaReturn
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "BetaReturn", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "BetaReturn", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator Respawn
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "Respawn", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "Respawn", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator HazardRespawn
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "HazardRespawn", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "HazardRespawn", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator Die
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "Die", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "Die", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator DieFromHazard
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "DieFromHazard", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "DieFromHazard", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator StartRecoil
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "StartRecoil", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "StartRecoil", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator Invulnerable
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "Invulnerable", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "Invulnerable", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator FirstFadeIn
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "FirstFadeIn", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "FirstFadeIn", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator TilemapTestPause
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "TilemapTestPause", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "TilemapTestPause", true)), (Delegate)(object)value);
			}
		}

		public static event Manipulator CheckForTerrainThunk
		{
			add
			{
				HookEndpointManager.Modify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "CheckForTerrainThunk", true)), (Delegate)(object)value);
			}
			remove
			{
				HookEndpointManager.Unmodify((MethodBase)Extensions.GetStateMachineTarget(ReflectionHelper.GetMethodInfo(typeof(HeroController), "CheckForTerrainThunk", true)), (Delegate)(object)value);
			}
		}
	}
}
namespace Satchel.JsonConverters
{
	public class ColorConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Color val = default(Color);
				if (ColorUtility.TryParseHtmlString("#" + reader.Value, ref val))
				{
					return val;
				}
				Debug.LogError((object)$"Failed to parse color : {reader.Value}");
			}
			catch (Exception ex)
			{
				Debug.LogError((object)$"Failed to parse color {objectType} : {ex.Message}");
			}
			return (object)new Color(1f, 1f, 1f, 1f);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			string text = ColorUtility.ToHtmlStringRGB((Color)value);
			writer.WriteValue(text);
		}
	}
}
namespace Satchel.Futils
{
	public class EventInterceptor
	{
		public string fromState;

		public string eventName;

		public Func<bool> shouldIntercept = () => true;

		public Action onIntercept;
	}
	public class InterceptorAction : FsmStateAction
	{
		public Func<string> method;

		public override void Reset()
		{
			method = null;
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			((FsmStateAction)this).Fsm.Event(method?.Invoke());
			((FsmStateAction)this).Finish();
		}
	}
	public class TransitionInterceptor
	{
		public string fromState;

		public string eventName;

		public string toStateDefault;

		public string toStateCustom;

		public Func<bool> shouldIntercept = () => true;

		public Action<string, string> onIntercept;

		public static string defaultEvent = "Intercept-DEFAULT";

		public static string customEvent = "Intercept-CUSTOM";

		public string GetNextEvent(string interceptedState, string interceptedEvent)
		{
			if (shouldIntercept != null && shouldIntercept())
			{
				if (onIntercept != null)
				{
					onIntercept(interceptedState, interceptedEvent);
				}
				return customEvent;
			}
			return defaultEvent;
		}

		public string GetNextEventOrDefault(string interceptedState, string interceptedEvent)
		{
			return GetNextEvent(interceptedState, interceptedEvent) ?? defaultEvent;
		}
	}
	public class CustomFsmAction : FsmStateAction
	{
		public Action method;

		public CustomFsmAction(Action method)
		{
			this.method = method;
		}

		public CustomFsmAction()
		{
		}

		public override void Reset()
		{
			method = null;
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			method?.Invoke();
			((FsmStateAction)this).Finish();
		}
	}
	public class CustomFsmActionUpdate : FsmStateAction
	{
		public Action method;

		public CustomFsmActionUpdate(Action method)
		{
			this.method = method;
		}

		public CustomFsmActionUpdate()
		{
		}

		public override void Reset()
		{
			method = null;
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			method?.Invoke();
		}

		public override void OnUpdate()
		{
			method?.Invoke();
		}
	}
	public class CustomFsmActionFixedUpdate : FsmStateAction
	{
		public Action method;

		public CustomFsmActionFixedUpdate(Action method)
		{
			this.method = method;
		}

		public CustomFsmActionFixedUpdate()
		{
		}

		public override void Reset()
		{
			method = null;
			((FsmStateAction)this).Reset();
		}

		public override void OnEnter()
		{
			method?.Invoke();
		}

		public override void OnFixedUpdate()
		{
			method?.Invoke();
		}
	}
	public static class FsmVariables
	{
		public static NamedVariable[] GetVariables<T>(this PlayMakerFSM fsm) where T : NamedVariable, new()
		{
			FsmVariables fsmVariables = fsm.FsmVariables;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(FsmFloat))
			{
				return (NamedVariable[])(object)fsmVariables.FloatVariables;
			}
			if (typeFromHandle == typeof(FsmInt))
			{
				return (NamedVariable[])(object)fsmVariables.IntVariables;
			}
			if (typeFromHandle == typeof(FsmBool))
			{
				return (NamedVariable[])(object)fsmVariables.BoolVariables;
			}
			if (typeFromHandle == typeof(FsmString))
			{
				return (NamedVariable[])(object)fsmVariables.StringVariables;
			}
			if (typeFromHandle == typeof(FsmVector2))
			{
				return (NamedVariable[])(object)fsmVariables.Vector2Variables;
			}
			if (typeFromHandle == typeof(FsmVector3))
			{
				return (NamedVariable[])(object)fsmVariables.Vector3Variables;
			}
			if (typeFromHandle == typeof(FsmColor))
			{
				return (NamedVariable[])(object)fsmVariables.ColorVariables;
			}
			if (typeFromHandle == typeof(FsmRect))
			{
				return (NamedVariable[])(object)fsmVariables.RectVariables;
			}
			if (typeFromHandle == typeof(FsmQuaternion))
			{
				return (NamedVariable[])(object)fsmVariables.QuaternionVariables;
			}
			if (typeFromHandle == typeof(FsmGameObject))
			{
				return (NamedVariable[])(object)fsmVariables.GameObjectVariables;
			}
			return null;
		}

		public static void SetVariables<T>(this PlayMakerFSM fsm, NamedVariable[] namedVariables) where T : NamedVariable, new()
		{
			FsmVariables fsmVariables = fsm.FsmVariables;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(FsmFloat))
			{
				fsmVariables.FloatVariables = (FsmFloat[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmInt))
			{
				fsmVariables.IntVariables = (FsmInt[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmBool))
			{
				fsmVariables.BoolVariables = (FsmBool[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmString))
			{
				fsmVariables.StringVariables = (FsmString[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmVector2))
			{
				fsmVariables.Vector2Variables = (FsmVector2[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmVector3))
			{
				fsmVariables.Vector3Variables = (FsmVector3[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmColor))
			{
				fsmVariables.ColorVariables = (FsmColor[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmRect))
			{
				fsmVariables.RectVariables = (FsmRect[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmQuaternion))
			{
				fsmVariables.QuaternionVariables = (FsmQuaternion[])(object)namedVariables;
			}
			if (typeFromHandle == typeof(FsmGameObject))
			{
				fsmVariables.GameObjectVariables = (FsmGameObject[])(object)namedVariables;
			}
		}

		public static void AddVariable<T>(this PlayMakerFSM fsm, string name) where T : NamedVariable, new()
		{
			fsm.AddGetVariable<T>(name);
		}

		public static T AddGetVariable<T>(this PlayMakerFSM fsm, string name) where T : NamedVariable, new()
		{
			T[] array = (T[])(object)fsm.GetVariables<T>();
			if (array == null)
			{
				return default(T);
			}
			List<T> list = new List<T>(array);
			T val = new T();
			((NamedVariable)val).Name = name;
			T val2 = val;
			list.Add(val2);
			NamedVariable[] namedVariables = (NamedVariable[])(object)list.ToArray();
			fsm.SetVariables<T>(namedVariables);
			return val2;
		}

		public static T GetVariable<T>(this PlayMakerFSM fsm, string name) where T : NamedVariable, new()
		{
			T[] array = (T[])(object)fsm.GetVariables<T>();
			if (array == null)
			{
				return default(T);
			}
			return Array.Find(array, (T x) => ((NamedVariable)x).Name == name);
		}
	}
	public static class InterceptorExtensions
	{
		public static bool HasIntercept(this PlayMakerFSM fsm, TransitionInterceptor interceptor)
		{
			string stateName = "Satchel_Intercept_" + interceptor.fromState + "_" + interceptor.eventName;
			FsmState state;
			return fsm.TryGetState(stateName, out state);
		}

		public static void Intercept(this PlayMakerFSM fsm, TransitionInterceptor interceptor)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			string text = "Satchel_Intercept_" + interceptor.fromState + "_" + interceptor.eventName;
			FsmState state = new FsmState(fsm.Fsm)
			{
				Name = text
			};
			state.InsertAction((FsmStateAction)(object)new InterceptorAction
			{
				method = () => interceptor.GetNextEventOrDefault(interceptor.fromState, interceptor.eventName)
			}, 0);
			fsm.AddState(state);
			fsm.ChangeTransition(interceptor.fromState, interceptor.eventName, text);
			fsm.AddTransition(text, TransitionInterceptor.defaultEvent, interceptor.toStateDefault);
			fsm.AddTransition(text, TransitionInterceptor.customEvent, interceptor.toStateCustom);
		}

		public static void Reverse(this PlayMakerFSM fsm, TransitionInterceptor interceptor)
		{
			string text = "Satchel_Intercept_" + interceptor.fromState + "_" + interceptor.eventName;
			if (fsm.TryGetState(text, out var state))
			{
				fsm.ChangeTransition(interceptor.fromState, interceptor.eventName, interceptor.toStateDefault);
				fsm.RemoveTransition(text, TransitionInterceptor.defaultEvent);
				fsm.RemoveTransition(text, TransitionInterceptor.customEvent);
				fsm.RemoveState(state.Name);
				return;
			}
			throw new ArgumentException("FSM \"" + fsm.FsmName + "\" does not have state \"" + text + "\"");
		}

		public static bool HasIntercept(this PlayMakerFSM fsm, EventInterceptor interceptor)
		{
			string stateName = "Intercept_Event_" + interceptor.fromState + "_" + interceptor.eventName;
			FsmState state;
			return fsm.TryGetState(stateName, out state);
		}

		public static void Intercept(this PlayMakerFSM fsm, EventInterceptor interceptor)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			string text = "Intercept_Event_" + interceptor.fromState + "_" + interceptor.eventName;
			FsmState state = new FsmState(fsm.Fsm)
			{
				Name = text
			};
			state.InsertAction((FsmStateAction)(object)new InterceptorAction
			{
				method = delegate
				{
					if (interceptor.shouldIntercept != null && interceptor.shouldIntercept())
					{
						interceptor.onIntercept();
					}
					return "back to " + interceptor.fromState;
				}
			}, 0);
			fsm.AddState(state);
			fsm.AddTransition(interceptor.fromState, interceptor.eventName, text);
			fsm.AddTransition(text, "back to " + interceptor.fromState, interceptor.fromState);
		}

		public static void Reverse(this PlayMakerFSM fsm, EventInterceptor interceptor)
		{
			string text = "Intercept_Event_" + interceptor.fromState + "_" + interceptor.eventName;
			if (fsm.TryGetState(text, out var state))
			{
				fsm.RemoveTransition(interceptor.fromState, interceptor.eventName);
				fsm.RemoveTransition(text, "back to " + interceptor.fromState);
				fsm.RemoveState(state.Name);
				return;
			}
			throw new ArgumentException("FSM \"" + fsm.FsmName + "\" does not have state \"" + text + "\"");
		}
	}
}
namespace Satchel.Futils.Serialiser
{
	internal static class ActionReader
	{
		public unsafe static object GetFsmObject(this ActionData actionData, int index, int dataVersion)
		{
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Expected I4, but got Unknown
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Expected O, but got Unknown
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Expected O, but got Unknown
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Expected O, but got Unknown
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Expected O, but got Unknown
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Expected O, but got Unknown
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Expected O, but got Unknown
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Expected O, but got Unknown
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Expected O, but got Unknown
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Invalid comparison between Unknown and I4
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Invalid comparison between Unknown and I4
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Invalid comparison between Unknown and I4
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Invalid comparison between Unknown and I4
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Invalid comparison between Unknown and I4
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Invalid comparison between Unknown and I4
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(ReflectionHelper.GetField<ActionData, List<byte>>(actionData, "byteData").ToArray()));
			List<FsmEnum> field = ReflectionHelper.GetField<ActionData, List<FsmEnum>>(actionData, "fsmEnumParams");
			List<FsmBool> field2 = ReflectionHelper.GetField<ActionData, List<FsmBool>>(actionData, "fsmBoolParams");
			List<FsmInt> field3 = ReflectionHelper.GetField<ActionData, List<FsmInt>>(actionData, "fsmIntParams");
			List<FsmFloat> field4 = ReflectionHelper.GetField<ActionData, List<FsmFloat>>(actionData, "fsmFloatParams");
			List<FsmVector2> field5 = ReflectionHelper.GetField<ActionData, List<FsmVector2>>(actionData, "fsmVector2Params");
			List<FsmVector3> field6 = ReflectionHelper.GetField<ActionData, List<FsmVector3>>(actionData, "fsmVector3Params");
			List<FsmQuaternion> field7 = ReflectionHelper.GetField<ActionData, List<FsmQuaternion>>(actionData, "fsmQuaternionParams");
			List<FsmColor> field8 = ReflectionHelper.GetField<ActionData, List<FsmColor>>(actionData, "fsmColorParams");
			List<FsmRect> field9 = ReflectionHelper.GetField<ActionData, List<FsmRect>>(actionData, "fsmRectParams");
			List<FsmGameObject> field10 = ReflectionHelper.GetField<ActionData, List<FsmGameObject>>(actionData, "fsmGameObjectParams");
			List<FsmOwnerDefault> field11 = ReflectionHelper.GetField<ActionData, List<FsmOwnerDefault>>(actionData, "fsmOwnerDefaultParams");
			List<FsmObject> field12 = ReflectionHelper.GetField<ActionData, List<FsmObject>>(actionData, "fsmObjectParams");
			List<FsmVar> field13 = ReflectionHelper.GetField<ActionData, List<FsmVar>>(actionData, "fsmVarParams");
			List<FsmString> field14 = ReflectionHelper.GetField<ActionData, List<FsmString>>(actionData, "fsmStringParams");
			List<string> field15 = ReflectionHelper.GetField<ActionData, List<string>>(actionData, "stringParams");
			List<FsmEventTarget> field16 = ReflectionHelper.GetField<ActionData, List<FsmEventTarget>>(actionData, "fsmEventTargetParams");
			List<FsmArray> field17 = ReflectionHelper.GetField<ActionData, List<FsmArray>>(actionData, "fsmArrayParams");
			List<Object> field18 = ReflectionHelper.GetField<ActionData, List<Object>>(actionData, "unityObjectParams");
			List<FunctionCall> field19 = ReflectionHelper.GetField<ActionData, List<FunctionCall>>(actionData, "functionCallParams");
			ParamDataType val = ReflectionHelper.GetField<ActionData, List<ParamDataType>>(actionData, "paramDataType")[index];
			int num = ReflectionHelper.GetField<ActionData, List<int>>(actionData, "paramDataPos")[index];
			int num2 = ReflectionHelper.GetField<ActionData, List<int>>(actionData, "paramByteDataSize")[index];
			binaryReader.BaseStream.Position = num;
			ParamDataType val2 = val;
			object obj;
			Rect val4;
			switch ((int)val2)
			{
			case 0:
				obj = binaryReader.ReadInt32();
				break;
			case 16:
				if (dataVersion == 1)
				{
					obj = (object)new FsmInt
					{
						Value = binaryReader.ReadInt32()
					};
					break;
				}
				if (dataVersion > 1)
				{
					obj = field3[num];
					break;
				}
				goto default;
			case 7:
				obj = binaryReader.ReadInt32();
				break;
			case 1:
				obj = binaryReader.ReadBoolean();
				break;
			case 17:
				if (dataVersion == 1)
				{
					obj = (object)new FsmBool
					{
						Value = binaryReader.ReadBoolean()
					};
					break;
				}
				if (dataVersion > 1)
				{
					obj = field2[num];
					break;
				}
				goto default;
			case 2:
				obj = binaryReader.ReadSingle();
				break;
			case 15:
				if (dataVersion == 1)
				{
					obj = (object)new FsmFloat
					{
						Value = binaryReader.ReadSingle()
					};
					break;
				}
				if (dataVersion > 1)
				{
					obj = field4[num];
					break;
				}
				goto default;
			case 3:
				obj = Encoding.UTF8.GetString(binaryReader.ReadBytes(num2));
				break;
			case 23:
				obj = ((dataVersion != 1) ? ((object)field15[num]) : ((object)new FsmEvent
				{
					name = Encoding.UTF8.GetString(binaryReader.ReadBytes(num2))
				}));
				break;
			case 8:
				obj = (object)new Vector2
				{
					x = binaryReader.ReadSingle(),
					y = binaryReader.ReadSingle()
				};
				break;
			case 37:
				if (dataVersion == 1)
				{
					obj = (object)new FsmVector2
					{
						Value = new Vector2
						{
							x = binaryReader.ReadSingle(),
							y = binaryReader.ReadSingle()
						}
					};
					break;
				}
				if (dataVersion > 1)
				{
					obj = field5[num];
					break;
				}
				goto default;
			case 9:
				obj = (object)new Vector3
				{
					x = binaryReader.ReadSingle(),
					y = binaryReader.ReadSingle(),
					z = binaryReader.ReadSingle()
				};
				break;
			case 28:
				if (dataVersion == 1)
				{
					obj = (object)new FsmVector3
					{
						Value = new Vector3
						{
							x = binaryReader.ReadSingle(),
							y = binaryReader.ReadSingle(),
							z = binaryReader.ReadSingle()
						}
					};
					break;
				}
				if (dataVersion > 1)
				{
					obj = field6[num];
					break;
				}
				goto default;
			case 34:
				obj = (object)new Quaternion
				{
					x = binaryReader.ReadSingle(),
					y = binaryReader.ReadSingle(),
					z = binaryReader.ReadSingle(),
					w = binaryReader.ReadSingle()
				};
				break;
			case 35:
				if (dataVersion == 1)
				{
					obj = (object)new FsmQuaternion
					{
						Value = new Quaternion
						{
							x = binaryReader.ReadSingle(),
							y = binaryReader.ReadSingle(),
							z = binaryReader.ReadSingle(),
							w = binaryReader.ReadSingle()
						}
					};
					break;
				}
				if (dataVersion > 1)
				{
					obj = field7[num];
					break;
				}
				goto default;
			case 4:
				obj = (object)new Color
				{
					r = binaryReader.ReadSingle(),
					g = binaryReader.ReadSingle(),
					b = binaryReader.ReadSingle(),
					a = binaryReader.ReadSingle()
				};
				break;
			case 25:
				if (dataVersion == 1)
				{
					obj = (object)new FsmColor
					{
						Value = new Color
						{
							r = binaryReader.ReadSingle(),
							g = binaryReader.ReadSingle(),
							b = binaryReader.ReadSingle(),
							a = binaryReader.ReadSingle()
						}
					};
					break;
				}
				if (dataVersion > 1)
				{
					obj = field8[num];
					break;
				}
				goto default;
			case 11:
				val4 = default(Rect);
				((Rect)(ref val4)).x = binaryReader.ReadSingle();
				((Rect)(ref val4)).y = binaryReader.ReadSingle();
				((Rect)(ref val4)).width = binaryReader.ReadSingle();
				((Rect)(ref val4)).height = binaryReader.ReadSingle();
				obj = val4;
				break;
			case 30:
				if (dataVersion == 1)
				{
					FsmRect val3 = new FsmRect();
					val4 = default(Rect);
					((Rect)(ref val4)).x = binaryReader.ReadSingle();
					((Rect)(ref val4)).y = binaryReader.ReadSingle();
					((Rect)(ref val4)).width = binaryReader.ReadSingle();
					((Rect)(ref val4)).height = binaryReader.ReadSingle();
					val3.Value = val4;
					obj = (object)val3;
					break;
				}
				if (dataVersion > 1)
				{
					obj = field9[num];
					break;
				}
				goto default;
			case 42:
				if (dataVersion > 1)
				{
					obj = field[num];
					break;
				}
				goto default;
			case 19:
				obj = field10[num];
				break;
			case 20:
				obj = field11[num];
				break;
			case 24:
				obj = field12[num];
				break;
			case 39:
				obj = field13[num];
				break;
			case 18:
				obj = field14[num];
				break;
			case 31:
				obj = field16[num];
				break;
			case 41:
				obj = field17[num];
				break;
			case 5:
				obj = $"ObjRef([{field18[num]}])";
				break;
			case 21:
				obj = field19[num];
				break;
			case 12:
				obj = "[Array]";
				break;
			default:
				obj = "[" + ((object)(*(ParamDataType*)(&val))/*cast due to constrained. prefix*/).ToString() + " not implemented]";
				break;
			}
			NamedVariable val5;
			if (dataVersion == 1)
			{
				val5 = (NamedVariable)((obj is NamedVariable) ? obj : null);
				if (val5 != null)
				{
					if ((int)val <= 25)
					{
						if (val - 15 <= 2 || (int)val == 25)
						{
							goto IL_0793;
						}
					}
					else if ((int)val == 28 || (int)val == 35 || (int)val == 37)
					{
						goto IL_0793;
					}
				}
			}
			goto IL_07cd;
			IL_07cd:
			return obj;
			IL_0793:
			val5.UseVariable = binaryReader.ReadBoolean();
			int count = num2 - ((int)binaryReader.BaseStream.Position - num);
			val5.Name = Encoding.UTF8.GetString(binaryReader.ReadBytes(count));
			goto IL_07cd;
		}
	}
	public static class Serialiser
	{
		public static void EmitJsonFile(this PlayMakerFSM fsm)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			string contents = JsonConvert.SerializeObject((object)new FsmDataInstance(fsm), (Formatting)1, new JsonSerializerSettings
			{
				ReferenceLoopHandling = (ReferenceLoopHandling)1,
				Error = delegate(object sender, ErrorEventArgs args)
				{
					((Loggable)Satchel.Instance).LogError(args.ErrorContext.Error.Message);
					args.ErrorContext.Handled = true;
				}
			});
			File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), ((Object)((Component)fsm).gameObject).name + "-" + fsm.FsmName + ".json"), contents);
		}
	}
	public static class utils
	{
		internal static ActionScriptEntry GetActionData(FsmState state, ActionScriptEntry ase, int i, int dataVersion)
		{
			ActionData actionData = state.ActionData;
			List<string> field = ReflectionHelper.GetField<ActionData, List<string>>(actionData, "actionNames");
			((Loggable)Satchel.Instance).Log(field[i] + " | " + ase.Name);
			if (ase.Name != field[i])
			{
				return ase;
			}
			List<string> field2 = ReflectionHelper.GetField<ActionData, List<string>>(actionData, "paramName");
			List<int> field3 = ReflectionHelper.GetField<ActionData, List<int>>(actionData, "actionStartIndex");
			List<ParamDataType> field4 = ReflectionHelper.GetField<ActionData, List<ParamDataType>>(actionData, "paramDataType");
			int num = field3[i];
			int num2 = ((i != field.Count - 1) ? field3[i + 1] : field4.Count);
			for (int j = num; j < num2; j++)
			{
				string item = field2[j];
				object fsmObject = actionData.GetFsmObject(j, dataVersion);
				ase.Values.Add(new Tuple<string, object>(item, fsmObject));
			}
			return ase;
		}
	}
	public class FsmState
	{
		public string name;

		public string description;

		public FsmTransition[] transitions;

		public List<ActionScriptEntry> actionData;

		public byte colorIndex;

		public Rect position;

		public bool isBreakpoint;

		public bool isSequence;

		public bool hideUnused;

		public FsmState(FsmState state)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			base..ctor();
			name = state.Name;
			description = state.Description;
			colorIndex = (byte)state.ColorIndex;
			position = state.Position;
			isBreakpoint = state.IsBreakpoint;
			isSequence = state.IsSequence;
			hideUnused = state.HideUnused;
			transitions = new FsmTransition[state.Transitions.Length];
			int num = 0;
			FsmTransition[] array = state.Transitions;
			foreach (FsmTransition transition in array)
			{
				transitions[num] = new FsmTransition(transition);
				num++;
			}
			actionData = new List<ActionScriptEntry>();
			num = 0;
			FsmStateAction[] actions = state.Actions;
			foreach (FsmStateAction val in actions)
			{
				ActionScriptEntry item = new ActionScriptEntry
				{
					Name = ((object)val).GetType().ToString(),
					Enabled = val.Enabled,
					Values = new List<Tuple<string, object>>()
				};
				actionData.Add(item);
				num++;
			}
		}

		public override string ToString()
		{
			return "State " + name;
		}
	}
	public class FsmEvent
	{
		public string name;

		public bool isSystemEvent;

		public bool isGlobal;

		public FsmEvent()
		{
		}

		public FsmEvent(FsmEvent ev)
		{
			name = ev.Name;
			isSystemEvent = ev.IsSystemEvent;
			isGlobal = ev.IsGlobal;
		}

		public override string ToString()
		{
			return "Event(" + name + ")";
		}
	}
	public class FsmTransition
	{
		public FsmEvent fsmEvent;

		public string toState;

		public int linkStyle;

		public int linkConstraint;

		public byte colorIndex;

		public FsmTransition(FsmTransition transition)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected I4, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected I4, but got Unknown
			base..ctor();
			toState = transition.ToState;
			fsmEvent = new FsmEvent(transition.FsmEvent);
			linkStyle = (int)transition.LinkStyle;
			linkConstraint = (int)transition.LinkConstraint;
			colorIndex = (byte)transition.ColorIndex;
		}

		public override string ToString()
		{
			return "Transition(" + fsmEvent.name + " -> " + toState + ")";
		}
	}
	public class FsmDataInstance
	{
		public string fsmName;

		public string goName;

		public List<FsmState> states = new List<FsmState>();

		public List<FsmEvent> events = new List<FsmEvent>();

		public List<FsmVariableData> variables = new List<FsmVariableData>();

		public List<FsmNodeData> globalTransitions = new List<FsmNodeData>();

		public int dataVersion;

		public FsmDataInstance(PlayMakerFSM fsm)
		{
			fsmName = fsm.Fsm.Name;
			dataVersion = fsm.Fsm.DataVersion;
			goName = ((Object)((Component)fsm).gameObject).name;
			int num = 0;
			fsm.GiveStatesPosition();
			FsmState[] array = fsm.Fsm.States;
			foreach (FsmState state in array)
			{
				states.Add(new FsmState(state));
				int num2 = 0;
				foreach (ActionScriptEntry actionDatum in states[num].actionData)
				{
					try
					{
						utils.GetActionData(state, actionDatum, num2, dataVersion);
					}
					catch (Exception ex)
					{
						((Loggable)Satchel.Instance).Log(ex.ToString());
					}
					num2++;
				}
				num++;
			}
			FsmEvent[] array2 = fsm.Fsm.Events;
			foreach (FsmEvent ev in array2)
			{
				events.Add(new FsmEvent(ev));
			}
			FsmVariables val = fsm.Fsm.Variables;
			List<FsmVariableData> list = variables;
			NamedVariable[] floatVariables = (NamedVariable[])(object)val.FloatVariables;
			list.Add(new FsmVariableData("Floats", floatVariables));
			List<FsmVariableData> list2 = variables;
			floatVariables = (NamedVariable[])(object)val.IntVariables;
			list2.Add(new FsmVariableData("Ints", floatVariables));
			List<FsmVariableData> list3 = variables;
			floatVariables = (NamedVariable[])(object)val.BoolVariables;
			list3.Add(new FsmVariableData("Bools", floatVariables));
			List<FsmVariableData> list4 = variables;
			floatVariables = (NamedVariable[])(object)val.StringVariables;
			list4.Add(new FsmVariableData("Strings", floatVariables));
			List<FsmVariableData> list5 = variables;
			floatVariables = (NamedVariable[])(object)val.Vector2Variables;
			list5.Add(new FsmVariableData("Vector2s", floatVariables));
			List<FsmVariableData> list6 = variables;
			floatVariables = (NamedVariable[])(object)val.Vector3Variables;
			list6.Add(new FsmVariableData("Vector3s", floatVariables));
			List<FsmVariableData> list7 = variables;
			floatVariables = (NamedVariable[])(object)val.ColorVariables;
			list7.Add(new FsmVariableData("Colors", floatVariables));
			List<FsmVariableData> list8 = variables;
			floatVariables = (NamedVariable[])(object)val.RectVariables;
			list8.Add(new FsmVariableData("Rects", floatVariables));
			List<FsmVariableData> list9 = variables;
			floatVariables = (NamedVariable[])(object)val.QuaternionVariables;
			list9.Add(new FsmVariableData("Quaternions", floatVariables));
			List<FsmVariableData> list10 = variables;
			floatVariables = (NamedVariable[])(object)val.GameObjectVariables;
			list10.Add(new FsmVariableData("GameObjects", floatVariables));
			List<FsmVariableData> list11 = variables;
			floatVariables = (NamedVariable[])(object)val.ObjectVariables;
			list11.Add(new FsmVariableData("Objects", floatVariables));
			List<FsmVariableData> list12 = variables;
			floatVariables = (NamedVariable[])(object)val.MaterialVariables;
			list12.Add(new FsmVariableData("Materials", floatVariables));
			List<FsmVariableData> list13 = variables;
			floatVariables = (NamedVariable[])(object)val.TextureVariables;
			list13.Add(new FsmVariableData("Textures", floatVariables));
		}
	}
	public class FsmVariableData
	{
		public string Type { get; set; }

		public List<Tuple<string, object>> Values { get; set; }

		public FsmVariableData(string type, NamedVariable[] Vars)
		{
			Type = type;
			Values = new List<Tuple<string, object>>();
			foreach (NamedVariable val in Vars)
			{
				Values.Add(new Tuple<string, object>(val.Name, val.RawValue));
			}
		}
	}
	public class FsmGlobalTransition
	{
		public FsmEvent fsmEvent;

		public string toState;

		public int linkStyle;

		public int linkConstraint;

		public byte colorIndex;
	}
	public class ActionScriptEntry
	{
		public string Name;

		public List<Tuple<string, object>> Values;

		public bool Enabled;
	}
	public class FsmNodeData
	{
		public bool isGlobal;

		public string name;

		public FsmTransition[] transitions;

		public Rect transform;

		public Color stateColor;

		public Color transitionColor;
	}
}
namespace Satchel.Futils.Extractors
{
	public static class AudioClips
	{
		public static Dictionary<string, AudioClip> GetAudioClips(this PlayMakerFSM self)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Expected O, but got Unknown
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Expected O, but got Unknown
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Expected O, but got Unknown
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Expected O, but got Unknown
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Expected O, but got Unknown
			Dictionary<string, AudioClip> dictionary = new Dictionary<string, AudioClip>();
			FsmState[] fsmStates = self.FsmStates;
			for (int i = 0; i < fsmStates.Length; i++)
			{
				FsmStateAction[] actions = fsmStates[i].Actions;
				foreach (FsmStateAction val in actions)
				{
					((object)val).GetType();
					if (val is AudioPlay)
					{
						AudioPlay val2 = (AudioPlay)val;
						if (val2.oneShotClip == null)
						{
							((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							continue;
						}
						Object value = val2.oneShotClip.Value;
						AudioClip val3 = (AudioClip)(object)((value is AudioClip) ? value : null);
						dictionary[((Object)val3).name] = val3;
					}
					else if (val is AudioPlayerOneShot)
					{
						AudioClip[] audioClips = ((AudioPlayerOneShot)val).audioClips;
						foreach (AudioClip val4 in audioClips)
						{
							if ((Object)(object)val4 == (Object)null)
							{
								((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							}
							else
							{
								dictionary[((Object)val4).name] = val4;
							}
						}
					}
					else if (val is AudioPlayerOneShotSingle)
					{
						AudioPlayerOneShotSingle val5 = (AudioPlayerOneShotSingle)val;
						if (val5.audioClip == null)
						{
							((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							continue;
						}
						Object value2 = val5.audioClip.Value;
						AudioClip val6 = (AudioClip)(object)((value2 is AudioClip) ? value2 : null);
						dictionary[((Object)val6).name] = val6;
					}
					else if (val is AudioPlayRandom)
					{
						AudioClip[] audioClips = ((AudioPlayRandom)val).audioClips;
						foreach (AudioClip val7 in audioClips)
						{
							if ((Object)(object)val7 == (Object)null)
							{
								((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							}
							else
							{
								dictionary[((Object)val7).name] = val7;
							}
						}
					}
					else if (val is AudioPlayRandomSingle)
					{
						AudioPlayRandomSingle val8 = (AudioPlayRandomSingle)val;
						if (val8.audioClip == null)
						{
							((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							continue;
						}
						Object value3 = val8.audioClip.Value;
						AudioClip val9 = (AudioClip)(object)((value3 is AudioClip) ? value3 : null);
						dictionary[((Object)val9).name] = val9;
					}
					else if (val is AudioPlaySimple)
					{
						AudioPlaySimple val10 = (AudioPlaySimple)val;
						if (val10.oneShotClip == null)
						{
							((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							continue;
						}
						Object value4 = val10.oneShotClip.Value;
						AudioClip val11 = (AudioClip)(object)((value4 is AudioClip) ? value4 : null);
						dictionary[((Object)val11).name] = val11;
					}
					else if (val is AudioPlayV2)
					{
						AudioPlayV2 val12 = (AudioPlayV2)val;
						if (val12.oneShotClip == null)
						{
							((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							continue;
						}
						Object value5 = val12.oneShotClip.Value;
						AudioClip val13 = (AudioClip)(object)((value5 is AudioClip) ? value5 : null);
						dictionary[((Object)val13).name] = val13;
					}
					else if (val is PlayRandomSound)
					{
						AudioClip[] audioClips = ((PlayRandomSound)val).audioClips;
						foreach (AudioClip val14 in audioClips)
						{
							if ((Object)(object)val14 == (Object)null)
							{
								((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							}
							else
							{
								dictionary[((Object)val14).name] = val14;
							}
						}
					}
					else if (val is PlaySound)
					{
						PlaySound val15 = (PlaySound)val;
						if (val15.clip == null)
						{
							((Loggable)Satchel.Instance).LogFine("clip null in action " + val.Name);
							continue;
						}
						Object value6 = val15.clip.Value;
						AudioClip val16 = (AudioClip)(object)((value6 is AudioClip) ? value6 : null);
						dictionary[((Object)val16).name] = val16;
					}
				}
			}
			return dictionary;
		}
	}
}
namespace Satchel.BetterMenus
{
	public class InputField : Element
	{
		public GameObject currentField;

		private static GameObject _prefab;

		private string _userInput = string.Empty;

		private Text _label;

		private Text _placeholderText;

		private InputField _inputField;

		private RectTransform _underLine;

		public int characterLimit;

		public string placeholder;

		public Action<string> storeValue;

		public Func<string> loadValue;

		public InputFieldConfig config;

		public static GameObject prefab
		{
			get
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_018c: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Expected O, but got Unknown
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01df: Unknown result type (might be due to invalid IL or missing references)
				//IL_021d: Unknown result type (might be due to invalid IL or missing references)
				//IL_022c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0231: Unknown result type (might be due to invalid IL or missing references)
				//IL_026e: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)_prefab == (Object)null)
				{
					GameObject val = new GameObject();
					Object.DontDestroyOnLoad((Object)(object)val);
					val.AddComponent<RectTransform>().sizeDelta = new Vector2(1000f, 60f);
					GameObject obj = DefaultControls.CreateInputField(default(Resources));
					obj.transform.SetParent(val.transform, false);
					((Object)obj).name = "InputField";
					obj.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 60f);
					obj.RemoveComponent<Image>();
					obj.transform.localPosition = new Vector3(375f, 0f);
					InputField component = obj.GetComponent<InputField>();
					component.textComponent.font = MenuResources.TrajanRegular;
					((Graphic)component.textComponent).color = Color.white;
					component.textComponent.alignment = (TextAnchor)4;
					component.textComponent.text = "";
					component.textComponent.fontSize = 46;
					component.contentType = (ContentType)0;
					component.characterLimit = 10;
					Text component2 = obj.Find("Placeholder").GetComponent<Text>();
					component2.font = MenuResources.TrajanRegular;
					((Graphic)component2).color = Color.grey;
					component2.alignment = (TextAnchor)4;
					component2.text = "";
					component2.fontStyle = (FontStyle)0;
					component2.fontSize = 46;
					GameObject val2 = new GameObject("Underline");
					val2.AddComponent<CanvasRenderer>();
					((Graphic)val2.AddComponent<Image>()).color = Color.white;
					val2.transform.SetParent(val.transform, false);
					Transform transform = val2.transform;
					transform.position += new Vector3(375f, -25f);
					GameObjectUtils.GetAddComponent<RectTransform>(val2).sizeDelta = new Vector2(125f, 3f);
					GameObject obj2 = DefaultControls.CreateText(default(Resources));
					Text componentInChildren = obj2.GetComponentInChildren<Text>();
					obj2.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, 60f);
					((Graphic)componentInChildren).color = Color.white;
					componentInChildren.fontSize = 46;
					componentInChildren.resizeTextMaxSize = 46;
					componentInChildren.font = MenuResources.TrajanBold;
					componentInChildren.alignment = (TextAnchor)3;
					((Object)obj2).name = "Label";
					Transform transform2 = obj2.transform;
					transform2.localPosition += new Vector3(-150f, 0f);
					obj2.transform.SetParent(val.transform, false);
					MenuSelectable addComponent = val.GetAddComponent<MenuSelectable>();
					addComponent.rightCursor = Utils.CreateRegularMenuCursor(val, leftSide: false);
					addComponent.leftCursor = Utils.CreateRegularMenuCursor(val, leftSide: true);
					addComponent.cancelAction = (CancelAction)14;
					val.SetActive(false);
					_prefab = val;
				}
				return _prefab;
			}
		}

		public string userInput
		{
			get
			{
				return _userInput;
			}
			set
			{
				_userInput = value;
				inputField.text = userInput;
			}
		}

		public Text label
		{
			get
			{
				if ((Object)(object)_label == (Object)null)
				{
					GameObject obj = currentField.Find("Label");
					_label = ((obj != null) ? obj.GetComponent<Text>() : null);
				}
				return _label;
			}
		}

		public Text placeholderText
		{
			get
			{
				if ((Object)(object)_placeholderText == (Object)null)
				{
					GameObject obj = currentField.Find("InputField");
					object obj2;
					if (obj == null)
					{
						obj2 = null;
					}
					else
					{
						GameObject obj3 = obj.Find("Placeholder");
						obj2 = ((obj3 != null) ? obj3.GetComponent<Text>() : null);
					}
					_placeholderText = (Text)obj2;
				}
				return _placeholderText;
			}
		}

		public InputField inputField
		{
			get
			{
				if ((Object)(object)_inputField == (Object)null)
				{
					GameObject obj = currentField.Find("InputField");
					_inputField = ((obj != null) ? obj.GetComponent<InputField>() : null);
				}
				return _inputField;
			}
		}

		public RectTransform underLine
		{
			get
			{
				if ((Object)(object)_underLine == (Object)null)
				{
					_underLine = currentField.Find("Underline").GetComponent<RectTransform>();
				}
				return _underLine;
			}
		}

		public InputField(string name, Action<string> _storeValue, Func<string> _loadValue, string _placeholder = "", int _characterLimit = 10, InputFieldConfig? _config = null, string Id = "__Usename")
			: base(Id, name)
		{
			config = _config ?? InputFieldConfig.DefaultText;
			Name = name;
			storeValue = _storeValue;
			loadValue = _loadValue;
			placeholder = _placeholder;
			characterLimit = _characterLimit;
		}

		private GameObject AddField(GameObject fieldParent)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			currentField = Object.Instantiate<GameObject>(prefab, fieldParent.transform);
			((Object)currentField).name = Name;
			currentField.transform.parent = fieldParent.transform;
			switch (config.saveType)
			{
			case InputFieldSaveType.ValueChanged:
			{
				OnChangeEvent val2 = new OnChangeEvent();
				((UnityEvent<string>)(object)val2).AddListener((UnityAction<string>)storeValue.Invoke);
				inputField.onValueChanged = val2;
				break;
			}
			case InputFieldSaveType.EditEnd:
			{
				SubmitEvent val = new SubmitEvent();
				((UnityEvent<string>)(object)val).AddListener((UnityAction<string>)storeValue.Invoke);
				inputField.onEndEdit = val;
				break;
			}
			}
			currentField.SetActive(true);
			return currentField;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			if (Name == null)
			{
				throw new ArgumentNullException("Name", "Name cannot be null");
			}
			if (storeValue == null)
			{
				throw new ArgumentNullException("storeValue", "StoreValue cannot be null");
			}
			if (loadValue == null)
			{
				throw new ArgumentNullException("loadValue", "LoadValue cannot be null");
			}
			if (placeholder == null)
			{
				throw new ArgumentNullException("placeholder", "PlaceHolder cannot be null");
			}
			GameObject val = default(GameObject);
			StaticContent.AddStaticPanel(c, Name + "panel", new RelVector2(new Vector2(1000f, 60f)), ref val);
			AddField(val);
			c.NavGraph.AddNavigationNode((Selectable)(object)inputField);
			currentField.GetComponent<MenuSelectable>().customCancelAction = delegate
			{
				Instance.CancelAction();
			};
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(val));
			}
			base.gameObject = val;
			base.OnBuilt += Update;
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(val);
		}

		public override void Update()
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			inputField.contentType = (ContentType)9;
			inputField.characterValidation = (CharacterValidation)0;
			inputField.inputType = (InputType)0;
			inputField.textComponent.fontSize = config.fontSize;
			inputField.characterLimit = characterLimit;
			placeholderText.text = placeholder;
			label.text = Name;
			userInput = loadValue();
			RectTransform component = ((Component)inputField).GetComponent<RectTransform>();
			Vector2 sizeDelta = component.sizeDelta;
			sizeDelta.x = config.inputBoxWidth;
			component.sizeDelta = sizeDelta;
			Transform transform = ((Component)component).transform;
			Vector3 localPosition = ((Component)component).transform.localPosition;
			localPosition.x = 500f - config.inputBoxWidth * 0.45f;
			transform.localPosition = localPosition;
			RectTransform obj = underLine;
			sizeDelta = underLine.sizeDelta;
			sizeDelta.x = config.inputBoxWidth * 0.9f;
			obj.sizeDelta = sizeDelta;
			Transform transform2 = ((Component)underLine).transform;
			localPosition = ((Component)underLine).transform.localPosition;
			localPosition.x = 500f - config.inputBoxWidth * 0.45f;
			transform2.localPosition = localPosition;
		}
	}
	public abstract class BaseElement
	{
		public string Name;

		public BaseElement Parent;

		public string Id { get; protected set; }

		public GameObject gameObject { get; protected set; }

		public bool isVisible { get; set; } = true;

		public event EventHandler<UpdateEventArgs> OnUpdate;

		public event EventHandler<VisibilityChangeEventArgs> OnVisibilityChange;

		public BaseElement()
		{
		}

		public BaseElement(string Id)
		{
			this.Id = Id;
		}

		public BaseElement(string Id, string Name)
		{
			this.Id = ((Id == "__UseName") ? Name : Id);
		}

		public abstract void Update();

		public void Show()
		{
			if (!isVisible)
			{
				isVisible = true;
				UpdateInternal();
				OnVisibilityChange?.Invoke(this, new VisibilityChangeEventArgs
				{
					Target = this
				});
			}
		}

		public void Hide()
		{
			if (isVisible)
			{
				isVisible = false;
				UpdateInternal();
				OnVisibilityChange?.Invoke(this, new VisibilityChangeEventArgs
				{
					Target = this
				});
			}
		}

		internal void UpdateInternal()
		{
			Update();
			OnUpdate?.Invoke(this, new UpdateEventArgs
			{
				Target = this
			});
			if (Parent != null)
			{
				((IContainer)Parent)?.Reflow();
			}
		}

		public void updateAfter(Action<BaseElement> callback)
		{
			callback(this);
			UpdateInternal();
		}
	}
	public abstract class Element : BaseElement
	{
		public event Action OnBuilt;

		public Element(string Id)
			: base(Id)
		{
		}

		public Element(string Id, string Name)
			: base(Id, Name)
		{
		}

		public abstract GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true);

		protected void OnBuiltInvoke()
		{
			OnBuilt?.Invoke();
		}
	}
	public abstract class MenuElement : BaseElement
	{
	}
	public class BaseEventArgs : EventArgs
	{
		public BaseElement Target { get; set; }
	}
	public class UpdateEventArgs : BaseEventArgs
	{
	}
	public class ValueChangeEventArgs : BaseEventArgs
	{
	}
	public class VisibilityChangeEventArgs : BaseEventArgs
	{
	}
	public class ReflowEventArgs : BaseEventArgs
	{
	}
	public class ContainerBuiltEventArgs : BaseEventArgs
	{
	}
	public class GameObjectRow
	{
		public List<GameObject> Row;

		public Element Parent;

		public GameObjectRow(List<GameObject> row)
		{
			Row = row;
		}

		public GameObjectRow(GameObject FirstGo)
		{
			Row = new List<GameObject> { FirstGo };
		}

		public GameObjectRow(GameObjectRow menuOptionGos)
		{
			Row = menuOptionGos.Row;
		}

		public GameObjectRow(GameObjectRow firstRow, GameObjectRow secondRow)
		{
			Row = new List<GameObject>();
			firstRow.Row.ForEach(delegate(GameObject go)
			{
				Row.Add(go);
			});
			secondRow.Row.ForEach(delegate(GameObject go)
			{
				Row.Add(go);
			});
		}

		public GameObjectRow()
		{
			Row = new List<GameObject>();
		}

		public int ActiveCount()
		{
			int num = 0;
			foreach (GameObject item in Row)
			{
				if ((Object)(object)item != (Object)null && item.activeInHierarchy)
				{
					num++;
				}
			}
			return num;
		}
	}
	public interface IContainer
	{
		event EventHandler<ReflowEventArgs> OnReflow;

		event EventHandler<ContainerBuiltEventArgs> OnBuilt;

		Element Find(string Id);

		void Reflow(bool silent = false);

		void TriggerBuiltEvent();
	}
	public interface IShadowElement
	{
		Element[] GetElements();

		Element Find(string Id);
	}
	public class KeyAndButtonActionSet : PlayerActionSet
	{
		public readonly PlayerAction KeyBind;

		public readonly PlayerAction ButtonBind;

		public bool IsPressed
		{
			get
			{
				if (!((OneAxisInputControl)KeyBind).IsPressed)
				{
					return ((OneAxisInputControl)ButtonBind).IsPressed;
				}
				return true;
			}
		}

		public bool WasPressed
		{
			get
			{
				if (!((OneAxisInputControl)KeyBind).WasPressed)
				{
					return ((OneAxisInputControl)ButtonBind).WasPressed;
				}
				return true;
			}
		}

		public bool WasReleased
		{
			get
			{
				if (!((OneAxisInputControl)KeyBind).WasReleased)
				{
					return ((OneAxisInputControl)ButtonBind).WasReleased;
				}
				return true;
			}
		}

		public bool WasRepeated
		{
			get
			{
				if (!((OneAxisInputControl)KeyBind).WasRepeated)
				{
					return ((OneAxisInputControl)ButtonBind).WasRepeated;
				}
				return true;
			}
		}

		public KeyAndButtonActionSet()
		{
			KeyBind = ((PlayerActionSet)this).CreatePlayerAction("KeyBind");
			ButtonBind = ((PlayerActionSet)this).CreatePlayerAction("ButtonBind");
		}

		public KeyAndButtonActionSet(Key defaultKeyBind, InputControlType defaultButtonBind)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected I4, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			this..ctor();
			KeyBind.AddDefaultBinding((Key[])(object)new Key[1] { (Key)(int)defaultKeyBind });
			ButtonBind.AddDefaultBinding(defaultButtonBind);
		}
	}
	public static class KeybindContent
	{
		public static ContentArea AddCustomButtonBind(this ContentArea content, string name, PlayerAction action, ButtonBindConfig config, out MappableControllerButton mappableControllerButton)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Expected O, but got Unknown
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Expected O, but got Unknown
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Expected O, but got Unknown
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Expected O, but got Unknown
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Expected O, but got Unknown
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Expected O, but got Unknown
			KeybindStyle keybindStyle = config.Style ?? KeybindStyle.VanillaStyle;
			config.selectableArea.GetValueOrDefault();
			GameObject val = new GameObject(name ?? "");
			Object.DontDestroyOnLoad((Object)(object)val);
			val.transform.SetParent(content.ContentObject.transform, false);
			val.AddComponent<CanvasRenderer>();
			RectTransform val2 = val.AddComponent<RectTransform>();
			RelVector2 val3 = new RelVector2(new Vector2(650f, 100f));
			RectTransformData baseTransformData = ((RelVector2)(ref val3)).GetBaseTransformData();
			((RectTransformData)(ref baseTransformData)).Apply(val2);
			content.Layout.ModifyNext(val2);
			MappableControllerButton mapButton = val.AddComponent<MappableControllerButton>();
			mapButton.InitCustomActions(action.Owner, action);
			MappableControllerButton obj = mapButton;
			((MenuSelectable)obj).cancelAction = (CancelAction)14;
			((MenuSelectable)obj).customCancelAction = delegate
			{
				mapButton.AbortRebind();
				config.CancelAction?.Invoke(mapButton);
			};
			content.NavGraph.AddNavigationNode((Selectable)(object)mapButton);
			GameObject val4 = new GameObject("Text");
			Object.DontDestroyOnLoad((Object)val4);
			val4.transform.SetParent(val.transform, false);
			val4.AddComponent<CanvasRenderer>();
			RectTransform val5 = val4.AddComponent<RectTransform>();
			val5.sizeDelta = new Vector2(0f, 0f);
			val5.anchorMin = new Vector2(0f, 0f);
			val5.anchorMax = new Vector2(1f, 1f);
			val5.anchoredPosition = new Vector2(0f, 0f);
			val5.pivot = new Vector2(0.5f, 0.5f);
			Text obj2 = val4.AddComponent<Text>();
			obj2.font = MenuResources.TrajanBold;
			obj2.fontSize = keybindStyle.LabelTextSize;
			obj2.resizeTextMaxSize = keybindStyle.LabelTextSize;
			obj2.alignment = (TextAnchor)3;
			obj2.text = config.Label;
			obj2.supportRichText = true;
			obj2.verticalOverflow = (VerticalWrapMode)1;
			obj2.horizontalOverflow = (HorizontalWrapMode)1;
			val4.AddComponent<FixVerticalAlign>();
			GameObject val6 = new GameObject("CursorLeft");
			Object.DontDestroyOnLoad((Object)(object)val6);
			val6.transform.SetParent(val.transform, false);
			val6.AddComponent<CanvasRenderer>();
			RectTransform obj3 = val6.AddComponent<RectTransform>();
			obj3.sizeDelta = new Vector2(154f, 112f);
			obj3.pivot = new Vector2(0.5f, 0.5f);
			obj3.anchorMin = new Vector2(0f, 0.5f);
			obj3.anchorMax = new Vector2(0f, 0.5f);
			obj3.anchoredPosition = new Vector2(-52f, 0f);
			((Transform)obj3).localScale = new Vector3(0.4f, 0.4f, 0.4f);
			Animator val7 = val6.AddComponent<Animator>();
			val7.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
			val7.updateMode = (AnimatorUpdateMode)2;
			val7.applyRootMotion = false;
			val6.AddComponent<Image>();
			((MenuSelectable)mapButton).leftCursor = val7;
			GameObject val8 = new GameObject("CursorRight");
			Object.DontDestroyOnLoad((Object)(object)val8);
			val8.transform.SetParent(val.transform, false);
			val8.AddComponent<CanvasRenderer>();
			RectTransform obj4 = val8.AddComponent<RectTransform>();
			obj4.sizeDelta = new Vector2(154f, 112f);
			obj4.pivot = new Vector2(0.5f, 0.5f);
			obj4.anchorMin = new Vector2(1f, 0.5f);
			obj4.anchorMax = new Vector2(1f, 0.5f);
			obj4.anchoredPosition = new Vector2(52f, 0f);
			((Transform)obj4).localScale = new Vector3(-0.4f, 0.4f, 0.4f);
			Animator val9 = val8.AddComponent<Animator>();
			val9.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
			val9.updateMode = (AnimatorUpdateMode)2;
			val9.applyRootMotion = false;
			val8.AddComponent<Image>();
			((MenuSelectable)mapButton).rightCursor = val9;
			GameObject val10 = new GameObject("Keymap");
			Object.DontDestroyOnLoad((Object)(object)val10);
			val10.transform.SetParent(val.transform, false);
			val10.AddComponent<CanvasRenderer>();
			RectTransform val11 = val10.AddComponent<RectTransform>();
			val11.sizeDelta = new Vector2(145.8f, 82.4f);
			val11.anchorMin = new Vector2(1f, 0.5f);
			val11.anchorMax = new Vector2(1f, 0.5f);
			val11.anchoredPosition = new Vector2(0f, 0f);
			val11.pivot = new Vector2(1f, 0.5f);
			Image val12 = val10.AddComponent<Image>();
			val12.preserveAspect = true;
			mapButton.buttonmapSprite = val12;
			GameObject val13 = new GameObject("Text");
			Object.DontDestroyOnLoad((Object)val13);
			val13.transform.SetParent(val10.transform, false);
			val13.AddComponent<CanvasRenderer>();
			RectTransform val14 = val13.AddComponent<RectTransform>();
			val14.sizeDelta = new Vector2(85f, 60f);
			val14.anchorMin = new Vector2(0f, 0.5f);
			val14.anchorMax = new Vector2(1f, 0.5f);
			val14.anchoredPosition = new Vector2(32f, 0f);
			val14.pivot = new Vector2(0.5f, 0.5f);
			Text val15 = val13.AddComponent<Text>();
			val15.font = MenuResources.Perpetua;
			val15.fontSize = 35;
			mapButton.buttonmapText = val15;
			val13.AddComponent<FixVerticalAlign>().labelFixType = (LabelFixType)1;
			GameObject val16 = new GameObject("throbber");
			Object.DontDestroyOnLoad((Object)(object)val16);
			val16.transform.SetParent(val10.transform, false);
			mapButton.listeningThrobber = val16.AddComponent<Throbber>();
			ReflectionHelper.SetField<Throbber, Sprite[]>(mapButton.listeningThrobber, "sprites", (Sprite[])(object)new Sprite[0]);
			mapButton.GetBindingPublic();
			mapButton.ShowCurrentBinding();
			mappableControllerButton = mapButton;
			ApplySelectableArea(config, val, val10, val6, val8, val11, val14, val5, val15);
			return content;
		}

		public static void ApplySelectableArea(MappableControllerButton mapButton, ButtonBindConfig config)
		{
			GameObject gameObject = ((Component)mapButton).gameObject;
			GameObject val = gameObject.Find("Keymap");
			GameObject cursorL = gameObject.Find("CursorLeft");
			GameObject cursorR = gameObject.Find("CursorRight");
			RectTransform component = val.GetComponent<RectTransform>();
			RectTransform component2 = val.Find("Text").GetComponent<RectTransform>();
			RectTransform component3 = gameObject.Find("Text").GetComponent<RectTransform>();
			Text component4 = val.Find("Text").GetComponent<Text>();
			ApplySelectableArea(config, gameObject, val, cursorL, cursorR, component, component2, component3, component4);
		}

		public static void ApplySelectableArea(ButtonBindConfig config, GameObject buttonBind, GameObject keymap, GameObject cursorL, GameObject cursorR, RectTransform keymapRt, RectTransform buttonmapTextRt, RectTransform textRt, Text buttonmapTextText)
		{
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			if (config.selectableArea == SelectableArea.ButtonOnly)
			{
				cursorL.transform.SetParent(keymap.transform, false);
				cursorR.transform.SetParent(keymap.transform, false);
				keymapRt.pivot = new Vector2(0.5f, 0.5f);
				buttonmapTextRt.anchoredPosition = new Vector2(0f, 0f);
				buttonmapTextText.alignment = (TextAnchor)4;
				if (config.Label.Length == 0)
				{
					textRt.anchorMin = new Vector2(0f, 0f);
					textRt.anchorMax = new Vector2(0f, 0f);
				}
			}
			else
			{
				cursorL.transform.SetParent(buttonBind.transform, false);
				cursorR.transform.SetParent(buttonBind.transform, false);
				keymapRt.pivot = new Vector2(1f, 0.5f);
				buttonmapTextRt.anchoredPosition = new Vector2(32f, 0f);
				buttonmapTextText.alignment = (TextAnchor)4;
				textRt.anchorMin = new Vector2(0f, 0f);
				textRt.anchorMax = new Vector2(1f, 1f);
			}
		}

		public static ContentArea AddCustomKeybind(this ContentArea content, string name, PlayerAction action, KeybindConfig config)
		{
			MappableKey mappableKey;
			return content.AddCustomKeybind(name, action, config, out mappableKey);
		}

		public static ContentArea AddCustomKeybind(this ContentArea content, string name, PlayerAction action, KeybindConfig config, out MappableKey mappableKey)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Expected O, but got Unknown
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Expected O, but got Unknown
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Expected O, but got Unknown
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Expected O, but got Unknown
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			KeybindStyle keybindStyle = config.Style ?? KeybindStyle.VanillaStyle;
			config.selectableArea.GetValueOrDefault();
			GameObject val = new GameObject(name ?? "");
			Object.DontDestroyOnLoad((Object)(object)val);
			val.transform.SetParent(content.ContentObject.transform, false);
			val.AddComponent<CanvasRenderer>();
			RectTransform val2 = val.AddComponent<RectTransform>();
			RelVector2 val3 = new RelVector2(new Vector2(650f, 100f));
			RectTransformData baseTransformData = ((RelVector2)(ref val3)).GetBaseTransformData();
			((RectTransformData)(ref baseTransformData)).Apply(val2);
			content.Layout.ModifyNext(val2);
			GameObject val4 = new GameObject("Text");
			Object.DontDestroyOnLoad((Object)val4);
			val4.transform.SetParent(val.transform, false);
			val4.AddComponent<CanvasRenderer>();
			RectTransform val5 = val4.AddComponent<RectTransform>();
			val5.sizeDelta = new Vector2(0f, 0f);
			val5.anchorMin = new Vector2(0f, 0f);
			val5.anchorMax = new Vector2(1f, 1f);
			val5.anchoredPosition = new Vector2(0f, 0f);
			val5.pivot = new Vector2(0.5f, 0.5f);
			Text obj = val4.AddComponent<Text>();
			obj.font = MenuResources.TrajanBold;
			obj.fontSize = keybindStyle.LabelTextSize;
			obj.resizeTextMaxSize = keybindStyle.LabelTextSize;
			obj.alignment = (TextAnchor)3;
			obj.text = config.Label;
			obj.supportRichText = true;
			obj.verticalOverflow = (VerticalWrapMode)1;
			obj.horizontalOverflow = (HorizontalWrapMode)1;
			val4.AddComponent<FixVerticalAlign>();
			GameObject val6 = new GameObject("CursorLeft");
			Object.DontDestroyOnLoad((Object)(object)val6);
			val6.transform.SetParent(val.transform, false);
			val6.AddComponent<CanvasRenderer>();
			RectTransform obj2 = val6.AddComponent<RectTransform>();
			obj2.sizeDelta = new Vector2(154f, 112f);
			obj2.pivot = new Vector2(0.5f, 0.5f);
			obj2.anchorMin = new Vector2(0f, 0.5f);
			obj2.anchorMax = new Vector2(0f, 0.5f);
			obj2.anchoredPosition = new Vector2(-52f, 0f);
			((Transform)obj2).localScale = new Vector3(0.4f, 0.4f, 0.4f);
			Animator val7 = val6.AddComponent<Animator>();
			val7.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
			val7.updateMode = (AnimatorUpdateMode)2;
			val7.applyRootMotion = false;
			val6.AddComponent<Image>();
			GameObject val8 = new GameObject("CursorRight");
			Object.DontDestroyOnLoad((Object)(object)val8);
			val8.transform.SetParent(val.transform, false);
			val8.AddComponent<CanvasRenderer>();
			RectTransform obj3 = val8.AddComponent<RectTransform>();
			obj3.sizeDelta = new Vector2(154f, 112f);
			obj3.pivot = new Vector2(0.5f, 0.5f);
			obj3.anchorMin = new Vector2(1f, 0.5f);
			obj3.anchorMax = new Vector2(1f, 0.5f);
			obj3.anchoredPosition = new Vector2(52f, 0f);
			((Transform)obj3).localScale = new Vector3(-0.4f, 0.4f, 0.4f);
			Animator val9 = val8.AddComponent<Animator>();
			val9.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
			val9.updateMode = (AnimatorUpdateMode)2;
			val9.applyRootMotion = false;
			val8.AddComponent<Image>();
			GameObject val10 = new GameObject("Keymap");
			Object.DontDestroyOnLoad((Object)(object)val10);
			val10.transform.SetParent(val.transform, false);
			val10.AddComponent<CanvasRenderer>();
			RectTransform val11 = val10.AddComponent<RectTransform>();
			val11.sizeDelta = new Vector2(145.8f, 82.4f);
			val11.anchorMin = new Vector2(1f, 0.5f);
			val11.anchorMax = new Vector2(1f, 0.5f);
			val11.anchoredPosition = new Vector2(0f, 0f);
			val11.pivot = new Vector2(1f, 0.5f);
			Image val12 = val10.AddComponent<Image>();
			val12.preserveAspect = true;
			GameObject val13 = new GameObject("Text");
			Object.DontDestroyOnLoad((Object)val13);
			val13.transform.SetParent(val10.transform, false);
			val13.AddComponent<CanvasRenderer>();
			RectTransform val14 = val13.AddComponent<RectTransform>();
			val14.sizeDelta = new Vector2(65f, 60f);
			val14.anchorMin = new Vector2(0.5f, 0.5f);
			val14.anchorMax = new Vector2(0.5f, 0.5f);
			val14.anchoredPosition = new Vector2(32f, 0f);
			val14.pivot = new Vector2(0.5f, 0.5f);
			Text val15 = val13.AddComponent<Text>();
			val15.font = MenuResources.Perpetua;
			val13.AddComponent<FixVerticalAlign>().labelFixType = (LabelFixType)1;
			MappableKey mapKey = val.AddComponent<MappableKey>();
			mapKey.InitCustomActions(action.Owner, action);
			MappableKey obj4 = mapKey;
			((MenuSelectable)obj4).cancelAction = (CancelAction)14;
			((MenuSelectable)obj4).customCancelAction = delegate
			{
				mapKey.AbortRebind();
				config.CancelAction?.Invoke(mapKey);
			};
			content.NavGraph.AddNavigationNode((Selectable)(object)mapKey);
			((MenuSelectable)mapKey).leftCursor = val7;
			((MenuSelectable)mapKey).rightCursor = val9;
			mapKey.keymapSprite = val12;
			mapKey.keymapText = val15;
			mapKey.GetBinding();
			mapKey.ShowCurrentBinding();
			mappableKey = mapKey;
			ApplySelectableArea(config, val, val10, val6, val8, val11, val14, val5, val15, mapKey);
			return content;
		}

		public static void ApplySelectableArea(MappableKey mapKey, KeybindConfig config)
		{
			GameObject gameObject = ((Component)mapKey).gameObject;
			GameObject val = gameObject.Find("Keymap");
			GameObject cursorL = gameObject.Find("CursorLeft");
			GameObject cursorR = gameObject.Find("CursorRight");
			RectTransform component = val.GetComponent<RectTransform>();
			RectTransform component2 = val.Find("Text").GetComponent<RectTransform>();
			RectTransform component3 = gameObject.Find("Text").GetComponent<RectTransform>();
			Text component4 = val.Find("Text").GetComponent<Text>();
			ApplySelectableArea(config, gameObject, val, cursorL, cursorR, component, component2, component3, component4, mapKey);
		}

		public static void ApplySelectableArea(KeybindConfig config, GameObject keybind, GameObject keymap, GameObject cursorL, GameObject cursorR, RectTransform keymapRt, RectTransform keymapTextRt, RectTransform textRt, Text keymapTextText, MappableKey mapKey)
		{
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			if (config.selectableArea == SelectableArea.ButtonOnly)
			{
				cursorL.transform.SetParent(keymap.transform, false);
				cursorR.transform.SetParent(keymap.transform, false);
				keymapRt.pivot = new Vector2(0.5f, 0.5f);
				keymapTextRt.anchoredPosition = new Vector2(0f, 0f);
				keymapTextText.alignment = (TextAnchor)4;
				ReflectionHelper.SetField<MappableKey, float>(mapKey, "sqrX", 0f);
				ReflectionHelper.SetField<MappableKey, float>(mapKey, "blankWidth", 70f);
				ReflectionHelper.SetField<MappableKey, int>(mapKey, "blankFontSize", 36);
				ReflectionHelper.SetField<MappableKey, int>(mapKey, "sqrMaxFont", 36);
				ReflectionHelper.SetField<MappableKey, int>(mapKey, "sqrFontSize", 36);
				ReflectionHelper.SetField<MappableKey, bool>(mapKey, "blankBestFit", true);
				ReflectionHelper.SetField<MappableKey, TextAnchor>(mapKey, "blankAlignment", (TextAnchor)4);
				if (config.Label.Length == 0)
				{
					textRt.anchorMin = new Vector2(0f, 0f);
					textRt.anchorMax = new Vector2(0f, 0f);
				}
			}
			else
			{
				cursorL.transform.SetParent(keybind.transform, false);
				cursorR.transform.SetParent(keybind.transform, false);
				keymapRt.pivot = new Vector2(1f, 0.5f);
				keymapTextRt.anchoredPosition = new Vector2(0f, 0f);
				keymapTextText.alignment = (TextAnchor)4;
				ReflectionHelper.SetField<MappableKey, float>(mapKey, "sqrX", 32f);
				ReflectionHelper.SetField<MappableKey, float>(mapKey, "blankWidth", 165f);
				ReflectionHelper.SetField<MappableKey, bool>(mapKey, "blankBestFit", false);
				ReflectionHelper.SetField<MappableKey, int>(mapKey, "blankFontSize", 36);
				ReflectionHelper.SetField<MappableKey, int>(mapKey, "sqrMaxFont", 36);
				ReflectionHelper.SetField<MappableKey, int>(mapKey, "sqrFontSize", 36);
				ReflectionHelper.SetField<MappableKey, TextAnchor>(mapKey, "blankAlignment", (TextAnchor)5);
				textRt.anchorMin = new Vector2(0f, 0f);
				textRt.anchorMax = new Vector2(1f, 1f);
			}
		}
	}
	public class ButtonBind : Element
	{
		public PlayerAction PlayerAction;

		public SelectableArea SelectableArea;

		private ButtonBindConfig buttonBindConfig;

		public ButtonBind(string name, PlayerAction playerAction, string Id = "__UseName")
			: base(Id, name)
		{
			Name = name;
			PlayerAction = playerAction;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			if (Name == null)
			{
				throw new ArgumentNullException("Name", "Name cannot be null");
			}
			if (PlayerAction == null)
			{
				throw new ArgumentNullException("PlayerAction", "PlayerAction cannot be null");
			}
			buttonBindConfig = new ButtonBindConfig
			{
				CancelAction = delegate
				{
					Instance.CancelAction();
				},
				Label = Name,
				selectableArea = SelectableArea
			};
			c.AddCustomButtonBind(Name, PlayerAction, buttonBindConfig, out var mappableControllerButton);
			base.gameObject = ((Component)mappableControllerButton).gameObject;
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(((Component)mappableControllerButton).gameObject));
			}
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(((Component)mappableControllerButton).gameObject);
		}

		public override void Update()
		{
			MappableControllerButton component = base.gameObject.GetComponent<MappableControllerButton>();
			component.InitCustomActions(PlayerAction.Owner, PlayerAction);
			((Component)base.gameObject.transform.Find("Text")).GetComponent<Text>().text = Name;
			KeybindContent.ApplySelectableArea(component, buttonBindConfig);
			component.ShowCurrentBinding();
		}
	}
	public class CustomSlider : Element
	{
		public GameObject currentSlider;

		private bool isReady;

		public float minValue;

		public float maxValue = 5f;

		public bool wholeNumbers;

		private float _value;

		private Text _label;

		private Text _valueLabel;

		private Slider _slider;

		private static GameObject _prefab;

		public Action<float> StoreValue;

		public Func<float> LoadValue;

		public static GameObject prefab
		{
			get
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)_prefab == (Object)null)
				{
					GameObject obj = Object.Instantiate<GameObject>(((Component)((Component)UIManager.instance).gameObject.transform.Find("UICanvas/AudioMenuScreen/Content/MusicVolume/MusicSlider")).gameObject);
					obj.SetActive(false);
					Slider component = obj.GetComponent<Slider>();
					Direction direction = component.direction;
					RectTransform fillRect = component.fillRect;
					RectTransform handleRect = component.handleRect;
					Image image = ((Selectable)component).image;
					SpriteState spriteState = ((Selectable)component).spriteState;
					Graphic targetGraphic = ((Selectable)component).targetGraphic;
					Object.DestroyImmediate((Object)(object)component);
					obj.RemoveComponent<Slider>();
					Slider obj2 = obj.AddComponent<Slider>();
					obj2.direction = direction;
					obj2.fillRect = fillRect;
					obj2.handleRect = handleRect;
					((Selectable)obj2).image = image;
					((Selectable)obj2).spriteState = spriteState;
					((Selectable)obj2).targetGraphic = targetGraphic;
					obj.RemoveComponent<MenuAudioSlider>();
					obj.Find("Label")?.RemoveComponent<AutoLocalizeTextUI>();
					obj.GetComponent<MenuPreventDeselect>().cancelAction = (CancelAction)14;
					_prefab = obj;
				}
				return _prefab;
			}
		}

		public float value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				UpdateValueLabel();
			}
		}

		public Text label
		{
			get
			{
				if ((Object)(object)_label == (Object)null)
				{
					GameObject obj = currentSlider.Find("Label");
					_label = ((obj != null) ? obj.GetComponent<Text>() : null);
				}
				return _label;
			}
		}

		public Text valueLabel
		{
			get
			{
				if ((Object)(object)_valueLabel == (Object)null)
				{
					GameObject obj = currentSlider.Find("MusicValue");
					_valueLabel = ((obj != null) ? obj.GetComponent<Text>() : null);
				}
				return _valueLabel;
			}
		}

		public Slider slider
		{
			get
			{
				if ((Object)(object)_slider == (Object)null)
				{
					_slider = currentSlider.GetComponent<Slider>();
				}
				return _slider;
			}
		}

		public GameObject AddSlider(GameObject SliderParent)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			currentSlider = Object.Instantiate<GameObject>(prefab, SliderParent.transform);
			((Object)currentSlider).name = Name;
			currentSlider.transform.parent = SliderParent.transform;
			Action<float> action = delegate(float newValue)
			{
				if (isReady && Math.Abs(minValue - slider.minValue) < Mathf.Epsilon && Math.Abs(maxValue - slider.maxValue) < Mathf.Epsilon && wholeNumbers == slider.wholeNumbers)
				{
					value = newValue;
					label.text = Name;
					StoreValue?.Invoke(newValue);
				}
			};
			SliderEvent val = new SliderEvent();
			((UnityEvent<float>)(object)val).AddListener((UnityAction<float>)action.Invoke);
			slider.onValueChanged = val;
			currentSlider.SetActive(true);
			return currentSlider;
		}

		private void FixSliderNavigation()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			Slider obj = slider;
			Navigation navigation = default(Navigation);
			((Navigation)(ref navigation)).mode = (Mode)4;
			Navigation navigation2 = ((Selectable)slider).navigation;
			((Navigation)(ref navigation)).selectOnUp = ((Navigation)(ref navigation2)).selectOnUp;
			navigation2 = ((Selectable)slider).navigation;
			((Navigation)(ref navigation)).selectOnDown = ((Navigation)(ref navigation2)).selectOnDown;
			((Navigation)(ref navigation)).selectOnLeft = null;
			((Navigation)(ref navigation)).selectOnRight = null;
			((Selectable)obj).navigation = navigation;
		}

		[Obsolete("This Constructor is obsolete, use the one with minValue and maxValue.")]
		public CustomSlider(string name, Action<float> storeValue, Func<float> loadValue, string Id = "__UseName")
			: base(Id, name)
		{
			Name = name;
			StoreValue = storeValue;
			LoadValue = loadValue;
		}

		public CustomSlider(string name, Action<float> storeValue, Func<float> loadValue, float minValue, float maxValue, bool wholeNumbers = false, string Id = "__UseName")
			: base(Id, name)
		{
			Name = name;
			StoreValue = storeValue;
			LoadValue = loadValue;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.wholeNumbers = wholeNumbers;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			if (Name == null)
			{
				throw new ArgumentNullException("Name", "Name cannot be null");
			}
			if (StoreValue == null)
			{
				throw new ArgumentNullException("StoreValue", "StoreValue cannot be null");
			}
			if (LoadValue == null)
			{
				throw new ArgumentNullException("LoadValue", "SavedValue cannot be null");
			}
			GameObject val = default(GameObject);
			StaticContent.AddStaticPanel(c, Name + "panel", new RelVector2(new Vector2(200f, 105f)), ref val);
			AddSlider(val);
			c.NavGraph.AddNavigationNode((Selectable)(object)slider);
			((Component)slider).GetComponent<MenuPreventDeselect>().customCancelAction = delegate
			{
				Instance.CancelAction();
			};
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(val));
			}
			base.gameObject = val;
			base.OnBuilt += Update;
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(val);
		}

		public override void Update()
		{
			slider.wholeNumbers = wholeNumbers;
			slider.minValue = minValue;
			slider.maxValue = maxValue;
			value = LoadValue();
			slider.value = value;
			isReady = true;
			label.text = Name;
			FixSliderNavigation();
		}

		protected virtual void UpdateValueLabel()
		{
			valueLabel.text = (wholeNumbers ? $"{value}" : $"{value:0.0}");
		}
	}
	public class HorizontalOption : Element
	{
		public string Description;

		public string[] Values;

		public Action<int> ApplySetting;

		public Func<int> LoadSetting;

		public HorizontalOption(string name, string description, string[] values, Action<int> applySetting, Func<int> loadSetting, string Id = "__UseName")
			: base(Id, name)
		{
			Name = name;
			Description = description;
			Values = values;
			ApplySetting = applySetting;
			LoadSetting = loadSetting;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected O, but got Unknown
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Expected O, but got Unknown
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			if (Name == null)
			{
				throw new ArgumentNullException("Name", "Name cannot be null");
			}
			if (Description == null)
			{
				throw new ArgumentNullException("Description", "Description cannot be null");
			}
			if (Values == null)
			{
				throw new ArgumentNullException("Values", "Values cannot be null");
			}
			if (ApplySetting == null)
			{
				throw new ArgumentNullException("ApplySetting", "ApplySetting cannot be null");
			}
			if (LoadSetting == null)
			{
				throw new ArgumentNullException("LoadSetting", "LoadSetting cannot be null");
			}
			MenuOptionHorizontal option = default(MenuOptionHorizontal);
			HorizontalOptionContent.AddHorizontalOption(c, Name, new HorizontalOptionConfig
			{
				ApplySetting = (ApplySetting)delegate(MenuSetting _, int i)
				{
					ApplySetting(i);
				},
				RefreshSetting = (RefreshSetting)delegate(MenuSetting s, bool _)
				{
					s.optionList.SetOptionTo(LoadSetting());
				},
				CancelAction = delegate
				{
					Instance.CancelAction();
				},
				Description = new DescriptionInfo
				{
					Text = Description
				},
				Label = Name,
				Options = Values,
				Style = HorizontalOptionStyle.VanillaStyle
			}, ref option);
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(((Component)option).gameObject));
			}
			base.gameObject = ((Component)option).gameObject;
			Action value = delegate
			{
				if ((Object)(object)option != (Object)null && (Object)(object)option.menuSetting != (Object)null)
				{
					option.menuSetting.RefreshValueFromGameSettings(false);
				}
			};
			base.OnBuilt += value;
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(((Component)option).gameObject);
		}

		public override void Update()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			MenuOptionHorizontal option = base.gameObject.GetComponent<MenuOptionHorizontal>();
			option.optionList = Values;
			option.menuSetting.customApplySetting = (ApplySetting)delegate(MenuSetting _, int i)
			{
				ApplySetting(i);
			};
			option.menuSetting.customRefreshSetting = (RefreshSetting)delegate(MenuSetting s, bool _)
			{
				int num = LoadSetting();
				if (num >= 0 && num < option.optionList.Length)
				{
					s.optionList.SetOptionTo(num);
				}
			};
			option.menuSetting.RefreshValueFromGameSettings(false);
			((Component)base.gameObject.transform.Find("Label")).GetComponent<Text>().text = Name;
			((Component)base.gameObject.transform.Find("Description")).GetComponent<Text>().text = Description;
		}
	}
	public class KeyBind : Element
	{
		public PlayerAction PlayerAction;

		public SelectableArea SelectableArea;

		private KeybindConfig keybindConfig;

		public KeyBind(string name, PlayerAction playerAction, string Id = "__UseName")
			: base(Id, name)
		{
			Name = name;
			PlayerAction = playerAction;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			if (Name == null)
			{
				throw new ArgumentNullException("Name", "Name cannot be null");
			}
			if (PlayerAction == null)
			{
				throw new ArgumentNullException("PlayerAction", "PlayerAction cannot be null");
			}
			keybindConfig = new KeybindConfig
			{
				CancelAction = delegate
				{
					Instance.CancelAction();
				},
				Label = Name,
				selectableArea = SelectableArea
			};
			c.AddCustomKeybind(Name, PlayerAction, keybindConfig, out var mappableKey);
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(((Component)mappableKey).gameObject));
			}
			base.gameObject = ((Component)mappableKey).gameObject;
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(((Component)mappableKey).gameObject);
		}

		public override void Update()
		{
			MappableKey component = base.gameObject.GetComponent<MappableKey>();
			component.InitCustomActions(PlayerAction.Owner, PlayerAction);
			((Component)base.gameObject.transform.Find("Text")).GetComponent<Text>().text = Name;
			KeybindContent.ApplySelectableArea(component, keybindConfig);
			component.ShowCurrentBinding();
		}
	}
	public class MenuButton : Element
	{
		public Action<MenuButton> SubmitAction;

		public string Description;

		public bool Proceed;

		public MenuButton(string name, string description, Action<MenuButton> submitAction, bool proceed = false, string Id = "__UseName")
			: base(Id, name)
		{
			Name = name;
			Description = description;
			SubmitAction = submitAction;
			Proceed = proceed;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			if (Name == null)
			{
				throw new ArgumentNullException("Name", "Name cannot be null");
			}
			if (SubmitAction == null)
			{
				throw new ArgumentNullException("SubmitAction", "SubmitAction cannot be null");
			}
			if (Description == null)
			{
				throw new ArgumentNullException("Description", "Description cannot be null");
			}
			MenuButton val = default(MenuButton);
			MenuButtonContent.AddMenuButton(c, Name, new MenuButtonConfig
			{
				CancelAction = delegate
				{
					Instance.CancelAction();
				},
				Description = new DescriptionInfo
				{
					Text = Description
				},
				Label = Name,
				Proceed = Proceed,
				SubmitAction = SubmitAction
			}, ref val);
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(((Component)val).gameObject));
			}
			base.gameObject = ((Component)val).gameObject;
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(((Component)val).gameObject);
		}

		public override void Update()
		{
			((Component)base.gameObject.transform.Find("Label")).GetComponent<Text>().text = Name;
			((Component)base.gameObject.transform.Find("Description")).GetComponent<Text>().text = Description;
			base.gameObject.GetComponent<MenuButton>().submitAction = SubmitAction;
		}
	}
	public class MenuRow : Element, IShadowElement
	{
		public List<Element> Row;

		public float XDelta = 750f;

		public Element[] GetElements()
		{
			return Row.ToArray();
		}

		public Element Find(string Id)
		{
			return Row?.Find((Element e) => e.Id == Id);
		}

		public MenuRow(List<Element> Row, string Id)
			: base(Id)
		{
			this.Row = Row;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			int count = Row.Count;
			IContentLayout layout = c.Layout;
			RegularGridLayout val = (RegularGridLayout)(object)((layout is RegularGridLayout) ? layout : null);
			RelVector2 itemAdvance = val.ItemAdvance;
			List<GameObject> list = new List<GameObject>();
			if (count > 1)
			{
				((RelVector2)(ref itemAdvance)).x = new RelLength(XDelta);
				val.ChangeColumns(count, 0.5f, (RelVector2?)itemAdvance, 0.5f);
			}
			foreach (Element item in Row)
			{
				if (item is IShadowElement)
				{
					((Loggable)Satchel.Instance).LogError("[Satchel] - Cannot create an IShadowElement inside another IShadowElement");
					return new GameObjectRow();
				}
				item.Parent = Parent;
				foreach (GameObject item2 in item.Create(c, Instance, AddToList: false).Row)
				{
					list.Add(item2);
				}
			}
			if (count > 1)
			{
				RelVector2 itemAdvance2 = val.ItemAdvance;
				((RelVector2)(ref itemAdvance2)).x = new RelLength(0f);
				val.ChangeColumns(1, 0.5f - 1f / (2f * (float)count), (RelVector2?)itemAdvance2, 0.5f);
			}
			GameObjectRow gameObjectRow = new GameObjectRow(list);
			gameObjectRow.Parent = this;
			if (AddToList)
			{
				Instance.MenuOrder.Add(gameObjectRow);
			}
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return gameObjectRow;
		}

		public override void Update()
		{
			foreach (Element item in Row)
			{
				item?.UpdateInternal();
			}
		}
	}
	public class StaticPanel : Element
	{
		public Action<GameObject> CreateCustomItem;

		public float Width;

		public StaticPanel(string name, Action<GameObject> createCustomItem, float width = 1000f, string Id = "__UseName")
			: base(Id, name)
		{
			Name = name;
			CreateCustomItem = createCustomItem;
			Width = width;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (CreateCustomItem == null)
			{
				throw new ArgumentNullException("CreateCustomItem", "CreateCustomItem cannot be null");
			}
			GameObject val = default(GameObject);
			StaticContent.AddStaticPanel(c, Name, new RelVector2(new Vector2(Width, 105f)), ref val);
			CreateCustomItem(val);
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(val));
			}
			base.gameObject = val;
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(val);
		}

		public override void Update()
		{
		}
	}
	public class TextPanel : Element
	{
		public float Width;

		public int FontSize;

		public TextFont Font;

		public TextAnchor Anchor;

		public AdditionalTextPanelConfig? Config;

		public TextPanel(string name, float width = 1000f, int fontSize = 35, string Id = "__UseName")
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Font = (TextFont)1;
			Anchor = (TextAnchor)4;
			base..ctor(Id, name);
			Name = name;
			Width = width;
			FontSize = fontSize;
		}

		public override GameObjectRow Create(ContentArea c, Menu Instance, bool AddToList = true)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (Name == null)
			{
				throw new ArgumentNullException("Name", "Name cannot be null");
			}
			Text val = default(Text);
			StaticContent.AddTextPanel(c, Name, new RelVector2(new Vector2(Width, 105f)), new TextPanelConfig
			{
				Anchor = Anchor,
				Font = Font,
				Size = FontSize,
				Text = Name
			}, ref val);
			if (AddToList)
			{
				Instance.MenuOrder.Add(new GameObjectRow(((Component)val).gameObject));
			}
			base.gameObject = ((Component)val).gameObject;
			((IContainer)Parent).OnBuilt += delegate
			{
				OnBuiltInvoke();
			};
			return new GameObjectRow(base.gameObject);
		}

		public override void Update()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected I4, but got Unknown
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			base.gameObject.GetComponent<Text>().text = Name;
			base.gameObject.GetComponent<Text>().fontSize = FontSize;
			base.gameObject.GetComponent<Text>().alignment = Anchor;
			Text component = base.gameObject.GetComponent<Text>();
			TextFont font = Font;
			component.font = (Font)((int)font switch
			{
				0 => MenuResources.TrajanRegular, 
				1 => MenuResources.TrajanBold, 
				2 => MenuResources.Perpetua, 
				_ => MenuResources.TrajanRegular, 
			});
			base.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 105f);
			Config?.SetAdditionalTextProperties(base.gameObject.GetComponent<Text>());
		}
	}
	public static class Blueprints
	{
		public static void AddConfirmDialog(this Menu initialMenu, Menu DialogMenu)
		{
			initialMenu.CancelAction = delegate
			{
				initialMenu.ShowDialog(DialogMenu);
			};
		}

		public static void ShowDialog(this Menu initialMenu, Menu DialogMenu)
		{
			DialogMenu.returnScreen = initialMenu.GetCachedMenuScreen(initialMenu.returnScreen);
			Utils.GoToMenuScreen(DialogMenu.GetCachedMenuScreen(DialogMenu.returnScreen));
		}

		public static Menu CreateDialogMenu(string title, string subTitle, string[] Options, Action<string> OnButtonPress, int optionsPerRow = 2)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			Menu menu = new Menu("");
			if (title != string.Empty)
			{
				menu.AddElement(new TextPanel(title, 1000f, 55)
				{
					Font = (TextFont)1
				});
			}
			if (subTitle != string.Empty)
			{
				menu.AddElement(new TextPanel(subTitle, 1000f, 39)
				{
					Font = (TextFont)1
				});
			}
			for (int i = 0; i < Options.Length; i += optionsPerRow)
			{
				List<Element> list = new List<Element>();
				for (int j = 0; j < optionsPerRow && i + j < Options.Length; j++)
				{
					string option = Options[i + j];
					list.Add(new MenuButton(option, "", delegate
					{
						OnButtonPress(option);
					}));
				}
				menu.AddElement(new MenuRow(list, $"Options{i}")
				{
					XDelta = ((list.Count > 2) ? 250 : 500)
				});
			}
			menu.OnBuilt += delegate
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				RectTransform component = ((Component)menu.menuScreen).gameObject.GetComponent<RectTransform>();
				component.sizeDelta = new Vector2(0f, 250f);
				component.anchoredPosition = new Vector2(0f, -200f);
				GameObject val = ((Component)menu.menuScreen).gameObject.Find("BackButton");
				if ((Object)(object)val != (Object)null)
				{
					Object.Destroy((Object)(object)val);
				}
			};
			return menu;
		}

		public static Menu AddConfirmDialog(this Menu initialMenu, string title, string subTitle, string[] Options, Action<string> OnButtonPress, int optionsPerRow = 2)
		{
			Menu menu = CreateDialogMenu(title, subTitle, Options, OnButtonPress, optionsPerRow);
			initialMenu.AddConfirmDialog(menu);
			menu.GetMenuScreen(initialMenu.menuScreen);
			return menu;
		}

		public static InputField FloatInputField(string name, Action<float> _storeValue, Func<float> _loadValue, float _defaultValue = 0f, string _placeholder = "", int _characterLimit = 7, InputFieldConfig? _config = null, string Id = "__Usename")
		{
			InputFieldConfig value = _config ?? InputFieldConfig.DefaultFloatNumber;
			return new InputField(_storeValue: delegate(string s)
			{
				if (float.TryParse(s, out var result))
				{
					_storeValue(result);
				}
				else if (s != string.Empty)
				{
					((Loggable)Satchel.Instance).LogWarn("Inputted value " + s + " is not a valid float in " + name + " input field");
				}
			}, _loadValue: () => _loadValue().ToString(), name: name, _placeholder: _placeholder, _characterLimit: _characterLimit, _config: value, Id: Id);
		}

		public static HorizontalOption GenericHorizontalOption<T>(string name, string description, T[] values, Action<T> applySetting, Func<T> loadSetting, string Id = "__UseName") where T : IFormattable
		{
			if (Id == "__UseName")
			{
				Id = name;
			}
			return new HorizontalOption(name, description, values.Select((T i) => i.ToString()).ToArray(), delegate(int i)
			{
				applySetting(values[i]);
			}, delegate
			{
				T val = loadSetting();
				if (!values.Contains(val))
				{
					throw new IndexOutOfRangeException($"Your provided array for the horizontal option: {name} did not include {val} in it");
				}
				return Array.IndexOf(values, val, 0, values.Length);
			}, Id);
		}

		public static MenuScreen GetCachedMenuScreen(this Menu MenuRef, MenuScreen returnScreen)
		{
			if ((Object)(object)MenuRef.menuScreen == (Object)null)
			{
				return MenuRef.GetMenuScreen(returnScreen);
			}
			return MenuRef.menuScreen;
		}

		public static HorizontalOption HorizontalBoolOption(string name, string description, Action<bool> applySetting, Func<bool> loadSetting, [CanBeNull] string _true = null, [CanBeNull] string _false = null, string Id = "__UseName")
		{
			return new HorizontalOption(name, description, new string[2]
			{
				_true ?? Language.Get("MOH_ON", "MainMenu"),
				_false ?? Language.Get("MOH_OFF", "MainMenu")
			}, delegate(int i)
			{
				applySetting(i == 0);
			}, () => (!loadSetting()) ? 1 : 0, Id);
		}

		public static InputField IntInputField(string name, Action<int> _storeValue, Func<int> _loadValue, int _defaultValue = 0, string _placeholder = "", int _characterLimit = 5, InputFieldConfig? _config = null, string Id = "__Usename")
		{
			InputFieldConfig value = _config ?? InputFieldConfig.DefaultWholeNumber;
			return new InputField(_storeValue: delegate(string s)
			{
				if (int.TryParse(s, out var result))
				{
					_storeValue(result);
				}
				else if (s != string.Empty)
				{
					((Loggable)Satchel.Instance).LogWarn("Inputted value " + s + " is not a valid integer in " + name + " input field");
				}
			}, _loadValue: () => _loadValue().ToString(), name: name, _placeholder: _placeholder, _characterLimit: _characterLimit, _config: value, Id: Id);
		}

		public static MenuRow KeyAndButtonBind(string name, PlayerAction keyBindAction, PlayerAction buttonBindAction, string Id = "__UseName")
		{
			if (Id == "__UseName")
			{
				Id = name;
			}
			return new MenuRow(new List<Element>
			{
				new KeyBind(name, keyBindAction, Id + "key")
				{
					SelectableArea = SelectableArea.ButtonOnly
				},
				new ButtonBind("", buttonBindAction, Id + "button")
				{
					SelectableArea = SelectableArea.ButtonOnly
				}
			}, Id)
			{
				XDelta = 200f
			};
		}

		public static MenuRow KeyAndButtonBind(string name, KeyAndButtonActionSet keyAndButtonActionSet, string Id = "__UseName")
		{
			return KeyAndButtonBind(name, keyAndButtonActionSet.KeyBind, keyAndButtonActionSet.ButtonBind, Id);
		}

		public static HorizontalOption CreateToggle(this ModToggleDelegates toggleDelegates, string name, string description)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return toggleDelegates.CreateToggle(name, description, Language.Get("MOH_ON", "MainMenu"), Language.Get("MOH_OFF", "MainMenu"));
		}

		public static HorizontalOption CreateToggle(this ModToggleDelegates toggleDelegates, string name, string description, string on, string off)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return new HorizontalOption(name, description, new string[2] { on, off }, delegate(int i)
			{
				toggleDelegates.SetModEnabled(i == 0);
			}, () => (!toggleDelegates.GetModEnabled()) ? 1 : 0, "ModToggle");
		}

		public static MenuButton NavigateToMenu(string name, string description, Func<MenuScreen> getScreen)
		{
			return new MenuButton(name, description, delegate
			{
				Utils.GoToMenuScreen(getScreen());
			}, proceed: true);
		}

		public static MenuButton ToggleButton(string name, string description, Action<bool> applySetting, Func<bool> loadSetting, string Id = "__UseName")
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (Id == "__UseName")
			{
				Id = name;
			}
			Color ON = Color.white;
			Color OFF = Color.grey;
			MenuButton toggleButton = new MenuButton(name, description, delegate(MenuButton button)
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				bool flag = !loadSetting();
				((Graphic)((Component)((Component)button).transform.Find("Label")).GetComponent<Text>()).color = (flag ? ON : OFF);
				applySetting(flag);
			}, proceed: false, Id);
			toggleButton.OnBuilt += delegate
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				((Graphic)((Component)toggleButton.gameObject.transform.Find("Label")).GetComponent<Text>()).color = (loadSetting() ? ON : OFF);
			};
			return toggleButton;
		}

		public static void UpdateVisibility(this Menu menuRef, bool isVisible, string[] elements)
		{
			foreach (string elementId in elements)
			{
				Element element = menuRef.Find(elementId);
				if (element != null)
				{
					element.isVisible = isVisible;
				}
			}
			menuRef.Update();
		}
	}
	public class Menu : MenuElement, IContainer
	{
		private readonly List<Element> Elements;

		private readonly Dictionary<string, Element> ElementDict;

		private int Columns;

		private int Index;

		private Menu Instance;

		private RelVector2 ItemAdvance;

		private AnchoredPosition Start;

		private GridNavGraph NavigationGraph;

		public List<GameObjectRow> MenuOrder;

		public MenuScreen menuScreen;

		public MenuScreen returnScreen;

		public Action CancelAction;

		private Vector2Int IndexPos
		{
			get
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				return new Vector2Int(Index % Columns, Index / Columns);
			}
		}

		public event EventHandler<ContainerBuiltEventArgs> OnBuilt;

		public event EventHandler<ReflowEventArgs> OnReflow;

		public void AddElement(Element elem)
		{
			Elements.Add(elem);
			ElementDict[elem.Id] = elem;
		}

		public Menu(string name, Element[] elements)
			: this(name)
		{
			foreach (Element elem in elements)
			{
				AddElement(elem);
			}
		}

		public Menu(string name)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			Elements = new List<Element>();
			ElementDict = new Dictionary<string, Element>();
			Columns = 1;
			ItemAdvance = new RelVector2(new Vector2(0f, -105f));
			Start = new AnchoredPosition
			{
				ChildAnchor = new Vector2(0.5f, 1f),
				ParentAnchor = new Vector2(0.5f, 1f),
				Offset = default(Vector2)
			};
			MenuOrder = new List<GameObjectRow>();
			base..ctor();
			Parent = null;
			base.gameObject = null;
			Name = name;
			Instance = this;
			MenuOrder.Clear();
			ResetPositioners();
			UIManager.ShowMenu += new hook_ShowMenu(ShowMenu);
		}

		private IEnumerator ShowMenu(orig_ShowMenu orig, UIManager self, MenuScreen menu)
		{
			if ((Object)(object)menu == (Object)(object)menuScreen)
			{
				menu.screenCanvasGroup.alpha = 0f;
				((Component)menu.screenCanvasGroup).gameObject.SetActive(true);
				UpdateInternal();
				((Component)menu.screenCanvasGroup).gameObject.SetActive(false);
			}
			yield return orig.Invoke(self, menu);
		}

		public Element Find(string ElementId)
		{
			if (ElementDict.TryGetValue(ElementId, out var value))
			{
				return value;
			}
			((Loggable)Satchel.Instance).LogError("No such Element with id " + ElementId);
			return null;
		}

		public MenuScreen GetMenuScreen(MenuScreen _returnScreen)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			returnScreen = _returnScreen;
			CancelAction = delegate
			{
				Utils.GoToMenuScreen(returnScreen);
			};
			MenuBuilder val = Utils.CreateMenuBuilder(Name, out NavigationGraph);
			MenuButton backButton = null;
			val.AddContent((IContentLayout)(object)default(NullContentLayout), (Action<ContentArea>)delegate(ContentArea c)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				ScrollbarConfig val2 = new ScrollbarConfig
				{
					CancelAction = delegate
					{
						CancelAction();
					}
				};
				Navigation navigation = default(Navigation);
				((Navigation)(ref navigation)).mode = (Mode)4;
				((Navigation)(ref navigation)).selectOnUp = (Selectable)(object)backButton;
				((Navigation)(ref navigation)).selectOnDown = (Selectable)(object)backButton;
				val2.Navigation = navigation;
				val2.Position = new AnchoredPosition
				{
					ChildAnchor = new Vector2(0f, 1f),
					ParentAnchor = new Vector2(1f, 1f),
					Offset = new Vector2(-310f, 0f)
				};
				ScrollPaneContent.AddScrollPaneContent(c, val2, new RelLength((float)Elements.Count() * 105f), (IContentLayout)(object)RegularGridLayout.CreateVerticalLayout(105f, default(Vector2)), (Action<ContentArea>)delegate(ContentArea d)
				{
					AddModMenuContent(Elements, d);
				});
			});
			val.AddBackButton(Instance, out backButton);
			menuScreen = val.Build();
			TriggerBuiltEvent();
			return menuScreen;
		}

		private void ApplyElementVisibility(Element elem)
		{
			if ((Object)(object)elem.gameObject == (Object)null)
			{
				if (elem is IShadowElement shadowElement)
				{
					Element[] elements = shadowElement.GetElements();
					foreach (Element elem2 in elements)
					{
						ApplyElementVisibility(elem2);
					}
				}
				else
				{
					((Loggable)Satchel.Instance).LogError($"No GameObject for {elem.GetType()} {elem.Name}");
				}
			}
			else
			{
				elem.gameObject.SetActive(elem.isVisible);
			}
		}

		private void AddModMenuContent(List<Element> AllMenuOptions, ContentArea c)
		{
			foreach (Element AllMenuOption in AllMenuOptions)
			{
				AllMenuOption.Parent = this;
				AllMenuOption.Create(c, Instance);
				ApplyElementVisibility(AllMenuOption);
			}
		}

		private void ResetPositioners()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			ItemAdvance = new RelVector2(new Vector2(0f, -105f));
			Start = new AnchoredPosition
			{
				ChildAnchor = new Vector2(0.5f, 1f),
				ParentAnchor = new Vector2(0.5f, 1f),
				Offset = default(Vector2)
			};
			Index = 0;
		}

		private void ModifyNext(GameObject go)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (go.gameObject.activeInHierarchy)
			{
				AnchoredPosition val = Start + ItemAdvance * Vector2Int.op_Implicit(IndexPos);
				((AnchoredPosition)(ref val)).Reposition(go.gameObject.GetComponent<RectTransform>());
				Index++;
			}
		}

		public void TriggerBuiltEvent()
		{
			OnBuilt?.Invoke(this, new ContainerBuiltEventArgs
			{
				Target = this
			});
		}

		public void Reflow(bool silent = false)
		{
			Elements.ForEach(ApplyElementVisibility);
			Reorder();
			if (!silent)
			{
				OnReflow?.Invoke(this, new ReflowEventArgs
				{
					Target = this
				});
			}
		}

		public void Reorder()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			ReflectionHelper.SetField<GridNavGraph, List<List<Selectable>>>(NavigationGraph, "grid", new List<List<Selectable>>
			{
				new List<Selectable>()
			});
			ReflectionHelper.SetProperty<GridNavGraph, int>(NavigationGraph, "Columns", 1);
			foreach (GameObjectRow item in MenuOrder)
			{
				int num = item.ActiveCount();
				RelVector2 itemAdvance = ItemAdvance;
				if (num > 1)
				{
					float num2 = ((item.Parent != null) ? ((MenuRow)item.Parent).XDelta : 750f);
					((RelVector2)(ref itemAdvance)).x = new RelLength(num2);
					ChangeColumns(num, 0.5f, itemAdvance);
				}
				foreach (GameObject item2 in item.Row)
				{
					if ((Object)(object)item2 != (Object)null && item2.activeSelf)
					{
						ModifyNext(item2);
						Selectable component = item2.GetComponent<Selectable>();
						component = (((Object)(object)component == (Object)null) ? item2.GetComponentInChildren<Selectable>() : component);
						if ((Object)(object)component != (Object)null)
						{
							NavigationGraph.AddNavigationNode(component);
						}
					}
				}
				if (num > 1)
				{
					RelVector2 itemAdvance2 = ItemAdvance;
					((RelVector2)(ref itemAdvance2)).x = new RelLength(0f);
					ChangeColumns(1, 0.5f - 1f / (2f * (float)num), itemAdvance2);
				}
			}
			try
			{
				((Component)menuScreen).gameObject.AddComponent<AutoSelector>().Start = NavigationGraph.BuildNavigation();
			}
			catch (Exception ex)
			{
				((Loggable)Satchel.Instance).LogError((object)ex);
			}
			ResetPositioners();
		}

		private void ChangeColumns(int columns, float originalAnchor = 0.5f, RelVector2? newSize = null, float newAnchor = 0.5f)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			RelVector2 itemAdvance = (RelVector2)(((??)newSize) ?? ItemAdvance);
			RelVector2 itemAdvance2 = ItemAdvance;
			RelLength y = ((RelVector2)(ref itemAdvance2)).y;
			Vector2Int indexPos = IndexPos;
			RelLength val = y * (float)((Vector2Int)(ref indexPos)).y;
			itemAdvance2 = ItemAdvance;
			RelLength val2 = ((RelVector2)(ref itemAdvance2)).x * (float)Columns * originalAnchor - ((RelVector2)(ref itemAdvance)).x * (float)columns * newAnchor;
			RelLength val3 = Start.ChildAnchor.x * ((RelVector2)(ref itemAdvance)).x + val2;
			Index = 0;
			Columns = columns;
			Start += new RelVector2(val3, val);
			ItemAdvance = itemAdvance;
		}

		public static MenuScreen Create(string Title, MenuScreen modListMenu, Element[] MenuOptions, out Menu betterMenuMod)
		{
			betterMenuMod = new Menu(Title, MenuOptions);
			return betterMenuMod.GetMenuScreen(modListMenu);
		}

		public static MenuScreen Create(string Title, MenuScreen modListMenu, Element[] MenuOptions)
		{
			return new Menu(Title, MenuOptions).GetMenuScreen(modListMenu);
		}

		public override void Update()
		{
			((Component)menuScreen).gameObject.Find("Title").GetComponent<Text>().text = Name;
			Elements.ForEach(delegate(Element elem)
			{
				elem.Update();
			});
			Reflow();
		}
	}
	public static class Utils
	{
		public static MenuBuilder CreateMenuBuilder(string Title)
		{
			GridNavGraph navGraph;
			return CreateMenuBuilder(Title, out navGraph);
		}

		public static MenuBuilder CreateMenuBuilder(string Title, out GridNavGraph navGraph)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			navGraph = new GridNavGraph(1);
			return new MenuBuilder(((Component)UIManager.instance.UICanvas).gameObject, Title).CreateTitle(Title, MenuTitleStyle.vanillaStyle).CreateContentPane(RectTransformData.FromSizeAndPos(new RelVector2(new Vector2(1920f, 903f)), new AnchoredPosition(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -60f)))).CreateControlPane(RectTransformData.FromSizeAndPos(new RelVector2(new Vector2(1920f, 259f)), new AnchoredPosition(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -502f))))
				.SetDefaultNavGraph((INavigationGraph)(object)navGraph);
		}

		public static MenuBuilder AddBackButton(this MenuBuilder builder, MenuScreen returnScreen, out MenuButton backButton)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			MenuButton BackButton = null;
			builder.AddControls((IContentLayout)new SingleContentLayout(new AnchoredPosition(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -64f))), (Action<ContentArea>)delegate(ContentArea c)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				MenuButtonContent.AddMenuButton(c, "BackButton", new MenuButtonConfig
				{
					Label = Language.Get("NAV_BACK", "MainMenu"),
					CancelAction = delegate
					{
						GoToMenuScreen(returnScreen);
					},
					SubmitAction = delegate
					{
						GoToMenuScreen(returnScreen);
					},
					Style = MenuButtonStyle.VanillaStyle,
					Proceed = true
				}, ref BackButton);
			});
			backButton = BackButton;
			return builder;
		}

		public static MenuBuilder AddBackButton(this MenuBuilder builder, MenuScreen returnScreen)
		{
			MenuButton backButton;
			return builder.AddBackButton(returnScreen, out backButton);
		}

		public static MenuBuilder AddControlButton(this MenuBuilder builder, string name, Vector2 offset, Action<MenuSelectable> cancelAction, Action<MenuButton> submitAction)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			return builder.AddControls((IContentLayout)new SingleContentLayout(new AnchoredPosition(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), offset)), (Action<ContentArea>)delegate(ContentArea c)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				MenuButtonContent.AddMenuButton(c, name, new MenuButtonConfig
				{
					Label = name,
					CancelAction = cancelAction,
					SubmitAction = submitAction,
					Style = MenuButtonStyle.VanillaStyle,
					Proceed = true
				});
			});
		}

		public static MenuBuilder AddBackButton(this MenuBuilder builder, Menu menuRef, out MenuButton backButton)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			MenuButton BackButton = null;
			builder.AddControls((IContentLayout)new SingleContentLayout(new AnchoredPosition(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -64f))), (Action<ContentArea>)delegate(ContentArea c)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				MenuButtonContent.AddMenuButton(c, "BackButton", new MenuButtonConfig
				{
					Label = Language.Get("NAV_BACK", "MainMenu"),
					CancelAction = delegate
					{
						menuRef.CancelAction();
					},
					SubmitAction = delegate
					{
						menuRef.CancelAction();
					},
					Style = MenuButtonStyle.VanillaStyle,
					Proceed = true
				}, ref BackButton);
			});
			backButton = BackButton;
			return builder;
		}

		public static void GoToMenuScreen(MenuScreen menuScreen)
		{
			UIManager.instance.UIGoToDynamicMenu(menuScreen);
		}

		public static Animator CreateRegularMenuCursor(GameObject parent, bool leftSide)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject("Cursor" + (leftSide ? "Left" : "Right"));
			val.transform.SetParent(parent.transform, false);
			val.AddComponent<CanvasRenderer>();
			RectTransform obj = val.AddComponent<RectTransform>();
			Vector2 sizeDelta = default(Vector2);
			Vector2 pivot = default(Vector2);
			Vector2 anchorMin = default(Vector2);
			Vector2 anchorMax = default(Vector2);
			Vector2 anchoredPosition = default(Vector2);
			Vector3 localScale = default(Vector3);
			if (leftSide)
			{
				((Vector2)(ref sizeDelta))..ctor(164f, 119f);
				((Vector2)(ref pivot))..ctor(0.5f, 0.5f);
				((Vector2)(ref anchorMin))..ctor(0f, 0.5f);
				((Vector2)(ref anchorMax))..ctor(0f, 0.5f);
				((Vector2)(ref anchoredPosition))..ctor(-65f, 0f);
				((Vector3)(ref localScale))..ctor(0.4f, 0.4f, 0.4f);
			}
			else
			{
				((Vector2)(ref sizeDelta))..ctor(164f, 119f);
				((Vector2)(ref pivot))..ctor(0.5f, 0.5f);
				((Vector2)(ref anchorMin))..ctor(1f, 0.5f);
				((Vector2)(ref anchorMax))..ctor(1f, 0.5f);
				((Vector2)(ref anchoredPosition))..ctor(65f, 0f);
				((Vector3)(ref localScale))..ctor(-0.4f, 0.4f, 0.4f);
			}
			obj.sizeDelta = sizeDelta;
			obj.pivot = pivot;
			obj.anchorMin = anchorMin;
			obj.anchorMax = anchorMax;
			obj.anchoredPosition = anchoredPosition;
			((Transform)obj).localScale = localScale;
			Animator obj2 = val.AddComponent<Animator>();
			obj2.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
			obj2.updateMode = (AnimatorUpdateMode)2;
			obj2.applyRootMotion = false;
			val.AddComponent<Image>();
			return obj2;
		}
	}
}
namespace Satchel.BetterMenus.Config
{
	public struct InputFieldConfig
	{
		public int fontSize;

		public float inputBoxWidth;

		public ContentType contentType;

		public InputFieldSaveType saveType;

		public CharacterValidation characterValidation;

		public static readonly InputFieldConfig DefaultText;

		public static readonly InputFieldConfig DefaultWholeNumber;

		public static readonly InputFieldConfig DefaultFloatNumber;

		static InputFieldConfig()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			DefaultText = new InputFieldConfig
			{
				fontSize = 46,
				inputBoxWidth = 300f,
				contentType = (ContentType)0,
				characterValidation = (CharacterValidation)0,
				saveType = InputFieldSaveType.ValueChanged
			};
			DefaultWholeNumber = new InputFieldConfig
			{
				fontSize = 46,
				inputBoxWidth = 150f,
				contentType = (ContentType)2,
				characterValidation = (CharacterValidation)1,
				saveType = InputFieldSaveType.EditEnd
			};
			DefaultFloatNumber = new InputFieldConfig
			{
				fontSize = 46,
				inputBoxWidth = 210f,
				contentType = (ContentType)3,
				characterValidation = (CharacterValidation)2,
				saveType = InputFieldSaveType.EditEnd
			};
		}
	}
	public struct AdditionalTextPanelConfig
	{
		public FontStyle? fontStyle;

		public HorizontalWrapMode? horizontalOverflow;

		public float? lineSpacing;

		public Color? color;

		public void SetAdditionalTextProperties(Text text)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (fontStyle.HasValue)
			{
				text.fontStyle = fontStyle.Value;
			}
			if (horizontalOverflow.HasValue)
			{
				text.horizontalOverflow = horizontalOverflow.Value;
			}
			if (lineSpacing.HasValue)
			{
				text.lineSpacing = lineSpacing.Value;
			}
			if (color.HasValue)
			{
				((Graphic)text).color = color.Value;
			}
		}
	}
	public enum SelectableArea
	{
		Full,
		ButtonOnly
	}
	public struct KeybindConfig
	{
		public string Label;

		public KeybindStyle? Style;

		public Action<MappableKey> CancelAction;

		public SelectableArea? selectableArea;
	}
	public struct ButtonBindConfig
	{
		public string Label;

		public KeybindStyle? Style;

		public Action<MappableControllerButton> CancelAction;

		public SelectableArea? selectableArea;
	}
	public struct KeybindStyle
	{
		public static readonly KeybindStyle VanillaStyle = new KeybindStyle
		{
			LabelTextSize = 37
		};

		public int LabelTextSize;
	}
	public enum InputFieldSaveType
	{
		ValueChanged,
		EditEnd
	}
}
namespace Satchel.BetterMenus.Attributes
{
	public static class BuildMenuFromAttributes
	{
		public static Menu Build<T>(string name, T settings)
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			Menu menu = new Menu(name);
			foreach (MemberInfo item in from member in typeof(T).GetMembers()
				where member.GetCustomAttribute<BetterMenusIgnore>() == null && member.GetCustomAttribute<ElementAttribute>() != null
				orderby member.GetCustomAttribute<ElementAttribute>().Order
				select member)
			{
				ElementAttribute customAttribute = item.GetCustomAttribute<ElementAttribute>();
				if (customAttribute != null)
				{
					if (customAttribute.VerifyCorrectFieldType(item))
					{
						customAttribute.CreateElement(item, settings).ToList().ForEach(menu.AddElement);
						continue;
					}
					menu.AddElement(new TextPanel("the type of " + item.Name + " is incorrect")
					{
						Config = new AdditionalTextPanelConfig
						{
							color = Color.red,
							fontStyle = (FontStyle)2
						}
					});
				}
			}
			return menu;
		}
	}
	public abstract class ElementAttribute : Attribute
	{
		public string Name;

		public int Order;

		protected const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		protected ElementAttribute(string name, int order)
		{
			Name = name;
			Order = order;
		}

		public abstract bool VerifyCorrectFieldType(MemberInfo memberInfo);

		public abstract Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings);

		protected static bool CheckFieldOrPropertyType(MemberInfo memberInfo, Type type)
		{
			if (!(memberInfo is FieldInfo fieldInfo) || !(fieldInfo.FieldType == type))
			{
				if (memberInfo is PropertyInfo propertyInfo)
				{
					return propertyInfo.PropertyType == type;
				}
				return false;
			}
			return true;
		}

		protected static void SetValue<Settings>(MemberInfo member, Settings settings, object value)
		{
			if (!(member is FieldInfo fieldInfo))
			{
				if (!(member is PropertyInfo propertyInfo))
				{
					throw new InvalidOperationException("You cannot call set value on members that arent fields or properties");
				}
				propertyInfo.SetValue(settings, value);
			}
			else
			{
				fieldInfo.SetValue(settings, value);
			}
		}

		protected static Type GetMemberType(MemberInfo memberInfo)
		{
			if (!(memberInfo is FieldInfo fieldInfo))
			{
				return (memberInfo as PropertyInfo).PropertyType;
			}
			return fieldInfo.FieldType;
		}

		protected static object GetValue<Settings>(MemberInfo member, Settings settings)
		{
			if (!(member is FieldInfo fieldInfo))
			{
				if (member is PropertyInfo propertyInfo)
				{
					return propertyInfo.GetValue(settings);
				}
				throw new InvalidOperationException("You cannot call get value on members that arent fields or properties");
			}
			return fieldInfo.GetValue(settings);
		}
	}
	public class BetterMenusIgnore : Attribute
	{
	}
	public abstract class GenericHorizontalOptionAttribute<T> : ElementAttribute
	{
		public List<string> Options;

		public string Description;

		public GenericHorizontalOptionAttribute(string name, string description, string[] options, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
			Description = description;
			Options = options.ToList();
		}

		public GenericHorizontalOptionAttribute(string name, string description, T[] options, [CallerLineNumber] int order = 0)
			: this(name, description, options.Select((T x) => x.ToString()).ToArray(), order)
		{
		}

		public abstract T FromStringConvertor(string value);

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo || memberInfo is PropertyInfo)
			{
				return true;
			}
			return false;
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1]
			{
				new HorizontalOption(Name, Description, Options.ToArray(), delegate(int i)
				{
					ElementAttribute.SetValue(memberInfo, settings, FromStringConvertor(Options[i]));
				}, delegate
				{
					T value = (T)ElementAttribute.GetValue(memberInfo, settings);
					return Options.FindIndex((string x) => EqualityComparer<T>.Default.Equals(FromStringConvertor(x), value));
				})
			};
		}
	}
	public class HorizontalStringOptionAttribute : GenericHorizontalOptionAttribute<string>
	{
		public HorizontalStringOptionAttribute(string name, string description, string[] options, [CallerLineNumber] int order = 0)
			: base(name, description, options, order)
		{
		}

		public override string FromStringConvertor(string value)
		{
			return value;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(string));
		}
	}
	public class HorizontalEnumOptionAttribute<T> : GenericHorizontalOptionAttribute<T> where T : Enum
	{
		public HorizontalEnumOptionAttribute(string name, string description, [CallerLineNumber] int order = 0)
			: base(name, description, Enum.GetNames(typeof(T)), order)
		{
		}

		public override T FromStringConvertor(string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			if (!(memberInfo is FieldInfo fieldInfo) || !fieldInfo.FieldType.IsSubclassOf(typeof(Enum)))
			{
				if (memberInfo is PropertyInfo propertyInfo)
				{
					return propertyInfo.PropertyType.IsSubclassOf(typeof(Enum));
				}
				return false;
			}
			return true;
		}
	}
	public class HorizontalBoolOptionAttribute : GenericHorizontalOptionAttribute<bool>
	{
		public HorizontalBoolOptionAttribute(string name, string description, string _true = "True", string _false = "False", [CallerLineNumber] int order = 0)
			: base(name, description, new string[2] { _true, _false }, order)
		{
		}

		public override bool FromStringConvertor(string value)
		{
			return value == Options[0];
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(bool));
		}
	}
	public class HorizontalIntOptionAttribute : GenericHorizontalOptionAttribute<int>
	{
		public HorizontalIntOptionAttribute(string name, string description, int[] options, [CallerLineNumber] int order = 0)
			: base(name, description, options, order)
		{
		}

		public HorizontalIntOptionAttribute(string name, string description, int start, int stop, int step = 1, [CallerLineNumber] int order = 0)
			: base(name, description, GetOptions(start, stop, step), order)
		{
		}

		public override int FromStringConvertor(string value)
		{
			return int.Parse(value);
		}

		private static int[] GetOptions(int start, int stop, int step)
		{
			List<int> list = new List<int>();
			for (int i = start; i < stop; i += step)
			{
				list.Add(i);
			}
			return list.ToArray();
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(int));
		}
	}
	public class HorizontalFloatOptionAttribute : GenericHorizontalOptionAttribute<float>
	{
		public HorizontalFloatOptionAttribute(string name, string description, float[] options, [CallerLineNumber] int order = 0)
			: base(name, description, options, order)
		{
		}

		public HorizontalFloatOptionAttribute(string name, string description, float start, float stop, float step = 1f, [CallerLineNumber] int order = 0)
			: base(name, description, GetOptions(start, stop, step), order)
		{
		}

		public override float FromStringConvertor(string value)
		{
			return float.Parse(value);
		}

		private static float[] GetOptions(float start, float stop, float step)
		{
			List<float> list = new List<float>();
			decimal num = (decimal)stop;
			decimal num2 = (decimal)step;
			for (decimal num3 = (decimal)start; num3 < num; num3 += num2)
			{
				list.Add(decimal.ToSingle(num3));
			}
			list.Add(stop);
			return list.ToArray();
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(float));
		}
	}
	public class StringInputFieldAttribute : ElementAttribute
	{
		public string PlaceHolder;

		public int CharacterLimit;

		public StringInputFieldAttribute(string name, string placeHolder = "", int characterLimit = 10, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
			PlaceHolder = placeHolder;
			CharacterLimit = characterLimit;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(string));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1]
			{
				new InputField(Name, delegate(string s)
				{
					ElementAttribute.SetValue(memberInfo, settings, s);
				}, () => (string)ElementAttribute.GetValue(memberInfo, settings), PlaceHolder, CharacterLimit)
			};
		}
	}
	public class IntInputFieldAttribute : ElementAttribute
	{
		public int DefaultValue;

		public string PlaceHolder;

		public int CharacterLimit;

		public IntInputFieldAttribute(string name, int defaultValue = 0, string placeHolder = "", int characterLimit = 5, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
			DefaultValue = defaultValue;
			PlaceHolder = placeHolder;
			CharacterLimit = characterLimit;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(int));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1] { Blueprints.IntInputField(Name, delegate(int i)
			{
				ElementAttribute.SetValue(memberInfo, settings, i);
			}, () => (int)ElementAttribute.GetValue(memberInfo, settings), DefaultValue, PlaceHolder, CharacterLimit) };
		}
	}
	public class FloatInputFieldAttribute : ElementAttribute
	{
		public float DefaultValue;

		public string PlaceHolder;

		public int CharacterLimit;

		public FloatInputFieldAttribute(string name, float defaultValue = 0f, string placeHolder = "", int characterLimit = 7, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
			DefaultValue = defaultValue;
			PlaceHolder = placeHolder;
			CharacterLimit = characterLimit;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(float));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1] { Blueprints.FloatInputField(Name, delegate(float f)
			{
				ElementAttribute.SetValue(memberInfo, settings, f);
			}, () => (float)ElementAttribute.GetValue(memberInfo, settings), DefaultValue, PlaceHolder, CharacterLimit) };
		}
	}
	public class PlayerActionSetAttribute : ElementAttribute
	{
		public PlayerActionSetAttribute([CallerLineNumber] int order = 0)
			: base("", order)
		{
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			if (!(memberInfo is FieldInfo fieldInfo) || !fieldInfo.FieldType.IsSubclassOf(typeof(PlayerActionSet)))
			{
				if (memberInfo is PropertyInfo propertyInfo)
				{
					return propertyInfo.PropertyType.IsSubclassOf(typeof(PlayerActionSet));
				}
				return false;
			}
			return true;
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return (from bindField in ElementAttribute.GetMemberType(memberInfo).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				where bindField.GetCustomAttribute<BindAttribute>() != null
				orderby bindField.GetCustomAttribute<BindAttribute>().Order
				select bindField.GetCustomAttribute<BindAttribute>().CreateBind(bindField, ElementAttribute.GetValue(memberInfo, settings))).ToArray();
		}
	}
	public abstract class BindAttribute : ElementAttribute
	{
		public BindAttribute(string name, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(PlayerAction));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return Array.Empty<Element>();
		}

		public abstract Element CreateBind(MemberInfo memberInfo, object playerActionSet);
	}
	public class KeyBindAttribute : BindAttribute
	{
		public KeyBindAttribute(string name, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
		}

		public override Element CreateBind(MemberInfo memberInfo, object playerActionSet)
		{
			string name = Name;
			object value = ElementAttribute.GetValue(memberInfo, playerActionSet);
			return new KeyBind(name, (PlayerAction)((value is PlayerAction) ? value : null));
		}
	}
	public class ButtonBindAttribute : BindAttribute
	{
		public ButtonBindAttribute(string name, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
		}

		public override Element CreateBind(MemberInfo memberInfo, object playerActionSet)
		{
			string name = Name;
			object value = ElementAttribute.GetValue(memberInfo, playerActionSet);
			return new ButtonBind(name, (PlayerAction)((value is PlayerAction) ? value : null));
		}
	}
	public class KeyAndButtonBindAttribute : ElementAttribute
	{
		public KeyAndButtonBindAttribute(string name, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(KeyAndButtonActionSet));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1] { Blueprints.KeyAndButtonBind(Name, (KeyAndButtonActionSet)ElementAttribute.GetValue(memberInfo, settings)) };
		}
	}
	public class MenuButtonAttribute : ElementAttribute
	{
		public string Description;

		public MenuButtonAttribute(string name, string description, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
			Description = description;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			if (memberInfo is MethodInfo methodInfo && methodInfo.GetParameters().Length == 0 && !methodInfo.IsGenericMethod)
			{
				return !methodInfo.IsStatic;
			}
			return false;
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			MethodInfo methodInfo = memberInfo as MethodInfo;
			return new Element[1]
			{
				new MenuButton(Name, Description, delegate
				{
					methodInfo.Invoke(settings, null);
				})
			};
		}
	}
	public class FloatSliderAttribute : ElementAttribute
	{
		public float MinValue;

		public float MaxValue;

		public FloatSliderAttribute(string name, float minValue, float maxValue, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(float));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1]
			{
				new CustomSlider(Name, delegate(float f)
				{
					ElementAttribute.SetValue(memberInfo, settings, f);
				}, () => (float)ElementAttribute.GetValue(memberInfo, settings), MinValue, MaxValue)
			};
		}
	}
	public class IntSliderAttribute : ElementAttribute
	{
		public int MinValue;

		public int MaxValue;

		public IntSliderAttribute(string name, int minValue, int maxValue, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(int));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1]
			{
				new CustomSlider(Name, delegate(float f)
				{
					ElementAttribute.SetValue(memberInfo, settings, (int)f);
				}, () => (int)ElementAttribute.GetValue(memberInfo, settings), MinValue, MaxValue, wholeNumbers: true)
			};
		}
	}
	public class StaticPanelAttribute : ElementAttribute
	{
		public StaticPanelAttribute(string name, [CallerLineNumber] int order = 0)
			: base(name, order)
		{
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			if (memberInfo is MethodInfo methodInfo && methodInfo.GetParameters().Length == 1 && methodInfo.GetParameters()[0].ParameterType == typeof(GameObject) && !methodInfo.IsGenericMethod)
			{
				return !methodInfo.IsStatic;
			}
			return false;
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			MethodInfo methodInfo = memberInfo as MethodInfo;
			return new Element[1]
			{
				new StaticPanel(Name, delegate(GameObject go)
				{
					methodInfo.Invoke(settings, new object[1] { go });
				})
			};
		}
	}
	public class TextPanelAttribute : ElementAttribute
	{
		public float Width;

		public int FontSize;

		public TextPanelAttribute(float width = 1000f, int fontSize = 35, [CallerLineNumber] int order = 0)
			: base("", order)
		{
			Width = width;
			FontSize = fontSize;
		}

		public override bool VerifyCorrectFieldType(MemberInfo memberInfo)
		{
			return ElementAttribute.CheckFieldOrPropertyType(memberInfo, typeof(string));
		}

		public override Element[] CreateElement<Settings>(MemberInfo memberInfo, Settings settings)
		{
			return new Element[1]
			{
				new TextPanel((string)ElementAttribute.GetValue(memberInfo, settings), Width, FontSize)
			};
		}
	}
}

using System;
using System.Collections.Generic;
using System.Threading;
using NAudio.Wave;

namespace HapticsBridge;

internal static class Chime
{
	public static void Play(BufferedWaveProvider buffer)
	{
		List<float> samples = new List<float>();
		Action<double> action = delegate(double sec)
		{
			for (int i = 0; i < (int)(sec * 48000.0); i++)
			{
				samples.Add(0f);
				samples.Add(0f);
			}
		};
		double[][] array = new double[3][]
		{
			new double[3] { 180.0, 1.0, 0.0 },
			new double[3] { 250.0, 0.0, 1.0 },
			new double[3] { 330.0, 1.0, 1.0 }
		};
		foreach (double[] array2 in array)
		{
			int num2 = 5280;
			for (int num3 = 0; num3 < num2; num3++)
			{
				double num4 = (double)num3 / 48000.0;
				float num5 = (float)(Math.Min(1.0, num4 / 0.005) * Math.Exp((0.0 - num4) * 28.0));
				float num6 = 0.85f * num5 * (float)Math.Sin(Math.PI * 2.0 * array2[0] * num4);
				samples.Add(num6 * (float)array2[1]);
				samples.Add(num6 * (float)array2[2]);
			}
			action(0.05);
		}
		action(0.06);
		int num7 = 43200;
		double num8 = 0.0;
		double num9 = 0.0;
		for (int num10 = 0; num10 < num7; num10++)
		{
			double num11 = (double)num10 / 48000.0;
			double num12 = (double)num10 / (double)num7;
			double num13 = 200.0 + 140.0 * num12;
			num8 += Math.PI * 2.0 * num13 / 48000.0;
			num9 += Math.PI * 2.0 * (num13 * 1.06) / 48000.0;
			float num14 = 0.6f + 0.4f * (float)Math.Sin(Math.PI * 18.0 * num11);
			float num15 = (float)(Math.Min(1.0, num11 / 0.05) * (1.0 - Math.Pow(num12, 2.2)));
			float num16 = 0.55f * num14 * num15 * ((float)Math.Sin(num8) + 0.6f * (float)Math.Sin(num9));
			float num17 = 0.5f + 0.5f * (float)Math.Sin(10.681415022205297 * num11);
			samples.Add(num16 * (1f - num17));
			samples.Add(num16 * num17);
		}
		action(0.05);
		byte[] array3 = new byte[samples.Count * 4];
		Buffer.BlockCopy(samples.ToArray(), 0, array3, 0, array3.Length);
		int num18 = Environment.TickCount + (int)(1000.0 * (double)samples.Count / 2.0 / 48000.0) + 5000;
		for (int num19 = 0; num19 < array3.Length; num19 += 3840)
		{
			while (buffer.BufferedDuration.TotalMilliseconds > 120.0)
			{
				if (Environment.TickCount > num18)
				{
					return;
				}
				Thread.Sleep(5);
			}
			buffer.AddSamples(array3, num19, Math.Min(3840, array3.Length - num19));
		}
		while (buffer.BufferedDuration.TotalMilliseconds > 0.0 && Environment.TickCount <= num18)
		{
			Thread.Sleep(10);
		}
	}
}

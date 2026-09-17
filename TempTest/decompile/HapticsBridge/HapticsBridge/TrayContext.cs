using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HapticsBridge;

internal sealed class TrayContext : ApplicationContext
{
	private readonly NotifyIcon trayIcon;

	private readonly Engine engine;

	private readonly Timer poll;

	private readonly Icon iconGray = MakeIcon(Color.FromArgb(140, 140, 140));

	private readonly Icon iconBlue = MakeIcon(Color.FromArgb(80, 140, 235));

	private readonly Icon iconOrange = MakeIcon(Color.FromArgb(235, 160, 40));

	private readonly Icon iconGreen = MakeIcon(Color.FromArgb(70, 190, 90));

	private Engine.State lastState = (Engine.State)(-1);

	private string lastDetail;

	private readonly List<ToolStripMenuItem> presetItems = new List<ToolStripMenuItem>();

	public TrayContext(string[] args)
	{
		engine = new Engine(args);
		ContextMenuStrip contextMenuStrip = new ContextMenuStrip
		{
			Items = 
			{
				{
					"Play test chime",
					(Image)null,
					(EventHandler)delegate
					{
						engine.RequestChime();
					}
				},
				(ToolStripItem)BuildLatencyMenu(),
				{
					"Open log",
					(Image)null,
					(EventHandler)delegate
					{
						Process.Start(new ProcessStartInfo(Engine.LogPath)
						{
							UseShellExecute = true
						});
					}
				},
				(ToolStripItem)new ToolStripSeparator(),
				{
					"Exit",
					(Image)null,
					(EventHandler)delegate
					{
						ExitThread();
					}
				}
			}
		};
		trayIcon = new NotifyIcon
		{
			Icon = iconGray,
			Text = "Silksong Haptics: starting...",
			Visible = true,
			ContextMenuStrip = contextMenuStrip
		};
		poll = new Timer
		{
			Interval = 400
		};
		poll.Tick += delegate
		{
			// 游戏断开（--exit-on-disconnect）时自动退出主线程，右下角托盘随之消失。
			if (engine.StopRequested)
			{
				poll.Stop();
				ExitThread();
				return;
			}
			UpdateTray();
		};
		poll.Start();
		engine.Start();
	}

	private ToolStripMenuItem BuildLatencyMenu()
	{
		ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Latency");
		string[] array = new string[4] { "Leave on Reliable unless haptics lag or glitch.", "Higher = steadier (Bluetooth, busy PCs).", "Lower = snappier, but can crackle or cut out —", "especially on Bluetooth. Minimal is wired-only." };
		foreach (string text in array)
		{
			toolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(text)
			{
				Enabled = false
			});
		}
		toolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
		Engine.LatencyPreset[] presets = Engine.Presets;
		for (int i = 0; i < presets.Length; i++)
		{
			Engine.LatencyPreset latencyPreset = presets[i];
			string key = latencyPreset.Key;
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(latencyPreset.Label + " — " + latencyPreset.Blurb);
			toolStripMenuItem2.Tag = key;
			toolStripMenuItem2.Click += delegate
			{
				engine.ApplyPreset(key);
				RefreshPresetChecks();
			};
			presetItems.Add(toolStripMenuItem2);
			toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
		}
		toolStripMenuItem.DropDownOpening += delegate
		{
			RefreshPresetChecks();
		};
		RefreshPresetChecks();
		return toolStripMenuItem;
	}

	private void RefreshPresetChecks()
	{
		string currentPresetKey = engine.CurrentPresetKey;
		foreach (ToolStripMenuItem presetItem in presetItems)
		{
			presetItem.Checked = (string)presetItem.Tag == currentPresetKey;
		}
	}

	private void UpdateTray()
	{
		Engine.State currentState = engine.CurrentState;
		string stateDetail = engine.StateDetail;
		if (currentState != lastState || !(stateDetail == lastDetail))
		{
			lastState = currentState;
			lastDetail = stateDetail;
			Icon icon;
			string text;
			switch (currentState)
			{
			case Engine.State.GameConnected:
				icon = iconGreen;
				text = "Silksong Haptics: game connected";
				break;
			case Engine.State.Listening:
				icon = iconOrange;
				text = "Silksong Haptics: ready, waiting for game";
				break;
			case Engine.State.Extracting:
				icon = iconBlue;
				text = "Silksong Haptics: extracting clips " + stateDetail;
				break;
			default:
				icon = iconGray;
				text = "Silksong Haptics: no DualSense audio device";
				break;
			}
			trayIcon.Icon = icon;
			trayIcon.Text = ((text.Length > 63) ? text.Substring(0, 63) : text);
		}
	}

	protected override void ExitThreadCore()
	{
		poll.Stop();
		engine.Stop();
		trayIcon.Visible = false;
		trayIcon.Dispose();
		base.ExitThreadCore();
	}

	private static Icon MakeIcon(Color color)
	{
		using Bitmap bitmap = new Bitmap(16, 16);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.Clear(Color.Transparent);
			using (SolidBrush brush = new SolidBrush(color))
			{
				graphics.FillEllipse(brush, 2, 2, 12, 12);
			}
			using Pen pen = new Pen(Color.FromArgb(60, 0, 0, 0));
			graphics.DrawEllipse(pen, 2, 2, 12, 12);
		}
		return Icon.FromHandle(bitmap.GetHicon());
	}
}

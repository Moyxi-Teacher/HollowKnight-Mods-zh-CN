using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HapticsBridge;

internal static class GameLocator
{
	private const string GameExe = "Hollow Knight Silksong.exe";

	private static readonly string RememberFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gamepath.txt");

	public static string Locate(bool interactive)
	{
		for (DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); directoryInfo != null; directoryInfo = directoryInfo.Parent)
		{
			if (File.Exists(Path.Combine(directoryInfo.FullName, "Hollow Knight Silksong.exe")))
			{
				return directoryInfo.FullName;
			}
		}
		try
		{
			if (File.Exists(RememberFile))
			{
				string text = Validate(CleanUserPath(File.ReadAllText(RememberFile)));
				if (text != null)
				{
					return text;
				}
			}
		}
		catch
		{
		}
		string[] array = new string[2] { "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Hollow Knight Silksong", "C:\\Program Files\\Steam\\steamapps\\common\\Hollow Knight Silksong" };
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = Validate(array[i]);
			if (text2 != null)
			{
				return text2;
			}
		}
		if (!interactive)
		{
			return null;
		}
		return PromptUser();
	}

	public static string CleanUserPath(string raw)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return null;
		}
		string text = raw.Trim();
		text = text.Trim('"', '\'', ' ', '\t', '\r', '\n');
		if (text.Length == 0)
		{
			return null;
		}
		text = Environment.ExpandEnvironmentVariables(text);
		text = text.Replace('/', '\\');
		if (text.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
		{
			try
			{
				text = Path.GetDirectoryName(text);
			}
			catch
			{
				return null;
			}
		}
		if (text == null)
		{
			return null;
		}
		text = text.TrimEnd('\\', ' ');
		if (text.Length != 0)
		{
			return text;
		}
		return null;
	}

	public static string Validate(string dir)
	{
		if (string.IsNullOrEmpty(dir))
		{
			return null;
		}
		try
		{
			if (File.Exists(Path.Combine(dir, "Hollow Knight Silksong.exe")))
			{
				return dir;
			}
			string text = Path.Combine(dir, "Hollow Knight Silksong");
			if (File.Exists(Path.Combine(text, "Hollow Knight Silksong.exe")))
			{
				return text;
			}
			if (dir.EndsWith("Hollow Knight Silksong_Data", StringComparison.OrdinalIgnoreCase))
			{
				string directoryName = Path.GetDirectoryName(dir);
				if (directoryName != null && File.Exists(Path.Combine(directoryName, "Hollow Knight Silksong.exe")))
				{
					return directoryName;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static string PromptUser()
	{
		Form form = new Form();
		try
		{
			form.Text = "Silksong Haptics - locate game";
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.MaximizeBox = false;
			form.MinimizeBox = false;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.ClientSize = new Size(560, 130);
			Label label = new Label
			{
				Text = "Couldn't find Hollow Knight: Silksong automatically.\nPaste your Silksong install folder below (any format is fine), or Browse.",
				Location = new Point(12, 10),
				Size = new Size(536, 36)
			};
			TextBox box = new TextBox
			{
				Location = new Point(12, 52),
				Size = new Size(440, 24)
			};
			Button button = new Button
			{
				Text = "Browse...",
				Location = new Point(460, 50),
				Size = new Size(88, 26)
			};
			Button button2 = new Button
			{
				Text = "OK",
				Location = new Point(372, 90),
				Size = new Size(80, 28),
				DialogResult = DialogResult.OK
			};
			Button button3 = new Button
			{
				Text = "Cancel",
				Location = new Point(460, 90),
				Size = new Size(88, 28),
				DialogResult = DialogResult.Cancel
			};
			button.Click += delegate
			{
				using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
				{
					Description = "Select the Hollow Knight Silksong folder"
				};
				if (folderBrowserDialog.ShowDialog(form) == DialogResult.OK)
				{
					box.Text = folderBrowserDialog.SelectedPath;
				}
			};
			form.Controls.AddRange(new Control[5] { label, box, button, button2, button3 });
			form.AcceptButton = button2;
			form.CancelButton = button3;
			string text2;
			while (true)
			{
				if (form.ShowDialog() != DialogResult.OK)
				{
					return null;
				}
				string text = CleanUserPath(box.Text);
				text2 = Validate(text);
				if (text2 != null)
				{
					break;
				}
				MessageBox.Show(string.Format("Couldn't find \"{0}\" under:\n{1}\n\nPlease check the path and try again.", "Hollow Knight Silksong.exe", text ?? "(empty)"), "Not quite");
			}
			try
			{
				File.WriteAllText(RememberFile, text2);
			}
			catch
			{
			}
			return text2;
		}
		finally
		{
			if (form != null)
			{
				((IDisposable)form).Dispose();
			}
		}
	}
}

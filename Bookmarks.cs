using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace ClassicExplorer
{
    public static class Bookmarks
    {
        private static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "bookmarks.txt");

        public static void Save(string title, string url)
        {
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.AppendAllText(filePath, title + "|" + url + Environment.NewLine);
            } catch { }
        }

        public static void LoadToMenu(ToolStripMenuItem favMenu, EventHandler onClick, Image icon)
        {
            try {
                if (File.Exists(filePath)) {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 2) {
                            ToolStripMenuItem item = new ToolStripMenuItem(parts[0]);
                            item.ToolTipText = parts[1]; // URL прячем в подсказку
                            item.Image = icon;
                            item.Click += onClick;
                            favMenu.DropDownItems.Add(item);
                        }
                    }
                }
            } catch { }
        }
    }
}
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace ClassicExplorer
{
    public class IEForm : Form
    {
        private TabControl tabControl;
        private MenuStrip menu;
        private ToolStrip toolbar;
        private ToolStrip addressStrip;
        private StatusStrip status;
        private ToolStripTextBox addressBar;
        
        private ToolStripLabel lblAddress;
        private ToolStripStatusLabel lblStatus;
        
        private ToolStripButton btnBack, btnForward, btnStop, btnRefresh, btnHome, btnFav, btnMute, btnDownloads, btnNewTab, btnCloseTab;
        
        private bool isDarkMode = false; 
        private bool isIncognito = false;
        private bool isFamilyMode = false;
        private string currentProfileFolder = "";
        private string searchEngine = "https://www.google.com/search?q=";
        private bool isFullscreen = false;
        private FormWindowState prevWindowState;

        private ToolStripMenuItem fileMenu, editMenu, viewMenu, favMenu, toolsMenu, helpMenu;
        private ToolStripMenuItem langMenu;

        private Image TryLoadImage(string filename)
        {
            try {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sprites", filename);
                if (File.Exists(path)) return Image.FromFile(path);
            } catch { } return null;
        }

        private ToolStripButton CreateButton(string imageFile, EventHandler onClick)
        {
            ToolStripButton btn = new ToolStripButton();
            btn.Image = TryLoadImage(imageFile);
            btn.TextImageRelation = TextImageRelation.ImageAboveText;
            btn.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btn.Click += onClick;
            return btn;
        }

        private WebView2 GetCurrentBrowser()
        {
            if (tabControl != null && tabControl.SelectedTab != null && tabControl.SelectedTab.Controls.Count > 0)
                return tabControl.SelectedTab.Controls[0] as WebView2;
            return null;
        }

        public IEForm(string startUrl, bool incognitoMode)
        {
            isIncognito = incognitoMode;
            if (isIncognito) currentProfileFolder = Path.Combine(Path.GetTempPath(), "IE_InPrivate_" + Guid.NewGuid().ToString("N"));
            else currentProfileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClassicExplorerProfile");

            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true; 
            try { this.Icon = new Icon("icon.ico"); } catch { }

            menu = new MenuStrip();
            
            // --- ФАЙЛ ---
            fileMenu = new ToolStripMenuItem { Image = TryLoadImage("menu-file.png") };
            ToolStripMenuItem newWindowItem = new ToolStripMenuItem("New Window") { Image = TryLoadImage("gnome-run.png") };
            newWindowItem.Click += (s, e) => Process.Start(Application.ExecutablePath);
            ToolStripMenuItem openFileItem = new ToolStripMenuItem("Open File (Ctrl+O)") { Image = TryLoadImage("folder-open.png") };
            openFileItem.Click += (s, e) => OpenLocalFile();
            ToolStripMenuItem printItem = new ToolStripMenuItem("Print...") { Image = TryLoadImage("document-print.png") };
            printItem.Click += (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null) b.CoreWebView2.ShowPrintUI(); };
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => this.Close();
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { newWindowItem, openFileItem, new ToolStripSeparator(), printItem, new ToolStripSeparator(), exitItem });

            // --- ПРАВКА ---
            editMenu = new ToolStripMenuItem { Image = TryLoadImage("menu-edit.png") };
            ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy") { Image = TryLoadImage("edit-copy.png") };
            copyItem.Click += (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null) b.CoreWebView2.ExecuteScriptAsync("document.execCommand('copy')"); };
            ToolStripMenuItem pasteItem = new ToolStripMenuItem("Paste") { Image = TryLoadImage("edit-paste.png") };
            pasteItem.Click += (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null) b.CoreWebView2.ExecuteScriptAsync("document.execCommand('paste')"); };
            ToolStripMenuItem copyUrlItem = new ToolStripMenuItem("Copy Page URL");
            copyUrlItem.Click += (s, e) => { if (addressBar.Text != "") Clipboard.SetText(addressBar.Text); };
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { copyItem, pasteItem, new ToolStripSeparator(), copyUrlItem });

            // --- ВИД ---
            viewMenu = new ToolStripMenuItem { Image = TryLoadImage("menu-view.png") };
            ToolStripMenuItem fullscreenItem = new ToolStripMenuItem("Fullscreen (F11)") { Image = TryLoadImage("view-fullscreen.png") };
            fullscreenItem.Click += (s, e) => ToggleFullscreen();
            
            langMenu = new ToolStripMenuItem("Language");
            string[] langNames = { "English", "Русский", "Українська", "Беларуская", "Polski", "Deutsch", "Français", "Español", "Italiano", "Português", "中文", "日本語", "한국어", "العربية", "हिन्दी", "Türkçe", "Nederlands", "Svenska", "Čeština", "Română" };
            for(int i=0; i<20; i++) {
                int index = i; 
                ToolStripMenuItem lItem = new ToolStripMenuItem(langNames[i]);
                lItem.Click += (s, e) => SetLanguage((AppLanguage)index);
                langMenu.DropDownItems.Add(lItem);
            }
            viewMenu.DropDownItems.AddRange(new ToolStripItem[] { fullscreenItem, new ToolStripSeparator(), langMenu });

            // --- ИЗБРАННОЕ ---
            favMenu = new ToolStripMenuItem { Image = TryLoadImage("menu-favorites.png") };
            ToolStripMenuItem duplicateTabItem = new ToolStripMenuItem("Duplicate Tab");
            duplicateTabItem.Click += (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.Source != null) AddNewTab(b.Source.ToString()); };
            favMenu.DropDownItems.AddRange(new ToolStripItem[] { duplicateTabItem, new ToolStripSeparator() });
            
            // ЗАГРУЗКА ЗАКЛАДОК ИЗ ФАЙЛА Bookmarks.cs
            Bookmarks.LoadToMenu(favMenu, (s, e) => Navigate(((ToolStripMenuItem)s).ToolTipText), TryLoadImage("emblem-web.png"));

            // --- СЕРВИС ---
            toolsMenu = new ToolStripMenuItem { Image = TryLoadImage("menu-tools.png") };
            ToolStripMenuItem clearDataItem = new ToolStripMenuItem("Clear Browsing Data...");
            clearDataItem.Click += (s, e) => {
                WebView2 b = GetCurrentBrowser();
                if (b != null && b.CoreWebView2 != null) {
                    b.CoreWebView2.CookieManager.DeleteAllCookies();
                    MessageBox.Show("Cookies and session data cleared!", "Classic Explorer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            ToolStripMenuItem devToolsItem = new ToolStripMenuItem("Developer Tools") { Image = TryLoadImage("applications-development.png") };
            devToolsItem.Click += (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null) b.CoreWebView2.OpenDevToolsWindow(); };
            toolsMenu.DropDownItems.AddRange(new ToolStripItem[] { clearDataItem, devToolsItem });

            // --- СПРАВКА ---
            helpMenu = new ToolStripMenuItem { Image = TryLoadImage("menu-help.png") };
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About") { Image = TryLoadImage("help-about.png") };
            aboutItem.Click += (s, e) => MessageBox.Show("Classic Explorer\nВерсия: 2.0 Modular\n\nРазработчики:\n- Pixelshot70\n- vvllaadd_1230", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            helpMenu.DropDownItems.Add(aboutItem);

            menu.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, viewMenu, favMenu, toolsMenu, helpMenu });

            // ПАНЕЛЬ ИНСТРУМЕНТОВ
            toolbar = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden, ImageScalingSize = new Size(32, 32) };

            btnBack = CreateButton("notification-audio-previous.png", (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null && b.CoreWebView2.CanGoBack) b.CoreWebView2.GoBack(); });
            btnForward = CreateButton("notification-audio-next.png", (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null && b.CoreWebView2.CanGoForward) b.CoreWebView2.GoForward(); });
            btnStop = CreateButton("gtk-cancel.png", (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null) b.CoreWebView2.Stop(); });
            btnRefresh = CreateButton("notification-audio-play.png", (s, e) => { WebView2 b = GetCurrentBrowser(); if (b != null && b.CoreWebView2 != null) b.CoreWebView2.Reload(); });
            btnHome = CreateButton("start-here.png", (s, e) => Navigate("https://www.google.com"));
            
            btnFav = CreateButton("emblem-favorite.png", (s, e) => {
                WebView2 b = GetCurrentBrowser();
                if (b != null && b.Source != null && b.CoreWebView2 != null) {
                    string title = b.CoreWebView2.DocumentTitle;
                    string url = b.Source.ToString();
                    Bookmarks.Save(title, url); 
                    ToolStripMenuItem newBookmark = new ToolStripMenuItem(title) { ToolTipText = url, Image = TryLoadImage("emblem-web.png") };
                    newBookmark.Click += (ss, ee) => AddNewTab(url);
                    favMenu.DropDownItems.Add(newBookmark);
                }
            });

            btnMute = CreateButton("audio-volume-medium.png", (s, e) => {
                WebView2 b = GetCurrentBrowser();
                if (b != null && b.CoreWebView2 != null) {
                    b.CoreWebView2.IsMuted = !b.CoreWebView2.IsMuted;
                    btnMute.Image = TryLoadImage(b.CoreWebView2.IsMuted ? "audio-volume-muted.png" : "audio-volume-medium.png");
                }
            });
            
            btnDownloads = CreateButton("folder-downloads.png", (s, e) => Process.Start("explorer.exe", "shell:downloads"));
            btnNewTab = CreateButton("emblem-new.png", (s, e) => AddNewTab("https://www.google.com"));
            btnCloseTab = CreateButton("gtk-cancel.png", (s, e) => CloseCurrentTab());

            toolbar.Items.AddRange(new ToolStripItem[] { btnBack, btnForward, new ToolStripSeparator(), btnStop, btnRefresh, btnHome, new ToolStripSeparator(), btnFav, btnMute, btnDownloads, new ToolStripSeparator(), btnNewTab, btnCloseTab });

            // АДРЕСНАЯ СТРОКА
            addressStrip = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden };
            lblAddress = new ToolStripLabel();
            addressStrip.Items.Add(lblAddress);

            addressBar = new ToolStripTextBox { Size = new Size(600, 25) };
            addressBar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Navigate(addressBar.Text); };
            addressStrip.Items.Add(addressBar);

            ToolStripButton btnGo = new ToolStripButton { Image = TryLoadImage("gnome-do.png"), DisplayStyle = ToolStripItemDisplayStyle.Image };
            btnGo.Click += (s, e) => Navigate(addressBar.Text);
            addressStrip.Items.Add(btnGo);

            // СТАТУС БАР
            status = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            status.Items.Add(lblStatus);
            this.Controls.Add(status);

            // ПАНЕЛЬ ВКЛАДОК
            tabControl = new TabControl { Dock = DockStyle.Fill };
            tabControl.SelectedIndexChanged += (s, e) => {
                WebView2 b = GetCurrentBrowser();
                if (b != null && b.Source != null) addressBar.Text = b.Source.ToString();
                else addressBar.Text = "";
            };

            this.Controls.Add(tabControl);
            this.Controls.Add(addressStrip);
            this.Controls.Add(toolbar);
            this.Controls.Add(menu);
            this.MainMenuStrip = menu;

            // СТАВИМ АНГЛИЙСКИЙ ПО УМОЛЧАНИЮ ПЕРЕД ЗАГРУЗКОЙ ВКЛАДКИ
            SetLanguage(AppLanguage.EN); 
            
            // Если есть локальная StartPage.html, грузим её
            string localStart = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "StartPage.html");
            if (startUrl == "https://www.google.com" && File.Exists(localStart)) startUrl = localStart;

            AddNewTab(startUrl);

            this.FormClosing += (s, e) => {
                if (isIncognito) {
                    foreach (TabPage page in tabControl.TabPages) {
                        if (page.Controls.Count > 0) {
                            WebView2 b = page.Controls[0] as WebView2;
                            if (b != null) b.Dispose();
                        }
                    }
                    try { Directory.Delete(currentProfileFolder, true); } catch { }
                }
            };
        }

        private async void AddNewTab(string url)
        {
            TabPage newPage = new TabPage(Loc.Get("Loading"));
            WebView2 newBrowser = new WebView2 { Dock = DockStyle.Fill };
            
            newPage.Controls.Add(newBrowser);
            tabControl.TabPages.Add(newPage);
            tabControl.SelectedTab = newPage;

            var environment = await CoreWebView2Environment.CreateAsync(null, currentProfileFolder, null);
            await newBrowser.EnsureCoreWebView2Async(environment);

            if (newBrowser.CoreWebView2 != null)
            {
                newBrowser.CoreWebView2.DocumentTitleChanged += (s, e) => { newPage.Text = newBrowser.CoreWebView2.DocumentTitle; };
                newBrowser.NavigationCompleted += (s, e) => {
                    if (newBrowser == GetCurrentBrowser() && newBrowser.Source != null) addressBar.Text = newBrowser.Source.ToString();
                };
                newBrowser.CoreWebView2.Navigate(url);
            }
        }

        private void CloseCurrentTab()
        {
            if (tabControl.TabPages.Count > 1) {
                TabPage current = tabControl.SelectedTab;
                WebView2 b = GetCurrentBrowser();
                if (b != null) b.Dispose();
                tabControl.TabPages.Remove(current);
            }
        }

        private void OpenLocalFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "HTML Files|*.html;*.htm|All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK) AddNewTab(ofd.FileName);
        }

        private void ToggleFullscreen()
        {
            isFullscreen = !isFullscreen;
            if (isFullscreen) {
                prevWindowState = this.WindowState;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                menu.Visible = false; addressStrip.Visible = false; toolbar.Visible = false; status.Visible = false;
            } else {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = prevWindowState;
                menu.Visible = true; addressStrip.Visible = true; toolbar.Visible = true; status.Visible = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F11) { ToggleFullscreen(); return true; }
            if (keyData == (Keys.Control | Keys.T)) { AddNewTab("https://www.google.com"); return true; }
            if (keyData == (Keys.Control | Keys.W)) { CloseCurrentTab(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Navigate(string urlText)
        {
            if (!string.IsNullOrWhiteSpace(urlText)) {
                string url = urlText;
                if (!url.Contains(".") && !url.Contains(":/") && !url.Contains(":\\")) url = searchEngine + Uri.EscapeDataString(url);
                else if (!url.StartsWith("http") && !url.StartsWith("file://") && !Path.IsPathRooted(url)) url = "https://" + url;

                WebView2 b = GetCurrentBrowser();
                if (b != null && b.CoreWebView2 != null) b.CoreWebView2.Navigate(url);
            }
        }

        private void SetLanguage(AppLanguage lang)
        {
            Loc.CurrentLang = lang;
            this.Text = isIncognito ? "Classic Explorer - InPrivate" : "Classic Explorer";
            
            fileMenu.Text = Loc.Get("MenuFile"); 
            editMenu.Text = Loc.Get("MenuEdit"); 
            viewMenu.Text = Loc.Get("MenuView"); 
            favMenu.Text = Loc.Get("MenuFav"); 
            toolsMenu.Text = Loc.Get("MenuTools"); 
            helpMenu.Text = Loc.Get("MenuHelp");
            
            lblAddress.Text = Loc.Get("Address"); 
            lblStatus.Text = isIncognito ? Loc.Get("InPrivate") : Loc.Get("Ready");
            
            // ИСПРАВЛЕНИЕ: Теперь текст кнопок меняется без проблем!
            btnBack.Text = Loc.Get("Back");
            btnForward.Text = Loc.Get("Forward");
            btnStop.Text = Loc.Get("Stop");
            btnRefresh.Text = Loc.Get("Refresh");
            btnHome.Text = Loc.Get("Home");
            btnFav.Text = Loc.Get("Favorite");
            btnMute.Text = Loc.Get("Sound");
            btnDownloads.Text = Loc.Get("Downloads");
            btnNewTab.Text = Loc.Get("NewTab");
            btnCloseTab.Text = Loc.Get("CloseTab");

            // ИСПРАВЛЕНИЕ: Логика проверки пустой вкладки
            if (tabControl != null && tabControl.SelectedTab != null) {
                if (tabControl.SelectedTab.Text == "Loading..." || tabControl.SelectedTab.Text == "Загрузка...") {
                    tabControl.SelectedTab.Text = Loc.Get("Loading");
                }
            }
        }

        // ГЛОБАЛЬНЫЙ ОТЛОВЩИК ОШИБОК! 
        // Теперь вместо вылета будет красивое окно с причиной.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                string startUrl = "https://www.google.com";
                bool incognito = false;

                foreach (string arg in args) {
                    if (arg == "--incognito") incognito = true;
                    else startUrl = arg;
                }
                
                Application.Run(new IEForm(startUrl, incognito));
            }
            catch (Exception ex)
            {
                // Если что-то пойдет не так при запуске, мы увидим это окно!
                MessageBox.Show("Критическая ошибка при запуске:\n\n" + ex.Message + "\n\n" + ex.StackTrace, "Ошибка Classic Explorer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
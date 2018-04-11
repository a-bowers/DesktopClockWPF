using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace DesktopClockWPF
{
	using med = System.Windows.Media;

	public partial class MainWindow : Window
	{
		Timer timer = new Timer();
		NotifyIcon icon = new NotifyIcon();
		ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
		ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem();
		ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
		ColorDialog colorDialog = new ColorDialog();


		public MainWindow() {
			InitializeComponent();

			Screen scrn = Screen.AllScreens.Last();
			if(scrn == null) {
				System.Windows.MessageBox.Show("No screens.");
				Close();
			}

			Rectangle bounds = scrn.Bounds;
			Left = bounds.Right - Width;
			Top = bounds.Top;

			this.Loaded += new RoutedEventHandler(OnLoad);

			timer.Interval = 1000;
			timer.Tick += new EventHandler(OnTick);
			timer.Start();

			icon.Icon = Properties.Resources.CounterIcon;
			icon.Text = "Desktop Clock";
			icon.Visible = true;
			icon.ContextMenuStrip = contextMenuStrip1;

			contextMenuStrip1.Items.AddRange(new []{toolStripMenuItem1, toolStripMenuItem2});
			toolStripMenuItem1.Text = "Close";
			toolStripMenuItem1.Click += new EventHandler(toolStripMenuItem1_Click);

			toolStripMenuItem2.Text = "Colour";
			toolStripMenuItem2.Click += new EventHandler(toolStripMenuItem2_Click);

			//to hide from alt-tab menu
			//https://social.msdn.microsoft.com/Forums/vstudio/en-US/8e3a788e-1e14-4751-a756-2d68358f898b/hide-icon-in-alttab?forum=wpf
			Window w = new Window();
			w.Top = w.Left = -100;
			w.Width = w.Height = 1;
			w.WindowStyle = WindowStyle.ToolWindow;
			w.ShowInTaskbar = false;
			w.Show();
			Owner = w;
			w.Hide();

			OnTick(this, null);
		}

		void OnLoad(object sender, RoutedEventArgs e)
		{
			SendToBack();
		}

		void OnTick(object sender, EventArgs e) {
			TimeLabel.Content = DateTime.Now.ToShortTimeString();
		}

		void toolStripMenuItem1_Click(object sender, EventArgs e) {
			Close();
		}

		void toolStripMenuItem2_Click(object sender, EventArgs e) {
			if(colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				TimeLabel.Foreground = ColourToBrush(colorDialog.Color);
		}

		med.Brush ColourToBrush(System.Drawing.Color col) {
			var colour = med.Color.FromArgb(col.A, col.R, col.G, col.B);
			return new SolidColorBrush(colour);
		}

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWind, IntPtr hWindInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		const UInt32 SWP_NOSIZE = 0x0001, SWP_NOMOVE = 0x0002, SWP_NOACTIVATE = 0x0010;
		static readonly IntPtr HWIND_BOTTOM = new IntPtr(1);

		void SendToBack()
		{
			var hWind = new WindowInteropHelper(this).Handle;
			SetWindowPos(hWind, HWIND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
		}
	}
}
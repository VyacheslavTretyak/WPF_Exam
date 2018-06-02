using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;

namespace KeyboardTrainer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private bool caps = false;
		private bool isTape = false;
		private List<char> lowChars;
		private List<char> capsChars;
		private string line = "";
		private string inputString = "";
		private DateTime startTime;

		private int fails = 0;
		public int Fails
		{
			get { return fails; }
			set
			{
				fails = value;
				OnPropertyChanged();
			}
		}
		private int speed = 0;
		public int Speed
		{
			get { return speed; }
			set
			{
				speed = value;
				OnPropertyChanged();
			}
		}
		protected void OnPropertyChanged(string propertyName = null)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				var e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			GetChars();
			SetChar();
			btnStop.IsEnabled = false;
			btnStart.IsEnabled = true;
		}

		private void GetChars()
		{
			lowChars = new List<char>();
			capsChars = new List<char>();
			foreach (Grid grid in keyboard.Children)
			{
				foreach (Control item in grid.Children)
				{
					ButtonWithKey btn = item as ButtonWithKey;
					if (btn != null)
					{
						if (btn.CharCaps != null)
						{
							lowChars.Add(btn.Char[0]);
							capsChars.Add(btn.CharCaps[0]);
						}
					}
				}
			}
		}

		private void LoadKeys()
		{
			foreach (Grid grid in keyboard.Children)
			{
				foreach (Control item in grid.Children)
				{
					ButtonWithKey btn = item as ButtonWithKey;
					btn?.SetChar(caps);
				}
			}
		}

		private void Button_KeyDown(object sender, KeyEventArgs e)
		{
			//MessageBox.Show(e.Key.ToString());
			foreach (Grid grid in keyboard.Children)
			{
				foreach (Control item in grid.Children)
				{
					ButtonWithKey btn = item as ButtonWithKey;
					if (btn != null)
					{
						if (btn.Key == Key.CapsLock || btn.Key == Key.LeftShift || btn.Key == Key.RightShift)
						{
							SetChar();
						}
						if (btn.Key == e.Key)
						{
							btn.IsPress = true;
							if (btn.CharCaps != null && isTape)
							{
								if (!caps)
								{
									Verify(btn.Char);
								}
								else
								{
									Verify(btn.CharCaps);
								}
							}
						}
					}
				}
			}
		}

		private void Verify(string ch)
		{
			inputString += ch;
			if (ch[0] != line[inputString.Length - 1])
			{
				Fails++;
				tblInput.Inlines.Add(new Run(ch) { Background = new SolidColorBrush(Colors.Coral) });
			}
			else
			{
				tblInput.Inlines.Add(new Run(ch));
			}

			if (inputString.Length == line.Length)
			{
				Stop();
				return;
			}
			var s1 = new Run(line.Substring(0, inputString.Length));
			var s2 = new Bold(new Run(line[inputString.Length].ToString()))
			{
				Foreground = new SolidColorBrush(Colors.Green),
				Background = new SolidColorBrush(Colors.LemonChiffon),
			};
			var s3 = new Run(line.Substring(inputString.Length + 1));
			tblLine.Inlines.Clear();
			tblLine.Inlines.Add(s1);
			tblLine.Inlines.Add(s2);
			tblLine.Inlines.Add(s3);
			PrintSpeed();
		}

		private void SetChar()
		{
			caps = (Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled;
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				caps = !caps;
			}
			LoadKeys();
		}
		private void PrintSpeed()
		{			
			TimeSpan ts = DateTime.Now - startTime;
			Speed = (int)(inputString.Length / ts.TotalMinutes);
		}

		private void Button_KeyUp(object sender, KeyEventArgs e)
		{
			foreach (Grid grid in keyboard.Children)
			{
				foreach (Control item in grid.Children)
				{
					ButtonWithKey btn = item as ButtonWithKey;
					if (btn != null)
					{
						if (btn.Key == Key.CapsLock || btn.Key == Key.LeftShift || btn.Key == Key.RightShift)
						{
							SetChar();
						}
						if (btn.Key == e.Key)
						{
							btn.IsPress = false;
						}
					}
				}
			}
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			isTape = true;
			btnStop.IsEnabled = true;
			btnStart.IsEnabled = false;
			Tape();
		}

		private void Tape()
		{
			startTime = DateTime.Now;
			Speed = 0;
			Fails = 0;
			line = "";
			string seq = "";
			Random rnd = new Random();
			List<char> chars = new List<char>();
			chars.AddRange(lowChars);
			if (cbCaseSens.IsChecked == true)
			{
				chars.AddRange(capsChars);
			}
			for (int i = 0; i < slider.Value + 1; i++)
			{
				seq += chars[rnd.Next() % chars.Count];
			}
			FormattedText seqFormat = new FormattedText(
				seq,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(tblLine.FontFamily, tblLine.FontStyle, tblLine.FontWeight, tblLine.FontStretch),
				tblLine.FontSize,
				tblLine.Foreground,
				new NumberSubstitution(),
				TextFormattingMode.Display);
			FormattedText lineFormat = new FormattedText(
				line,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(tblLine.FontFamily, tblLine.FontStyle, tblLine.FontWeight, tblLine.FontStretch),
				tblLine.FontSize,
				tblLine.Foreground,
				new NumberSubstitution(),
				TextFormattingMode.Display);
			while (lineFormat.Width < tblLine.ActualWidth - seqFormat.Width - 25)
			{
				line += seq;
				lineFormat = new FormattedText(
				line,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(tblLine.FontFamily, tblLine.FontStyle, tblLine.FontWeight, tblLine.FontStretch),
				tblLine.FontSize,
				tblLine.Foreground,
				new NumberSubstitution(),
				TextFormattingMode.Display);
			}
			tblLine.Inlines.Add(line);
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			Stop();
		}

		private void Stop()
		{
			tblLine.Inlines.Clear();
			tblInput.Inlines.Clear();
			inputString = "";
			isTape = false;
			btnStop.IsEnabled = false;
			btnStart.IsEnabled = true;
			//string res = $"Difficulty : {slider.Value}\nFail : {Fails}\nMax speed : {Speed}";			
			//MessageBox.Show(res);
		}
	}
}

using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace KeyboardTrainer
{	
	class ButtonWithKey: Button
	{
		//static ButtonWithKey()
		//{
		//	//DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonWithKey),
		//	  //new FrameworkPropertyMetadata(typeof(ButtonWithKey)));

		//}
		////[Description("The image displayed by the button"), Category("Common Properties")]
		////public Key Key { get; set; }		

		public Key Key
		{
			get { return (Key)GetValue(KeyProperty); }
			set { SetValue(KeyProperty, value); }
		}
		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(ButtonWithKey));

		
		public bool IsPress
		{
			get { return (bool)GetValue(IsPressProperty); }
			set { SetValue(IsPressProperty, value); }
		}
		public static readonly DependencyProperty IsPressProperty = DependencyProperty.Register("IsPress", typeof(bool), typeof(ButtonWithKey));

		public string Char
		{
			get { return (string)GetValue(CharProperty); }
			set { SetValue(CharProperty, value); }
		}
		public static readonly DependencyProperty CharProperty = DependencyProperty.Register("Char", typeof(string), typeof(ButtonWithKey));

		public string CharCaps
		{
			get { return (string)GetValue(CharCapsProperty); }
			set { SetValue(CharCapsProperty, value); }
		}
		public static readonly DependencyProperty CharCapsProperty = DependencyProperty.Register("CharCaps", typeof(string), typeof(ButtonWithKey));

		public void SetChar(bool caps)
		{
			if (caps)
			{
				if (CharCaps == null)
				{
					Content = Char;
				}
				else
				{
					Content = CharCaps;
				}
			}
			else
			{
				Content = Char;
			}
		}

	}
}

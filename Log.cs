using System;

namespace SenseoBT
{
	public class Log
	{
		public Log(string txt, int img) {
			this.Text = txt;
			this.Image = img;
		}

		public string Text { get; set; }

		public int Image { get; set; }
	}
}


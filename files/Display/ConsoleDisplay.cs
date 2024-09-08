using System.Runtime.InteropServices;

namespace ConsoleEngine
{
	public class ConsoleDisplay // console display with double buffering
	{
		private int Width, Height;
		public int[] BackBuffer;
		private int[] FrontBuffer;

		private const char FULL_BLOCK = '█';
		private const char DARK_SHADE = '▓';
		private const char MEDIUM_SHADE = '▒';
		private const char LIGHT_SHADE = '░';
		private const char DOT = '.';

		public ConsoleDisplay(int width = 96, int height = 54)
		{
			SetConsoleFont("Consolas", 6, 6);

			Width = width;
			Height = height;
			BackBuffer = new int[Width * Height];
			FrontBuffer = new int[Width * Height];

			Array.Fill(BackBuffer, 0);
			Array.Fill(FrontBuffer, 0);

			Console.SetWindowSize(Math.Min(Width * 2, Console.LargestWindowWidth), Math.Min(Height + 1, Console.LargestWindowHeight));
			Console.CursorVisible = false;
			Console.Clear();
		}

		public void Render()
		{
			BackBuffer = FrameBuffer.buffer;
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					int index = y * Width + x;
					if (BackBuffer[index] != FrontBuffer[index])
					{
						Console.SetCursorPosition(x * 2, y);
						Console.Write(new string(MapIntToChar(BackBuffer[index]), 2));
						FrontBuffer[index] = BackBuffer[index];
					}
				}
			}
			Console.SetCursorPosition(0, 0);
		}

		private char MapIntToChar(int value) // shading
		{
			char shade =
			 value < 0 ? ' ' :
			 value < 60 ? FULL_BLOCK :
			 value < 120 ? DARK_SHADE :
			 value < 220 ? MEDIUM_SHADE :
			 value < 300 ? LIGHT_SHADE : DOT;

			return shade;
		}

		static void SetConsoleFont(string fontName, short sizeX, short sizeY) // change "pixel" size
		{
			IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
			if (hnd != IntPtr.Zero)
			{
				CONSOLE_FONT_INFO_EX info = new CONSOLE_FONT_INFO_EX();
				info.cbSize = (uint)Marshal.SizeOf(info);

				if (GetCurrentConsoleFontEx(hnd, false, ref info))
				{
					info.dwFontSize = new COORD(sizeX, sizeY);
					info.FontFamily = TMPF_TRUETYPE;
					info.FaceName = fontName;

					SetCurrentConsoleFontEx(hnd, false, ref info);
				}
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern bool SetCurrentConsoleFontEx(
			IntPtr consoleOutput,
			bool maximumWindow,
			ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern bool GetCurrentConsoleFontEx(
			IntPtr consoleOutput,
			bool maximumWindow,
			ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

		private const int STD_OUTPUT_HANDLE = -11;
		private const int TMPF_TRUETYPE = 4;
		private const int LF_FACESIZE = 32;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CONSOLE_FONT_INFO_EX
		{
			public uint cbSize;
			public uint nFont;
			public COORD dwFontSize;
			public int FontFamily;
			public int FontWeight;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
			public string FaceName;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct COORD
		{
			public short X;
			public short Y;

			public COORD(short x, short y)
			{
				X = x;
				Y = y;
			}
		}
	}
}

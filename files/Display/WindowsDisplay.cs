using System.Runtime.InteropServices;

namespace ConsoleEngine
{
	public class WindowsDisplay : IDisposable
	{
		private const int CS_HREDRAW = 0x0002;
		private const int CS_VREDRAW = 0x0001;
		private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
		private const int DIB_RGB_COLORS = 0;
		private const uint WM_CLOSE = 0x0010;
		private const uint WM_DESTROY = 0x0002;

		private IntPtr hWnd;
		private IntPtr hDC;
		private IntPtr hBitmap;
		private IntPtr hOldBitmap;
		private IntPtr pBits;

		private uint width;
		private uint height;

		private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		public WindowsDisplay(uint windowWidth, uint windowHeight)
		{
			this.width = windowWidth;
			this.height = windowHeight;

			CreateWindow();
			CreateDIB();
		}

		private void CreateWindow()
		{
			WNDCLASSEX wc = new WNDCLASSEX();
			wc.cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX));
			wc.style = CS_HREDRAW | CS_VREDRAW;
			wc.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(new WndProcDelegate(WndProc));
			wc.cbClsExtra = 0;
			wc.cbWndExtra = 0;
			wc.hInstance = IntPtr.Zero;
			wc.hIcon = IntPtr.Zero;
			wc.hCursor = IntPtr.Zero;
			wc.hbrBackground = IntPtr.Zero;
			wc.lpszMenuName = null;
			wc.lpszClassName = "RenderWindowClass";
			wc.hIconSm = IntPtr.Zero;

			RegisterClassEx(ref wc);

			hWnd = CreateWindowEx(
				0,
				"RenderWindowClass",
				"Render Window",
				WS_OVERLAPPEDWINDOW,
				100, 100, (int)width, (int)height,
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			if (hWnd == IntPtr.Zero)
			{
				throw new Exception("Failed to create window");
			}

			ShowWindow(hWnd, 1);
			UpdateWindow(hWnd);
		}

		private void CreateDIB()
		{
			BITMAPINFO bmi = new BITMAPINFO();
			bmi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
			bmi.bmiHeader.biWidth = FrameBuffer.Width;
			bmi.bmiHeader.biHeight = -FrameBuffer.Height;
			bmi.bmiHeader.biPlanes = 1;
			bmi.bmiHeader.biBitCount = 32;
			bmi.bmiHeader.biCompression = 0;
			bmi.bmiHeader.biSizeImage = (uint)(FrameBuffer.Width * FrameBuffer.Height * 4);
			bmi.bmiHeader.biXPelsPerMeter = 0;
			bmi.bmiHeader.biYPelsPerMeter = 0;
			bmi.bmiHeader.biClrUsed = 0;
			bmi.bmiHeader.biClrImportant = 0;

			IntPtr hdc = CreateCompatibleDC(IntPtr.Zero);
			hBitmap = CreateDIBSection(hdc, ref bmi, DIB_RGB_COLORS, out pBits, IntPtr.Zero, 0);
			hOldBitmap = SelectObject(hdc, hBitmap);

			DeleteDC(hdc);
		}

		public void Render()
		{
			IntPtr hdc = GetDC(hWnd);

			BITMAPINFO bmi = new BITMAPINFO();
			bmi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
			bmi.bmiHeader.biWidth = FrameBuffer.Width;
			bmi.bmiHeader.biHeight = -FrameBuffer.Height;
			bmi.bmiHeader.biPlanes = 1;
			bmi.bmiHeader.biBitCount = 32;
			bmi.bmiHeader.biCompression = 0;

			GCHandle handle = GCHandle.Alloc(FrameBuffer.buffer, GCHandleType.Pinned);
			IntPtr pBuffer = handle.AddrOfPinnedObject();

			StretchDIBits(hdc,
						  0, 0, (int)width, (int)height,
						  0, 0, FrameBuffer.Width, FrameBuffer.Height,
						  pBuffer, ref bmi, DIB_RGB_COLORS, SRCCOPY);

			handle.Free();

			ReleaseDC(hWnd, hdc);
		}
		[DllImport("gdi32.dll")]
		private static extern int StretchDIBits(IntPtr hdc, int XDest, int YDest, int nDestWidth, int nDestHeight, int XSrc, int YSrc, int nSrcWidth, int nSrcHeight, IntPtr lpBits, [In] ref BITMAPINFO lpBitsInfo, uint iUsage, uint dwRop);

		private const uint SRCCOPY = 0x00CC0020;
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		private static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, uint dwRop);

		[DllImport("gdi32.dll")]
		private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

		private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			switch (msg)
			{
				case WM_CLOSE:
					DestroyWindow(hWnd);
					return IntPtr.Zero;
				case WM_DESTROY:
					PostQuitMessage(0);
					return IntPtr.Zero;
				default:
					return DefWindowProc(hWnd, msg, wParam, lParam);
			}
		}

		public void Dispose()
		{
			if (hOldBitmap != IntPtr.Zero)
			{
				SelectObject(hDC, hOldBitmap);
				hOldBitmap = IntPtr.Zero;
			}

			if (hBitmap != IntPtr.Zero)
			{
				DeleteObject(hBitmap);
				hBitmap = IntPtr.Zero;
			}

			if (hDC != IntPtr.Zero)
			{
				ReleaseDC(hWnd, hDC);
				hDC = IntPtr.Zero;
			}

			if (hWnd != IntPtr.Zero)
			{
				DestroyWindow(hWnd);
				hWnd = IntPtr.Zero;
			}
		}

		[DllImport("user32.dll")]
		private static extern bool RegisterClassEx([In] ref WNDCLASSEX lpwcx);

		[DllImport("user32.dll")]
		private static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern bool UpdateWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern void PostQuitMessage(int nExitCode);

		[DllImport("user32.dll")]
		private static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool DestroyWindow(IntPtr hWnd);

		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

		[DllImport("gdi32.dll")]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		private static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		private static extern int SetDIBitsToDevice(IntPtr hdc, int XDest, int YDest, uint dwWidth, uint dwHeight, int XSrc, int YSrc, uint uStartScan, uint cScanLines, IntPtr lpvBits, [In] ref BITMAPINFO lpbmi, uint fuColorUse);

		[StructLayout(LayoutKind.Sequential)]
		struct WNDCLASSEX
		{
			public uint cbSize;
			public uint style;
			public IntPtr lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			[MarshalAs(UnmanagedType.LPStr)]
			public string lpszMenuName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string lpszClassName;
			public IntPtr hIconSm;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct BITMAPINFOHEADER
		{
			public uint biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public uint biCompression;
			public uint biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct BITMAPINFO
		{
			public BITMAPINFOHEADER bmiHeader;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public uint[] bmiColors;
		}
	}
}

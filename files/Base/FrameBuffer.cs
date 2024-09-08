using System.Drawing;
using System.Numerics;
using System.Text;

namespace ConsoleEngine
{
	public static class FrameBuffer // frame buffer for rendering
	{
		public static int[] buffer;
		public static int Width;
		public static int Height;

		public static void Initialize(int width, int height)
		{
			Width = width;
			Height = height;
			buffer = new int[width * height];
		}

		public static void Clear(int color = -1)
		{
			Array.Fill(buffer, color);
		}

		public static void SetPixel(int x, int y, int color)
		{
			if (x >= 0 && x < Width && y >= 0 && y < Height)
			{
				buffer[y * Width + x] = color;
			}
		}
		public static int GetPixel(int x, int y)
		{
			if (x >= 0 && x < Width && y >= 0 && y < Height)
			{
				return buffer[y * Width + x];
			}
			else
			{
				return -1;
			}
		}

		public static void DrawLine(int x1, int y1, int x2, int y2, int color)
		{
			int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
			int dy = -Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
			int err = dx + dy, e2;

			while (true)
			{
				SetPixel(x1, y1, color);
				if (x1 == x2 && y1 == y2) break;
				e2 = 2 * err;
				if (e2 >= dy) { err += dy; x1 += sx; }
				if (e2 <= dx) { err += dx; y1 += sy; }
			}
		}

		public static void DrawPolygon(List<Vector2> points, bool isFilled, int color)
		{
			if (points.Count < 3) return;

			for (int i = 0; i < points.Count; i++)
			{
				Vector2 p1 = points[i];
				Vector2 p2 = points[(i + 1) % points.Count];
				DrawLine((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, color);
			}

			if (isFilled)
			{
				FillPolygon(points, color);
			}
		}

		private static void FillPolygon(List<Vector2> points, int color)
		{
			int minY = (int)points.Min(p => p.Y);
			int maxY = (int)points.Max(p => p.Y);

			for (int y = minY; y <= maxY; y++)
			{
				List<int> nodeX = new List<int>();
				int j = points.Count - 1;
				for (int i = 0; i < points.Count; i++)
				{
					if ((points[i].Y < y && points[j].Y >= y) || (points[j].Y < y && points[i].Y >= y))
					{
						nodeX.Add((int)(points[i].X + (y - points[i].Y) / (points[j].Y - points[i].Y) * (points[j].X - points[i].X)));
					}
					j = i;
				}

				nodeX.Sort();
				for (int i = 0; i < nodeX.Count; i += 2)
				{
					if (nodeX[i] >= Width) break;
					if (nodeX[i + 1] > 0)
					{
						nodeX[i] = Math.Max(0, nodeX[i]);
						nodeX[i + 1] = Math.Min(Width - 1, nodeX[i + 1]);
						for (int x = nodeX[i]; x <= nodeX[i + 1]; x++)
						{
							SetPixel(x, y, color);
						}
					}
				}
			}
		}
	}
}

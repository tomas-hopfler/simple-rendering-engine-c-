using System.Runtime.InteropServices;

namespace ConsoleEngine;

public class Program
{
	[DllImport("kernel32.dll")]
	static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	// Program variables
	static bool isRunning = true;
	static Scene scene = new Scene();
	static Input input = new Input(scene); // mouse-keyboard input (q for exit)

	// Other Variables
	static float angle = (float)Math.PI / 50;
	static Mesh mesh = MeshLoader.Import("Drive:\\file.obj"); // load .obj file

	private static void Start() // called at beginning
	{
		scene.Add(mesh); // add mesh into scene to render it

		// do something
		mesh.Simplify(0.5f); // simplify mesh by 50%
		mesh.Scale(0.1f, 0.1f, 0.1f);
		mesh.Move(0, -10, 40);
		mesh.Rotate(0, 45, 0);
		mesh.SetColor(100,100,100);
	}

	private static void Update() // called each frame
	{
		mesh.Rotate(0,angle,0);
	}

	public static void Main(string[] args)
	{
		//-------------------------- for rendering inside console
		//int fx = 150;
		//int fy = 100;
		//scene.Camera.SetResolution(fx , fy);
		//FrameBuffer.Initialize(fx, fy);
		//ConsoleDisplay condis = new ConsoleDisplay(fx, fy);

		//-------------------------- for rendering inside window
		int wdx = 800;
		int wdy = 600;
		scene.Camera.SetResolution(wdx, wdy);
		FrameBuffer.Initialize(wdx, wdy);
		WindowsDisplay windis = new WindowsDisplay((uint)wdx * 1, (uint)wdy * 1);

		Start();

		while (isRunning)
		{
			isRunning = input.isRunning; // end loop by pressing "q"

			Update();

			//-------------------------- for rendering inside console
			//scene.Render(condis);

			//-------------------------- for rendering inside window
			scene.Render(windis);

			Thread.Sleep(1); // delay between frames
		}
	}
}
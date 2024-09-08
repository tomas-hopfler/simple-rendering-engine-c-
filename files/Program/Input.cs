
using System.Runtime.InteropServices;
using System.Numerics;

namespace ConsoleEngine
{
	public class Input
	{
		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		public bool isRunning = true;
		public float MovementSpeed = 1.5f;
		public float RotationSpeed = 0.3f;
		public Scene scene;

		public Input(Scene scene)
		{
			this.scene = scene;

			Thread inputThread = new Thread(HandleInput);
			inputThread.Start();
		}

		private void HandleInput()
		{
			while (isRunning)
			{
				CheckKeyboardInput();
				CheckMouseInput();
				Thread.Sleep(1);
			}
		}

		private void CheckKeyboardInput()
		{
			if (IsKeyPressed(0x51)) // Q
			{
				isRunning= false;
			}
			if (IsKeyPressed(0x57)) // W
			{
				scene.Camera.Move(new Vector3(0, 0, MovementSpeed));
			}
			if (IsKeyPressed(0x53)) // S
			{
				scene.Camera.Move(new Vector3(0, 0, -MovementSpeed));
			}
			if (IsKeyPressed(0x41)) // A
			{
				scene.Camera.Move(new Vector3(-MovementSpeed, 0, 0));
			}
			if (IsKeyPressed(0x44)) // D
			{
				scene.Camera.Move(new Vector3(MovementSpeed, 0, 0));
			}
			if (IsKeyPressed(0x26)) // Up Arrow
			{
				scene.Camera.Rotate(new Vector3(-RotationSpeed, 0, 0));
			}
			if (IsKeyPressed(0x28)) // Down Arrow
			{
				scene.Camera.Rotate(new Vector3(RotationSpeed, 0, 0));
			}
			if (IsKeyPressed(0x25))  // Left Arrow
			{
				scene.Camera.Rotate(new Vector3(0, RotationSpeed, 0));
			}
			if (IsKeyPressed(0x27)) // Right Arrow
			{
				scene.Camera.Rotate(new Vector3(0, -RotationSpeed, 0));
			}
		}
		private static bool IsKeyPressed(int key)
		{
			return (GetAsyncKeyState(key) & 0x8000) != 0;
		}

		private void CheckMouseInput()
		{
			(int deltaX, int deltaY) = MouseInput.GetMouseDelta();
			if (deltaX != 0 || deltaY != 0)
			{
				scene.Camera.Rotate(new Vector3(deltaY * RotationSpeed, -deltaX * RotationSpeed, 0));
			}
		}

		public class MouseInput
		{
			[DllImport("user32.dll")]
			private static extern bool GetCursorPos(out POINT lpPoint);

			[DllImport("user32.dll")]
			private static extern bool SetCursorPos(int X, int Y);

			[StructLayout(LayoutKind.Sequential)]
			private struct POINT
			{
				public int X;
				public int Y;
			}

			private static POINT previousMousePosition;
			private static bool isFirstMouseMove = true;

			public static (int deltaX, int deltaY) GetMouseDelta()
			{
				POINT currentMousePosition;
				if (GetCursorPos(out currentMousePosition))
				{
					if (isFirstMouseMove)
					{
						isFirstMouseMove = false;
						previousMousePosition = currentMousePosition;
						return (0, 0);
					}

					int deltaX = currentMousePosition.X - previousMousePosition.X;
					int deltaY = currentMousePosition.Y - previousMousePosition.Y;

					SetCursorPos(1920 / 2, 1080 / 2);
					previousMousePosition = new POINT { X = 1920 / 2, Y = 1080 / 2 };

					return (deltaX, deltaY);
				}
				return (0, 0);
			}
		}
	}
}

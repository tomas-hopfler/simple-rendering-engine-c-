using System.Numerics;

namespace ConsoleEngine
{
	public class Camera
	{
		public double FOV { get; set; }
		public Vector3 Position { get; private set; }
		public Vector3 Rotation { get; private set; }
		public double AspectRatio { get; set; }
		public int ScreenWidth { get; set; }
		public int ScreenHeight { get; set; }
		public double ViewDistance { get; set; }

		public int type = 0;

		public Camera(Vector3 position, Vector3 rotation, int screenWidth, int screenHeight, double fov = 60)
		{
			FOV = fov;
			Position = position;
			Rotation = rotation;
			SetResolution(screenWidth, screenHeight);
			ViewDistance = 1000;
		}

		public void SetResolution(int screenWidth, int screenHeight)
		{
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;
			AspectRatio = (double)screenWidth / screenHeight;
		}

		public void Move(Vector3 translation)
		{
			float cosY = (float)Math.Cos(Rotation.Y * Math.PI / 180.0);
			float sinY = (float)Math.Sin(Rotation.Y * Math.PI / 180.0);

			Position += new Vector3(
				translation.X * cosY - translation.Z * sinY,
				translation.Y,
				translation.X * sinY + translation.Z * cosY
			);
		}

		public void Rotate(Vector3 rotation)
		{
			// lock (-90 , 90)
			Vector3 newRotation = Rotation;
			newRotation.X = Math.Max(-90, Math.Min(90, newRotation.X + rotation.X));

			newRotation.Y += rotation.Y;
			newRotation.Z += rotation.Z;

			// normalize to 360
			newRotation.Y = (newRotation.Y + 360) % 360;

			Rotation = newRotation;
		}

		public bool IsVertexVisible(Vertex vertex)
		{
			Vertex temp = Project(vertex);
			Vector3 vc = new Vector3(temp.Position.X, temp.Position.Y,temp.Position.Z);
			Vector3? projected = vc;
			if (!projected.HasValue) return false;

			return projected.Value.X >= 0 && projected.Value.X < ScreenWidth &&
				   projected.Value.Y >= 0 && projected.Value.Y < ScreenHeight;
		}

		public Vertex? Project(Vertex v) // project vertex from 3d to 2d
		{
			if (Vector3.Distance(Position, v.Position) < ViewDistance)
			{
				Vector3 cameraSpace = TransformToCameraSpace(v.Position);

				// check if in front of camera
				if (cameraSpace.Z <= 0) return null;

				// perspective projection
				float tanHalfFOV = (float)Math.Tan(FOV * 0.5 * Math.PI / 180.0);
				float x = cameraSpace.X / (cameraSpace.Z * tanHalfFOV * (float)AspectRatio);
				float y = cameraSpace.Y / (cameraSpace.Z * tanHalfFOV);

				x = x * 0.5f + 0.5f;
				y = -y * 0.5f + 0.5f;

				x *= ScreenWidth;
				y *= ScreenHeight;

				return new Vertex(x, y, cameraSpace.Z);
			}
			return null;
		}

		public Vector3 TransformToCameraSpace(Vector3 v)
		{
			Vector3 translated = v - Position;

			float cosY = (float)Math.Cos(-Rotation.Y * Math.PI / 180.0);
			float sinY = (float)Math.Sin(-Rotation.Y * Math.PI / 180.0);
			float cosX = (float)Math.Cos(-Rotation.X * Math.PI / 180.0);
			float sinX = (float)Math.Sin(-Rotation.X * Math.PI / 180.0);
			float cosZ = (float)Math.Cos(-Rotation.Z * Math.PI / 180.0);
			float sinZ = (float)Math.Sin(-Rotation.Z * Math.PI / 180.0);

			float rotatedX = translated.X * cosY - translated.Z * sinY;
			float rotatedZ = translated.X * sinY + translated.Z * cosY;

			float finalY = translated.Y * cosX - rotatedZ * sinX;
			float finalZ = translated.Y * sinX + rotatedZ * cosX;

			float finalX = rotatedX * cosZ - finalY * sinZ;
			finalY = rotatedX * sinZ + finalY * cosZ;

			return new Vector3(finalX, finalY, finalZ);
		}

		public Vector3 CameraToWorldMovement(Vector3 cameraSpaceMovement)
		{
			float cosY = (float)Math.Cos(Rotation.Y * Math.PI / 180.0);
			float sinY = (float)Math.Sin(Rotation.Y * Math.PI / 180.0);

			return new Vector3(
				cameraSpaceMovement.X * cosY - cameraSpaceMovement.Z * sinY,
				cameraSpaceMovement.Y,
				cameraSpaceMovement.X * sinY + cameraSpaceMovement.Z * cosY
			);
		}
	}
}

using System.Drawing;

namespace ConsoleEngine
{
    public class Pyramid : Mesh
    {
        public Pyramid(float size, float x = 0, float y = 0, float z = 0)
        {
			float halfWidth = size / 2;
			float halfDepth = size / 2;

            Vertices.Add(new Vertex(-halfWidth, 0, -halfDepth)); // bottom left
            Vertices.Add(new Vertex(halfWidth, 0, -halfDepth));  // bottom right
            Vertices.Add(new Vertex(halfWidth, 0, halfDepth));   // top right
            Vertices.Add(new Vertex(-halfWidth, 0, halfDepth));  // top left

            Vertices.Add(new Vertex(0, size, 0));


			Faces.Add(new Face(new List<Vertex> { Vertices[0], Vertices[1], Vertices[2], Vertices[3] }));
			Faces.Add(new Face(new List<Vertex> { Vertices[0], Vertices[1], Vertices[4] })); // front
			Faces.Add(new Face(new List<Vertex> { Vertices[1], Vertices[2], Vertices[4] })); // right
			Faces.Add(new Face(new List<Vertex> { Vertices[2], Vertices[3], Vertices[4] })); // back
			Faces.Add(new Face(new List<Vertex> { Vertices[3], Vertices[0], Vertices[4] })); // left

			Move(x, y, z);
			Scale(size, size, size);
		}
	}
}

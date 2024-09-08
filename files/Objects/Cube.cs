using ConsoleEngine;
using System.Numerics;
using System.Security.Cryptography;

namespace ConsoleEngine
{
	public class Cube : Mesh
	{
		public Cube(float size, float x = 0, float y = 0, float z = 0)
		{
			float halfSize = size / 2;

			Vertices.Add(new Vertex(-halfSize, -halfSize, -halfSize)); // 0
			Vertices.Add(new Vertex(halfSize, -halfSize, -halfSize));  // 1
			Vertices.Add(new Vertex(halfSize, halfSize, -halfSize));   // 2
			Vertices.Add(new Vertex(-halfSize, halfSize, -halfSize));  // 3
			Vertices.Add(new Vertex(-halfSize, -halfSize, halfSize));  // 4
			Vertices.Add(new Vertex(halfSize, -halfSize, halfSize));   // 5
			Vertices.Add(new Vertex(halfSize, halfSize, halfSize));    // 6
			Vertices.Add(new Vertex(-halfSize, halfSize, halfSize));   // 7

			Faces.Add(new Face(new List<Vertex> { Vertices[0], Vertices[1], Vertices[2], Vertices[3] })); // front
			Faces.Add(new Face(new List<Vertex> { Vertices[5], Vertices[4], Vertices[7], Vertices[6] })); // back
			Faces.Add(new Face(new List<Vertex> { Vertices[4], Vertices[0], Vertices[3], Vertices[7] })); // left
			Faces.Add(new Face(new List<Vertex> { Vertices[1], Vertices[5], Vertices[6], Vertices[2] })); // right
			Faces.Add(new Face(new List<Vertex> { Vertices[3], Vertices[2], Vertices[6], Vertices[7] })); // top
			Faces.Add(new Face(new List<Vertex> { Vertices[4], Vertices[5], Vertices[1], Vertices[0] })); // bottom

			Move(x, y, z);
			Scale(size, size, size);
		}
	}
}

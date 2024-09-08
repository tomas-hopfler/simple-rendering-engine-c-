namespace ConsoleEngine
{
    public class Prism : Mesh
    {
        public Prism(float width, float height, float depth, float x = 0, float y = 0, float z = 0)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;
            float halfDepth = depth / 2;

            Vertices.Add(new Vertex(-halfWidth, -halfHeight, -halfDepth)); // 0: front bottom left
            Vertices.Add(new Vertex(halfWidth, -halfHeight, -halfDepth));  // 1: front bottom right
            Vertices.Add(new Vertex(halfWidth, halfHeight, -halfDepth));   // 2: front top right
            Vertices.Add(new Vertex(-halfWidth, halfHeight, -halfDepth));  // 3: front top left
            Vertices.Add(new Vertex(-halfWidth, -halfHeight, halfDepth));  // 4: back bottom left
            Vertices.Add(new Vertex(halfWidth, -halfHeight, halfDepth));   // 5: back bottom right
            Vertices.Add(new Vertex(halfWidth, halfHeight, halfDepth));    // 6: back top right
            Vertices.Add(new Vertex(-halfWidth, halfHeight, halfDepth));   // 7: back top left

			Faces.Add(new Face(new List<Vertex> { Vertices[0], Vertices[1], Vertices[2], Vertices[3] })); // front
			Faces.Add(new Face(new List<Vertex> { Vertices[5], Vertices[4], Vertices[7], Vertices[6] })); // back
			Faces.Add(new Face(new List<Vertex> { Vertices[4], Vertices[0], Vertices[3], Vertices[7] })); // left
			Faces.Add(new Face(new List<Vertex> { Vertices[1], Vertices[5], Vertices[6], Vertices[2] })); // right
			Faces.Add(new Face(new List<Vertex> { Vertices[3], Vertices[2], Vertices[6], Vertices[7] })); // top
			Faces.Add(new Face(new List<Vertex> { Vertices[4], Vertices[5], Vertices[1], Vertices[0] })); // bottom

			Move(x, y, z);
        }
    }
}

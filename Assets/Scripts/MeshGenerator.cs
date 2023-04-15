using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	[SerializeField] private GameObject colliderPrefab;
    [SerializeField] private float zPos, thickness = 0.2f, zScale = 1f;

    private List<Vector2> _positions = new List<Vector2>();
    private float _currentPosX, _currentPosY;
	private bool _hasUnsubscribed;
	
    private MeshFilter _meshFilter;

    private Mesh _mesh;
    private List<Vector3> _vertices;
    private List<int> _tris;

	private void OnEnable()
    {
		GameEvents.Singleton.addPointToPath += OnAddPoint;
		GameEvents.Singleton.endPath += OnEndPath;
	}
    
    private void OnDisable()
    {
		if(_hasUnsubscribed) return;
		GameEvents.Singleton.addPointToPath -= OnAddPoint;
		GameEvents.Singleton.endPath -= OnEndPath;
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

	private void OnAddPoint(Vector2 newPosition)
    {
		_positions.Add(newPosition);
        _meshFilter.mesh = GenerateMesh();
    }
    
    private void OnEndPath()
    {
		if(_positions.Count < 2)
			Destroy(gameObject);
		
		OnDisable();
		_hasUnsubscribed = true;
	}

	private Mesh GenerateMesh()
	{
		if (_positions.Count < 2) return null;

		var currentCollisionMesh = new Mesh {name = $"Custom {_positions.Count - 1}"};
		var collisionVerts = new List<Vector3>();

		if (!_meshFilter.sharedMesh)
        {
            _mesh = new Mesh {name = "Bridge"};
            _vertices = new List<Vector3>();
        }
        else
        {
            //remove last quad verts and tris
            _vertices.RemoveRange(_vertices.Count - 4, 4);
        }

		var faceLoops = GenerateFaceLoopVerts(_positions.Count - 1);
        _vertices.AddRange(faceLoops);
		collisionVerts.AddRange(faceLoops);

		if(_positions.Count - 1 < 2) //insert starting face
		{
			var startingFace = GenerateStartFace();
			_vertices.InsertRange(0, startingFace);
			collisionVerts.InsertRange(0, startingFace);
		}

        //insert end face
        _vertices.AddRange(GenerateEndFace());
        
        //calculate triangles
        _tris = GenerateAllTris(_vertices.Count);

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _tris.ToArray();

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
		
		currentCollisionMesh.vertices = collisionVerts.ToArray();
		currentCollisionMesh.triangles = GenerateAllTris(collisionVerts.Count).ToArray();
		//recalculating these because collision meshes have one sided faces,
		//in -> out = no collision.
		//out -> in = collision
		currentCollisionMesh.RecalculateNormals();
		currentCollisionMesh.RecalculateBounds();
        
		//TODO: OBJECT POOLING
        Instantiate(colliderPrefab, transform).GetComponent<MeshCollider>().sharedMesh = currentCollisionMesh;
		return _mesh;
    }

    private List<Vector3> GenerateFaceLoopVerts(int currentIndex)
    {
        var list = new List<Vector3>();
		var haveToCalculate = currentIndex < 2;
		
        Vector3 currentPoint = new Vector3(_positions[currentIndex].x, _positions[currentIndex].y, zPos - zScale / 2);
        Vector3 prevPoint = new Vector3(_positions[currentIndex - 1].x, _positions[currentIndex - 1].y, zPos - zScale / 2);
        
        //front => x, y
        var zVal = zPos - zScale / 2;
        var direction = currentPoint - prevPoint;
        var norm = NormalVector(direction.normalized);
        var normal = new Vector3(norm.x, norm.y, 0f);

        Vector3 bottomLeft, topLeft;
        if(haveToCalculate)
        {
            bottomLeft = prevPoint - normal * thickness / 2;
            list.Add(new Vector3(bottomLeft.x, bottomLeft.y, zVal));
			
			if(currentIndex > 1)
			{
				topLeft = prevPoint + normal * thickness / 2;
				list.Add(new Vector3(topLeft.x, topLeft.y, zVal));
			}
			else
				list.Add(new Vector3(bottomLeft.x, bottomLeft.y, zVal));
        }
        else
		{
			bottomLeft = _vertices[_vertices.Count - 16 + 2];
            topLeft = _vertices[_vertices.Count - 16 + 3];
            list.Add(bottomLeft);
            list.Add(topLeft);
        }
        
        var bottomRight = currentPoint - normal * thickness / 2;
        var topRight = currentPoint + normal * thickness / 2;

        list.Add(new Vector3(bottomRight.x, bottomRight.y, zVal));
        list.Add(new Vector3(topRight.x, topRight.y, zVal));
        
        //back => -x, -y
        zVal = zPos + zScale / 2;
        direction = prevPoint - currentPoint;
        norm = NormalVector(direction.normalized);
        normal = new Vector3(norm.x, norm.y, 0f);
        
        if(haveToCalculate)
        {
			topLeft = prevPoint + normal * thickness / 2;
			list.Add(new Vector3(topLeft.x, topLeft.y, zVal));

			if (currentIndex > 1)
			{
				topLeft = prevPoint + normal * thickness / 2;
				list.Add(new Vector3(topLeft.x, topLeft.y, zVal));
			}
			else
				list.Add(new Vector3(bottomLeft.x, bottomLeft.y, zVal));
		}
        else
        {
            bottomLeft = _vertices[_vertices.Count - 16 + 7];
            topLeft = _vertices[_vertices.Count - 16 + 6];
			list.Add(topLeft);
			list.Add(bottomLeft);
        }

        bottomRight = currentPoint - normal * thickness / 2;
        topRight = currentPoint + normal * thickness / 2;
        
        list.Add(new Vector3(bottomRight.x, bottomRight.y, zVal));
        list.Add(new Vector3(topRight.x, topRight.y, zVal));

		//top => duplication
        list.Add(list[1]);
        list.Add(list[4]);
        list.Add(list[3]);
        list.Add(list[6]);
		
        //bottom => duplication
        list.Add(list[5]);
        list.Add(list[0]);
        list.Add(list[7]);
        list.Add(list[2]);
        
        return list;
    }

    private Vector2 NormalVector(Vector2 unit)
    {
        return new Vector2(-unit.y, unit.x);
    }

    private List<Vector3> GenerateStartFace()
    {
        return new List<Vector3>()
        {
            //left
            _vertices[5],
            _vertices[4],
            _vertices[0],
            _vertices[1],
        };
    }

    private List<Vector3> GenerateEndFace()
    {
        return new List<Vector3>()
        {
            //right
            _vertices[_vertices.Count - 16 + 2],
            _vertices[_vertices.Count - 16 + 3],
            _vertices[_vertices.Count - 16 + 7],
            _vertices[_vertices.Count - 16 + 6]
        };
    }

    private List<int> GenerateAllTris(int total)
    {
        var result = new List<int>();
        var i = 0;
        while (i < total)
        {
            result.Add(i);
            result.Add(i + 1);
            result.Add(i + 2);
            
            result.Add(i + 1);
            result.Add(i + 3);
            result.Add(i + 2);
            
            i += 4;
        }

        return result;
    }
}

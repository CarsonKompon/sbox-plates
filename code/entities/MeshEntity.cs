using Sandbox;

public partial class MeshEntity : ModelEntity
{
	[Net] public string ModelString { get; set; }
	[Net] public string MaterialOverride { get; set; } = "";
	public Model VertexModel => VertexMeshBuilder.Models[ModelString];

	[Net, Predicted] public Vector3 scale {get;set;} = new Vector3(1f, 1f, 0.03f);
	[Net, Predicted] public Vector3 toScale {get;set;} = new Vector3(1f, 1f, 0.03f);
	public PhysicsMotionType motionType = PhysicsMotionType.Static;
    private string material = "materials/plate.vmat";
	private string surface = "normal";
	private string _lastModel;
	private string _lastMaterial;

	public override void Spawn()
	{
		ConstructModel();
	}

	[Sandbox.Event.Tick]
	public virtual void Tick()
	{
		if (!VertexMeshBuilder.Models.ContainsKey(ModelString))
		{
			return; // happens after a hot reload :()
		}
		if (ModelString != "" && ModelString != _lastModel)
		{
			Model = VertexModel;
			SetupPhysicsFromModel(motionType);

			_lastModel = ModelString;
			_lastMaterial = "";
		}
		if (IsClient && MaterialOverride != null && MaterialOverride != "" && _lastMaterial != MaterialOverride)
		{
			SceneObject.SetMaterialOverride(Material.Load(MaterialOverride));
			_lastMaterial = MaterialOverride;
		}
	}

    public void SetSurface(string newSurface)
    {
        surface = newSurface;
        ConstructModel();
    }

	public void SetMaterial(string newMaterial)
    {
		material = newMaterial;
        ConstructModel();
    }

	public void ConstructModel()
	{
		ModelString = VertexMeshBuilder.GenerateRectangleServer((int)MathX.Floor(scale.x*200), (int)MathX.Floor(scale.y*200), (int)MathX.Floor(scale.z*200), material, surface);
		Model = VertexModel;
		SetupPhysicsFromModel(motionType);
	}
}
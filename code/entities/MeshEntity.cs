namespace Sandbox
{
	public partial class MeshEntity : Prop
	{
		[Net]
		public string model { get; set; }
		[Net]
		public string MaterialOverride { get; set; } = "";
		public Model VertexModel => VertexMeshBuilder.Models[model];

		private string _lastModel;
		private string _lastMaterial;

		[Event.Tick]
		public void Tick()
		{
			if ( !VertexMeshBuilder.Models.ContainsKey( model ) ) {
				return; // happens after a hot reload :()
			}
			if ( model != "" && model != _lastModel ) {
				SetModel( VertexModel.Name );
				SetupPhysicsFromModel( PhysicsMotionType.Static );

				_lastModel = model;
				_lastMaterial = "";
			}
			if ( IsClient && MaterialOverride != null && MaterialOverride != "" && _lastMaterial != MaterialOverride ) {
				SceneObject.SetMaterialOverride( Material.Load( MaterialOverride ) );
				_lastMaterial = MaterialOverride;
			}
		}
	}
}

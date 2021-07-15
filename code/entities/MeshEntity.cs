namespace Sandbox
{
	public partial class MeshEntity : Prop
	{
		[Net]
		public string Model { get; set; }
		[Net]
		public string MaterialOverride { get; set; } = "";
		public Model VertexModel => VertexMeshBuilder.Models[Model];

		private string _lastModel;
		private string _lastMaterial;

		[Event.Tick]
		public void Tick()
		{
			if ( !VertexMeshBuilder.Models.ContainsKey( Model ) ) {
				return; // happens after a hot reload :()
			}
			if ( Model != "" && Model != _lastModel ) {
				SetModel( VertexModel );
				SetupPhysicsFromModel( PhysicsMotionType.Static );

				_lastModel = Model;
				_lastMaterial = "";
			}
			if ( IsClient && MaterialOverride != null && MaterialOverride != "" && _lastMaterial != MaterialOverride ) {
				SceneObject.SetMaterialOverride( Material.Load( MaterialOverride ) );
				_lastMaterial = MaterialOverride;
			}
		}
	}
}
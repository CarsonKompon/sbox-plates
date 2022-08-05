// namespace Sandbox;
// public partial class MeshEntity : ModelEntity
// {
//     [Net]
//     public string model { get; set; }
//     [Net]
//     public string MaterialOverride { get; set; } = "";
//     public Model VertexModel => VertexMeshBuilder.Models[model];

//     public Vector3 scale = new Vector3(1, 1, 0.01f);
//     [Net] public Vector3 toScale {get;set;} = new Vector3(1, 1, 0.01f);

//     private string lastModel;
//     private string lastMaterial;

//     public override void Spawn(){
//         ConstructModel(scale.x, scale.y, scale.z);
//     }

//     [Event.Tick]
//     public void Tick()
//     {
//         if ( !VertexMeshBuilder.Models.ContainsKey( model ) ) {
//             ConstructModel(scale.x, scale.y, scale.z);
//             return; // happens after a hot reload :()
//         }
//         if ( model != lastModel ) {
//             Model = VertexModel;
//             SetupPhysicsFromModel( PhysicsMotionType.Static );

//             lastModel = model;
//             lastMaterial = "";
//         }
//         if ( IsClient && MaterialOverride != null && MaterialOverride != "" && lastMaterial != MaterialOverride ) {
//             SceneObject.SetMaterialOverride( Material.Load( MaterialOverride ) );
//             lastMaterial = MaterialOverride;
//         }
//     }

//     public void ConstructModel( float length = 1f, float width = 1f, float height = 0.01f, int texScale = 64 ){
//         model = VertexMeshBuilder.GenerateRectangleServer((int)MathX.Floor(length*200), (int)MathX.Floor(width*200), (int)MathX.Floor(height*200), texScale);
//         Model = VertexModel;
//         SetupPhysicsFromModel( PhysicsMotionType.Static );
//     }
// }
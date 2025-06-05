// Decompiled with JetBrains decompiler
// Type: Arcana.Misc.PlaneClipManager
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using UnityEngine;

#nullable disable
namespace Arcana.Misc
{
  public class PlaneClipManager : MonoBehaviour, ISerializationCallbackReceiver
  {
    private const string CLIP_SURFACE_SHADER = "Hidden/Clip Plane/Surface";
    private const string USE_WORLD_SPACE_PROPERTY = "_UseWorldSpace";
    private const string PLANE_VECTOR_PROPERTY = "_PlaneVector";
    private MaterialPropertyBlock _matPropBlock;
    private bool useWorldSpace = true;
    public Material material;
    public Vector3 planePoint = Vector3.zero;
    [SerializeField]
    private Vector4 serializable_planeVector;
    [SerializeField]
    private bool serializable_useWorldSpace;

    private MaterialPropertyBlock matPropBlock
    {
      get
      {
        return this._matPropBlock = this._matPropBlock == null ? new MaterialPropertyBlock() : this._matPropBlock;
      }
    }

    private Renderer meshRenderer
    {
      get
      {
        SkinnedMeshRenderer component;
        return this.TryGetComponent<SkinnedMeshRenderer>(out component) ? (Renderer) component : (Renderer) this.GetComponent<MeshRenderer>();
      }
    }

    public Vector3 planeNormal
    {
      get
      {
        Vector4 planeVector = this.GetPlaneVector();
        return new Vector3(planeVector.x, planeVector.y, planeVector.z);
      }
      set
      {
        Vector3 planePoint = this.planePoint;
        this.SetPlaneVector(new Vector3?(value));
        this.planePoint = planePoint;
      }
    }

    public Vector4 planeVector
    {
      get => this.GetPlaneVector();
      set
      {
        this.SetPlaneVector(new Vector3?(new Vector3(value.x, value.y, value.z)), new float?(value.w));
      }
    }

    private Mesh mesh
    {
      get
      {
        MeshFilter component1 = this.GetComponent<MeshFilter>();
        if ((bool) (Object) component1)
          return component1.sharedMesh;
        SkinnedMeshRenderer component2;
        return this.TryGetComponent<SkinnedMeshRenderer>(out component2) ? component2.sharedMesh : (Mesh) null;
      }
    }

    private void OnEnable() => this.ApplySerializedValues();

    private void Update()
    {
      this.UpdateClipPlane();
      this.SetPlaneVector(new Vector3?((Vector3) this.GetPlaneVector()));
      Debug.LogError((object) string.Format("\n{0}\n{1}\n{2}", (object) this.planePoint, (object) this.planeNormal, (object) this.planeVector));
    }

    private void SetPlaneVector(Vector3? normal = null, float? dist = null)
    {
      Vector4 planeVector = this.GetPlaneVector();
      normal = new Vector3?((normal ?? new Vector3(planeVector.x, planeVector.y, planeVector.z)).normalized);
      dist = new float?((float) ((double) dist ?? (double) planeVector.w));
      dist = new float?(Vector3.Dot(this.planeNormal, this.planePoint));
      this.meshRenderer.GetPropertyBlock(this.matPropBlock);
      this.matPropBlock.SetVector("_PlaneVector", new Vector4(normal.Value.x, normal.Value.y, normal.Value.z, dist.Value));
      this.meshRenderer.SetPropertyBlock(this.matPropBlock);
    }

    private Vector4 GetPlaneVector() => this.matPropBlock.GetVector("_PlaneVector");

    private void SetUseWorldSpace(bool ws)
    {
      this.meshRenderer.GetPropertyBlock(this.matPropBlock);
      this.matPropBlock.SetFloat("_UseWorldSpace", ws ? 1f : 0.0f);
      this.meshRenderer.SetPropertyBlock(this.matPropBlock);
    }

    private bool GetUseWorldSpace() => (double) this.matPropBlock.GetFloat("_UseWorldSpace") == 1.0;

    private void UpdateClipPlane()
    {
      Vector3 planeNormal = this.planeNormal;
      float w = this.planeVector.w;
      Transform transform = this.transform;
      Vector3 vector3 = transform.position + planeNormal * w - Vector3.Project(transform.position, planeNormal);
      Quaternion.LookRotation(-new Vector3(planeNormal.x, planeNormal.y, planeNormal.z));
      this.planeVector = this.planeVector;
      this.useWorldSpace = this.useWorldSpace;
    }

    private void OnDrawGizmosSelected()
    {
      if ((Object) this.mesh == (Object) null)
        return;
      Vector3 planeNormal = this.planeNormal;
      Vector3 planePoint = this.planePoint;
      if (!this.useWorldSpace)
      {
        float num = Vector3.Dot(planeNormal, this.transform.position);
        Vector3 vector3 = this.transform.position - planeNormal * num;
        Gizmos.matrix = Matrix4x4.TRS(planePoint + vector3, Quaternion.LookRotation(new Vector3(planeNormal.x, planeNormal.y, planeNormal.z)), Vector3.one);
      }
      else
        Gizmos.matrix = this.transform.localToWorldMatrix * Matrix4x4.TRS(planePoint, Quaternion.LookRotation(new Vector3(planeNormal.x, planeNormal.y, planeNormal.z)), Vector3.one);
      float num1 = this.mesh.bounds.extents.magnitude * 2f;
      Color color = Gizmos.color with { a = 0.25f };
      Gizmos.color = color;
      Gizmos.DrawCube(Vector3.forward * 0.0001f, new Vector3(1f, 1f, 0.0f) * num1);
      color.a = 1f;
      Gizmos.color = color;
      Gizmos.DrawRay(Vector3.zero, Vector3.forward);
      Gizmos.DrawWireCube(Vector3.zero, new Vector3(1f, 1f, 0.0f) * num1);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
      this.serializable_planeVector = this.planeVector;
      this.serializable_useWorldSpace = this.useWorldSpace;
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
    }

    private void ApplySerializedValues()
    {
      this.planeVector = this.serializable_planeVector;
      this.useWorldSpace = this.serializable_useWorldSpace;
    }
  }
}

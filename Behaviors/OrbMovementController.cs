// Decompiled with JetBrains decompiler
// Type: Arcana.Behaviors.OrbMovementController
// Assembly: Arcana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 65905B20-66BD-42E4-A253-6E9698453FEE
// Assembly location: C:\Users\mrbea\AppData\Local\Temp\Rar$DIa22948.21955.rartemp\Arcana.dll

using ThunderRoad;
using UnityEngine;

#nullable disable
namespace Arcana.Behaviors
{
  public class OrbMovementController : MonoBehaviour
  {
    public Item item;
    public float adjustmentSpeed = 4f;
    public float orbHeight = 0.9f;

    private void FixedUpdate()
    {
      RaycastHit raycastHit;
      if (!Physics.Raycast(this.transform.position, Vector3.down, ref raycastHit, 20f, OrbMovementController.GetRaycastMask()))
        return;
      Extensions.GetPhysicBody((Component) this.item).useGravity = false;
      float b = ((RaycastHit) ref raycastHit).point.y + this.orbHeight;
      Vector3 position = this.transform.position;
      float y = Mathf.Lerp(position.y, b, Time.deltaTime * this.adjustmentSpeed);
      Extensions.GetPhysicBody((Component) this.item).MovePosition(new Vector3(position.x, y, position.z));
    }

    private static int GetRaycastMask()
    {
      return 0 | 1 << GameManager.GetLayer((LayerName) 0) | 1 << GameManager.GetLayer((LayerName) 1) | 1 << GameManager.GetLayer((LayerName) 3) | 1 << GameManager.GetLayer((LayerName) 14) | 1 << GameManager.GetLayer((LayerName) 19) | 1 << GameManager.GetLayer((LayerName) 20);
    }
  }
}

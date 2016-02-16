// Decompiled with JetBrains decompiler
// Type: Meta.VectorMeshObject
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using Stateless;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Vectrosity;

namespace Meta
{
  [AddComponentMenu("Meta/VectorMeshObject")]
  public class VectorMeshObject : MonoBehaviour
  {
    private StateMachine<VectorMeshObject.State, VectorMeshObject.Trigger> _state;
    private List<VectorMeshObject> vectorChildren;
    private VectorMeshObject vectorParent;
    private bool nonRenderingController;
    private bool _controlChildrenRenderers;
    public bool useDefaultStyle;
    public bool preserveColors;
    public VectorMeshObject.MetaVectorStyle vectorStyle;
    private VectorLine line;
    private VectorPoints points;
    private float _fadeTime;
    private float transitionStartTime;
    public float minimumAngle;
    private VectorMeshObject.OrderingFunction _orderFunction;
    public VectorMeshObject.State initialState;
    private int childrenReady;

    public bool ready { get; private set; }

    public bool controlChildrenRenderers
    {
      get
      {
        return this._controlChildrenRenderers;
      }
      set
      {
        if (Object.op_Inequality((Object) this.vectorParent, (Object) null))
          this._controlChildrenRenderers = false;
        else
          this._controlChildrenRenderers = value;
      }
    }

    public float fadeTime
    {
      get
      {
        return this._fadeTime;
      }
      set
      {
        this._fadeTime = value;
        using (List<VectorMeshObject>.Enumerator enumerator = this.vectorChildren.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            VectorMeshObject current = enumerator.Current;
            Debug.Log((object) ((Object) ((Component) this).get_gameObject()).get_name());
            current.fadeTime = value;
          }
        }
      }
    }

    public VectorMeshObject.OrderingFunction orderFunction
    {
      get
      {
        return this._orderFunction;
      }
      set
      {
        this._orderFunction = value;
        using (List<VectorMeshObject>.Enumerator enumerator = this.vectorChildren.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.orderFunction = value;
        }
      }
    }

    public event VectorMeshObject.TellParentDelegate TellParentReady;

    public event VectorMeshObject.TellParentToChange TellParentToRemove;

    public event VectorMeshObject.TellParentToChange TellParentToAdd;

    public VectorMeshObject()
    {
      base.\u002Ector();
    }

    private void InitVectrosityElements(List<Vector3> newlines, List<Vector3> newpoints)
    {
      if (this.line != null)
      {
        this.line.StopDrawing3DAuto();
        VectorLine.Destroy(ref this.line);
      }
      this.line = new VectorLine("line " + ((Object) ((Component) this).get_gameObject()).get_name(), newlines, this.vectorStyle.lineMaterial, this.vectorStyle.lineWidth, (LineType) 1);
      this.line.set_color(this.vectorStyle.lineColor);
      this.line.set_drawEnd(0);
      this.line.set_drawTransform(((Component) this).get_transform());
      if (this.points != null)
      {
        ((VectorLine) this.points).StopDrawing3DAuto();
        VectorLine.Destroy(ref this.points);
      }
      this.points = new VectorPoints("points " + ((Object) ((Component) this).get_gameObject()).get_name(), newpoints, this.vectorStyle.pointMaterial, this.vectorStyle.pointWidth);
      ((VectorLine) this.points).set_color(this.vectorStyle.pointColor);
      ((VectorLine) this.points).set_drawEnd(0);
      ((VectorLine) this.points).set_drawTransform(((Component) this).get_transform());
    }

    private void ChangeDrawEndings(bool buildingIn = true)
    {
      float num1 = (float) this.line.get_points3().Count / this.fadeTime;
      float num2 = (float) ((VectorLine) this.points).get_points3().Count / this.fadeTime;
      float num3 = Time.get_realtimeSinceStartup() - this.transitionStartTime;
      this.line.set_drawEnd(!buildingIn ? Mathf.RoundToInt((float) this.line.get_points3().Count - num3 * num1) : Mathf.RoundToInt(num3 * num1));
      ((VectorLine) this.points).set_drawEnd(!buildingIn ? Mathf.RoundToInt((float) ((VectorLine) this.points).get_points3().Count - num3 * num2) : Mathf.RoundToInt(num3 * num2));
    }

    public void BuildToggle()
    {
      if (this._state.get_State() != VectorMeshObject.State.SHOWN && this._state.get_State() != VectorMeshObject.State.HIDDEN)
        return;
      this._state.Fire(VectorMeshObject.Trigger.BUILD);
    }

    public bool Cancel()
    {
      if (this.ready)
        return false;
      this._state.Fire(VectorMeshObject.Trigger.CANCEL);
      return true;
    }

    public bool Build()
    {
      if (!this.ready)
        return false;
      this._state.Fire(VectorMeshObject.Trigger.BUILD);
      return true;
    }

    public void BuildWhenReady()
    {
      this.StartCoroutine(this.WaitandBuildWhenReady());
    }

    [DebuggerHidden]
    private IEnumerator WaitandBuildWhenReady()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new VectorMeshObject.\u003CWaitandBuildWhenReady\u003Ec__Iterator5()
      {
        \u003C\u003Ef__this = this
      };
    }

    public VectorMeshObject.State CurrentState()
    {
      return this._state.get_State();
    }

    public bool Flip()
    {
      if (!this.ready)
        return false;
      this._state.Fire(VectorMeshObject.Trigger.FLIP);
      return true;
    }

    public void InitStateless()
    {
      this._state = new StateMachine<VectorMeshObject.State, VectorMeshObject.Trigger>(this.initialState);
      this._state.Configure(VectorMeshObject.State.HIDDEN).PermitReentry(VectorMeshObject.Trigger.REFRESH).Permit(VectorMeshObject.Trigger.BUILD, VectorMeshObject.State.BUILDING_IN).Permit(VectorMeshObject.Trigger.FLIP, VectorMeshObject.State.SHOWN).OnEntry(new Action(this.Hide));
      this._state.Configure(VectorMeshObject.State.BUILDING_IN).Permit(VectorMeshObject.Trigger.STOP, VectorMeshObject.State.SHOWN).Permit(VectorMeshObject.Trigger.CANCEL, VectorMeshObject.State.HIDDEN).OnEntry(new Action(this.BuildIn));
      this._state.Configure(VectorMeshObject.State.SHOWN).PermitReentry(VectorMeshObject.Trigger.REFRESH).Permit(VectorMeshObject.Trigger.BUILD, VectorMeshObject.State.BUILDING_OUT).Permit(VectorMeshObject.Trigger.FLIP, VectorMeshObject.State.HIDDEN).OnEntry(new Action(this.Show));
      this._state.Configure(VectorMeshObject.State.BUILDING_OUT).Permit(VectorMeshObject.Trigger.STOP, VectorMeshObject.State.HIDDEN).Permit(VectorMeshObject.Trigger.CANCEL, VectorMeshObject.State.SHOWN).OnEntry(new Action(this.BuildOut));
    }

    private void Hide()
    {
      if (!this.nonRenderingController)
      {
        this.HideOutline();
        this.SetRenderer(false);
      }
      this.ready = true;
    }

    private void Show()
    {
      if (!this.nonRenderingController)
      {
        this.ShowOutline();
        this.SetRenderer(true);
      }
      this.ready = true;
    }

    private void BuildIn()
    {
      this.StartCoroutine(this.Transition(true));
    }

    private void BuildOut()
    {
      this.StartCoroutine(this.Transition(false));
    }

    [DebuggerHidden]
    private IEnumerator Transition(bool buildingIn)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new VectorMeshObject.\u003CTransition\u003Ec__Iterator6()
      {
        buildingIn = buildingIn,
        \u003C\u0024\u003EbuildingIn = buildingIn,
        \u003C\u003Ef__this = this
      };
    }

    public void HideOutline()
    {
      this.line.StopDrawing3DAuto();
      this.line.set_active(false);
      ((VectorLine) this.points).StopDrawing3DAuto();
      ((VectorLine) this.points).set_active(false);
    }

    public void ShowOutline()
    {
      if (this.line != null)
      {
        this.line.set_active(true);
        this.line.set_drawEnd(this.line.get_points3().Count);
        this.line.Draw3DAuto();
      }
      if (this.points == null)
        return;
      ((VectorLine) this.points).set_active(true);
      ((VectorLine) this.points).set_drawEnd(((VectorLine) this.points).get_points3().Count);
      ((VectorLine) this.points).Draw3DAuto();
    }

    private void SetRenderer(bool state)
    {
      ((Renderer) ((Component) this).GetComponent<Renderer>()).set_enabled(state);
    }

    private void OnDisable()
    {
      this.Cancel();
      if (!this.nonRenderingController)
        this.HideOutline();
      if (!Object.op_Inequality((Object) this.vectorParent, (Object) null))
        return;
      this.TellParentToRemove(this);
    }

    private void OnEnable()
    {
      if (this._state.get_State() == VectorMeshObject.State.SHOWN || this._state.get_State() == VectorMeshObject.State.HIDDEN)
        this._state.Fire(VectorMeshObject.Trigger.REFRESH);
      if (!Object.op_Inequality((Object) this.vectorParent, (Object) null))
        return;
      this.TellParentToAdd(this);
    }

    private void RemoveChild(VectorMeshObject vmo)
    {
      if (vmo.ready && !this.ready)
        --this.childrenReady;
      this.vectorChildren.Remove(vmo);
    }

    private void AddChildBack(VectorMeshObject vmo)
    {
      this.vectorChildren.Add(vmo);
      if ((this._state.get_State() != VectorMeshObject.State.HIDDEN && this._state.get_State() != VectorMeshObject.State.BUILDING_OUT || vmo.CurrentState() == VectorMeshObject.State.HIDDEN) && (this._state.get_State() != VectorMeshObject.State.SHOWN && this._state.get_State() != VectorMeshObject.State.BUILDING_IN || vmo.CurrentState() == VectorMeshObject.State.SHOWN))
        return;
      vmo.Flip();
    }

    private void ChildReady()
    {
      ++this.childrenReady;
    }

    public void InitChildren()
    {
      MeshRenderer[] meshRendererArray = (MeshRenderer[]) ((Component) ((Component) this).get_transform()).GetComponentsInChildren<MeshRenderer>();
      this.vectorChildren = new List<VectorMeshObject>();
      foreach (Component component in meshRendererArray)
        this.AddChild(component.get_gameObject());
    }

    private void AddChild(GameObject go)
    {
      if (Object.op_Equality((Object) go.GetComponent<VectorMeshObject>(), (Object) null))
        go.get_gameObject().AddComponent<VectorMeshObject>();
      VectorMeshObject vectorMeshObject = (VectorMeshObject) go.GetComponent<VectorMeshObject>();
      vectorMeshObject._controlChildrenRenderers = false;
      vectorMeshObject.vectorParent = this;
      vectorMeshObject.vectorChildren = new List<VectorMeshObject>();
      vectorMeshObject.nonRenderingController = false;
      vectorMeshObject.preserveColors = this.preserveColors;
      vectorMeshObject.vectorStyle = this.vectorStyle;
      vectorMeshObject.fadeTime = this.fadeTime;
      vectorMeshObject.TellParentReady += new VectorMeshObject.TellParentDelegate(this.ChildReady);
      vectorMeshObject.TellParentToAdd += new VectorMeshObject.TellParentToChange(this.AddChildBack);
      vectorMeshObject.TellParentToRemove += new VectorMeshObject.TellParentToChange(this.RemoveChild);
      this.vectorChildren.Add(vectorMeshObject);
    }

    public void ApplyVectorStyle()
    {
      if (!this.nonRenderingController)
      {
        for (int index = 0; index < ((Renderer) ((Component) this).GetComponent<Renderer>()).get_materials().Length; ++index)
        {
          Material material = ((Renderer) ((Component) this).GetComponent<Renderer>()).get_materials()[0];
          Color color = (Color) null;
          if (this.preserveColors && material.HasProperty("_Color"))
            color = material.get_color();
          ((Renderer) ((Component) this).GetComponent<Renderer>()).get_materials()[index] = this.vectorStyle.surfaceMaterial;
          if (this.preserveColors && material.HasProperty("_Color"))
            ((Renderer) ((Component) this).GetComponent<Renderer>()).get_materials()[index].set_color(color);
        }
        ((Renderer) ((Component) this).GetComponent<Renderer>()).set_material(this.vectorStyle.surfaceMaterial);
      }
      this.vectorStyle.lineMaterial.set_color(this.vectorStyle.lineColor);
      this.vectorStyle.pointMaterial.set_color(this.vectorStyle.pointColor);
    }

    public void SetVectorStyleDefaults()
    {
      this.vectorStyle.surfaceMaterial = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.surfaceMaterial;
      this.vectorStyle.lineMaterial = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.lineMaterial;
      this.vectorStyle.pointMaterial = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.pointMaterial;
      this.vectorStyle.lineWidth = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.lineWidth;
      this.vectorStyle.pointWidth = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.pointWidth;
      this.vectorStyle.surfaceColor = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.surfaceColor;
      this.vectorStyle.lineColor = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.lineColor;
      this.vectorStyle.pointColor = MetaSingleton<MetaUI>.Instance.defaultVectorStyle.pointColor;
    }

    private void MakeFrameAndPoints(Mesh mesh, out List<Vector3> lines, out List<Vector3> points)
    {
      Dictionary<ulong, VectorMeshObject.Line> dictionary1 = new Dictionary<ulong, VectorMeshObject.Line>();
      Dictionary<int, List<VectorMeshObject.Line>> dictionary2 = new Dictionary<int, List<VectorMeshObject.Line>>();
      int[] triangles = mesh.get_triangles();
      Vector3[] vertices = mesh.get_vertices();
      Vector3[] normals = mesh.get_normals();
      int[] numArray = new int[6]
      {
        0,
        1,
        0,
        2,
        1,
        2
      };
      int index1 = 0;
      while (index1 < triangles.Length)
      {
        for (int index2 = 0; index2 < 3; ++index2)
        {
          VectorMeshObject.Line line = new VectorMeshObject.Line(vertices[triangles[index1 + numArray[index2 * 2]]], vertices[triangles[index1 + numArray[index2 * 2 + 1]]], normals[triangles[index1]]);
          if (dictionary1.ContainsKey(line.LineHashCode))
          {
            if ((double) Vector3.Angle(dictionary1[line.LineHashCode].normal, line.normal) < (double) this.minimumAngle)
              dictionary1.Remove(line.LineHashCode);
          }
          else
          {
            dictionary1[line.LineHashCode] = line;
            // ISSUE: explicit reference operation
            if (!dictionary2.ContainsKey(((Vector3) @line.direction).GetHashCode()))
            {
              // ISSUE: explicit reference operation
              dictionary2[((Vector3) @line.direction).GetHashCode()] = new List<VectorMeshObject.Line>();
            }
            // ISSUE: explicit reference operation
            dictionary2[((Vector3) @line.direction).GetHashCode()].Add(line);
          }
        }
        index1 += 3;
      }
      using (Dictionary<int, List<VectorMeshObject.Line>>.Enumerator enumerator1 = dictionary2.GetEnumerator())
      {
        while (enumerator1.MoveNext())
        {
          List<VectorMeshObject.Line> list = enumerator1.Current.Value;
          for (int index2 = list.Count - 1; index2 > -1; --index2)
          {
            using (List<VectorMeshObject.Line>.Enumerator enumerator2 = list.GetEnumerator())
            {
              while (enumerator2.MoveNext())
              {
                VectorMeshObject.Line current = enumerator2.Current;
                if (list[index2].Merge(current))
                {
                  dictionary1.Remove(current.LineHashCode);
                  // ISSUE: explicit reference operation
                  dictionary2[((Vector3) @current.direction).GetHashCode()].Remove(current);
                  break;
                }
              }
            }
          }
        }
      }
      lines = new List<Vector3>();
      points = new List<Vector3>();
      if (dictionary1.Count > 12000)
      {
        Debug.Log((object) "Ignoring Mesh as it is too big");
      }
      else
      {
        using (List<VectorMeshObject.Line>.Enumerator enumerator = (this._orderFunction != null ? Enumerable.ToList<VectorMeshObject.Line>((IEnumerable<VectorMeshObject.Line>) Enumerable.OrderBy<VectorMeshObject.Line, float>((IEnumerable<VectorMeshObject.Line>) dictionary1.Values, (Func<VectorMeshObject.Line, float>) (x => Mathf.Min(this._orderFunction(x.pointA), this._orderFunction(x.pointB))))) : Enumerable.ToList<VectorMeshObject.Line>((IEnumerable<VectorMeshObject.Line>) dictionary1.Values)).GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            VectorMeshObject.Line current = enumerator.Current;
            lines.Add(current.pointA);
            lines.Add(current.pointB);
          }
        }
        points = Enumerable.ToList<Vector3>(Enumerable.Distinct<Vector3>((IEnumerable<Vector3>) lines));
      }
    }

    private float SortByRadial(Vector3 v)
    {
      return Vector3.Distance(Vector3.get_zero(), v);
    }

    private float SortByY(Vector3 v)
    {
      return (float) v.y;
    }

    private void Start()
    {
      this.InitStateless();
      this.InitVectorMeshObject();
    }

    private void InitVectorMeshObject()
    {
      if (Object.op_Equality((Object) ((Component) this).get_gameObject().GetComponent<MeshRenderer>(), (Object) null))
        this.nonRenderingController = true;
      if (this.useDefaultStyle)
        this.SetVectorStyleDefaults();
      this.ApplyVectorStyle();
      if (this.controlChildrenRenderers)
        this.InitChildren();
      if (!this.nonRenderingController)
      {
        List<Vector3> lines;
        List<Vector3> points;
        this.MakeFrameAndPoints(((MeshFilter) ((Component) this).GetComponent<MeshFilter>()).get_mesh(), out lines, out points);
        this.InitVectrosityElements(lines, points);
      }
      if (!this.controlChildrenRenderers)
        this.ready = true;
      else
        this.StartCoroutine(this.WaitForChildrenReady());
      if (Object.op_Inequality((Object) this.vectorParent, (Object) null))
      {
        this.TellParentReady();
        this.controlChildrenRenderers = false;
      }
      this._state.Fire(VectorMeshObject.Trigger.REFRESH);
    }

    [DebuggerHidden]
    private IEnumerator WaitForChildrenReady()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new VectorMeshObject.\u003CWaitForChildrenReady\u003Ec__Iterator7()
      {
        \u003C\u003Ef__this = this
      };
    }

    public enum State
    {
      UNKNOWN,
      HIDDEN,
      BUILDING_IN,
      SHOWN,
      BUILDING_OUT,
    }

    private enum Trigger
    {
      BUILD,
      STOP,
      CANCEL,
      FLIP,
      REFRESH,
      PAUSE,
    }

    [Serializable]
    public class MetaVectorStyle
    {
      public Color surfaceColor = Color.get_cyan();
      public Color lineColor = Color.get_green();
      public Color pointColor = Color.get_red();
      public float lineWidth = 12f;
      public float pointWidth = 1f;
      public Material surfaceMaterial;
      public Material lineMaterial;
      public Material pointMaterial;
    }

    private class Line
    {
      public Vector3 pointA;
      public Vector3 pointB;
      public Vector3 direction;
      public Vector3 normal;
      public readonly ulong LineHashCode;

      public Line(Vector3 point1, Vector3 point2, Vector3 newNormal)
      {
        bool flag = false;
        if (point1.x != point2.x)
        {
          if (point1.x > point2.x)
            flag = true;
        }
        else if (point1.y != point2.y)
        {
          if (point1.y > point2.y)
            flag = true;
        }
        else if (point1.z > point2.z)
          flag = true;
        if (flag)
        {
          this.pointA = point2;
          this.pointB = point1;
        }
        else
        {
          this.pointA = point1;
          this.pointB = point2;
        }
        Vector3 vector3 = Vector3.op_Subtraction(this.pointA, this.pointB);
        // ISSUE: explicit reference operation
        this.direction = ((Vector3) @vector3).get_normalized();
        this.direction = new Vector3((float) Math.Round((double) this.direction.x, 4), (float) Math.Round((double) this.direction.y, 4), (float) Math.Round((double) this.direction.z, 4));
        this.normal = newNormal;
        uint num1 = 0;
        for (int index = 0; index < 3; ++index)
        {
          // ISSUE: explicit reference operation
          num1 = num1 >> 5 ^ (uint) ((Vector3) @this.pointA).get_Item(index).GetHashCode();
        }
        this.LineHashCode = (ulong) num1;
        this.LineHashCode = this.LineHashCode << 32;
        uint num2 = 0;
        for (int index = 0; index < 3; ++index)
        {
          // ISSUE: explicit reference operation
          num2 = num2 >> 5 ^ (uint) ((Vector3) @this.pointB).get_Item(index).GetHashCode();
        }
        this.LineHashCode = this.LineHashCode | (ulong) num2;
      }

      public override bool Equals(object obj)
      {
        VectorMeshObject.Line line = obj as VectorMeshObject.Line;
        if (line == null || !Vector3.op_Equality(line.pointA, this.pointA))
          return false;
        return Vector3.op_Equality(line.pointB, this.pointB);
      }

      public override int GetHashCode()
      {
        return (int) this.LineHashCode;
      }

      public bool Merge(VectorMeshObject.Line other)
      {
        if (Vector3.op_Inequality(other.direction, this.direction))
          return false;
        bool flag = false;
        if (!Vector3.op_Equality(this.pointA, other.pointA) || !Vector3.op_Equality(this.pointB, other.pointB))
        {
          if (Vector3.op_Equality(this.pointA, other.pointA))
          {
            this.pointA = other.pointB;
            flag = true;
          }
          else if (Vector3.op_Equality(this.pointA, other.pointB))
          {
            this.pointA = other.pointA;
            flag = true;
          }
          else if (Vector3.op_Equality(this.pointB, other.pointA))
          {
            this.pointB = other.pointB;
            flag = true;
          }
          else if (Vector3.op_Equality(this.pointB, other.pointB))
          {
            this.pointB = other.pointA;
            flag = true;
          }
        }
        return flag;
      }
    }

    public delegate void TellParentDelegate();

    public delegate void TellParentToChange(VectorMeshObject vmo);

    public delegate float OrderingFunction(Vector3 v);
  }
}

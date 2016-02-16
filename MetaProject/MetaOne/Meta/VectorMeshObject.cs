using Stateless;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Vectrosity;

namespace Meta
{
	[AddComponentMenu("Meta/VectorMeshObject")]
	public class VectorMeshObject : MonoBehaviour
	{
		public enum State
		{
			UNKNOWN,
			HIDDEN,
			BUILDING_IN,
			SHOWN,
			BUILDING_OUT
		}

		private enum Trigger
		{
			BUILD,
			STOP,
			CANCEL,
			FLIP,
			REFRESH,
			PAUSE
		}

		[Serializable]
		public class MetaVectorStyle
		{
			public Color surfaceColor = Color.get_cyan();

			public Color lineColor = Color.get_green();

			public Color pointColor = Color.get_red();

			public Material surfaceMaterial;

			public Material lineMaterial;

			public Material pointMaterial;

			public float lineWidth = 12f;

			public float pointWidth = 1f;
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
					{
						flag = true;
					}
				}
				else if (point1.y != point2.y)
				{
					if (point1.y > point2.y)
					{
						flag = true;
					}
				}
				else if (point1.z > point2.z)
				{
					flag = true;
				}
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
				this.direction = (this.pointA - this.pointB).get_normalized();
				this.direction = new Vector3((float)Math.Round((double)this.direction.x, 4), (float)Math.Round((double)this.direction.y, 4), (float)Math.Round((double)this.direction.z, 4));
				this.normal = newNormal;
				uint num = 0u;
				for (int i = 0; i < 3; i++)
				{
					num >>= 5;
					num ^= (uint)this.pointA.get_Item(i).GetHashCode();
				}
				this.LineHashCode = (ulong)num;
				this.LineHashCode <<= 32;
				num = 0u;
				for (int j = 0; j < 3; j++)
				{
					num >>= 5;
					num ^= (uint)this.pointB.get_Item(j).GetHashCode();
				}
				this.LineHashCode |= (ulong)num;
			}

			public override bool Equals(object obj)
			{
				VectorMeshObject.Line line = obj as VectorMeshObject.Line;
				return line != null && line.pointA == this.pointA && line.pointB == this.pointB;
			}

			public override int GetHashCode()
			{
				return (int)this.LineHashCode;
			}

			public bool Merge(VectorMeshObject.Line other)
			{
				if (other.direction != this.direction)
				{
					return false;
				}
				bool result = false;
				if (!(this.pointA == other.pointA) || !(this.pointB == other.pointB))
				{
					if (this.pointA == other.pointA)
					{
						this.pointA = other.pointB;
						result = true;
					}
					else if (this.pointA == other.pointB)
					{
						this.pointA = other.pointA;
						result = true;
					}
					else if (this.pointB == other.pointA)
					{
						this.pointB = other.pointB;
						result = true;
					}
					else if (this.pointB == other.pointB)
					{
						this.pointB = other.pointA;
						result = true;
					}
				}
				return result;
			}
		}

		public delegate void TellParentDelegate();

		public delegate void TellParentToChange(VectorMeshObject vmo);

		public delegate float OrderingFunction(Vector3 v);

		private StateMachine<VectorMeshObject.State, VectorMeshObject.Trigger> _state = new StateMachine<VectorMeshObject.State, VectorMeshObject.Trigger>(VectorMeshObject.State.UNKNOWN);

		private List<VectorMeshObject> vectorChildren = new List<VectorMeshObject>();

		private VectorMeshObject vectorParent;

		private bool nonRenderingController;

		private bool _controlChildrenRenderers = true;

		public bool useDefaultStyle = true;

		public bool preserveColors;

		public VectorMeshObject.MetaVectorStyle vectorStyle = new VectorMeshObject.MetaVectorStyle();

		private VectorLine line;

		private VectorPoints points;

		private float _fadeTime = 2f;

		private float transitionStartTime;

		public float minimumAngle = 5f;

		private VectorMeshObject.OrderingFunction _orderFunction;

		public VectorMeshObject.State initialState = VectorMeshObject.State.SHOWN;

		private int childrenReady;

		public event VectorMeshObject.TellParentDelegate TellParentReady
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.TellParentReady = (VectorMeshObject.TellParentDelegate)Delegate.Combine(this.TellParentReady, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.TellParentReady = (VectorMeshObject.TellParentDelegate)Delegate.Remove(this.TellParentReady, value);
			}
		}

		public event VectorMeshObject.TellParentToChange TellParentToRemove
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.TellParentToRemove = (VectorMeshObject.TellParentToChange)Delegate.Combine(this.TellParentToRemove, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.TellParentToRemove = (VectorMeshObject.TellParentToChange)Delegate.Remove(this.TellParentToRemove, value);
			}
		}

		public event VectorMeshObject.TellParentToChange TellParentToAdd
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.TellParentToAdd = (VectorMeshObject.TellParentToChange)Delegate.Combine(this.TellParentToAdd, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.TellParentToAdd = (VectorMeshObject.TellParentToChange)Delegate.Remove(this.TellParentToAdd, value);
			}
		}

		public bool ready
		{
			get;
			private set;
		}

		public bool controlChildrenRenderers
		{
			get
			{
				return this._controlChildrenRenderers;
			}
			set
			{
				if (this.vectorParent != null)
				{
					this._controlChildrenRenderers = false;
				}
				else
				{
					this._controlChildrenRenderers = value;
				}
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
				foreach (VectorMeshObject current in this.vectorChildren)
				{
					Debug.Log(base.get_gameObject().get_name());
					current.fadeTime = value;
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
				foreach (VectorMeshObject current in this.vectorChildren)
				{
					current.orderFunction = value;
				}
			}
		}

		private void InitVectrosityElements(List<Vector3> newlines, List<Vector3> newpoints)
		{
			if (this.line != null)
			{
				this.line.StopDrawing3DAuto();
				VectorLine.Destroy(ref this.line);
			}
			this.line = new VectorLine("line " + base.get_gameObject().get_name(), newlines, this.vectorStyle.lineMaterial, this.vectorStyle.lineWidth, 1);
			this.line.set_color(this.vectorStyle.lineColor);
			this.line.set_drawEnd(0);
			this.line.set_drawTransform(base.get_transform());
			if (this.points != null)
			{
				this.points.StopDrawing3DAuto();
				VectorLine.Destroy(ref this.points);
			}
			this.points = new VectorPoints("points " + base.get_gameObject().get_name(), newpoints, this.vectorStyle.pointMaterial, this.vectorStyle.pointWidth);
			this.points.set_color(this.vectorStyle.pointColor);
			this.points.set_drawEnd(0);
			this.points.set_drawTransform(base.get_transform());
		}

		private void ChangeDrawEndings(bool buildingIn = true)
		{
			float num = (float)this.line.get_points3().Count / this.fadeTime;
			float num2 = (float)this.points.get_points3().Count / this.fadeTime;
			float num3 = Time.get_realtimeSinceStartup() - this.transitionStartTime;
			this.line.set_drawEnd((!buildingIn) ? Mathf.RoundToInt((float)this.line.get_points3().Count - num3 * num) : Mathf.RoundToInt(num3 * num));
			this.points.set_drawEnd((!buildingIn) ? Mathf.RoundToInt((float)this.points.get_points3().Count - num3 * num2) : Mathf.RoundToInt(num3 * num2));
		}

		public void BuildToggle()
		{
			if (this._state.get_State() == VectorMeshObject.State.SHOWN || this._state.get_State() == VectorMeshObject.State.HIDDEN)
			{
				this._state.Fire(VectorMeshObject.Trigger.BUILD);
			}
		}

		public bool Cancel()
		{
			if (!this.ready)
			{
				this._state.Fire(VectorMeshObject.Trigger.CANCEL);
				return true;
			}
			return false;
		}

		public bool Build()
		{
			if (this.ready)
			{
				this._state.Fire(VectorMeshObject.Trigger.BUILD);
				return true;
			}
			return false;
		}

		public void BuildWhenReady()
		{
			base.StartCoroutine(this.WaitandBuildWhenReady());
		}

		[DebuggerHidden]
		private IEnumerator WaitandBuildWhenReady()
		{
			VectorMeshObject.<WaitandBuildWhenReady>c__Iterator5 <WaitandBuildWhenReady>c__Iterator = new VectorMeshObject.<WaitandBuildWhenReady>c__Iterator5();
			<WaitandBuildWhenReady>c__Iterator.<>f__this = this;
			return <WaitandBuildWhenReady>c__Iterator;
		}

		public VectorMeshObject.State CurrentState()
		{
			return this._state.get_State();
		}

		public bool Flip()
		{
			if (this.ready)
			{
				this._state.Fire(VectorMeshObject.Trigger.FLIP);
				return true;
			}
			return false;
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
			base.StartCoroutine(this.Transition(true));
		}

		private void BuildOut()
		{
			base.StartCoroutine(this.Transition(false));
		}

		[DebuggerHidden]
		private IEnumerator Transition(bool buildingIn)
		{
			VectorMeshObject.<Transition>c__Iterator6 <Transition>c__Iterator = new VectorMeshObject.<Transition>c__Iterator6();
			<Transition>c__Iterator.buildingIn = buildingIn;
			<Transition>c__Iterator.<$>buildingIn = buildingIn;
			<Transition>c__Iterator.<>f__this = this;
			return <Transition>c__Iterator;
		}

		public void HideOutline()
		{
			this.line.StopDrawing3DAuto();
			this.line.set_active(false);
			this.points.StopDrawing3DAuto();
			this.points.set_active(false);
		}

		public void ShowOutline()
		{
			if (this.line != null)
			{
				this.line.set_active(true);
				this.line.set_drawEnd(this.line.get_points3().Count);
				this.line.Draw3DAuto();
			}
			if (this.points != null)
			{
				this.points.set_active(true);
				this.points.set_drawEnd(this.points.get_points3().Count);
				this.points.Draw3DAuto();
			}
		}

		private void SetRenderer(bool state)
		{
			base.GetComponent<Renderer>().set_enabled(state);
		}

		private void OnDisable()
		{
			this.Cancel();
			if (!this.nonRenderingController)
			{
				this.HideOutline();
			}
			if (this.vectorParent != null)
			{
				this.TellParentToRemove(this);
			}
		}

		private void OnEnable()
		{
			if (this._state.get_State() == VectorMeshObject.State.SHOWN || this._state.get_State() == VectorMeshObject.State.HIDDEN)
			{
				this._state.Fire(VectorMeshObject.Trigger.REFRESH);
			}
			if (this.vectorParent != null)
			{
				this.TellParentToAdd(this);
			}
		}

		private void RemoveChild(VectorMeshObject vmo)
		{
			if (vmo.ready && !this.ready)
			{
				this.childrenReady--;
			}
			this.vectorChildren.Remove(vmo);
		}

		private void AddChildBack(VectorMeshObject vmo)
		{
			this.vectorChildren.Add(vmo);
			if (((this._state.get_State() == VectorMeshObject.State.HIDDEN || this._state.get_State() == VectorMeshObject.State.BUILDING_OUT) && vmo.CurrentState() != VectorMeshObject.State.HIDDEN) || ((this._state.get_State() == VectorMeshObject.State.SHOWN || this._state.get_State() == VectorMeshObject.State.BUILDING_IN) && vmo.CurrentState() != VectorMeshObject.State.SHOWN))
			{
				vmo.Flip();
			}
		}

		private void ChildReady()
		{
			this.childrenReady++;
		}

		public void InitChildren()
		{
			MeshRenderer[] componentsInChildren = base.get_transform().GetComponentsInChildren<MeshRenderer>();
			this.vectorChildren = new List<VectorMeshObject>();
			MeshRenderer[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshRenderer meshRenderer = array[i];
				this.AddChild(meshRenderer.get_gameObject());
			}
		}

		private void AddChild(GameObject go)
		{
			if (go.GetComponent<VectorMeshObject>() == null)
			{
				go.get_gameObject().AddComponent<VectorMeshObject>();
			}
			VectorMeshObject component = go.GetComponent<VectorMeshObject>();
			component._controlChildrenRenderers = false;
			component.vectorParent = this;
			component.vectorChildren = new List<VectorMeshObject>();
			component.nonRenderingController = false;
			component.preserveColors = this.preserveColors;
			component.vectorStyle = this.vectorStyle;
			component.fadeTime = this.fadeTime;
			VectorMeshObject expr_69 = component;
			expr_69.TellParentReady = (VectorMeshObject.TellParentDelegate)Delegate.Combine(expr_69.TellParentReady, new VectorMeshObject.TellParentDelegate(this.ChildReady));
			VectorMeshObject expr_8B = component;
			expr_8B.TellParentToAdd = (VectorMeshObject.TellParentToChange)Delegate.Combine(expr_8B.TellParentToAdd, new VectorMeshObject.TellParentToChange(this.AddChildBack));
			VectorMeshObject expr_AD = component;
			expr_AD.TellParentToRemove = (VectorMeshObject.TellParentToChange)Delegate.Combine(expr_AD.TellParentToRemove, new VectorMeshObject.TellParentToChange(this.RemoveChild));
			this.vectorChildren.Add(component);
		}

		public void ApplyVectorStyle()
		{
			if (!this.nonRenderingController)
			{
				for (int i = 0; i < base.GetComponent<Renderer>().get_materials().Length; i++)
				{
					Material material = base.GetComponent<Renderer>().get_materials()[0];
					Color color = default(Color);
					if (this.preserveColors && material.HasProperty("_Color"))
					{
						color = material.get_color();
					}
					base.GetComponent<Renderer>().get_materials()[i] = this.vectorStyle.surfaceMaterial;
					if (this.preserveColors && material.HasProperty("_Color"))
					{
						base.GetComponent<Renderer>().get_materials()[i].set_color(color);
					}
				}
				base.GetComponent<Renderer>().set_material(this.vectorStyle.surfaceMaterial);
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
			Dictionary<ulong, VectorMeshObject.Line> dictionary = new Dictionary<ulong, VectorMeshObject.Line>();
			Dictionary<int, List<VectorMeshObject.Line>> dictionary2 = new Dictionary<int, List<VectorMeshObject.Line>>();
			int[] triangles = mesh.get_triangles();
			Vector3[] vertices = mesh.get_vertices();
			Vector3[] normals = mesh.get_normals();
			int[] array = new int[]
			{
				0,
				1,
				0,
				2,
				1,
				2
			};
			for (int i = 0; i < triangles.Length; i += 3)
			{
				for (int j = 0; j < 3; j++)
				{
					VectorMeshObject.Line line = new VectorMeshObject.Line(vertices[triangles[i + array[j * 2]]], vertices[triangles[i + array[j * 2 + 1]]], normals[triangles[i]]);
					if (dictionary.ContainsKey(line.LineHashCode))
					{
						if (Vector3.Angle(dictionary[line.LineHashCode].normal, line.normal) < this.minimumAngle)
						{
							dictionary.Remove(line.LineHashCode);
						}
					}
					else
					{
						dictionary[line.LineHashCode] = line;
						if (!dictionary2.ContainsKey(line.direction.GetHashCode()))
						{
							dictionary2[line.direction.GetHashCode()] = new List<VectorMeshObject.Line>();
						}
						dictionary2[line.direction.GetHashCode()].Add(line);
					}
				}
			}
			foreach (KeyValuePair<int, List<VectorMeshObject.Line>> current in dictionary2)
			{
				List<VectorMeshObject.Line> value = current.Value;
				for (int k = value.Count - 1; k > -1; k--)
				{
					foreach (VectorMeshObject.Line current2 in value)
					{
						if (value[k].Merge(current2))
						{
							dictionary.Remove(current2.LineHashCode);
							dictionary2[current2.direction.GetHashCode()].Remove(current2);
							break;
						}
					}
				}
			}
			lines = new List<Vector3>();
			points = new List<Vector3>();
			if (dictionary.Count > 12000)
			{
				Debug.Log("Ignoring Mesh as it is too big");
				return;
			}
			List<VectorMeshObject.Line> list;
			if (this._orderFunction == null)
			{
				list = dictionary.Values.ToList<VectorMeshObject.Line>();
			}
			else
			{
				list = (from x in dictionary.Values
				orderby Mathf.Min(this._orderFunction(x.pointA), this._orderFunction(x.pointB))
				select x).ToList<VectorMeshObject.Line>();
			}
			foreach (VectorMeshObject.Line current3 in list)
			{
				lines.Add(current3.pointA);
				lines.Add(current3.pointB);
			}
			points = lines.Distinct<Vector3>().ToList<Vector3>();
		}

		private float SortByRadial(Vector3 v)
		{
			return Vector3.Distance(Vector3.get_zero(), v);
		}

		private float SortByY(Vector3 v)
		{
			return v.y;
		}

		private void Start()
		{
			this.InitStateless();
			this.InitVectorMeshObject();
		}

		private void InitVectorMeshObject()
		{
			if (base.get_gameObject().GetComponent<MeshRenderer>() == null)
			{
				this.nonRenderingController = true;
			}
			if (this.useDefaultStyle)
			{
				this.SetVectorStyleDefaults();
			}
			this.ApplyVectorStyle();
			if (this.controlChildrenRenderers)
			{
				this.InitChildren();
			}
			if (!this.nonRenderingController)
			{
				List<Vector3> newlines;
				List<Vector3> newpoints;
				this.MakeFrameAndPoints(base.GetComponent<MeshFilter>().get_mesh(), out newlines, out newpoints);
				this.InitVectrosityElements(newlines, newpoints);
			}
			if (!this.controlChildrenRenderers)
			{
				this.ready = true;
			}
			else
			{
				base.StartCoroutine(this.WaitForChildrenReady());
			}
			if (this.vectorParent != null)
			{
				this.TellParentReady();
				this.controlChildrenRenderers = false;
			}
			this._state.Fire(VectorMeshObject.Trigger.REFRESH);
		}

		[DebuggerHidden]
		private IEnumerator WaitForChildrenReady()
		{
			VectorMeshObject.<WaitForChildrenReady>c__Iterator7 <WaitForChildrenReady>c__Iterator = new VectorMeshObject.<WaitForChildrenReady>c__Iterator7();
			<WaitForChildrenReady>c__Iterator.<>f__this = this;
			return <WaitForChildrenReady>c__Iterator;
		}
	}
}

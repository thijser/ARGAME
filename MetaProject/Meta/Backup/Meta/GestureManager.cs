// Decompiled with JetBrains decompiler
// Type: Meta.GestureManager
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Meta
{
  internal class GestureManager : MetaSingleton<GestureManager>
  {
    [SerializeField]
    private float _touchDwellTime = 1f;
    [SerializeField]
    private float _pointDwellTime = 1f;
    [SerializeField]
    private bool _enableGrab = true;
    [SerializeField]
    private HandType _defaultGrabbingHandType = HandType.EITHER;
    [SerializeField]
    private float _defaultGrabbableDistance = 0.1f;
    [SerializeField]
    private bool _enableGestureSounds = true;
    [SerializeField]
    private bool _enablePinch = true;
    [SerializeField]
    private HandType _defaultPinchingHandType = HandType.EITHER;
    [SerializeField]
    private float _defaultPinchableDistance = 0.1f;
    private MetaPhysics _physics = new MetaPhysics();
    private bool _markerGrabDistIncrease = true;
    private float _markerGrabDistance = 0.2f;
    private float _grabSwitchingBias = 1.25f;
    private float _minBound = 0.2f;
    private GestureManager.HandGestureData[] _handGestureData;
    [SerializeField]
    private AudioClip _grabOn;
    [SerializeField]
    private AudioClip _grabOff;
    [SerializeField]
    private AudioClip _pinchOn;
    [SerializeField]
    private AudioClip _pinchOff;
    [SerializeField]
    private AudioClip _outOfBounds;
    private AudioSource _gestureSound;
    private bool _twoHandedGesturing;
    private Vector3 _prevHandsVector;
    private Quaternion _prevObjRotation;
    private Transform _controlledObj;
    private Vector3 _initialObjOffset;
    private float _initialHandDistance;
    private Vector3 _initialScale;

    private void Start()
    {
      this._handGestureData = new GestureManager.HandGestureData[2];
      this._handGestureData[0] = new GestureManager.HandGestureData(HandType.LEFT);
      this._handGestureData[1] = new GestureManager.HandGestureData(HandType.RIGHT);
      this._gestureSound = (AudioSource) ((Component) this).GetComponent<AudioSource>();
    }

    private void Update()
    {
      this.UpdateGestures();
      this.ProcessStability();
      this.ProcessTwoHandedGestures();
      this.ProcessHands();
      this.UpdateGrabCircles();
    }

    private void UpdateGestures()
    {
      for (int index = 0; index < 2; ++index)
      {
        if (this._handGestureData[index].m_hand != null)
          this._handGestureData[index].m_currGesture = this._handGestureData[index].m_hand.gesture.type;
      }
    }

    private void ProcessHands()
    {
      if (this._twoHandedGesturing)
        return;
      for (int index = 0; index < 2; ++index)
      {
        if (this._handGestureData[index].m_hand != null)
        {
          if (this._handGestureData[index].m_currMode != MetaGesture.NONE)
          {
            MetaBody metaBody = (MetaBody) ((Component) this._handGestureData[index].m_controlledObj).GetComponent<MetaBody>();
            if (this._handGestureData[index].m_currMode == MetaGesture.GRAB)
            {
              ((Component) this._handGestureData[index].m_controlledObj).SendMessage("OnHold", (object) this._handGestureData[index].m_handType, (SendMessageOptions) 1);
              if (metaBody.moveObjectOnGrab)
              {
                ((Component) this._handGestureData[index].m_controlledObj).SendMessage("OnGrabMove", (object) this._handGestureData[index].m_handType, (SendMessageOptions) 1);
                this._physics.MoveObj(this._handGestureData[index].m_controlledObj, this._handGestureData[index].m_hand.palm.position, this._handGestureData[index].m_initialObjOffset);
                this.CheckBounds(this._handGestureData[index]);
                this._handGestureData[index].m_prevPalmPos = this._handGestureData[index].m_hand.palm.position;
              }
              if (metaBody.rotateObjectOnGrab)
              {
                ((Component) this._handGestureData[index].m_controlledObj).SendMessage("OnGrabRotate", (object) this._handGestureData[index].m_handType, (SendMessageOptions) 1);
                Vector3 handsVector = Vector3.op_Subtraction(this._handGestureData[index].m_hand.palm.position, this._handGestureData[index].m_controlledObj.get_position());
                this._physics.RotateObj(this._handGestureData[index].m_controlledObj, ref this._handGestureData[index].m_prevObjRot, ref this._handGestureData[index].m_prevHandVector, handsVector);
              }
              if (metaBody.scaleObjectOnGrab)
              {
                ((Component) this._handGestureData[index].m_controlledObj).SendMessage("OnGrabScale", (object) this._handGestureData[index].m_handType, (SendMessageOptions) 1);
                float handDist = Vector3.Distance(this._handGestureData[index].m_hand.palm.position, ((Component) Camera.get_main()).get_transform().get_position());
                this._physics.ScaleObj(this._handGestureData[index].m_controlledObj, this._handGestureData[index].m_initialDistance, handDist, this._handGestureData[index].m_initialScale, false);
              }
              MarkerTargetIndicator.UpdateClosestMarkerID(this._handGestureData[index].m_controlledObj, true);
            }
            else if (this._handGestureData[index].m_currMode == MetaGesture.PINCH)
            {
              ((Component) this._handGestureData[index].m_controlledObj).SendMessage("OnPinchHold", (object) this._handGestureData[index].m_handType, (SendMessageOptions) 1);
              if (metaBody.moveObjectOnPinch)
              {
                ((Component) this._handGestureData[index].m_controlledObj).SendMessage("OnPinchMove", (object) this._handGestureData[index].m_handType, (SendMessageOptions) 1);
                this._physics.MoveObj(this._handGestureData[index].m_controlledObj, this._handGestureData[index].m_hand.palm.position, this._handGestureData[index].m_initialObjOffset);
              }
              if (metaBody.scaleObjectOnPinch)
              {
                ((Component) this._handGestureData[index].m_controlledObj).SendMessage("OnPinchScale", (object) this._handGestureData[index].m_handType, (SendMessageOptions) 1);
                float handDist = Vector3.Distance(this._handGestureData[index].m_hand.palm.position, ((Component) Camera.get_main()).get_transform().get_position());
                this._physics.ScaleObj(this._handGestureData[index].m_controlledObj, this._handGestureData[index].m_initialDistance, handDist, this._handGestureData[index].m_initialScale, false);
              }
            }
            this.CheckRelease(this._handGestureData[index]);
          }
          else
          {
            this.CheckGesture(this._handGestureData[index]);
            this._handGestureData[index].m_prevPalmPos = ((Component) Camera.get_main()).get_transform().get_position();
          }
          this.CheckTouchGesture(this._handGestureData[index]);
          this.CheckPointGesture(this._handGestureData[index]);
        }
      }
    }

    private void ProcessStability()
    {
      for (int index = 0; index < 2; ++index)
      {
        if (Hands.GetHands()[index] != null)
        {
          this._handGestureData[index].recentLocations[this._handGestureData[index].index++ % this._handGestureData[index].stallFrames] = this._handGestureData[index].m_hand.pointer.position;
          if (this._handGestureData[index].index >= this._handGestureData[index].stallFrames)
          {
            this._handGestureData[index].average.x = (__Null) (double) (this._handGestureData[index].average.y = this._handGestureData[index].average.z = (__Null) 0.0f);
            foreach (Vector3 vector3_1 in this._handGestureData[index].recentLocations)
            {
              GestureManager.HandGestureData handGestureData = this._handGestureData[index];
              Vector3 vector3_2 = Vector3.op_Addition(handGestureData.average, vector3_1);
              handGestureData.average = vector3_2;
            }
            GestureManager.HandGestureData handGestureData1 = this._handGestureData[index];
            Vector3 vector3_3 = Vector3.op_Division(handGestureData1.average, (float) this._handGestureData[index].stallFrames);
            handGestureData1.average = vector3_3;
            Vector3 vector3_4 = (Vector3) null;
            foreach (Vector3 vector3_1 in this._handGestureData[index].recentLocations)
            {
              // ISSUE: explicit reference operation
              // ISSUE: variable of a reference type
              Vector3& local1 = @vector3_4;
              // ISSUE: explicit reference operation
              // ISSUE: variable of the null type
              __Null local2 = (^local1).x + vector3_1.x * vector3_1.x;
              // ISSUE: explicit reference operation
              (^local1).x = local2;
              // ISSUE: explicit reference operation
              // ISSUE: variable of a reference type
              Vector3& local3 = @vector3_4;
              // ISSUE: explicit reference operation
              // ISSUE: variable of the null type
              __Null local4 = (^local3).y + vector3_1.y * vector3_1.y;
              // ISSUE: explicit reference operation
              (^local3).y = local4;
              // ISSUE: explicit reference operation
              // ISSUE: variable of a reference type
              Vector3& local5 = @vector3_4;
              // ISSUE: explicit reference operation
              // ISSUE: variable of the null type
              __Null local6 = (^local5).z + vector3_1.z * vector3_1.z;
              // ISSUE: explicit reference operation
              (^local5).z = local6;
            }
            Vector3 vector3_5 = Vector3.op_Division(vector3_4, (float) this._handGestureData[index].stallFrames);
            this._handGestureData[index].variance.x = vector3_5.x - this._handGestureData[index].average.x * this._handGestureData[index].average.x;
            this._handGestureData[index].variance.y = vector3_5.y - this._handGestureData[index].average.y * this._handGestureData[index].average.y;
            this._handGestureData[index].variance.z = vector3_5.z - this._handGestureData[index].average.z * this._handGestureData[index].average.z;
            this._handGestureData[index].stable = this._handGestureData[index].variance.x < (double) this._handGestureData[index].threshold && this._handGestureData[index].variance.y < (double) this._handGestureData[index].threshold && this._handGestureData[index].variance.z < (double) this._handGestureData[index].threshold;
          }
        }
      }
    }

    private void ProcessTwoHandedGestures()
    {
      if (this._handGestureData[0].m_currGesture == this._handGestureData[1].m_currGesture)
      {
        if (this._handGestureData[0].m_currGesture == MetaGesture.GRAB)
        {
          if (this._twoHandedGesturing)
          {
            MetaBody metaBody = (MetaBody) ((Component) this._controlledObj).GetComponent<MetaBody>();
            if (metaBody.moveObjectOnTwoHandedGrab)
              this._physics.MoveObj(this._controlledObj, Vector3.op_Division(Vector3.op_Addition(Hands.right.palm.position, Hands.left.palm.position), 2f), this._initialObjOffset);
            else if (metaBody.rotateObjectOnTwoHandedGrab)
            {
              this._physics.RotateObj(this._controlledObj, ref this._prevObjRotation, ref this._prevHandsVector, Vector3.op_Subtraction(Hands.right.palm.position, Hands.left.palm.position));
            }
            else
            {
              if (!metaBody.scaleObjectOnTwoHandedGrab)
                return;
              this._physics.ScaleObj(this._controlledObj, this._initialHandDistance, Vector3.Distance(Hands.left.palm.position, Hands.right.palm.position), this._initialScale, true);
            }
          }
          else
          {
            Transform closestObj = this.FindClosestObj((GestureManager.HandGestureData) null, MetaGesture.GRAB);
            if (!Object.op_Inequality((Object) closestObj, (Object) null))
              return;
            this.StartTwoHandedGesture(closestObj);
          }
        }
        else
          this.EndTwoHandedGesture();
      }
      else
        this.EndTwoHandedGesture();
    }

    private void StartTwoHandedGesture(Transform closestObj)
    {
      this._controlledObj = closestObj;
      MetaBody metaBody = (MetaBody) ((Component) this._controlledObj).GetComponent<MetaBody>();
      if (this._handGestureData[0].m_currMode == MetaGesture.NONE && this._handGestureData[1].m_currMode == MetaGesture.NONE && this._enableGestureSounds)
      {
        this._gestureSound.set_clip(this._grabOn);
        this._gestureSound.Play();
      }
      metaBody.gesture = MetaGesture.GRAB;
      metaBody.grabbed = true;
      this._handGestureData[0].m_closestObj = this._controlledObj;
      this._handGestureData[1].m_closestObj = this._controlledObj;
      HandsInputModule.ToggleHand(HandType.LEFT, true);
      HandsInputModule.ToggleHand(HandType.RIGHT, true);
      this._initialObjOffset = Vector3.op_Subtraction(Vector3.op_Division(Vector3.op_Addition(Hands.right.palm.position, Hands.left.palm.position), 2f), this._controlledObj.get_position());
      this._prevHandsVector = Vector3.op_Subtraction(Hands.right.palm.position, Hands.left.palm.position);
      this._prevObjRotation = this._controlledObj.get_rotation();
      this._initialHandDistance = Vector3.Distance(Hands.left.palm.position, Hands.right.palm.position);
      this._initialScale = this._controlledObj.get_localScale();
      this._twoHandedGesturing = true;
    }

    private void EndTwoHandedGesture()
    {
      if (!this._twoHandedGesturing)
        return;
      this._twoHandedGesturing = false;
      this.EnableHands();
      this._handGestureData[0].m_currMode = MetaGesture.NONE;
      this._handGestureData[1].m_currMode = MetaGesture.NONE;
      ((MetaBody) ((Component) this._controlledObj).GetComponent<MetaBody>()).gesture = MetaGesture.NONE;
      ((MetaBody) ((Component) this._controlledObj).GetComponent<MetaBody>()).grabbed = false;
      this._controlledObj = (Transform) null;
    }

    private void CheckBounds(GestureManager.HandGestureData handGestureData)
    {
      float num1 = Vector3.Distance(handGestureData.m_prevPalmPos, ((Component) Camera.get_main()).get_transform().get_position());
      float num2 = Vector3.Distance(handGestureData.m_hand.palm.position, ((Component) Camera.get_main()).get_transform().get_position());
      if ((double) num2 >= (double) this._minBound || (double) num2 >= (double) num1 || (double) num1 == 0.0)
        return;
      this.ReleaseObj(handGestureData, true);
      Vector3 forward = ((Component) Camera.get_main()).get_transform().get_forward();
      Vector3 vector3 = Vector3.op_Addition(handGestureData.m_hand.palm.position, Vector3.op_Multiply(0.1f, forward));
      if ((double) Vector3.Distance(vector3, ((Component) Camera.get_main()).get_transform().get_position()) < (double) this._minBound)
        vector3 = Vector3.op_Addition(handGestureData.m_prevPalmPos, Vector3.op_Multiply(0.1f, forward));
      LeanTween.move(((Component) handGestureData.m_controlledObj).get_gameObject(), vector3, 1f).setEase((LeanTweenType) 18);
      this._enableGrab = false;
      this._enablePinch = false;
      this.Invoke("EnableGrabPinch", 1f);
    }

    private void EnableGrabPinch()
    {
      this._enableGrab = true;
      this._enablePinch = true;
    }

    private void CheckGesture(GestureManager.HandGestureData handGestureData)
    {
      this.FindNextGrabbaleObj(handGestureData);
      Transform controlledObj = (Transform) null;
      if (handGestureData.m_currGesture == MetaGesture.GRAB)
        controlledObj = handGestureData.m_closestObj;
      else if (handGestureData.m_currGesture == MetaGesture.PINCH)
        controlledObj = this.FindClosestObj(handGestureData, MetaGesture.PINCH);
      if (!Object.op_Inequality((Object) controlledObj, (Object) null))
        return;
      this.SetControlledObj(handGestureData, controlledObj);
    }

    private void CheckTouchGesture(GestureManager.HandGestureData handGestureData)
    {
      List<GameObject> list = new List<GameObject>();
      foreach (Collider collider in Physics.OverlapSphere(handGestureData.m_hand.pointer.gameObject.get_transform().get_position(), ((SphereCollider) handGestureData.m_hand.pointer.gameObject.GetComponent<Collider>()).get_radius()))
      {
        if (MetaSingleton<MetaManager>.Instance.touchables.Contains(((Component) collider).get_transform()) && (((MetaBody) ((Component) collider).GetComponent<MetaBody>()).useDefaultTouchSettings || ((MetaBody) ((Component) collider).GetComponent<MetaBody>()).touchingHandType == HandType.EITHER || ((MetaBody) ((Component) collider).GetComponent<MetaBody>()).touchingHandType == handGestureData.m_handType))
        {
          list.Add(((Component) collider).get_gameObject());
          if (handGestureData.m_touchedObjects.Contains(((Component) collider).get_gameObject()))
          {
            ((Component) collider).get_gameObject().SendMessage("OnTouchHold", (object) handGestureData.m_handType, (SendMessageOptions) 1);
            Dictionary<GameObject, float> dictionary1;
            GameObject gameObject1;
            (dictionary1 = handGestureData.m_touchDwellTimers)[gameObject1 = ((Component) collider).get_gameObject()] = dictionary1[gameObject1] + Time.get_deltaTime();
            float num = !((MetaBody) ((Component) collider).GetComponent<MetaBody>()).useDefaultTouchSettings ? ((MetaBody) ((Component) collider).GetComponent<MetaBody>()).touchDwellTime : this._touchDwellTime;
            float completion = handGestureData.m_touchDwellTimers[((Component) collider).get_gameObject()] / num;
            if ((((MetaBody) ((Component) collider).GetComponent<MetaBody>()).touchableDwellable || ((MetaBody) ((Component) collider).GetComponent<MetaBody>()).useDefaultTouchSettings) && (handGestureData.stable || !((MetaBody) ((Component) collider).GetComponent<MetaBody>()).useDefaultTouchSettings && !((MetaBody) ((Component) collider).GetComponent<MetaBody>()).touchDwellMustBeStable))
            {
              Dictionary<GameObject, float> dictionary2;
              GameObject gameObject2;
              (dictionary2 = handGestureData.m_touchDwellTimers)[gameObject2 = ((Component) collider).get_gameObject()] = dictionary2[gameObject2] + Time.get_deltaTime();
              if (MetaSingleton<InputIndicators>.Instance.dwellIndicators)
                MetaSingleton<InputIndicators>.Instance.UpdateDwellIndicators(handGestureData.m_handType, completion);
              else
                MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
            }
            else
            {
              MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
              handGestureData.m_touchDwellTimers[((Component) collider).get_gameObject()] = 0.0f;
            }
            if (((MetaBody) ((Component) collider).GetComponent<MetaBody>()).touchableDwellable && (double) handGestureData.m_touchDwellTimers[((Component) collider).get_gameObject()] >= (double) this._touchDwellTime)
              ((Component) collider).get_gameObject().SendMessage("OnTouchDwell", (object) handGestureData.m_handType, (SendMessageOptions) 1);
          }
          else
          {
            handGestureData.m_touchedObjects.Add(((Component) collider).get_gameObject());
            ((Component) collider).get_gameObject().SendMessage("OnTouchEnter", (object) handGestureData.m_handType, (SendMessageOptions) 1);
            handGestureData.m_touchDwellTimers.Add(((Component) collider).get_gameObject(), 0.0f);
          }
        }
      }
      using (List<GameObject>.Enumerator enumerator = handGestureData.m_touchedObjects.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          GameObject current = enumerator.Current;
          if (!list.Contains(current))
          {
            current.SendMessage("OnTouchExit", (object) handGestureData.m_handType, (SendMessageOptions) 1);
            if ((double) handGestureData.m_touchDwellTimers[current] >= (double) this._touchDwellTime && ((MetaBody) current.GetComponent<MetaBody>()).touchableDwellable)
              current.SendMessage("OnTouchDwellExit", (object) handGestureData.m_handType, (SendMessageOptions) 1);
            MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
            handGestureData.m_touchDwellTimers.Remove(current);
          }
        }
      }
      handGestureData.m_touchedObjects = list;
    }

    private void CheckPointGesture(GestureManager.HandGestureData handGestureData)
    {
      GameObject objectOfInterest = handGestureData.m_hand.pointer.objectOfInterest;
      if (handGestureData.m_hand.isValid && Object.op_Inequality((Object) objectOfInterest, (Object) null) && MetaSingleton<MetaManager>.Instance.pointables.Contains(objectOfInterest.get_transform()) && (((MetaBody) objectOfInterest.GetComponent<MetaBody>()).useDefaultPointSettings || ((MetaBody) objectOfInterest.GetComponent<MetaBody>()).pointingHandType == HandType.EITHER || ((MetaBody) objectOfInterest.GetComponent<MetaBody>()).pointingHandType == handGestureData.m_handType))
      {
        if (Object.op_Equality((Object) handGestureData.m_pointedAtObject, (Object) objectOfInterest))
        {
          objectOfInterest.SendMessage("OnPointHold", (object) handGestureData.m_handType, (SendMessageOptions) 1);
          handGestureData.m_pointedAtTime += Time.get_deltaTime();
          if (!((MetaBody) objectOfInterest.GetComponent<MetaBody>()).useDefaultPointSettings && !((MetaBody) objectOfInterest.GetComponent<MetaBody>()).pointableDwellable || !handGestureData.stable && (((MetaBody) objectOfInterest.GetComponent<MetaBody>()).useDefaultPointSettings || ((MetaBody) objectOfInterest.GetComponent<MetaBody>()).pointDwellMustBeStable))
            return;
          float num = !((MetaBody) objectOfInterest.GetComponent<MetaBody>()).useDefaultPointSettings ? ((MetaBody) objectOfInterest.GetComponent<MetaBody>()).pointDwellTime : this._pointDwellTime;
          if ((double) handGestureData.m_pointedAtTime >= (double) num)
            objectOfInterest.get_gameObject().SendMessage("OnPointDwell", (object) handGestureData.m_handType, (SendMessageOptions) 1);
          float completion = handGestureData.m_pointedAtTime / num;
          if (MetaSingleton<InputIndicators>.Instance.dwellIndicators)
            MetaSingleton<InputIndicators>.Instance.UpdateDwellIndicators(handGestureData.m_handType, completion);
          else
            MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
        }
        else
        {
          objectOfInterest.SendMessage("OnPointEnter", (object) handGestureData.m_handType, (SendMessageOptions) 1);
          handGestureData.m_pointedAtTime = 0.0f;
          if (Object.op_Inequality((Object) handGestureData.m_pointedAtObject, (Object) null))
            handGestureData.m_pointedAtObject.SendMessage("OnPointExit", (object) handGestureData.m_handType, (SendMessageOptions) 1);
          handGestureData.m_pointedAtObject = objectOfInterest;
        }
      }
      else
      {
        handGestureData.m_pointedAtTime = 0.0f;
        if (Object.op_Inequality((Object) handGestureData.m_pointedAtObject, (Object) null))
          handGestureData.m_pointedAtObject.SendMessage("OnPointExit", (object) handGestureData.m_handType, (SendMessageOptions) 1);
        MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
      }
    }

    private Transform FindClosestObj(GestureManager.HandGestureData handGestureData, MetaGesture gesture)
    {
      Transform transform = (Transform) null;
      float num1 = 0.0f;
      Vector3 vector3;
      List<Transform> list;
      if (gesture == MetaGesture.GRAB)
      {
        vector3 = handGestureData == null ? Vector3.op_Division(Vector3.op_Addition(Hands.left.palm.position, Hands.right.palm.position), 2f) : handGestureData.m_hand.palm.position;
        list = MetaSingleton<MetaManager>.Instance.grabbables;
      }
      else
      {
        vector3 = handGestureData == null ? Vector3.op_Division(Vector3.op_Addition(Hands.left.gesture.position, Hands.right.gesture.position), 2f) : handGestureData.m_hand.gesture.position;
        list = MetaSingleton<MetaManager>.Instance.pinchables;
      }
      using (List<Transform>.Enumerator enumerator = list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Transform current = enumerator.Current;
          if (Object.op_Inequality((Object) current, (Object) null))
          {
            MetaBody metaBody = (MetaBody) ((Component) current).GetComponent<MetaBody>();
            if (Object.op_Inequality((Object) metaBody, (Object) null) && (handGestureData == null || metaBody.gesture == MetaGesture.NONE) && (((Component) current).get_gameObject().get_activeInHierarchy() && (handGestureData != null || metaBody.twoHandedGrabbable)))
            {
              float num2 = Vector3.Distance(vector3, current.get_position());
              if ((double) num2 >= (double) this.GesturableDistance(metaBody, gesture))
              {
                if (Object.op_Inequality((Object) ((Component) current).GetComponent<Collider>(), (Object) null))
                {
                  Bounds bounds = ((Collider) ((Component) current).GetComponent<Collider>()).get_bounds();
                  // ISSUE: explicit reference operation
                  if (!((Bounds) @bounds).Contains(vector3))
                    continue;
                }
                else
                  continue;
              }
              if ((this.GestureHandType(metaBody, gesture) == HandType.EITHER || handGestureData == null || this.GestureHandType(metaBody, gesture) == handGestureData.m_handType) && ((double) num1 == 0.0 || (double) num2 < (double) num1))
              {
                transform = current;
                num1 = num2;
              }
            }
          }
        }
      }
      return transform;
    }

    private float GesturableDistance(MetaBody metaBody, MetaGesture gesture)
    {
      if (gesture == MetaGesture.GRAB)
      {
        float num = !metaBody.useDefaultGrabSettings ? metaBody.grabbableDistance : this._defaultGrabbableDistance;
        if (metaBody.markerTarget && metaBody.markerTargetID != -1 && (!metaBody.grabbed && this._markerGrabDistIncrease) && (double) this._markerGrabDistance > (double) num)
          num = this._markerGrabDistance;
        return num;
      }
      if (gesture != MetaGesture.PINCH)
        return 0.0f;
      if (metaBody.useDefaultPinchSettings)
        return this._defaultPinchableDistance;
      return metaBody.pinchableDistance;
    }

    private HandType GestureHandType(MetaBody metaBody, MetaGesture gesture)
    {
      if (gesture == MetaGesture.GRAB)
      {
        if (metaBody.useDefaultGrabSettings)
          return this._defaultGrabbingHandType;
        return metaBody.grabbingHandType;
      }
      if (gesture != MetaGesture.PINCH)
        return HandType.UNKNOWN;
      if (metaBody.useDefaultPinchSettings)
        return this._defaultPinchingHandType;
      return metaBody.pinchingHandType;
    }

    private void FindNextGrabbaleObj(GestureManager.HandGestureData handGestureData)
    {
      Transform closestObj = this.FindClosestObj(handGestureData, MetaGesture.GRAB);
      if (Object.op_Inequality((Object) closestObj, (Object) null) && Object.op_Inequality((Object) handGestureData.m_closestObj, (Object) null))
      {
        Vector3 position = handGestureData.m_hand.palm.position;
        if ((double) (Vector3.Distance(position, closestObj.get_position()) * this._grabSwitchingBias) >= (double) Vector3.Distance(position, handGestureData.m_closestObj.get_position()))
          return;
        handGestureData.m_closestObj = closestObj;
      }
      else
        handGestureData.m_closestObj = closestObj;
    }

    private void SetControlledObj(GestureManager.HandGestureData handGestureData, Transform controlledObj)
    {
      MetaBody metaBody = (MetaBody) ((Component) controlledObj).GetComponent<MetaBody>();
      if ((handGestureData.m_currGesture != MetaGesture.GRAB || !this._enableGrab) && (handGestureData.m_currGesture != MetaGesture.PINCH || !this._enablePinch))
        return;
      handGestureData.m_controlledObj = controlledObj;
      handGestureData.m_initialObjOffset = Vector3.op_Subtraction(handGestureData.m_hand.palm.position, controlledObj.get_position());
      handGestureData.m_prevObjRot = controlledObj.get_rotation();
      handGestureData.m_prevHandVector = Vector3.op_Subtraction(handGestureData.m_hand.palm.position, handGestureData.m_controlledObj.get_position());
      handGestureData.m_initialDistance = Vector3.Distance(handGestureData.m_hand.palm.position, ((Component) Camera.get_main()).get_transform().get_position());
      handGestureData.m_initialScale = controlledObj.get_localScale();
      HandsInputModule.ToggleHand(handGestureData.m_handType, true);
      if (handGestureData.m_currGesture == MetaGesture.GRAB && this._enableGrab)
      {
        handGestureData.m_currMode = MetaGesture.GRAB;
        handGestureData.m_controlledObj = controlledObj;
        metaBody.gesture = MetaGesture.GRAB;
        metaBody.grabbed = true;
        ((Component) controlledObj).SendMessage("OnGrab", (object) handGestureData.m_handType, (SendMessageOptions) 1);
        this._gestureSound.set_clip(this._grabOn);
      }
      else if (handGestureData.m_currGesture == MetaGesture.PINCH && this._enablePinch)
      {
        handGestureData.m_currMode = MetaGesture.PINCH;
        handGestureData.m_controlledObj = controlledObj;
        metaBody.gesture = MetaGesture.PINCH;
        metaBody.pinched = true;
        ((Component) controlledObj).SendMessage("OnPinch", (object) handGestureData.m_handType, (SendMessageOptions) 1);
        this._gestureSound.set_clip(this._pinchOn);
      }
      if (!this._enableGestureSounds)
        return;
      this._gestureSound.Play();
    }

    private void CheckRelease(GestureManager.HandGestureData handGestureData)
    {
      if ((handGestureData.m_currMode != MetaGesture.GRAB || handGestureData.m_currGesture != MetaGesture.OPEN) && (handGestureData.m_currMode == MetaGesture.GRAB || handGestureData.m_currGesture == handGestureData.m_currMode) && handGestureData.m_hand.isValid)
        return;
      this.ReleaseObj(handGestureData, false);
    }

    private void ReleaseObj(GestureManager.HandGestureData handGestureData, bool forced = false)
    {
      MetaBody metaBody = (MetaBody) ((Component) handGestureData.m_controlledObj).GetComponent<MetaBody>();
      metaBody.gesture = MetaGesture.NONE;
      if (handGestureData.m_currMode == MetaGesture.GRAB)
      {
        metaBody.grabbed = false;
        ((Component) handGestureData.m_controlledObj).SendMessage("OnRelease", (object) handGestureData.m_handType, (SendMessageOptions) 1);
        ((Component) handGestureData.m_controlledObj).SendMessage("OnGrabRelease", (object) handGestureData.m_handType, (SendMessageOptions) 1);
        MarkerTargetIndicator.UpdateClosestMarkerID(handGestureData.m_controlledObj, false);
        if (this._enableGestureSounds && !forced)
        {
          this._gestureSound.set_clip(this._grabOff);
          this._gestureSound.Play();
        }
      }
      else if (handGestureData.m_currMode == MetaGesture.PINCH)
      {
        metaBody.pinched = false;
        ((Component) handGestureData.m_controlledObj).SendMessage("OnPinchRelease", (object) handGestureData.m_handType, (SendMessageOptions) 1);
        if (this._enableGestureSounds && !forced)
        {
          this._gestureSound.set_clip(this._pinchOff);
          this._gestureSound.Play();
        }
      }
      if (forced)
      {
        this._gestureSound.set_clip(this._outOfBounds);
        this._gestureSound.Play();
      }
      handGestureData.m_currMode = MetaGesture.NONE;
      this.StartCoroutine("EnableHand", (object) handGestureData.m_handType);
    }

    private void EnableHands()
    {
      this.StartCoroutine("EnableHand", (object) HandType.LEFT);
      this.StartCoroutine("EnableHand", (object) HandType.RIGHT);
    }

    [DebuggerHidden]
    private IEnumerator EnableHand(HandType hand)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GestureManager.\u003CEnableHand\u003Ec__Iterator3()
      {
        hand = hand,
        \u003C\u0024\u003Ehand = hand,
        \u003C\u003Ef__this = this
      };
    }

    private void UpdateGrabCircles()
    {
      for (int hand = 0; hand < 2; ++hand)
        MetaSingleton<InputIndicators>.Instance.UpdateGrabCircles(hand, Object.op_Inequality((Object) this._handGestureData[hand].m_closestObj, (Object) null));
    }

    private class HandGestureData
    {
      public float threshold = 5E-05f;
      public int stallFrames = 30;
      public HandType m_handType;
      public Hand m_hand;
      public Vector3 m_prevPalmPos;
      public MetaGesture m_currGesture;
      public MetaGesture m_currMode;
      public Transform m_closestObj;
      public Transform m_controlledObj;
      public Vector3 m_initialObjOffset;
      public Vector3 m_initialScale;
      public float m_initialDistance;
      public Quaternion m_prevObjRot;
      public Vector3 m_prevHandVector;
      public GameObject m_pointedAtObject;
      public float m_pointedAtTime;
      public List<GameObject> m_touchedObjects;
      public Dictionary<GameObject, float> m_touchDwellTimers;
      public Vector3 variance;
      public bool stable;
      public Vector3 average;
      public Vector3[] recentLocations;
      public int index;

      public HandGestureData(HandType handType)
      {
        this.m_handType = handType;
        this.m_currMode = MetaGesture.NONE;
        this.m_hand = Hands.GetHands()[(int) this.m_handType];
        this.m_touchedObjects = new List<GameObject>();
        this.m_touchDwellTimers = new Dictionary<GameObject, float>();
        this.variance = (Vector3) null;
        this.recentLocations = new Vector3[this.stallFrames];
      }
    }
  }
}

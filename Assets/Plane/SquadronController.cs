using ClassExtansion;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Plane
{
    public class SquadronController : MonoBehaviour
    {
        private bool isInit = false;

        private Transform _shipTransform;
        private GameObject _planePrefab;
        private PlaneData _planeModel;

        private int _maxPlaneCount;
        private GameObject[]  _planeGraphicsSlots;
        private PlaneController[] _planeControllerSlots;

        private float _shipDistance;
        private float _onePlaneAngleOffset;
        private float _rotationSpeed;
        private float _rotationOffset;

        public Vector2 ShipPosition
        {
            get
            {
                return _shipTransform.position;
            }
        }

        private int EmptySlotIndex
        {
            get
            {
                if (_planeControllerSlots == null)
                {
                    throw new NullReferenceException("_PlaneControllerSlots");
                }
                for (int i = 0; i < _planeControllerSlots.Length; i++)
                {
                    if (_planeControllerSlots[i] == null)
                        return i;
                }
                return -1;
            }
        }

        public void Init(Transform shipTransform, GameObject planePrefab, PlaneData planeModel,int maxPlaneCount,float shipDistance,float rotationSpeed)
        {
            if (shipTransform == null) throw new ArgumentNullException("shipTransform");
            if (planePrefab == null) throw new ArgumentNullException("planePrefab");
            if (planeModel == null) throw new ArgumentNullException("planeModel");
            _shipTransform = shipTransform;
            _planeModel = planeModel;
            _planePrefab = planePrefab;
            _maxPlaneCount = maxPlaneCount;
            _planeGraphicsSlots = new GameObject[_maxPlaneCount];
            _planeControllerSlots = new PlaneController[_maxPlaneCount];
            _shipDistance = shipDistance;
            _onePlaneAngleOffset = 360 / _maxPlaneCount;
            _rotationSpeed = rotationSpeed;
            _rotationOffset = 0;
            isInit = true;
        }

        public Transform GetOtherPlaneInMinDistance(int slotIndex, float minOtherPlaneDistance,out Vector2 otherVelocity)
        {
            Vector2 currentPosition = _planeGraphicsSlots[slotIndex].transform.position;
            otherVelocity = Vector2.zero;
            for (int i =0;i<_planeGraphicsSlots.Length;i++)
            {
                if(_planeGraphicsSlots[i]!=null && i!=slotIndex)
                {
                    if(Vector2.Distance(currentPosition,_planeGraphicsSlots[i].transform.position)<minOtherPlaneDistance)
                    {
                        otherVelocity = _planeControllerSlots[i].Velocity;
                        return _planeGraphicsSlots[i].transform;
                    }
                }
            }
            return null;
        }

        public void StartPlane()
        {
            if(!isInit)
            {
                Debug.LogError("SquadronController no init");
                return;
            }
            int emptySlotIndex = EmptySlotIndex;
            if(emptySlotIndex<0)
            {
                Debug.Log("All planes are airborne");
                return;
            }
            GameObject graphics = CreatePlaneGraphics();
            PlaneController controller = CreatePlaneController(graphics,emptySlotIndex,_planeModel);
            _planeControllerSlots[emptySlotIndex] = controller;
            _planeGraphicsSlots[emptySlotIndex] = graphics;
        }

        public void DestroyPlane(int slotindex)
        {
            _planeControllerSlots[slotindex] = null;
            Destroy(_planeGraphicsSlots[slotindex]);
            _planeGraphicsSlots[slotindex] = null;
        }

        private PlaneController CreatePlaneController(GameObject graphics,int index,PlaneData model)
        {
            return new PlaneController(index,graphics, model, this);
        }

        private GameObject CreatePlaneGraphics()
        {
            GameObject plane = Instantiate(_planePrefab,transform);
            plane.transform.position = _shipTransform.position;
            return plane;
        }

        private void FixedUpdate()
        {
            if (!isInit) return;
            UpdatePlanesTargets();
            for(int i =0;i<_planeControllerSlots.Length;i++)
            {
                if (_planeControllerSlots[i] != null)
                    _planeControllerSlots[i].Update();
            }
        }
        /// <summary>
        /// Устанавливаем для самолетов цель для преследования авианосца, точки крутятся вокруг корабля
        /// Самолет сам выбирает лететь к этой цели, уклонятся от другого самолета или лететь на корабль
        /// </summary>
        private void UpdatePlanesTargets()
        {
            Vector2 direction = new Vector2(0,1).Rotate(_rotationOffset);
            Vector2 shipPosition = _shipTransform.position;
            for (int i = 0; i < _planeControllerSlots.Length; i++)
            {
                if (_planeControllerSlots[i] != null)
                {
                    Vector2 target = shipPosition + direction * _shipDistance;
                    _planeControllerSlots[i].SetFollowTarget(target);
                }
                direction = direction.Rotate(_onePlaneAngleOffset);
            }
            _rotationOffset += _rotationSpeed;
        }
    }
}

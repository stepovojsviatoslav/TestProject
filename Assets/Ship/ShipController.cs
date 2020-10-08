using System;
using ClassExtansion;
using UnityEngine;
using Plane;

namespace Ship
{
    public class ShipController : MonoBehaviour {

        public ShipData Model;

        private Vector2 _direction;
        private float _linearSpeed;
        private float _angularSpeed;
        private short _linearAccelerationDirection;
        private short _angularAccelerationDirection;

        private SquadronController _planeController;

        private void Start()
        {
            _direction = transform.up;
            _linearSpeed = 0;
            _angularSpeed = 0;
            _linearAccelerationDirection = 0;
            _angularAccelerationDirection = 0;
            if(Model != null)
            {
                GameObject planeControllerGameObject = new GameObject(name + "PlaneController");
                _planeController = planeControllerGameObject.AddComponent<SquadronController>();
                _planeController.Init(transform, Model.PlanePrefab, Model.PlaneModel,Model.PlaneCount,Model.PlaneDistance,Model.PlaneRotationSpeed);
            }
            else
            {
                throw new NullReferenceException("Ship model");
            }
        }

        private void FixedUpdate()
        {
            //Упрощение: ускорение не наращивается постепенно, оно константно.
            AngularMovement();
            LinearMovement();
            WaterResistance();
            ApplyVelocity();
        }

        private void AngularMovement()
        {
            if (_angularAccelerationDirection != 0)
            {
                _angularSpeed = AccelerationSpeed(_angularSpeed, Model.AngularAcceleration, Model.MaxAngularSpeed, _angularAccelerationDirection);
                
            }
        }

        private void LinearMovement()
        {
            if (_linearAccelerationDirection != 0)
            {
                _linearSpeed = AccelerationSpeed(_linearSpeed, Model.LinearAcceleration, Model.MaxLinearSpeed, _linearAccelerationDirection);
            }
            
        }

        private void WaterResistance()
        {
            //Очевидно что сопротивление должно расчитываться иначе, в тз не указанно как именно, так что упрощенный вариант
            //чтобы была уменьшающаяся инерция при остановке прикладывания силы
            _linearSpeed = SpeedResistance(_linearSpeed, Model.WaterResistance);
            _angularSpeed = SpeedResistance(_angularSpeed, Model.WaterResistance);
        }
        private void ApplyVelocity()
        {
            _direction = _direction.Rotate(_angularSpeed);
            Vector3 velocity = _direction.normalized * _linearSpeed;
            transform.position += velocity;
            transform.up = _direction;
        }

        private float SpeedResistance(float speed,float resistance)
        {
            float unSingnedSpeed = Math.Abs(speed);
            if (unSingnedSpeed > 0.001f)
            {
                float sign = unSingnedSpeed / speed;
                speed -= sign * resistance;
            }
            return speed;
        }

        private float AccelerationSpeed(float speed,float acceleration,float maxSpeed,float direction)
        {
            speed += acceleration * direction;
            float unSingnedSpeed = Math.Abs(speed);
            if (unSingnedSpeed > maxSpeed)
            {
                float sign = unSingnedSpeed / speed;
                speed = maxSpeed * sign;
            }
            return speed;
        }

        public void SpeedUp()
        {
            _linearAccelerationDirection = 1;
        }

        public void SpeedDown()
        {
            _linearAccelerationDirection = -1;
        }

        public void SpeedNoChange()
        {
            _linearAccelerationDirection = 0;
        }

        public void DirectionRight()
        {
            _angularAccelerationDirection = -1;
        }

        public void DirectionLeft()
        {
            _angularAccelerationDirection = 1;
        }

        public void DirectionNoChange()
        {
            _angularAccelerationDirection = 0;
        }

        public void StartPlane()
        {
            if(_planeController!=null)
            {
                _planeController.StartPlane();
            }
        }
    }
}

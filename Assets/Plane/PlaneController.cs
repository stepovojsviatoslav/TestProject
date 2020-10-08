using UnityEngine;

namespace Plane
{
    public class PlaneController
    {
        enum Decision
        {
            WillLand,
            Follow,
            Evading
        }

        private SquadronController _squadronController;
        private Transform _graphics;
        private PlaneData _model;
        private int _slotIndex;
        private float _endLifeTime;
        private Vector3 _followTarget;
        private Vector3 _target;
        private Decision _decision;

        private Vector2 _direction;
        private float _linearSpeed;
        private Vector3 _velocity;

        public Vector2 Velocity
        {
            get { return _velocity; }
        }

        public PlaneController(int index,GameObject graphics,PlaneData model, SquadronController squadronController)
        {
            _squadronController = squadronController;
            _graphics = graphics.transform;
            _slotIndex = index;
            _model = model;
            _endLifeTime = Time.time+_model.LifeTimeSeconds;
            _direction = _graphics.up;
            _linearSpeed = _model.MinLinearSpeed;

        }

        public void SetFollowTarget(Vector2 target)
        {
            _followTarget = target;
        }

        public void Update()
        {
            if (IsLifeTimeout())
                return;
            ArrivalDecision();
            CalculateSpeeds();
            ApplyVelocity();
            DebugPoints();
            IsWillLandOnShip();
        }

        /// <summary>
        /// Принимаем решение о маневре в порядки приоритетов
        /// 1) Уклоняемся от другого самолета
        /// 2) Летим на корабль если топливо(время на исходе)
        /// 3) Летим к точке приследования
        /// </summary>
        private void ArrivalDecision()
        {
            Vector2 otherVelocity;
            Transform otherPlane = _squadronController.GetOtherPlaneInMinDistance(_slotIndex, _model.MinOtherPlaneDistance,out otherVelocity);
            if (otherPlane != null)
            {
                float distance = Vector2.Distance(_graphics.position, otherPlane.position);
                float period = distance/_model.MaxLinearSpeed;
                _target = otherPlane.position - new Vector3(otherVelocity.x * period,otherVelocity.y*period);
                _decision = Decision.Evading;
                return;
            }
            Vector2 shipPosition = _squadronController.ShipPosition;
            if(Time.time + _model.MinLifeTimeDeltaWillLand >= _endLifeTime)
            {
                _target = shipPosition;
                _decision = Decision.WillLand;
                return;
            }
            _target = _followTarget;
            _decision = Decision.Follow;
        }

        private void CalculateSpeeds()
        {
            Vector2 targetDirection = _target - _graphics.position;
            float angleDirection = Vector2.Dot(targetDirection, _direction);
            _linearSpeed = targetDirection.magnitude > _model.MaxLinearSpeed ? _model.MaxLinearSpeed : targetDirection.magnitude;
            if (angleDirection<=0 || _linearSpeed<_model.MinLinearSpeed)
            {
                _linearSpeed = _model.MinLinearSpeed;
            }
        }

        private void ApplyVelocity()
        {
            Vector2 targetDirection = _target - _graphics.position;
            _direction = Vector2.Lerp(_direction, targetDirection, _model.MaxAngularSpeed);
            _velocity = _direction.normalized * _linearSpeed;
            _graphics.position += _velocity;
            _graphics.up = _direction;
        }

        private void IsWillLandOnShip()
        {
           if(_decision == Decision.WillLand)
            {
                Vector2 shipPosition = _squadronController.ShipPosition;
                float toShipDistance = Vector2.Distance(shipPosition, _graphics.position);
                if(toShipDistance< _model.MinDistanceForLanding)
                {
                    Debug.Log("Plane landing");
                    Destroy();
                }
            }
        }

        private bool IsLifeTimeout()
        {
            if (_endLifeTime <= Time.time)
            {
                Debug.Log("Plane Crashed On Time");
                Destroy();
                return true;
            }
            return false;
        }

        private void Destroy()
        {
            _squadronController.DestroyPlane(_slotIndex);
        }

        private void DebugPoints()
        {
            Debug.DrawLine(_target, _graphics.transform.position);
        }
    }
}

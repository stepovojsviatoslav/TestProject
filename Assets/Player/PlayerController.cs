using Ship;
using UnityEngine;
namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private const float _movementDelta = 0.5f;

        public ShipController Ship;

        void Update()
        {
            if (Ship != null)
            {
                float speed = Input.GetAxis("Speed");
                //Не очень красивая кострукция,но позволяет избегать избыточных проверок
                //Ждем С#7 и удобный switch
                if (speed > _movementDelta)
                {
                    Ship.SpeedUp();
                }
                else
                {
                    if (speed < -_movementDelta)
                    {
                        Ship.SpeedDown();
                    }else
                    {
                        Ship.SpeedNoChange();
                    }
                }
                
                float direction = Input.GetAxis("Direction");
                if (direction > _movementDelta)
                {
                    Ship.DirectionRight();
                }
                else
                {
                    if (direction < -_movementDelta)
                    {
                        Ship.DirectionLeft();
                    }else
                    {
                        Ship.DirectionNoChange();
                    }
                }
                if(Input.GetButtonDown("StartPlane"))
                {
                    Ship.StartPlane();
                }
            }
        }
    }
}

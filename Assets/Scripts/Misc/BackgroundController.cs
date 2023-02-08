using UnityEngine;

namespace Assets.Scripts.Misc
{
    class BackgroundController : MonoBehaviour
    {
        public Vector2 parallaxEffectMultiplier;
        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        float textUnitSizeX;
        float textUnitSizeY;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
            lastCameraPosition = cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textUnitSizeX = texture.width / sprite.pixelsPerUnit;
            textUnitSizeY = texture.height / sprite.pixelsPerUnit;
        }

        private void LateUpdate()
        {
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3( deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;

            if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
            }
            if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textUnitSizeY)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textUnitSizeY;
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
            }
        }
    }
}

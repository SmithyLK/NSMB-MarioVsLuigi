using NSMB.Quantum;
using NSMB.Utilities.Extensions;
using Quantum;
using UnityEngine;
using UnityEngine.UI;

namespace NSMB.UI.Game.Track {
    public class TrackIcon : QuantumSceneViewComponent {

        //---Serialized Variables
        [SerializeField] private float trackMinX, trackMaxX;
        [SerializeField] protected Image image;
        [SerializeField] private Image upArrow, downArrow;

        //---Private Variables
        protected EntityRef targetEntity;
        protected Transform targetTransform;
        protected PlayerElements playerElements;
        protected VersusStageData stage;

        private float levelWidthReciprocal;
        private float levelMinX;
        private float trackWidth;

        public virtual void OnValidate() {
            this.SetIfNull(ref image);
        }

        public void Initialize(PlayerElements playerElements, EntityRef targetEntity, Transform targetTransform) {
            this.playerElements = playerElements;
            this.targetEntity = targetEntity;
            this.targetTransform = targetTransform;

            Frame f = Updater.ObservedGame.Frames.Predicted;
            stage = f.FindAsset<VersusStageData>(f.Map.UserAsset);
            
            levelMinX = stage.StageWorldMin.X.AsFloat;
            trackWidth = trackMaxX - trackMinX;
            levelWidthReciprocal = 2f / stage.TileDimensions.X;

            name = $"TrackIcon ({targetEntity})";
            OnLateUpdateView();
        }

        public override void OnLateUpdateView() {
            if (!targetTransform) {
                return;
            }

            float percentage = (targetTransform.position.x - levelMinX) * levelWidthReciprocal;
            transform.localPosition = new(percentage * trackWidth - trackMaxX, transform.localPosition.y, transform.localPosition.z);

            if (upArrow && downArrow) {
                float stageHeight = (stage.CameraMaxPosition.Y - stage.CameraMinPosition.Y).AsFloat;
                if (stageHeight > 18) {
                    // Thirds
                    upArrow.enabled = targetTransform.position.y > (stage.CameraMaxPosition.Y.AsFloat) - (stageHeight / 3f);
                    downArrow.enabled = targetTransform.position.y < (stage.CameraMinPosition.Y.AsFloat) + (stageHeight / 3f);
                } else if (stageHeight > 12) {
                    // Halves
                    upArrow.enabled = targetTransform.position.y > ((stage.CameraMaxPosition.Y + stage.CameraMinPosition.Y).AsFloat / 2f);
                    downArrow.enabled = !upArrow.enabled;
                } else {
                    // Screen bounds
                    upArrow.enabled = targetTransform.position.y > stage.CameraMaxPosition.Y.AsFloat;
                    downArrow.enabled = targetTransform.position.y < stage.CameraMinPosition.Y.AsFloat;
                }
            }
        }
    }
}

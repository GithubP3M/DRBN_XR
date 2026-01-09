using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Interaction.Toolkit.Examples
{
    /// <summary>
    /// Use this class to manage locomotion control schemes and configuration preferences.
    /// </summary>
    public class LocomotionSchemeManager : MonoBehaviour
    {
        /// <summary>
        /// Sets which source provides the forward direction for continuous movement.
        /// </summary>
        public enum MoveForwardSource
        {
            /// <summary>
            /// Use the head (HMD) as the forward source.
            /// </summary>
            Head,
            /// <summary>
            /// Use the left hand controller as the forward source.
            /// </summary>
            LeftHand,
            /// <summary>
            /// Use the right hand controller as the forward source.
            /// </summary>
            RightHand,
        }

        /// <summary>
        /// Sets which movement scheme is used.
        /// </summary>
        public enum MoveScheme
        {
            /// <summary>
            /// Use continuous movement (smooth locomotion).
            /// </summary>
            Continuous,
            /// <summary>
            /// Use noncontinuous movement (e.g., teleportation).
            /// </summary>
            Noncontinuous,
        }

        /// <summary>
        /// Sets which turn style is used.
        /// </summary>
        public enum TurnStyle
        {
            /// <summary>
            /// Use continuous turning (smooth rotation).
            /// </summary>
            Continuous,
            /// <summary>
            /// Use snap turning (discrete rotation).
            /// </summary>
            Snap,
        }

        [SerializeField]
        [Tooltip("The move scheme to use.")]
        MoveScheme m_MoveScheme = MoveScheme.Noncontinuous;

        /// <summary>
        /// The move scheme to use.
        /// </summary>
        public MoveScheme moveScheme
        {
            get => m_MoveScheme;
            set
            {
                m_MoveScheme = value;
                UpdateMoveScheme();
            }
        }

        [SerializeField]
        [Tooltip("The turn style to use.")]
        TurnStyle m_TurnStyle = TurnStyle.Snap;

        /// <summary>
        /// The turn style to use.
        /// </summary>
        public TurnStyle turnStyle
        {
            get => m_TurnStyle;
            set
            {
                m_TurnStyle = value;
                UpdateTurnStyle();
            }
        }

        [SerializeField]
        [Tooltip("The source Transform to define the forward direction of continuous movement.")]
        MoveForwardSource m_MoveForwardSource = MoveForwardSource.Head;

        /// <summary>
        /// The source Transform to define the forward direction of continuous movement.
        /// </summary>
        public MoveForwardSource moveForwardSource
        {
            get => m_MoveForwardSource;
            set
            {
                m_MoveForwardSource = value;
                UpdateMoveForwardSource();
            }
        }

        [SerializeField]
        [Tooltip("The Continuous Move Provider used to move the player.")]
        ContinuousMoveProviderBase m_ContinuousMoveProvider;

        /// <summary>
        /// The Continuous Move Provider used to move the player.
        /// </summary>
        public ContinuousMoveProviderBase continuousMoveProvider
        {
            get => m_ContinuousMoveProvider;
            set => m_ContinuousMoveProvider = value;
        }

        [SerializeField]
        [Tooltip("The Continuous Turn Provider used to rotate the player.")]
        ContinuousTurnProviderBase m_ContinuousTurnProvider;

        /// <summary>
        /// The Continuous Turn Provider used to rotate the player.
        /// </summary>
        public ContinuousTurnProviderBase continuousTurnProvider
        {
            get => m_ContinuousTurnProvider;
            set => m_ContinuousTurnProvider = value;
        }

        [SerializeField]
        [Tooltip("The Snap Turn Provider used to rotate the player.")]
        SnapTurnProviderBase m_SnapTurnProvider;

        /// <summary>
        /// The Snap Turn Provider used to rotate the player.
        /// </summary>
        public SnapTurnProviderBase snapTurnProvider
        {
            get => m_SnapTurnProvider;
            set => m_SnapTurnProvider = value;
        }

        [SerializeField]
        [Tooltip("The Transform that defines the forward direction for head-based movement.")]
        Transform m_HeadForwardSource;

        [SerializeField]
        [Tooltip("The Transform that defines the forward direction for left hand-based movement.")]
        Transform m_LeftHandForwardSource;

        [SerializeField]
        [Tooltip("The Transform that defines the forward direction for right hand-based movement.")]
        Transform m_RightHandForwardSource;

        protected void OnEnable()
        {
            UpdateMoveScheme();
            UpdateTurnStyle();
            UpdateMoveForwardSource();
        }

        void UpdateMoveScheme()
        {
            if (m_ContinuousMoveProvider != null)
            {
                m_ContinuousMoveProvider.enabled = m_MoveScheme == MoveScheme.Continuous;
            }
        }

        void UpdateTurnStyle()
        {
            if (m_ContinuousTurnProvider != null)
            {
                m_ContinuousTurnProvider.enabled = m_TurnStyle == TurnStyle.Continuous;
            }

            if (m_SnapTurnProvider != null)
            {
                m_SnapTurnProvider.enabled = m_TurnStyle == TurnStyle.Snap;
            }
        }

        void UpdateMoveForwardSource()
        {
            if (m_ContinuousMoveProvider == null)
                return;

            Transform forwardSource = null;
            switch (m_MoveForwardSource)
            {
                case MoveForwardSource.Head:
                    forwardSource = m_HeadForwardSource;
                    break;
                case MoveForwardSource.LeftHand:
                    forwardSource = m_LeftHandForwardSource;
                    break;
                case MoveForwardSource.RightHand:
                    forwardSource = m_RightHandForwardSource;
                    break;
            }

            m_ContinuousMoveProvider.forwardSource = forwardSource;
        }
    }
}

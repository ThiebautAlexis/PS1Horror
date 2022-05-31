using EnhancedEditor;
using UnityEngine;

namespace HorrorPS1.Core
{
    /// <summary>
    /// Base class to derive every MonoBehaviour of the project from.
    /// <para/>
    /// DreadfulBehaviour uses component activation callbacks to automatically
    /// register / unregister itself from updates, avoiding using Unity update callbacks
    /// for a better result at lower cost.
    /// You can register to a variety of callbacks using the <see cref="UpdateRegistration"/> flag.
    /// <para/>
    /// Also avec access to basic activation / deactivation callbacks that ensure being called
    /// when its state really changes.
    /// <para/>
    /// You can use the OnPaused callback to implement specific behaviours when the object gets paused / unpaused,
    /// which can happen in a variety of situations like a combo or a slow motion effect.
    /// </summary>
    public class HorrorBehaviour : MonoBehaviour
    {
        #region Update Registration
        /// <summary>
        /// Override this to specify this object update registration.
        /// <para/>
        /// Use "base.UpdateRegistration | <see cref="TheDreadfulShow.UpdateRegistration"/>.value"
        /// to add a registration, or override it by setting its direct value.
        /// </summary>
        public virtual UpdateRegistration UpdateRegistration => 0;
        #endregion

        #region Global Members
        public int ID => GetInstanceID();

        // -----------------------

        [Section("Dreadful Behaviour")]

        [SerializeField, ReadOnly] protected bool isActivated = false;
        #endregion

        #region State Callbacks
        /// <summary>
        /// Called when the object pause state changes.
        /// </summary>
        protected virtual void OnPaused(bool _isPaused) { }

        /// <summary>
        /// Called when the object is activated.
        /// </summary>
        protected virtual void OnActivation()
        {
            if (UpdateRegistration != 0)
                UpdateManager.Instance.Register(this, UpdateRegistration);
        }

        /// <summary>
        /// Called when the object is deactivated.
        /// </summary>
        protected virtual void OnDeactivation()
        {
            if (UpdateRegistration != 0)
                UpdateManager.Instance.Unregister(this, UpdateRegistration);
        }
        #endregion

        #region Comparison
        /// <summary>
        /// Compare two object.
        /// True if they are the same, false otherwise.
        /// </summary>
        public bool Compare(HorrorBehaviour _other) => ID == _other.ID;
        #endregion

        #region MonoBehaviour
        protected virtual void OnEnable()
        {
            if (!isActivated)
            {
                isActivated = true;
                OnActivation(); 
            }
        }

        protected virtual void OnDisable()
        {
            if (isActivated && !GameManager.IsQuittingApplication)
            {
                isActivated = false;
                OnDeactivation();
            }
        }
        #endregion
    }
}

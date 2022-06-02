using HorrorPS1.Core;
using UnityEngine;

namespace HorrorPS1.Movable
{
    /// <summary>
    /// Base class to derive all game triggers from.
    /// Provides easy-to-use callbacks when something enters or exits this trigger.
    /// </summary>
	public abstract class Trigger : HorrorBehaviour
    {
        #region Callbacks
        /// <summary>
        /// Called when something enters this trigger.
        /// </summary>
        /// <param name="_movable">Movable who entered this trigger.</param>
        public virtual void OnEnter(Movable _movable) { }

        /// <summary>
        /// Called when something exits this trigger.
        /// </summary>
        /// <param name="_movable">Movable who exited this trigger.</param>
        public virtual void OnExit(Movable _movable) { }
		#endregion
    }
}

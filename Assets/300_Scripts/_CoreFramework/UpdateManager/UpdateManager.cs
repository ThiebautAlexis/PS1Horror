using EnhancedEditor;
using HorrorPS1.Tools;
using System;
using UnityEngine;

namespace HorrorPS1.Core
{
    #region Update Interfaces
    // -------------------------------------------
    // Update Interfaces
    // -------------------------------------------

    public interface IEarlyUpdate   { void Update(); }
    public interface IInputUpdate   { void Update(); }
    public interface IDynamicUpdate { void Update(); }
    public interface IUpdate        { void Update(); }
    public interface IMovableUpdate { void Update(); }
    public interface ILateUpdate    { void Update(); }
    #endregion

    [Flags]
    public enum UpdateRegistration
    {
        Early = 1 << 0,
        Input = 1 << 1,
        Update = 1 << 2,
        Dynamic = 1 << 3,
        Movable = 1 << 4,
        Late = 1 << 5
    }

    public class UpdateManager : SingletonBehaviour<UpdateManager>
    {
        #region Global Members
        private Buffer<IEarlyUpdate> earlyUpdates = new Buffer<IEarlyUpdate>();
        private Buffer<IInputUpdate> inputUpdates = new Buffer<IInputUpdate>(1);
        private Buffer<IDynamicUpdate> dynamicUpdates = new Buffer<IDynamicUpdate>(5);
        private Buffer<IUpdate> updates = new Buffer<IUpdate>(10);
        private Buffer<IMovableUpdate> movableUpdates = new Buffer<IMovableUpdate>(10);
        private Buffer<ILateUpdate> lateUpdates = new Buffer<ILateUpdate>();
        #endregion

        #region Defined Registrations
        /// <summary>
        /// Registers an object on early update.
        /// </summary>
        public void Register(IEarlyUpdate _update) => earlyUpdates.Add(_update);

        /// <summary>
        /// Unregisters an object from early update.
        /// </summary>
        public void Unregister(IEarlyUpdate _update) => earlyUpdates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Registers an object on input update.
        /// </summary>
        public void Register(IInputUpdate _update) => inputUpdates.Add(_update);

        /// <summary>
        /// Unregisters an object from input update.
        /// </summary>
        public void Unregister(IInputUpdate _update) => inputUpdates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Registers an object on dynamic update.
        /// </summary>
        public void Register(IDynamicUpdate _update) => dynamicUpdates.Add(_update);

        /// <summary>
        /// Unregisters an object from dynamic update.
        /// </summary>
        public void Unregister(IDynamicUpdate _update) => dynamicUpdates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Registers an object on global update.
        /// </summary>
        public void Register(IUpdate _update) => updates.Add(_update);

        /// <summary>
        /// Unregisters an object from global update.
        /// </summary>
        public void Unregister(IUpdate _update) => updates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Registers an object on movables update.
        /// </summary>
        public void Register(IMovableUpdate _update) => movableUpdates.Add(_update);

        /// <summary>
        /// Unregisters an object from movables update.
        /// </summary>
        public void Unregister(IMovableUpdate _update) => movableUpdates.Remove(_update);

        // ------------------------------

        /// <summary>
        /// Registers an object on late update.
        /// </summary>
        public void Register(ILateUpdate _update) => lateUpdates.Add(_update);

        /// <summary>
        /// Unregisters an object from late update.
        /// </summary>
        public void Unregister(ILateUpdate _update) => lateUpdates.Remove(_update);
        #endregion

        #region Global Registration
        /// <summary>
        /// Registers an object on defined updates.
        /// </summary>
        /// <typeparam name="T">Object type to register.</typeparam>
        /// <param name="_object">Object to be registered on update(s).</param>
        /// <param name="_registration">Defined update registration (can use multiple).</param>
        public void Register<T>(T _object, UpdateRegistration _registration)
        {
            if ((_registration & UpdateRegistration.Early) != 0)
                Register((IEarlyUpdate)_object);

            if ((_registration & UpdateRegistration.Input) != 0)
                Register((IInputUpdate)_object);

            if ((_registration & UpdateRegistration.Update) != 0)
                Register((IUpdate)_object);

            if ((_registration & UpdateRegistration.Dynamic) != 0)
                Register((IDynamicUpdate)_object);

            if ((_registration & UpdateRegistration.Movable) != 0)
                Register((IMovableUpdate)_object);

            if ((_registration & UpdateRegistration.Late) != 0)
                Register((ILateUpdate)_object);
        }

        /// <summary>
        /// Unregisters an object from defined updates.
        /// </summary>
        /// <typeparam name="T">Object type to unregister.</typeparam>
        /// <param name="_object">Object to be unregistered from update(s).</param>
        /// <param name="_registration">Defined update unregistration (can use multiple).</param>
        public void Unregister<T>(T _object, UpdateRegistration _registration)
        {
            if ((_registration & UpdateRegistration.Early) != 0)
                Unregister((IEarlyUpdate)_object);

            if ((_registration & UpdateRegistration.Input) != 0)
                Unregister((IInputUpdate)_object);

            if ((_registration & UpdateRegistration.Update) != 0)
                Unregister((IUpdate)_object);

            if ((_registration & UpdateRegistration.Dynamic) != 0)
                Unregister((IDynamicUpdate)_object);

            if ((_registration & UpdateRegistration.Movable) != 0)
                Unregister((IMovableUpdate)_object);

            if ((_registration & UpdateRegistration.Late) != 0)
                Unregister((ILateUpdate)_object);
        }
        #endregion

        #region MonoBehaviour
        private void Update()
        {
            // Call all registered interfaces update.
            int _i;

            {
                IEarlyUpdate[] _updates = earlyUpdates.Array;
                for (_i = earlyUpdates.Count; _i-- > 0;)
                    _updates[_i].Update();
            }

            {
                IInputUpdate[] _updates = inputUpdates.Array;
                for (_i = inputUpdates.Count; _i-- > 0;)
                    _updates[_i].Update();
            }

            {
                IDynamicUpdate[] _updates = dynamicUpdates.Array;
                for (_i = dynamicUpdates.Count; _i-- > 0;)
                    _updates[_i].Update();
            }

            {
                IUpdate[] _updates = updates.Array;
                for (_i = updates.Count; _i-- > 0;)
                    _updates[_i].Update();
            }

            {
                IMovableUpdate[] _updates = movableUpdates.Array;
                for (_i = movableUpdates.Count; _i-- > 0;)
                    _updates[_i].Update();
            }

            {
                ILateUpdate[] _updates = lateUpdates.Array;
                for (_i = lateUpdates.Count; _i-- > 0;)
                    _updates[_i].Update();
            }
        }
        #endregion
    }
}

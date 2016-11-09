﻿namespace Framework
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    ///     This is the abstract base class for any object that provides property change notifications.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Public Events

        /// <summary>
        ///     Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///     Returns whether an exception is thrown, or if a Debug.Fail() is used
        ///     when an invalid property name is passed to the VerifyPropertyName method.
        ///     The default value is false, but subclasses used by unit tests might
        ///     override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Warns the developer if this object does not have
        ///     a public property with the specified name. This
        ///     method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // If you raise PropertyChanged and do not specify a property name,
            // all properties on the object are considered to be changed by the binding system.
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            foreach (var prop in this.GetType().GetTypeInfo().DeclaredProperties)
            {
                if (prop.Name == propertyName)
                {
                    return;
                }
            }
            var msg = "Invalid property name: " + propertyName;

            if (this.ThrowOnInvalidPropertyName)
            {
                throw new ArgumentException(msg);
            }
            Debug.Assert(false, msg);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
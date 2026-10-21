using CatoriUCLibrary.Objects;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CatoriUCLibrary.Views
{
    public class UserControlWithProperties : UserControl
    {
        private ControlProperties _controlProperties = new ControlProperties();
        private bool _applyingProperties;

        public virtual ControlProperties ControlProperties
        {
            get => _controlProperties;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _applyingProperties = true;
                try
                {
                    _controlProperties = value;
                    ApplyProperties(value);
                }
                finally
                {
                    _applyingProperties = false;
                }
                ControlPropertiesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? ControlPropertiesChanged;

        /// <summary>
        /// Called when ControlProperties is being applied; derived controls should override to map properties.
        /// </summary>
        protected virtual void ApplyProperties(ControlProperties properties) { }

        /// <summary>
        /// Called by derived controls when a dependency property changes to record the new value into ControlProperties.
        /// </summary>
        protected void OnConfigurationPropertyChanged(Action<ControlProperties> apply)
        {
            if (_applyingProperties) return;
            var props = ControlProperties ?? new ControlProperties();
            apply(props);
            _controlProperties = props;
            ControlPropertiesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

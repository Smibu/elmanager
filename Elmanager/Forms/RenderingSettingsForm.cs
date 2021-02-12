using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
    internal delegate void SettingChangedEventHandler(RenderingSettings settings);

    internal partial class RenderingSettingsForm : FormMod
    {
        private readonly RenderingSettings _settings;

        internal RenderingSettingsForm()
        {
            InitializeComponent();
            _settings = new RenderingSettings();
            SettingsGrid.SelectedObject = _settings;
        }

        internal RenderingSettingsForm(RenderingSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            SettingsGrid.SelectedObject = _settings;
        }

        internal event SettingChangedEventHandler Changed = delegate { };

        private void CloseSettings(object sender, EventArgs e)
        {
            Close();
        }

        private void SettingChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Changed(_settings.Clone());
        }
    }

    internal class CustomFileNameEditor : FileNameEditor
    {
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "Elasto Mania LGR files (*.lgr)|*.lgr";
            openFileDialog.CheckFileExists = true;
        }
    }
}
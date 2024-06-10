using System;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.UI;

namespace Elmanager.LevelEditor.Playing;

internal partial class PlaySettingsForm : FormMod
{
    private Button? _currButton;
    public PlaySettings Settings { get; }

    public PlaySettingsForm(PlaySettings settings)
    {
        InitializeComponent();
        Settings = new PlaySettings(settings);
        UpdateGui();
    }

    private void UpdateGui()
    {
        _currButton = null;
        dyingComboBox.SelectedIndex = (int)Settings.DyingBehavior;
        followDriverComboBox.SelectedIndex = (int)Settings.FollowDriverOption;
        gasButton.Text = Settings.Gas.ToString();
        brakeButton.Text = Settings.Brake.ToString();
        brakeAliasButton.Text = Settings.BrakeAlias.ToString();
        leftVoltButton.Text = Settings.LeftVolt.ToString();
        rightVoltButton.Text = Settings.RightVolt.ToString();
        aloVoltButton.Text = Settings.AloVolt.ToString();
        turnButton.Text = Settings.Turn.ToString();
        escAliasButton.Text = Settings.EscAlias.ToString();
        saveButton.Text = Settings.Save.ToString();
        loadButton.Text = Settings.Load.ToString();
        disableShortcutsCheckBox.Checked = Settings.DisableShortcuts;
        constantFpsCheckBox.Checked = Settings.ConstantFps;
        fpsTextBox.Text = Settings.PhysicsFps.ToString();
        toggleFullscreenCheckBox.Checked = Settings.ToggleFullscreen;
    }

    private void OkButtonClick(object sender, EventArgs e)
    {
        Settings.DyingBehavior = (DyingBehavior)dyingComboBox.SelectedIndex;
        Settings.FollowDriverOption = (FollowDriverOption)followDriverComboBox.SelectedIndex;
        Settings.DisableShortcuts = disableShortcutsCheckBox.Checked;
        Settings.ConstantFps = constantFpsCheckBox.Checked;
        Settings.ToggleFullscreen = toggleFullscreenCheckBox.Checked;
        var newFps = fpsTextBox.Value;
        const int minFps = 79;
        const int maxFps = 1000;
        if (newFps is < minFps or > maxFps)
        {
            UiUtils.ShowError($"Physics FPS must be between {minFps} and {maxFps}.");
            return;
        }
        Settings.PhysicsFps = newFps;
        DialogResult = DialogResult.OK;
    }

    private void CancelButtonClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }

    private void KeyButtonClick(object sender, EventArgs e)
    {
        WaitKeyChoose((sender as Button)!);
    }

    private void WaitKeyChoose(Button button)
    {
        UpdateGui();
        button.Text = "Press a key or ESC to cancel";
        _currButton = button;
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if (HandleKeyDown(keyData))
        {
            return true;
        }

        return base.ProcessDialogKey(keyData);
    }

    private bool HandleKeyDown(Keys key)
    {
        if (key == (Keys.Menu | Keys.Control | Keys.Alt))
        {
            key = KeyboardUtils.IsKeyDown(Keys.RMenu) ? Keys.RMenu : Keys.LMenu;
        }
        else if (key == (Keys.ShiftKey | Keys.Shift))
        {
            key = KeyboardUtils.IsKeyDown(Keys.RShiftKey) ? Keys.RShiftKey : Keys.LShiftKey;
        }
        else if (key == (Keys.Menu | Keys.Alt))
        {
            key = KeyboardUtils.IsKeyDown(Keys.RMenu) ? Keys.RMenu : Keys.LMenu;
        }
        else if (key == (Keys.ControlKey | Keys.Control))
        {
            // Alt Gr seems to cause 2 events with the first one coming here; ignore it.
            if (KeyboardUtils.IsKeyDown(Keys.RMenu))
            {
                return false;
            }

            key = KeyboardUtils.IsKeyDown(Keys.RControlKey) ? Keys.RControlKey : Keys.LControlKey;
        }

        if (_currButton is null)
        {
            return false;
        }

        if (key == Keys.Escape)
        {
            UpdateGui();
            return true;
        }

        _currButton.Text = key.ToString();
        if (_currButton == gasButton)
        {
            Settings.Gas = key;
        }
        else if (_currButton == brakeButton)
        {
            Settings.Brake = key;
        }
        else if (_currButton == brakeAliasButton)
        {
            Settings.BrakeAlias = key;
        }
        else if (_currButton == leftVoltButton)
        {
            Settings.LeftVolt = key;
        }
        else if (_currButton == rightVoltButton)
        {
            Settings.RightVolt = key;
        }
        else if (_currButton == aloVoltButton)
        {
            Settings.AloVolt = key;
        }
        else if (_currButton == turnButton)
        {
            Settings.Turn = key;
        }
        else if (_currButton == saveButton)
        {
            Settings.Save = key;
        }
        else if (_currButton == loadButton)
        {
            Settings.Load = key;
        }
        else if (_currButton == escAliasButton)
        {
            Settings.EscAlias = key;
        }

        UpdateGui();
        return true;
    }
}
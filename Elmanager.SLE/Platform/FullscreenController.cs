using System;
using Avalonia.Controls;

namespace Elmanager.SLE.Platform;

internal sealed class FullscreenController
{
    private readonly IFullscreenHandler _handler;

    public FullscreenController(
        Control owner,
        Action<bool> fullscreenChanged,
        Action fullscreenDismissed,
        Action<Exception> errorHandler)
    {
        _handler = OperatingSystem.IsBrowser()
            ? new BrowserFullscreenHandler(fullscreenChanged, fullscreenDismissed, errorHandler)
            : new DesktopFullscreenHandler(owner, fullscreenChanged);
    }

    public bool IsFullscreen => _handler.IsFullscreen;

    public void Toggle() => SetFullscreen(!IsFullscreen);

    public void SetFullscreen(bool fullscreen) => _handler.SetFullscreen(fullscreen);

    private interface IFullscreenHandler
    {
        bool IsFullscreen { get; }
        void SetFullscreen(bool fullscreen);
    }

    private sealed class DesktopFullscreenHandler(
        Control owner,
        Action<bool> fullscreenChanged) : IFullscreenHandler
    {
        private WindowState? _windowStateBeforeFullscreen;
        private Window Window => (Window)TopLevel.GetTopLevel(owner)!;

        public bool IsFullscreen { get; private set; }

        public void SetFullscreen(bool fullscreen)
        {
            var window = Window;
            if (fullscreen)
            {
                if (window.WindowState != WindowState.FullScreen)
                {
                    _windowStateBeforeFullscreen = window.WindowState;
                    window.WindowState = WindowState.FullScreen;
                }
            }
            else
            {
                if (window.WindowState == WindowState.FullScreen)
                {
                    window.WindowState = _windowStateBeforeFullscreen ?? WindowState.Normal;
                }

                _windowStateBeforeFullscreen = null;
            }

            IsFullscreen = fullscreen;
            fullscreenChanged(fullscreen);
        }
    }

    private sealed class BrowserFullscreenHandler : IFullscreenHandler
    {
        private readonly Action<Exception> _errorHandler;
        private readonly Action<bool> _fullscreenChanged;
        private readonly Action _fullscreenDismissed;
        private bool _exitRequested;

        public BrowserFullscreenHandler(
            Action<bool> fullscreenChanged,
            Action fullscreenDismissed,
            Action<Exception> errorHandler)
        {
            _fullscreenChanged = fullscreenChanged;
            _fullscreenDismissed = fullscreenDismissed;
            _errorHandler = errorHandler;
            BrowserInterop.SubscribeFullscreenChange(OnFullscreenChanged);
        }

        public bool IsFullscreen { get; private set; }

        public async void SetFullscreen(bool fullscreen)
        {
            if (fullscreen == IsFullscreen)
            {
                return;
            }

            _exitRequested = !fullscreen;
            try
            {
                await BrowserInterop.SetFullscreen(fullscreen);
            }
            catch (Exception ex)
            {
                if (!fullscreen)
                {
                    _exitRequested = false;
                }

                _errorHandler(ex);
            }
        }

        private void OnFullscreenChanged(bool fullscreen)
        {
            var dismissed = IsFullscreen && !fullscreen && !_exitRequested;
            IsFullscreen = fullscreen;
            if (!fullscreen)
            {
                _exitRequested = false;
            }

            _fullscreenChanged(fullscreen);
            if (dismissed)
            {
                _fullscreenDismissed();
            }
        }
    }
}

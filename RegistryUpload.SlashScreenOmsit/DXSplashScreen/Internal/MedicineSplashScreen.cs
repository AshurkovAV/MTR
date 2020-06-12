using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Omsit.UpdateSlashScreen.DXSplashScreen.Internal
{

    public class MedicineSplashScreen
{
    // Fields
    private static volatile Thread internalThreadCore = null;
    private static bool? isIndeterminateCore = null;
    private static volatile Window screenCore = null;
    public static readonly DependencyProperty SplashScreenTypeProperty = DependencyProperty.RegisterAttached("SplashScreenType", typeof(Type), typeof(MedicineSplashScreen), new PropertyMetadata(null, new PropertyChangedCallback(MedicineSplashScreen.SplashScreenTypePropertyChanged)));
    private static volatile AutoResetEvent syncEvent = new AutoResetEvent(false);

    // Methods
    public static void Close()
    {
        if (InternalThread == null)
        {
            throw new InvalidOperationException("Show splash screen before calling this method.");
        }
        SplashScreen.Dispatcher.BeginInvoke(new Action(CloseCore), new object[0]);
    }

    private static void CloseCore()
    {
        ((ISplashScreenEx) SplashScreen).CloseSplashScreen();
    }

    private static void CreateThread(Type type)
    {
        InternalThread = new Thread(new ParameterizedThreadStart(MedicineSplashScreen.InternalThreadEntryPoint));
        InternalThread.SetApartmentState(ApartmentState.STA);
        InternalThread.Start(type);
        SyncEvent.WaitOne();
    }

    public static Type GetSplashScreenType(DependencyObject obj)
    {
        return (Type) obj.GetValue(SplashScreenTypeProperty);
    }

    private static void InternalThreadEntryPoint(object type)
    {
        SplashScreen = (Window) Activator.CreateInstance((Type) type);
        SplashScreen.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SetProgressStateCore(true);
        SyncEvent.Set();
        SplashScreen.ShowDialog();
        SplashScreen = null;
        InternalThread = null;
    }

    public static void Progress(double value)
    {
        if (InternalThread == null)
        {
            throw new InvalidOperationException("Show splash screen before calling this method.");
        }
        if (SplashScreen != null)
        {
            SplashScreen.Dispatcher.BeginInvoke(new Action<double>(MedicineSplashScreen.ProgressCore), new object[] { value });
        }
    }

    public static void Progress(string value)
    {
        if (InternalThread == null)
        {
            throw new InvalidOperationException("Show splash screen before calling this method.");
        }
        if (SplashScreen != null)
        {
            SplashScreen.Dispatcher.BeginInvoke(new Action<string>(MedicineSplashScreen.ProgressCore), new object[] { value });
        }
    }

    private static void ProgressCore(double value)
    {
        ((ISplashScreenEx) SplashScreen).Progress(value);
        SetProgressStateCore(false);
    }

    private static void ProgressCore(string value)
    {
        ((ISplashScreenEx)SplashScreen).Progress(value);
        SetProgressStateCore(false);
    }

    private static void SetProgressStateCore(bool isIndeterminate)
    {
        if (isIndeterminate != isIndeterminateCore)
        {
            ((ISplashScreenEx) SplashScreen).SetProgressState(isIndeterminate);
            isIndeterminateCore = new bool?(isIndeterminate);
        }
    }

    public static void SetSplashScreenType(DependencyObject obj, Type value)
    {
        obj.SetValue(SplashScreenTypeProperty, value);
    }

    public static void Show<T>() where T: Window, ISplashScreenEx, new()
    {
        ShowCore(typeof(T));
    }

    private static void ShowCore(Type type)
    {
        if (InternalThread != null)
        {
            throw new InvalidOperationException("Splash screen has been displayed. Only one splash screen can be displayed simultaneously.");
        }
        CreateThread(type);
    }

    internal static void SplashScreenTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (!DesignerProperties.GetIsInDesignMode(obj))
        {
            Window window = obj as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Object to which SplashScreenType property is attached must be derived from Window class.");
            }
            Type newValue = e.NewValue as Type;
            if (newValue == null)
            {
                throw new InvalidCastException("Use 'x:Type' XAML markup extension to initialize this property.\nExample: dxc:DXSplashScreen.SplashScreenType=\"{x:Type local:SplashScreenWindow1}\"");
            }
            ShowCore(newValue);
            window.Dispatcher.BeginInvoke(new Action(Close), DispatcherPriority.Loaded, new object[0]);
            //window.Dispatcher.BeginInvoke(new Func<bool>(this.Activate), DispatcherPriority.Loaded, new object[0]);
        }
    }

    // Properties
    protected internal static Thread InternalThread
    {
        get
        {
            return internalThreadCore;
        }
        set
        {
            internalThreadCore = value;
        }
    }

    public static bool IsActive
    {
        get
        {
            return (InternalThread != null);
        }
    }

    protected internal static Window SplashScreen
    {
        get
        {
            return screenCore;
        }
        set
        {
            screenCore = value;
        }
    }

    private static AutoResetEvent SyncEvent
    {
        get
        {
            return syncEvent;
        }
    }
}

 

 

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Match3.Analyzer;
using Match3.Engine;
using Match3.Engine.Descriptions.Levels;
using Match3.Engine.Levels;
using Match3.Providers;

namespace Match3.Editor.Windows
{
  /// <summary>
  /// Логика взаимодействия для AnalyzeWindow.xaml
  /// </summary>
  public partial class AnalyzeWindow : Window
  {
    private CancellationTokenSource _cancellationTokenSource;

    public AnalyzeWindow()
    {
      InitializeComponent();
    }

    public async void Analyze(LevelDescription level, int energy)
    {
      if (DialogHost.IsOpen)
      {
        MessageBox.Show("In process");
        return;
      }

      DialogHost.IsOpen = true;

      Match3LevelStatistic result = null;

      TextBox.Clear();

      _cancellationTokenSource = new CancellationTokenSource();

      var task = Task.Run(() =>
      {
        var cancellation = new AnalyzerCancellation();
        using (_cancellationTokenSource.Token.Register(cancellation.Cancel))
        {
          var provider = new Match3ProviderFactory();
          var factory = new Match3Factory(provider.Create(AppSettings.Setting),
            EngineEnvironment.DefaultDebugServer);
          var analyzer = new Match3Analyzer(factory);

          var swaps = 0;
          var variants = 0;
          var success = 0;
          var fail = 0;

          result = analyzer.Analyze(level, cancellation, new ProgressHandler(Dispatcher)
          {
            IncreaseSwapsAction = () =>
            {
              ++swaps;
              ProgressSwapsLabel.Content = $"Swaps:\t\t{swaps}";
            },
            IncreaseVariantAction = () =>
            {
              ++variants;
              ProgressVariantsLabel.Content = $"Variant:\t\t{variants}";
            },
            IncreaseSuccessAction = () =>
            {
              ++success;
              float percent = 0;
              if (fail == 0)
              {
                if (success == 0) percent = 0f;
                else percent = 1f;
              }
              else
              {
                percent = (success / (float)fail);
              }
              ProgressBar.Value = percent;
              ProgressSuccessLabel.Content = $"Success:\t\t{success} [{(percent * 100f):F}%]";
              ProgressFailLabel.Content = $"Fail:\t\t{fail} [{((1 - percent) * 100f):F}%]";
            },
            IncreaseFailAction = () =>
            {
              ++fail;
              float percent = 0;
              if (fail == 0)
              {
                if (success == 0) percent = 0f;
                else percent = 1f;
              }
              else
              {
                percent = (success / (float)fail);
              }
              ProgressBar.Value = percent;
              ProgressSuccessLabel.Content = $"Success:\t\t{success} [{(percent * 100f):F}%]";
              ProgressFailLabel.Content = $"Fail:\t\t{fail} [{((1 - percent) * 100f):F}%]";
            }
          });
        }
      }, _cancellationTokenSource.Token);

      await task;

      if (task.IsCanceled) return;

      if (result != null)
      {
        var fields = result.GetType().GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public);
        var properties = result.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
        foreach (var fieldInfo in fields)
        {
          try
          {
            TextBox.AppendText($"{fieldInfo.Name}: {fieldInfo.GetValue(result)}" + Environment.NewLine);
          }
          catch (Exception e)
          {
            TextBox.AppendText($"{fieldInfo.Name}: {e.Message}" + Environment.NewLine);
          }
        }
        foreach (var propertyInfo in properties)
        {
          try
          {
            TextBox.AppendText($"{propertyInfo.Name}: {propertyInfo.GetValue(result, null)}" + Environment.NewLine);
          }
          catch (Exception e)
          {
            TextBox.AppendText($"{propertyInfo.Name}: {e.Message}" + Environment.NewLine);
          }
        }
      }

      DialogHost.IsOpen = false;
    }

    private class ProgressHandler : IProgressHandler
    {
      private readonly Dispatcher _dispatcher;

      public Action IncreaseSwapsAction;
      public Action IncreaseVariantAction;
      public Action IncreaseSuccessAction;
      public Action IncreaseFailAction;

      public ProgressHandler(Dispatcher dispatcher)
      {
        _dispatcher = dispatcher;
      }

      public void IncreaseSwaps()
      {
        if (IncreaseSwapsAction != null)
        {
          _dispatcher.InvokeAsync(IncreaseSwapsAction, DispatcherPriority.Background).Task.ConfigureAwait(false);
        }
      }

      public void IncreaseVariant()
      {
        if (IncreaseVariantAction != null)
        {
          _dispatcher.InvokeAsync(IncreaseVariantAction, DispatcherPriority.Background).Task.ConfigureAwait(false);
        }
      }

      public void IncreaseSuccess()
      {
        if (IncreaseSuccessAction != null)
        {
          _dispatcher.InvokeAsync(IncreaseSuccessAction, DispatcherPriority.Background).Task.ConfigureAwait(false);
        }
      }

      public void IncreaseFail()
      {
        if (IncreaseFailAction != null)
        {
          _dispatcher.InvokeAsync(IncreaseFailAction, DispatcherPriority.Background).Task.ConfigureAwait(false);
        }
      }
    }

    private void AnalyzeWindow_OnClosed(object sender, EventArgs e)
    {
      _cancellationTokenSource.Cancel();
    }
  }
}

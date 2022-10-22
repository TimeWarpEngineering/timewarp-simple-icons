namespace TimeWarp.Simple.Icons;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading.Tasks;

public partial class SimpleIconBase : IComponent
{
  private RenderFragment RenderFragment;
  private RenderHandle RenderHandle;

  [Parameter(CaptureUnmatchedValues = true)]
  public IReadOnlyDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

  public SimpleIconBase()
  {
    RenderFragment = delegate (RenderTreeBuilder builder)
    {
      BuildRenderTree(builder);
    };
  }
 
  void IComponent.Attach(RenderHandle renderHandle) => RenderHandle = renderHandle;

  public Task SetParametersAsync(ParameterView parameters)
  {
    parameters.SetParameterProperties(this);
    RenderHandle.Render(RenderFragment);
    return Task.CompletedTask;
  }

  protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }
}

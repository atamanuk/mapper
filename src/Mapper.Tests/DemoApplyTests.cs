using Mapper.Demo.Mapping;
using Mapper.Demo.Models;
using MapperDemo.Pages;
using Newtonsoft.Json;
using Xunit;

namespace Mapper.Tests;

public sealed class DemoApplyTests
{
    [Fact]
    public void OnPostMap_Shows_Mapped_Target_Patch()
    {
        var model = CreateModel();

        model.OnPostMap();

        Assert.NotNull(model.ResultJson);
        Assert.Contains("\"path\": \"/DisplayName\"", model.ResultJson, StringComparison.Ordinal);
        Assert.Contains("\"path\": \"/Age\"", model.ResultJson, StringComparison.Ordinal);
        Assert.Null(model.MapErrorMessage);
        Assert.Null(model.ApplyResultJson);
    }

    [Fact]
    public void OnPostApply_Rebuilds_Mapped_Patch_And_Applies_It_To_A_Fresh_Target()
    {
        var model = CreateModel();

        model.OnPostApply();

        Assert.NotNull(model.ResultJson);
        Assert.Null(model.MapErrorMessage);
        Assert.Null(model.ApplyErrorMessage);

        var appliedTarget = JsonConvert.DeserializeObject<Target>(model.ApplyResultJson!);

        Assert.NotNull(appliedTarget);
        Assert.Equal("Alice", appliedTarget.DisplayName);
        Assert.Equal(42, appliedTarget.Age);
    }

    [Fact]
    public void OnPostApply_Shows_Map_Error_And_No_Fake_Apply_Result_When_Input_Is_Invalid()
    {
        var model = CreateModel();
        model.PatchInput = "[";

        model.OnPostApply();

        Assert.NotNull(model.MapErrorMessage);
        Assert.Null(model.ApplyErrorMessage);
        Assert.Null(model.ResultJson);
        Assert.Null(model.ApplyResultJson);
    }

    [Fact]
    public void OnPostApply_Shows_Apply_Error_When_Mapped_Value_Cannot_Be_Applied()
    {
        var model = CreateModel();
        model.PatchInput = """
            [
              { "op": "test", "path": "/Age", "value": 42 }
            ]
            """;

        model.OnPostApply();

        Assert.NotNull(model.ResultJson);
        Assert.Null(model.MapErrorMessage);
        Assert.NotNull(model.ApplyErrorMessage);
        Assert.Contains("Patch apply failed", model.ApplyErrorMessage, StringComparison.Ordinal);
        Assert.Null(model.ApplyResultJson);
    }

    private static IndexModel CreateModel() =>
        new(new global::Mapper.Mapper(new DemoProfile()));
}

using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mapper.Demo.Models;
using Mapper.Demo.Services;
using Newtonsoft.Json;
using PatchMapper = Mapper.Mapper;

namespace MapperDemo.Pages;

public sealed class IndexModel : PageModel
{
    private const string DefaultPatch = """
        [
          { "op": "replace", "path": "/Name", "value": "Alice" },
          { "op": "replace", "path": "/Age", "value": 42 }
        ]
        """;
    private static readonly JsonSerializerSettings ResultSerializerSettings = new()
    {
        Formatting = Formatting.Indented
    };

    private readonly PatchMapper _mapper;

    public IndexModel(PatchMapper mapper)
    {
        _mapper = mapper;
    }

    [BindProperty]
    public string PatchInput { get; set; } = DefaultPatch;

    public string? ResultJson { get; private set; }

    public string? ApplyResultJson { get; private set; }

    public string? MapErrorMessage { get; private set; }

    public string? ApplyErrorMessage { get; private set; }

    public IReadOnlyList<string> SourceFields => DemoMetadata.Describe<Source>();

    public IReadOnlyList<string> TargetFields => DemoMetadata.Describe<Target>();

    public string InitialTargetJson => SerializeObject(CreateInitialTarget());

    public void OnGet()
    {
    }

    public void OnPostMap()
    {
        if (!TryBuildMappedPatch(out var targetPatch))
        {
            return;
        }

        ResultJson = PatchJsonSerializer.Serialize(targetPatch);
    }

    public void OnPostApply()
    {
        if (!TryBuildMappedPatch(out var targetPatch))
        {
            return;
        }

        ResultJson = PatchJsonSerializer.Serialize(targetPatch);

        var target = CreateInitialTarget();

        try
        {
            targetPatch.ApplyTo(
                target,
                error =>
                {
                    var key = error.Operation?.path ?? string.Empty;
                    ModelState.AddModelError(key, error.ErrorMessage);
                });

            if (!ModelState.IsValid)
            {
                ApplyErrorMessage = BuildApplyErrorMessage();
                return;
            }

            ApplyResultJson = SerializeObject(target);
        }
        catch (InvalidOperationException exception)
        {
            ApplyErrorMessage = $"Patch apply failed: {exception.Message}";
        }
    }

    private bool TryBuildMappedPatch(out JsonPatchDocument<Target> targetPatch)
    {
        PatchInput ??= string.Empty;

        if (string.IsNullOrWhiteSpace(PatchInput))
        {
            targetPatch = new JsonPatchDocument<Target>();
            MapErrorMessage = "Json Patch is required.";
            return false;
        }

        try
        {
            var sourcePatch = PatchJsonSerializer.Deserialize<Source>(PatchInput);
            targetPatch = _mapper.Map<Source, Target>(sourcePatch);
            return true;
        }
        catch (JsonException exception)
        {
            targetPatch = new JsonPatchDocument<Target>();
            MapErrorMessage = $"Invalid JSON Patch: {exception.Message}";
            return false;
        }
        catch (InvalidOperationException exception)
        {
            targetPatch = new JsonPatchDocument<Target>();
            MapErrorMessage = $"Mapping failed: {exception.Message}";
            return false;
        }
        catch (NotSupportedException exception)
        {
            targetPatch = new JsonPatchDocument<Target>();
            MapErrorMessage = $"Mapping failed: {exception.Message}";
            return false;
        }
    }

    private string BuildApplyErrorMessage()
    {
        var messages = ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(
                entry => entry.Value!.Errors.Select(
                    error => string.IsNullOrWhiteSpace(entry.Key)
                        ? error.ErrorMessage
                        : $"{entry.Key}: {error.ErrorMessage}"))
            .ToArray();

        return messages.Length == 0
            ? "Patch apply failed."
            : $"Patch apply failed: {string.Join("; ", messages)}";
    }

    private static Target CreateInitialTarget() =>
        new()
        {
            DisplayName = "Before patch",
            Age = 7
        };

    private static string SerializeObject<TModel>(TModel model) =>
        JsonConvert.SerializeObject(model, ResultSerializerSettings);
}

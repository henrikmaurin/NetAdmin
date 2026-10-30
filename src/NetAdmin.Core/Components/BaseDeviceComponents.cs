using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NetAdmin.Core.Models;

namespace NetAdmin.Core.Components
{
    // Common base for simple components that work with a HardwareDevice
    public abstract class DeviceComponentBase : ComponentBase
    {
        [Parameter, EditorRequired]
        public HardwareDevice Device { get; set; } = default!;

        [Parameter]
        public EventCallback<HardwareDevice> DeviceChanged { get; set; }

        [Parameter]
        public bool Compact { get; set; }

        protected Task NotifyDeviceChanged(HardwareDevice device)
        {
            Device = device;
            return DeviceChanged.InvokeAsync(device);
        }

        // Render helper for anchor-styled action buttons. Compact style toggles padding and font-size.
        // variant: CSS variant name (primary, blue, accent, green, shelly etc.)
        protected RenderFragment AnchorAction(string href, string title, string content, string variant = "primary") => builder =>
        {
            // Render an anchor styled via CSS utility classes. Variant determines the color scheme.
            builder.OpenElement(0, "a");
            builder.AddAttribute(1, "href", href);
            builder.AddAttribute(2, "title", title);
            builder.AddAttribute(3, "class", Compact ? $"action-anchor action-anchor--{variant} compact" : $"action-anchor action-anchor--{variant}");
            builder.AddContent(4, content);
            builder.CloseElement();
        };

        // Render helper for clickable buttons with an async handler.
        // variant: CSS variant name (primary, blue, accent, green, shelly etc.)
        protected RenderFragment ButtonAction(Func<Task> onClick, string content, string variant = "primary") => builder =>
        {
            builder.OpenElement(10, "button");
            builder.AddAttribute(11, "class", Compact ? $"action-button action-button--{variant} compact" : $"action-button action-button--{variant}");
            builder.AddAttribute(12, "onclick", EventCallback.Factory.Create(this, onClick));
            builder.AddContent(13, content);
            builder.CloseElement();
        };

        // Small info/status box used across header/editor components.
        protected RenderFragment InfoBox(string? message, bool success = true) => builder =>
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            builder.OpenElement(20, "div");
            builder.AddAttribute(21, "class", success ? "info-box info-box--success" : "info-box info-box--error");
            builder.AddContent(22, message);
            builder.CloseElement();
        };

        // Small tag / badge helper used for protocol lists etc.
        protected RenderFragment TagBadge(string text, string bg = "#f3e8ff", string fg = "#6b21a8") => builder =>
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            builder.OpenElement(30, "span");
            builder.AddAttribute(31, "class", "tag-badge");
            builder.AddContent(32, text);
            builder.CloseElement();
        };
    }

    // Typed variant for editors that prefer a concrete device type
    public abstract class TypedDeviceComponentBase<TDevice> : ComponentBase where TDevice : HardwareDevice
    {
        [Parameter, EditorRequired]
        public TDevice Device { get; set; } = default!;

        [Parameter]
        public EventCallback<TDevice> DeviceChanged { get; set; }

        [Parameter]
        public bool Compact { get; set; }

        protected Task NotifyDeviceChanged(TDevice device)
        {
            Device = device;
            return DeviceChanged.InvokeAsync(device);
        }

        // Mirror Anchor/Button helpers on typed base as well
        protected RenderFragment AnchorAction(string href, string title, string content, string variant = "primary") => builder =>
        {
            builder.OpenElement(0, "a");
            builder.AddAttribute(1, "href", href);
            builder.AddAttribute(2, "title", title);
            builder.AddAttribute(3, "class", Compact ? $"action-anchor action-anchor--{variant} compact" : $"action-anchor action-anchor--{variant}");
            builder.AddContent(4, content);
            builder.CloseElement();
        };

        protected RenderFragment ButtonAction(Func<Task> onClick, string content, string variant = "primary") => builder =>
        {
            builder.OpenElement(10, "button");
            builder.AddAttribute(11, "class", Compact ? $"action-button action-button--{variant} compact" : $"action-button action-button--{variant}");
            builder.AddAttribute(12, "onclick", EventCallback.Factory.Create(this, onClick));
            builder.AddContent(13, content);
            builder.CloseElement();
        };

        // Mirror common helpers so typed bases can also use InfoBox/TagBadge
        protected RenderFragment InfoBox(string? message, bool success = true) => builder =>
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            builder.OpenElement(40, "div");
            builder.AddAttribute(41, "class", success ? "info-box info-box--success" : "info-box info-box--error");
            builder.AddContent(42, message);
            builder.CloseElement();
        };

        protected RenderFragment TagBadge(string text, string bg = "#f3e8ff", string fg = "#6b21a8") => builder =>
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            builder.OpenElement(50, "span");
            builder.AddAttribute(51, "class", "tag-badge");
            builder.AddContent(52, text);
            builder.CloseElement();
        };
    }

    /// <summary>
    /// Shared base class for catalog-backed device editors to eliminate repeated vendor/model selector logic.
    /// </summary>
    public abstract class CatalogDeviceEditorBase<TDevice> : TypedDeviceComponentBase<TDevice> where TDevice : HardwareDevice
    {
        [Inject]
        protected ProductCatalogRegistry Catalog { get; set; } = default!;

        protected abstract HardwareCategory TargetCategory { get; }

        protected bool CustomVendorMode { get; set; }
        protected bool CustomModelMode { get; set; }

        protected IReadOnlyList<string> GetCatalogVendors() => Catalog.GetVendorsByCategory(TargetCategory);

        protected string? GetMatchedVendor(string? currentVendor) =>
            GetCatalogVendors().FirstOrDefault(v => v.Equals(currentVendor, StringComparison.OrdinalIgnoreCase));

        protected string GetVendorSelectValue(string? currentVendor) =>
            CustomVendorMode || GetMatchedVendor(currentVendor) is null ? "__custom__" : GetMatchedVendor(currentVendor)!;

        protected IReadOnlyList<HardwareProductDefinition> GetVendorModels(string? currentVendor)
        {
            var matched = GetMatchedVendor(currentVendor);
            return matched is not null ? Catalog.GetByCategoryAndVendor(TargetCategory, matched) : [];
        }

        protected HardwareProductDefinition? GetSelectedProduct(string? currentVendor, string? currentModel) =>
            !CustomModelMode ? GetVendorModels(currentVendor).FirstOrDefault(p => p.MatchesCodeOrModel(currentModel)) : null;

        protected string GetModelSelectValue(string? currentVendor, string? currentModel) =>
            GetSelectedProduct(currentVendor, currentModel)?.ProductId ?? "__custom__";

        protected virtual TDevice ApplyVendor(TDevice current, string vendor) => (TDevice)(current with { Vendor = vendor });
        protected virtual TDevice ApplyModel(TDevice current, string model) => (TDevice)(current with { Model = model });
        protected virtual TDevice ApplyCatalogProduct(TDevice current, HardwareProductDefinition product) => (TDevice)product.ApplyToDevice(current);

        protected Task OnVendorSelectedAsync(string? vendor)
        {
            if (string.IsNullOrWhiteSpace(vendor) || vendor == "__custom__")
            {
                CustomVendorMode = true;
                CustomModelMode = true;
                return Task.CompletedTask;
            }

            CustomVendorMode = false;
            CustomModelMode = false;

            var firstModel = Catalog.GetByCategoryAndVendor(TargetCategory, vendor).FirstOrDefault();
            if (firstModel is not null)
            {
                return OnCatalogProductSelectedAsync(firstModel.ProductId);
            }

            return NotifyDeviceChanged(ApplyVendor(Device, vendor));
        }

        protected Task OnCatalogProductSelectedAsync(string? productId)
        {
            if (string.IsNullOrWhiteSpace(productId) || productId == "__custom__")
            {
                CustomModelMode = true;
                return Task.CompletedTask;
            }

            CustomModelMode = false;
            var product = Catalog.GetById(productId);
            if (product is null)
                return Task.CompletedTask;

            return NotifyDeviceChanged(ApplyCatalogProduct(Device, product));
        }

        protected Task OnCustomVendorInputAsync(string vendor)
        {
            CustomVendorMode = !GetCatalogVendors().Any(v => v.Equals(vendor, StringComparison.OrdinalIgnoreCase));
            return NotifyDeviceChanged(ApplyVendor(Device, vendor));
        }

        protected Task OnCustomModelInputAsync(string model)
        {
            CustomModelMode = Catalog.FindMatch(Device.Vendor, model) is null;
            return NotifyDeviceChanged(ApplyModel(Device, model));
        }
    }
}

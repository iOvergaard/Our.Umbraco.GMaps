using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Our.Umbraco.GMaps.Core.Models;
using Our.Umbraco.GMaps.Core.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Our.Umbraco.GMaps.Core.Models.Configuration;

namespace Our.Umbraco.GMaps.Core.PropertyValueConverter
{
    public class SingleMapPropertyValueConverter : PropertyValueConverterBase
    {
        private GoogleMaps googleMapsConfig;

        public SingleMapPropertyValueConverter(IOptionsMonitor<GoogleMaps> googleMapsConfig)
        {
            this.googleMapsConfig = googleMapsConfig.CurrentValue;
            googleMapsConfig.OnChange(config => this.googleMapsConfig = config);
        }
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.Equals(Constants.MapPropertyAlias);

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(Map);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Element;

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            Map model = null;
            if (inter != null)
            {
                var jsonString = inter.ToString();

                // Handle pre v2.0.0 data (Removes the prefix 'google.maps.maptypeid.')
                jsonString = jsonString.Replace("google.maps.maptypeid.", string.Empty, StringComparison.InvariantCultureIgnoreCase);
                model = JsonSerializer.Deserialize<Map>(jsonString);
            }

            if (model != null)
            {
                model.MapConfig.ApiKey = googleMapsConfig.ApiKey;

                // Get API key and mapStyle from configuration
                var config = propertyType.DataType.ConfigurationAs<Dictionary<string, object>>();

                if (config != null)
                {
                    if (config.TryGetValue("apikey", out var apiKey) && apiKey != null)
                    {
                        model.MapConfig.ApiKey = apiKey.ToString();
                    }

                    if (config.TryGetValue("mapstyle", out var mapStyle) && mapStyle != null)
                    {
                        var style = JsonSerializer.Deserialize<MapStyle>(mapStyle.ToString());
                        model.MapConfig.Style = style?.Selectedstyle?.Json;
                    }
                }
            }

            return model;
        }
    }
}

using Tech.Aquisitions.Customers.Infrascructure.FeatureManager.Interfaces;

namespace Tech.Aquisitions.Customers.Infrascructure.FeatureManager;

public sealed class FeatureManagement : IFeatureManagement
{
    private readonly IDictionary<string, bool> _features;

    public FeatureManagement(IDictionary<string, bool> features)
    {
        _features = features;
    }

    public bool IsFeatureAvailable(string featureName)
    {
        if (_features.ContainsKey(featureName))
            return _features[featureName];

        return false;
    }
}

namespace UniAgile.Game
{
    public struct FeatureModel
    {
        public string FeatureId;
        public string ActiveFeatureVariantId;
        public bool   IsActive => !string.IsNullOrEmpty(ActiveFeatureVariantId);
    }
}
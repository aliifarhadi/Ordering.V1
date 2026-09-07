namespace AeroTech.Ordering.Domain._Shared.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum |
        AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method |
        AttributeTargets.Field | AttributeTargets.Constructor,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class SpecRefAttribute : Attribute
    {
        public SpecRefAttribute(string reference) => Reference = reference;

        public string Reference { get; }
    }
}

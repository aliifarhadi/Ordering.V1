namespace AeroTech.Ordering.Domain._Shared.Attributes
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum |
        AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method |
        AttributeTargets.Field | AttributeTargets.Constructor,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class BlockedAttribute : Attribute
    {
        public BlockedAttribute(string identifier, string what, string blockedOn)
        {
            Identifier = identifier;
            What = what;
            BlockedOn = blockedOn;
        }

        public string Identifier { get; }

        public string What { get; }

        public string BlockedOn { get; }
    }
}
